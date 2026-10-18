using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Payroll
{
    public partial class Statement : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();
        private int _teacherId;
        private int _periodId;
        private DateTime _periodStart;
        private DateTime _periodEnd;
        private decimal _lessonRateApplied;

        public int PeriodId { get { return _periodId; } }

        public int PayrollId
        {
            get
            {
                int v;
                return int.TryParse(Request["payrollId"], out v) ? v : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SecurityHelper.DenyUnlessPath();

                if (IsPostBack) return;

                if (PayrollId == 0)
                {
                    SetInfo("لم يتم تحديد رقم الاستحقاق.", "danger");
                    return;
                }

                int ownTeacherId = 0;
                if (SecurityHelper.CurrentRole == UserRole.Teacher)
                {
                    string employeeNo = SecurityHelper.CurrentEmployeeNo;
                    if (string.IsNullOrWhiteSpace(employeeNo))
                    {
                        SetInfo("لا يوجد حساب معلم مرتبط بهذا المستخدم.", "danger");
                        return;
                    }

                    ownTeacherId = db.Teachers
                        .Where(t => t.EmployeeNo == employeeNo)
                        .Select(t => t.Id)
                        .FirstOrDefault();

                    if (ownTeacherId == 0)
                    {
                        SetInfo("لا يوجد سجل معلم مرتبط بهذا المستخدم.", "danger");
                        return;
                    }
                }

                var payroll = db.TeacherPayrolls
                    .Where(p => p.Id == PayrollId && (ownTeacherId == 0 || p.TeacherId == ownTeacherId))
                    .Select(p => new
                    {
                        p.PayrollPeriodId,
                        p.TeacherId,
                        p.EarnedAmount,
                        p.DeductionAmount,
                        p.AdjustmentAmount,
                        p.NetAmount,
                        p.LessonRateApplied,
                        p.HourlyRateApplied,
                        p.EmploymentTypeApplied,
                        p.BaseSalaryApplied,
                        p.ScheduledLessons,
                        p.FullAttendance,
                        p.PartialAttendance,
                        p.AbsentExcused,
                        p.AbsentUnexcused,
                        p.Cancelled,
                        p.SubstitutionFull,
                        p.SubstitutionPartial
                    })
                    .FirstOrDefault();

                if (payroll == null)
                {
                    SetInfo("الاستحقاق غير موجود.", "danger");
                    return;
                }

                _teacherId = payroll.TeacherId;
                _periodId = payroll.PayrollPeriodId;
                _lessonRateApplied = payroll.LessonRateApplied;

                var teacher = db.Teachers.Find(payroll.TeacherId);
                var period = db.PayrollPeriods.Find(payroll.PayrollPeriodId);
                if (teacher == null || period == null)
                {
                    SetInfo("تعذر جلب بيانات المعلم أو الفترة.", "danger");
                    return;
                }

                _periodStart = period.StartDate;
                _periodEnd = period.EndDate;

                string[] months = { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                                    "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };

                string periodText = months[period.Month - 1] + " " + period.Year;

                // ==== الهيدر (Hero) ====
                lblTeacherName.Text = Server.HtmlEncode(teacher.Name);
                lblPeriod.Text = Server.HtmlEncode(periodText);

                // ==== بطاقة بيانات المعلم والعقد ====
                lblTeacherName2.Text = Server.HtmlEncode(teacher.Name);
                lblPeriod2.Text = Server.HtmlEncode(periodText);
                lblEmployment.Text = Server.HtmlEncode(EnumHelper.GetDisplayName(teacher.EmploymentType));

                if (payroll.EmploymentTypeApplied == EmploymentType.Permanent ||
                    payroll.EmploymentTypeApplied == EmploymentType.PermanentSupervisor)
                {
                    lblLessonRate.Text = "راتب شهري ثابت: " +
                        payroll.BaseSalaryApplied.ToString("N2") + " د.ل";
                }
                else if (payroll.EmploymentTypeApplied == EmploymentType.PerLesson)
                {
                    lblLessonRate.Text = payroll.LessonRateApplied > 0m
                        ? payroll.LessonRateApplied.ToString("N2") + " د.ل / حصة"
                        : "سعر مختلف حسب المادة";
                }
                else
                {
                    lblLessonRate.Text = payroll.HourlyRateApplied > 0m
                        ? payroll.HourlyRateApplied.ToString("N2") + " د.ل / ساعة"
                        : payroll.LessonRateApplied.ToString("N2") + " د.ل / حصة";
                }

                // ==== بطاقة الملخص المالي ====

                // ⭐ حساب إجمالي التعويضات من أسطر الحصص
                int subFull = (int)AttendanceStatus.SubstitutionFull;
                int subPartial = (int)AttendanceStatus.SubstitutionPartial;

                decimal substitutions = db.PayrollLessonLines
                    .Where(l => l.TeacherPayrollId == PayrollId
                                && (l.Status == subFull || l.Status == subPartial))
                    .Sum(l => (decimal?)l.EarnedAmount) ?? 0m;

                // ⭐ المستحق الأساسي = EarnedAmount − التعويضات
                decimal baseEarned = payroll.EarnedAmount - substitutions;
                if (baseEarned < 0m) baseEarned = 0m;

                lblEarned.Text = baseEarned.ToString("N2") + " د.ل";
                lblSubstitutions.Text = substitutions.ToString("N2") + " د.ل";
                lblDeductions.Text = payroll.DeductionAmount.ToString("N2") + " د.ل";
                lblAdjustments.Text = payroll.AdjustmentAmount.ToString("N2") + " د.ل";
                lblNet.Text = payroll.NetAmount.ToString("N2") + " د.ل";

                LoadStatement();
                LoadItems();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.Statement.Page_Load");
            }
        }

        #region Data Loading

        private void LoadStatement()
        {
            try
            {
                var payrollHeader = db.TeacherPayrolls.Find(PayrollId);
                if (payrollHeader == null)
                {
                    gvRecords.DataSource = null;
                    gvRecords.DataBind();
                    return;
                }

                // ==== عقد براتب شهري ثابت: لا تُعرض الحصص كأجر إضافي ====
                if (payrollHeader.EmploymentTypeApplied == EmploymentType.Permanent ||
                    payrollHeader.EmploymentTypeApplied == EmploymentType.PermanentSupervisor)
                {
                    gvRecords.DataSource = null;
                    gvRecords.DataBind();
                    SetInfo("هذا العقد راتب شهري ثابت؛ الحصص الدراسية الظاهرة في الحضور لا تُحوّل إلى أجر حصص إضافي. قيمة الراتب الأساسي محفوظة في رأس الاستحقاق.", "neutral");
                    return;
                }

                // ==== معلم بالحصة: العرض من PayrollLessonLines ====
                if (payrollHeader.EmploymentTypeApplied == EmploymentType.PerLesson)
                {
                    var lines = (from l in db.PayrollLessonLines
                                 join s in db.Subjects on l.SubjectId equals s.Id into subjects
                                 from s in subjects.DefaultIfEmpty()
                                 join c in db.ClassRooms on l.ClassRoomId equals c.Id into classes
                                 from c in classes.DefaultIfEmpty()
                                 where l.TeacherPayrollId == PayrollId
                                 orderby l.Date, l.PeriodNumber
                                 select new
                                 {
                                     Id = l.Id,
                                     Date = l.Date,
                                     PeriodNumber = l.PeriodNumber,
                                     DurationMinutes = l.DurationMinutes,
                                     SubjectName = s != null ? s.Name : "—",
                                     ClassRoomName = c != null ? c.Name : "—",
                                     Status = l.Status,
                                     ActualMinutes = l.ActualMinutes,
                                     LessonRate = l.LessonRate,
                                     DeductionAmount = l.DeductionAmount,
                                     EarnedAmount = l.EarnedAmount
                                 }).ToList();

                    gvRecords.DataSource = lines.Select(r => new
                    {
                        r.Id,
                        r.Date,
                        r.PeriodNumber,
                        StartTimeText = "—",
                        EndTimeText = "—",
                        r.DurationMinutes,
                        r.SubjectName,
                        r.ClassRoomName,
                        r.Status,
                        r.ActualMinutes,
                        RateDisplay = r.LessonRate.ToString("N2"),
                        LineDisplay = r.DeductionAmount > 0m
                            ? "-" + r.DeductionAmount.ToString("N2")
                            : (r.EarnedAmount > 0m ? "+" + r.EarnedAmount.ToString("N2") : "0.00")
                    }).ToList();

                    gvRecords.DataBind();
                    return;
                }

                // ==== توافق مع السجلات القديمة ذات الأجر بالساعة/الحصة ====
                var service = new PayrollCalculationService(db);
                var rows = (from r in db.AttendanceRecords
                            where r.TeacherId == _teacherId
                                  && r.Date >= _periodStart
                                  && r.Date <= _periodEnd
                                  && r.IsApproved
                            join d in db.DailyLessons on r.DailyLessonId equals d.Id into days
                            from d in days.DefaultIfEmpty()
                            orderby r.Date, r.PeriodNumber
                            select new
                            {
                                r.Id,
                                r.Date,
                                r.PeriodNumber,
                                StartTime = d != null ? d.StartTime : (TimeSpan?)null,
                                EndTime = d != null ? d.EndTime : (TimeSpan?)null,
                                DurationMinutes = d != null && d.DurationMinutes > 0 ? d.DurationMinutes : 45,
                                SubjectName = d != null && d.Subject != null ? d.Subject.Name : "—",
                                ClassRoomName = d != null && d.ClassRoom != null ? d.ClassRoom.Name : "—",
                                r.Status,
                                r.ActualMinutes,
                                r.LessonRateApplied,
                                r.HourlyRateApplied
                            }).ToList();

                var result = rows.Select(r =>
                {
                    var status = (AttendanceStatus)r.Status;
                    decimal rate = r.LessonRateApplied;
                    decimal line = service.RateForLesson(rate, status, r.ActualMinutes, r.DurationMinutes);

                    string lineDisplay;
                    if (line > 0m) lineDisplay = "+" + line.ToString("N2");
                    else lineDisplay = "0.00";

                    return new
                    {
                        r.Id,
                        r.Date,
                        r.PeriodNumber,
                        StartTimeText = r.StartTime.HasValue ? r.StartTime.Value.ToString(@"hh\:mm") : "—",
                        EndTimeText = r.EndTime.HasValue ? r.EndTime.Value.ToString(@"hh\:mm") : "—",
                        r.DurationMinutes,
                        r.SubjectName,
                        r.ClassRoomName,
                        r.Status,
                        r.ActualMinutes,
                        RateDisplay = rate.ToString("N2"),
                        LineDisplay = lineDisplay
                    };
                }).ToList();

                gvRecords.DataSource = result;
                gvRecords.DataBind();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.Statement.LoadStatement");
            }
        }

        private void LoadItems()
        {
            try
            {
                var items = db.PayrollItems
                    .Where(i => i.TeacherPayrollId == PayrollId)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToList();

                gvItems.DataSource = items;
                gvItems.DataBind();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.Statement.LoadItems");
            }
        }

        #endregion

        #region Display Helpers

        protected string StatusName(int status)
        {
            return EnumHelper.GetDisplayName((AttendanceStatus)status);
        }

        protected string StatusClass(int status)
        {
            var s = (AttendanceStatus)status;
            switch (s)
            {
                case AttendanceStatus.PresentFull:
                case AttendanceStatus.SubstitutionFull:
                    return "badge-green";

                case AttendanceStatus.PresentPartial:
                case AttendanceStatus.SubstitutionPartial:
                case AttendanceStatus.AbsentExcused:
                    return "badge-yellow";

                case AttendanceStatus.AbsentUnexcused:
                    return "badge-red";

                case AttendanceStatus.Cancelled:
                    return "badge-gray";

                default:
                    return "badge-gray";
            }
        }

        protected string ItemTypeName(int itemType)
        {
            return EnumHelper.GetDisplayName((PayrollItemType)itemType);
        }

        protected string GetLineClass(object lineDisplay)
        {
            string s = lineDisplay == null ? "" : lineDisplay.ToString().Trim();

            if (s.StartsWith("+")) return "pos";
            if (s.StartsWith("-")) return "neg";
            return "neutral";
        }

        #endregion

        #region Paging

        protected void gvRecords_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            try
            {
                gvRecords.PageIndex = e.NewPageIndex;
                LoadStatement();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.Statement.gvRecords_PageIndexChanging");
            }
        }

        #endregion

        #region Info / Error

        private void SetInfo(string message, string kind)
        {
            if (lblInfo == null) return;

            string k = string.IsNullOrWhiteSpace(kind) ? "neutral" : kind;
            lblInfo.CssClass = "tf-info-msg " + k;
            lblInfo.Text = Server.HtmlEncode(message ?? "");
            lblInfo.Visible = true;
        }

        private void HandleException(Exception ex, string source)
        {
            try { AppErrorLogger.Log(ex, source); }
            catch { }

            string userMessage = "حدث خطأ غير متوقع أثناء تنفيذ العملية. حاول مرة أخرى.";
            try
            {
                string mapped = ErrorMessageHelper.GetUserMessage(ex, source);
                if (!string.IsNullOrWhiteSpace(mapped))
                    userMessage = mapped;
            }
            catch { }

            SetInfo(userMessage, "danger");
        }

        #endregion

        protected override void OnUnload(EventArgs e)
        {
            if (db != null)
            {
                try { db.Dispose(); } catch { }
            }
            base.OnUnload(e);
        }
    }
}