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
    public partial class FinancialReport : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

           protected void Page_Load(object sender, EventArgs e)
           {
               // Local guard: revenue figures are finance-only. Do not rely on the
               // Site.Master path check alone.
               SecurityHelper.DenyUnlessPath();
               ConfigureWhatsAppButton();
               SecurityHelper.HideViewExportControls(this);

               if (!IsPostBack)
            {
                txtStartDate.Text = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
                txtEndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                LoadAcademicYears();
                LoadGradeLevels();
                ConfigureSectionFilter();
            }
        }

        private void LoadAcademicYears()
        {
            var years = db.AcademicYears.OrderByDescending(y => y.Id).ToList();

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
            EducationStage stage = EnumHelper.GetCurrentStage();

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

            bool showSection = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = showSection;

            if (!showSection)
            {
                if (ddlSection.Items.FindByValue("") != null)
                {
                    ddlSection.ClearSelection();
                    ddlSection.Items.FindByValue("").Selected = true;
                }
            }
        }

        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
        }

        // =========================================================
        // عرض التقرير
        // =========================================================
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime startDate, endDate;
            if (!DateTime.TryParse(txtStartDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate)) startDate = DateTime.Today.AddMonths(-1);
            if (!DateTime.TryParse(txtEndDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate)) endDate = DateTime.Today;

            if (startDate.Date > endDate.Date) { ClearSummary(); return; }

            DateTime endExclusive = endDate.Date.AddDays(1);

            IQueryable<Invoice> query = db.Invoices
                .Include(i => i.Student)
                .Include(i => i.Lines)
                .Include(i => i.AcademicYear)
                .Where(i => i.InvoiceDate >= startDate.Date
                         && i.InvoiceDate < endExclusive
                         && i.Status != InvoiceStatus.Cancelled);

            int yearId;
            if (int.TryParse(ddlAcademicYear.SelectedValue, out yearId) && yearId > 0)
                query = query.Where(i => i.AcademicYearId == yearId);

            int gradeLevelId;
            if (int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) && gradeLevelId > 0)
                query = query.Where(i => i.Student.ClassRoom != null
                                      && i.Student.ClassRoom.GradeLevelId == gradeLevelId);

            string section = ddlSection.SelectedValue;
            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(i => i.Student.Section == section);

            var invoices = query
                .OrderBy(i => i.Student.Name)
                .ThenBy(i => i.InvoiceDate)
                .ToList();

            var ids = invoices.Select(i => i.Id).ToList();
            var allPayments = ids.Count == 0
                ? new List<Payment>()
                : db.Payments.Where(p => p.InvoiceId.HasValue && ids.Contains(p.InvoiceId.Value)).ToList();

            var paidByInvoice = allPayments
                .GroupBy(p => p.InvoiceId.Value)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            var reportData = invoices.Select(i =>
            {
                decimal paid = paidByInvoice.ContainsKey(i.Id) ? paidByInvoice[i.Id] : 0m;
                decimal remaining = Math.Max(0m, i.NetAmount - paid);

                return new
                {
                    AcademicYearName = i.AcademicYear != null ? i.AcademicYear.Name : i.AcademicYearId.ToString(),
                    StudentName = i.Student != null ? i.Student.Name : "—",
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    DiscountAmount = i.DiscountAmount,
                    NetAmount = i.NetAmount,
                    TotalPaid = paid,
                    Remaining = remaining,
                    Status = remaining <= 0 ? "مدفوعة بالكامل"
                           : (paid > 0 ? "مدفوعة جزئياً" : "غير مدفوعة")
                };
            }).ToList();

            gvReport.DataSource = reportData;
            gvReport.DataBind();
            pnlSummary.Visible = true;

            lblTotalStudents.Text = invoices.Select(i => i.StudentId).Distinct().Count().ToString();
            lblTotalFees.Text = invoices.Sum(i => i.TotalAmount).ToString("N2");
            lblTotalDiscount.Text = invoices.Sum(i => i.DiscountAmount).ToString("N2");
            lblTotalNet.Text = invoices.Sum(i => i.NetAmount).ToString("N2");
            lblTotalPayments.Text = allPayments.Sum(p => p.Amount).ToString("N2");
            lblRemaining.Text = Math.Max(0m,
                invoices.Sum(i => i.NetAmount) - allPayments.Sum(p => p.Amount)).ToString("N2");
        }

        private void ClearSummary()
        {
            lblTotalStudents.Text = "0";
            lblTotalFees.Text = "0.00";
            lblTotalDiscount.Text = "0.00";
            lblTotalNet.Text = "0.00";
            lblTotalPayments.Text = "0.00";
            lblRemaining.Text = "0.00";
            pnlSummary.Visible = true;
            gvReport.DataSource = null;
            gvReport.DataBind();
        }

        // =========================================================
        // تصدير التقرير بصيغة Word (.docx)
        // =========================================================
        protected void btnExportWord_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyViewExport();
            DateTime startDate, endDate;
            if (!DateTime.TryParse(txtStartDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate)) startDate = DateTime.Today.AddMonths(-1);
            if (!DateTime.TryParse(txtEndDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate)) endDate = DateTime.Today;

            if (startDate.Date > endDate.Date)
            {
                Response.Redirect(Request.RawUrl);
                return;
            }

            DateTime endExclusive = endDate.Date.AddDays(1);

            IQueryable<Invoice> query = db.Invoices
                .Include(i => i.Student)
                .Include(i => i.Lines)
                .Include(i => i.AcademicYear)
                .Where(i => i.InvoiceDate >= startDate.Date
                         && i.InvoiceDate < endExclusive
                         && i.Status != InvoiceStatus.Cancelled);

            int yearId;
            if (int.TryParse(ddlAcademicYear.SelectedValue, out yearId) && yearId > 0)
                query = query.Where(i => i.AcademicYearId == yearId);

            int gradeLevelId;
            if (int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) && gradeLevelId > 0)
                query = query.Where(i => i.Student.ClassRoom != null
                                      && i.Student.ClassRoom.GradeLevelId == gradeLevelId);

            string section = ddlSection.SelectedValue;
            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(i => i.Student.Section == section);

            var invoices = query
                .OrderBy(i => i.Student.Name)
                .ThenBy(i => i.InvoiceDate)
                .ToList();

            var ids = invoices.Select(i => i.Id).ToList();
            var allPayments = ids.Count == 0
                ? new List<Payment>()
                : db.Payments.Where(p => p.InvoiceId.HasValue && ids.Contains(p.InvoiceId.Value)).ToList();

            var paidByInvoice = allPayments
                .GroupBy(p => p.InvoiceId.Value)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            var rows = invoices.Select(i =>
            {
                decimal paid = paidByInvoice.ContainsKey(i.Id) ? paidByInvoice[i.Id] : 0m;
                decimal remaining = Math.Max(0m, i.NetAmount - paid);

                return new WordRow
                {
                    AcademicYearName = i.AcademicYear != null ? i.AcademicYear.Name : i.AcademicYearId.ToString(),
                    StudentName = i.Student != null ? i.Student.Name : "—",
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    DiscountAmount = i.DiscountAmount,
                    NetAmount = i.NetAmount,
                    TotalPaid = paid,
                    Remaining = remaining,
                    Status = remaining <= 0 ? "مدفوعة بالكامل"
                           : (paid > 0 ? "مدفوعة جزئياً" : "غير مدفوعة")
                };
            }).ToList();

            int totalStudents = invoices.Select(i => i.StudentId).Distinct().Count();
            decimal totalFees = invoices.Sum(i => i.TotalAmount);
            decimal totalDiscount = invoices.Sum(i => i.DiscountAmount);
            decimal totalNet = invoices.Sum(i => i.NetAmount);
            decimal totalPaid = allPayments.Sum(p => p.Amount);
            decimal totalRemaining = Math.Max(0m, totalNet - totalPaid);

            byte[] docxBytes = BuildDocxReport(
                startDate, endDate,
                totalStudents, totalFees, totalDiscount, totalNet, totalPaid, totalRemaining,
                rows);

            string fileName = "FinancialReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".docx";

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

        // =========================================================
        private void ConfigureWhatsAppButton()
        {
            string managerPhone = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_MANAGER_PHONE", "WhatsAppManagerPhone");
            string number = WhatsAppCloudApi.NormalizeRecipientNumber(managerPhone);
            btnExportWhatsAppPdf.Visible = number.Length >= 10;
            if (!btnExportWhatsAppPdf.Visible) return;

            string script = "var s=document.getElementById('" + txtStartDate.ClientID + "').value;" +
                "var e=document.getElementById('" + txtEndDate.ClientID + "').value;" +
                "if(s&&e&&s>e){alert('تاريخ البداية يجب أن يسبق تاريخ النهاية.');return false;}" +
                "var msg='السلام عليكم، أرسل لكم التقرير المالي للفترة '+s+' إلى '+e+'.';" +
                "window.open('https://wa.me/" + number + "?text='+encodeURIComponent(msg),'_blank');";
            btnExportWhatsAppPdf.OnClientClick = script;
        }

        protected void btnExportWhatsAppPdf_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyViewExport();

            DateTime startDate, endDate;
            if (!DateTime.TryParse(txtStartDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                startDate = DateTime.Today.AddMonths(-1);
            if (!DateTime.TryParse(txtEndDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                endDate = DateTime.Today;
            if (startDate.Date > endDate.Date) return;

            string managerPhone = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_MANAGER_PHONE", "WhatsAppManagerPhone");
            if (WhatsAppCloudApi.NormalizeRecipientNumber(managerPhone).Length < 10) return;

            DateTime endExclusive = endDate.Date.AddDays(1);
            IQueryable<Invoice> query = db.Invoices
                .Include(i => i.Student)
                .Include(i => i.Lines)
                .Include(i => i.AcademicYear)
                .Where(i => i.InvoiceDate >= startDate.Date
                         && i.InvoiceDate < endExclusive
                         && i.Status != InvoiceStatus.Cancelled);

            int yearId;
            if (int.TryParse(ddlAcademicYear.SelectedValue, out yearId) && yearId > 0)
                query = query.Where(i => i.AcademicYearId == yearId);
            int gradeLevelId;
            if (int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) && gradeLevelId > 0)
                query = query.Where(i => i.Student.ClassRoom != null
                                      && i.Student.ClassRoom.GradeLevelId == gradeLevelId);
            string section = ddlSection.SelectedValue;
            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(i => i.Student.Section == section);

            var invoices = query.OrderBy(i => i.Student.Name).ThenBy(i => i.InvoiceDate).ToList();
            var ids = invoices.Select(i => i.Id).ToList();
            var payments = ids.Count == 0
                ? new List<Payment>()
                : db.Payments.Where(p => p.InvoiceId.HasValue && ids.Contains(p.InvoiceId.Value)).ToList();
            var paidByInvoice = payments.GroupBy(p => p.InvoiceId.Value)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));
            var rows = invoices.Select(i =>
            {
                decimal paid = paidByInvoice.ContainsKey(i.Id) ? paidByInvoice[i.Id] : 0m;
                decimal remaining = Math.Max(0m, i.NetAmount - paid);
                return new FinancialReportPdfRow
                {
                    AcademicYearName = i.AcademicYear != null ? i.AcademicYear.Name : i.AcademicYearId.ToString(),
                    StudentName = i.Student != null ? i.Student.Name : "—",
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    DiscountAmount = i.DiscountAmount,
                    NetAmount = i.NetAmount,
                    TotalPaid = paid,
                    Remaining = remaining,
                    Status = remaining <= 0 ? "مدفوعة بالكامل" : (paid > 0 ? "مدفوعة جزئياً" : "غير مدفوعة")
                };
            }).ToList();

            decimal totalFees = invoices.Sum(i => i.TotalAmount);
            decimal totalDiscount = invoices.Sum(i => i.DiscountAmount);
            decimal totalNet = invoices.Sum(i => i.NetAmount);
            decimal totalPaid = payments.Sum(p => p.Amount);
            byte[] pdf = FinancialReportPdf.Create(
                startDate, endDate,
                ddlAcademicYear.SelectedItem == null ? "الكل" : ddlAcademicYear.SelectedItem.Text,
                ddlGradeLevel.SelectedItem == null ? "الكل" : ddlGradeLevel.SelectedItem.Text,
                section,
                invoices.Select(i => i.StudentId).Distinct().Count(),
                totalFees, totalDiscount, totalNet, totalPaid,
                Math.Max(0m, totalNet - totalPaid), rows);

            string fileName = "FinancialReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf";
            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.AddHeader("Content-Length", pdf.Length.ToString(CultureInfo.InvariantCulture));
            Response.BinaryWrite(pdf);
            Response.Flush();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest();
        }
        // صف بسيط للبيانات
        // =========================================================
        private class WordRow
        {
            public string AcademicYearName { get; set; }
            public string StudentName { get; set; }
            public string InvoiceNumber { get; set; }
            public DateTime InvoiceDate { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal NetAmount { get; set; }
            public decimal TotalPaid { get; set; }
            public decimal Remaining { get; set; }
            public string Status { get; set; }
        }

        // =========================================================
        // بناء ملف DOCX في الذاكرة
        // =========================================================
        private byte[] BuildDocxReport(
            DateTime startDate, DateTime endDate,
            int totalStudents, decimal totalFees, decimal totalDiscount,
            decimal totalNet, decimal totalPaid, decimal totalRemaining,
            List<WordRow> rows)
        {
            using (var ms = new MemoryStream())
            {
                using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                {
                    var mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new W.Document();
                    var body = mainPart.Document.AppendChild(new W.Body());

                    // ===== إعدادات الصفحة (A4 أفقي) =====
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

                    // ===== العنوان =====
                    body.AppendChild(CreateParagraph(
                        "مدرسة نور الملائكة",
                        fontSize: 36, bold: true, color: "00B14F",
                        align: WJustify.Center));

                    body.AppendChild(CreateParagraph(
                        "التقرير المالي والتحصيل",
                        fontSize: 26, bold: true, color: "212529",
                        align: WJustify.Center));

                    string dateLine = "من: " + startDate.ToString("yyyy-MM-dd")
                        + "   |   إلى: " + endDate.ToString("yyyy-MM-dd")
                        + "   |   تاريخ الطباعة: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                    body.AppendChild(CreateParagraph(
                        dateLine,
                        fontSize: 18, bold: false, color: "495057",
                        align: WJustify.Center));

                    body.AppendChild(new W.Paragraph());

                    // ===== جدول الملخص =====
                    var summaryTable = CreateTable(new int[] { 1666, 1666, 1666, 1666, 1666, 1666 });

                    summaryTable.AppendChild(CreateRow(new[]
                    {
                        "الطلاب", "الإجمالي", "الخصومات", "الصافي", "المحصل", "المتبقي"
                    }, isHeader: true));

                    summaryTable.AppendChild(CreateRow(new[]
                    {
                        totalStudents.ToString(),
                        totalFees.ToString("N2") + " د.ل",
                        totalDiscount.ToString("N2") + " د.ل",
                        totalNet.ToString("N2") + " د.ل",
                        totalPaid.ToString("N2") + " د.ل",
                        totalRemaining.ToString("N2") + " د.ل"
                    }, isHeader: false, isBold: true));

                    body.AppendChild(summaryTable);
                    body.AppendChild(new W.Paragraph());

                    // ===== جدول التفاصيل =====
                    var detailsTable = CreateTable(new int[]
                    {
                        400, 1100, 1900, 1100, 1000, 1000, 1000, 1000, 1000, 1000, 1300
                    });

                    detailsTable.AppendChild(CreateRow(new[]
                    {
                        "م", "السنة الدراسية", "الطالب", "الفاتورة", "التاريخ",
                        "الإجمالي", "الخصم", "الصافي", "المدفوع", "المتبقي", "الحالة"
                    }, isHeader: true));

                    int idx = 1;
                    foreach (var r in rows)
                    {
                        bool even = idx % 2 == 0;
                        detailsTable.AppendChild(CreateRow(new[]
                        {
                            idx.ToString(),
                            r.AcademicYearName,
                            r.StudentName,
                            r.InvoiceNumber,
                            r.InvoiceDate.ToString("yyyy-MM-dd"),
                            r.TotalAmount.ToString("N2"),
                            r.DiscountAmount.ToString("N2"),
                            r.NetAmount.ToString("N2"),
                            r.TotalPaid.ToString("N2"),
                            r.Remaining.ToString("N2"),
                            r.Status
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
        // إنشاء فقرة
        // =========================================================
        private W.Paragraph CreateParagraph(
            string text, int fontSize, bool bold, string color, WJustify align)
        {
            var runProps = new W.RunProperties(
                new W.RunFonts
                {
                    Ascii = "Arial",
                    HighAnsi = "Arial",
                    ComplexScript = "Arial"
                },
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

            var run = new W.Run(
                runProps,
                new W.Text(text ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve });

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
                    new W.TableCellVerticalAlignment { Val = W.TableVerticalAlignmentValues.Center },
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
                    new W.RunFonts
                    {
                        Ascii = "Arial",
                        HighAnsi = "Arial",
                        ComplexScript = "Arial"
                    },
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

                var paraProps = new W.ParagraphProperties(
                    new W.Justification { Val = WJustify.Center },
                    new W.BiDi());

                var para = new W.Paragraph(
                    paraProps,
                    new W.Run(runProps,
                        new W.Text(val ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve }));

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

        // =========================================================
        // ✅ تنسيق حالة الفاتورة كـ badge
        // =========================================================
        protected string FormatStatus(object status)
        {
            string value = Convert.ToString(status).Trim();

            if (value.Contains("مدفوعة بالكامل"))
                return "<span class='fr-status paid'><i class='fas fa-check-circle' style='font-size:10px;'></i> " + System.Web.HttpUtility.HtmlEncode(value) + "</span>";

            if (value.Contains("مدفوعة جزئي"))
                return "<span class='fr-status partial'><i class='fas fa-hourglass-half' style='font-size:10px;'></i> " + System.Web.HttpUtility.HtmlEncode(value) + "</span>";

            if (value.Contains("غير مدفوعة"))
                return "<span class='fr-status unpaid'><i class='fas fa-times-circle' style='font-size:10px;'></i> " + System.Web.HttpUtility.HtmlEncode(value) + "</span>";

            return "<span class='fr-status partial'>" + System.Web.HttpUtility.HtmlEncode(value) + "</span>";
        }
    }
}