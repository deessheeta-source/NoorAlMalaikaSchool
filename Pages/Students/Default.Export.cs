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
    }
}