using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace NoorAlMalaikaSchool.Services
{
    public sealed class FinancialReportPdfRow
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

    public static class FinancialReportPdf
    {
        private const int W = 1680, H = 1188, Left = 55, CW = 1570;
        private static readonly int[] Widths = { 135, 230, 160, 140, 150, 140, 150, 150, 150, 165 };
        private static readonly string[] Headers = { "السنة", "الطالب", "الفاتورة", "التاريخ", "الإجمالي", "الخصم/الإعفاء", "الصافي", "المدفوع", "المتبقي", "الحالة" };
        private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("ar-LY");

        public static byte[] Create(DateTime from, DateTime to, string year, string grade, string section,
            int students, decimal total, decimal discount, decimal net, decimal paid, decimal remaining,
            IList<FinancialReportPdfRow> rows)
        {
            rows = rows ?? new List<FinancialReportPdfRow>();
            var images = new List<byte[]>();
            using (var fonts = new PrivateFontCollection())
            {
                FontFamily family;
                try
                {
                    string fontPath = System.Web.HttpContext.Current.Server.MapPath("~/Content/fonts/droid-arabic-kufi/ArbFONTS-Droid Arabic Kufi.ttf");
                    fonts.AddFontFile(fontPath);
                    family = fonts.Families[0];
                }
                catch { family = new FontFamily("Arial"); }
                int start = 0, page = 1;
                do
                {
                    bool first = page == 1;
                    int top = first ? 330 : 230, hh = 56, rh = 42;
                    int capacity = Math.Max(1, (H - top - hh - 70) / rh);
                    int count = Math.Min(capacity, rows.Count - start);
                    images.Add(Render(family, from, to, year, grade, section, students, total, discount, net, paid, remaining,
                        rows, start, count, page, first, top, hh, rh));
                    start += count;
                    page++;
                } while (start < rows.Count);
            }
            return MakePdf(images);
        }

        private static byte[] Render(FontFamily family, DateTime from, DateTime to, string year, string grade, string section,
            int students, decimal total, decimal discount, decimal net, decimal paid, decimal remaining,
            IList<FinancialReportPdfRow> rows, int start, int count, int page, bool first, int top, int hh, int rh)
        {
            using (var bmp = new Bitmap(W, H, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bmp))
            using (var ms = new MemoryStream())
            using (var fmt = Rtl())
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.Clear(Color.White);
                using (var title = new Font(family, 32, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var sub = new Font(family, 22, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var normal = new Font(family, 18, FontStyle.Regular, GraphicsUnit.Pixel))
                using (var small = new Font(family, 15, FontStyle.Regular, GraphicsUnit.Pixel))
                using (var header = new Font(family, 16, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var rf = new Font(family, 15, FontStyle.Regular, GraphicsUnit.Pixel))
                using (var ink = new SolidBrush(Color.FromArgb(33, 45, 41)))
                using (var green = new SolidBrush(Color.FromArgb(0, 145, 65)))
                using (var muted = new SolidBrush(Color.FromArgb(95, 105, 101)))
                using (var pen = new Pen(Color.FromArgb(204, 222, 211), 1))
                {
                    g.DrawString("مدرسة نور الملائكة", title, green, new RectangleF(Left, 35, CW, 48), fmt);
                    g.DrawString("التقرير المالي وتفاصيل الفواتير", sub, ink, new RectangleF(Left, 88, CW, 38), fmt);
                    string period = "الفترة: " + from.ToString("yyyy-MM-dd") + " إلى " + to.ToString("yyyy-MM-dd") +
                        "   |   السنة الدراسية: " + Safe(year) + "   |   الصف: " + Safe(grade) +
                        (String.IsNullOrWhiteSpace(section) ? "" : "   |   القسم: " + section);
                    g.DrawString(period, normal, muted, new RectangleF(Left, 137, CW, 30), fmt);
                    if (first) DrawSummary(g, family, students, total, discount, net, paid, remaining, pen, ink, muted, fmt);
                    using (var bg = new SolidBrush(Color.FromArgb(227, 243, 233)))
                    using (var dark = new SolidBrush(Color.FromArgb(28, 61, 45)))
                    {
                        g.FillRectangle(bg, Left, top, CW, hh);
                        int x = Left + CW;
                        for (int i = 0; i < Widths.Length; i++)
                        {
                            x -= Widths[i];
                            g.DrawRectangle(pen, x, top, Widths[i], hh);
                            g.DrawString(Headers[i], header, dark, new RectangleF(x + 4, top + 4, Widths[i] - 8, hh - 8), fmt);
                        }
                    }
                    if (count == 0)
                        g.DrawString("لا توجد فواتير ضمن الفترة المحددة.", normal, muted, new RectangleF(Left, top + hh + 24, CW, rh), fmt);
                    for (int ri = 0; ri < count; ri++)
                    {
                        int y = top + hh + ri * rh;
                        var row = rows[start + ri];
                        if (ri % 2 == 1) using (var alt = new SolidBrush(Color.FromArgb(247, 251, 248))) g.FillRectangle(alt, Left, y, CW, rh);
                        string[] v = { Safe(row.AcademicYearName), Safe(row.StudentName), Safe(row.InvoiceNumber), row.InvoiceDate.ToString("yyyy-MM-dd"),
                            row.TotalAmount.ToString("N2", Culture), row.DiscountAmount.ToString("N2", Culture), row.NetAmount.ToString("N2", Culture),
                            row.TotalPaid.ToString("N2", Culture), row.Remaining.ToString("N2", Culture), Safe(row.Status) };
                        int x = Left + CW;
                        for (int i = 0; i < Widths.Length; i++)
                        {
                            x -= Widths[i];
                            g.DrawRectangle(pen, x, y, Widths[i], rh);
                            g.DrawString(v[i], rf, ink, new RectangleF(x + 4, y + 3, Widths[i] - 8, rh - 6), fmt);
                        }
                    }
                    g.DrawString("تاريخ الإنشاء: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "   |   صفحة " + page,
                        small, muted, new RectangleF(Left, H - 48, CW, 24), fmt);
                }
                var encoder = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg");
                using (var ep = new EncoderParameters(1))
                {
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 86L);
                    bmp.Save(ms, encoder, ep);
                }
                return ms.ToArray();
            }
        }

        private static void DrawSummary(Graphics g, FontFamily family, int students, decimal total, decimal discount,
            decimal net, decimal paid, decimal remaining, Pen pen, Brush ink, Brush muted, StringFormat fmt)
        {
            string[] labels = { "الطلاب", "الإجمالي", "الخصومات", "الصافي", "المحصل", "المتبقي" };
            string[] values = { students.ToString("N0", Culture), total.ToString("N2", Culture), discount.ToString("N2", Culture),
                net.ToString("N2", Culture), paid.ToString("N2", Culture), remaining.ToString("N2", Culture) };
            int y = 185, width = CW / labels.Length;
            using (var lf = new Font(family, 16, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var vf = new Font(family, 20, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var bg = new SolidBrush(Color.FromArgb(236, 248, 240)))
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    int x = Left + CW - (i + 1) * width;
                    g.FillRectangle(bg, x, y, width, 100);
                    g.DrawRectangle(pen, x, y, width, 100);
                    g.DrawString(labels[i], lf, muted, new RectangleF(x + 5, y + 12, width - 10, 25), fmt);
                    g.DrawString(values[i] + (i == 0 ? "" : " د.ل"), vf, ink, new RectangleF(x + 5, y + 46, width - 10, 34), fmt);
                }
            }
        }

        private static StringFormat Rtl() { return new StringFormat(StringFormatFlags.DirectionRightToLeft) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter }; }
        private static string Safe(string s) { return String.IsNullOrWhiteSpace(s) ? "—" : s.Trim(); }
        private static byte[] A(string s) { return Encoding.ASCII.GetBytes(s); }
        private static void Put(Stream s, byte[] b) { s.Write(b, 0, b.Length); }

        private static byte[] MakePdf(IList<byte[]> images)
        {
            int objects = 2 + images.Count * 3;
            var offsets = new long[objects + 1];
            using (var output = new MemoryStream())
            {
                Put(output, new byte[] { 37, 80, 68, 70, 45, 49, 46, 52, 10, 37, 226, 227, 207, 211, 10 });
                Obj(output, offsets, 1, A("<</Type/Catalog/Pages 2 0 R>>"));
                var kids = new StringBuilder("<</Type/Pages/Count ").Append(images.Count).Append("/Kids[");
                for (int i = 0; i < images.Count; i++) kids.Append(3 + 3 * i).Append(" 0 R ");
                kids.Append("]>>");
                Obj(output, offsets, 2, A(kids.ToString()));
                for (int i = 0; i < images.Count; i++)
                {
                    int p = 3 + 3 * i, im = p + 1, c = p + 2;
                    Obj(output, offsets, p, A("<</Type/Page/Parent 2 0 R/MediaBox[0 0 842 595]/Resources<</XObject<</Im0 " + im + " 0 R>>>>/Contents " + c + " 0 R>>"));
                    StreamObj(output, offsets, im, A("<</Type/XObject/Subtype/Image/Width " + W + "/Height " + H + "/ColorSpace/DeviceRGB/BitsPerComponent 8/Filter/DCTDecode/Length " + images[i].Length + ">>"), images[i]);
                    byte[] content = A("q 842 0 0 595 0 0 cm /Im0 Do Q");
                    StreamObj(output, offsets, c, A("<</Length " + content.Length + ">>"), content);
                }
                long xref = output.Position;
                Put(output, A("xref\n0 " + (objects + 1) + "\n0000000000 65535 f \n"));
                for (int i = 1; i <= objects; i++) Put(output, A(offsets[i].ToString("D10", CultureInfo.InvariantCulture) + " 00000 n \n"));
                Put(output, A("trailer\n<</Size " + (objects + 1) + "/Root 1 0 R>>\nstartxref\n" + xref + "\n%%EOF"));
                return output.ToArray();
            }
        }
        private static void Obj(Stream s, long[] offsets, int n, byte[] body) { offsets[n] = s.Position; Put(s, A(n + " 0 obj\n")); Put(s, body); Put(s, A("\nendobj\n")); }
        private static void StreamObj(Stream s, long[] offsets, int n, byte[] dict, byte[] data) { offsets[n] = s.Position; Put(s, A(n + " 0 obj\n")); Put(s, dict); Put(s, A("\nstream\n")); Put(s, data); Put(s, A("\nendstream\nendobj\n")); }
    }
}