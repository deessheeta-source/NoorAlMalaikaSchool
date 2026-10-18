using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Data.Services;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Reports
{
    public partial class StudentFinancialStatement : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private EducationStage CurrentStage
        {
            get { return EnumHelper.GetCurrentStage(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local guard: this page shows per-student balances, fees and discount
            // history, so it must not rely on the Site.Master path check alone.
            SecurityHelper.DenyUnlessPath();
               SecurityHelper.HideViewExportControls(this);

            if (!IsPostBack)
            {
                LoadAcademicYears();
                LoadGradeLevels();
                ConfigureSectionFilter();
                LoadDivisions();
                LoadStudentsFilter();
                LoadReport();
            }
        }

        // =========================================================
        // تحميل السنة الدراسية
        // =========================================================
        private void LoadAcademicYears()
        {
            var years = db.AcademicYears
                .OrderByDescending(y => y.StartDate)
                .ToList();

            ddlYear.Items.Clear();
            ddlYear.Items.Add(new ListItem("الكل", ""));

            foreach (var y in years)
            {
                ddlYear.Items.Add(new ListItem(y.Name, y.Id.ToString()));
            }

            var current = years.FirstOrDefault(y => y.IsCurrent);
            if (current != null)
            {
                ddlYear.SelectedValue = current.Id.ToString();
            }
        }

        // =========================================================
        // تحميل الصفوف حسب المرحلة الحالية
        // =========================================================
        private void LoadGradeLevels()
        {
            EducationStage stage = CurrentStage;

            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();
            ddlGradeLevel.Items.Add(new ListItem("الكل", ""));

            foreach (var gl in gradeLevels)
            {
                ddlGradeLevel.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));
            }
        }

        // =========================================================
        // إظهار فلتر القسم للصفوف الثانوية فقط
        // =========================================================
        private bool IsSecondaryGrade(int gradeLevelId)
        {
            if (gradeLevelId <= 0) return false;

            try
            {
                string gradeName = db.GradeLevels
                    .AsNoTracking()
                    .Where(g => g.Id == gradeLevelId)
                    .Select(g => g.Name)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(gradeName)) return false;

                gradeName = gradeName.Trim();
                return gradeName == "ثاني ثانوي" || gradeName == "ثالث ثانوي";
            }
            catch { return false; }
        }

        private void LoadDivisions()
        {
            string selected = ddlDivision.SelectedValue;
            var divisions = db.ClassRooms.AsNoTracking()
                .Where(c => c.GradeLevel.Stage == CurrentStage
                    && c.DivisionName != null && c.DivisionName.Trim() != "")
                .Select(c => c.DivisionName).Distinct().OrderBy(x => x).ToList();

            ddlDivision.Items.Clear();
            ddlDivision.Items.Add(new ListItem("الكل", ""));
            foreach (var division in divisions)
                ddlDivision.Items.Add(new ListItem(division, division));
            if (!string.IsNullOrEmpty(selected) && ddlDivision.Items.FindByValue(selected) != null)
                ddlDivision.SelectedValue = selected;
        }

        private void LoadStudentsFilter()
        {
            string selected = ddlStudent.SelectedValue;
            var students = db.Students.AsNoTracking()
                .Where(s => s.IsActive && s.Stage == CurrentStage)
                .OrderBy(s => s.Name)
                .Select(s => new { s.Id, s.Name, s.RegistrationNo }).ToList();

            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("جميع الطلاب", ""));
            foreach (var student in students)
            {
                string label = student.Name;
                if (!string.IsNullOrWhiteSpace(student.RegistrationNo))
                    label += " - " + student.RegistrationNo;
                ddlStudent.Items.Add(new ListItem(label, student.Id.ToString()));
            }
            if (!string.IsNullOrEmpty(selected) && ddlStudent.Items.FindByValue(selected) != null)
                ddlStudent.SelectedValue = selected;
        }

        private void ConfigureSectionFilter()
        {
            int gradeLevelId;
            int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId);

            bool showSection = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = showSection;

            if (!showSection)
            {
                if (ddlSection.Items.FindByValue("") != null)
                {
                    ddlSection.ClearSelection();
                    ddlSection.Items.FindByValue("").Selected = true;
                }
            }
        }

        // =========================================================
        // بناء رابط التفاصيل مع مراعاة stage
        // =========================================================
        protected string BuildDetailsUrl(object studentId)
        {
            string stageEnc = Server.UrlEncode(
                Session["Stage"] as string ?? "Basic");

            return ResolveClientUrl("~/Pages/Payments/StudentReport.aspx")
                + "?studentId=" + Convert.ToString(studentId)
                + "&stage=" + stageEnc;
        }

        // =========================================================
        // تنسيق عرض القسم
        // =========================================================

        protected string FormatSection(object section)
        {
            if (section == null || section == DBNull.Value)
                return "<span class='sf-section-badge empty'>—</span>";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class='sf-section-badge empty'>—</span>";

            string display;
            if (value == "علمي") display = "القسم العلمي";
            else if (value == "أدبي") display = "القسم الأدبي";
            else display = value;

            return "<span class='sf-section-badge'>"
                + HttpUtility.HtmlEncode(display) + "</span>";
        }

        // =========================================================
        // الأحداث
        // =========================================================
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport();
        }

        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            LoadReport();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport();
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport();
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySummaryType();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        // =========================================================
        // بناء الاستعلام الأساسي
        // =========================================================
        private List<Student> GetFilteredStudents()
        {
            EducationStage stage = CurrentStage;

            int? selectedYearId = null;
            int yearId;
            if (int.TryParse(ddlYear.SelectedValue, out yearId) && yearId > 0)
                selectedYearId = yearId;

            int? selectedGradeLevelId = null;
            int gradeLevelId;
            if (int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) && gradeLevelId > 0)
                selectedGradeLevelId = gradeLevelId;

            string section = ddlSection.SelectedValue;
            if (string.IsNullOrWhiteSpace(section)) section = null;

            string search = txtSearch.Text.Trim();

            IQueryable<Student> query = db.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.ClassRoom.GradeLevel)
                .Where(s => s.IsActive
                         && s.Stage == stage
                         && s.ClassRoom != null);

            if (selectedYearId.HasValue)
            {
                query = query.Where(s =>
                    s.ClassRoom.AcademicYearId == selectedYearId.Value);
            }

            if (selectedGradeLevelId.HasValue)
            {
                query = query.Where(s =>
                    s.ClassRoom.GradeLevelId == selectedGradeLevelId.Value);
            }

            if (!string.IsNullOrWhiteSpace(section))
            {
                query = query.Where(s => s.Section == section);
            }

            string division = ddlDivision.SelectedValue;
            if (!string.IsNullOrWhiteSpace(division))
                query = query.Where(s => s.ClassRoom.DivisionName == division);

            int selectedStudentId;
            if (int.TryParse(ddlStudent.SelectedValue, out selectedStudentId) && selectedStudentId > 0)
                query = query.Where(s => s.Id == selectedStudentId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Name.Contains(search)
                    || s.RegistrationNo.Contains(search));
            }

            return query.OrderBy(s => s.Name).ToList();
        }

        // =========================================================
        // تحميل الكشف المالي
        // =========================================================
        private void LoadReport()
        {
            var students = GetFilteredStudents();
            var studentIds = students.Select(s => s.Id).ToList();

            // الفواتير
            IQueryable<Invoice> invQuery = db.Invoices
                .Include(i => i.Lines)
                .Where(i => studentIds.Contains(i.StudentId)
                         && i.Status != InvoiceStatus.Cancelled
                         && i.Status != InvoiceStatus.Draft);

            int yearId;
            if (int.TryParse(ddlYear.SelectedValue, out yearId) && yearId > 0)
            {
                invQuery = invQuery.Where(i => i.AcademicYearId == yearId);
            }

            var invoices = invQuery.ToList();
            var invoiceIds = invoices.Select(i => i.Id).ToList();

            // المدفوعات
            var allPayments = invoiceIds.Count == 0
                ? new List<Payment>()
                : db.Payments
                    .Where(p => p.InvoiceId.HasValue
                             && invoiceIds.Contains(p.InvoiceId.Value))
                    .ToList();

            var paymentsByStudent = allPayments
                .GroupBy(p => p.StudentId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // أسماء السنوات الدراسية
            var yearIds = students
                .Where(s => s.ClassRoom != null)
                .Select(s => s.ClassRoom.AcademicYearId)
                .Distinct()
                .ToList();

            var yearNameById = db.AcademicYears
                .Where(y => yearIds.Contains(y.Id))
                .ToDictionary(y => y.Id, y => y.Name);

            decimal totalSubscription = 0m;
            decimal totalDiscount = 0m;
            decimal totalDue = 0m;
            decimal totalPaid = 0m;
            decimal totalRemaining = 0m;
            var rows = new List<object>();

            foreach (var student in students)
            {
                int yearIdForStudent = student.ClassRoom != null
                    ? student.ClassRoom.AcademicYearId
                    : 0;

                var studentInvoices = invoices.Where(i => i.StudentId == student.Id).ToList();
                decimal subscription = studentInvoices.Sum(i => i.TotalAmount);
                decimal discount = studentInvoices.Sum(i => i.DiscountAmount);
                decimal due = studentInvoices.Sum(i => i.NetAmount);

                decimal paid = paymentsByStudent.ContainsKey(student.Id)
                    ? paymentsByStudent[student.Id].Sum(p => p.Amount)
                    : 0m;

                decimal remaining = Math.Max(0m, due - paid);

                totalSubscription += subscription;
                totalDiscount += discount;
                totalDue += due;
                totalPaid += paid;
                totalRemaining += remaining;

                rows.Add(new
                {
                    StudentId = student.Id,
                    StudentName = student.Name,
                    RegistrationNo = student.RegistrationNo,
                    ClassName = student.ClassRoom != null
                        ? student.ClassRoom.Name
                        : "",
                    Section = student.Section,
                    YearName = yearNameById.ContainsKey(yearIdForStudent)
                        ? yearNameById[yearIdForStudent]
                        : "",
                    TotalSubscription = subscription,
                    TotalDiscount = discount,
                    TotalDue = due,
                    TotalPaid = paid,
                    Remaining = remaining
                });
            }

            gvReport.DataSource = rows;
            gvReport.DataBind();

            lblStudents.Text = students.Count.ToString();
            lblTotalSubscription.Text = totalSubscription.ToString("N2");
            lblTotalDiscount.Text = totalDiscount.ToString("N2");
            lblTotalDue.Text = totalDue.ToString("N2");
            lblTotalPaid.Text = totalPaid.ToString("N2");
            lblTotalRemaining.Text = totalRemaining.ToString("N2");

            pnlSummary.Visible = true;
            ApplySummaryType();
        }

        private void ApplySummaryType()
        {
            string type = ddlType.SelectedValue;
            pnlSummaryStudents.Visible = string.IsNullOrEmpty(type);
            pnlSummarySubscription.Visible = string.IsNullOrEmpty(type) || type == "subscription";
            pnlSummaryDiscount.Visible = string.IsNullOrEmpty(type) || type == "discount";
            pnlSummaryDue.Visible = string.IsNullOrEmpty(type) || type == "due";
            pnlSummaryPaid.Visible = string.IsNullOrEmpty(type) || type == "paid";
            pnlSummaryRemaining.Visible = string.IsNullOrEmpty(type) || type == "remaining";
        }

        protected void gvReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReport.PageIndex = e.NewPageIndex;
            LoadReport();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}