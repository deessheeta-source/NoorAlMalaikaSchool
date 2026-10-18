using System;
using System.Linq;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.DiscountPolicies
{
    public partial class Delete : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private int PolicyId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadPolicy();
            }
        }

        // =========================================================
        // تحميل بيانات السياسة
        // =========================================================
        private void LoadPolicy()
        {
            var policy = db.DiscountPolicies.Find(PolicyId);

            if (policy == null)
            {
                lblMessage.Text = "لم يتم العثور على السياسة";
                lblMessage.CssClass = "dp-message danger";
                pnlDetails.Visible = false;
                return;
            }

            lblName.Text = policy.Name;
            lblType.Text = policy.Type.GetDisplayName();

            lblValue.Text = policy.Kind == DiscountKind.Percentage
                ? policy.Value.ToString("N0") + "%"
                : policy.Value.ToString("N2") + " د.ل";

            lblDescription.Text = string.IsNullOrEmpty(policy.Description)
                ? "—"
                : policy.Description;

            // عدد مرات الاستخدام
            int usageCount = db.InvoiceDiscounts
                .Count(d => d.DiscountPolicyId == policy.Id);

            if (usageCount > 0)
            {
                lblWarningUsages.Text =
                    $" (تم استخدام هذه السياسة في {usageCount} بند خصم)";
            }
        }

        // =========================================================
        // ✅ حذف السياسة فعلياً
        // =========================================================
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var policy = db.DiscountPolicies.Find(PolicyId);

            if (policy == null)
            {
                lblMessage.Text = "لم يتم العثور على السياسة";
                lblMessage.CssClass = "dp-message danger";
                return;
            }

            // =====================================================
            // ✅ التحقق من الارتباطات قبل الحذف
            // =====================================================
            bool usedInInvoices = db.InvoiceDiscounts
                .Any(d => d.DiscountPolicyId == policy.Id);

            bool usedInStudentDiscounts = db.StudentDiscounts
                .Any(d => d.DiscountPolicyId == policy.Id);

            if (usedInInvoices || usedInStudentDiscounts)
            {
                lblMessage.Text =
                    "لا يمكن حذف هذه السياسة لأنها مستخدمة في فواتير أو خصومات طلاب. " +
                    "يرجى تعطيلها بدلاً من ذلك للحفاظ على سلامة السجل المالي.";
                lblMessage.CssClass = "dp-message danger";
                return;
            }

            try
            {
                // =================================================
                // ✅ الحذف الفعلي من قاعدة البيانات
                // =================================================
                db.DiscountPolicies.Remove(policy);
                db.SaveChanges();

                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;

                lblMessage.Text = "تعذر حذف السياسة: " +
                    Server.HtmlEncode(inner.Message);
                lblMessage.CssClass = "dp-message danger";
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}