using System;
using System.Globalization;
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

// ✅ alias لتفادي التعارض مع WebControls
using W = DocumentFormat.OpenXml.Wordprocessing;
using WJustify = DocumentFormat.OpenXml.Wordprocessing.JustificationValues;
using WBorder = DocumentFormat.OpenXml.Wordprocessing.BorderValues;

namespace NoorAlMalaikaSchool.Pages.Reports
{
    public partial class AttendanceReport : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private const string ReportFont = "Droid Arabic Kufi";

           protected void Page_Load(object sender, EventArgs e)
           {
               // Local guard, evaluated before the try/catch so a denied request never
               // reaches the report body and cannot be reported as a handled error.
               SecurityHelper.DenyUnlessPath();
               SecurityHelper.HideViewExportControls(this);

               try
            {
                if (!IsPostBack)
                {
                    LoadGradeLevels();
                    ResetSectionFilter();
                    LoadClassRooms();

                    if (string.IsNullOrWhiteSpace(txtStartDate.Text))
                        txtStartDate.Text = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");

                    if (string.IsNullOrWhiteSpace(txtEndDate.Text))
                        txtEndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                HandleError("حدث خطأ أثناء تحميل صفحة تقرير الحضور.", ex);
            }
        }

        // =========================================================
        // ✅ الصف: تحميل GradeLevels حسب المرحلة
        // =========================================================
        private void LoadGradeLevels()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();
            ddlGradeLevel.Items.Add(new ListItem("الكل", ""));

            foreach (var g in gradeLevels)
                ddlGradeLevel.Items.Add(new ListItem(g.Name, g.Id.ToString()));
        }

        // =========================================================
        // ✅ هل الصف ثانوي (ثاني/ثالث)؟
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

        private void ConfigureSectionFilter()
        {
            int gradeLevelId;
            int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId);

            bool show = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = show;

            if (!show) SelectSection("");
        }

        private void ResetSectionFilter()
        {
            SelectSection("");
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
        // ✅ استخراج حرف الشعبة من اسم القاعة (حل احتياطي)
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

        private string ExtractSectionFromName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            if (classRoomName.Contains("علمي")) return "علمي";
            if (classRoomName.Contains("أدبي")) return "أدبي";

            return null;
        }

        // =========================================================
        // ✅ تحميل الشعب: "شعبة أ" / "شعبة ب"
        // =========================================================
        private void LoadClassRooms()
        {
            try
            {
                EducationStage stage = EnumHelper.GetCurrentStage();

                int gradeLevelId;
                int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId);

                string selectedSection = GetSelectedSection();
                string previousValue = ddlClassRoom.SelectedValue;

                IQueryable<ClassRoom> query = db.ClassRooms.AsNoTracking();

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

                var rooms = query
                    .OrderBy(c => c.GradeLevelId)
                    .ThenBy(c => c.DivisionName)
                    .ThenBy(c => c.Name)
                    .Select(c => new { c.Id, c.Name, c.DivisionName, c.SectionName })
                    .ToList();

                // ✅ استخرج الحرف + القسم (DivisionName له الأولوية)
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

                // ✅ توليد حرف تسلسلي لمن ليس له حرف
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
                HandleError("تعذر تحميل قائمة الشعب.", ex);
            }
        }

        // =========================================================
        // ✅ RowDataBound — تعيين عرض ولون شريط النسبة
        // =========================================================
        protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            try
            {
                decimal pct = 0m;
                var item = e.Row.DataItem;
                if (item != null)
                {
                    var prop = item.GetType().GetProperty("Percentage");
                    if (prop != null)
                    {
                        try { pct = Convert.ToDecimal(prop.GetValue(item)); }
                        catch { pct = 0m; }
                    }
                }

                var barFill = e.Row.FindControl("barFill")
                              as System.Web.UI.HtmlControls.HtmlGenericControl;
                if (barFill == null) return;

                string cssClass = "percentage-bar-fill";
                if (pct >= 85m) cssClass += " high";
                else if (pct >= 70m) cssClass += " medium";
                else cssClass += " low";

                barFill.Attributes["class"] = cssClass;
                barFill.Attributes["style"] = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "width:{0:F1}%;", pct);
            }
            catch { }
        }

        // =========================================================
        // ✅ أحداث الفلاتر
        // =========================================================
        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            LoadClassRooms();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadClassRooms();
        }

        // =========================================================
        // عرض التقرير
        // =========================================================
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                var result = BuildReport();
                if (result == null) return;

                var rows = result.Value.Rows;

                gvReport.DataSource = rows;
                gvReport.DataBind();

                lblCountPill.Text = rows.Count.ToString();

                if (rows.Count > 0)
                {
                    int totalPresent = rows.Sum(r => r.TotalPresent);
                    int totalAbsent = rows.Sum(r => r.TotalAbsent);
                    int totalDays = totalPresent + totalAbsent;

                    decimal overall = totalDays > 0
                        ? Math.Round(((decimal)totalPresent / totalDays) * 100m, 1)
                        : 0m;

                    lblTotalStudents.Text = rows.Count.ToString();
                    lblTotalPresent.Text = totalPresent.ToString();
                    lblTotalAbsent.Text = totalAbsent.ToString();
                    lblOverallPercentage.Text = overall.ToString("N1") + "%";

                    pnlSummary.Visible = true;
                    pnlEmpty.Visible = false;
                    pnlReport.Visible = true;
                }
                else
                {
                    pnlSummary.Visible = false;
                    pnlEmpty.Visible = true;
                    pnlReport.Visible = false;
                }

                ShowSuccess("تم عرض التقرير بنجاح. عدد الطلاب: " + rows.Count);
            }
            catch (Exception ex)
            {
                HandleError("تعذر إنشاء تقرير الحضور.", ex);
            }
        }

        // =========================================================
        // بناء التقرير
        // =========================================================
        private ReportResult? BuildReport()
        {
            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParse(txtStartDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                startDate = DateTime.Today.AddMonths(-1);

            if (!DateTime.TryParse(txtEndDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                endDate = DateTime.Today;

            startDate = startDate.Date;
            endDate = endDate.Date;

            if (startDate > endDate)
            {
                ShowError("تاريخ البداية لا يمكن أن يكون بعد تاريخ النهاية.");
                return null;
            }

            EducationStage currentStage = EnumHelper.GetCurrentStage();

            // ✅ فلتر الصف (GradeLevel)
            int gradeLevelId;
            bool hasGradeFilter =
                int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId)
                && gradeLevelId > 0;

            // ✅ فلتر القسم
            string selectedSection = GetSelectedSection();

            // ✅ فلتر الشعبة
            int selectedClassId;
            bool hasClassFilter =
                int.TryParse(ddlClassRoom.SelectedValue, out selectedClassId)
                && selectedClassId > 0;

            IQueryable<Student> query = db.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.ClassRoom.GradeLevel)   // ✅ لتحميل اسم الصف
                .Where(s => s.IsActive && s.Stage == currentStage);

            if (hasGradeFilter)
            {
                int gid = gradeLevelId;
                query = query.Where(s => s.ClassRoom != null && s.ClassRoom.GradeLevelId == gid);
            }

            if (!string.IsNullOrWhiteSpace(selectedSection))
            {
                string sec = selectedSection;
                query = query.Where(s => s.Section == sec ||
                                          (s.Section == null && s.ClassRoom.SectionName == sec));
            }

            if (hasClassFilter)
            {
                int cid = selectedClassId;
                query = query.Where(s => s.ClassRoomId == cid);
            }

            var students = query
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ThenBy(s => s.Id)
                .ToList();

            if (students.Count == 0)
            {
                ShowInfo("لا يوجد طلاب مطابقون للمعايير المحددة.");
                return new ReportResult
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Rows = new List<AttendanceReportRow>()
                };
            }

            List<int> studentIds = students.Select(s => s.Id).Distinct().ToList();
            DateTime endExclusive = endDate.AddDays(1);

            var attendanceRows = db.Attendances
                .AsNoTracking()
                .Where(a => a.Date >= startDate && a.Date < endExclusive)
                .Where(a => studentIds.Contains(a.StudentId))
                .Select(a => new { StudentId = a.StudentId, IsPresent = a.IsPresent })
                .ToList();

            var attendanceByStudent = attendanceRows
                .GroupBy(a => a.StudentId)
                .ToDictionary(
                    g => g.Key,
                    g => new AttendanceSummary
                    {
                        Present = g.Count(x => x.IsPresent),
                        Absent = g.Count(x => !x.IsPresent)
                    });

            var reportData = new List<AttendanceReportRow>();

            foreach (var student in students)
            {
                int present = 0;
                int absent = 0;

                AttendanceSummary summary;
                if (attendanceByStudent.TryGetValue(student.Id, out summary))
                {
                    present = summary.Present;
                    absent = summary.Absent;
                }

                int total = present + absent;
                decimal percentage = total > 0
                    ? Math.Round(((decimal)present / total) * 100m, 2)
                    : 0m;

                // ✅ اسم الصف
                string className = student.ClassRoom != null
                    ? (student.ClassRoom.GradeLevel != null
                        ? student.ClassRoom.GradeLevel.Name
                        : student.ClassRoom.Name)
                    : "";

                // ✅ القسم: Student.Section أو ClassRoom.SectionName
                string sectionValue = !string.IsNullOrWhiteSpace(student.Section)
                    ? student.Section
                    : (student.ClassRoom != null ? student.ClassRoom.SectionName : null);

                string sectionText = string.IsNullOrWhiteSpace(sectionValue)
                    ? "—"
                    : (sectionValue == "علمي" ? "القسم العلمي" :
                       sectionValue == "أدبي" ? "القسم الأدبي" : sectionValue);

                // ✅ الشعبة: DivisionName أولاً، ثم استخراج الحرف من Name
                string divisionLetter = null;
                if (student.ClassRoom != null)
                {
                    divisionLetter = !string.IsNullOrWhiteSpace(student.ClassRoom.DivisionName)
                        ? student.ClassRoom.DivisionName.Trim()
                        : ExtractDivisionLetter(student.ClassRoom.Name);
                }

                string divisionText = string.IsNullOrWhiteSpace(divisionLetter)
                    ? "—"
                    : "شعبة " + divisionLetter;

                reportData.Add(new AttendanceReportRow
                {
                    StudentName = student.Name,
                    ClassName = className,
                    SectionText = sectionText,
                    DivisionText = divisionText,
                    TotalPresent = present,
                    TotalAbsent = absent,
                    Percentage = percentage
                });
            }

            return new ReportResult
            {
                StartDate = startDate,
                EndDate = endDate,
                Rows = reportData
            };
        }

        // =========================================================
        // تصدير ورد
        // =========================================================
        protected void btnExportWord_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyViewExport();
            try
            {
                var result = BuildReport();
                if (result == null) return;

                string filterInfo = BuildFilterInfo();
                string stageDisplay = EnumHelper.GetStageArabic(
                    EnumHelper.GetCurrentStage().ToString());

                byte[] docxBytes = BuildDocxReport(
                    result.Value.StartDate,
                    result.Value.EndDate,
                    result.Value.Rows,
                    filterInfo,
                    stageDisplay);

                string fileName = "AttendanceReport_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".docx";

                Response.Clear();
                Response.ClearHeaders();
                Response.ContentType =
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("Content-Disposition",
                    "attachment; filename=\"" + fileName + "\"");
                Response.AddHeader("Content-Length", docxBytes.Length.ToString());
                Response.BinaryWrite(docxBytes);
                Response.Flush();
                Response.SuppressContent = true;
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                HandleError("تعذر تصدير التقرير بصيغة ورد.", ex);
            }
        }

        private string BuildFilterInfo()
        {
            var parts = new List<string>();

            string gradeName = ddlGradeLevel.SelectedIndex > 0
                ? ddlGradeLevel.SelectedItem.Text : "";
            if (!string.IsNullOrWhiteSpace(gradeName) && gradeName != "الكل")
                parts.Add("الصف: " + gradeName);

            string sectionName = ddlSection.SelectedIndex > 0
                ? ddlSection.SelectedItem.Text : "";
            if (pnlSectionFilter.Visible
                && !string.IsNullOrWhiteSpace(sectionName)
                && sectionName != "جميع الأقسام")
            {
                parts.Add("القسم: " + sectionName);
            }

            string roomName = ddlClassRoom.SelectedIndex > 0
                ? ddlClassRoom.SelectedItem.Text : "";
            if (!string.IsNullOrWhiteSpace(roomName) && roomName != "جميع الشعب")
                parts.Add("الشعبة: " + roomName);

            return parts.Count > 0 ? string.Join(" | ", parts) : "الكل";
        }

        // =========================================================
        // بناء ملف DOCX
        // =========================================================
        private byte[] BuildDocxReport(
            DateTime startDate,
            DateTime endDate,
            List<AttendanceReportRow> rows,
            string filterInfo,
            string stageDisplay)
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
                        "تقرير الحضور والغياب — " + stageDisplay,
                        fontSize: 24, bold: true, color: "212529",
                        align: WJustify.Center));

                    body.AppendChild(CreateParagraph(
                        "الفلاتر : " + filterInfo,
                        fontSize: 18, bold: true, color: "495057",
                        align: WJustify.Center));

                    string dateLine = "من: " + startDate.ToString("yyyy-MM-dd")
                        + "   |   إلى: " + endDate.ToString("yyyy-MM-dd")
                        + "   |   تاريخ الطباعة: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                    body.AppendChild(CreateParagraph(
                        dateLine,
                        fontSize: 18, bold: false, color: "495057",
                        align: WJustify.Center));

                    body.AppendChild(new W.Paragraph());

                    if (rows.Count > 0)
                    {
                        int totalPresent = rows.Sum(r => r.TotalPresent);
                        int totalAbsent = rows.Sum(r => r.TotalAbsent);
                        int totalDays = totalPresent + totalAbsent;

                        decimal overall = totalDays > 0
                            ? Math.Round(((decimal)totalPresent / totalDays) * 100m, 1)
                            : 0m;

                        var summaryTable = CreateTable(new int[]
                        {
                            2500, 2500, 2500, 2500
                        });

                        summaryTable.AppendChild(CreateRow(new[]
                        {
                            "عدد الطلاب", "إجمالي الحضور", "إجمالي الغياب", "نسبة الحضور العامة"
                        }, isHeader: true));

                        summaryTable.AppendChild(CreateRow(new[]
                        {
                            rows.Count.ToString(),
                            totalPresent.ToString(),
                            totalAbsent.ToString(),
                            overall.ToString("N1") + "%"
                        }, isHeader: false, isBold: true));

                        body.AppendChild(summaryTable);
                        body.AppendChild(new W.Paragraph());
                    }

                    // ✅ أعمدة الجدول: 8 أعمدة (مع القسم والشعبة)
                    var detailsTable = CreateTable(new int[]
                    {
                        700, 2600, 2000, 1500, 1500, 1500, 1500, 1500
                    });

                    detailsTable.AppendChild(CreateRow(new[]
                    {
                        "م", "اسم الطالب", "الصف", "القسم", "الشعبة",
                        "أيام الحضور", "أيام الغياب", "نسبة الحضور"
                    }, isHeader: true));

                    int idx = 1;
                    foreach (var r in rows)
                    {
                        bool even = idx % 2 == 0;
                        detailsTable.AppendChild(CreateRow(new[]
                        {
                            idx.ToString(),
                            r.StudentName,
                            r.ClassName,
                            r.SectionText,
                            r.DivisionText,
                            r.TotalPresent.ToString(),
                            r.TotalAbsent.ToString(),
                            r.Percentage.ToString("N1") + "%"
                        }, isHeader: false, isEven: even));

                        idx++;
                    }

                    body.AppendChild(detailsTable);
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
        // الخط الافتراضي
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
                    new W.FontSize { Val = "18" },
                    new W.FontSizeComplexScript { Val = "18" }
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

        // =========================================================
        // نماذج مساعدة
        // =========================================================
        private sealed class AttendanceSummary
        {
            public int Present { get; set; }
            public int Absent { get; set; }
        }

        private sealed class AttendanceReportRow
        {
            public string StudentName { get; set; }
            public string ClassName { get; set; }
            public string SectionText { get; set; }   // ✅ القسم
            public string DivisionText { get; set; }  // ✅ الشعبة
            public int TotalPresent { get; set; }
            public int TotalAbsent { get; set; }
            public decimal Percentage { get; set; }
        }

        private struct ReportResult
        {
            public DateTime StartDate;
            public DateTime EndDate;
            public List<AttendanceReportRow> Rows;
        }

        // =========================================================
        // رسائل
        // =========================================================
        private void ShowSuccess(string message)
        {
            if (lblMessage == null) return;
            lblMessage.Text = "<i class='fas fa-check-circle'></i> " + Server.HtmlEncode(message);
            lblMessage.CssClass = "report-message text-success";
        }

        private void ShowInfo(string message)
        {
            if (lblMessage == null) return;
            lblMessage.Text = "<i class='fas fa-info-circle'></i> " + Server.HtmlEncode(message);
            lblMessage.CssClass = "report-message text-info";
        }

        private void ShowError(string message)
        {
            if (lblMessage != null)
            {
                lblMessage.Text = "<i class='fas fa-exclamation-triangle'></i> " + Server.HtmlEncode(message);
                lblMessage.CssClass = "report-message text-danger";
            }

            System.Diagnostics.Trace.TraceError("AttendanceReport: " + message);
        }

        private void HandleError(string title, Exception ex)
        {
            Exception deepest = ex;
            while (deepest.InnerException != null)
                deepest = deepest.InnerException;

            ShowError(title + " التفاصيل: " + deepest.Message);

            System.Diagnostics.Trace.TraceError(
                "AttendanceReport ERROR:" + Environment.NewLine + ex.ToString());
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}