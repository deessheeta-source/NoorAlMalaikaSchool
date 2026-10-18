using System;
using System.Data.Entity;
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
    public partial class TeachersFinances : Page
    {
        private SchoolDbContext db;

        private static readonly string[] ArabicMonthNames =
        {
            "", "يناير", "فبراير", "مارس", "أبريل",
            "مايو", "يونيو", "يوليو", "أغسطس",
            "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
        };

        private int _ownTeacherId;
        private bool _ownTeacherResolved;

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                db = new SchoolDbContext();

                SecurityHelper.DenyUnlessPath();

                var stage = EnumHelper.GetCurrentStage();
                lblStage.Text = "مرحلة النظام الحالية: " + (stage == EducationStage.Basic
                    ? "الأساسية"
                    : "الثانوية");

                if (!IsPostBack)
                {
                    LoadPeriods();
                    LoadStageFilter();
                }

                LoadFinances();
            }
            catch (Exception ex)
            {
                HandleException(ex, "TeachersFinances.Page_Load");
            }
        }

        #endregion

        #region Selectors

        private void LoadPeriods()
        {
            var periods = db.PayrollPeriods
                .AsNoTracking()
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToList();

            ddlPeriod.Items.Clear();
            ddlPeriod.Items.Add(new ListItem("-- اختر الفترة --", "0"));

            foreach (var period in periods)
            {
                ddlPeriod.Items.Add(new ListItem(
                    PeriodText(period) + " — " + StatusName(period.Status),
                    period.Id.ToString(CultureInfo.InvariantCulture)));
            }

            if (ddlPeriod.Items.Count > 1)
                ddlPeriod.SelectedIndex = 1;
        }

        private void LoadStageFilter()
        {
            if (ddlStageFilter == null) return;

            ddlStageFilter.ClearSelection();

            ListItem all = ddlStageFilter.Items.FindByValue("all");
            if (all != null) all.Selected = true;
        }

        private static string PeriodText(PayrollPeriod period)
        {
            if (period == null) return "";
            if (period.Month < 1 || period.Month > 12)
                return period.Year.ToString(CultureInfo.InvariantCulture);
            return ArabicMonthNames[period.Month] + " " +
                   period.Year.ToString(CultureInfo.InvariantCulture);
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvFinances.PageIndex = 0;
            LoadFinances();
        }

        protected void ddlStageFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvFinances.PageIndex = 0;
            LoadFinances();
        }

        #endregion

        #region Data

        private void LoadFinances()
        {
            int periodId = SelectedPeriodId();

            if (periodId <= 0)
            {
                SetKpis(0, 0m, 0m, 0m, 0m);
                gvFinances.DataSource = null;
                gvFinances.DataBind();
                lblRowsCount.Text = "0";
                ShowEmpty(true);
                SetInfo("اختر الفترة المالية المطلوبة لعرض التفاصيل.");
                return;
            }

            var period = db.PayrollPeriods
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == periodId);

            if (period == null)
            {
                SetKpis(0, 0m, 0m, 0m, 0m);
                gvFinances.DataSource = null;
                gvFinances.DataBind();
                lblRowsCount.Text = "0";
                ShowEmpty(true);
                SetInfo("الفترة المالية غير موجودة.");
                return;
            }

            int ownTeacherId = ResolveOwnTeacherId();
            if (SecurityHelper.CurrentRole == UserRole.Teacher && ownTeacherId <= 0)
            {
                SetKpis(0, 0m, 0m, 0m, 0m);
                gvFinances.DataSource = null;
                gvFinances.DataBind();
                lblRowsCount.Text = "0";
                ShowEmpty(true);
                SetInfo("لا يوجد سجل معلم مرتبط بحسابك. راجع إدارة النظام لربط رقمك التعريفي بسجلك.");
                return;
            }

            var teachers = db.Teachers
                .AsNoTracking()
                .Where(t => t.IsActive)
                .ToList();

            EducationStage? stage = SelectedStage();

            bool isTeacherView = SecurityHelper.CurrentRole == UserRole.Teacher;

            if (stage.HasValue && !isTeacherView)
                teachers = teachers.Where(t => t.Stage == stage.Value).ToList();

            if (ownTeacherId > 0)
                teachers = teachers.Where(t => t.Id == ownTeacherId).ToList();

            var payrolls = db.TeacherPayrolls
                .AsNoTracking()
                .Where(p => p.PayrollPeriodId == periodId)
                .ToList()
                .GroupBy(p => p.TeacherId)
                .ToDictionary(g => g.Key, g => g.First());

            var rows = teachers
                .OrderBy(t => t.Name)
                .Select(t =>
                {
                    TeacherPayroll payroll;
                    payrolls.TryGetValue(t.Id, out payroll);

                    decimal paidHours = payroll == null ? 0m : Math.Round(payroll.TotalMinutes / 60m, 2);
                    decimal earnedAmount = payroll == null ? 0m : payroll.EarnedAmount;
                    decimal deductionAmount = payroll == null ? 0m : payroll.DeductionAmount;
                    decimal adjustmentAmount = payroll == null ? 0m : payroll.AdjustmentAmount;
                    decimal netAmount = payroll == null ? 0m : payroll.NetAmount;
                    bool isCalculated = payroll != null;
                    int payrollId = payroll == null ? 0 : payroll.Id;

                    string statusClass = isCalculated
                        ? "status-indicator green"
                        : "status-indicator gray";
                    string statusText = isCalculated ? "محسوب" : "لم يُحسب";

                    string actionClass = payrollId > 0
                        ? "action-btn primary"
                        : "action-btn disabled";

                    string statementUrl = payrollId > 0
                        ? ResolveClientUrl("~/Pages/Payroll/Statement.aspx") +
                          "?payrollId=" + payrollId.ToString(CultureInfo.InvariantCulture)
                        : ResolveClientUrl("~/Pages/Payroll/Statement.aspx");

                    return new
                    {
                        PayrollId = payrollId,
                        TeacherId = t.Id,
                        TeacherName = t.Name ?? "",
                        EmployeeNoText = string.IsNullOrWhiteSpace(t.EmployeeNo)
                            ? "بدون رقم تعريفي"
                            : "رقم تعريفي: " + t.EmployeeNo,
                        EmploymentText = EmploymentDisplayText(t.EmploymentType),
                        PaidHours = paidHours,
                        EarnedAmountText = earnedAmount.ToString("N2", CultureInfo.InvariantCulture),
                        DeductionAmountText = deductionAmount.ToString("N2", CultureInfo.InvariantCulture),
                        AdjustmentAmountText = adjustmentAmount.ToString("N2", CultureInfo.InvariantCulture),
                        NetAmountText = netAmount.ToString("N2", CultureInfo.InvariantCulture),
                        StatusClass = statusClass,
                        StatusText = statusText,
                        ActionClass = actionClass,
                        StatementUrl = statementUrl,

                        // قيم خام لحساب الـ KPIs
                        PaidHoursRaw = paidHours,
                        EarnedAmountRaw = earnedAmount,
                        DeductionAmountRaw = deductionAmount,
                        NetAmountRaw = netAmount,
                        IsCalculated = isCalculated
                    };
                })
                .ToList();

            gvFinances.DataSource = rows;
            gvFinances.DataBind();

            lblRowsCount.Text = rows.Count.ToString(CultureInfo.InvariantCulture);

            SetKpis(
                rows.Count,
                rows.Sum(r => r.PaidHoursRaw),
                rows.Sum(r => r.EarnedAmountRaw),
                rows.Sum(r => r.DeductionAmountRaw),
                rows.Sum(r => r.NetAmountRaw));

            ShowEmpty(rows.Count == 0);

            if (rows.Count == 0)
            {
                SetInfo("لا يوجد معلمون نشطون مطابقون للفلترة المحددة في هذه الفترة.");
                return;
            }

            int calculated = rows.Count(r => r.IsCalculated);
            string stageText = stage.HasValue
                ? (stage.Value == EducationStage.Basic ? "المرحلة الأساسية" : "المرحلة الثانوية")
                : "كل المراحل";

            string message = string.Format(
                "الفترة: {0} — الحالة: {1} — المرحلة: {2} — تم حساب {3} من {4} معلم/ة.",
                PeriodText(period),
                StatusName(period.Status),
                stageText,
                calculated,
                rows.Count);

            if (SecurityHelper.CurrentRole == UserRole.Teacher)
                message = "تعرض هذه الصفحة تفاصيل راتبك أنت فقط. " + message;

            if (period.Status == PayrollPeriodStatus.Closed)
                message += " الفترة مغلقة نهائياً ولا تقبل أي تعديلات.";

            SetInfo(message);
        }

        private int SelectedPeriodId()
        {
            int value;
            if (ddlPeriod != null &&
                int.TryParse(ddlPeriod.SelectedValue, out value) &&
                value > 0)
            {
                return value;
            }

            return 0;
        }

        private EducationStage? SelectedStage()
        {
            if (ddlStageFilter == null) return null;

            string value = ddlStageFilter.SelectedValue;
            if (string.IsNullOrEmpty(value) ||
                value.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            EducationStage stage;
            if (Enum.TryParse<EducationStage>(value, true, out stage))
                return stage;

            return null;
        }

        private int ResolveOwnTeacherId()
        {
            if (_ownTeacherResolved) return _ownTeacherId;

            _ownTeacherResolved = true;
            _ownTeacherId = 0;

            if (SecurityHelper.CurrentRole != UserRole.Teacher) return 0;

            string employeeNo = SecurityHelper.CurrentEmployeeNo;
            if (string.IsNullOrWhiteSpace(employeeNo)) return 0;

            _ownTeacherId = db.Teachers
                .AsNoTracking()
                .Where(t => t.EmployeeNo == employeeNo)
                .Select(t => t.Id)
                .FirstOrDefault();

            return _ownTeacherId;
        }

        #endregion

        #region Rendering Helpers

        private void SetKpis(
            int teachersCount,
            decimal totalHours,
            decimal totalEarned,
            decimal totalDeductions,
            decimal totalNet)
        {
            lblTeachersCount.Text = teachersCount.ToString(CultureInfo.InvariantCulture);
            lblTotalHours.Text = totalHours.ToString("0.##", CultureInfo.InvariantCulture);
            lblTotalEarned.Text = totalEarned.ToString("N2", CultureInfo.InvariantCulture);
            lblTotalDeductions.Text = totalDeductions.ToString("N2", CultureInfo.InvariantCulture);
            lblTotalNet.Text = totalNet.ToString("N2", CultureInfo.InvariantCulture);
        }

        private void ShowEmpty(bool visible)
        {
            if (pnlEmpty == null) return;
            pnlEmpty.Visible = visible;
        }

        private void SetInfo(string text)
        {
            if (lblInfo != null) lblInfo.Text = Server.HtmlEncode(text ?? "");
        }

        /// <summary>
        /// نص نوع التعاقد المعروض في هذه الصفحة فقط،
        /// دون التأثير على باقي صفحات النظام.
        /// </summary>
        private static string EmploymentDisplayText(EmploymentType type)
        {
            switch (type)
            {
                case EmploymentType.Permanent:
                    return "معلم دائم - راتب شهري";
                case EmploymentType.PermanentSupervisor:
                    return "مشرف دائم - راتب شهري";
                case EmploymentType.PartTime:
                    return "أجر بالحصص";
                default:
                    return type.GetDisplayName();
            }
        }

        /// <summary>«—» بدل صفر مضلّل.</summary>
        private static string MoneyOrDashInternal(decimal? value)
        {
            if (!value.HasValue || value.Value <= 0m) return "—";
            return value.Value.ToString("N2", CultureInfo.InvariantCulture);
        }

        private static string HoursOrDashInternal(decimal value)
        {
            if (value <= 0m) return "—";
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public string StatusName(PayrollPeriodStatus status)
        {
            switch (status)
            {
                case PayrollPeriodStatus.Open: return "مفتوحة";
                case PayrollPeriodStatus.UnderReview: return "قيد المراجعة";
                case PayrollPeriodStatus.Approved: return "معتمدة";
                case PayrollPeriodStatus.Closed: return "مغلقة";
                default: return "";
            }
        }

        #endregion

        #region Paging / Messages / Dispose

        protected void gvFinances_PageIndexChanging(
            object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvFinances.PageIndex = e.NewPageIndex;
                LoadFinances();
            }
            catch (Exception ex)
            {
                HandleException(ex, "TeachersFinances.gvFinances_PageIndexChanging");
            }
        }

        private void HandleException(Exception ex, string source)
        {
            try { AppErrorLogger.Log(ex, source); }
            catch { }

            if (lblMessage != null)
            {
                lblMessage.CssClass = "ta-alert danger";

                string userMessage = "حدث خطأ غير متوقع أثناء تنفيذ العملية. حاول مرة أخرى.";
                try
                {
                    string mapped = ErrorMessageHelper.GetUserMessage(ex, source);
                    if (!string.IsNullOrWhiteSpace(mapped))
                        userMessage = mapped;
                }
                catch { }

                lblMessage.Text =
                    "<i class='fas fa-exclamation-triangle'></i><span>" +
                    Server.HtmlEncode(userMessage) +
                    "</span>";
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null)
            {
                try { db.Dispose(); } catch { }
            }
            base.OnUnload(e);
        }

        #endregion
    }
}