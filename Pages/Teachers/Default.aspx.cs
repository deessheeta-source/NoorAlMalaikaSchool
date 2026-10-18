using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Teachers
{
    public partial class Default : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadSpecialtyFilter();
                LoadAcademicYearFilter();
                LoadTeachers();
            }
        }

        #endregion

        #region Load Filters

        /// <summary>
        /// تعبئة قائمة المواد (التخصصات) من جدول المواد للمرحلة الحالية.
        /// </summary>
        private void LoadSpecialtyFilter()
        {
            var stage = EnumHelper.GetCurrentStage();

            var subjectNames = _context.Subjects
                .Where(s => s.Stage == stage)
                .Select(s => s.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            ddlSpecialty.Items.Clear();
            ddlSpecialty.Items.Add(new ListItem("جميع المواد", ""));

            foreach (var name in subjectNames)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    ddlSpecialty.Items.Add(new ListItem(name, name));
                }
            }

            // إضافة المواد القديمة (التي قد لا تكون في جدول المواد) إن وجدت في بطاقات المعلمين
            var teacherSpecialties = _context.Teachers
                .Where(t => t.Stage == stage && t.Specialty != null && t.Specialty != "")
                .Select(t => t.Specialty)
                .Distinct()
                .ToList();

            foreach (var sp in teacherSpecialties)
            {
                if (string.IsNullOrWhiteSpace(sp)) continue;
                if (ddlSpecialty.Items.FindByValue(sp) != null) continue;

                ddlSpecialty.Items.Add(new ListItem(sp, sp));
            }
        }

        /// <summary>
        /// تعبئة قائمة السنوات الدراسية.
        /// </summary>
        private void LoadAcademicYearFilter()
        {
            var years = _context.AcademicYears
                .OrderByDescending(y => y.Name)
                .ToList();

            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("جميع السنوات", ""));

            foreach (var y in years)
            {
                ddlAcademicYear.Items.Add(new ListItem(y.Name, y.Id.ToString()));
            }

            // افتراضياً: السنة الحالية إن وُجدت
            var current = years.FirstOrDefault(y => y.IsCurrent);
            if (current != null)
            {
                ddlAcademicYear.SelectedValue = current.Id.ToString();
            }
        }

        #endregion

        #region Load Teachers

        private void LoadTeachers()
        {
            var stage = EnumHelper.GetCurrentStage();

            var query = _context.Teachers
                .AsNoTracking()
                .Where(t => t.Stage == stage);

            // ✅ فلتر الاسم
            string search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Name.Contains(search));
            }

            // ✅ فلتر المادة (التخصص)
            string specialty = ddlSpecialty.SelectedValue;
            if (!string.IsNullOrWhiteSpace(specialty))
            {
                query = query.Where(t => t.Specialty == specialty);
            }

            // ✅ فلتر السنة الدراسية
            //    السنة مرتبطة مباشرة ببطاقة المعلم، وليس بوجود حصة في الجدول.
            int yearId;
            if (int.TryParse(ddlAcademicYear.SelectedValue, out yearId) && yearId > 0)
            {
                query = query.Where(t => t.AcademicYearId == yearId);
            }

            int totalCount = query.Count();

            var teachers = query
                .OrderBy(t => t.Name)
                .ThenBy(t => t.Id)
                .Skip(gvTeachers.PageIndex * gvTeachers.PageSize)
                .Take(gvTeachers.PageSize)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Gender,
                    t.Specialty,
                    t.LessonRate,
                    t.Phone,
                    t.IsActive
                })
                .ToList();

            gvTeachers.AllowCustomPaging = true;
            gvTeachers.VirtualItemCount = totalCount;
            gvTeachers.DataSource = teachers;
            gvTeachers.DataBind();

            lblCount.Text = "عدد المعلمين: " + totalCount;
            pnlNoResults.Visible = totalCount == 0;
        }

        #endregion

        #region Handlers

        protected void btnAddTeacher_Click(object sender, EventArgs e)
        {
            Response.Redirect("Create.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvTeachers.PageIndex = 0;
            LoadTeachers();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            gvTeachers.PageIndex = 0;
            txtSearch.Text = string.Empty;

            if (ddlSpecialty.Items.Count > 0)
                ddlSpecialty.SelectedIndex = 0;

            if (ddlAcademicYear.Items.Count > 0)
                ddlAcademicYear.SelectedIndex = 0;

            LoadTeachers();
        }

        protected void gvTeachers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            int id = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditTeacher":
                    Response.Redirect("Edit.aspx?id=" + id);
                    break;
                case "DeleteTeacher":
                    Response.Redirect("Delete.aspx?id=" + id);
                    break;
            }
        }

        protected void gvTeachers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTeachers.PageIndex = e.NewPageIndex;
            LoadTeachers();
        }

        #endregion

        #region Helpers

        protected string GetEmploymentName(EmploymentType type)
        {
            switch (type)
            {
                case EmploymentType.Temporary: return "مؤقت";
                case EmploymentType.PartTime: return "جزئي";
                case EmploymentType.PerLesson: return "أجر بالحصص";
                case EmploymentType.PermanentSupervisor: return "مشرف دائم - راتب شهري";
                default: return "دائم - راتب شهري";
            }
        }

        protected string GetEmploymentCss(EmploymentType type)
        {
            switch (type)
            {
                case EmploymentType.Temporary: return "emp-type-badge muaqat";
                case EmploymentType.PartTime: return "emp-type-badge juzi";
                case EmploymentType.PerLesson: return "emp-type-badge bilhesas";
                case EmploymentType.PermanentSupervisor: return "emp-type-badge daimi";
                default: return "emp-type-badge daimi";
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }

        #endregion
    }
}