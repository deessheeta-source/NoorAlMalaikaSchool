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

namespace NoorAlMalaikaSchool.Pages.Payroll
{
    public partial class Details : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private static readonly string[] ArabicMonths =
        {
            "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
            "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
        };

        private int _periodId;
        private bool _periodIdResolved;

        private EmploymentType? _selectedEmploymentType;
        private bool _employmentTypeResolved;

        public int PeriodId
        {
            get
            {
                if (_periodIdResolved) return _periodId;

                if (ddlPeriod != null && ddlPeriod.SelectedValue != null)
                {
                    int selected;
                    if (int.TryParse(ddlPeriod.SelectedValue, out selected) && selected > 0)
                    {
                        _periodId = selected;
                        _periodIdResolved = true;
                        return _periodId;
                    }
                }

                int v;
                _periodId = int.TryParse(Request["id"], out v) ? v : 0;
                _periodIdResolved = true;
                return _periodId;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SecurityHelper.DenyUnlessPath();

                if (!IsPostBack)
                {
                    if (LoadPeriods())
                        return;

                    LoadStageFilter();
                    LoadEmploymentFilter();
                }

                LoadPage();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.Details.Page_Load");
            }
        }

        #region Selectors

        private bool LoadPeriods()
        {
            var periods = db.PayrollPeriods
                .AsNoTracking()
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToList();

            ddlPeriod.Items.Clear();
            ddlPeriod.Items.Add(new ListItem("-- اختر الفترة --", "0"));

            foreach (var p in periods)
            {
                int idx = (p.Month >= 1 && p.Month <= 12) ? p.Month - 1 : 0;
                ddlPeriod.Items.Add(new ListItem(
                    ArabicMonths[idx] + " " + p.Year + " — " + StatusName(p.Status),
                    p.Id.ToString(CultureInfo.InvariantCulture)));
            }

            string current = PeriodId.ToString(CultureInfo.InvariantCulture);
            ListItem item = ddlPeriod.Items.FindByValue(current);

            if (item != null)
            {
                ddlPeriod.ClearSelection();
                item.Selected = true;
                return false;
            }

            if (ddlPeriod.Items.Count > 1)
            {
                ddlPeriod.SelectedIndex = 1;

                int firstId;
                if (int.TryParse(ddlPeriod.Items[1].Value, out firstId) && firstId > 0)
                {
                    Response.Redirect(
                        "Details.aspx?id=" + firstId.ToString(CultureInfo.InvariantCulture) +
                        "&stage=" + (EnumHelper.GetCurrentStage() == EducationStage.Basic ? "Basic" : "Secondary"),
                        false);
                    System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return true;
                }
            }

            return false;
        }

        private void LoadStageFilter()
        {
            if (ddlStageFilter == null) return;

            ddlStageFilter.ClearSelection();

            ListItem all = ddlStageFilter.Items.FindByValue("all");
            if (all != null) all.Selected = true;
        }

        private void LoadEmploymentFilter()
        {
            if (ddlEmploymentFilter == null) return;

            ddlEmploymentFilter.ClearSelection();

            ListItem all = ddlEmploymentFilter.Items.FindByValue("all");
            if (all != null) all.Selected = true;
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected;
            if (int.TryParse(ddlPeriod.SelectedValue, out selected) && selected > 0)
            {
                string stageParam = EnumHelper.GetCurrentStage() == EducationStage.Basic
                    ? "Basic" : "Secondary";

                Response.Redirect(
                    "Details.aspx?id=" + selected.ToString(CultureInfo.InvariantCulture) +
                    "&stage=" + stageParam);
            }
        }

        protected void ddlStageFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            _periodIdResolved = false;
            gvPayroll.PageIndex = 0;
            LoadPage();
        }

        protected void ddlEmploymentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            _employmentTypeResolved = false;
            gvPayroll.PageIndex = 0;
            LoadPage();
        }

        private EducationStage? SelectedStage()
        {
            if (ddlStageFilter == null) return null;

            string value = ddlStageFilter.SelectedValue;
            if (string.IsNullOrEmpty(value) ||
                value.Equals("all", StringComparison.OrdinalIgnoreCase))
                return null;

            EducationStage stage;
            if (Enum.TryParse(value, true, out stage))
                return stage;

            return null;
        }

        private EmploymentType? SelectedEmploymentType()
        {
            if (_employmentTypeResolved) return _selectedEmploymentType;

            _employmentTypeResolved = true;

            if (ddlEmploymentFilter == null)
            {
                _selectedEmploymentType = null;
                return null;
            }

            string value = ddlEmploymentFilter.SelectedValue;
            if (string.IsNullOrEmpty(value) ||
                value.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                _selectedEmploymentType = null;
                return null;
            }

            EmploymentType type;
            if (Enum.TryParse(value, true, out type))
            {
                _selectedEmploymentType = type;
                return type;
            }

            _selectedEmploymentType = null;
            return null;
        }

        #endregion

        #region Page Loading

        private void LoadPage()
        {
            if (PeriodId <= 0)
            {
                lblTotalSalary.Text = "0.00";
                lblLessonsSalary.Text = "0.00";
                lblTotalAdjustments.Text = "0.00";
                SetInfoLine("لم يتم تحديد رقم الفترة المالية.");
                SetInfo("لم يتم تحديد رقم الفترة المالية.", "danger");
                return;
            }

            var period = db.PayrollPeriods.AsNoTracking().FirstOrDefault(p => p.Id == PeriodId);
            if (period == null)
            {
                lblTotalSalary.Text = "0.00";
                lblLessonsSalary.Text = "0.00";
                lblTotalAdjustments.Text = "0.00";
                SetInfoLine("الفترة المالية غير موجودة.");
                SetInfo("الفترة المالية غير موجودة.", "danger");
                return;
            }

            int idx = (period.Month >= 1 && period.Month <= 12) ? period.Month - 1 : 0;
            string periodText = ArabicMonths[idx] + " " + period.Year;

            lblPeriodTitle.Text = Server.HtmlEncode(periodText);

            string statusText;
            string statusClass;

            switch (period.Status)
            {
                case PayrollPeriodStatus.Open: statusText = "مفتوح"; statusClass = "badge-blue"; break;
                case PayrollPeriodStatus.UnderReview: statusText = "قيد المراجعة"; statusClass = "badge-yellow"; break;
                case PayrollPeriodStatus.Approved: statusText = "معتمد"; statusClass = "badge-green"; break;
                case PayrollPeriodStatus.Closed: statusText = "مغلق"; statusClass = "badge-red"; break;
                default: statusText = "—"; statusClass = ""; break;
            }

            lblStatus.Text = Server.HtmlEncode(statusText);
            lblStatus.CssClass = "tf-hero-chip " + statusClass;

            bool isClosed = period.Status == PayrollPeriodStatus.Closed;
            divItems.Visible = !isClosed;

            LoadSummary();
            LoadPayroll();
            LoadTeachersDropDown();

            EducationStage? stage = SelectedStage();
            string stageText = stage.HasValue
                ? (stage.Value == EducationStage.Basic ? "المرحلة الأساسية" : "المرحلة الثانوية")
                : "كل المراحل";

            EmploymentType? empType = SelectedEmploymentType();
            string empText = empType.HasValue
                ? EmploymentDisplayText(empType.Value)
                : "كل الأنواع";

            SetInfoLine("الفترة: " + periodText + " — الحالة: " + statusText +
                        " — المرحلة: " + stageText +
                        " — نوع التعاقد: " + empText +
                        " — عدد الصفوف: " + lblRowsCount.Text);
        }

        #endregion

        #region Data

        private void LoadSummary()
        {
            EmploymentType? employmentFilter = SelectedEmploymentType();
            EducationStage? stageFilter = SelectedStage();

            var query = from p in db.TeacherPayrolls.AsNoTracking()
                        join t in db.Teachers.AsNoTracking() on p.TeacherId equals t.Id
                        where p.PayrollPeriodId == PeriodId
                        select new
                        {
                            p.BaseSalaryApplied,
                            p.EarnedAmount,
                            p.NetAmount,
                            p.DeductionAmount,
                            p.AdjustmentAmount,
                            t.EmploymentType,
                            t.Stage
                        };

            if (employmentFilter.HasValue)
                query = query.Where(x => x.EmploymentType == employmentFilter.Value);

            if (stageFilter.HasValue)
                query = query.Where(x => x.Stage == stageFilter.Value);

            var payrollsWithTeachers = query.ToList();

            lblTeacherCount.Text = payrollsWithTeachers.Count.ToString(CultureInfo.InvariantCulture);

            // ⭐ إجمالي مرتب الحصص = مجموع EarnedAmount لمعلمي الحصة فقط
            decimal lessonsSalary = payrollsWithTeachers
                .Where(x => x.EmploymentType == EmploymentType.PerLesson)
                .Sum(x => (decimal?)x.EarnedAmount)
                .GetValueOrDefault();
            lblLessonsSalary.Text = lessonsSalary.ToString("N2", CultureInfo.InvariantCulture);

            // إجمالي المرتبات الشهرية (الدائمون والمشرفون)
            decimal monthlySalaries = payrollsWithTeachers
                .Where(x => x.EmploymentType == EmploymentType.Permanent ||
                            x.EmploymentType == EmploymentType.PermanentSupervisor)
                .Sum(x => (decimal?)x.EarnedAmount)
                .GetValueOrDefault();
            lblTotalSalary.Text = monthlySalaries.ToString("N2", CultureInfo.InvariantCulture);

            // إجمالي المكافآت والتسويات
            decimal totalAdjustments = payrollsWithTeachers
                .Sum(x => (decimal?)x.AdjustmentAmount)
                .GetValueOrDefault();
            lblTotalAdjustments.Text = totalAdjustments.ToString("N2", CultureInfo.InvariantCulture);

            // إجمالي الخصومات
            decimal totalDeductions = payrollsWithTeachers
                .Sum(x => (decimal?)x.DeductionAmount)
                .GetValueOrDefault();
            lblTotalDeductions.Text = totalDeductions.ToString("N2", CultureInfo.InvariantCulture);

            // إجمالي صافي المرتبات = مجموع الصافي (بعد كل الخصومات والمكافآت)
            decimal grandNetTotal = payrollsWithTeachers
                .Sum(x => (decimal?)x.NetAmount)
                .GetValueOrDefault();
            lblTotalNet.Text = grandNetTotal.ToString("N2", CultureInfo.InvariantCulture);
        }

        private void LoadPayroll()
        {
            EducationStage? stage = SelectedStage();
            EmploymentType? employmentFilter = SelectedEmploymentType();

            var query = from p in db.TeacherPayrolls.AsNoTracking()
                        join t in db.Teachers.AsNoTracking() on p.TeacherId equals t.Id
                        where p.PayrollPeriodId == PeriodId
                        select new { p, t };

            if (stage.HasValue)
                query = query.Where(x => x.t.Stage == stage.Value);

            if (employmentFilter.HasValue)
                query = query.Where(x => x.t.EmploymentType == employmentFilter.Value);

            var raw = query
                .OrderBy(x => x.t.Name)
                .Select(x => new
                {
                    PayrollId = x.p.Id,
                    TeacherId = x.p.TeacherId,
                    TeacherName = x.t.Name,
                    EmployeeNo = x.t.EmployeeNo,
                    EmploymentType = x.t.EmploymentType,
                    x.p.EarnedAmount,
                    x.p.DeductionAmount,
                    x.p.AdjustmentAmount,
                    x.p.NetAmount
                })
                .ToList();

            // ⭐ خريطة مبالغ التعويضات لكل استحقاق
            //    تُجمع من أسطر الحصص التي حالتها SubstitutionFull أو SubstitutionPartial
            var payrollIds = raw.Select(r => r.PayrollId).ToList();

            Dictionary<int, decimal> substitutionMap = new Dictionary<int, decimal>();

            if (payrollIds.Count > 0)
            {
                int subFull = (int)AttendanceStatus.SubstitutionFull;
                int subPartial = (int)AttendanceStatus.SubstitutionPartial;

                var subRows = db.PayrollLessonLines
                    .AsNoTracking()
                    .Where(l => payrollIds.Contains(l.TeacherPayrollId)
                                && (l.Status == subFull || l.Status == subPartial))
                    .GroupBy(l => l.TeacherPayrollId)
                    .Select(g => new
                    {
                        PayrollId = g.Key,
                        Amount = g.Sum(x => x.EarnedAmount)
                    })
                    .ToList();

                foreach (var r in subRows)
                    substitutionMap[r.PayrollId] = r.Amount;
            }

            var rows = raw.Select(x =>
            {
                decimal substitutionAmount = substitutionMap.ContainsKey(x.PayrollId)
                    ? substitutionMap[x.PayrollId]
                    : 0m;

                // ⭐ المستحق الأساسي = إجمالي المستحق − التعويضات
                //    (التعويضات تُعرض في عمودها المستقل، والمجموع يبقى كما هو)
                decimal baseEarned = x.EarnedAmount - substitutionAmount;
                if (baseEarned < 0m) baseEarned = 0m;

                bool hasSubstitution = substitutionAmount > 0m;

                return new
                {
                    Id = x.PayrollId,
                    TeacherId = x.TeacherId,
                    TeacherName = x.TeacherName ?? "",
                    Initials = BuildInitials(x.TeacherName),
                    EmployeeNoText = string.IsNullOrWhiteSpace(x.EmployeeNo)
                        ? "بدون رقم تعريفي"
                        : "رقم تعريفي: " + x.EmployeeNo,
                    EmploymentText = EmploymentDisplayText(x.EmploymentType),
                    EarnedAmountText = baseEarned.ToString("N2", CultureInfo.InvariantCulture),
                    DeductionAmountText = x.DeductionAmount.ToString("N2", CultureInfo.InvariantCulture),
                    SubstitutionAmountText = hasSubstitution
                        ? "+" + substitutionAmount.ToString("N2", CultureInfo.InvariantCulture)
                        : "—",
                    SubstitutionAmountClass = hasSubstitution ? "money pos" : "money neutral",
                    AdjustmentAmountText = x.AdjustmentAmount.ToString("N2", CultureInfo.InvariantCulture),
                    NetAmountText = x.NetAmount.ToString("N2", CultureInfo.InvariantCulture),
                    StatusClass = "status-indicator green",
                    StatusText = "محسوب",
                    ActionClass = "action-btn primary",
                    StatementUrl = (x.EmploymentType == EmploymentType.Permanent ||
                                    x.EmploymentType == EmploymentType.PermanentSupervisor
                        ? "PermanentStatement.aspx?payrollId="
                        : "Statement.aspx?payrollId=") +
                        x.PayrollId.ToString(CultureInfo.InvariantCulture) +
                        "&periodId=" + PeriodId.ToString(CultureInfo.InvariantCulture)
                };
            }).ToList();

            gvPayroll.DataSource = rows;
            gvPayroll.DataBind();

            lblRowsCount.Text = rows.Count.ToString(CultureInfo.InvariantCulture);

            if (pnlEmpty != null)
                pnlEmpty.Visible = rows.Count == 0;
        }

        private void LoadTeachersDropDown()
        {
            EmploymentType? employmentFilter = SelectedEmploymentType();

            var query = from p in db.TeacherPayrolls.AsNoTracking()
                        join t in db.Teachers.AsNoTracking() on p.TeacherId equals t.Id
                        where p.PayrollPeriodId == PeriodId
                        select new { p.TeacherId, t.EmploymentType };

            if (employmentFilter.HasValue)
                query = query.Where(x => x.EmploymentType == employmentFilter.Value);

            var teacherIds = query.Select(x => x.TeacherId).Distinct().ToArray();

            var teachers = db.Teachers
                .AsNoTracking()
                .Where(t => teacherIds.Contains(t.Id))
                .OrderBy(t => t.Name)
                .ToList();

            ddlTeacher.DataSource = teachers;
            ddlTeacher.DataTextField = "Name";
            ddlTeacher.DataValueField = "Id";
            ddlTeacher.DataBind();
            ddlTeacher.Items.Insert(0, new ListItem("-- اختر --", ""));
        }

        #endregion

        #region Events

        protected void gvPayroll_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvPayroll.PageIndex = e.NewPageIndex;
                LoadPayroll();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.Details.gvPayroll_PageIndexChanging");
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                int teacherId;
                decimal amount;
                int itemType;

                if (!int.TryParse(ddlTeacher.SelectedValue, out teacherId) || teacherId <= 0)
                {
                    SetInfo("يرجى اختيار المعلم.", "danger");
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out amount) || amount <= 0)
                {
                    SetInfo("أدخل قيمة صحيحة أكبر من صفر.", "danger");
                    return;
                }

                if (!int.TryParse(ddlItemType.SelectedValue, out itemType))
                {
                    SetInfo("نوع البند غير صالح.", "danger");
                    return;
                }

                var payroll = db.TeacherPayrolls
                    .FirstOrDefault(p => p.PayrollPeriodId == PeriodId && p.TeacherId == teacherId);

                if (payroll == null)
                {
                    SetInfo("لم يُحسب استحقاق هذا المعلم بعد.", "danger");
                    return;
                }

                var item = new PayrollItem
                {
                    TeacherPayrollId = payroll.Id,
                    Type = (PayrollItemType)itemType,
                    Amount = amount,
                    Reason = (txtReason.Text ?? "").Trim(),
                    ApprovedBy = EnumHelper.CurrentUserName,
                    CreatedAt = DateTime.Now
                };

                db.PayrollItems.Add(item);
                db.SaveChanges();

                var service = new PayrollCalculationService(db);
                service.AdjustWithItems(payroll);

                SetInfo("تمت إضافة البند وتحديث الصافي بنجاح.", "success");

                txtAmount.Text = "";
                txtReason.Text = "";

                LoadSummary();
                LoadPayroll();

                upDetails.Update();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.Details.btnAddItem_Click");
            }
        }

        #endregion

        #region Helpers

        private static string BuildInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "؟";

            name = name.Trim();
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) return "؟";
            if (parts.Length == 1) return parts[0].Substring(0, 1);

            return parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1);
        }

        private static string EmploymentDisplayText(EmploymentType type)
        {
            switch (type)
            {
                case EmploymentType.Permanent: return "معلم دائم - راتب شهري";
                case EmploymentType.PermanentSupervisor: return "مشرف دائم - راتب شهري";
                case EmploymentType.PerLesson: return "معلم بالحصة";
                case EmploymentType.PartTime: return "أجر بالحصص";
                default: return type.GetDisplayName();
            }
        }

        private static string StatusName(PayrollPeriodStatus status)
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

        #region Info / Error

        private void SetInfoLine(string message)
        {
            if (lblInfoLine != null)
                lblInfoLine.Text = Server.HtmlEncode(message ?? "");
        }

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