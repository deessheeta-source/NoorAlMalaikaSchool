using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace NoorAlMalaikaSchool.Pages.Payments
{
    public partial class Edit : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        // ✅ خاصية عامة يستخدمها الـ ASPX
        public string PageReturnUrl { get; private set; }

        private int PaymentId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        private int StudentId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["studentId"], out id) ? id : 0;
            }
        }

        private Payment _payment;

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();

            // ✅ تهيئة قيمة رابط العودة
            PageReturnUrl = BuildReturnUrl();

            try
            {
                if (PaymentId <= 0)
                {
                    RedirectToList();
                    return;
                }

                if (!IsPostBack)
                {
                    LoadPayment();
                }
            }
            catch (Exception ex)
            {
                HandleException("تعذر تحميل بيانات الدفعة.", ex);
            }
        }

        // =========================================================
        // بناء رابط العودة (StudentFinances.aspx + studentId)
        // =========================================================
        private string BuildReturnUrl()
        {
            string stageEnc = Server.UrlEncode(Session["Stage"] as string ?? "Basic");

            int studentId = StudentId;
            if (studentId <= 0 && PaymentId > 0)
            {
                try
                {
                    studentId = db.Payments
                        .AsNoTracking()
                        .Where(p => p.Id == PaymentId)
                        .Select(p => p.StudentId)
                        .FirstOrDefault();
                }
                catch { }
            }

            if (studentId > 0)
            {
                return ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                       + "?studentId=" + studentId
                       + "&stage=" + stageEnc;
            }

            return ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                   + "?stage=" + stageEnc;
        }

        // =========================================================
        // تحميل بيانات الدفعة
        // =========================================================
        private void LoadPayment()
        {
            _payment = db.Payments
                .Include(p => p.Fee)
                .Include(p => p.Student)
                .Include(p => p.Invoice)
                .FirstOrDefault(p => p.Id == PaymentId);

            if (_payment == null)
            {
                ShowError("الدفعة غير موجودة.");
                pnlEdit.Visible = false;
                return;
            }

            lblReceiptNumber.Text = string.IsNullOrEmpty(_payment.ReceiptNumber)
                ? "—"
                : _payment.ReceiptNumber;

            lblStudentName.Text = _payment.Student != null
                ? _payment.Student.Name
                : "—";

            lblFeeName.Text = _payment.Fee != null
                ? _payment.Fee.Name
                : "—";

            lblCurrentAmount.Text = _payment.Amount.ToString("N2");
            lblCurrentDate.Text = _payment.PaymentDate.ToString("yyyy-MM-dd");
            lblCurrentMethod.Text = EnumHelper.GetPaymentMethodName(_payment.Method);

            pnlSummary.Visible = true;
            pnlEdit.Visible = true;
            btnSave.Visible = true;

            txtAmount.Text = _payment.Amount.ToString("0.##", CultureInfo.InvariantCulture);
            txtPaymentDate.Text = _payment.PaymentDate.ToString("yyyy-MM-dd");
            txtReceiptNumber.Text = _payment.ReceiptNumber ?? "";
            txtNotes.Text = _payment.Notes ?? "";

            if (ddlPaymentMethod.Items.FindByValue(_payment.Method.ToString()) != null)
                ddlPaymentMethod.SelectedValue = _payment.Method.ToString();

            lblMaxAmount.Text = GetMaxEditableAmount().ToString("N2");
        }

        // =========================================================
        // الحد الأقصى المسموح
        // =========================================================
        private decimal GetMaxEditableAmount()
        {
            if (_payment == null) return 0m;

            if (!_payment.InvoiceId.HasValue) return _payment.Amount;

            var invoice = _payment.Invoice;
            if (invoice == null)
            {
                invoice = db.Invoices
                    .AsNoTracking()
                    .FirstOrDefault(i => i.Id == _payment.InvoiceId.Value);
            }

            if (invoice == null) return _payment.Amount;

            decimal paidExceptThis = db.Payments
                .Where(p => p.InvoiceId == invoice.Id && p.Id != _payment.Id)
                .Select(p => (decimal?)p.Amount)
                .Sum() ?? 0m;

            decimal max = invoice.NetAmount - paidExceptThis;
            if (max < 0) max = 0m;

            return max;
        }

        // =========================================================
        // حفظ التعديلات
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ClearMessage();

                if (PaymentId <= 0)
                {
                    ShowError("رقم الدفعة غير صالح.");
                    return;
                }

                var payment = db.Payments
                    .Include(p => p.Invoice)
                    .FirstOrDefault(p => p.Id == PaymentId);

                if (payment == null)
                {
                    ShowError("الدفعة غير موجودة.");
                    return;
                }

                _payment = payment;

                decimal amount;
                if (!decimal.TryParse(txtAmount.Text.Trim(),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out amount) || amount <= 0)
                {
                    ShowError("يرجى إدخال مبلغ صحيح أكبر من صفر.");
                    return;
                }

                decimal maxAmount = GetMaxEditableAmount();
                if (amount > maxAmount)
                {
                    ShowError("المبلغ لا يمكن أن يتجاوز " + maxAmount.ToString("N2") + " د.ل.");
                    return;
                }

                PaymentMethod method;
                if (!Enum.TryParse(ddlPaymentMethod.SelectedValue, true, out method))
                {
                    ShowError("طريقة الدفع غير صحيحة.");
                    return;
                }

                DateTime paymentDate;
                if (!DateTime.TryParse(txtPaymentDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out paymentDate))
                {
                    ShowError("تاريخ الدفع غير صحيح.");
                    return;
                }

                string receipt = txtReceiptNumber.Text == null
                    ? ""
                    : txtReceiptNumber.Text.Trim();

                if (string.IsNullOrWhiteSpace(receipt))
                    receipt = payment.ReceiptNumber ?? "";

                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        payment.Amount = amount;
                        payment.Method = method;
                        payment.PaymentDate = paymentDate;
                        payment.ReceiptNumber = receipt;
                        payment.Notes = txtNotes.Text == null ? "" : txtNotes.Text.Trim();

                        if (payment.InvoiceId.HasValue)
                        {
                            var invoice = db.Invoices
                                .FirstOrDefault(i => i.Id == payment.InvoiceId.Value);

                            if (invoice != null)
                            {
                                decimal totalPaid = db.Payments
                                    .Where(p => p.InvoiceId == invoice.Id && p.Id != payment.Id)
                                    .Select(p => (decimal?)p.Amount)
                                    .Sum() ?? 0m;

                                totalPaid += amount;

                                invoice.PaidAmount = Math.Min(invoice.NetAmount, Math.Max(0m, totalPaid));

                                if (invoice.PaidAmount <= 0)
                                    invoice.Status = InvoiceStatus.Issued;
                                else if (invoice.PaidAmount >= invoice.NetAmount)
                                    invoice.Status = InvoiceStatus.Paid;
                                else
                                    invoice.Status = InvoiceStatus.PartiallyPaid;
                            }
                        }

                        db.SaveChanges();
                        tx.Commit();

                        RedirectToList("تم حفظ التعديلات بنجاح.");
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException("تعذر حفظ تعديلات الدفعة.", ex);
            }
        }

        // =========================================================
        // دوال مساعدة
        // =========================================================
        private void RedirectToList(string successMessage = "")
        {
            int studentId = StudentId;
            if (studentId <= 0 && _payment != null)
                studentId = _payment.StudentId;

            string stageEnc = Server.UrlEncode(Session["Stage"] as string ?? "Basic");

            string url = ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                       + "?studentId=" + studentId
                       + "&stage=" + stageEnc;

            if (!string.IsNullOrEmpty(successMessage))
                url += "&msg=" + Server.UrlEncode(successMessage);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowError(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = "ep-message danger";
        }

        private void ClearMessage()
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "ep-message";
        }

        private void HandleException(string friendlyMessage, Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Payments.Edit Error: " + ex);

            string message = GetInnerExceptionMessage(ex);
            if (string.IsNullOrWhiteSpace(message))
            {
                ShowError(friendlyMessage);
                return;
            }

            ShowError(friendlyMessage + " التفاصيل: " + message);
        }

        private string GetInnerExceptionMessage(Exception ex)
        {
            Exception current = ex;
            while (current.InnerException != null)
                current = current.InnerException;

            string message = current.Message;
            if (string.IsNullOrWhiteSpace(message)) return "";
            if (message.Length > 700) message = message.Substring(0, 700);
            return message;
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}
