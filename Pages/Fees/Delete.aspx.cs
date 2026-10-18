using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Fees
{
    public partial class Delete : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();
        private int feeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!int.TryParse(Request.QueryString["id"], out feeId))
            {
                pnlNotFound.Visible = true;
                pnlForm.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                LoadFee();
            }
        }

        private void LoadFee()
        {
            var fee = db.Fees.FirstOrDefault(f => f.Id == feeId);
            if (fee == null)
            {
                pnlNotFound.Visible = true;
                pnlForm.Visible = false;
                return;
            }

            pnlNotFound.Visible = false;
            pnlForm.Visible = true;

            lblName.Text = fee.Name;
            lblAmount.Text = fee.Amount.ToString("N2");
            lblClassRoom.Text = fee.ClassRoom != null ? fee.ClassRoom.Name : "";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var fee = db.Fees.FirstOrDefault(f => f.Id == feeId);
            if (fee == null) return;

            var hasPayments = db.Payments.Any(p => p.FeeId == feeId);
            var hasInvoiceLines = db.InvoiceLines.Any(l => l.FeeId == feeId);
            if (hasPayments || hasInvoiceLines)
            {
                lblMessage.Text = hasPayments
                    ? "لا يمكن حذف هذا الرسم لارتباطه بمدفوعات وفواتير سابقة."
                    : "لا يمكن حذف هذا الرسم لارتباطه ببنود فواتير سابقة.";
                lblMessage.CssClass = "text-danger mb-3 d-block";
                return;
            }

            db.Fees.Remove(fee);
            db.SaveChanges();
            Response.Redirect("Default.aspx?stage=" + (Session["Stage"] ?? ""));
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}
