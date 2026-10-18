using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Payments
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        // ← الحقل الذي يستدعيه الـ ASPX في: <a href="<%= _returnUrl %>">
        protected string _returnUrl = "";

        public string CurrentStage
        {
            get { return Session["Stage"] as string ?? EducationStage.Basic.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                ConfigureReturnUrl();
                LoadPayments();
            }
        }

        // =========================================================
        // ضبط رابط العودة العام (_returnUrl)
        // الأولوية:
        // 1) returnUrl صريح (إن كان محلياً)
        // 2) studentId → StudentFinances.aspx?studentId=X
        // 3) الافتراضي → StudentFinances.aspx
        // =========================================================
        private void ConfigureReturnUrl()
        {
            string stageEnc = Server.UrlEncode(Session["Stage"] as string ?? "Basic");

            // 1) returnUrl صريح
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

            // 2) studentId → StudentFinances.aspx
            int studentId;
            if (int.TryParse(Request.QueryString["studentId"], out studentId) && studentId > 0)
            {
                _returnUrl = ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                           + "?studentId=" + studentId
                           + "&stage=" + stageEnc;
                return;
            }

            // 3) الافتراضي → StudentFinances.aspx
            _returnUrl = ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                       + "?stage=" + stageEnc;
        }

        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!(url.StartsWith("/") || url.StartsWith("~/"))) return false;
            if (url.StartsWith("//") || url.StartsWith("/\\")) return false;
            return true;
        }

        // =========================================================
        // تحميل المدفوعات
        // =========================================================
        private void LoadPayments()
        {
            string search = txtSearch.Text.Trim();
            EducationStage stage = EnumHelper.GetCurrentStage();
            string stageValue = Session["Stage"] as string ?? "";
            lblStage.Text = EnumHelper.GetStageArabic(stageValue);

            IQueryable<Payment> query = db.Payments
                .Include(p => p.Student)
                .Include(p => p.Fee)
                .Where(p => p.Student.Stage == stage);

            // ✅ فلترة تلقائية بـ studentId من الـ QueryString
            int qsStudentId;
            if (int.TryParse(Request.QueryString["studentId"], out qsStudentId) && qsStudentId > 0)
            {
                query = query.Where(p => p.StudentId == qsStudentId);
            }

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Student != null && p.Student.Name.Contains(search));

            var paymentsInDb = query
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new
                {
                    p.StudentId,
                    StudentName = p.Student != null ? p.Student.Name : "",
                    ClassRoomName = p.Student != null && p.Student.ClassRoom != null ? p.Student.ClassRoom.Name : "",
                    FeeName = p.Fee != null ? p.Fee.Name : "",
                    p.Amount,
                    p.PaymentDate,
                    Method = p.Method,
                    p.ReceiptNumber
                }).ToList();

            var payments = paymentsInDb
                .Select(p => new
                {
                    p.StudentId,
                    p.StudentName,
                    p.ClassRoomName,
                    p.FeeName,
                    p.Amount,
                    p.PaymentDate,
                    MethodName = EnumHelper.GetPaymentMethodName(p.Method),
                    p.ReceiptNumber
                }).ToList();

            rptPayments.DataSource = payments;
            rptPayments.DataBind();

            pnlEmpty.Visible = payments.Count == 0;
            pnlTable.Visible = payments.Count > 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadPayments();
        }

        protected string FormatAmount(decimal amount)
        {
            return amount.ToString("N2") + " د.ل";
        }

        protected string RenderMethodBadge(object methodNameObj)
        {
            string name = methodNameObj == null ? "" : methodNameObj.ToString();
            string color = "#6c757d";
            switch (name)
            {
                case "نقدي": color = "#00b14f"; break;
                case "تحويل بنكي": color = "#0d6efd"; break;
                case "شيك": color = "#fd7e14"; break;
                case "بطاقة مصرفية": color = "#6f42c1"; break;
            }
            return "<span class=\"badge px-3 py-2\" style=\"color: " + color + "; background: transparent;\">"
                + HttpUtility.HtmlEncode(name) + "</span>";
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}