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

// ✅ alias لتفادي التعارض مع WebControls
using W = DocumentFormat.OpenXml.Wordprocessing;
using WJustify = DocumentFormat.OpenXml.Wordprocessing.JustificationValues;
using WBorder = DocumentFormat.OpenXml.Wordprocessing.BorderValues;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Attendance
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        // ✅ الخط المعتمد للتقرير
        private const string ReportFont = "Droid Arabic Kufi";

        private EducationStage CurrentStage
        {
            get { return EnumHelper.GetCurrentStage(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadGradeLevels(CurrentStage);
                ResetSectionFilter();
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                LoadAttendances();
            }
        }

        // =========================================================
        // دوال مساعدة
        // =========================================================
        private int GetSelectedGradeLevelId()
        {
            int id;
            if (!int.TryParse(ddlClassRoom.SelectedValue, out id)) return 0;
            return id > 0 ? id : 0;
        }

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
            int gradeLevelId = GetSelectedGradeLevelId();
            bool showSection = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = showSection;

            if (!showSection)
                SelectSection("");
        }

        private void ResetSectionFilter()
        {
            SelectSection("");
            pnlSectionFilter.Visible = false;
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

        // =========================================================
        // تنسيق القسم — نسخة النص (تُستخدم في Word)
        // =========================================================
        protected string FormatSection(object section)
        {
            if (section == null || section == DBNull.Value)
                return "<span class=\"text-muted\">—</span>";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class=\"text-muted\">—</span>";

            string display;
            if (value == "علمي") display = "القسم العلمي";
            else if (value == "أدبي") display = "القسم الأدبي";
            else display = value;

            return "<span class=\"badge px-3 py-2\">"
                + System.Web.HttpUtility.HtmlEncode(display) + "</span>";
        }

        // =========================================================
        // ✅ تنسيق القسم كـ Badge ملوّن (للـ GridView)
        // =========================================================
        protected string FormatSectionBadge(object section)
        {
            if (section == null || section == DBNull.Value)
                return "<span class='text-muted'>—</span>";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class='text-muted'>—</span>";

            if (value == "علمي")
                return "<span class='section-badge sci'>القسم العلمي</span>";

            if (value == "أدبي")
                return "<span class='section-badge lit'>القسم الأدبي</span>";

            return "<span class='section-badge'>" + System.Web.HttpUtility.HtmlEncode(value) + "</span>";
        }

        // =========================================================
        // تنسيق الشعبة — نسخة النص (تُستخدم في Word)
        // =========================================================
        protected string FormatDivision(object division)
        {
            if (division == null || division == DBNull.Value)
                return "<span class=\"text-muted\">—</span>";

            string value = Convert.ToString(division).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class=\"text-muted\">—</span>";

            return "<span class=\"badge bg-light text-dark px-3 py-2\">"
                + "شعبة " + System.Web.HttpUtility.HtmlEncode(value) + "</span>";
        }

        // =========================================================
        // ✅ تنسيق الشعبة كـ Badge ملوّن (للـ GridView)
        // =========================================================
        protected string FormatDivisionBadge(object division)
        {
            if (division == null || division == DBNull.Value)
                return "<span class='text-muted'>—</span>";

            string value = Convert.ToString(division).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class='text-muted'>—</span>";

            return "<span class='division-badge'><i class='fas fa-chair' style='font-size:10px;'></i> شعبة "
                + System.Web.HttpUtility.HtmlEncode(value) + "</span>";
        }

        // =========================================================
        // تحميل الصفوف
        // =========================================================
        private void LoadGradeLevels(EducationStage stage)
        {
            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlClassRoom.Items.Clear();
            ddlClassRoom.Items.Add(new ListItem("الكل", ""));

            foreach (var g in gradeLevels)
                ddlClassRoom.Items.Add(new ListItem(g.Name, g.Id.ToString()));
        }

        // =========================================================
        // نموذج صف الحضور
        // =========================================================
        private class AttendanceRow
        {
            public string StudentName { get; set; }
            public string ClassName { get; set; }
            public string Section { get; set; }
            public string Division { get; set; }
            public DateTime Date { get; set; }
            public bool IsPresent { get; set; }
            public string Notes { get; set; }
            public int ClassRoomId { get; set; }
        }

        // =========================================================
        // بناء بيانات الحضور (مشترك بين العرض و Word)
        // =========================================================
        private List<AttendanceRow> BuildAttendanceData()
        {
            DateTime filterDate;
            if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out filterDate))
                filterDate = DateTime.Today;
            filterDate = filterDate.Date;

            DateTime dateFilter = filterDate;
            int gradeLevelId = GetSelectedGradeLevelId();
            int glId = gradeLevelId;
            string section = GetSelectedSection();
            string sec = section;
            string division = GetSelectedDivision();
            string div = division;
            EducationStage stage = CurrentStage;

            IQueryable<Models.Attendance> query = db.Attendances
                .Include("Student.ClassRoom.GradeLevel")
                .Where(a => a.Date == dateFilter)
                .Where(a => a.Student.Stage == stage);

            if (glId > 0)
            {
                query = query.Where(a => a.Student.ClassRoom != null
                                      && a.Student.ClassRoom.GradeLevelId == glId);
            }

            // ✅ الفلترة بالقسم عبر ClassRoom.SectionName
            if (!string.IsNullOrWhiteSpace(sec))
            {
                query = query.Where(a => a.Student.ClassRoom != null
                                      && a.Student.ClassRoom.SectionName == sec);
            }

            // ✅ الفلترة بالشعبة عبر ClassRoom.DivisionName
            if (!string.IsNullOrWhiteSpace(div))
            {
                query = query.Where(a => a.Student.ClassRoom != null
                                      && a.Student.ClassRoom.DivisionName == div);
            }

            return query
                .OrderBy(a => a.Student.Name)
                .ThenBy(a => a.StudentId)
                .ToList()
                .Select(a => new AttendanceRow
                {
                    StudentName = a.Student != null ? a.Student.Name : "",
                    ClassName = (a.Student != null
                                && a.Student.ClassRoom != null
                                && a.Student.ClassRoom.GradeLevel != null)
                                    ? a.Student.ClassRoom.GradeLevel.Name
                                    : "",
                    Section = a.Student != null && a.Student.ClassRoom != null
                              ? a.Student.ClassRoom.SectionName : null,
                    Division = a.Student != null && a.Student.ClassRoom != null
                               ? a.Student.ClassRoom.DivisionName : null,
                    Date = a.Date,
                    IsPresent = a.IsPresent,
                    Notes = a.Notes ?? "",
                    ClassRoomId = a.Student != null ? a.Student.ClassRoomId : 0
                })
                .ToList();
        }

        // =========================================================
        // تحميل سجلات الحضور
        // =========================================================
        private void LoadAttendances()
        {
            var records = BuildAttendanceData();

            gvAttendance.DataSource = records;
            gvAttendance.DataBind();

            lblInfo.Text = records.Count > 0
                ? "عدد السجلات: " + records.Count
                : "لا توجد سجلات في هذا التاريخ";

            // ✅ عدّاد Pill في رأس البطاقة
            lblCountPill.Text = records.Count.ToString();

            // ✅ حالة "لا توجد سجلات"
            pnlEmpty.Visible = records.Count == 0;
        }

        // =========================================================
        // الأحداث
        // =========================================================
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            LoadAttendances();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            LoadGradeLevels(CurrentStage);

            if (ddlClassRoom.Items.Count > 0)
                ddlClassRoom.SelectedIndex = 0;

            if (ddlDivision.Items.Count > 0)
                ddlDivision.SelectedIndex = 0;

            ResetSectionFilter();
            LoadAttendances();
        }

        protected void ddlClassRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            LoadAttendances();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAttendances();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAttendances();
        }

        protected void gvAttendance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAttendance.PageIndex = e.NewPageIndex;
            LoadAttendances();
        }

        protected void gvAttendance_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = (e.Row.CssClass + " attendance-table-header-row").Trim();
            else if (e.Row.RowType == DataControlRowType.Pager)
                e.Row.CssClass = (e.Row.CssClass + " attendance-table-pager-row").Trim();
        }

        protected void gvAttendance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            string[] mobileLabels =
            {
                "رقم", "اسم الطالب", "الصف", "القسم", "الشعبة",
                "التاريخ", "الحالة", "ملاحظات", "تعديل"
            };
            for (int i = 0; i < e.Row.Cells.Count && i < mobileLabels.Length; i++)
                e.Row.Cells[i].Attributes["data-label"] = mobileLabels[i];
        }

        // =========================================================
        // تصدير ورد (.docx)
        // =========================================================
        protected void btnExportWord_Click(object sender, EventArgs e)
        {
            var rows = BuildAttendanceData();

            DateTime filterDate;
            if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out filterDate))
                filterDate = DateTime.Today;
            filterDate = filterDate.Date;

            string gradeName = ddlClassRoom.SelectedItem != null
                ? ddlClassRoom.SelectedItem.Text
                : "الكل";

            string sectionDisplay = "";
            string sectionVal = GetSelectedSection();
            if (sectionVal == "علمي") sectionDisplay = "القسم العلمي";
            else if (sectionVal == "أدبي") sectionDisplay = "القسم الأدبي";

            string divisionDisplay = "";
            string divisionVal = GetSelectedDivision();
            if (!string.IsNullOrWhiteSpace(divisionVal))
                divisionDisplay = "شعبة " + divisionVal;

            EducationStage stage = CurrentStage;
            string stageDisplay = stage == EducationStage.Basic ? "الأساسية" : "الثانوية";

            byte[] docxBytes = BuildDocxReport(rows, filterDate, stageDisplay,
                                               gradeName, sectionDisplay, divisionDisplay);

            string fileName = "Attendance_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".docx";

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

        // =========================================================
        // بناء ملف DOCX
        // =========================================================
        private byte[] BuildDocxReport(
            List<AttendanceRow> rows,
            DateTime filterDate,
            string stageDisplay,
            string gradeName,
            string sectionDisplay,
            string divisionDisplay)
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
                        "عدد السجلات : " + rows.Count,
                        fontSize: 20, bold: true, color: "495057",
                        align: WJustify.Center));

                    body.AppendChild(CreateParagraph(
                        "سجل الحضور والغياب",
                        fontSize: 22, bold: true, color: "212529",
                        align: WJustify.Center));

                    body.AppendChild(CreateParagraph(
                        "المرحلة : " + stageDisplay,
                        fontSize: 20, bold: true, color: "212529",
                        align: WJustify.Center));

                    string gradeLine = "الصف : " + gradeName;
                    if (!string.IsNullOrWhiteSpace(sectionDisplay))
                        gradeLine += " | " + sectionDisplay;
                    if (!string.IsNullOrWhiteSpace(divisionDisplay))
                        gradeLine += " | " + divisionDisplay;

                    body.AppendChild(CreateParagraph(
                        gradeLine,
                        fontSize: 20, bold: true, color: "212529",
                        align: WJustify.Center));

                    body.AppendChild(CreateParagraph(
                        "التاريخ : " + filterDate.ToString("yyyy-MM-dd"),
                        fontSize: 18, bold: false, color: "495057",
                        align: WJustify.Center));

                    body.AppendChild(new W.Paragraph());

                    // جدول البيانات — مع عمود الشعبة
                    var table = CreateTable(new int[]
                    {
                        500, 2400, 1800, 1600, 1200, 1200, 1200, 2600
                    });

                    table.AppendChild(CreateRow(new[]
                    {
                        "م", "اسم الطالب", "الصف", "القسم", "الشعبة",
                        "الحالة", "التاريخ", "ملاحظات"
                    }, isHeader: true));

                    int idx = 1;
                    foreach (var r in rows)
                    {
                        bool even = idx % 2 == 0;

                        string sectionText = "—";
                        if (r.Section == "علمي") sectionText = "القسم العلمي";
                        else if (r.Section == "أدبي") sectionText = "القسم الأدبي";
                        else if (!string.IsNullOrWhiteSpace(r.Section)) sectionText = r.Section;

                        string divisionText = string.IsNullOrWhiteSpace(r.Division)
                            ? "—"
                            : "شعبة " + r.Division;

                        table.AppendChild(CreateRow(new[]
                        {
                            idx.ToString(),
                            r.StudentName ?? "",
                            r.ClassName ?? "",
                            sectionText,
                            divisionText,
                            r.IsPresent ? "حاضر" : "غائب",
                            r.Date.ToString("yyyy-MM-dd"),
                            r.Notes ?? ""
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

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}
