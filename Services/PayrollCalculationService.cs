using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// محرك الرواتب المركزي.
    ///
    /// أنواع التعاقد المدعومة في الحساب المالي:
    /// 1) Permanent: راتب شهري ثابت.
    /// 2) PerLesson: أجر حسب الحصص المنفذة فعلياً، مع حفظ تفصيل كل حصة.
    /// 3) PermanentSupervisor: راتب شهري ثابت للمشرف الدائم.
    /// </summary>
    public class PayrollCalculationService
    {
        private readonly SchoolDbContext db;

        /// <summary>
        /// ⭐ Cache لتخزين أسعار الحصص لكل (معلم + مادة + تاريخ) لتفادي N+1 queries.
        /// </summary>
        private Dictionary<string, decimal> _rateCache;

        /// <summary>
        /// ⭐ Cache لتخزين الأسعار الافتراضية لكل معلم (LessonRate).
        /// </summary>
        private Dictionary<int, decimal> _teacherDefaultRateCache;

        public PayrollCalculationService(SchoolDbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            db = context;
            _rateCache = new Dictionary<string, decimal>();
            _teacherDefaultRateCache = new Dictionary<int, decimal>();
        }

        public class CalculationResult
        {
            public int TeachersProcessed { get; set; }
            public int LessonsCounted { get; set; }
            public decimal TotalEarned { get; set; }
            public decimal TotalDeducted { get; set; }
            public bool InvalidPeriod { get; set; }
            public double ElapsedSeconds { get; set; }
        }

        public decimal RateForLesson(decimal lessonRate, AttendanceStatus status, int actualMinutes, int durationMinutes)
        {
            if (lessonRate <= 0m || durationMinutes <= 0) return 0m;

            bool isAttended =
                status == AttendanceStatus.PresentFull ||
                status == AttendanceStatus.PresentPartial ||
                status == AttendanceStatus.SubstitutionFull ||
                status == AttendanceStatus.SubstitutionPartial;

            if (!isAttended) return 0m;

            int actual = Math.Max(0, Math.Min(actualMinutes, durationMinutes));
            if (actual >= durationMinutes) return lessonRate;

            return decimal.Round(lessonRate * actual / (decimal)durationMinutes, 2);
        }

        /// <summary>
        /// لا يوجد خصم إضافي لأي حالة، لأن عدم الاحتساب في RateForLesson يكفي.
        /// خصم الغياب على معلمي الحصة يساوي صفراً دائماً؛ الخصومات الحقيقية
        /// تأتي فقط من البنود اليدوية (PayrollItems).
        /// </summary>
        public CalculationResult CalculatePeriod(int payrollPeriodId, string userName)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = new CalculationResult();

            // إعادة تصفير الـ Cache لكل عملية حساب
            _rateCache.Clear();
            _teacherDefaultRateCache.Clear();

            var period = db.PayrollPeriods.Find(payrollPeriodId);
            if (period == null)
            {
                result.InvalidPeriod = true;
                return result;
            }

            if (period.Status == PayrollPeriodStatus.Closed)
                throw new InvalidOperationException("لا يمكن إعادة حساب فترة رواتب مغلقة.");

            // التحقق من جاهزية الحضور (لمعلمي الحصة)
            var attendanceValidation = new TeacherAttendanceFinancialService(db)
                .ValidatePeriod(payrollPeriodId);

            if (!attendanceValidation.IsReady)
            {
                throw new InvalidOperationException(attendanceValidation.Summary +
                    " يجب معالجة النواقص والتعارضات الخاصة بمعلمي الحصة ثم إعادة المحاولة.");
            }

            // ⭐ معاملة واحدة ذرية: كل التحديثات تُحفظ معاً أو لا شيء يُحفظ
            // (يمنع الاحتساب الجزئي عند تعطل الحساب في منتصفه).
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    // ⭐ جلب كل البيانات دفعة واحدة (تحسين كبير للأداء)
            var existingPayrollTeacherIds = db.TeacherPayrolls
                .Where(p => p.PayrollPeriodId == payrollPeriodId)
                .Select(p => p.TeacherId)
                .ToList();

            var records = db.AttendanceRecords
                .Include(r => r.DailyLesson)
                .Where(r => r.Date >= period.StartDate && r.Date <= period.EndDate)
                .Where(r => r.IsApproved)
                .OrderBy(r => r.Date)
                .ThenBy(r => r.PeriodNumber)
                .ToList();

            var recordsByTeacher = records
                .GroupBy(r => r.TeacherId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var recordTeacherIds = recordsByTeacher.Keys.ToList();

            var teacherIds = db.Teachers
                .Where(t => t.IsActive || existingPayrollTeacherIds.Contains(t.Id) || recordTeacherIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToList();

            var teachers = db.Teachers
                .Where(t => teacherIds.Contains(t.Id))
                .ToList();

            var payrolls = db.TeacherPayrolls
                .Include(p => p.Items)
                .Where(p => p.PayrollPeriodId == payrollPeriodId)
                .ToList()
                .ToDictionary(p => p.TeacherId);

            // ⭐ تحميل كل أسعار المواد مسبقاً في استعلام واحد (بدل N استعلام)
            var allAssignments = db.TeacherSubjectAssignments
                .Where(a => a.IsActive &&
                            a.StartDate <= period.EndDate &&
                            (!a.EndDate.HasValue || a.EndDate.Value >= period.StartDate) &&
                            teacherIds.Contains(a.TeacherId))
                .Select(a => new
                {
                    a.TeacherId,
                    a.SubjectId,
                    a.StartDate,
                    a.EndDate,
                    a.LessonRate
                })
                .ToList();

            // بناء الـ Cache مسبقاً
            foreach (var a in allAssignments)
            {
                string key = a.TeacherId + "_" + a.SubjectId;
                if (!_rateCache.ContainsKey(key) || _rateCache[key] < a.LessonRate)
                {
                    _rateCache[key] = a.LessonRate;
                }
            }

            // Cache للأسعار الافتراضية
            foreach (var t in teachers)
            {
                _teacherDefaultRateCache[t.Id] = t.LessonRate;
            }

            // حذف تفاصيل الحصص السابقة فقط (البنود اليدوية لا تُحذف)
            var oldLines = db.PayrollLessonLines
                .Where(l => l.TeacherPayroll.PayrollPeriodId == payrollPeriodId)
                .ToList();

            if (oldLines.Count > 0)
            {
                db.PayrollLessonLines.RemoveRange(oldLines);
                db.SaveChanges(); // حفظ الحذف أولاً
            }

            int processedCount = 0;

            foreach (var teacher in teachers)
            {
                TeacherPayroll payroll;
                if (!payrolls.TryGetValue(teacher.Id, out payroll))
                {
                    payroll = new TeacherPayroll
                    {
                        PayrollPeriodId = payrollPeriodId,
                        TeacherId = teacher.Id
                    };
                    db.TeacherPayrolls.Add(payroll);
                    payrolls[teacher.Id] = payroll;
                }

                var teacherRecords = recordsByTeacher.ContainsKey(teacher.Id)
                    ? recordsByTeacher[teacher.Id]
                    : new List<AttendanceRecord>();

                ResetCalculatedFields(payroll);

                payroll.EmploymentTypeApplied = teacher.EmploymentType;
                payroll.BaseSalaryApplied = decimal.Round(Math.Max(0m, teacher.BaseSalary), 2);
                payroll.MonthlyHoursApplied = decimal.Round(Math.Max(0m, teacher.MonthlyHours), 2);
                payroll.HourlyRateApplied = decimal.Round(Math.Max(0m, teacher.HourlyRate), 2);

                int full = 0, partial = 0, excused = 0, unexcused = 0;
                int cancelled = 0, subFull = 0, subPartial = 0;
                int totalPaidMinutes = 0;
                decimal earned = 0m;

                foreach (var rec in teacherRecords)
                {
                    result.LessonsCounted++;
                    int duration = rec.DailyLesson != null && rec.DailyLesson.DurationMinutes > 0
                        ? rec.DailyLesson.DurationMinutes : 45;
                    int actual = Math.Max(0, Math.Min(rec.ActualMinutes, duration));

                    switch (rec.Status)
                    {
                        case AttendanceStatus.PresentFull:
                        case AttendanceStatus.PresentPartial:
                            if (actual >= duration) full++; else partial++;
                            totalPaidMinutes += actual;
                            break;

                        case AttendanceStatus.AbsentExcused:
                            excused++;
                            break;

                        case AttendanceStatus.AbsentUnexcused:
                            unexcused++;
                            break;

                        case AttendanceStatus.SubstitutionFull:
                        case AttendanceStatus.SubstitutionPartial:
                            if (actual >= duration) subFull++; else subPartial++;
                            totalPaidMinutes += actual;
                            break;

                        case AttendanceStatus.Cancelled:
                            cancelled++;
                            break;
                    }
                }

                payroll.ScheduledLessons = teacherRecords.Count;
                payroll.FullAttendance = full;
                payroll.PartialAttendance = partial;
                payroll.AbsentExcused = excused;
                payroll.AbsentUnexcused = unexcused;
                payroll.Cancelled = cancelled;
                payroll.SubstitutionFull = subFull;
                payroll.SubstitutionPartial = subPartial;
                payroll.TotalMinutes = totalPaidMinutes;

                if (teacher.EmploymentType == EmploymentType.PerLesson)
                {
                    foreach (var rec in teacherRecords)
                    {
                        int duration = rec.DailyLesson != null && rec.DailyLesson.DurationMinutes > 0
                            ? rec.DailyLesson.DurationMinutes : 45;
                        int actual = Math.Max(0, Math.Min(rec.ActualMinutes, duration));
                        decimal rate = ResolveHistoricalLessonRate(teacher.Id, rec);
                        decimal lineEarned = RateForLesson(rate, rec.Status, actual, duration);

                        if (lineEarned > 0m) earned += lineEarned;

                        var daily = rec.DailyLesson;
                        db.PayrollLessonLines.Add(new PayrollLessonLine
                        {
                            TeacherPayroll = payroll,
                            AttendanceRecordId = rec.Id,
                            SubjectId = daily != null ? (int?)daily.SubjectId : null,
                            ClassRoomId = daily != null ? (int?)daily.ClassRoomId : null,
                            Date = rec.Date.Date,
                            PeriodNumber = rec.PeriodNumber,
                            DurationMinutes = duration,
                            ActualMinutes = actual,
                            Status = (int)rec.Status,
                            LessonRate = decimal.Round(rate, 2),
                            EarnedAmount = decimal.Round(lineEarned, 2),
                            DeductionAmount = 0m
                        });
                    }

                    payroll.EarnedAmount = decimal.Round(earned, 2);
                    payroll.LessonRateApplied = GetSingleOrZeroRate(teacherRecords, teacher.Id);
                    payroll.DeductionAmount = 0m;
                }
                else if (teacher.EmploymentType == EmploymentType.Permanent ||
                         teacher.EmploymentType == EmploymentType.PermanentSupervisor)
                {
                    payroll.EarnedAmount = payroll.BaseSalaryApplied;
                    payroll.LessonRateApplied = 0m;
                    payroll.DeductionAmount = 0m;
                }
                else if (teacher.EmploymentType == EmploymentType.PartTime)
                {
                    payroll.EarnedAmount = payroll.HourlyRateApplied > 0m
                        ? decimal.Round(totalPaidMinutes * payroll.HourlyRateApplied / 60m, 2)
                        : 0m;
                    payroll.LessonRateApplied = GetSingleOrZeroRate(teacherRecords, teacher.Id);
                    payroll.DeductionAmount = 0m;
                }
                else
                {
                    decimal legacyEarned = 0m;
                    foreach (var rec in teacherRecords)
                    {
                        int duration = rec.DailyLesson != null && rec.DailyLesson.DurationMinutes > 0
                            ? rec.DailyLesson.DurationMinutes : 45;
                        int actual = Math.Max(0, Math.Min(rec.ActualMinutes, duration));
                        legacyEarned += RateForLesson(ResolveHistoricalLessonRate(teacher.Id, rec), rec.Status, actual, duration);
                    }
                    payroll.EarnedAmount = decimal.Round(legacyEarned, 2);
                    payroll.LessonRateApplied = GetSingleOrZeroRate(teacherRecords, teacher.Id);
                    payroll.DeductionAmount = 0m;
                }

                // البنود اليدوية
                decimal itemDeductions = payroll.Items == null
                    ? 0m
                    : payroll.Items.Where(i => i.Type == PayrollItemType.Deduction).Sum(i => i.Amount);
                decimal itemAdjustments = payroll.Items == null
                    ? 0m
                    : payroll.Items.Where(i => i.Type != PayrollItemType.Deduction).Sum(i => i.Amount);

                payroll.DeductionAmount = decimal.Round(payroll.DeductionAmount + itemDeductions, 2);
                payroll.AdjustmentAmount = decimal.Round(itemAdjustments, 2);
                payroll.NetAmount = decimal.Round(
                    payroll.EarnedAmount - payroll.DeductionAmount + payroll.AdjustmentAmount, 2);

                result.TotalEarned += payroll.EarnedAmount;
                result.TotalDeducted += payroll.DeductionAmount;
                result.TeachersProcessed++;

                // ⭐ حفظ متقطع كل 20 معلم (لتجنب فقدان العمل عند خطأ)
                processedCount++;
                if (processedCount % 20 == 0)
                {
                    db.SaveChanges();
                }
            }

            db.SaveChanges();

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            sw.Stop();
            result.ElapsedSeconds = sw.Elapsed.TotalSeconds;
            return result;
        }

        private void ResetCalculatedFields(TeacherPayroll payroll)
        {
            payroll.ScheduledLessons = 0;
            payroll.FullAttendance = 0;
            payroll.PartialAttendance = 0;
            payroll.AbsentExcused = 0;
            payroll.AbsentUnexcused = 0;
            payroll.Cancelled = 0;
            payroll.SubstitutionFull = 0;
            payroll.SubstitutionPartial = 0;
            payroll.TotalMinutes = 0;
            payroll.LessonRateApplied = 0m;
            payroll.HourlyRateApplied = 0m;
            payroll.BaseSalaryApplied = 0m;
            payroll.MonthlyHoursApplied = 0m;
            payroll.EarnedAmount = 0m;
            payroll.DeductionAmount = 0m;
            payroll.AdjustmentAmount = 0m;
            payroll.NetAmount = 0m;
        }

        /// <summary>
        /// ⭐ نسخة محسّنة تستخدم الـ Cache بدل الاستعلام لكل حصة.
        /// </summary>
        private decimal ResolveHistoricalLessonRate(int teacherId, AttendanceRecord record)
        {
            if (record != null && record.LessonRateApplied > 0m)
                return record.LessonRateApplied;

            if (record != null && record.DailyLesson != null)
            {
                string key = teacherId + "_" + record.DailyLesson.SubjectId;
                decimal cachedRate;
                if (_rateCache.TryGetValue(key, out cachedRate) && cachedRate > 0m)
                    return cachedRate;
            }

            // السعر الافتراضي للمعلم
            decimal defaultRate;
            if (_teacherDefaultRateCache.TryGetValue(teacherId, out defaultRate) && defaultRate > 0m)
                return defaultRate;

            return 0m;
        }

        private decimal GetSingleOrZeroRate(List<AttendanceRecord> records, int teacherId)
        {
            var rates = records
                .Select(r => ResolveHistoricalLessonRate(teacherId, r))
                .Where(r => r > 0m)
                .Distinct()
                .ToList();

            return rates.Count == 1 ? rates[0] : 0m;
        }

        public void AdjustWithItems(TeacherPayroll payroll)
        {
            if (payroll == null) throw new ArgumentNullException("payroll");

            var items = db.PayrollItems
                .Where(i => i.TeacherPayrollId == payroll.Id)
                .ToList();

            decimal itemDeductions = items
                .Where(i => i.Type == PayrollItemType.Deduction)
                .Select(i => i.Amount)
                .DefaultIfEmpty(0m)
                .Sum();

            decimal itemAdjustments = items
                .Where(i => i.Type != PayrollItemType.Deduction)
                .Select(i => i.Amount)
                .DefaultIfEmpty(0m)
                .Sum();

            decimal attendanceDeduction = payroll.EmploymentTypeApplied == EmploymentType.PerLesson
                ? Math.Max(0m, payroll.DeductionAmount - itemDeductions)
                : 0m;

            payroll.DeductionAmount = decimal.Round(attendanceDeduction + itemDeductions, 2);
            payroll.AdjustmentAmount = decimal.Round(itemAdjustments, 2);
            payroll.NetAmount = decimal.Round(
                payroll.EarnedAmount - payroll.DeductionAmount + payroll.AdjustmentAmount, 2);

            db.SaveChanges();
        }
    }
}