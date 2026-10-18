using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class Periods : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private static readonly string[] ArabicMonths =
        {
            "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
            "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
        };

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SecurityHelper.DenyUnlessPath();

                if (!IsPostBack)
                {
                    BindPeriods();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "Periods.Page_Load");
            }
        }

        #endregion

        #region Bind

        private void BindPeriods()
        {
            var periods = db.PayrollPeriods
                .AsNoTracking()
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .Select(p => new
                {
                    p.Id,
                    p.Year,
                    p.Month,
                    p.StartDate,
                    p.EndDate,
                    Status = (int?)p.Status
                })
                .ToList();

            gvPeriods.DataSource = periods;
            gvPeriods.DataBind();

            if (pnlEmpty != null)
                pnlEmpty.Visible = periods.Count == 0;
        }

        #endregion

        #region Display Helpers

        /// <summary>يعرض اسم الشهر بالعربية.</summary>
        protected string GetMonthName(int? month)
        {
            if (!month.HasValue || month.Value < 1 || month.Value > 12)
                return "";

            return ArabicMonths[month.Value - 1];
        }

        /// <summary>يعرض نص حالة الفترة المالية.</summary>
        protected string GetStatusName(int? status)
        {
            if (!status.HasValue) return "—";

            switch ((PayrollPeriodStatus)status.Value)
            {
                case PayrollPeriodStatus.Open: return "مفتوحة";
                case PayrollPeriodStatus.UnderReview: return "قيد المراجعة";
                case PayrollPeriodStatus.Approved: return "معتمدة";
                case PayrollPeriodStatus.Closed: return "مغلقة";
                default: return "—";
            }
        }

        /// <summary>يعرض كلاس CSS للحالة.</summary>
        protected string GetStatusClass(int? status)
        {
            if (!status.HasValue) return "badge-blue";

            switch ((PayrollPeriodStatus)status.Value)
            {
                case PayrollPeriodStatus.Open: return "badge-blue";
                case PayrollPeriodStatus.UnderReview: return "badge-yellow";
                case PayrollPeriodStatus.Approved: return "badge-green";
                case PayrollPeriodStatus.Closed: return "badge-red";
                default: return "badge-blue";
            }
        }

        /// <summary>يعرض إجمالي صافي المستحقات للفترة.</summary>
        protected string GetPeriodTotal(int? periodId)
        {
            if (!periodId.HasValue || periodId.Value <= 0)
                return "0.00";

            decimal total = db.TeacherPayrolls
                .Where(tp => tp.PayrollPeriodId == periodId.Value)
                .Sum(tp => (decimal?)tp.NetAmount)
                .GetValueOrDefault();

            return total.ToString("N2", CultureInfo.InvariantCulture);
        }

        #endregion

        #region Grid Command

        protected void gvPeriods_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int periodId;
                if (!int.TryParse(e.CommandArgument as string, out periodId) || periodId <= 0)
                {
                    ShowMessage("معرّف الفترة غير صالح.", false);
                    return;
                }

                var period = db.PayrollPeriods.FirstOrDefault(p => p.Id == periodId);
                if (period == null)
                {
                    ShowMessage("الفترة المالية غير موجودة.", false);
                    return;
                }

                switch (e.CommandName)
                {
                    case "Check":
                        RunReadinessCheck(period);
                        break;

                    case "Calc":
                        CalculatePeriod(period);
                        break;

                    case "Approve":
                        ApprovePeriod(period);
                        break;

                    case "Close":
                        ClosePeriod(period);
                        break;

                    default:
                        ShowMessage("إجراء غير معروف.", false);
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "Periods.gvPeriods_RowCommand");
            }
        }

        #endregion

        #region Check (Readiness)

        private void RunReadinessCheck(PayrollPeriod period)
        {
            if (pnlReadiness != null)
                pnlReadiness.Visible = true;

            // فحص واحد موحّد: نفس مصدر الجاهزية الذي تعتمده بوابة الحساب
            // (TeacherAttendanceFinancialService.ValidatePeriod) — يشمل تغطية الجدول → DailyLessons،
            // سجلات الحضور، الاعتماد، التعويضيات، والتعارضات المالية لكل معلمي الحصة.
            var validation = new TeacherAttendanceFinancialService(db)
                .ValidatePeriod(period.Id);

            var rows = (validation.Issues ?? new List<TeacherAttendanceFinancialService.ValidationIssue>())
                .Where(i => i != null)
                .Select(i => new
                {
                    Severity = i.IsError ? "خطأ" : "تنبيه",
                    DateText = i.Date.HasValue
                        ? i.Date.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        : "",
                    TeacherName = i.TeacherName ?? "",
                    PeriodNumber = i.PeriodNumber,
                    Message = i.Message ?? ""
                })
                .ToList();

            int errorCount = rows.Count(r => r.Severity == "خطأ");
            int warnCount = rows.Count - errorCount;

            string badge = validation.IsReady
                ? "<span class='badge-green'><i class='fas fa-check-circle'></i> جاهزة للحساب</span>"
                : "<span class='badge-red'><i class='fas fa-times-circle'></i> غير جاهزة</span>";

            if (lblReadinessInfo != null)
            {
                lblReadinessInfo.Text =
                    badge
                    + " &nbsp; <span class='stat red'>أخطاء: " +
                      errorCount.ToString(CultureInfo.InvariantCulture) + "</span>"
                    + " &nbsp; <span class='stat orange'>تنبيهات: " +
                      warnCount.ToString(CultureInfo.InvariantCulture) + "</span>"
                    + " &nbsp; <span class='stat blue'>" + Server.HtmlEncode(validation.Summary) + "</span>";
            }

            if (gvIssues != null)
            {
                gvIssues.DataSource = rows;
                gvIssues.DataBind();
            }

            if (pnlIssuesEmpty != null)
                pnlIssuesEmpty.Visible = rows.Count == 0;

            ShowMessage(
                validation.IsReady
                    ? "الفحص اكتمل — الفترة جاهزة للحساب المالي."
                    : "الفحص اكتمل — يوجد " + errorCount.ToString(CultureInfo.InvariantCulture) +
                      " خطأ يجب معالجته قبل الحساب (" + validation.Summary + ").",
                errorCount == 0);
        }

        #endregion

        #region Calculate

        private void CalculatePeriod(PayrollPeriod period)
        {
            if (period.Status == PayrollPeriodStatus.Closed)
            {
                ShowMessage("لا يمكن حساب فترة مالية مغلقة.", false);
                return;
            }

            try { db.Database.CommandTimeout = 300; } catch { }

            var service = new PayrollCalculationService(db);
            var result = service.CalculatePeriod(period.Id, EnumHelper.CurrentUserName);

            if (result.InvalidPeriod)
            {
                ShowMessage("الفترة المالية غير موجودة.", false);
                return;
            }

            string message = string.Format(
                "✅ تم الحساب بنجاح في {0:0.00} ثانية — " +
                "عدد المعلمين: {1} — عدد الحصص: {2} — " +
                "الإجمالي المستحق: {3} د.ل — الإجمالي المخصوم: {4} د.ل",
                result.ElapsedSeconds,
                result.TeachersProcessed,
                result.LessonsCounted,
                result.TotalEarned.ToString("N2", CultureInfo.InvariantCulture),
                result.TotalDeducted.ToString("N2", CultureInfo.InvariantCulture));

            ShowMessage(message, true);
            BindPeriods();
        }

        #endregion

        #region Approve / Close

        private void ApprovePeriod(PayrollPeriod period)
        {
            if (period.Status == PayrollPeriodStatus.Closed)
            {
                ShowMessage("الفترة مغلقة بالفعل.", false);
                return;
            }

            if (period.Status == PayrollPeriodStatus.Approved)
            {
                ShowMessage("الفترة معتمدة بالفعل.", false);
                return;
            }

            period.Status = PayrollPeriodStatus.Approved;
            db.SaveChanges();

            ShowMessage("تم اعتماد الفترة المالية بنجاح.", true);
            BindPeriods();
        }

        private void ClosePeriod(PayrollPeriod period)
        {
            if (period.Status == PayrollPeriodStatus.Closed)
            {
                ShowMessage("الفترة مغلقة بالفعل.", false);
                return;
            }

            if (period.Status != PayrollPeriodStatus.Approved)
            {
                ShowMessage("لا يمكن إغلاق فترة غير معتمدة. اعتمد الفترة أولاً.", false);
                return;
            }

            period.Status = PayrollPeriodStatus.Closed;
            db.SaveChanges();

            ShowMessage("تم إغلاق الفترة المالية نهائياً.", true);
            BindPeriods();
        }

        #endregion

        #region Sync

        /// <summary>
        /// مزامنة الحصص للفترات المفتوحة: توليد/تحديث DailyLessons من الجدول المعتمد.
        /// </summary>
        protected void btnSyncAll_Click(object sender, EventArgs e)
        {
            try
            {
                var openPeriods = db.PayrollPeriods
                    .Where(p => p.Status == PayrollPeriodStatus.Open ||
                                p.Status == PayrollPeriodStatus.UnderReview)
                    .ToList();

                if (openPeriods.Count == 0)
                {
                    ShowMessage("لا توجد فترات مفتوحة للمزامنة.", false);
                    return;
                }

                try { db.Database.CommandTimeout = 300; } catch { }

                var service = new DailyLessonService(db);
                int totalAdded = 0;
                int totalUpdated = 0;
                int totalSkipped = 0;
                int totalWithAttendance = 0;

                foreach (var period in openPeriods)
                {
                    var result = service.GenerateForRange(period.StartDate, period.EndDate);
                    totalAdded += result.Added;
                    totalUpdated += result.Updated;
                    totalSkipped += result.SkippedUnchanged;
                    totalWithAttendance += result.SkippedWithAttendance;
                }

                ShowMessage(
                    "تمت مزامنة " + openPeriods.Count.ToString(CultureInfo.InvariantCulture) +
                    " فترة — أُضيفت " + totalAdded.ToString(CultureInfo.InvariantCulture) +
                    " حصة، عُدّلت " + totalUpdated.ToString(CultureInfo.InvariantCulture) +
                    "، وحُفظت " + totalSkipped.ToString(CultureInfo.InvariantCulture) +
                    " دون تغيير" +
                    (totalWithAttendance > 0
                        ? "، و" + totalWithAttendance.ToString(CultureInfo.InvariantCulture) +
                          " حصة لأنها تحتوي على سجل حضور سابق"
                        : "") + ".",
                    true);

                BindPeriods();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Periods.btnSyncAll_Click");
            }
        }

        #endregion

        #region Messages / Error

        private void ShowMessage(string text, bool success)
        {
            if (lblMessage == null) return;

            lblMessage.CssClass = success ? "ta-alert success" : "ta-alert danger";
            lblMessage.Text =
                "<i class='fas " + (success ? "fa-check-circle" : "fa-exclamation-triangle") + "'></i>" +
                "<span>" + Server.HtmlEncode(text) + "</span>";
        }

        private void HandleException(Exception ex, string source)
        {
            try { AppErrorLogger.Log(ex, source); }
            catch { }

            string userMessage = "حدث خطأ غير متوقع أثناء تنفيذ العملية. حاول مرة أخرى.";

            if (ex is InvalidOperationException)
                userMessage = ex.Message;
            else if (ex is System.Data.Entity.Core.EntityException)
                userMessage = "تعذر الاتصال بقاعدة البيانات أو انتهت المهلة. حاول مرة أخرى.";
            else
            {
                try
                {
                    string mapped = ErrorMessageHelper.GetUserMessage(ex, source);
                    if (!string.IsNullOrWhiteSpace(mapped))
                        userMessage = mapped;
                }
                catch { }
            }

            ShowMessage(userMessage, false);
        }

        #endregion

        #region Dispose

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) { try { db.Dispose(); } catch { } }
            base.OnUnload(e);
        }

        #endregion
    }
}