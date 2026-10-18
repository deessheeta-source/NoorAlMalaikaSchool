using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Classes
{
    public partial class Default : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                string stage = Request.QueryString["stage"];
                if (!string.IsNullOrEmpty(stage))
                    Session["Stage"] = stage;
                else if (Session["Stage"] != null)
                    stage = Session["Stage"].ToString();

                if (!string.IsNullOrEmpty(stage))
                {
                    lblStage.Text = "المستوى: " + EnumHelper.GetStageArabic(stage);
                    lblStage.Visible = true;
                }
                else
                {
                    lblStage.Visible = false;
                }

                LoadClasses(stage);
            }
        }

        protected string FormatSection(object section)
        {
            if (section == null || section == DBNull.Value) return "—";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value)) return "—";

            if (value == "علمي") return "القسم العلمي";
            if (value == "أدبي") return "القسم الأدبي";

            return value;
        }

        protected string FormatDivision(object division)
        {
            if (division == null || division == DBNull.Value) return "—";

            string value = Convert.ToString(division).Trim();
            if (string.IsNullOrWhiteSpace(value)) return "—";

            return "شعبة " + value;
        }

        private void LoadClasses(string stage)
        {
            var query = _context.ClassRooms.AsQueryable();

            EducationStage stageEnum;
            if (!string.IsNullOrEmpty(stage) && Enum.TryParse<EducationStage>(stage, true, out stageEnum))
            {
                var gradeLevelIds = EnumHelper.GetGradeLevelIdsForStage(_context, stageEnum);
                query = query.Where(c => gradeLevelIds.Contains(c.GradeLevelId));
            }

            var classes = query
                .Select(c => new
                {
                    c.Id,
                    c.SectionName,
                    c.DivisionName,
                    GradeLevel = c.GradeLevel,
                    AcademicYear = c.AcademicYear
                })
                .ToList()
                .OrderBy(x => x.AcademicYear != null ? x.AcademicYear.Name : "")
                .ThenBy(x => x.GradeLevel != null ? x.GradeLevel.SortOrder : int.MaxValue)
                .ThenBy(x => x.GradeLevel != null ? x.GradeLevel.Name : "")
                .ThenBy(x => x.SectionName)
                .ThenBy(x => x.DivisionName)
                .ToList();

            gvClasses.DataSource = classes;
            gvClasses.DataBind();

            pnlNoResults.Visible = classes.Count == 0;
        }

        protected void btnAddClass_Click(object sender, EventArgs e)
        {
            string stage = Session["Stage"] != null ? Session["Stage"].ToString() : "";
            Response.Redirect("Create.aspx?stage=" + Server.UrlEncode(stage), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void gvClasses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            int id = Convert.ToInt32(e.CommandArgument);
            string stage = Session["Stage"] != null ? Server.UrlEncode(Session["Stage"].ToString()) : "";

            switch (e.CommandName)
            {
                case "EditClass":
                    Response.Redirect("Edit.aspx?id=" + id + "&stage=" + stage, false);
                    Context.ApplicationInstance.CompleteRequest();
                    break;
                case "DeleteClass":
                    Response.Redirect("Delete.aspx?id=" + id + "&stage=" + stage, false);
                    Context.ApplicationInstance.CompleteRequest();
                    break;
            }
        }

        protected void gvClasses_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvClasses.PageIndex = e.NewPageIndex;
            string stage = Session["Stage"] != null ? Session["Stage"].ToString() : null;
            LoadClasses(stage);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }
    }
}