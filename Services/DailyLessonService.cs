using System;
using System.Linq;
using System.Data.Entity;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// توليد ومزامنة الحصص اليومية من الجدول المعتمد لكل تاريخ.
    /// مستخدمة من صفحة التوليد الجماعية وصفحة التسجيل اليومية (المحطة الموحدة).
    /// </summary>
    public class DailyLessonService
    {
        private readonly SchoolDbContext db;

        public DailyLessonService(SchoolDbContext context)
        {
            db = context;
        }

        public class GenerationResult
        {
            public bool IsWeekend { get; set; }
            public int ScheduledEntries { get; set; }
            public int Added { get; set; }
            public int Updated { get; set; }
            public int SkippedWithAttendance { get; set; }
            public int SkippedUnchanged { get; set; }
        }


        /// <summary>
        /// توليد/مزامنة جميع الحصص في نطاق زمني، مع تجاوز الجمعة والسبت.
        /// تستخدم عند فتح الفترة المالية أو لإصلاح فترة تم فتحها سابقاً.
        /// </summary>
        public GenerationRangeResult GenerateForRange(DateTime startDate, DateTime endDate)
        {
            var result = new GenerationRangeResult();
            startDate = startDate.Date;
            endDate = endDate.Date;

            if (endDate < startDate)
                return result;

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var day = GenerateForDate(date);
                result.DaysProcessed++;
                result.ScheduledEntries += day.ScheduledEntries;
                result.Added += day.Added;
                result.Updated += day.Updated;
                result.SkippedWithAttendance += day.SkippedWithAttendance;
                result.SkippedUnchanged += day.SkippedUnchanged;
            }

            return result;
        }

        public class GenerationRangeResult
        {
            public int DaysProcessed { get; set; }
            public int ScheduledEntries { get; set; }
            public int Added { get; set; }
            public int Updated { get; set; }
            public int SkippedWithAttendance { get; set; }
            public int SkippedUnchanged { get; set; }
        }

        public GenerationResult GenerateForDate(DateTime date)
        {
            var result = new GenerationResult();

            if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
            {
                result.IsWeekend = true;
                return result;
            }

            WeekDay day = (WeekDay)((int)date.DayOfWeek + 2);

            var entries = db.TimetableEntries
                .Where(t => t.DayOfWeek == day && t.Status == LessonStatus.Approved)
                .ToList();
            result.ScheduledEntries = entries.Count;

            var periods = db.LessonPeriods
                .Where(p => p.IsActive)
                .OrderBy(p => p.PeriodNumber)
                .ToList();

            // بعض قواعد البيانات القديمة قد تحتوي على وقت داخل حقل Date؛
            // المقارنة بالتاريخ فقط تمنع إنشاء حصص يومية مكررة.
            var existingLessons = db.DailyLessons
                .Where(d => DbFunctions.TruncateTime(d.Date) == date.Date)
                .ToList();

            foreach (var entry in entries)
            {
                // أولاً نبحث عن الربط الصحيح بسجل الجدول.
                var existing = existingLessons.FirstOrDefault(d =>
                    d.ScheduleEntryId.HasValue && d.ScheduleEntryId.Value == entry.Id);

                // ترحيل آمن للحصص القديمة التي أُنشئت قبل إضافة ScheduleEntryId.
                // لا نغيّر بيانات الحضور/الاستحقاق؛ نربط الحصة اليومية بسجل الجدول
                // فقط عندما تتطابق الهوية الفعلية: التاريخ + الحصة + الصف + المعلم + المادة.
                if (existing == null)
                {
                    existing = existingLessons.FirstOrDefault(d =>
                        !d.ScheduleEntryId.HasValue &&
                        d.PeriodNumber == entry.PeriodNumber &&
                        d.ClassRoomId == entry.ClassRoomId &&
                        d.TeacherId == entry.TeacherId &&
                        d.SubjectId == entry.SubjectId);

                    if (existing != null)
                    {
                        existing.ScheduleEntryId = entry.Id;
                        result.Updated++;
                    }
                }

                var period = periods.FirstOrDefault(p => p.PeriodNumber == entry.PeriodNumber);
                TimeSpan startTime;
                TimeSpan endTime;
                int duration;

                // مصدر وقت الحصة: وقت الجدول المعتمد أولاً، ثم تعريف أوقات الحصص.
                // لا نستخدم رقم الحصة كساعة زمنية؛ فهذا كان ينتج أوقاتاً خاطئة عند
                // غياب LessonPeriod.
                if (entry.StartTime.HasValue && entry.EndTime.HasValue)
                {
                    startTime = entry.StartTime.Value;
                    endTime = entry.EndTime.Value;
                    duration = entry.DurationMinutes > 0
                        ? entry.DurationMinutes
                        : (int)Math.Max(1, (endTime - startTime).TotalMinutes);
                }
                else if (period != null)
                {
                    startTime = period.StartTime;
                    endTime = period.EndTime;
                    duration = period.DurationMinutes;
                }
                else
                {
                    // حالة طوارئ فقط عند وجود سجل جدول ناقص التوقيت وتعريف LessonPeriods مفقود.
                    // نستخدم بداية ثابتة آمنة بدلاً من تحويل رقم الحصة إلى ساعة.
                    startTime = TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(Math.Max(0, entry.PeriodNumber - 1) * 45));
                    duration = entry.DurationMinutes > 0 ? entry.DurationMinutes : 45;
                    endTime = startTime.Add(TimeSpan.FromMinutes(duration));
                }

                if (existing == null)
                {
                    var lesson = new DailyLesson
                    {
                        Date = date,
                        PeriodNumber = entry.PeriodNumber,
                        StartTime = startTime,
                        EndTime = endTime,
                        DurationMinutes = duration,
                        TeacherId = entry.TeacherId,
                        SubjectId = entry.SubjectId,
                        ClassRoomId = entry.ClassRoomId,
                        ScheduleEntryId = entry.Id,
                        Status = LessonStatus.Approved
                    };

                    db.DailyLessons.Add(lesson);
                    existingLessons.Add(lesson);
                    result.Added++;
                    continue;
                }

                // إذا تم تسجيل حضور لهذه الحصة تعتبر بياناتها تاريخية ولا نعيد
                // تشكيلها عند تغيير أوقات الحصص حتى لا نغيّر أساس الاستحقاق المالي.
                bool hasAttendance = db.AttendanceRecords.Any(r => r.DailyLessonId == existing.Id);
                if (hasAttendance)
                {
                    result.SkippedWithAttendance++;
                    continue;
                }

                bool changed = existing.StartTime != startTime
                    || existing.EndTime != endTime
                    || existing.DurationMinutes != duration
                    || existing.TeacherId != entry.TeacherId
                    || existing.SubjectId != entry.SubjectId
                    || existing.ClassRoomId != entry.ClassRoomId
                    || existing.ScheduleEntryId != entry.Id
                    || existing.Status != LessonStatus.Approved;

                if (!changed)
                {
                    result.SkippedUnchanged++;
                    continue;
                }

                existing.StartTime = startTime;
                existing.EndTime = endTime;
                existing.DurationMinutes = duration;
                existing.TeacherId = entry.TeacherId;
                existing.SubjectId = entry.SubjectId;
                existing.ClassRoomId = entry.ClassRoomId;
                existing.ScheduleEntryId = entry.Id;
                existing.Status = LessonStatus.Approved;
                result.Updated++;
            }

            db.SaveChanges();
            return result;
        }
    }
}