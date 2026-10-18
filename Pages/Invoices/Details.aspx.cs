using System;
using System.Data.Entity;
using System.Linq;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Invoices
{
    public partial class Details : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        public Invoice Invoice { get; private set; }

        // ✅ الحقل الذي يستخدمه الـ ASPX في: <a href="<%= _returnUrl %>">
        protected string _returnUrl = "";

        private int InvoiceId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            // ✅ الإصلاح: نادِ ConfigureReturnUrl دائماً (حتى بعد PostBack)
            ConfigureReturnUrl();

            if (!IsPostBack)
            {
                LoadInvoice();
            }
        }

        // =========================================================
        // ✅ ضبط رابط العودة العام (_returnUrl)
        //    الأولوية: studentId (رابط / فاتورة / جلسة) → StudentFinances.aspx
        //    ثم: returnUrl صريح
        //    ثم: StudentFinances.aspx العامة
        // =========================================================
        private void ConfigureReturnUrl()
        {
            string stageEnc = Server.UrlEncode(Session["Stage"] as string ?? "Basic");

            // ✅ الأولوية القصوى: studentId من أي مصدر → صفحة الطالب
            int studentId = GetStudentIdForReturn();
            if (studentId > 0)
            {
                _returnUrl = ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                           + "?studentId=" + studentId
                           + "&stage=" + stageEnc;
                return;
            }

            // fallback 1: returnUrl صريح
            string returnUrl = Request.QueryString["returnUrl"];
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                string decoded = Server.UrlDecode(returnUrl);
                if (IsLocalUrl(decoded))
                {
                    _returnUrl = decoded;
                    return;
                }
            }

            // fallback 2: صفحة الطالب العامة
            _returnUrl = ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                       + "?stage=" + stageEnc;
        }

        // =========================================================
        // ✅ استخراج studentId من عدة مصادر
        // =========================================================
        private int GetStudentIdForReturn()
        {
            // 1) من الـ Query String
            int studentId;
            if (int.TryParse(Request.QueryString["studentId"], out studentId) && studentId > 0)
                return studentId;

            // 2) من الفاتورة نفسها (الأكثر موثوقية)
            if (InvoiceId > 0)
            {
                try
                {
                    int fromInvoice = db.Invoices
                        .AsNoTracking()
                        .Where(i => i.Id == InvoiceId)
                        .Select(i => i.StudentId)
                        .FirstOrDefault();

                    if (fromInvoice > 0) return fromInvoice;
                }
                catch { /* تجاهل */ }
            }

            // 3) من الـ Session
            object sessionStudentId = Session["CurrentStudentId"];
            int sessionId;
            if (sessionStudentId != null
                && int.TryParse(sessionStudentId.ToString(), out sessionId)
                && sessionId > 0)
            {
                return sessionId;
            }

            return 0;
        }

        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!(url.StartsWith("/") || url.StartsWith("~/"))) return false;
            if (url.StartsWith("//") || url.StartsWith("/\\")) return false;
            return true;
        }

        // =========================================================
        // تحميل الفاتورة
        // =========================================================
        private void LoadInvoice()
        {
            Invoice = db.Invoices
                .Include(i => i.Student.ClassRoom)
                .Include(i => i.Student.ClassRoom.GradeLevel)
                .Include(i => i.Student.Parent)
                .Include(i => i.AcademicYear)
                .Include(i => i.Lines)
                .Include(i => i.Discounts)
                .FirstOrDefault(i => i.Id == InvoiceId);

            if (Invoice == null)
            {
                Response.Redirect(_returnUrl);
                return;
            }

            // =====================================================
            // بيانات المدرسة
            // =====================================================
            lblInvoiceNumber.Text = Invoice.InvoiceNumber;
            lblDate.Text = Invoice.InvoiceDate.ToString("yyyy-MM-dd");
            lblYear.Text = Invoice.AcademicYear != null ? Invoice.AcademicYear.Name : "";

            // =====================================================
            // بيانات الطالب
            // =====================================================
            lblStudent.Text = Invoice.Student != null ? Invoice.Student.Name : "";

            lblClass.Text =
                Invoice.Student != null
                && Invoice.Student.ClassRoom != null
                && Invoice.Student.ClassRoom.GradeLevel != null
                    ? Invoice.Student.ClassRoom.GradeLevel.Name
                    : (Invoice.Student != null && Invoice.Student.ClassRoom != null
                        ? Invoice.Student.ClassRoom.Name
                        : "");

            // ---------- القسم ----------
            string section = Invoice.Student != null ? Invoice.Student.Section : null;

            if (string.IsNullOrWhiteSpace(section)
                && Invoice.Student != null
                && Invoice.Student.ClassRoom != null)
            {
                section = Invoice.Student.ClassRoom.SectionName;
            }

            if (string.IsNullOrWhiteSpace(section))
                lblSection.Text = "—";
            else if (section == "علمي")
                lblSection.Text = "القسم العلمي";
            else if (section == "أدبي")
                lblSection.Text = "القسم الأدبي";
            else
                lblSection.Text = section;

            // ---------- الشعبة ----------
            string division = Invoice.Student != null
                              && Invoice.Student.ClassRoom != null
                                ? Invoice.Student.ClassRoom.DivisionName
                                : null;

            lblDivision.Text = string.IsNullOrWhiteSpace(division)
                ? "—"
                : "شعبة " + division.Trim();

            // ---------- ولي الأمر ----------
            lblParent.Text =
                Invoice.Student != null && Invoice.Student.Parent != null
                    ? Invoice.Student.Parent.Name
                    : "—";

            // =====================================================
            // بنود الفاتورة
            // =====================================================
            var lines = Invoice.Lines
                .Select(l => new
                {
                    l.Description,
                    TypeText = l.IsManual ? "يدوي" : "رسوم",
                    l.Amount
                }).ToList();

            gvLines.DataSource = lines;
            gvLines.DataBind();

            // =====================================================
            // الخصومات والإعفاءات
            // =====================================================
            var discounts = Invoice.Discounts
                .Select(d => new
                {
                    d.Name,
                    ValueText = d.Type.GetDisplayName() + " (" + d.Value.ToString("N2") + ")",
                    d.DiscountAmount
                }).ToList();

            gvDiscounts.DataSource = discounts;
            gvDiscounts.DataBind();

            // =====================================================
            // المدفوعات
            // =====================================================
            decimal actualPaid = db.Payments
                .Where(p => p.InvoiceId == Invoice.Id)
                .Select(p => (decimal?)p.Amount)
                .Sum() ?? 0m;

            Invoice.PaidAmount = Math.Min(Invoice.NetAmount, Math.Max(0m, actualPaid));

            // =====================================================
            // الملخص
            // =====================================================
            lblTotal.Text = Invoice.TotalAmount.ToString("N2") + " د.ل";
            lblDiscount.Text = Invoice.DiscountAmount.ToString("N2") + " د.ل";
            lblNet.Text = Invoice.NetAmount.ToString("N2") + " د.ل";
            lblPaid.Text = Invoice.PaidAmount.ToString("N2") + " د.ل";

            decimal remaining = Invoice.NetAmount - Invoice.PaidAmount;
            if (remaining < 0) remaining = 0;
            lblRemaining.Text = remaining.ToString("N2") + " د.ل";

            // =====================================================
            // ✅ رقم الفاتورة في الهيرو
            // =====================================================
            lblHeroInvoiceNo.Text =
                string.IsNullOrWhiteSpace(Invoice.InvoiceNumber)
                    ? "—"
                    : Invoice.InvoiceNumber;
        }

        // =========================================================
        // ✅ إرجاع كلاس نوع البند (يدوي / رسوم)
        // =========================================================
        protected string GetLineTypeCss(object typeText)
        {
            string value = Convert.ToString(typeText).Trim();

            if (value == "يدوي")
                return "id-type-badge manual";

            return "id-type-badge fee";
        }

        // =========================================================
        // حالة الفاتورة
        // =========================================================
        protected string GetStatusText()
        {
            return Invoice != null ? Invoice.Status.GetDisplayName() : "";
        }

        protected string GetStatusBadge()
        {
            if (Invoice == null) return "id-status-badge draft";

            switch (Invoice.Status)
            {
                case InvoiceStatus.Paid: return "id-status-badge paid";
                case InvoiceStatus.PartiallyPaid: return "id-status-badge partial";
                case InvoiceStatus.Issued: return "id-status-badge issued";
                case InvoiceStatus.Draft: return "id-status-badge draft";
                case InvoiceStatus.Cancelled: return "id-status-badge cancelled";
                default: return "id-status-badge draft";
            }
        }

        // =========================================================
        // التخلص من DbContext
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}