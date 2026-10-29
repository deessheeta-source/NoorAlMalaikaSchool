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
                    .Include(s => s.Parent)
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

                    case "WhatsAppGuardian":
                        OpenWhatsAppForGuardian(id, encodedStage);
                        break;

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

        protected bool CanUseWhatsApp()
        {
            return SecurityHelper.CanAccess(
                UserRole.Admin, UserRole.Accountant, UserRole.Supervisor, UserRole.DataEntry);
        }

        protected bool HasGuardianPhone(object parent)
        {
            NoorAlMalaikaSchool.Models.Parent guardian =
                parent as NoorAlMalaikaSchool.Models.Parent;
            return guardian != null && !string.IsNullOrWhiteSpace(guardian.Phone);
        }

        private void OpenWhatsAppForGuardian(int studentId, string encodedStage)
        {
            var student = _context.Students.AsNoTracking()
                .Where(s => s.Id == studentId)
                .Select(s => new
                {
                    s.Id,
                    s.ClassRoomId,
                    ParentPhone = s.Parent.Phone
                })
                .FirstOrDefault();

            if (student == null)
            {
                ShowFlashInPage("الطالب غير موجود.", "danger");
                return;
            }

            if (string.IsNullOrWhiteSpace(student.ParentPhone))
            {
                ShowFlashInPage("لا يوجد رقم هاتف مسجل لولي الأمر.", "danger");
                return;
            }

            string url = ResolveUrl("~/Pages/WhatsApp/Default.aspx") +
                "?view=compose&audience=Individual&studentId=" + student.Id +
                "&classId=" + student.ClassRoomId +
                "&stage=" + encodedStage;

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
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