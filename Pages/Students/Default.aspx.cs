using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;

using W = DocumentFormat.OpenXml.Wordprocessing;
using WJustify = DocumentFormat.OpenXml.Wordprocessing.JustificationValues;
using WBorder = DocumentFormat.OpenXml.Wordprocessing.BorderValues;
using S = DocumentFormat.OpenXml.Spreadsheet;
using SBorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues;
using SFillVal = DocumentFormat.OpenXml.Spreadsheet.PatternValues;
using SHAlignVal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues;
using SVAlignVal = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Students
{
    public partial class Default : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();
        private const string ReportFont = "Droid Arabic Kufi";
        private const string FlashMessageKey = "Students_FlashMessage";

        // =========================================================
        // Page Lifecycle
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            try
            {
                if (IsPostBack) return;

                string stage = Request.QueryString["stage"];
                if (!string.IsNullOrWhiteSpace(stage)) Session["Stage"] = stage;
                else if (Session["Stage"] != null) stage = Convert.ToString(Session["Stage"]);

                if (!string.IsNullOrWhiteSpace(stage))
                {
                    try { lblStage.Text = "المستوى: " + EnumHelper.GetStageArabic(stage); }
                    catch (Exception exStage) { LogError(exStage); lblStage.Text = "المستوى: " + stage; }
                    lblStage.Visible = true;
                }
                else
                {
                    lblStage.Visible = false;
                }

                LoadClassRooms(stage);
                ResetSectionFilter();
                LoadClassRoomFilter();
                LoadStudents(stage);

                TryShowFlashMessage();
            }
            catch (Exception ex)
            {
                LogError(ex);
                pnlNoResults.Visible = true;
                lblCount.Text = "تعذر تحميل البيانات.";
            }
        }

        // =========================================================
        // Flash Message
        // =========================================================
        private void TryShowFlashMessage()
        {
            try
            {
                var flash = Session[FlashMessageKey] as Delete.FlashMessage;
                if (flash == null) return;

                Session.Remove(FlashMessageKey);

                if (lblFlashMessage == null) return;

                lblFlashMessage.CssClass = flash.Type == "success"
                    ? "students-flash success"
                    : "students-flash danger";

                string iconClass = flash.Type == "success" ? "fa-check-circle" : "fa-exclamation-triangle";
                lblFlashMessage.Text =
                    "<i class='fas " + iconClass + "'></i>" +
                    "<span>" + Server.HtmlEncode(flash.Text) + "</span>";
            }
            catch { }
        }

        private void SetFlashMessage(string text, string type)
        {
            try
            {
                Session[FlashMessageKey] = new Delete.FlashMessage
                {
                    Text = text,
                    Type = type
                };
            }
            catch { }
        }

        private void ShowFlashInPage(string text, string type)
        {
            if (lblFlashMessage == null) return;

            lblFlashMessage.CssClass = type == "success"
                ? "students-flash success"
                : "students-flash danger";

            string icon = type == "success" ? "fa-check-circle" : "fa-exclamation-triangle";

            lblFlashMessage.Text =
                "<i class='fas " + icon + "'></i>" +
                "<span>" + Server.HtmlEncode(text) + "</span>";
        }

        // =========================================================
        // Formatting Helpers
        // =========================================================
        protected string FormatGender(object gender)
        {
            if (gender == null || gender == DBNull.Value) return "—";
            try { return (Gender)gender == Gender.Male ? "ذكر" : "أنثى"; }
            catch { return "—"; }
        }

        protected string FormatClassRoom(object classRoom)
        {
            if (classRoom == null || classRoom == DBNull.Value) return "غير محدد";

            try
            {
                var type = classRoom.GetType();
                var gradeLevelProp = type.GetProperty("GradeLevel");

                if (gradeLevelProp != null)
                {
                    var gradeLevel = gradeLevelProp.GetValue(classRoom);
                    if (gradeLevel != null)
                    {
                        var nameProp = gradeLevel.GetType().GetProperty("Name");
                        if (nameProp != null)
                        {
                            var gradeName = nameProp.GetValue(gradeLevel) as string;
                            if (!string.IsNullOrWhiteSpace(gradeName))
                                return gradeName.Trim();
                        }
                    }
                }

                var prop = type.GetProperty("Name");
                if (prop == null) return "غير محدد";

                var name = prop.GetValue(classRoom) as string;
                if (string.IsNullOrWhiteSpace(name)) return "غير محدد";

                int dashIndex = name.IndexOf(" - ");
                if (dashIndex > 0) name = name.Substring(0, dashIndex).Trim();

                return name;
            }
            catch { return "غير محدد"; }
        }

        protected string FormatSection(object section)
        {
            if (section == null || section == DBNull.Value) return "—";
            string value = Convert.ToString(section);
            if (string.IsNullOrWhiteSpace(value)) return "—";
            if (value == "علمي") return "القسم العلمي";
            if (value == "أدبي") return "القسم الأدبي";
            return value;
        }

        protected string FormatSectionBadge(object section)
        {
            if (section == null || section == DBNull.Value)
                return "<span class='text-muted'>—</span>";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class='text-muted'>—</span>";

            if (value == "علمي")
                return "<span class='section-badge' style='color:#0d6efd;'>القسم العلمي</span>";

            if (value == "أدبي")
                return "<span class='section-badge' style='color:#5a2fa0;'>القسم الأدبي</span>";

            return "<span class='section-badge'>" + System.Web.HttpUtility.HtmlEncode(value) + "</span>";
        }

        protected bool SafeToBool(object value)
        {
            if (value == null || value == DBNull.Value) return false;
            try { return Convert.ToBoolean(value); } catch { return false; }
        }

        protected string FormatSectionRoom(object classRoom)
        {
            if (classRoom == null || classRoom == DBNull.Value)
                return "<span class='text-muted'>—</span>";

            try
            {
                var type = classRoom.GetType();
                var divProp = type.GetProperty("DivisionName");
                string division = divProp != null ? divProp.GetValue(classRoom) as string : null;

                if (!string.IsNullOrWhiteSpace(division))
                {
                    string display = "شعبة " + division.Trim();
                    return "<span class='class-badge'>" + System.Web.HttpUtility.HtmlEncode(display) + "</span>";
                }

                var nameProp = type.GetProperty("Name");
                string name = nameProp != null ? nameProp.GetValue(classRoom) as string : null;
                if (string.IsNullOrWhiteSpace(name))
                    return "<span class='text-muted'>—</span>";

                name = name.Trim();
                int idx = name.IndexOf("شعبة", StringComparison.Ordinal);
                if (idx < 0) return "<span class='text-muted'>—</span>";

                string sectionName = name.Substring(idx).Trim();
                return "<span class='class-badge'>" + System.Web.HttpUtility.HtmlEncode(sectionName) + "</span>";
            }
            catch { return "<span class='text-muted'>—</span>"; }
        }

        private string ExtractDivisionLetter(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            int idx = classRoomName.IndexOf("شعبة", StringComparison.Ordinal);
            if (idx < 0) return null;

            string after = classRoomName.Substring(idx + "شعبة".Length).Trim();
            if (string.IsNullOrWhiteSpace(after)) return null;

            return after[0].ToString();
        }

        private string ExtractSectionFromName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            if (classRoomName.Contains("علمي")) return "علمي";
            if (classRoomName.Contains("أدبي")) return "أدبي";

            return null;
        }

        // =========================================================
        // Data Loading
        // =========================================================
        private void LoadClassRooms(string stage)
        {
            try
            {
                IQueryable<GradeLevel> query = _context.GradeLevels.AsNoTracking();

                EducationStage stageEnum;
                if (!string.IsNullOrWhiteSpace(stage) && Enum.TryParse(stage, true, out stageEnum))
                    query = query.Where(g => g.Stage == stageEnum);

                var gradeLevels = query.OrderBy(g => g.SortOrder).ToList();

                ddlClassRoomFilter.Items.Clear();
                ddlClassRoomFilter.Items.Add(new ListItem("جميع الصفوف", "0"));

                foreach (var grade in gradeLevels)
                    ddlClassRoomFilter.Items.Add(new ListItem(grade.Name, grade.Id.ToString()));
            }
            catch (Exception ex)
            {
                LogError(ex);
                ddlClassRoomFilter.Items.Clear();
                ddlClassRoomFilter.Items.Add(new ListItem("جميع الصفوف", "0"));
            }
        }

        private void LoadClassRoomFilter()
        {
            try
            {
                int gradeLevelId = GetSelectedGradeLevelId();
                string section = GetSelectedSection();
                string previousValue = ddlClassRoom.SelectedValue;

                IQueryable<ClassRoom> query = _context.ClassRooms.AsNoTracking();

                if (gradeLevelId > 0)
                {
                    query = query.Where(c => c.GradeLevelId == gradeLevelId);
                }
                else
                {
                    string stage = GetCurrentStage();
                    EducationStage stageEnum;
                    if (!string.IsNullOrWhiteSpace(stage) && Enum.TryParse(stage, true, out stageEnum))
                    {
                        var gradeIds = _context.GradeLevels
                            .Where(g => g.Stage == stageEnum)
                            .Select(g => g.Id).ToList();

                        query = query.Where(c => gradeIds.Contains(c.GradeLevelId));
                    }
                }

                if (!string.IsNullOrWhiteSpace(section))
                    query = query.Where(c => c.SectionName == section || c.Name.Contains(section));

                var rooms = query
                    .OrderBy(c => c.GradeLevelId)
                    .ThenBy(c => c.DivisionName)
                    .ThenBy(c => c.Name)
                    .Select(c => new { c.Id, c.Name, c.DivisionName, c.SectionName })
                    .ToList();

                var roomInfos = rooms.Select(r => new
                {
                    r.Id,
                    r.Name,
                    Letter = !string.IsNullOrWhiteSpace(r.DivisionName)
                                ? r.DivisionName.Trim()
                                : ExtractDivisionLetter(r.Name),
                    Section = !string.IsNullOrWhiteSpace(r.SectionName)
                                ? r.SectionName.Trim()
                                : ExtractSectionFromName(r.Name)
                }).ToList();

                char nextLetter = 'أ';
                var finalRooms = roomInfos.Select(r =>
                {
                    string letter = !string.IsNullOrWhiteSpace(r.Letter)
                        ? r.Letter
                        : (nextLetter++).ToString();

                    return new { r.Id, Letter = letter, Section = r.Section };
                }).ToList();

                var duplicateLetters = finalRooms
                    .GroupBy(r => r.Letter)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                ddlClassRoom.Items.Clear();
                ddlClassRoom.Items.Add(new ListItem("جميع الشعب", ""));

                var added = new HashSet<string>();

                foreach (var r in finalRooms)
                {
                    string displayName = (duplicateLetters.Contains(r.Letter) && !string.IsNullOrWhiteSpace(r.Section))
                        ? "شعبة " + r.Letter + " - " + r.Section
                        : "شعبة " + r.Letter;

                    if (added.Contains(displayName)) continue;

                    added.Add(displayName);
                    ddlClassRoom.Items.Add(new ListItem(displayName, r.Id.ToString()));
                }

                ListItem prev = ddlClassRoom.Items.FindByValue(previousValue);
                if (prev != null)
                {
                    ddlClassRoom.ClearSelection();
                    prev.Selected = true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ddlClassRoom.Items.Clear();
                ddlClassRoom.Items.Add(new ListItem("جميع الشعب", ""));
            }
        }

        private int GetSelectedGradeLevelId()
        {
            int gradeLevelId;
            if (!int.TryParse(ddlClassRoomFilter.SelectedValue, out gradeLevelId)) return 0;
            return gradeLevelId > 0 ? gradeLevelId : 0;
        }

        private int GetSelectedClassRoomId()
        {
            if (ddlClassRoom == null) return 0;
            int classRoomId;
            if (!int.TryParse(ddlClassRoom.SelectedValue, out classRoomId)) return 0;
            return classRoomId > 0 ? classRoomId : 0;
        }

        private bool IsSecondaryGrade(int gradeLevelId)
        {
            if (gradeLevelId <= 0) return false;
            try
            {
                string gradeName = _context.GradeLevels.AsNoTracking()
                    .Where(g => g.Id == gradeLevelId)
                    .Select(g => g.Name).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(gradeName)) return false;
                gradeName = gradeName.Trim();
                return gradeName == "ثاني ثانوي" || gradeName == "ثالث ثانوي";
            }
            catch (Exception ex) { LogError(ex); return false; }
        }

        private void ConfigureSectionFilter()
        {
            int gradeLevelId = GetSelectedGradeLevelId();
            bool showSection = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = showSection;
            if (!showSection) SelectSection("0");
        }

        private void ResetSectionFilter()
        {
            SelectSection("0");
            pnlSectionFilter.Visible = false;
        }

        private void SelectSection(string value)
        {
            if (ddlSectionFilter == null) return;
            ListItem item = ddlSectionFilter.Items.FindByValue(value);
            if (item != null) { ddlSectionFilter.ClearSelection(); item.Selected = true; }
            else if (ddlSectionFilter.Items.Count > 0) ddlSectionFilter.SelectedIndex = 0;
        }

        private string GetSelectedSection()
        {
            if (ddlSectionFilter == null) return null;
            string value = ddlSectionFilter.SelectedValue;
            if (string.IsNullOrWhiteSpace(value) || value == "0") return null;
            return value;
        }

        private string GetCurrentStage()
        {
            return Session["Stage"] == null ? null : Convert.ToString(Session["Stage"]);
        }

        private string GetStageDisplayName()
        {
            string stage = GetCurrentStage();
            if (string.IsNullOrWhiteSpace(stage)) return "الأساسية";
            try { return EnumHelper.GetStageArabic(stage); }
            catch { return stage; }
        }

        private IQueryable<Student> BuildStudentsQuery(string stage)
        {
            var query = _context.Students.AsNoTracking().AsQueryable();

            EducationStage stageEnum;
            if (!string.IsNullOrWhiteSpace(stage) && Enum.TryParse(stage, true, out stageEnum))
                query = query.Where(s => s.Stage == stageEnum);

            string search = txtSearch.Text.Trim();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.Name.Contains(search));

            string studentNo = txtStudentNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(studentNo))
                query = query.Where(s => s.RegistrationNo == studentNo);

            int gradeLevelId = GetSelectedGradeLevelId();
            if (gradeLevelId > 0)
                query = query.Where(s => s.ClassRoom != null && s.ClassRoom.GradeLevelId == gradeLevelId);

            if (gradeLevelId > 0 && IsSecondaryGrade(gradeLevelId))
            {
                string section = GetSelectedSection();
                if (!string.IsNullOrWhiteSpace(section))
                    query = query.Where(s => s.Section == section ||
                                             (s.Section == null && s.ClassRoom.SectionName == section));
            }

            int classRoomId = GetSelectedClassRoomId();
            if (classRoomId > 0)
                query = query.Where(s => s.ClassRoomId == classRoomId);

            return query;
        }

        private void LoadStudents(string stage)
        {
            try
            {
                gvStudents.AllowCustomPaging = true;
                var query = BuildStudentsQuery(stage);
                int totalCount = query.Count();

                var students = query
                    .Include(s => s.ClassRoom.GradeLevel)
                    .OrderBy(s => s.Name).ThenBy(s => s.Id)
                    .Skip(gvStudents.PageIndex * gvStudents.PageSize)
                    .Take(gvStudents.PageSize)
                    .ToList();

                gvStudents.VirtualItemCount = totalCount;
                gvStudents.DataSource = students;
                gvStudents.DataBind();

                lblCount.Text = totalCount.ToString();
                pnlNoResults.Visible = totalCount == 0;
            }
            catch (Exception ex)
            {
                LogError(ex);
                gvStudents.DataSource = null;
                gvStudents.DataBind();
                pnlNoResults.Visible = true;
                lblCount.Text = "تعذر تحميل الطلاب.";
            }
        }

        // =========================================================
        // Events
        // =========================================================
        protected void btnAddStudent_Click(object sender, EventArgs e)
        {
            string stage = GetCurrentStage() ?? "";
            Response.Redirect("Create.aspx?stage=" + Server.UrlEncode(stage), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvStudents.PageIndex = 0;
            ConfigureSectionFilter();
            LoadClassRoomFilter();
            LoadStudents(GetCurrentStage());
        }

        protected void ddlClassRoomFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvStudents.PageIndex = 0;
            ConfigureSectionFilter();
            LoadClassRoomFilter();
            LoadStudents(GetCurrentStage());
        }

        protected void ddlSectionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvStudents.PageIndex = 0;
            ConfigureSectionFilter();
            LoadClassRoomFilter();
            LoadStudents(GetCurrentStage());
        }

        protected void ddlClassRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvStudents.PageIndex = 0;
            LoadStudents(GetCurrentStage());
        }

        protected void gvStudents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandArgument == null) return;
                int id;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

                string encodedStage = Server.UrlEncode(GetCurrentStage() ?? "");

                switch (e.CommandName)
                {
                    case "EditStudent":
                        Response.Redirect("Edit.aspx?id=" + id + "&stage=" + encodedStage, false);
                        Context.ApplicationInstance.CompleteRequest(); break;

                    case "ViewDetails":
                        Response.Redirect("Details.aspx?id=" + id + "&stage=" + encodedStage, false);
                        Context.ApplicationInstance.CompleteRequest(); break;

                    case "DeleteStudent":
                        Response.Redirect("Delete.aspx?id=" + id + "&stage=" + encodedStage, false);
                        Context.ApplicationInstance.CompleteRequest(); break;

                    case "DeleteHard":
                        Response.Redirect("Delete.aspx?id=" + id + "&mode=hard&stage=" + encodedStage, false);
                        Context.ApplicationInstance.CompleteRequest(); break;

                    case "ToggleStatus":
                        ToggleStudentStatus(id, encodedStage);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                lblCount.Text = "تعذر تنفيذ العملية.";
            }
        }

        private void ToggleStudentStatus(int id, string encodedStage)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                ShowFlashInPage("الطالب غير موجود.", "danger");
                return;
            }

            bool wasActive = student.IsActive;
            student.IsActive = !student.IsActive;
            _context.SaveChanges();

            string action = wasActive ? "تعطيل" : "إعادة تفعيل";
            string studentName = student.Name ?? "";

            SetFlashMessage("تم " + action + " الطالب: " + studentName, "success");
            ShowFlashInPage("✅ تم " + action + " الطالب بنجاح!", "success");

            LoadStudents(GetCurrentStage());
            ScheduleSelfRedirectAfterDelay(encodedStage);
        }

        private void ScheduleSelfRedirectAfterDelay(string encodedStage)
        {
            int gradeId = GetSelectedGradeLevelId();
            string section = GetSelectedSection() ?? "";
            int classRoomId = GetSelectedClassRoomId();

            List<string> qs = new List<string>();
            qs.Add("stage=" + encodedStage);
            if (gradeId > 0) qs.Add("grade=" + gradeId);
            if (!string.IsNullOrWhiteSpace(section)) qs.Add("section=" + Server.UrlEncode(section));
            if (classRoomId > 0) qs.Add("classRoom=" + classRoomId);

            string url = "Default.aspx?" + string.Join("&", qs);

            string script =
                "setTimeout(function(){ " +
                "  window.location.href = '" + url.Replace("'", "\\'") + "'; " +
                "}, 1800);";

            try
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "selfRedirectAfterDelay",
                    script,
                    true);
            }
            catch { }
        }

        protected void gvStudents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.CssClass = (e.Row.CssClass + " students-table-header-row").Trim();
                return;
            }

            if (e.Row.RowType == DataControlRowType.Pager)
            {
                e.Row.CssClass = (e.Row.CssClass + " students-table-pager-row").Trim();
                return;
            }

            if (e.Row.RowType != DataControlRowType.DataRow) return;

            string[] mobileLabels =
            {
                "اسم الطالب", "رقم التسجيل", "الرقم الوطني", "الصف", "القسم",
                "الشعبة", "الجنس", "الحالة", "إجراءات"
            };
            for (int i = 0; i < e.Row.Cells.Count && i < mobileLabels.Length; i++)
                e.Row.Cells[i].Attributes["data-label"] = mobileLabels[i];

            try
            {
                var student = e.Row.DataItem as Student;
                if (student == null) return;

                if (!student.IsActive)
                    e.Row.Style["background-color"] = "#fef2f2";

                var toggle = e.Row.FindControl("lnkToggle") as LinkButton;
                if (toggle == null) return;

                bool active = student.IsActive;
                toggle.ToolTip = active ? "تعطيل الطالب" : "إعادة تفعيل الطالب";
                toggle.OnClientClick = active
                    ? "return confirm('هل أنت متأكد من تعطيل هذا الطالب؟');"
                    : "return confirm('هل تريد إعادة تفعيل هذا الطالب؟');";
            }
            catch (Exception ex) { LogError(ex); }
        }

        protected void gvStudents_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudents.PageIndex = e.NewPageIndex;
            LoadStudents(GetCurrentStage());
        }

        // =========================================================
        // Export — Data Model
        // =========================================================
        private class StudentExportRow
        {
            public string Name { get; set; }
            public string RegistrationNo { get; set; }
            public string NationalId { get; set; }
            public string ClassName { get; set; }
            public string SectionText { get; set; }
            public string GenderName { get; set; }
            public string Nationality { get; set; }
            public string StatusName { get; set; }
        }

        private List<StudentExportRow> BuildExportRows()
        {
            var query = BuildStudentsQuery(GetCurrentStage());

            var raw = query
                .Include(s => s.ClassRoom.GradeLevel)
                .OrderBy(s => s.Name).ThenBy(s => s.Id)
                .Select(s => new
                {
                    s.Name,
                    s.RegistrationNo,
                    s.NationalId,
                    ClassName = (s.ClassRoom != null && s.ClassRoom.GradeLevel != null) ? s.ClassRoom.GradeLevel.Name : "",
                    s.Section,
                    ClassRoomSection = s.ClassRoom != null ? s.ClassRoom.SectionName : null,
                    s.Gender,
                    s.Nationality,
                    s.IsActive
                })
                .ToList();

            return raw.Select(s => new StudentExportRow
            {
                Name = s.Name,
                RegistrationNo = s.RegistrationNo,
                NationalId = s.NationalId,
                ClassName = s.ClassName,
                SectionText = FormatSection(!string.IsNullOrWhiteSpace(s.Section) ? s.Section : s.ClassRoomSection),
                GenderName = FormatGender(s.Gender),
                Nationality = s.Nationality,
                StatusName = s.IsActive ? "نشط" : "ملغي"
            }).ToList();
        }

        protected void btnExportWord_Click(object sender, EventArgs e)
        {
            var rows = BuildExportRows();

            byte[] docxBytes = BuildDocxStudentReport(
                rows, GetStageDisplayName(),
                ddlClassRoomFilter.SelectedItem != null && ddlClassRoomFilter.SelectedIndex > 0 ? ddlClassRoomFilter.SelectedItem.Text : "",
                ddlSectionFilter.SelectedItem != null && ddlSectionFilter.SelectedIndex > 0 ? ddlSectionFilter.SelectedItem.Text : "",
                ddlClassRoom.SelectedItem != null && ddlClassRoom.SelectedIndex > 0 ? ddlClassRoom.SelectedItem.Text : "");

            string fileName = "StudentsReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".docx";

            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.AddHeader("Content-Length", docxBytes.Length.ToString());
            Response.BinaryWrite(docxBytes);
            Response.Flush();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            var rows = BuildExportRows();

            byte[] xlsxBytes = BuildXlsxStudentReport(
                rows, GetStageDisplayName(),
                ddlClassRoomFilter.SelectedItem != null && ddlClassRoomFilter.SelectedIndex > 0 ? ddlClassRoomFilter.SelectedItem.Text : "",
                ddlSectionFilter.SelectedItem != null && ddlSectionFilter.SelectedIndex > 0 ? ddlSectionFilter.SelectedItem.Text : "",
                ddlClassRoom.SelectedItem != null && ddlClassRoom.SelectedIndex > 0 ? ddlClassRoom.SelectedItem.Text : "");

            string fileName = "StudentsReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";

            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.AddHeader("Content-Length", xlsxBytes.Length.ToString());
            Response.BinaryWrite(xlsxBytes);
            Response.Flush();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest();
        }

        // =========================================================
        // Word Builder
        // =========================================================
        private byte[] BuildDocxStudentReport(List<StudentExportRow> rows, string stageDisplay,
            string gradeLevelName, string sectionDisplay, string classRoomName)
        {
            using (var ms = new MemoryStream())
            {
                using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                {
                    var mainPart = doc.AddMainDocumentPart();
                    AddDefaultFontStyle(mainPart);
                    mainPart.Document = new W.Document();
                    var body = mainPart.Document.AppendChild(new W.Body());

                    var sectionProps = new W.SectionProperties(
                        new W.PageSize { Width = 16838, Height = 11906, Orient = W.PageOrientationValues.Landscape },
                        new W.PageMargin { Top = 850, Right = 850, Bottom = 850, Left = 850, Header = 720, Footer = 720, Gutter = 0 });

                    body.AppendChild(CreateParagraph("مدرسة نور الملائكة", 32, true, "00B14F", WJustify.Center));
                    body.AppendChild(CreateParagraph("عدد الطلاب : " + rows.Count, 20, true, "495057", WJustify.Center));
                    body.AppendChild(CreateParagraph("قائمة طلاب المرحلة : " + stageDisplay, 20, true, "212529", WJustify.Center));

                    var filters = new List<string>();
                    if (!string.IsNullOrWhiteSpace(gradeLevelName) && gradeLevelName != "جميع الصفوف") filters.Add("الصف: " + gradeLevelName);
                    if (!string.IsNullOrWhiteSpace(sectionDisplay) && sectionDisplay != "جميع الأقسام") filters.Add("القسم: " + sectionDisplay);
                    if (!string.IsNullOrWhiteSpace(classRoomName) && classRoomName != "جميع الشعب") filters.Add("الشعبة: " + classRoomName);

                    if (filters.Count > 0)
                        body.AppendChild(CreateParagraph("الفلاتر : " + string.Join(" | ", filters), 20, true, "212529", WJustify.Center));

                    body.AppendChild(new W.Paragraph());

                    var table = CreateTable(new int[] { 2400, 1300, 1500, 1300, 1300, 900, 1200, 1100 });
                    table.AppendChild(CreateRow(new[] { "اسم الطالب", "رقم التسجيل", "الرقم الوطني", "الصف", "القسم", "الجنس", "الجنسية", "الحالة" }, true));

                    int idx = 1;
                    foreach (var r in rows)
                    {
                        table.AppendChild(CreateRow(new[]
                        {
                            r.Name ?? "", r.RegistrationNo ?? "", r.NationalId ?? "", r.ClassName ?? "",
                            r.SectionText ?? "", r.GenderName ?? "", r.Nationality ?? "", r.StatusName ?? ""
                        }, false, idx % 2 == 0));
                        idx++;
                    }

                    body.AppendChild(table);
                    body.AppendChild(new W.Paragraph());
                    body.AppendChild(CreateParagraph("تم إنشاء التقرير تلقائياً من نظام مدرسة نور الملائكة", 16, false, "868E96", WJustify.Center));
                    body.AppendChild(sectionProps);
                    mainPart.Document.Save();
                }
                return ms.ToArray();
            }
        }

        // =========================================================
        // Excel Builder
        // =========================================================
        private byte[] BuildXlsxStudentReport(List<StudentExportRow> rows, string stageDisplay,
            string gradeLevelName, string sectionDisplay, string classRoomName)
        {
            using (var ms = new MemoryStream())
            {
                using (var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new S.Workbook();

                    var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = CreateXlsxStylesheet();
                    stylesPart.Stylesheet.Save();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new S.SheetData();

                    sheetData.AppendChild(CreateXlsxRow(1U, 32D, new[] { "مدرسة نور الملائكة" }, 1U, true));
                    sheetData.AppendChild(CreateXlsxRow(2U, 22D, new[] { "عدد الطلاب : " + rows.Count }, 2U, true));
                    sheetData.AppendChild(CreateXlsxRow(3U, 22D, new[] { "قائمة طلاب المرحلة : " + stageDisplay }, 2U, true));

                    var filters = new List<string>();
                    if (!string.IsNullOrWhiteSpace(gradeLevelName) && gradeLevelName != "جميع الصفوف") filters.Add("الصف: " + gradeLevelName);
                    if (!string.IsNullOrWhiteSpace(sectionDisplay) && sectionDisplay != "جميع الأقسام") filters.Add("القسم: " + sectionDisplay);
                    if (!string.IsNullOrWhiteSpace(classRoomName) && classRoomName != "جميع الشعب") filters.Add("الشعبة: " + classRoomName);

                    string filterText = filters.Count > 0 ? "الفلاتر : " + string.Join(" | ", filters) : "الفلاتر : الكل";
                    sheetData.AppendChild(CreateXlsxRow(4U, 22D, new[] { filterText }, 2U, true));
                    sheetData.AppendChild(new S.Row { RowIndex = 5U });

                    sheetData.AppendChild(CreateXlsxRow(6U, 28D,
                        new[] { "اسم الطالب", "رقم التسجيل", "الرقم الوطني", "الصف", "القسم", "الجنس", "الجنسية", "الحالة" },
                        3U, false));

                    uint currentRow = 7U;
                    int idx = 1;
                    foreach (var r in rows)
                    {
                        sheetData.AppendChild(CreateXlsxRow(currentRow, 22D, new[]
                        {
                            r.Name ?? "", r.RegistrationNo ?? "", r.NationalId ?? "", r.ClassName ?? "",
                            r.SectionText ?? "", r.GenderName ?? "", r.Nationality ?? "", r.StatusName ?? ""
                        }, idx % 2 == 0 ? 5U : 4U, false));
                        currentRow++; idx++;
                    }

                    var columns = new S.Columns();
                    double[] widths = { 30, 15, 18, 15, 15, 10, 12, 12 };
                    for (int i = 0; i < widths.Length; i++)
                        columns.AppendChild(new S.Column { Min = (uint)(i + 1), Max = (uint)(i + 1), Width = widths[i], CustomWidth = true });

                    var mergeCells = new S.MergeCells();
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A1:H1" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A2:H2" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A3:H3" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A4:H4" });
                    mergeCells.Count = 4U;

                    var sheetView = new S.SheetView { RightToLeft = true, WorkbookViewId = 0U, ShowGridLines = false };
                    sheetView.AppendChild(new S.Pane { VerticalSplit = 6D, TopLeftCell = "A7", ActivePane = S.PaneValues.BottomLeft, State = S.PaneStateValues.Frozen });
                    sheetView.AppendChild(new S.Selection { Pane = S.PaneValues.BottomLeft });

                    var sheetViews = new S.SheetViews();
                    sheetViews.AppendChild(sheetView);

                    worksheetPart.Worksheet = new S.Worksheet(sheetViews, columns, sheetData, mergeCells);

                    var sheets = workbookPart.Workbook.AppendChild(new S.Sheets());
                    sheets.AppendChild(new S.Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1U, Name = "قائمة الطلاب" });

                    workbookPart.Workbook.Save();
                }
                return ms.ToArray();
            }
        }

        private S.Row CreateXlsxRow(uint rowIndex, double height, string[] values, uint styleIndex, bool mergedTitleOnly)
        {
            var row = new S.Row { RowIndex = rowIndex, Height = height, CustomHeight = true };
            string[] cols = { "A", "B", "C", "D", "E", "F", "G", "H" };

            if (mergedTitleOnly)
                row.AppendChild(CreateInlineTextCell("A" + rowIndex, values[0], styleIndex));
            else
                for (int i = 0; i < values.Length && i < cols.Length; i++)
                    row.AppendChild(CreateInlineTextCell(cols[i] + rowIndex, values[i], styleIndex));

            return row;
        }

        private S.Cell CreateInlineTextCell(string reference, string text, uint styleIndex)
        {
            return new S.Cell
            {
                CellReference = reference,
                DataType = S.CellValues.InlineString,
                StyleIndex = styleIndex,
                InlineString = new S.InlineString(new S.Text(text ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve })
            };
        }

        private S.Stylesheet CreateXlsxStylesheet()
        {
            var stylesheet = new S.Stylesheet();

            var fonts = new S.Fonts();
            fonts.AppendChild(new S.Font(new S.FontSize { Val = 11D }, new S.FontName { Val = ReportFont }));
            fonts.AppendChild(new S.Font(new S.Bold(), new S.FontSize { Val = 16D }, new S.Color { Rgb = "FF00B14F" }, new S.FontName { Val = ReportFont }));
            fonts.AppendChild(new S.Font(new S.Bold(), new S.FontSize { Val = 11D }, new S.Color { Rgb = "FFFFFFFF" }, new S.FontName { Val = ReportFont }));
            fonts.AppendChild(new S.Font(new S.FontSize { Val = 10D }, new S.Color { Rgb = "FF495057" }, new S.FontName { Val = ReportFont }));
            fonts.Count = 4U;
            stylesheet.AppendChild(fonts);

            var fills = new S.Fills();
            fills.AppendChild(new S.Fill(new S.PatternFill { PatternType = SFillVal.None }));
            fills.AppendChild(new S.Fill(new S.PatternFill { PatternType = SFillVal.Gray125 }));
            fills.AppendChild(new S.Fill(new S.PatternFill { PatternType = SFillVal.Solid, ForegroundColor = new S.ForegroundColor { Rgb = "FF00B14F" } }));
            fills.AppendChild(new S.Fill(new S.PatternFill { PatternType = SFillVal.Solid, ForegroundColor = new S.ForegroundColor { Rgb = "FFF9FAFB" } }));
            fills.Count = 4U;
            stylesheet.AppendChild(fills);

            var borders = new S.Borders();
            borders.AppendChild(new S.Border());
            borders.AppendChild(new S.Border(
                new S.LeftBorder(new S.Color { Rgb = "FFD8E6DD" }) { Style = SBorderStyle.Thin },
                new S.RightBorder(new S.Color { Rgb = "FFD8E6DD" }) { Style = SBorderStyle.Thin },
                new S.TopBorder(new S.Color { Rgb = "FFD8E6DD" }) { Style = SBorderStyle.Thin },
                new S.BottomBorder(new S.Color { Rgb = "FFD8E6DD" }) { Style = SBorderStyle.Thin },
                new S.DiagonalBorder()));
            borders.Count = 2U;
            stylesheet.AppendChild(borders);

            var cellFormats = new S.CellFormats();
            cellFormats.AppendChild(new S.CellFormat { FontId = 0U, FillId = 0U, BorderId = 0U });
            cellFormats.AppendChild(new S.CellFormat { FontId = 1U, FillId = 0U, BorderId = 0U, ApplyFont = true, ApplyAlignment = true, Alignment = new S.Alignment { Horizontal = SHAlignVal.Center, Vertical = SVAlignVal.Center, ReadingOrder = 2U } });
            cellFormats.AppendChild(new S.CellFormat { FontId = 3U, FillId = 0U, BorderId = 0U, ApplyFont = true, ApplyAlignment = true, Alignment = new S.Alignment { Horizontal = SHAlignVal.Center, Vertical = SVAlignVal.Center, ReadingOrder = 2U } });
            cellFormats.AppendChild(new S.CellFormat { FontId = 2U, FillId = 2U, BorderId = 1U, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true, Alignment = new S.Alignment { Horizontal = SHAlignVal.Center, Vertical = SVAlignVal.Center, ReadingOrder = 2U, WrapText = true } });
            cellFormats.AppendChild(new S.CellFormat { FontId = 0U, FillId = 0U, BorderId = 1U, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true, Alignment = new S.Alignment { Horizontal = SHAlignVal.Center, Vertical = SVAlignVal.Center, ReadingOrder = 2U } });
            cellFormats.AppendChild(new S.CellFormat { FontId = 0U, FillId = 3U, BorderId = 1U, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true, Alignment = new S.Alignment { Horizontal = SHAlignVal.Center, Vertical = SVAlignVal.Center, ReadingOrder = 2U } });
            cellFormats.Count = 6U;
            stylesheet.AppendChild(cellFormats);

            return stylesheet;
        }

        // =========================================================
        // Word helpers
        // =========================================================
        private void AddDefaultFontStyle(MainDocumentPart mainPart)
        {
            var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            stylesPart.Styles = new W.Styles();
            var docDefaults = new W.DocDefaults();
            var rPrDefaults = new W.RunPropertiesDefault();
            var rPrBase = new W.RunPropertiesBaseStyle();
            rPrBase.AppendChild(CreateRunFonts());
            rPrBase.AppendChild(new W.FontSize { Val = "24" });
            rPrBase.AppendChild(new W.FontSizeComplexScript { Val = "24" });
            rPrDefaults.AppendChild(rPrBase);
            docDefaults.AppendChild(rPrDefaults);
            var pPrDefaults = new W.ParagraphPropertiesDefault();
            var pPrBase = new W.ParagraphPropertiesBaseStyle();
            pPrBase.AppendChild(new W.BiDi());
            pPrDefaults.AppendChild(pPrBase);
            docDefaults.AppendChild(pPrDefaults);
            stylesPart.Styles.AppendChild(docDefaults);
            stylesPart.Styles.Save();
        }

        private W.RunFonts CreateRunFonts()
        {
            return new W.RunFonts
            {
                Ascii = ReportFont,
                HighAnsi = ReportFont,
                ComplexScript = ReportFont,
                EastAsia = ReportFont
            };
        }

        private W.Paragraph CreateParagraph(string text, int fontSize, bool bold, string color, WJustify align)
        {
            var rp = new W.RunProperties(CreateRunFonts(),
                new W.FontSize { Val = fontSize.ToString() },
                new W.FontSizeComplexScript { Val = fontSize.ToString() });

            if (bold) { rp.AppendChild(new W.Bold()); rp.AppendChild(new W.BoldComplexScript()); }
            if (!string.IsNullOrEmpty(color)) rp.AppendChild(new W.Color { Val = color });
            rp.AppendChild(new W.RightToLeftText());
            rp.AppendChild(new W.ComplexScript());

            var run = new W.Run(rp, new W.Text(text ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve });
            var pp = new W.ParagraphProperties(new W.Justification { Val = align }, new W.BiDi());
            return new W.Paragraph(pp, run);
        }

        private W.Table CreateTable(int[] columnWidths)
        {
            var borders = new W.TableBorders(
                new W.TopBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.BottomBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.LeftBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.RightBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.InsideHorizontalBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.InsideVerticalBorder { Val = WBorder.Single, Size = 6, Color = "999999" });

            var table = new W.Table();
            table.AppendChild(new W.TableProperties(
                new W.TableWidth { Width = "5000", Type = W.TableWidthUnitValues.Pct },
                borders,
                new W.TableJustification { Val = W.TableRowAlignmentValues.Center },
                new W.BiDiVisual()));

            var grid = new W.TableGrid();
            foreach (var w in columnWidths) grid.AppendChild(new W.GridColumn { Width = w.ToString() });
            table.AppendChild(grid);
            return table;
        }

        private W.TableRow CreateRow(string[] values, bool isHeader, bool isEven = false)
        {
            var row = new W.TableRow();
            foreach (var val in values)
            {
                var cellProps = new W.TableCellProperties(
                    new W.TableCellVerticalAlignment { Val = W.TableVerticalAlignmentValues.Center },
                    new W.TableCellMargin
                    {
                        TopMargin = new W.TopMargin { Width = "40", Type = W.TableWidthUnitValues.Dxa },
                        BottomMargin = new W.BottomMargin { Width = "40", Type = W.TableWidthUnitValues.Dxa },
                        LeftMargin = new W.LeftMargin { Width = "80", Type = W.TableWidthUnitValues.Dxa },
                        RightMargin = new W.RightMargin { Width = "80", Type = W.TableWidthUnitValues.Dxa }
                    });

                if (isHeader) cellProps.AppendChild(new W.Shading { Val = W.ShadingPatternValues.Clear, Color = "auto", Fill = "00B14F" });
                else if (isEven) cellProps.AppendChild(new W.Shading { Val = W.ShadingPatternValues.Clear, Color = "auto", Fill = "F9FAFB" });

                var rp = new W.RunProperties(CreateRunFonts(),
                    new W.FontSize { Val = "16" },
                    new W.FontSizeComplexScript { Val = "16" });

                if (isHeader)
                {
                    rp.AppendChild(new W.Bold());
                    rp.AppendChild(new W.BoldComplexScript());
                    rp.AppendChild(new W.Color { Val = "FFFFFF" });
                }

                rp.AppendChild(new W.RightToLeftText());
                rp.AppendChild(new W.ComplexScript());

                var pp = new W.ParagraphProperties(new W.Justification { Val = WJustify.Center }, new W.BiDi());
                var para = new W.Paragraph(pp, new W.Run(rp, new W.Text(val ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve }));
                row.AppendChild(new W.TableCell(cellProps, para));
            }
            return row;
        }

        // =========================================================
        // Error Logging / Dispose
        // =========================================================
        private void LogError(Exception ex)
        {
            try
            {
                string path = Server.MapPath("~/App_Data/errors.log");
                if (path == null) return;
                File.AppendAllText(path,
                    "=== " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ===\r\n" +
                    "URL: " + Request.Url + "\r\n" + ex + "\r\n\r\n");
            }
            catch { }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) { try { _context.Dispose(); } catch { } }
            base.OnUnload(e);
        }
    }
}
