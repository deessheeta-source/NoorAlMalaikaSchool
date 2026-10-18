using System;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Classes
{
    public partial class Edit : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                string stage = Request.QueryString["stage"];
                if (!string.IsNullOrEmpty(stage))
                    hfStage.Value = stage;
                else if (Session["Stage"] != null)
                    hfStage.Value = Session["Stage"].ToString();

                string idStr = Request.QueryString["id"];
                int id;
                if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out id))
                {
                    PopulateDropdowns();
                    LoadClassRoom(id);
                }
                else
                {
                    pnlForm.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private bool IsSecondaryGrade(int gradeLevelId)
        {
            if (gradeLevelId <= 0) return false;

            try
            {
                string gradeName = _context.GradeLevels
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

        private void ConfigureSectionFilter()
        {
            int gradeLevelId;
            int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId);

            bool showSection = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = showSection;

            if (!showSection) SelectSection("");
        }

        private void SelectSection(string value)
        {
            if (ddlSection == null) return;
            if (value == null) value = "";

            ListItem item = ddlSection.Items.FindByValue(value);
            if (item != null)
            {
                ddlSection.ClearSelection();
                item.Selected = true;
            }
            else if (ddlSection.Items.Count > 0)
            {
                ddlSection.SelectedIndex = 0;
            }
        }

        private void SelectDivision(string value)
        {
            if (ddlDivision == null) return;
            if (value == null) value = "";

            ListItem item = ddlDivision.Items.FindByValue(value);
            if (item != null)
            {
                ddlDivision.ClearSelection();
                item.Selected = true;
            }
            else if (ddlDivision.Items.Count > 0)
            {
                ddlDivision.SelectedIndex = 0;
            }
        }

        private string GetSelectedSection()
        {
            if (ddlSection == null) return null;
            string value = ddlSection.SelectedValue;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value;
        }

        private string GetSelectedDivision()
        {
            if (ddlDivision == null) return null;
            string value = ddlDivision.SelectedValue;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value;
        }

        private string BuildClassName(string gradeLevelName, string section, string division)
        {
            if (string.IsNullOrWhiteSpace(gradeLevelName))
                gradeLevelName = "فصل";

            string result = gradeLevelName.Trim();

            if (!string.IsNullOrWhiteSpace(section))
            {
                string sectionText = section == "علمي" ? "علمي"
                                   : section == "أدبي" ? "أدبي"
                                   : section.Trim();
                result += " - " + sectionText;
            }

            if (!string.IsNullOrWhiteSpace(division))
                result += " - شعبة " + division.Trim();

            return result;
        }

        private void PopulateDropdowns()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            var gradeLevels = _context.GradeLevels
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();
            ddlGradeLevel.Items.Add(new ListItem("-- اختر الصف --", "0"));
            foreach (var gl in gradeLevels)
                ddlGradeLevel.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));

            var academicYears = _context.AcademicYears
                .OrderByDescending(a => a.Name)
                .ToList();

            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("-- اختر السنة --", "0"));
            foreach (var ay in academicYears)
                ddlAcademicYear.Items.Add(new ListItem(ay.Name, ay.Id.ToString()));
        }

        private void LoadClassRoom(int id)
        {
            var classRoom = _context.ClassRooms
                .Include("GradeLevel")
                .FirstOrDefault(c => c.Id == id);

            if (classRoom == null)
            {
                pnlForm.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            if (classRoom.GradeLevel == null
                || EnumHelper.GetStageForGradeLevel(classRoom.GradeLevel) != EnumHelper.GetCurrentStage())
            {
                pnlForm.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            hfClassId.Value = classRoom.Id.ToString();
            ddlGradeLevel.SelectedValue = classRoom.GradeLevelId.ToString();
            ddlAcademicYear.SelectedValue = classRoom.AcademicYearId.ToString();

            ConfigureSectionFilter();

            if (pnlSectionFilter.Visible)
                SelectSection(classRoom.SectionName);

            SelectDivision(classRoom.DivisionName);
        }

        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (!int.TryParse(hfClassId.Value, out id))
                    throw new InvalidOperationException("معرّف الفصل غير صالح.");

                var classRoom = _context.ClassRooms.Find(id);
                if (classRoom == null)
                {
                    lblMessage.CssClass = "alert alert-danger d-block";
                    lblMessage.Text = "الفصل غير موجود!";
                    return;
                }

                int gradeLevelId;
                if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                    throw new InvalidOperationException("يرجى اختيار الصف.");

                var gradeLevel = _context.GradeLevels.Find(gradeLevelId);
                if (gradeLevel == null || gradeLevel.Stage != EnumHelper.GetCurrentStage())
                    throw new InvalidOperationException("الصف المختار لا ينتمي إلى المرحلة التعليمية الحالية.");

                int academicYearId;
                if (!int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId) || academicYearId <= 0)
                    throw new InvalidOperationException("يرجى اختيار السنة الدراسية.");

                string sectionName = IsSecondaryGrade(gradeLevelId)
                    ? (GetSelectedSection() ?? "")
                    : "";

                string divisionName = GetSelectedDivision() ?? "";

                string className = BuildClassName(gradeLevel.Name, sectionName, divisionName);

                classRoom.Name = className;
                classRoom.GradeLevelId = gradeLevelId;
                classRoom.AcademicYearId = academicYearId;
                classRoom.SectionName = sectionName;
                classRoom.DivisionName = divisionName;

                _context.SaveChanges();

                lblMessage.CssClass = "alert alert-success d-block";
                lblMessage.Text = "تم تعديل بيانات الفصل بنجاح!";

                Response.Redirect("Default.aspx?stage=" + hfStage.Value, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "alert alert-danger d-block";
                lblMessage.Text = ErrorMessageHelper.GetUserMessage(ex, GetType().FullName);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?stage=" + hfStage.Value, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }
    }
}