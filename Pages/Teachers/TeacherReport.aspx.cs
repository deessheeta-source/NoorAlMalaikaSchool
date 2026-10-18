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

namespace NoorAlMalaikaSchool.Pages.Teachers
{
    public partial class TeacherReport : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private const string ReportFont = "Droid Arabic Kufi";

        private EducationStage CurrentStage
        {
            get { return EnumHelper.GetCurrentStage(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            SecurityHelper.HideViewExportControls(this);
            if (!IsPostBack)
            {
                LoadSpecialtyFilter();
                LoadTeachers();
            }
        }

        private string GetStageDisplayName()
        {
            return CurrentStage == EducationStage.Basic ? "الأساسية" : "الثانوية";
        }

        // =========================================================
        // تحميل فلتر التخصص
        // =========================================================
        private void LoadSpecialtyFilter()
        {
            var stage = CurrentStage;

            // 1) من جدول المواد
            var subjectNames = db.Subjects
                .AsNoTracking()
                .Where(s => s.Stage == stage)
                .Select(s => s.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            ddlSpecialty.Items.Clear();
            ddlSpecialty.Items.Add(new ListItem("جميع التخصصات", ""));

            foreach (var name in subjectNames)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    ddlSpecialty.Items.Add(new ListItem(name, name));
                }
            }

            // 2) إضافة التخصصات القديمة من بطاقات المعلمين
            var teacherSpecialties = db.Teachers
                .AsNoTracking()
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

        // =========================================================
        // بناء استعلام المعلمين مع الفلاتر
        // =========================================================
        private IQueryable<Teacher> BuildTeachersQuery()
        {
            var stage = CurrentStage;

            IQueryable<Teacher> query = db.Teachers
                .AsNoTracking()
                .Where(t => t.Stage == stage);

            // فلتر البحث بالاسم
            string search = txtSearch.Text != null ? txtSearch.Text.Trim() : "";
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Name.Contains(search));
            }

            // فلتر التخصص
            string specialty = ddlSpecialty.SelectedValue;
            if (!string.IsNullOrWhiteSpace(specialty))
            {
                query = query.Where(t => t.Specialty == specialty);
            }

            // فلتر الجنس
            string genderValue = ddlGender.SelectedValue;
            if (!string.IsNullOrWhiteSpace(genderValue))
            {
                Gender gender;
                if (Enum.TryParse(genderValue, true, out gender))
                {
                    query = query.Where(t => t.Gender == gender);
                }
            }

            // فلتر الحالة
            string statusValue = ddlStatus.SelectedValue;
            if (!string.IsNullOrWhiteSpace(statusValue))
            {
                bool isActive = statusValue == "1";
                query = query.Where(t => t.IsActive == isActive);
            }

            return query;
        }

        // =========================================================
        // تحميل المعلمين في الجدول
        // =========================================================
        private void LoadTeachers()
        {
            var query = BuildTeachersQuery();

            var teachers = query
                .OrderBy(t => t.Name)
                .ThenBy(t => t.Id)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Gender,
                    t.Specialty,
                    t.Phone,
                    t.IsActive
                })
                .ToList()
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    GenderName = EnumHelper.GetDisplayName(t.Gender),
                    t.Specialty,
                    t.Phone,
                    t.IsActive
                })
                .ToList();

            gvTeachers.DataSource = teachers;
            gvTeachers.DataBind();

            lblCount.Text = teachers.Count.ToString();
        }

        // =========================================================
        // أحداث الفلاتر
        // =========================================================
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            gvTeachers.PageIndex = 0;
            LoadTeachers();
        }

        protected void ddlSpecialty_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvTeachers.PageIndex = 0;
            LoadTeachers();
        }

        protected void ddlGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvTeachers.PageIndex = 0;
            LoadTeachers();
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvTeachers.PageIndex = 0;
            LoadTeachers();
        }

        protected void gvTeachers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTeachers.PageIndex = e.NewPageIndex;
            LoadTeachers();
        }

        // =========================================================
        // نموذج صف التصدير
        // =========================================================
        private class TeacherReportRow
        {
            public string Name { get; set; }
            public string GenderName { get; set; }
            public string Specialty { get; set; }
            public string Phone { get; set; }
            public string StatusName { get; set; }
        }

        // =========================================================
        // بناء صفوف التصدير
        // =========================================================
        private List<TeacherReportRow> BuildExportRows()
        {
            var query = BuildTeachersQuery();

            var raw = query
                .OrderBy(t => t.Name)
                .ThenBy(t => t.Id)
                .Select(t => new
                {
                    t.Name,
                    t.Gender,
                    t.Specialty,
                    t.Phone,
                    t.IsActive
                })
                .ToList();

            return raw
                .Select(t => new TeacherReportRow
                {
                    Name = t.Name,
                    GenderName = EnumHelper.GetDisplayName(t.Gender),
                    Specialty = string.IsNullOrWhiteSpace(t.Specialty) ? "—" : t.Specialty,
                    Phone = string.IsNullOrWhiteSpace(t.Phone) ? "—" : t.Phone,
                    StatusName = t.IsActive ? "نشط" : "غير نشط"
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

            byte[] docxBytes = BuildDocxTeacherReport(
                rows,
                GetStageDisplayName(),
                ddlSpecialty.SelectedItem != null && ddlSpecialty.SelectedIndex > 0
                    ? ddlSpecialty.SelectedItem.Text
                    : "",
                ddlGender.SelectedItem != null && ddlGender.SelectedIndex > 0
                    ? ddlGender.SelectedItem.Text
                    : "",
                ddlStatus.SelectedItem != null && ddlStatus.SelectedIndex > 0
                    ? ddlStatus.SelectedItem.Text
                    : "");

            string fileName = "TeacherReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".docx";

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
        // تصدير اكسل
        // =========================================================
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyViewExport();
            var rows = BuildExportRows();

            byte[] xlsxBytes = BuildXlsxTeacherReport(
                rows,
                GetStageDisplayName(),
                ddlSpecialty.SelectedItem != null && ddlSpecialty.SelectedIndex > 0
                    ? ddlSpecialty.SelectedItem.Text
                    : "",
                ddlGender.SelectedItem != null && ddlGender.SelectedIndex > 0
                    ? ddlGender.SelectedItem.Text
                    : "",
                ddlStatus.SelectedItem != null && ddlStatus.SelectedIndex > 0
                    ? ddlStatus.SelectedItem.Text
                    : "");

            string fileName = "TeacherReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";

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
        // بناء ملف Word
        // =========================================================
        private byte[] BuildDocxTeacherReport(
            List<TeacherReportRow> rows,
            string stageDisplay,
            string specialtyDisplay,
            string genderDisplay,
            string statusDisplay)
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

                    // 1) اسم المدرسة
                    body.AppendChild(CreateParagraph(
                        "مدرسة نور الملائكة",
                        fontSize: 32, bold: true, color: "00B14F",
                        align: WJustify.Center));

                    // 2) عدد المعلمين
                    body.AppendChild(CreateParagraph(
                        "عدد المعلمين : " + rows.Count.ToString(),
                        fontSize: 20, bold: true, color: "495057",
                        align: WJustify.Center));

                    // 3) المرحلة
                    body.AppendChild(CreateParagraph(
                        "تقرير المعلمين - المرحلة : " + stageDisplay,
                        fontSize: 20, bold: true, color: "212529",
                        align: WJustify.Center));

                    // 4) سطر الفلاتر
                    var filterParts = new List<string>();
                    if (!string.IsNullOrWhiteSpace(specialtyDisplay)) filterParts.Add("التخصص: " + specialtyDisplay);
                    if (!string.IsNullOrWhiteSpace(genderDisplay)) filterParts.Add("الجنس: " + genderDisplay);
                    if (!string.IsNullOrWhiteSpace(statusDisplay)) filterParts.Add("الحالة: " + statusDisplay);

                    if (filterParts.Count > 0)
                    {
                        body.AppendChild(CreateParagraph(
                            "الفلاتر : " + string.Join(" | ", filterParts),
                            fontSize: 20, bold: true, color: "212529",
                            align: WJustify.Center));
                    }

                    body.AppendChild(new W.Paragraph());

                    // جدول بـ 5 أعمدة
                    var table = CreateTable(new int[]
                    {
                        3200, 1500, 2400, 2000, 1500
                    });

                    table.AppendChild(CreateRow(new[]
                    {
                        "الاسم", "الجنس", "التخصص", "الهاتف", "الحالة"
                    }, isHeader: true));

                    int idx = 1;
                    foreach (var r in rows)
                    {
                        bool even = idx % 2 == 0;
                        table.AppendChild(CreateRow(new[]
                        {
                            r.Name ?? "",
                            r.GenderName ?? "",
                            r.Specialty ?? "",
                            r.Phone ?? "",
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
        // بناء ملف Excel
        // =========================================================
        private byte[] BuildXlsxTeacherReport(
            List<TeacherReportRow> rows,
            string stageDisplay,
            string specialtyDisplay,
            string genderDisplay,
            string statusDisplay)
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
                        new[] { "عدد المعلمين : " + rows.Count.ToString() }, 2U, true));

                    sheetData.AppendChild(CreateXlsxRow(3U, 22D,
                        new[] { "تقرير المعلمين - المرحلة : " + stageDisplay }, 2U, true));

                    // سطر الفلاتر
                    var filterParts = new List<string>();
                    if (!string.IsNullOrWhiteSpace(specialtyDisplay)) filterParts.Add("التخصص: " + specialtyDisplay);
                    if (!string.IsNullOrWhiteSpace(genderDisplay)) filterParts.Add("الجنس: " + genderDisplay);
                    if (!string.IsNullOrWhiteSpace(statusDisplay)) filterParts.Add("الحالة: " + statusDisplay);

                    string filterText = filterParts.Count > 0
                        ? "الفلاتر : " + string.Join(" | ", filterParts)
                        : "الفلاتر : الكل";

                    sheetData.AppendChild(CreateXlsxRow(4U, 22D,
                        new[] { filterText }, 2U, true));

                    sheetData.AppendChild(new S.Row { RowIndex = 5U });

                    // رأس الجدول
                    string[] headers = { "الاسم", "الجنس", "التخصص", "الهاتف", "الحالة" };
                    sheetData.AppendChild(CreateXlsxRow(6U, 28D, headers, 3U, false));

                    // البيانات
                    uint currentRow = 7U;
                    int idx = 1;
                    foreach (var r in rows)
                    {
                        bool even = idx % 2 == 0;
                        uint style = even ? 5U : 4U;

                        string[] values = {
                            r.Name ?? "",
                            r.GenderName ?? "",
                            r.Specialty ?? "",
                            r.Phone ?? "",
                            r.StatusName ?? ""
                        };

                        sheetData.AppendChild(CreateXlsxRow(currentRow, 22D, values, style, false));
                        currentRow++;
                        idx++;
                    }

                    // الأعمدة
                    var columns = new S.Columns();
                    double[] widths = { 35, 15, 25, 20, 15 };
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

                    // دمج خلايا العنوان
                    var mergeCells = new S.MergeCells();
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A1:E1" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A2:E2" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A3:E3" });
                    mergeCells.AppendChild(new S.MergeCell { Reference = "A4:E4" });
                    mergeCells.Count = 4U;

                    // إعدادات العرض
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
                        Name = "تقرير المعلمين"
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

            string[] columnLetters = { "A", "B", "C", "D", "E" };

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

        // =========================================================
        // إنشاء خلية نصية Inline
        // =========================================================
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
        // بناء Stylesheet الخاص بـ Excel
        // =========================================================
        private S.Stylesheet CreateXlsxStylesheet()
        {
            var stylesheet = new S.Stylesheet();

            // Fonts
            var fonts = new S.Fonts();

            fonts.AppendChild(new S.Font(
                new S.FontSize { Val = 11D },
                new S.FontName { Val = ReportFont }
            ));

            fonts.AppendChild(new S.Font(
                new S.Bold(),
                new S.FontSize { Val = 16D },
                new S.Color { Rgb = "FF00B14F" },
                new S.FontName { Val = ReportFont }
            ));

            fonts.AppendChild(new S.Font(
                new S.Bold(),
                new S.FontSize { Val = 11D },
                new S.Color { Rgb = "FFFFFFFF" },
                new S.FontName { Val = ReportFont }
            ));

            fonts.AppendChild(new S.Font(
                new S.FontSize { Val = 10D },
                new S.Color { Rgb = "FF495057" },
                new S.FontName { Val = ReportFont }
            ));

            fonts.Count = 4U;
            stylesheet.AppendChild(fonts);

            // Fills
            var fills = new S.Fills();

            fills.AppendChild(new S.Fill(new S.PatternFill { PatternType = SFillVal.None }));
            fills.AppendChild(new S.Fill(new S.PatternFill { PatternType = SFillVal.Gray125 }));
            fills.AppendChild(new S.Fill(new S.PatternFill
            {
                PatternType = SFillVal.Solid,
                ForegroundColor = new S.ForegroundColor { Rgb = "FF00B14F" }
            }));
            fills.AppendChild(new S.Fill(new S.PatternFill
            {
                PatternType = SFillVal.Solid,
                ForegroundColor = new S.ForegroundColor { Rgb = "FFF9FAFB" }
            }));

            fills.Count = 4U;
            stylesheet.AppendChild(fills);

            // Borders
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

            // CellFormats
            var cellFormats = new S.CellFormats();

            cellFormats.AppendChild(new S.CellFormat
            {
                FontId = 0U,
                FillId = 0U,
                BorderId = 0U
            });

            cellFormats.AppendChild(new S.CellFormat
            {
                FontId = 1U,
                FillId = 0U,
                BorderId = 0U,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new S.Alignment
                {
                    Horizontal = SHAlignVal.Center,
                    Vertical = SVAlignVal.Center,
                    ReadingOrder = 2U
                }
            });

            cellFormats.AppendChild(new S.CellFormat
            {
                FontId = 3U,
                FillId = 0U,
                BorderId = 0U,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new S.Alignment
                {
                    Horizontal = SHAlignVal.Center,
                    Vertical = SVAlignVal.Center,
                    ReadingOrder = 2U
                }
            });

            cellFormats.AppendChild(new S.CellFormat
            {
                FontId = 2U,
                FillId = 2U,
                BorderId = 1U,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true,
                Alignment = new S.Alignment
                {
                    Horizontal = SHAlignVal.Center,
                    Vertical = SVAlignVal.Center,
                    ReadingOrder = 2U,
                    WrapText = true
                }
            });

            cellFormats.AppendChild(new S.CellFormat
            {
                FontId = 0U,
                FillId = 0U,
                BorderId = 1U,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true,
                Alignment = new S.Alignment
                {
                    Horizontal = SHAlignVal.Center,
                    Vertical = SVAlignVal.Center,
                    ReadingOrder = 2U
                }
            });

            cellFormats.AppendChild(new S.CellFormat
            {
                FontId = 0U,
                FillId = 3U,
                BorderId = 1U,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true,
                Alignment = new S.Alignment
                {
                    Horizontal = SHAlignVal.Center,
                    Vertical = SVAlignVal.Center,
                    ReadingOrder = 2U
                }
            });

            cellFormats.Count = 6U;
            stylesheet.AppendChild(cellFormats);

            return stylesheet;
        }

        // =========================================================
        // الخط الافتراضي (Word)
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