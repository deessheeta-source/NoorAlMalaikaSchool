using System;
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
    public partial class PermanentStaff : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private static readonly string[] ArabicMonths =
        {
            "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
            "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
        };

        private int _periodId;
        private bool _periodIdResolved;

        private EmploymentType? _selectedStaffType;
        private bool _staffTypeResolved;

        public int PeriodId
        {
            get
            {
                if (_periodIdResolved) return _periodId;
                if (ddlPeriod != null && !string.IsNullOrEmpty(ddlPeriod.SelectedValue))
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

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SecurityHelper.DenyUnlessPath();

                if (!IsPostBack)
                {
                    LoadStage();
                    LoadStaffTypeFilter();
                    LoadPeriods();
                    LoadItemTeachers();   // ⭐ تُستدعى مرة واحدة فقط
                }

                LoadPage();
            }
            catch (Exception ex)
            {
                HandleException(ex, "PermanentStaff.Page_Load");
            }
        }

        #endregion

        #region Selectors

        private void LoadStage()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();
            string stageText = stage == EducationStage.Basic
                ? "المرحلة الأساسية"
                : "المرحلة الثانوية";

            if (lblCurrentStage != null)
                lblCurrentStage.Text = stageText;
        }

        private void LoadStaffTypeFilter()
        {
            if (ddlStaffType == null) return;

            ddlStaffType.ClearSelection();
            ListItem all = ddlStaffType.Items.FindByValue("all");
            if (all != null) all.Selected = true;
        }

        private void LoadPeriods()
        {
            var periods = db.PayrollPeriods
                .AsNoTracking()
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToList();

            ddlPeriod.Items.Clear();

            if (periods.Count == 0)
            {
                ddlPeriod.Items.Add(new ListItem("-- لا توجد فترة --", "0"));
                return;
            }

            foreach (var p in periods)
            {
                int idx = (p.Month >= 1 && p.Month <= 12) ? p.Month - 1 : 0;
                string statusName;
                switch (p.Status)
                {
                    case PayrollPeriodStatus.Open: statusName = "مفتوحة"; break;
                    case PayrollPeriodStatus.UnderReview: statusName = "قيد المراجعة"; break;
                    case PayrollPeriodStatus.Approved: statusName = "معتمدة"; break;
                    case PayrollPeriodStatus.Closed: statusName = "مغلقة"; break;
                    default: statusName = ""; break;
                }

                ddlPeriod.Items.Add(new ListItem(
                    ArabicMonths[idx] + " " + p.Year + " — " + statusName,
                    p.Id.ToString(CultureInfo.InvariantCulture)));
            }

            // اختيار الفترة المطلوبة من QueryString أو الأحدث
            string current = PeriodId.ToString(CultureInfo.InvariantCulture);
            ListItem item = ddlPeriod.Items.FindByValue(current);
            if (item != null)
            {
                ddlPeriod.ClearSelection();
                item.Selected = true;
            }
            else
            {
                ddlPeriod.SelectedIndex = 0;
                _periodIdResolved = false;
            }
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            _periodIdResolved = false;
            gvStaff.PageIndex = 0;
            LoadPage();
        }

        protected void ddlStaffType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _staffTypeResolved = false;
            gvStaff.PageIndex = 0;
            LoadPage();
        }

        private EducationStage CurrentStage()
        {
            return EnumHelper.GetCurrentStage();
        }

        private EmploymentType? SelectedStaffType()
        {
            if (_staffTypeResolved) return _selectedStaffType;
            _staffTypeResolved = true;

            if (ddlStaffType == null)
            {
                _selectedStaffType = null;
                return null;
            }

            string value = ddlStaffType.SelectedValue;
            if (string.IsNullOrEmpty(value) ||
                value.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                _selectedStaffType = null;
                return null;
            }

            EmploymentType type;
            if (Enum.TryParse(value, true, out type))
            {
                _selectedStaffType = type;
                return type;
            }

            _selectedStaffType = null;
            return null;
        }

        #endregion

        #region Data Loading

        private void LoadPage()
        {
            if (PeriodId <= 0)
            {
                ClearPage("لا توجد فترات مالية — ابدأ بإنشاء فترة مالية من صفحة الفترات.");
                return;
            }

            LoadSummary();
            LoadStaffGrid();
            // ⭐ تم نقل LoadItemTeachers() إلى Page_Load (مرة واحدة فقط)
        }

        private void ClearPage(string message)
        {
            lblPermanentTeachersCount.Text = "0";
            lblPermanentSupervisorsCount.Text = "0";
            lblMonthlySalaryTotal.Text = "0.00";
            lblTotalNet.Text = "0.00";
            lblRowsCount.Text = "0";

            gvStaff.DataSource = null;
            gvStaff.DataBind();

            pnlEmpty.Visible = true;
            pnlAddItem.Visible = false;

            SetInfoLine(message);
        }

        private void LoadSummary()
        {
            EducationStage stage = CurrentStage();

            var allStaff = db.Teachers
                .AsNoTracking()
                .Where(t => t.Stage == stage
                            && t.IsActive
                            && (t.EmploymentType == EmploymentType.Permanent
                                || t.EmploymentType == EmploymentType.PermanentSupervisor))
                .Select(t => new
                {
                    t.EmploymentType,
                    t.BaseSalary
                })
                .ToList();

            int permTeachers = allStaff.Count(x => x.EmploymentType == EmploymentType.Permanent);
            int permSupervisors = allStaff.Count(x => x.EmploymentType == EmploymentType.PermanentSupervisor);
            decimal totalMonthly = allStaff.Sum(x => (decimal?)x.BaseSalary).GetValueOrDefault();

            lblPermanentTeachersCount.Text = permTeachers.ToString(CultureInfo.InvariantCulture);
            lblPermanentSupervisorsCount.Text = permSupervisors.ToString(CultureInfo.InvariantCulture);
            lblMonthlySalaryTotal.Text = totalMonthly.ToString("N2", CultureInfo.InvariantCulture);

            EmploymentType? staffFilter = SelectedStaffType();

            var payrollQuery = from p in db.TeacherPayrolls.AsNoTracking()
                               join t in db.Teachers.AsNoTracking() on p.TeacherId equals t.Id
                               where p.PayrollPeriodId == PeriodId
                                     && (t.EmploymentType == EmploymentType.Permanent
                                         || t.EmploymentType == EmploymentType.PermanentSupervisor)
                                     && t.Stage == stage
                               select new { p.NetAmount, t.EmploymentType };

            if (staffFilter.HasValue)
                payrollQuery = payrollQuery.Where(x => x.EmploymentType == staffFilter.Value);

            decimal totalNet = payrollQuery
                .Sum(x => (decimal?)x.NetAmount)
                .GetValueOrDefault();

            lblTotalNet.Text = totalNet.ToString("N2", CultureInfo.InvariantCulture);

            var period = db.PayrollPeriods.AsNoTracking().FirstOrDefault(p => p.Id == PeriodId);
            pnlAddItem.Visible = period != null && period.Status != PayrollPeriodStatus.Closed;
        }

        private void LoadStaffGrid()
        {
            EducationStage stage = CurrentStage();
            EmploymentType? staffFilter = SelectedStaffType();

            var query = from p in db.TeacherPayrolls.AsNoTracking()
                        join t in db.Teachers.AsNoTracking() on p.TeacherId equals t.Id
                        where p.PayrollPeriodId == PeriodId
                              && (t.EmploymentType == EmploymentType.Permanent
                                  || t.EmploymentType == EmploymentType.PermanentSupervisor)
                              && t.Stage == stage
                        select new { p, t };

            if (staffFilter.HasValue)
                query = query.Where(x => x.t.EmploymentType == staffFilter.Value);

            var raw = query
                .OrderBy(x => x.t.Name)
                .Select(x => new
                {
                    PayrollId = x.p.Id,
                    TeacherId = x.p.TeacherId,
                    TeacherName = x.t.Name,
                    EmployeeNo = x.t.EmployeeNo,
                    EmploymentType = x.t.EmploymentType,
                    x.p.BaseSalaryApplied,
                    x.p.EarnedAmount,
                    x.p.DeductionAmount,
                    x.p.AdjustmentAmount,
                    x.p.NetAmount
                })
                .ToList();

            var rows = raw.Select(x => new
            {
                Id = x.PayrollId,
                TeacherId = x.TeacherId,
                TeacherName = x.TeacherName ?? "",
                Initials = BuildInitials(x.TeacherName),
                EmployeeNoText = string.IsNullOrWhiteSpace(x.EmployeeNo)
                    ? "بدون رقم تعريفي"
                    : "رقم تعريفي: " + x.EmployeeNo,
                EmploymentText = EmploymentDisplayText(x.EmploymentType),
                EmploymentClass = x.EmploymentType == EmploymentType.PermanentSupervisor
                    ? "badge-soft purple"
                    : "badge-soft blue",
                BaseSalaryText = x.BaseSalaryApplied.ToString("N2", CultureInfo.InvariantCulture),
                EarnedAmountText = x.EarnedAmount.ToString("N2", CultureInfo.InvariantCulture),
                DeductionAmountText = x.DeductionAmount.ToString("N2", CultureInfo.InvariantCulture),
                AdjustmentAmountText = x.AdjustmentAmount.ToString("N2", CultureInfo.InvariantCulture),
                NetAmountText = x.NetAmount.ToString("N2", CultureInfo.InvariantCulture),
                StatementUrl = "../Payroll/PermanentStatement.aspx?payrollId=" +
                               x.PayrollId.ToString(CultureInfo.InvariantCulture) +
                               "&periodId=" + PeriodId.ToString(CultureInfo.InvariantCulture)
            }).ToList();

            gvStaff.DataSource = rows;
            gvStaff.DataBind();

            lblRowsCount.Text = rows.Count.ToString(CultureInfo.InvariantCulture);
            pnlEmpty.Visible = rows.Count == 0;

            var period = db.PayrollPeriods.AsNoTracking().FirstOrDefault(p => p.Id == PeriodId);
            if (period != null)
            {
                int idx = (period.Month >= 1 && period.Month <= 12) ? period.Month - 1 : 0;
                string periodText = ArabicMonths[idx] + " " + period.Year;

                string statusName;
                switch (period.Status)
                {
                    case PayrollPeriodStatus.Open: statusName = "مفتوحة"; break;
                    case PayrollPeriodStatus.UnderReview: statusName = "قيد المراجعة"; break;
                    case PayrollPeriodStatus.Approved: statusName = "معتمدة"; break;
                    case PayrollPeriodStatus.Closed: statusName = "مغلقة"; break;
                    default: statusName = ""; break;
                }

                SetInfoLine("الفترة: " + periodText + " — الحالة: " + statusName +
                            " — عدد الصفوف: " + rows.Count);
            }
        }

        private void LoadItemTeachers()
        {
            if (ddlItemTeacher == null) return;

            EducationStage stage = CurrentStage();

            var teachers = db.Teachers
                .AsNoTracking()
                .Where(t => t.Stage == stage
                            && t.IsActive
                            && (t.EmploymentType == EmploymentType.Permanent
                                || t.EmploymentType == EmploymentType.PermanentSupervisor))
                .OrderBy(t => t.Name)
                .ToList();

            ddlItemTeacher.Items.Clear();
            ddlItemTeacher.Items.Add(new ListItem("-- اختر --", ""));

            foreach (var t in teachers)
            {
                string label = t.Name;
                if (t.EmploymentType == EmploymentType.PermanentSupervisor)
                    label += " (مشرف دائم)";

                ddlItemTeacher.Items.Add(new ListItem(
                    label,
                    t.Id.ToString(CultureInfo.InvariantCulture)));
            }
        }

        #endregion

        #region Events

        protected void gvStaff_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvStaff.PageIndex = e.NewPageIndex;
                LoadStaffGrid();
            }
            catch (Exception ex)
            {
                HandleException(ex, "PermanentStaff.gvStaff_PageIndexChanging");
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                int teacherId;
                decimal amount;
                int itemType;

                if (!int.TryParse(ddlItemTeacher.SelectedValue, out teacherId) || teacherId <= 0)
                {
                    SetItemInfo("يرجى اختيار المعلم أو المشرف.", "danger");
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out amount) || amount <= 0)
                {
                    SetItemInfo("أدخل قيمة صحيحة أكبر من صفر.", "danger");
                    return;
                }

                if (!int.TryParse(ddlItemType.SelectedValue, out itemType))
                {
                    SetItemInfo("نوع البند غير صالح.", "danger");
                    return;
                }

                if (PeriodId <= 0)
                {
                    SetItemInfo("يرجى اختيار فترة مالية صالحة.", "danger");
                    return;
                }

                var period = db.PayrollPeriods
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == PeriodId);

                if (period == null)
                {
                    SetItemInfo("الفترة المالية غير موجودة.", "danger");
                    return;
                }

                if (period.Status == PayrollPeriodStatus.Closed)
                {
                    SetItemInfo("لا يمكن إضافة بنود لفترة مالية مغلقة.", "danger");
                    return;
                }

                var payroll = db.TeacherPayrolls
                    .FirstOrDefault(p => p.PayrollPeriodId == PeriodId && p.TeacherId == teacherId);

                if (payroll == null)
                {
                    SetItemInfo(
                        "لم يُحسب استحقاق هذا المعلم في الفترة المختارة بعد. " +
                        "اذهب إلى «الفترات المالية» واضغط «احسب» أولاً.",
                        "danger");
                    return;
                }

                if (payroll.EmploymentTypeApplied != EmploymentType.Permanent &&
                    payroll.EmploymentTypeApplied != EmploymentType.PermanentSupervisor)
                {
                    SetItemInfo("هذا المعلم ليس من النوع الدائم.", "danger");
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

                SetItemInfo("تمت إضافة البند وتحديث الصافي بنجاح.", "success");

                txtAmount.Text = "";
                txtReason.Text = "";

                // ⭐ تصفير اختيار المعلم بعد الإضافة (حتى لا يُضاف البند مرتين بالخطأ)
                if (ddlItemTeacher.Items.Count > 0)
                {
                    ddlItemTeacher.ClearSelection();
                    ddlItemTeacher.Items[0].Selected = true;
                }

                LoadSummary();
                LoadStaffGrid();
            }
            catch (Exception ex)
            {
                HandleException(ex, "PermanentStaff.btnAddItem_Click");
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

        #endregion

        #region Info / Error

        private void SetInfoLine(string message)
        {
            if (lblInfoLine != null)
                lblInfoLine.Text = Server.HtmlEncode(message ?? "");
        }

        private void SetItemInfo(string message, string kind)
        {
            if (lblItemInfo == null) return;

            string k = string.IsNullOrWhiteSpace(kind) ? "neutral" : kind;
            lblItemInfo.CssClass = "tf-info-msg " + k;
            lblItemInfo.Text = Server.HtmlEncode(message ?? "");
            lblItemInfo.Visible = true;
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

            SetItemInfo(userMessage, "danger");
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