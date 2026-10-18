using System;
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
    public partial class PermanentStatement : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private int _periodId;
        private decimal _baseSalary;
        private decimal _additions;
        private decimal _deductions;

        public int PeriodId { get { return _periodId; } }

        public int PayrollId
        {
            get
            {
                int v;
                return int.TryParse(Request["payrollId"], out v) ? v : 0;
            }
        }

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Local guard: this page shows a net salary, so it must not depend on
                // Site.Master path check alone.
                SecurityHelper.DenyUnlessPath();

                if (IsPostBack) return;

                if (PayrollId == 0)
                {
                    SetInfo("لم يتم تحديد رقم الاستحقاق.", "danger");
                    return;
                }

                // A teacher may only open their own record.
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
                        p.EmploymentTypeApplied,
                        p.BaseSalaryApplied
                    })
                    .FirstOrDefault();

                if (payroll == null)
                {
                    SetInfo("الاستحقاق غير موجود.", "danger");
                    return;
                }

                // This page is only for permanent staff.
                if (payroll.EmploymentTypeApplied != EmploymentType.Permanent &&
                    payroll.EmploymentTypeApplied != EmploymentType.PermanentSupervisor)
                {
                    SetInfo("هذه الصفحة مخصصة للمعلمين الدائمين والمشرفين الدائمين فقط. " +
                            "استخدم صفحة كشف الاستحقاق الشهري للمعلمين بالحصة.", "danger");
                    return;
                }

                _periodId = payroll.PayrollPeriodId;

                var teacher = db.Teachers.Find(payroll.TeacherId);
                var period = db.PayrollPeriods.Find(payroll.PayrollPeriodId);
                if (teacher == null || period == null)
                {
                    SetInfo("تعذر جلب بيانات المعلم أو الفترة.", "danger");
                    return;
                }

                string[] months =
                {
                    "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                    "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
                };

                int monthIdx = (period.Month >= 1 && period.Month <= 12) ? period.Month - 1 : 0;
                string periodText = months[monthIdx] + " " + period.Year;

                // ==== الهيدر (Hero) ====
                lblTeacherName.Text = Server.HtmlEncode(teacher.Name);
                lblPeriod.Text = Server.HtmlEncode(periodText);

                // ==== بطاقة بيانات المعلم ====
                lblTeacherName2.Text = Server.HtmlEncode(teacher.Name);
                lblPeriod2.Text = Server.HtmlEncode(periodText);
                lblEmployment.Text = Server.HtmlEncode(EmploymentDisplayText(payroll.EmploymentTypeApplied));
                lblBaseSalary.Text = payroll.BaseSalaryApplied.ToString("N2", CultureInfo.InvariantCulture) + " د.ل";

                string dateFrom = period.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                string dateTo = period.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                lblPeriodDates.Text = "من " + dateFrom + " إلى " + dateTo;

                // ==== الملخص المالي ====
                lblEarned.Text = payroll.EarnedAmount.ToString("N2", CultureInfo.InvariantCulture) + " د.ل";
                lblDeductions.Text = payroll.DeductionAmount.ToString("N2", CultureInfo.InvariantCulture) + " د.ل";
                lblAdjustments.Text = payroll.AdjustmentAmount.ToString("N2", CultureInfo.InvariantCulture) + " د.ل";
                lblNet.Text = payroll.NetAmount.ToString("N2", CultureInfo.InvariantCulture) + " د.ل";

                // ==== تفصيل مكونات الراتب ====
                ComputeBreakdown(payroll.EarnedAmount, payroll.DeductionAmount, payroll.AdjustmentAmount, payroll.NetAmount);

                // ==== تحميل البنود ====
                LoadItems();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.PermanentStatement.Page_Load");
            }
        }

        #endregion

        #region Data Loading

        /// <summary>
        /// يحسب مكونات الراتب من بيانات الاستحقاق والبنود اليدوية:
        ///  - الأساسي = BaseSalaryApplied (يُقرأ من الفترة)
        ///  - الإضافات = مجموع البنود غير الخصومات (من AdjustmentAmount)
        ///  - الخصومات = مجموع البنود من نوع Deduction + أي خصومات أخرى
        ///  - الصافي = NetAmount
        /// </summary>
        private void ComputeBreakdown(decimal earned, decimal deductions, decimal adjustments, decimal net)
        {
            _baseSalary = earned;
            _additions = adjustments > 0m ? adjustments : 0m;
            _deductions = deductions;

            lblBreakdownBase.Text = _baseSalary.ToString("N2", CultureInfo.InvariantCulture);
            lblBreakdownAdditions.Text = (_additions > 0m ? "+" : "") +
                _additions.ToString("N2", CultureInfo.InvariantCulture);
            lblBreakdownDeductions.Text = (_deductions > 0m ? "-" : "") +
                _deductions.ToString("N2", CultureInfo.InvariantCulture);
            lblBreakdownNet.Text = net.ToString("N2", CultureInfo.InvariantCulture);
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

                lblItemsCount.Text = items.Count.ToString(CultureInfo.InvariantCulture);

                if (pnlItemsEmpty != null)
                    pnlItemsEmpty.Visible = items.Count == 0;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.PermanentStatement.LoadItems");
            }
        }

        #endregion

        #region Display Helpers

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

        protected string ItemTypeName(int itemType)
        {
            return EnumHelper.GetDisplayName((PayrollItemType)itemType);
        }

        /// <summary>
        /// يرجع كلاس الوسم المناسب حسب نوع البند.
        ///  - Deduction (1)  → أحمر
        ///  - Bonus/Allowance/Correction/Adjustment → أخضر
        /// </summary>
        protected string ItemTypeClass(int itemType)
        {
            if (itemType == (int)PayrollItemType.Deduction)
                return "badge-soft red";

            return "badge-soft green";
        }

        #endregion

        #region Paging

        protected void gvItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvItems.PageIndex = e.NewPageIndex;
                LoadItems();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Payroll.PermanentStatement.gvItems_PageIndexChanging");
            }
        }

        #endregion

        #region Info / Error

        private void SetInfo(string message, string kind)
        {
            if (lblInfo == null) return;

            string k = string.IsNullOrWhiteSpace(kind) ? "info" : kind;
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