using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Fees
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadFees();
            }
        }

        // =========================================================
        // تحميل الرسوم حسب المرحلة الحالية
        // =========================================================
        private void LoadFees()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();
            string stageValue = Session["Stage"] as string ?? "";
            lblStage.Text = EnumHelper.GetStageArabic(stageValue);

            // معرفات الصفوف الدراسية (GradeLevels) التابعة للمرحلة الحالية
            var gradeLevelIds = EnumHelper
                .GetGradeLevelIdsForStage(db, stage)
                .ToList();

            // جلب الرسوم
            var feeRows = db.Fees
                .Include(f => f.GradeLevel)
                .Include(f => f.ClassRoom)
                .Include(f => f.AcademicYear)
                .Where(f => !f.GradeLevelId.HasValue
                         || gradeLevelIds.Contains(f.GradeLevelId.Value))
                .OrderBy(f => f.Type)
                .ThenBy(f => f.Name)
                .ThenBy(f => f.Id)
                .Select(f => new
                {
                    f.Id,
                    f.Name,
                    f.Amount,
                    f.Type,

                    f.AcademicYearId,
                    AcademicYearName = f.AcademicYear != null ? f.AcademicYear.Name : "",

                    GradeLevelId = f.GradeLevelId,
                    GradeLevelName = f.GradeLevel != null ? f.GradeLevel.Name : "",

                    SectionName = f.ClassRoom != null ? f.ClassRoom.SectionName : "",

                    // ✅ الشعبة من ClassRoom.DivisionName
                    DivisionName = f.ClassRoom != null ? f.ClassRoom.DivisionName : "",

                    Notes = f.Notes
                }).ToList();

            var fees = feeRows.Select(f => new
            {
                f.Id,
                f.Name,
                f.Amount,
                TypeName = f.Type.GetDisplayName(),
                f.AcademicYearId,
                f.AcademicYearName,
                f.GradeLevelId,
                f.GradeLevelName,
                f.SectionName,
                f.DivisionName,   // ✅ جديد
                f.Notes
            }).ToList();

            rptFees.DataSource = fees;
            rptFees.DataBind();

            pnlEmpty.Visible = fees.Count == 0;
            pnlTable.Visible = fees.Count > 0;

            // ✅ الإحصائيات
            lblTotalFees.Text = fees.Count.ToString();
            lblCountPill.Text = fees.Count.ToString();

            decimal totalAmount = fees.Sum(f => f.Amount);
            lblTotalAmount.Text = totalAmount.ToString("N2") + " د.ل";
        }

        // =========================================================
        // دوال مساعدة للعرض
        // =========================================================

        protected string GetFeeTypeName(FeeType type)
        {
            return type.GetDisplayName();
        }

        protected string FormatAmount(decimal amount)
        {
            return amount.ToString("N2") + " د.ل";
        }

        /// <summary>
        /// عرض شارة الصف
        /// </summary>
        protected string RenderGradeLevelBadge(object gradeLevelIdObj, object gradeLevelNameObj)
        {
            bool hasGrade = gradeLevelIdObj != null && gradeLevelIdObj != DBNull.Value;
            string name = gradeLevelNameObj == null ? "" : gradeLevelNameObj.ToString();

            if (hasGrade && !string.IsNullOrWhiteSpace(name))
            {
                return "<span class=\"fee-grade-badge\"> "
                    + HttpUtility.HtmlEncode(name) + "</span>";
            }

            return "<span class=\"fee-grade-badge all\"> "
                + "جميع الصفوف</span>";
        }

        /// <summary>
        /// عرض القسم (علمي / أدبي) للصفوف الثانوية فقط
        /// </summary>
        protected string RenderSection(object gradeLevelNameObj, object sectionNameObj)
        {
            string gradeName = gradeLevelNameObj == null ? "" : gradeLevelNameObj.ToString().Trim();
            string section = sectionNameObj == null ? "" : sectionNameObj.ToString().Trim();

            bool isSecondary = gradeName == "ثاني ثانوي" || gradeName == "ثالث ثانوي";
            if (!isSecondary || string.IsNullOrWhiteSpace(section))
            {
                return "<span class=\"text-muted\">—</span>";
            }

            string display;
            if (section == "علمي") display = "القسم العلمي";
            else if (section == "أدبي") display = "القسم الأدبي";
            else display = section;

            return "<span class=\"fee-section-badge\"><i class=\"fas fa-stream\" style=\"font-size:10px;\"></i> "
                + HttpUtility.HtmlEncode(display) + "</span>";
        }

        /// <summary>
        /// ✅ عرض الشعبة (شعبة أ / شعبة ب ...) — جديد
        /// </summary>
        protected string RenderDivision(object divisionObj)
        {
            string division = divisionObj == null || divisionObj == DBNull.Value
                                ? ""
                                : divisionObj.ToString().Trim();

            if (string.IsNullOrWhiteSpace(division))
            {
                return "<span class=\"text-muted\">—</span>";
            }

            return "<span class=\"fee-division-badge\"><i class=\"fas fa-chair\" style=\"font-size:10px;\"></i> "
                + "شعبة " + HttpUtility.HtmlEncode(division) + "</span>";
        }

        /// <summary>
        /// يبني رابط "رسوم الطلاب" مع تمرير السنة والصف والقسم
        /// </summary>
        protected string BuildStudentFeesUrl(object academicYearId, object gradeLevelId, object sectionName)
        {
            string stage = Session["Stage"] as string ?? "";

            var parts = new List<string>();
            parts.Add("stage=" + HttpUtility.UrlEncode(stage));

            if (academicYearId != null && academicYearId != DBNull.Value)
                parts.Add("year=" + academicYearId);

            if (gradeLevelId != null && gradeLevelId != DBNull.Value)
                parts.Add("grade=" + gradeLevelId);

            if (sectionName != null && sectionName != DBNull.Value)
            {
                string s = sectionName.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(s))
                    parts.Add("section=" + HttpUtility.UrlEncode(s));
            }

            return "StudentFinances.aspx?" + string.Join("&", parts);
        }

        // =========================================================
        // التخلص من DbContext
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}