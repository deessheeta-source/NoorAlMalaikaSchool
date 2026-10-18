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

namespace NoorAlMalaikaSchool.Pages.Attendance
{
    public partial class Report : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        // ✅ الخط المعتمد للتقرير
        private const string ReportFont = "Droid Arabic Kufi";

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            SecurityHelper.HideViewExportControls(this);
            if (!IsPostBack)
            {
                LoadClassRooms();
            }
        }

        private void LoadClassRooms()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();
            var ids = EnumHelper.GetClassRoomIdsForStage(db, stage);
            ddlClassRoom.DataSource = EnumHelper.SortClassRooms(
                db.ClassRooms.Where(c => ids.Contains(c.Id)).ToList());
            ddlClassRoom.DataTextField = "Name";
            ddlClassRoom.DataValueField = "Id";
            ddlClassRoom.DataBind();
        }

        // =========================================================
        // بناء التقرير (مشترك بين العرض والتصدير)
        // =========================================================
        private class ReportRow
        {
            public string StudentName { get; set; }
            public string ClassName { get; set; }
            public int TotalPresent { get; set; }
            public int TotalAbsent { get; set; }
            public double Percentage { get; set; }
        }

        private class ReportResult
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int TotalStudents { get; set; }
            public int TotalPresent { get; set; }
            public int TotalRecordedDays { get; set; }
            public double OverallAvg { get; set; }
            public List<ReportRow> Rows { get; set; }
        }

        private ReportResult BuildReportData()
        {
            DateTime startDate, endDate;
            if (!DateTime.TryParse(txtStartDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                startDate = DateTime.Today.AddMonths(-1);
            if (!DateTime.TryParse(txtEndDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                endDate = DateTime.Today;

            startDate = startDate.Date;
            endDate = endDate.Date;

            EducationStage currentStage = EnumHelper.GetCurrentStage();

            IQueryable<Student> studentsQuery = db.Students
                .Include(s => s.ClassRoom)
                .Where(s => s.Stage == currentStage);

            if (!string.IsNullOrEmpty(ddlClassRoom.SelectedValue))
            {
                int classRoomId = int.Parse(ddlClassRoom.SelectedValue);
                studentsQuery = studentsQuery.Where(s => s.ClassRoomId == classRoomId);
            }

            var students = studentsQuery
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ThenBy(s => s.Id)
                .ToList();

            var studentIds = students.Select(s => s.Id).ToList();
            DateTime endExclusive = endDate.AddDays(1);

            var attendanceByStudent = db.Attendances
                .AsNoTracking()
                .Where(a => a.Date >= startDate && a.Date < endExclusive)
                .Where(a => studentIds.Contains(a.StudentId))
                .GroupBy(a => a.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    Present = g.Count(a => a.IsPresent),
                    Absent = g.Count(a => !a.IsPresent)
                })
                .ToDictionary(x => x.StudentId);

            var rows = new List<ReportRow>();
            int totalPresent = 0;

            foreach (var student in students)
            {
                int present = 0;
                int absent = 0;

                var studentSummary = attendanceByStudent.ContainsKey(student.Id)
                    ? attendanceByStudent[student.Id]
                    : null;

                if (studentSummary != null)
                {
                    present = studentSummary.Present;
                    absent = studentSummary.Absent;
                }

                int totalDays = present + absent;
                double percentage = totalDays > 0
                    ? (double)present / totalDays * 100
                    : 0;

                rows.Add(new ReportRow
                {
                    StudentName = student.Name,
                    ClassName = student.ClassRoom != null ? student.ClassRoom.Name : "",
                    TotalPresent = present,
                    TotalAbsent = absent,
                    Percentage = percentage
                });

                totalPresent += present;
            }

            int totalRecordedDays = attendanceByStudent.Values.Sum(x => x.Present + x.Absent);
            double overallAvg = students.Count > 0 && totalRecordedDays > 0
                ? (double)totalPresent / totalRecordedDays * 100
                : 0;

            return new ReportResult
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalStudents = students.Count,
                TotalPresent = totalPresent,
                TotalRecordedDays = totalRecordedDays,
                OverallAvg = overallAvg,
                Rows = rows
            };
        }

        // =========================================================
        // عرض التقرير
        // =========================================================
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            var result = BuildReportData();

            gvReport.DataSource = result.Rows;
            gvReport.DataBind();

            divSummary.Visible = true;
            lblTotalStudents.Text = result.TotalStudents.ToString();
            lblAvgAttendance.Text = result.OverallAvg.ToString("F1") + "%";
        }

        // =========================================================
        // تصدير ورد (.docx)
        // =========================================================
        protected void btnExportWord_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyViewExport();
            var result = BuildReportData();

            string className = ddlClassRoom.SelectedItem != null
                ? ddlClassRoom.SelectedItem.Text
                : "الكل";

            byte[] docxBytes = BuildDocxReport(result, className);

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

        // =========================================================
        // بناء ملف DOCX
        // =========================================================
        private byte[] BuildDocxReport(ReportResult result, string className)
        {
            using (var ms = new MemoryStream())
            {
                using (var doc = WordprocessingDocument.Create(
                    ms, WordprocessingDocumentType.Document))
                {
                    var mainPart = doc.AddMainDocumentPart();

                    // ✅ الخط الافتراضي على مستوى المستند
                    AddDefaultFontStyle(mainPart);

                    mainPart.Document = new W.Document();
                    var body = mainPart.Document.AppendChild(new W.Body());

                    // إعدادات الصفحة A4 أفقي
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

                    // 2) عدد الطلاب
                    body.AppendChild(CreateParagraph(
                        "عدد الطلاب: " + result.TotalStudents,
                        fontSize: 20, bold: true, color: "495057",
                        align: WJustify.Center));

                    // 3) تقرير الحضور
                    body.AppendChild(CreateParagraph(
                        "تقرير الحضور والغياب",
                        fontSize: 22, bold: true, color: "212529",
                        align: WJustify.Center));

                    // 4) الصف
                    body.AppendChild(CreateParagraph(
                        "الصف: " + className,
                        fontSize: 20, bold: true, color: "212529",
                        align: WJustify.Center));

                    // 5) الفترة
                    body.AppendChild(CreateParagraph(
                        "الفترة: من " + result.StartDate.ToString("yyyy-MM-dd")
                        + " إلى " + result.EndDate.ToString("yyyy-MM-dd"),
                        fontSize: 18, bold: false, color: "495057",
                        align: WJustify.Center));

                    body.AppendChild(new W.Paragraph());

                    // جدول البيانات
                    var table = CreateTable(new int[]
                    {
                        700, 3200, 2600, 1800, 1800, 1800
                    });

                    table.AppendChild(CreateRow(new[]
                    {
                        "م", "اسم الطالب", "الصف",
                        "أيام الحضور", "أيام الغياب", "نسبة الحضور"
                    }, isHeader: true));

                    int idx = 1;
                    foreach (var r in result.Rows)
                    {
                        bool even = idx % 2 == 0;
                        table.AppendChild(CreateRow(new[]
                        {
                            idx.ToString(),
                            r.StudentName ?? "",
                            r.ClassName ?? "",
                            r.TotalPresent.ToString(),
                            r.TotalAbsent.ToString(),
                            r.Percentage.ToString("F1") + "%"
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
        // ✅ ضبط الخط الافتراضي على مستوى المستند (DocDefaults)
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

        // =========================================================
        // ✅ دالة موحّدة للخط المعتمد
        // =========================================================
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

        // =========================================================
        // إنشاء فقرة
        // =========================================================
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

        // =========================================================
        // إنشاء جدول بحدود
        // =========================================================
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

        // =========================================================
        // إنشاء صف داخل الجدول
        // =========================================================
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