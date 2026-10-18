using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Subjects
{
    public partial class Default : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadStage();
                SetupCategoryFilter();
                LoadSubjects();
            }
        }

        // =========================================================
        // تحميل المرحلة الحالية
        // =========================================================
        private void LoadStage()
        {
            var stage = EnumHelper.GetCurrentStage();

            CurrentStageLabel.Text =
                EnumHelper.GetStageArabic(stage.ToString());
        }

        // =========================================================
        // إعداد فلتر الصف / الفرع
        // يظهر فقط في المرحلة الثانوية
        // =========================================================
        private void SetupCategoryFilter()
        {
            var stage = EnumHelper.GetCurrentStage();

            pnlCategoryFilter.Visible =
                stage == EducationStage.Secondary;
        }

        // =========================================================
        // تحميل المواد
        // =========================================================
        private void LoadSubjects()
        {
            var stage = EnumHelper.GetCurrentStage();

            var query = _context.Subjects
                .AsNoTracking()
                .Where(s => s.Stage == stage);

            // -----------------------------------------------------
            // البحث بالاسم
            // -----------------------------------------------------
            string search = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(
                    s => s.Name.Contains(search));
            }

            // -----------------------------------------------------
            // فلتر الصف / الفرع
            // -----------------------------------------------------
            if (stage == EducationStage.Secondary &&
                !string.IsNullOrEmpty(ddlCategory.SelectedValue))
            {
                SubjectCategory category;

                if (Enum.TryParse(
                    ddlCategory.SelectedValue,
                    true,
                    out category))
                {
                    query = query.Where(
                        s => s.Category == category);
                }
            }

            // -----------------------------------------------------
            // العدد الإجمالي
            // -----------------------------------------------------
            int totalCount = query.Count();

            // -----------------------------------------------------
            // جلب البيانات من SQL أولاً
            //
            // مهم:
            // لا نستدعي GetCategoryName داخل LINQ to Entities
            // حتى لا يحاول Entity Framework تحويلها إلى SQL.
            // -----------------------------------------------------
            var subjects = query
                .OrderBy(s => s.Name)
                .ThenBy(s => s.Id)
                .Skip(gvSubjects.PageIndex * gvSubjects.PageSize)
                .Take(gvSubjects.PageSize)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Description,
                    s.Stage,
                    s.Category
                })
                .ToList();

            // -----------------------------------------------------
            // تحويل اسم الصف / الفرع بعد جلب البيانات
            // -----------------------------------------------------
            var displaySubjects = subjects
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Description,

                    CategoryName =
                        s.Stage == EducationStage.Basic
                            ? "أساسي"
                            : GetCategoryName(s.Category)
                })
                .ToList();

            // -----------------------------------------------------
            // ربط الجدول
            // -----------------------------------------------------
            gvSubjects.AllowCustomPaging = true;
            gvSubjects.VirtualItemCount = totalCount;

            gvSubjects.DataSource = displaySubjects;
            gvSubjects.DataBind();

            // -----------------------------------------------------
            // العدد
            // -----------------------------------------------------
            lblCount.Text = "عدد المواد: " + totalCount;
            // بـ:
            lblCount.Text = totalCount.ToString();

            // -----------------------------------------------------
            // رسالة عدم وجود نتائج
            // -----------------------------------------------------
            pnlNoResults.Visible =
                totalCount == 0;
        }

        // =========================================================
        // اسم الفرع بالعربية
        // =========================================================
        private string GetCategoryName(SubjectCategory category)
        {
            switch (category)
            {
                case SubjectCategory.FirstSecondary:
                    return "أول ثانوي";

                case SubjectCategory.Scientific:
                    return "ثاني وثالث علمي";

                case SubjectCategory.Literary:
                    return "ثاني وثالث أدبي";

                default:
                    return "عام";
            }
        }

        // =========================================================
        // إضافة مادة
        // =========================================================
        protected void btnAddSubject_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect("Create.aspx");
        }

        // =========================================================
        // بحث
        // =========================================================
        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            gvSubjects.PageIndex = 0;

            LoadSubjects();
        }

        // =========================================================
        // مسح البحث
        // =========================================================
        protected void btnClearSearch_Click(
            object sender,
            EventArgs e)
        {
            gvSubjects.PageIndex = 0;

            txtSearch.Text = string.Empty;

            if (ddlCategory.Items.Count > 0)
            {
                ddlCategory.SelectedIndex = 0;
            }

            LoadSubjects();
        }

        // =========================================================
        // تغيير الصف / الفرع
        // =========================================================
        protected void ddlCategory_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            gvSubjects.PageIndex = 0;

            LoadSubjects();
        }

        // =========================================================
        // إجراءات المواد
        // =========================================================
        protected void gvSubjects_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null)
            {
                return;
            }

            int id;

            if (!int.TryParse(
                e.CommandArgument.ToString(),
                out id))
            {
                return;
            }

            switch (e.CommandName)
            {
                case "EditSubject":

                    Response.Redirect(
                        "Edit.aspx?id=" + id);

                    break;

                case "DeleteSubject":

                    Response.Redirect(
                        "Delete.aspx?id=" + id);

                    break;
            }
        }

        // =========================================================
        // تغيير الصفحة
        // =========================================================
        protected void gvSubjects_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            gvSubjects.PageIndex = e.NewPageIndex;

            LoadSubjects();
        }

        // =========================================================
        // التخلص من الاتصال
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (_context != null)
            {
                _context.Dispose();
            }

            base.OnUnload(e);
        }
        // =========================================================
        // ✅ إرجاع كلاس البطاقة حسب نوع الصف / الفرع
        // =========================================================
        protected string GetCategoryCss(object categoryName)
        {
            string value = Convert.ToString(categoryName).Trim();

            if (value == "أساسي")
                return "sb-category-badge basic";

            return "sb-category-badge";
        }
    }
}