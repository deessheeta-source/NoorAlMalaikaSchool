using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// طبقة التحقق الموحدة بين:
    /// - الحصص اليومية DailyLessons
    /// - سجلات حضور المعلمين AttendanceRecords
    /// - الجدول الدراسي المعتمد TimetableEntries
    /// - التعويضيات Substitutions
    /// - الفترات المالية PayrollPeriods
    ///
    /// هذه الخدمة للقراءة والتحقق فقط ولا تقوم بتعديل البيانات.
    ///
    /// ✅ جميع دوال التحقق تدعم فلترة اختيارية حسب المرحلة الدراسية (Basic/Secondary).
    /// </summary>
    public class TeacherAttendanceFinancialService
    {
        private readonly SchoolDbContext db;

        public TeacherAttendanceFinancialService(SchoolDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            db = context;
        }

        #region Validation Models

        public class ValidationIssue
        {
            public string Severity { get; set; }

            public DateTime? Date { get; set; }

            public int? TeacherId { get; set; }

            public string TeacherName { get; set; }

            public int? DailyLessonId { get; set; }

            public int? PeriodNumber { get; set; }

            public string Message { get; set; }

            public bool IsError
            {
                get
                {
                    return string.Equals(
                        Severity,
                        "ERROR",
                        StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        public class ValidationResult
        {
            public bool IsReady { get; set; }

            public int ScheduledLessons { get; set; }

            public int RecordedLessons { get; set; }

            public int ApprovedLessons { get; set; }

            public int MissingLessons { get; set; }

            public int UnapprovedLessons { get; set; }

            public int ConflictCount { get; set; }

            public int WarningCount { get; set; }

            public bool IsFuturePeriod { get; set; }

            public List<ValidationIssue> Issues { get; private set; }

            public ValidationResult()
            {
                Issues = new List<ValidationIssue>();
            }

            public IEnumerable<ValidationIssue> Errors
            {
                get { return Issues.Where(i => i != null && i.IsError); }
            }

            public IEnumerable<ValidationIssue> Warnings
            {
                get { return Issues.Where(i => i != null && !i.IsError); }
            }

            public string Summary
            {
                get
                {
                    if (IsFuturePeriod)
                        return "الفترة المالية لم تبدأ بعد؛ لا يمكن فحص أو حساب حضورها المالي قبل وصول تاريخ بدايتها.";

                    if (IsReady)
                        return "الحضور مكتمل ومعتمد وجاهز للحساب المالي.";

                    return string.Format(
                        "الحضور غير جاهز مالياً: {0} حصة مجدولة حتى تاريخ الفحص، {1} مسجلة، {2} معتمدة، {3} ناقصة، {4} غير معتمدة، {5} تعارض مالي.",
                        ScheduledLessons,
                        RecordedLessons,
                        ApprovedLessons,
                        MissingLessons,
                        UnapprovedLessons,
                        ConflictCount);
                }
            }
        }

        #endregion

        #region Public Validation Methods

        /// <summary>
        /// فحص حضور يوم واحد.
        /// </summary>
        /// <param name="date">التاريخ المطلوب فحصه.</param>
        /// <param name="requireApproved">هل يجب أن تكون السجلات معتمدة؟</param>
        /// <param name="stage">
        /// المرحلة الدراسية المطلوبة (اختياري).
        /// عند تمريرها، يتم فحص حصص هذه المرحلة فقط.
        /// عند null، يتم فحص جميع المراحل.
        /// </param>
        public ValidationResult ValidateDate(
            DateTime date,
            bool requireApproved = true,
            EducationStage? stage = null)
        {
            DateTime targetDate = date.Date;

            return ValidateRange(
                targetDate,
                targetDate,
                requireApproved,
                stage);
        }

        /// <summary>
        /// فحص فترة رواتب كاملة.
        /// </summary>
        /// <param name="payrollPeriodId">معرف الفترة المالية.</param>
        /// <param name="requireApproved">هل يجب أن تكون السجلات معتمدة؟</param>
        /// <param name="stage">
        /// المرحلة الدراسية المطلوبة (اختياري).
        /// عند تمريرها، يتم فحص حصص هذه المرحلة فقط.
        /// عند null، يتم فحص جميع المراحل.
        /// </param>
        public ValidationResult ValidatePeriod(
            int payrollPeriodId,
            bool requireApproved = true,
            EducationStage? stage = null)
        {
            var result = new ValidationResult();

            var period = db.PayrollPeriods.Find(payrollPeriodId);

            if (period == null)
            {
                AddError(result, "الفترة المالية غير موجودة.");

                result.ConflictCount = 1;
                result.IsReady = false;

                return result;
            }

            DateTime startDate = period.StartDate.Date;
            DateTime endDate = period.EndDate.Date;

            /*
             * إذا كانت الفترة مفتوحة أو قيد المراجعة،
             * فلا يتم اعتبار الأيام المستقبلية حصصاً ناقصة.
             */
            if (period.Status == PayrollPeriodStatus.Open ||
                period.Status == PayrollPeriodStatus.UnderReview)
            {
                DateTime today = DateTime.Today;

                if (startDate > today)
                {
                    result.IsFuturePeriod = true;
                    AddWarning(result, "الفترة المالية لم تبدأ بعد. سيتم فحص الحضور والحساب المالي بعد وصول تاريخ البداية.");
                    result.IsReady = false;
                    return result;
                }
                else if (endDate > today)
                {
                    endDate = today;
                }
            }

            return ValidateRange(
                startDate,
                endDate,
                requireApproved,
                stage);
        }

        #endregion

        #region Main Validation

        private ValidationResult ValidateRange(
            DateTime startDate,
            DateTime endDate,
            bool requireApproved,
            EducationStage? stage)
        {
            var result = new ValidationResult();

            startDate = startDate.Date;
            endDate = endDate.Date;

            if (endDate < startDate)
            {
                AddWarning(
                    result,
                    "الفترة لم تبدأ بعد؛ لا توجد أيام مستحقة للفحص حالياً.");

                result.IsReady = false;

                return result;
            }

            DateTime exclusiveEnd = endDate.AddDays(1);

            /*
             * =========================================================
             * ✅ قائمة معرفات المعلمين في المرحلة المطلوبة (إن حُدّدت)
             * =========================================================
             */

            // الحضور المالي للحصص مطلوب فقط للمعلمين الذين نوع تعاقدهم أجر بالحصص.
            // المعلم الدائم والمشرف الدائم راتبهما شهري ثابت، لذلك لا يجب أن
            // يمنع غياب سجل حصة دراسية اعتماد راتبهما الشهري.
            List<int> stageTeacherIds = db.Teachers
                .Where(t => t.IsActive && t.EmploymentType == EmploymentType.PerLesson)
                .Where(t => !stage.HasValue || t.Stage == stage.Value)
                .Select(t => t.Id)
                .ToList();

            if (stageTeacherIds.Count == 0)
            {
                result.IsReady = true;
                result.ScheduledLessons = 0;
                result.RecordedLessons = 0;
                result.ApprovedLessons = 0;
                return result;
            }

            /*
             * =========================================================
             * 1. تحميل الحصص اليومية (مفلترة حسب المرحلة إن وُجدت)
             * =========================================================
             */

            var lessonsQuery = db.DailyLessons
                .Include(d => d.Teacher)
                .Where(d =>
                    d.Date >= startDate &&
                    d.Date < exclusiveEnd);

            if (stageTeacherIds != null)
            {
                lessonsQuery = lessonsQuery
                    .Where(d => stageTeacherIds.Contains(d.TeacherId));
            }

            var lessons = lessonsQuery
                .OrderBy(d => d.Date)
                .ThenBy(d => d.PeriodNumber)
                .ToList();

            var lessonIds = lessons
                .Select(d => d.Id)
                .Distinct()
                .ToList();

            result.ScheduledLessons = lessons.Count;

            /*
             * =========================================================
             * 2. تحميل سجلات الحضور
             * =========================================================
             */

            var records = lessonIds.Count == 0
                ? new List<AttendanceRecord>()
                : db.AttendanceRecords
                    .Include(r => r.DailyLesson)
                    .Include(r => r.Teacher)
                    .Where(r => lessonIds.Contains(r.DailyLessonId))
                    .ToList();

            result.RecordedLessons = records
                .Select(r => r.DailyLessonId)
                .Distinct()
                .Count();

            /*
             * =========================================================
             * 3. أسماء المعلمين
             * =========================================================
             */

            var teacherById = db.Teachers
                .AsNoTracking()
                .ToList()
                .GroupBy(t => t.Id)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().Name ?? "");

            /*
             * =========================================================
             * 4. تحميل التعويضيات المستخدمة في الحضور
             * =========================================================
             */

            var substitutionIds = records
                .Where(r => r.SourceSubstitutionId.HasValue)
                .Select(r => r.SourceSubstitutionId.Value)
                .Distinct()
                .ToList();

            var substitutions = substitutionIds.Count == 0
                ? new Dictionary<int, Substitution>()
                : db.Substitutions
                    .Where(s => substitutionIds.Contains(s.Id))
                    .ToList()
                    .GroupBy(s => s.Id)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First());

            /*
             * =========================================================
             * 5. تحميل الجدول الدراسي المعتمد (مفلتر حسب المرحلة)
             * =========================================================
             */

            var timetableQuery = db.TimetableEntries
                .AsNoTracking()
                .Where(t => t.Status == LessonStatus.Approved);

            if (stageTeacherIds != null)
            {
                timetableQuery = timetableQuery
                    .Where(t => stageTeacherIds.Contains(t.TeacherId));
            }

            var approvedTimetable = timetableQuery.ToList();

            /*
             * =========================================================
             * 6. التأكد من توليد الحصص من الجدول المعتمد
             * =========================================================
             */

            for (
                DateTime day = startDate;
                day <= endDate;
                day = day.AddDays(1))
            {
                /*
                 * الجمعة والسبت عطلة.
                 */
                if (day.DayOfWeek == DayOfWeek.Friday ||
                    day.DayOfWeek == DayOfWeek.Saturday)
                {
                    continue;
                }

                WeekDay weekDay;

                try
                {
                    weekDay = ToWeekDay(day.DayOfWeek);
                }
                catch (ArgumentOutOfRangeException)
                {
                    continue;
                }

                var expectedEntries = approvedTimetable
                    .Where(t => t.DayOfWeek == weekDay)
                    .ToList();

                foreach (var entry in expectedEntries)
                {
                    /*
                     * الطريقة الأولى:
                     * الربط المباشر عن طريق ScheduleEntryId.
                     */
                    bool generated = lessons.Any(l =>
                        l.Date.Date == day.Date &&
                        l.ScheduleEntryId.HasValue &&
                        l.ScheduleEntryId.Value == entry.Id);

                    /*
                     * الطريقة الثانية:
                     * توافق مع البيانات القديمة.
                     */
                    if (!generated)
                    {
                        generated = lessons.Any(l =>
                            l.Date.Date == day.Date &&
                            !l.ScheduleEntryId.HasValue &&
                            l.PeriodNumber == entry.PeriodNumber &&
                            l.TeacherId == entry.TeacherId &&
                            l.SubjectId == entry.SubjectId &&
                            l.ClassRoomId == entry.ClassRoomId);
                    }

                    if (!generated)
                    {
                        result.MissingLessons++;

                        result.Issues.Add(new ValidationIssue
                        {
                            Severity = "ERROR",
                            Date = day,
                            TeacherId = entry.TeacherId,
                            TeacherName =
                                teacherById.ContainsKey(entry.TeacherId)
                                    ? teacherById[entry.TeacherId]
                                    : "",
                            PeriodNumber = entry.PeriodNumber,
                            Message =
                                "الحصة موجودة في الجدول المعتمد لكنها لم تُولّد في DailyLessons لهذا التاريخ. " +
                                "اضغط «مزامنة حصص الفترة» لتوليد الحصص الناقصة."
                        });
                    }
                }
            }

            /*
             * =========================================================
             * 7. فحص كل حصة يومية
             * =========================================================
             */

            foreach (var lesson in lessons)
            {
                var original = records.FirstOrDefault(r =>
                    r.DailyLessonId == lesson.Id &&
                    r.TeacherId == lesson.TeacherId);

                /*
                 * لا يوجد حضور للمعلم الأصلي.
                 */
                if (original == null)
                {
                    result.MissingLessons++;

                    result.Issues.Add(new ValidationIssue
                    {
                        Severity = "ERROR",
                        Date = lesson.Date,
                        TeacherId = lesson.TeacherId,
                        TeacherName =
                            lesson.Teacher != null
                                ? lesson.Teacher.Name ?? ""
                                : "",
                        DailyLessonId = lesson.Id,
                        PeriodNumber = lesson.PeriodNumber,
                        Message =
                            "الحصة المجدولة لا تحتوي على سجل حضور للمعلم الأصلي."
                    });
                }
                else
                {
                    /*
                     * ==========================================
                     * اعتماد الحصة
                     * ==========================================
                     */

                    if (original.IsApproved &&
                        IsFinancialStatus(original.Status))
                    {
                        result.ApprovedLessons++;
                    }

                    /*
                     * ==========================================
                     * تاريخ الحضور
                     * ==========================================
                     */

                    if (original.Date.Date != lesson.Date.Date)
                    {
                        AddConflict(
                            result,
                            original,
                            "تاريخ سجل الحضور لا يطابق تاريخ الحصة اليومية.");
                    }

                    /*
                     * ==========================================
                     * الدقائق السالبة
                     * ==========================================
                     */

                    if (original.ActualMinutes < 0 ||
                        original.AbsentMinutes < 0)
                    {
                        AddConflict(
                            result,
                            original,
                            "دقائق الحضور أو الغياب تحتوي على قيمة سالبة.");
                    }

                    /*
                     * ==========================================
                     * الحضور / التعويضية (كاملة أو جزئية)
                     * 0 < minutes <= duration
                     * absent = duration - minutes
                     *
                     * الحالة لا تحدّد الدقائق، بل الاستحقاق يُحسب في
                     * PayrollCalculationService بنسبة الدقائق الفعلية على
                     * مدة الحصة مهما كانت التسمية المختارة.
                     * ==========================================
                     */

                    if (
                        original.Status == AttendanceStatus.PresentFull ||
                        original.Status == AttendanceStatus.PresentPartial ||
                        original.Status == AttendanceStatus.SubstitutionFull ||
                        original.Status == AttendanceStatus.SubstitutionPartial)
                    {
                        if (
                            original.ActualMinutes <= 0 ||
                            original.ActualMinutes > lesson.DurationMinutes ||
                            original.AbsentMinutes !=
                                lesson.DurationMinutes -
                                original.ActualMinutes)
                        {
                            AddConflict(
                                result,
                                original,
                                "الحضور/التعويضية يجب أن تكون دقائق حضورها بين 1 و " +
                                lesson.DurationMinutes +
                                " دقيقة، وأن تساوي دقائق الغياب الفرق المتبقي.");
                        }
                    }

                    /*
                     * ==========================================
                     * غياب بعذر
                     * 0..duration, absent = duration - minutes
                     * ==========================================
                     */

                    else if (original.Status == AttendanceStatus.AbsentExcused)
                    {
                        if (
                            original.ActualMinutes < 0 ||
                            original.ActualMinutes > lesson.DurationMinutes ||
                            original.AbsentMinutes !=
                                lesson.DurationMinutes -
                                original.ActualMinutes)
                        {
                            AddConflict(
                                result,
                                original,
                                "الغياب بعذر يجب أن تكون دقائق حضوره بين 0 و " +
                                lesson.DurationMinutes +
                                "، وأن تساوي دقائق الغياب الفرق المتبقي.");
                        }
                    }

                    /*
                     * ==========================================
                     * غياب بدون عذر
                     * minutes = 0, absent = 0
                     * ==========================================
                     */

                    else if (original.Status == AttendanceStatus.AbsentUnexcused)
                    {
                        if (
                            original.ActualMinutes != 0 ||
                            original.AbsentMinutes != 0)
                        {
                            AddConflict(
                                result,
                                original,
                                "الغياب بدون عذر يجب أن يسجَّل بصفر دقائق حضور وصفر دقائق غياب.");
                        }
                    }

                    /*
                     * ==========================================
                     * الحصة الملغاة
                     * 0..duration, absent = 0 دائماً
                     * ==========================================
                     */

                    else if (original.Status == AttendanceStatus.Cancelled)
                    {
                        if (
                            original.ActualMinutes < 0 ||
                            original.ActualMinutes > lesson.DurationMinutes ||
                            original.AbsentMinutes != 0)
                        {
                            AddConflict(
                                result,
                                original,
                                "الحصة الملغاة يجب أن تكون دقائق حضورها بين 0 و " +
                                lesson.DurationMinutes +
                                "، وأن تكون دقائق الغياب صفراً.");
                        }
                    }

                    /*
                     * ==========================================
                     * القيمة المالية
                     * ==========================================
                     */

                    if (
                        original.Status == AttendanceStatus.PresentFull ||
                        original.Status == AttendanceStatus.PresentPartial ||
                        original.Status == AttendanceStatus.SubstitutionFull ||
                        original.Status == AttendanceStatus.SubstitutionPartial)
                    {
                        if (original.LessonRateApplied <= 0m)
                        {
                            AddConflict(
                                result,
                                original,
                                "الحصة المستحقة مالياً لا تحتوي على قيمة حصة محفوظة في سجل الحضور.");
                        }
                    }

                    /*
                     * ==========================================
                     * الاعتماد
                     * ==========================================
                     */

                    if (!original.IsApproved)
                    {
                        result.UnapprovedLessons++;

                        if (requireApproved)
                        {
                            result.Issues.Add(new ValidationIssue
                            {
                                Severity = "ERROR",
                                Date = lesson.Date,
                                TeacherId = lesson.TeacherId,
                                TeacherName =
                                    lesson.Teacher != null
                                        ? lesson.Teacher.Name ?? ""
                                        : "",
                                DailyLessonId = lesson.Id,
                                PeriodNumber = lesson.PeriodNumber,
                                Message =
                                    "سجل الحضور موجود لكنه غير معتمد."
                            });
                        }
                    }
                }

                /*
                 * =========================================================
                 * 8. فحص جميع سجلات الحضور للحصة
                 * =========================================================
                 */

                var lessonRecords = records
                    .Where(r => r.DailyLessonId == lesson.Id)
                    .ToList();

                foreach (var rec in lessonRecords)
                {
                    bool isOriginal =
                        rec.TeacherId == lesson.TeacherId;

                    /*
                     * معلم مختلف بدون تعويضية.
                     */
                    if (!isOriginal &&
                        !rec.SourceSubstitutionId.HasValue)
                    {
                        AddConflict(
                            result,
                            rec,
                            "يوجد سجل حضور لمعلم غير المعلم الأصلي بدون ربط بتعويضية.");
                    }

                    /*
                     * يوجد ربط بتعويضية.
                     */
                    if (rec.SourceSubstitutionId.HasValue)
                    {
                        Substitution substitution = null;

                        substitutions.TryGetValue(
                            rec.SourceSubstitutionId.Value,
                            out substitution);

                        if (substitution == null)
                        {
                            AddConflict(
                                result,
                                rec,
                                "سجل الحضور المالي يشير إلى تعويضية غير موجودة.");
                        }
                        else
                        {
                            if (!substitution.IsApproved)
                            {
                                AddConflict(
                                    result,
                                    rec,
                                    "التعويضية المرتبطة بسجل الحضور غير معتمدة.");
                            }

                            if (requireApproved &&
                                !rec.IsApproved)
                            {
                                AddConflict(
                                    result,
                                    rec,
                                    "سجل حضور التعويضية غير معتمد.");
                            }

                            if (substitution.DailyLessonId != lesson.Id)
                            {
                                AddConflict(
                                    result,
                                    rec,
                                    "التعويضية مرتبطة بحصة يومية مختلفة عن سجل الحضور.");
                            }

                            if (substitution.SubstituteTeacherId != rec.TeacherId)
                            {
                                AddConflict(
                                    result,
                                    rec,
                                    "المعلم البديل في التعويضية لا يطابق المعلم الموجود في سجل الحضور.");
                            }
                        }
                    }
                }
            }

            /*
             * =========================================================
             * 9. سجلات الحضور التي لا ترتبط بحصص موجودة
             * =========================================================
             */

            var lessonIdSet = new HashSet<int>(lessonIds);

            foreach (var rec in records)
            {
                if (!lessonIdSet.Contains(rec.DailyLessonId))
                {
                    AddConflict(
                        result,
                        rec,
                        "سجل حضور مرتبط بحصة غير موجودة ضمن الحصص المجدولة.");
                }
            }

            /*
             * =========================================================
             * 10. تحديد جاهزية الفترة مالياً
             * =========================================================
             */

            result.IsReady =
                result.ScheduledLessons > 0 &&
                result.MissingLessons == 0 &&
                result.ConflictCount == 0 &&
                (
                    !requireApproved ||
                    result.UnapprovedLessons == 0
                );

            return result;
        }

        #endregion

        #region Payroll Lock

        /// <summary>
        /// التحقق هل التاريخ داخل فترة رواتب مغلقة.
        /// </summary>
        public bool IsDateLocked(
            DateTime date,
            out string message)
        {
            DateTime targetDate = date.Date;

            var period = db.PayrollPeriods
                .Where(p =>
                    p.StartDate <= targetDate &&
                    p.EndDate >= targetDate)
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();

            if (period != null &&
                period.Status == PayrollPeriodStatus.Closed)
            {
                message =
                    "التاريخ داخل فترة رواتب مغلقة ولا يمكن تعديل الحضور بعد إغلاقها.";

                return true;
            }

            message = null;

            return false;
        }

        #endregion

        #region Helpers

        private static WeekDay ToWeekDay(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday:
                    return WeekDay.Sunday;

                case DayOfWeek.Monday:
                    return WeekDay.Monday;

                case DayOfWeek.Tuesday:
                    return WeekDay.Tuesday;

                case DayOfWeek.Wednesday:
                    return WeekDay.Wednesday;

                case DayOfWeek.Thursday:
                    return WeekDay.Thursday;

                default:
                    throw new ArgumentOutOfRangeException(
                        "day",
                        day,
                        "اليوم غير مدعوم في الجدول الدراسي.");
            }
        }

        /// <summary>
        /// الحالات التي يمكن أن تدخل في التسوية المالية.
        /// </summary>
        private static bool IsFinancialStatus(
            AttendanceStatus status)
        {
            return
                status == AttendanceStatus.PresentFull ||
                status == AttendanceStatus.PresentPartial ||
                status == AttendanceStatus.AbsentExcused ||
                status == AttendanceStatus.AbsentUnexcused ||
                status == AttendanceStatus.SubstitutionFull ||
                status == AttendanceStatus.SubstitutionPartial ||
                status == AttendanceStatus.Cancelled;
        }

        private static void AddConflict(
            ValidationResult result,
            AttendanceRecord record,
            string message)
        {
            if (result == null)
                return;

            if (record == null)
            {
                result.ConflictCount++;

                result.Issues.Add(new ValidationIssue
                {
                    Severity = "ERROR",
                    Message = message
                });

                return;
            }

            result.ConflictCount++;

            result.Issues.Add(new ValidationIssue
            {
                Severity = "ERROR",
                Date = record.Date,
                TeacherId = record.TeacherId,
                TeacherName =
                    record.Teacher != null
                        ? record.Teacher.Name ?? ""
                        : "",
                DailyLessonId = record.DailyLessonId,
                PeriodNumber = record.PeriodNumber,
                Message = message
            });
        }

        private static void AddError(
            ValidationResult result,
            string message)
        {
            if (result == null)
                return;

            result.Issues.Add(new ValidationIssue
            {
                Severity = "ERROR",
                Message = message
            });
        }

        private static void AddWarning(
            ValidationResult result,
            string message)
        {
            if (result == null)
                return;

            result.WarningCount++;

            result.Issues.Add(new ValidationIssue
            {
                Severity = "WARNING",
                Message = message
            });
        }

        #endregion
    }
}