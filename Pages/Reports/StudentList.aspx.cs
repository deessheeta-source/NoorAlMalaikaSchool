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
using NoorAlMalaikaSchool.Services;

// Word aliases
using W = DocumentFormat.OpenXml.Wordprocessing;
using WJustify = DocumentFormat.OpenXml.Wordprocessing.JustificationValues;
using WBorder = DocumentFormat.OpenXml.Wordprocessing.BorderValues;

// Excel aliases
using S = DocumentFormat.OpenXml.Spreadsheet;
using SBorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues;
using SFillVal = DocumentFormat.OpenXml.Spreadsheet.PatternValues;
using SHAlignVal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues;
using SVAlignVal = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues;

namespace NoorAlMalaikaSchool.Pages.Reports
{
    public partial class StudentList : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private const string ReportFont = "Droid Arabic Kufi";

        private EducationStage CurrentStage
        {
            get { return GetCurrentStage(); }
        }

           protected void Page_Load(object sender, EventArgs e)
           {
               // Local guard: the student roster is broad but still leaves the school.
               // Do not rely on the Site.Master path check alone.
               SecurityHelper.DenyUnlessPath();
               SecurityHelper.HideViewExportControls(this);

               if (!IsPostBack)
            {
                LoadAcademicYears();
                LoadGradeLevels();
                ResetSectionFilter();
                LoadClassRooms();
                LoadStudents();
            }
        }

        private EducationStage GetCurrentStage()
        {
            string stageParam = Request.QueryString["stage"];
            EducationStage stage;

            if (!string.IsNullOrEmpty(stageParam)
                && Enum.TryParse(stageParam, true, out stage))
            {
                Session["Stage"] = stage.ToString();
                return stage;
            }

            string sessionStage = Session["Stage"] as string;
            if (!string.IsNullOrEmpty(sessionStage)
                && Enum.TryParse(sessionStage, true, out stage))
            {
                return stage;
            }

            return EducationStage.Basic;
        }

        private string GetStageDisplayName()
        {
            EducationStage stage = CurrentStage;
            return stage == EducationStage.Basic ? "الأساسية" : "الثانوية";
        }

        // =========================================================
        // دوال مساعدة للقسم
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

                if (string.IsNullOrWhiteSpace(gradeName))
                    return false;

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

            if (!showSection) SelectSection("0");
        }

        private void ResetSectionFilter()
        {
            SelectSection("0");
            pnlSectionFilter.Visible = false;
        }

        private void SelectSection(string value)
        {
            if (ddlSection == null) return;

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

        private string GetSelectedSection()
        {
            if (ddlSection == null) return null;
            string value = ddlSection.SelectedValue;
            if (string.IsNullOrWhiteSpace(value) || value == "0") return null;
            return value;
        }

        // =========================================================
        // ✅ استخراج حرف الشعبة من اسم القاعة (حل احتياطي فقط)
        // =========================================================
        private string ExtractDivisionLetter(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            int idx = classRoomName.IndexOf("شعبة", StringComparison.Ordinal);
            if (idx < 0) return null;

            string after = classRoomName.Substring(idx + "شعبة".Length).Trim();
            if (string.IsNullOrWhiteSpace(after)) return null;

            return after[0].ToString();
        }

        // =========================================================
        // ✅ استخراج القسم من اسم القاعة (حل احتياطي فقط)
        // =========================================================
        private string ExtractSectionFromName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            if (classRoomName.Contains("علمي")) return "علمي";
            if (classRoomName.Contains("أدبي")) return "أدبي";

            return null;
        }

        // =========================================================
        // ✅ تحميل قائمة الشعب - يقرأ DivisionName مباشرة
        // =========================================================
        private void LoadClassRooms()
        {
            EducationStage stage = CurrentStage;

            int academicYearId;
            int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId);

            int gradeLevelId;
            int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId);

            string selectedSection = GetSelectedSection();
            string previousValue = ddlClassRoom.SelectedValue;

            IQueryable<ClassRoom> query = db.ClassRooms.AsNoTracking();

            if (academicYearId > 0)
                query = query.Where(c => c.AcademicYearId == academicYearId);

            if (gradeLevelId > 0)
            {
                query = query.Where(c => c.GradeLevelId == gradeLevelId);
            }
            else
            {
                var gradeIds = db.GradeLevels
                    .Where(g => g.Stage == stage)
                    .Select(g => g.Id)
                    .ToList();

                query = query.Where(c => gradeIds.Contains(c.GradeLevelId));
            }

            if (!string.IsNullOrWhiteSpace(selectedSection))
                query = query.Where(c => c.SectionName == selectedSection ||
                                          c.Name.Contains(selectedSection));

            // ✅ اجلب DivisionName + SectionName
            var rooms = query
                .OrderBy(c => c.GradeLevelId)
                .ThenBy(c => c.DivisionName)
                .ThenBy(c => c.Name)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.DivisionName,
                    c.SectionName
                })
                .ToList();

            // ✅ استخرج الحرف + القسم (DivisionName له الأولوية)
            var roomInfos = rooms
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    Letter = !string.IsNullOrWhiteSpace(r.DivisionName)
                                ? r.DivisionName.Trim()
                                : ExtractDivisionLetter(r.Name),
                    Section = !string.IsNullOrWhiteSpace(r.SectionName)
                                ? r.SectionName.Trim()
                                : ExtractSectionFromName(r.Name)
                })
                .ToList();

            // ✅ توليد حرف تسلسلي لمن ليس له حرف
            char nextLetter = 'أ';
            var finalRooms = roomInfos
                .Select(r =>
                {
                    string letter = !string.IsNullOrWhiteSpace(r.Letter)
                        ? r.Letter
                        : (nextLetter++).ToString();

                    return new { r.Id, Letter = letter, Section = r.Section };
                })
                .ToList();

            // كشف التكرار (نفس الحرف في أكثر من قسم)
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
                string displayName;

                if (duplicateLetters.Contains(r.Letter) && !string.IsNullOrWhiteSpace(r.Section))
                {
                    // نفس الحرف في أكثر من قسم → أضف القسم للتمييز
                    displayName = "شعبة " + r.Letter + " - " + r.Section;
                }
                else
                {
                    // ✅ العرض المطلوب: "شعبة أ" / "شعبة ب"
                    displayName = "شعبة " + r.Letter;
                }

                if (added.Contains(displayName))
                    continue;

                added.Add(displayName);
                ddlClassRoom.Items.Add(new ListItem(displayName, r.Id.ToString()));
            }

            // استعادة الاختيار السابق
            ListItem prev = ddlClassRoom.Items.FindByValue(previousValue);
            if (prev != null)
            {
                ddlClassRoom.ClearSelection();
                prev.Selected = true;
            }
        }

        // =========================================================
        // ✅ إرجاع حرف الشعبة المختارة (لرأس التصدير)
        // =========================================================
        private string GetSelectedClassRoomName()
        {
            if (ddlClassRoom == null) return "";
            if (string.IsNullOrWhiteSpace(ddlClassRoom.SelectedValue)) return "";

            int roomId;
            if (!int.TryParse(ddlClassRoom.SelectedValue, out roomId) || roomId <= 0)
                return "";

            try
            {
                var room = db.ClassRooms
                    .AsNoTracking()
                    .Where(c => c.Id == roomId)
                    .Select(c => new { c.Name, c.DivisionName })
                    .FirstOrDefault();

                if (room == null) return "";

                string letter = !string.IsNullOrWhiteSpace(room.DivisionName)
                                ? room.DivisionName.Trim()
                                : ExtractDivisionLetter(room.Name);

                return string.IsNullOrWhiteSpace(letter) ? "" : letter;
            }
            catch { return ""; }
        }

        // =========================================================
        // تنسيق القسم
        // =========================================================
        protected string FormatSection(object section)
        {
            if (section == null || section == DBNull.Value) return "—";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value)) return "—";

            if (value == "علمي") return "القسم العلمي";
            if (value == "أدبي") return "القسم الأدبي";

            return value;
        }

        protected string FormatSectionBadge(object section)
        {
            if (section == null || section == DBNull.Value)
                return "<span class='section-badge empty'>—</span>";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class='section-badge empty'>—</span>";

            if (value == "علمي")
                return "<span class='section-badge scientific'> القسم العلمي</span>";

            if (value == "أدبي")
                return "<span class='section-badge literary'> القسم الأدبي</span>";

            return "<span class='section-badge'>" + System.Web.HttpUtility.HtmlEncode(value) + "</span>";
        }

        // =========================================================
        // ✅ تنسيق الشعبة (Badge)
        // =========================================================
        protected string FormatDivisionBadge(object division)
        {
            if (division == null || division == DBNull.Value)
                return "<span class='text-muted'>—</span>";

            string value = Convert.ToString(division).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class='text-muted'>—</span>";

            return "<span class='division-badge'><i class='fas fa-chair'></i> شعبة "
                + System.Web.HttpUtility.HtmlEncode(value) + "</span>";
        }

        private string GetSelectedGradeLevelName()
        {
            if (ddlGradeLevel == null) return "";
            if (string.IsNullOrWhiteSpace(ddlGradeLevel.SelectedValue)) return "";

            int gradeLevelId;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                return "";

            try
            {
                return db.GradeLevels
                    .AsNoTracking()
                    .Where(g => g.Id == gradeLevelId)
                    .Select(g => g.Name)
                    .FirstOrDefault() ?? "";
            }
            catch { return ""; }
        }

        private string GetSelectedSectionDisplay()
        {
            string section = GetSelectedSection();
            if (string.IsNullOrWhiteSpace(section)) return "";

            if (section == "علمي") return "القسم العلمي";
            if (section == "أدبي") return "القسم الأدبي";

            return section;
        }

        // =========================================================
        // تحميل السنة الدراسية
        // =========================================================
        private void LoadAcademicYears()
        {
            var years = db.AcademicYears.OrderByDescending(y => y.Name).ToList();

            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("الكل", ""));

            foreach (var y in years)
                ddlAcademicYear.Items.Add(new ListItem(y.Name, y.Id.ToString()));

            var current = years.FirstOrDefault(y => y.IsCurrent);
            if (current != null)
                ddlAcademicYear.SelectedValue = current.Id.ToString();
        }

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
                ddlGradeLevel.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));
        }

        private IQueryable<Student> BuildStudentsQuery()
        {
            EducationStage stage = CurrentStage;

            IQueryable<Student> query = db.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.ClassRoom.GradeLevel)
                .Where(s => s.Stage == stage);

            int academicYearId;
            if (int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId) && academicYearId > 0)
                query = query.Where(s => s.ClassRoom != null
                                      && s.ClassRoom.AcademicYearId == academicYearId);

            int gradeLevelId;
            if (int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) && gradeLevelId > 0)
                query = query.Where(s => s.ClassRoom != null
                                      && s.ClassRoom.GradeLevelId == gradeLevelId);

            string section = GetSelectedSection();
            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(s => s.Section == section ||
                                          (s.Section == null && s.ClassRoom.SectionName == section));

            int classRoomId;
            if (int.TryParse(ddlClassRoom.SelectedValue, out classRoomId) && classRoomId > 0)
                query = query.Where(s => s.ClassRoomId == classRoomId);

            string parentLink = ddlParentLink.SelectedValue;
            if (parentLink == "none")
                query = query.Where(s => s.ParentId == null);
            else if (parentLink == "linked")
                query = query.Where(s => s.ParentId != null);

            return query;
        }

        private void LoadStudents()
        {
            var query = BuildStudentsQuery();

            // ✅ اجلب DivisionName + SectionName من ClassRoom
            var studentsInDb = query
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    s.RegistrationNo,
                    s.Name,
                    ClassName = (s.ClassRoom != null && s.ClassRoom.GradeLevel != null)
                                    ? s.ClassRoom.GradeLevel.Name : "",
                    s.Section,
                    ClassRoomName = s.ClassRoom != null ? s.ClassRoom.Name : null,
                    ClassRoomDivision = s.ClassRoom != null ? s.ClassRoom.DivisionName : null,
                    ClassRoomSection = s.ClassRoom != null ? s.ClassRoom.SectionName : null,
                    Gender = s.Gender,
                    s.Nationality,
                    s.NationalId,
                    s.IsActive,
                    AcademicYearId = s.ClassRoom != null ? s.ClassRoom.AcademicYearId : 0
                })
                .ToList();

            var yearIds = studentsInDb.Select(s => s.AcademicYearId).Distinct().ToList();
            var yearNames = db.AcademicYears
                .Where(y => yearIds.Contains(y.Id))
                .ToDictionary(y => y.Id, y => y.Name);

            var students = studentsInDb
                .Select(s => new
                {
                    s.RegistrationNo,
                    s.Name,
                    s.ClassName,

                    // ✅ القسم: Student.Section، أو ClassRoom.SectionName كحل احتياطي
                    Section = !string.IsNullOrWhiteSpace(s.Section)
                                ? s.Section
                                : s.ClassRoomSection,

                    // ✅ الشعبة: DivisionName أولاً، ثم Name كحل احتياطي
                    DivisionName = !string.IsNullOrWhiteSpace(s.ClassRoomDivision)
                                    ? s.ClassRoomDivision.Trim()
                                    : ExtractDivisionLetter(s.ClassRoomName),

                    GenderName = EnumHelper.GetDisplayName(s.Gender),
                    s.Nationality,
                    s.NationalId,
                    AcademicYearName = s.AcademicYearId > 0 && yearNames.ContainsKey(s.AcademicYearId)
                        ? yearNames[s.AcademicYearId] : "",
                    StatusName = s.IsActive ? "نشط" : "غير نشط"
                }).ToList();

            gvStudents.DataSource = students;
            gvStudents.DataBind();

            lblCount.Text = students.Count.ToString();
        }

        // =========================================================
        // أحداث
        // =========================================================
        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlGradeLevel.Items.Count > 0)
                ddlGradeLevel.SelectedIndex = 0;

            ResetSectionFilter();
            LoadClassRooms();
            LoadStudents();
        }

        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            LoadClassRooms();
            LoadStudents();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadClassRooms();
            LoadStudents();
        }

        protected void ddlClassRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudents();
        }

        protected void ddlParentLink_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudents();
        }

        protected void gvStudents_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudents.PageIndex = e.NewPageIndex;
            LoadStudents();
        }

        // =========================================================
        // نموذج صف التصدير
        // =========================================================
        private class WordStudentRow
        {
            public string RegistrationNo { get; set; }
            public string Name { get; set; }
            public string ClassName { get; set; }
            public string SectionText { get; set; }
            public string DivisionText { get; set; }
            public string GenderName { get; set; }
            public string Nationality { get; set; }
            public string NationalId { get; set; }
            public string AcademicYearName { get; set; }
            public string StatusName { get; set; }
        }

        // =========================================================
        // بناء قائمة الصفوف
        // =========================================================
        private List<WordStudentRow> BuildExportRows()
        {
            var query = BuildStudentsQuery();

            var studentsInDb = query
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    s.RegistrationNo,
                    s.Name,
                    ClassName = (s.ClassRoom != null && s.ClassRoom.GradeLevel != null)
                                    ? s.ClassRoom.GradeLevel.Name : "",
                    s.Section,
                    ClassRoomName = s.ClassRoom != null ? s.ClassRoom.Name : null,
                    ClassRoomDivision = s.ClassRoom != null ? s.ClassRoom.DivisionName : null,
                    ClassRoomSection = s.ClassRoom != null ? s.ClassRoom.SectionName : null,
                    Gender = s.Gender,
                    s.Nationality,
                    s.NationalId,
                    s.IsActive,
                    AcademicYearId = s.ClassRoom != null ? s.ClassRoom.AcademicYearId : 0
                })
                .ToList();

            var yearIds = studentsInDb.Select(s => s.AcademicYearId).Distinct().ToList();
            var yearNames = db.AcademicYears
                .Where(y => yearIds.Contains(y.Id))
                .ToDictionary(y => y.Id, y => y.Name);

            return studentsInDb
                .Select(s =>
                {
                    string section = !string.IsNullOrWhiteSpace(s.Section)
                                        ? s.Section
                                        : s.ClassRoomSection;

                    string letter = !string.IsNullOrWhiteSpace(s.ClassRoomDivision)
                                    ? s.ClassRoomDivision.Trim()
                                    : ExtractDivisionLetter(s.ClassRoomName);

                    return new WordStudentRow
                    {
                        RegistrationNo = s.RegistrationNo,
                        Name = s.Name,
                        ClassName = s.ClassName,
                        SectionText = FormatSection(section),
                        DivisionText = string.IsNullOrWhiteSpace(letter)
                            ? "—"
                            : "شعبة " + letter,
                        GenderName = EnumHelper.GetDisplayName(s.Gender),
                        Nationality = s.Nationality,
                        NationalId = s.NationalId,
                        AcademicYearName = s.AcademicYearId > 0 && yearNames.ContainsKey(s.AcademicYearId)
                            ? yearNames[s.AcademicYearId] : "",
                        StatusName = s.IsActive ? "نشط" : "غير نشط"
                    };
                })
                .ToList();
        }

        // =========================================================
        // تصدير ورد
        // =========================================================
        protected void btnExportWord_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyViewExport();
            var rows = BuildExportRows();

            byte[] docxBytes = BuildDocxStudentList(
                rows,
                GetStageDisplayName(),
                GetSelectedGradeLevelName(),
                GetSelectedSectionDisplay(),
                GetSelectedClassRoomName());

            string fileName = "StudentList_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".docx";

            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType =
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.AddHeader("Content-Length", docxBytes.Length.ToString());
            Response.BinaryWrite(docxBytes);
            Response.Flush();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest();
        }

        // =========================================================
        // تصدير Excel
        // =========================================================
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyViewExport();
            var rows = BuildExportRows();

            byte[] xlsxBytes = BuildXlsxStudentList(
                rows,
                GetStageDisplayName(),
                GetSelectedGradeLevelName(),
                GetSelectedSectionDisplay(),
                GetSelectedClassRoomName());

            string fileName = "StudentList_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";

            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType =
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.AddHeader("Content-Length", xlsxBytes.Length.ToString());
            Response.BinaryWrite(xlsxBytes);
            Response.Flush();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest();
        }

        // =========================================================
        // بناء ملف DOCX
        // =========================================================
        private byte[] BuildDocxStudentList(
            List<WordStudentRow> rows,
            string stageDisplay,
            string gradeLevelName,
            string sectionDisplay,
            string classRoomName)
        {
            using (var ms = new MemoryStream())
            {
                using (var doc = WordprocessingDocument.Create(
                    ms, WordprocessingDocumentType.Document))
                {
                    var mainPart = doc.AddMainDocumentPart();

                    AddDefaultFontStyle(mainPart);

                    mainPart.Document = new W.Document();
                    var body = mainPart.Document.AppendChild(new W.Body());

                    var sectionProps = new W.SectionProperties(
                        new W.PageSize
                        {
                            Width = 16838,
                            Height = 11906,
                            Orient = W.PageOrientationValues.Landscape
                        },
                        new W.PageMargin
                        {
                            Top = 850,
                            Right = 850,
                            Bottom = 850,
                            Left = 850,
                            Header = 720,
                            Footer = 720,
                            Gutter = 0
                        }
                    );

                    body.AppendChild(CreateParagraph(
                        "مدرسة نور الملائكة",
                        fontSize: 32, bold: true, color: "00B14F",
                        align: WJustify.Center));

                    body.AppendChild(CreateParagraph(
                        "عدد الطلاب : " + rows.Count.ToString(),
                        fontSize: 20, bold: true, color: "495057",
                        align: WJustify.Center));

                    body.AppendChild(CreateParagraph(
                        "قائمة طلاب المرحلة : " + stageDisplay,
                        fontSize: 20, bold: true, color: "212529",
                        align: WJustify.Center));

                    string gradeText = string.IsNullOrWhiteSpace(gradeLevelName)
                        ? "الكل" : gradeLevelName;

                    if (!string.IsNullOrWhiteSpace(sectionDisplay))
                        gradeText += " | " + sectionDisplay;

                    if (!string.IsNullOrWhiteSpace(classRoomName))
                        gradeText += " | شعبة " + classRoomName;

                    body.AppendChild(CreateParagraph(
                        "الصف : " + gradeText,
                        fontSize: 20, bold: true, color: "212529",
                        align: WJustify.Center));

                    body.AppendChild(new W.Paragraph());

                    var table = CreateTable(new int[]
                    {
                        2200, 1200, 1400, 1300, 1200, 1100, 800, 1000, 1200, 1000
                    });

                    table.AppendChild(CreateRow(new[]
                    {
                        "اسم الطالب", "رقم التسجيل", "الرقم الوطني", "الصف", "القسم",
                        "الشعبة", "الجنس", "الجنسية", "السنة الدراسية", "الحالة"
                    }, isHeader: true));

                    int idx = 1;
                    foreach (var r in rows)
                    {
                        bool even = idx % 2 == 0;
                        table.AppendChild(CreateRow(new[]
                        {
                            r.Name ?? "",
                            r.RegistrationNo ?? "",
                            r.NationalId ?? "",
                            r.ClassName ?? "",
                            r.SectionText ?? "",
                            r.DivisionText ?? "",
                            r.GenderName ?? "",
                            r.Nationality ?? "",
                            r.AcademicYearName ?? "",
                            r.StatusName ?? ""
                        }, isHeader: false, isEven: even));

                        idx++;
                    }

                    body.AppendChild(table);
                    body.AppendChild(new W.Paragraph());

                    body.AppendChild(CreateParagraph(
                        "تم إنشاء التقرير تلقائياً من نظام مدرسة نور الملائكة",
                        fontSize: 16, bold: false, color: "868E96",
                        align: WJustify.Center));

                    body.AppendChild(sectionProps);

                    mainPart.Document.Save();
                }

                return ms.ToArray();
            }
        }

        // =========================================================
        // بناء ملف XLSX
        // =========================================================
        private byte[] BuildXlsxStudentList(
            List<WordStudentRow> rows,
            string stageDisplay,
            string gradeLevelName,
            string sectionDisplay,
            string classRoomName)
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

                    sheetData.AppendChild(CreateXlsxRow(1U, 32D,
                        new[] { "مدرسة نور الملائكة" }, 1U, true));

                    sheetData.AppendChild(CreateXlsxRow(2U, 22D,
                        new[] { "عدد الطلاب : " + rows.Count.ToString() }, 2U, true));

                    sheetData.AppendChild(CreateXlsxRow(3U, 22D,
                        new[] { "قائمة طلاب المرحلة : " + stageDisplay }, 2U, true));

                    string gradeText = string.IsNullOrWhiteSpace(gradeLevelName)
                        ? "الكل" : gradeLevelName;

                    if (!string.IsNullOrWhiteSpace(sectionDisplay))
                        gradeText += " | " + sectionDisplay;

                    if (!string.IsNullOrWhiteSpace(classRoomName))
                        gradeText += " | شعبة " + classRoomName;

                    sheetData.AppendChild(CreateXlsxRow(4U, 22D,
                        new[] { "الصف : " + gradeText }, 2U, true));

                    sheetData.AppendChild(new S.Row { RowIndex = 5U });

                    string[] headers = {
                        "اسم الطالب", "رقم التسجيل", "الرقم الوطني", "الصف", "القسم",
                        "الشعبة", "الجنس", "الجنسية", "السنة الدراسية", "الحالة"
                    };
                    sheetData.AppendChild(CreateXlsxRow(6U, 28D, headers, 3U, false));

                    uint currentRow = 7U;
                    int idx = 1;
                    foreach (var r in rows)
                    {
                        bool even = idx % 2 == 0;
                        uint style = even ? 5U : 4U;

                        string[] values = {
                            r.Name ?? "",
                            r.RegistrationNo ?? "",
                            r.NationalId ?? "",
                            r.ClassName ?? "",
                            r.SectionText ?? "",
                            r.DivisionText ?? "",
                            r.GenderName ?? "",
                            r.Nationality ?? "",
                            r.AcademicYearName ?? "",
                            r.StatusName ?? ""
                        };

                        sheetData.AppendChild(CreateXlsxRow(currentRow, 22D, values, style, false));
                        currentRow++;
                        idx++;
                    }

                    var columns = new S.Columns();
                    double[] widths = { 30, 15, 18, 15, 15, 12, 10, 12, 15, 12 };
                    for (int i = 0; i < widths.Length; i++)
                    {
                        columns.AppendChild(new S.Column
                        {
                            Min = (uint)(i + 1),
                            Max = (uint)(i + 1),
                            Width = widths[i],
                            CustomWidth = true
                        });
                    }

                    var mergeCells = new S.MergeCells();
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A1:J1" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A2:J2" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A3:J3" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A4:J4" });
                    mergeCells.Count = 4U;

                    var sheetView = new S.SheetView
                    {
                        RightToLeft = true,
                        WorkbookViewId = 0U,
                        ShowGridLines = false
                    };
                    sheetView.AppendChild(new S.Pane
                    {
                        VerticalSplit = 6D,
                        TopLeftCell = "A7",
                        ActivePane = S.PaneValues.BottomLeft,
                        State = S.PaneStateValues.Frozen
                    });
                    sheetView.AppendChild(new S.Selection
                    {
                        Pane = S.PaneValues.BottomLeft
                    });

                    var sheetViews = new S.SheetViews();
                    sheetViews.AppendChild(sheetView);

                    worksheetPart.Worksheet = new S.Worksheet(
                        sheetViews,
                        columns,
                        sheetData,
                        mergeCells
                    );

                    var sheets = workbookPart.Workbook.AppendChild(new S.Sheets());
                    sheets.AppendChild(new S.Sheet
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1U,
                        Name = "قائمة الطلاب"
                    });

                    workbookPart.Workbook.Save();
                }

                return ms.ToArray();
            }
        }

        // =========================================================
        // إنشاء صف Excel
        // =========================================================
        private S.Row CreateXlsxRow(
            uint rowIndex,
            double height,
            string[] values,
            uint styleIndex,
            bool mergedTitleOnly)
        {
            var row = new S.Row
            {
                RowIndex = rowIndex,
                Height = height,
                CustomHeight = true
            };

            string[] columnLetters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

            if (mergedTitleOnly)
            {
                row.AppendChild(CreateInlineTextCell("A" + rowIndex, values[0], styleIndex));
            }
            else
            {
                for (int i = 0; i < values.Length && i < columnLetters.Length; i++)
                {
                    row.AppendChild(CreateInlineTextCell(
                        columnLetters[i] + rowIndex, values[i], styleIndex));
                }
            }

            return row;
        }

        private S.Cell CreateInlineTextCell(string reference, string text, uint styleIndex)
        {
            return new S.Cell
            {
                CellReference = reference,
                DataType = S.CellValues.InlineString,
                StyleIndex = styleIndex,
                InlineString = new S.InlineString(
                    new S.Text(text ?? string.Empty)
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    }
                )
            };
        }

        // =========================================================
        // Stylesheet Excel
        // =========================================================
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
                new S.DiagonalBorder()
            ));
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
        // ضبط الخط الافتراضي (Word)
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

        private W.Paragraph CreateParagraph(
            string text, int fontSize, bool bold, string color, WJustify align)
        {
            var runProps = new W.RunProperties(
                CreateRunFonts(),
                new W.FontSize { Val = fontSize.ToString() },
                new W.FontSizeComplexScript { Val = fontSize.ToString() }
            );

            if (bold)
            {
                runProps.AppendChild(new W.Bold());
                runProps.AppendChild(new W.BoldComplexScript());
            }

            if (!string.IsNullOrEmpty(color))
                runProps.AppendChild(new W.Color { Val = color });

            runProps.AppendChild(new W.RightToLeftText());
            runProps.AppendChild(new W.ComplexScript());

            var run = new W.Run(
                runProps,
                new W.Text(text ?? string.Empty)
                {
                    Space = SpaceProcessingModeValues.Preserve
                });

            var paraProps = new W.ParagraphProperties(
                new W.Justification { Val = align },
                new W.BiDi());

            return new W.Paragraph(paraProps, run);
        }

        private W.Table CreateTable(int[] columnWidths)
        {
            var table = new W.Table();

            var borders = new W.TableBorders(
                new W.TopBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.BottomBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.LeftBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.RightBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.InsideHorizontalBorder { Val = WBorder.Single, Size = 6, Color = "999999" },
                new W.InsideVerticalBorder { Val = WBorder.Single, Size = 6, Color = "999999" });

            var tableProps = new W.TableProperties(
                new W.TableWidth { Width = "5000", Type = W.TableWidthUnitValues.Pct },
                borders,
                new W.TableJustification { Val = W.TableRowAlignmentValues.Center },
                new W.BiDiVisual());

            table.AppendChild(tableProps);

            var grid = new W.TableGrid();
            foreach (var w in columnWidths)
                grid.AppendChild(new W.GridColumn { Width = w.ToString() });

            table.AppendChild(grid);

            return table;
        }

        private W.TableRow CreateRow(
            string[] values, bool isHeader, bool isEven = false, bool isBold = false)
        {
            var row = new W.TableRow();

            foreach (var val in values)
            {
                var cellProps = new W.TableCellProperties(
                    new W.TableCellVerticalAlignment
                    {
                        Val = W.TableVerticalAlignmentValues.Center
                    },
                    new W.TableCellMargin
                    {
                        TopMargin = new W.TopMargin { Width = "40", Type = W.TableWidthUnitValues.Dxa },
                        BottomMargin = new W.BottomMargin { Width = "40", Type = W.TableWidthUnitValues.Dxa },
                        LeftMargin = new W.LeftMargin { Width = "80", Type = W.TableWidthUnitValues.Dxa },
                        RightMargin = new W.RightMargin { Width = "80", Type = W.TableWidthUnitValues.Dxa }
                    });

                if (isHeader)
                {
                    cellProps.AppendChild(new W.Shading
                    {
                        Val = W.ShadingPatternValues.Clear,
                        Color = "auto",
                        Fill = "00B14F"
                    });
                }
                else if (isEven)
                {
                    cellProps.AppendChild(new W.Shading
                    {
                        Val = W.ShadingPatternValues.Clear,
                        Color = "auto",
                        Fill = "F9FAFB"
                    });
                }

                var runProps = new W.RunProperties(
                    CreateRunFonts(),
                    new W.FontSize { Val = "16" },
                    new W.FontSizeComplexScript { Val = "16" }
                );

                if (isHeader)
                {
                    runProps.AppendChild(new W.Bold());
                    runProps.AppendChild(new W.BoldComplexScript());
                    runProps.AppendChild(new W.Color { Val = "FFFFFF" });
                }
                else if (isBold)
                {
                    runProps.AppendChild(new W.Bold());
                    runProps.AppendChild(new W.BoldComplexScript());
                }

                runProps.AppendChild(new W.RightToLeftText());
                runProps.AppendChild(new W.ComplexScript());

                var paraProps = new W.ParagraphProperties(
                    new W.Justification { Val = WJustify.Center },
                    new W.BiDi());

                var para = new W.Paragraph(
                    paraProps,
                    new W.Run(runProps,
                        new W.Text(val ?? string.Empty)
                        {
                            Space = SpaceProcessingModeValues.Preserve
                        }));

                var cell = new W.TableCell(cellProps, para);
                row.AppendChild(cell);
            }

            return row;
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}