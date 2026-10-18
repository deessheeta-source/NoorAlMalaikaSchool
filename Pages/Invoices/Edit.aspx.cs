using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Data.Services;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Invoices
{
    public partial class Edit : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        public string PageReturnUrl { get; private set; }

        // ✅ Manual discount input
        private sealed class ManualDiscountInput
        {
            public DiscountPolicy Policy { get; set; }
            public string TextValue { get; set; }
        }

        private int InvoiceId
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

        private Invoice _invoice;

        // =========================================================
        // Page Load
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            PageReturnUrl = BuildReturnUrl();

            try
            {
                if (InvoiceId <= 0)
                {
                    RedirectToList();
                    return;
                }

                if (!IsPostBack)
                {
                    LoadInvoice();
                }
            }
            catch (Exception ex)
            {
                HandleException("تعذر تحميل بيانات الفاتورة.", ex);
            }
        }

        // =========================================================
        // رابط العودة
        // =========================================================
        private string BuildReturnUrl()
        {
            string stageEnc = Server.UrlEncode(Session["Stage"] as string ?? "Basic");

            int studentId = StudentId;
            if (studentId <= 0 && InvoiceId > 0)
            {
                try
                {
                    studentId = db.Invoices.AsNoTracking()
                        .Where(i => i.Id == InvoiceId)
                        .Select(i => i.StudentId)
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
        // تحميل الفاتورة
        // =========================================================
        private void LoadInvoice()
        {
            _invoice = db.Invoices
                .Include(i => i.Student.ClassRoom.GradeLevel)
                .Include(i => i.Lines)
                .Include(i => i.Discounts)
                .Include(i => i.AcademicYear)
                .FirstOrDefault(i => i.Id == InvoiceId);

            if (_invoice == null)
            {
                ShowError("الفاتورة غير موجودة.");
                pnlEdit.Visible = false;
                return;
            }

            // === عرض البيانات المرجعية ===
            lblHeroInvoiceNo.Text = string.IsNullOrEmpty(_invoice.InvoiceNumber)
                ? "—"
                : _invoice.InvoiceNumber;

            lblStudentName.Text = _invoice.Student != null ? _invoice.Student.Name : "—";
            lblClassName.Text =
                _invoice.Student != null
                && _invoice.Student.ClassRoom != null
                && _invoice.Student.ClassRoom.GradeLevel != null
                    ? _invoice.Student.ClassRoom.GradeLevel.Name
                    : "—";

            lblCurrentDate.Text = _invoice.InvoiceDate.ToString("yyyy-MM-dd");
            lblCurrentTotal.Text = _invoice.TotalAmount.ToString("N2");
            lblCurrentDiscount.Text = _invoice.DiscountAmount.ToString("N2");
            lblCurrentNet.Text = _invoice.NetAmount.ToString("N2");

            // === تحذير الدفعات ===
            bool hasPayments = db.Payments.Any(p => p.InvoiceId == _invoice.Id);
            pnlPaymentWarning.Visible = hasPayments;

            pnlSummary.Visible = true;
            pnlEdit.Visible = true;
            btnSave.Visible = true;

            // === تعبئة الحقول ===
            txtInvoiceDate.Text = _invoice.InvoiceDate.ToString("yyyy-MM-dd");
            txtNotes.Text = _invoice.Notes ?? "";

            // === بنود الفاتورة ===
            var lineRows = _invoice.Lines
                .Select(l => new
                {
                    LineId = l.Id,
                    Description = l.Description ?? "—",
                    Amount = l.Amount
                })
                .ToList();

            gvLines.DataSource = lineRows;
            gvLines.DataBind();

            // === تحميل سياسات الخصم ===
            LoadDiscountPolicies(_invoice);

            // === عرض الملخص المبدئي ===
            UpdateLiveSummary();
        }

        // =========================================================
        // تحميل سياسات الخصم
        // =========================================================
        private void LoadDiscountPolicies(Invoice invoice)
        {
            var policies = db.DiscountPolicies
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ToList();

            cblDiscounts.Items.Clear();
            lblNoPolicies.Visible = policies.Count == 0;

            var siblingPolicy = GetSiblingDiscountPolicy();
            var earlyBirdPolicy = GetEarlyBirdDiscountPolicy();
            var annualSubPolicy = GetAnnualSubscriptionDiscountPolicy();

            // ✅ السياسات المُطبَّقة حالياً على الفاتورة
            var appliedDiscounts = invoice.Discounts != null
                ? invoice.Discounts.ToList()
                : new List<InvoiceDiscount>();

            foreach (var p in policies)
            {
                bool isSibling = siblingPolicy != null && p.Id == siblingPolicy.Id;
                bool isEarlyBird = earlyBirdPolicy != null && p.Id == earlyBirdPolicy.Id;
                bool isAnnualSub = annualSubPolicy != null && p.Id == annualSubPolicy.Id;

                string label = p.Name;

                if (isSibling || isEarlyBird || isAnnualSub || p.Value <= 0m)
                    label += " (يُدخَل يدوياً)";
                else if (p.Kind == DiscountKind.Percentage)
                    label += " (" + p.Value.ToString("N0") + "%)";
                else
                    label += " (" + p.Value.ToString("N2") + " د.ل)";

                ListItem item = new ListItem(label, p.Id.ToString());

                // ✅ تحديد إذا كانت مُطبَّقة على هذه الفاتورة
                var applied = appliedDiscounts.FirstOrDefault(d => d.DiscountPolicyId == p.Id);
                if (applied != null)
                {
                    item.Selected = true;

                    // ✅ تعبئة الحقل اليدوي إن كان هذا النوع يدوياً
                    if (isSibling)
                        txtSiblingDiscount.Text = applied.DiscountAmount.ToString("0.##", CultureInfo.InvariantCulture);
                    else if (isEarlyBird)
                        txtEarlyBirdDiscount.Text = applied.DiscountAmount.ToString("0.##", CultureInfo.InvariantCulture);
                    else if (isAnnualSub)
                        txtAnnualSubscriptionDiscount.Text = applied.DiscountAmount.ToString("0.##", CultureInfo.InvariantCulture);
                }

                cblDiscounts.Items.Add(item);
            }

            hfSiblingPolicyId.Value = siblingPolicy != null ? siblingPolicy.Id.ToString() : "";
            hfEarlyBirdPolicyId.Value = earlyBirdPolicy != null ? earlyBirdPolicy.Id.ToString() : "";
            hfAnnualSubscriptionPolicyId.Value = annualSubPolicy != null ? annualSubPolicy.Id.ToString() : "";
        }

        // =========================================================
        // إيجاد سياسة حسب الاسم
        // =========================================================
        private DiscountPolicy FindDiscountPolicyByName(params string[] keywords)
        {
            if (keywords == null || keywords.Length == 0) return null;

            var policies = db.DiscountPolicies
                .AsNoTracking()
                .Where(p => p.IsActive)
                .ToList();

            return policies.FirstOrDefault(p =>
            {
                string name = (p.Name ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name)) return false;

                return keywords.Any(k =>
                    !string.IsNullOrWhiteSpace(k) &&
                    name.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0);
            });
        }

        private DiscountPolicy GetSiblingDiscountPolicy()
            => FindDiscountPolicyByName("إخوة", "اخوة", "أخوة", "sibling");

        private DiscountPolicy GetEarlyBirdDiscountPolicy()
            => FindDiscountPolicyByName("تسجيل المبكر", "تسجيل مبكر", "earlybird", "early");

        private DiscountPolicy GetAnnualSubscriptionDiscountPolicy()
            => FindDiscountPolicyByName("الاشتراك السنوي", "اشتراك سنوي", "annualsubscription", "annual");

        // =========================================================
        // الحصول على مدخلات الخصومات اليدوية
        // =========================================================
        private List<ManualDiscountInput> GetManualDiscountInputs()
        {
            return new List<ManualDiscountInput>
            {
                new ManualDiscountInput { Policy = GetSiblingDiscountPolicy(), TextValue = txtSiblingDiscount.Text },
                new ManualDiscountInput { Policy = GetEarlyBirdDiscountPolicy(), TextValue = txtEarlyBirdDiscount.Text },
                new ManualDiscountInput { Policy = GetAnnualSubscriptionDiscountPolicy(), TextValue = txtAnnualSubscriptionDiscount.Text }
            };
        }

        // =========================================================
        // حساب الملخص بناءً على الحقول الحالية (بدون حفظ)
        // =========================================================
        private void UpdateLiveSummary()
        {
            decimal total = 0m;
            foreach (GridViewRow row in gvLines.Rows)
            {
                var txt = (TextBox)row.FindControl("txtLineAmount");
                if (txt == null) continue;
                decimal v;
                if (decimal.TryParse(txt.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out v))
                    total += v;
            }

            decimal discountTotal = 0m;

            // ✅ الخصومات التلقائية من السياسات المُختارة (نسبة/مبلغ ثابت)
            var selectedPolicyIds = cblDiscounts.Items.Cast<ListItem>()
                .Where(i => i.Selected)
                .Select(i => { int id; return int.TryParse(i.Value, out id) ? id : 0; })
                .Where(id => id > 0)
                .ToList();

            var manualPolicies = new[] {
                GetSiblingDiscountPolicy(),
                GetEarlyBirdDiscountPolicy(),
                GetAnnualSubscriptionDiscountPolicy()
            }.Where(p => p != null).Select(p => p.Id).ToHashSet();

            foreach (var id in selectedPolicyIds)
            {
                if (manualPolicies.Contains(id)) continue;

                var policy = db.DiscountPolicies.AsNoTracking().FirstOrDefault(p => p.Id == id);
                if (policy == null) continue;

                if (policy.Kind == DiscountKind.Percentage)
                    discountTotal += total * (policy.Value / 100m);
                else
                    discountTotal += policy.Value;
            }

            // ✅ الخصومات اليدوية
            foreach (var input in GetManualDiscountInputs())
            {
                if (input.Policy == null) continue;
                if (!selectedPolicyIds.Contains(input.Policy.Id)) continue;

                decimal v;
                if (decimal.TryParse((input.TextValue ?? "").Trim(),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out v) && v > 0)
                {
                    discountTotal += v;
                }
            }

            decimal net = Math.Max(0m, total - discountTotal);

            lblLiveTotal.Text = total.ToString("N2");
            lblLiveDiscount.Text = discountTotal.ToString("N2");
            lblLiveNet.Text = net.ToString("N2");
        }

        // =========================================================
        // زر إعادة حساب
        // =========================================================
        protected void btnRecalc_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateLiveSummary();
                ShowSuccess("تم إعادة حساب الملخص.");
            }
            catch (Exception ex)
            {
                HandleException("تعذر إعادة الحساب.", ex);
            }
        }

        // =========================================================
        // حفظ التعديلات
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ClearMessage();

                if (InvoiceId <= 0)
                {
                    ShowError("رقم الفاتورة غير صالح.");
                    return;
                }

                var invoice = db.Invoices
                    .Include(i => i.Lines)
                    .Include(i => i.Discounts)
                    .FirstOrDefault(i => i.Id == InvoiceId);

                if (invoice == null)
                {
                    ShowError("الفاتورة غير موجودة.");
                    return;
                }

                _invoice = invoice;

                // ---------- التحقق من التاريخ ----------
                DateTime invoiceDate;
                if (!DateTime.TryParse(txtInvoiceDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out invoiceDate))
                {
                    ShowError("تاريخ الفاتورة غير صحيح.");
                    return;
                }

                // ---------- التحقق من الدفعات ----------
                bool hasPayments = db.Payments.Any(p => p.InvoiceId == invoice.Id);

                if (hasPayments)
                {
                    var oldestPayment = db.Payments
                        .Where(p => p.InvoiceId == invoice.Id)
                        .OrderBy(p => p.PaymentDate)
                        .Select(p => (DateTime?)p.PaymentDate)
                        .FirstOrDefault();

                    if (oldestPayment.HasValue && invoiceDate > oldestPayment.Value)
                    {
                        ShowError("لا يمكن تغيير تاريخ الفاتورة إلى تاريخ لاحق لتاريخ الدفعات المسجلة ("
                            + oldestPayment.Value.ToString("yyyy-MM-dd") + ").");
                        return;
                    }
                }

                // ---------- بنود الفاتورة ----------
                decimal newTotal = 0m;
                var lineUpdates = new List<Tuple<InvoiceLine, decimal>>();

                foreach (GridViewRow row in gvLines.Rows)
                {
                    int lineId;
                    if (!int.TryParse(gvLines.DataKeys[row.RowIndex].Value.ToString(), out lineId))
                        continue;

                    var line = invoice.Lines.FirstOrDefault(l => l.Id == lineId);
                    if (line == null) continue;

                    var txtLineAmount = (TextBox)row.FindControl("txtLineAmount");
                    if (txtLineAmount == null) continue;

                    decimal newAmount;
                    if (!decimal.TryParse(txtLineAmount.Text.Trim(),
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out newAmount) || newAmount < 0)
                    {
                        ShowError("المبلغ في أحد البنود غير صحيح.");
                        return;
                    }

                    lineUpdates.Add(Tuple.Create(line, newAmount));
                    newTotal += newAmount;
                }

                // ---------- حساب الخصومات الجديدة ----------
                var selectedPolicyIds = cblDiscounts.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => { int id; return int.TryParse(i.Value, out id) ? id : 0; })
                    .Where(id => id > 0)
                    .ToList();

                var manualInputs = GetManualDiscountInputs();
                var manualApplied = new List<InvoiceDiscount>();

                foreach (var input in manualInputs)
                {
                    if (input.Policy == null) continue;
                    if (!selectedPolicyIds.Contains(input.Policy.Id)) continue;

                    decimal v;
                    if (decimal.TryParse((input.TextValue ?? "").Trim(),
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out v) && v > 0)
                    {
                        selectedPolicyIds.Remove(input.Policy.Id);
                        manualApplied.Add(new InvoiceDiscount
                        {
                            DiscountPolicyId = input.Policy.Id,
                            Name = input.Policy.Name,
                            Type = input.Policy.Type,
                            Value = v,
                            DiscountAmount = v,
                            AppliedToFeeType = input.Policy.AppliesToFeeType
                        });
                    }
                }

                // ✅ استخدام StudentFinancialService لحساب الخصومات التلقائية
                int academicYearId = invoice.AcademicYearId;
                var student = db.Students.Include(s => s.ClassRoom)
                    .FirstOrDefault(s => s.Id == invoice.StudentId);

                // ✅ إعادة بناء feeLines من البنود المُعدَّلة
                var feeLines = new List<StudentFinancialService.FeeAmount>();
                foreach (var upd in lineUpdates)
                {
                    var feeIdNullable = upd.Item1.FeeId;
                    if (!feeIdNullable.HasValue) continue;

                    var fee = db.Fees.Find(feeIdNullable.Value);
                    if (fee == null) continue;

                    feeLines.Add(new StudentFinancialService.FeeAmount
                    {
                        Fee = fee,
                        Amount = upd.Item2
                    });
                }

                StudentFinancialService.BenefitCalculation calc = null;
                if (student != null && feeLines.Count > 0)
                {
                    calc = StudentFinancialService.Calculate(
                        db, student, academicYearId, feeLines, selectedPolicyIds, invoiceDate);
                }

                // ✅ تجميع كل الخصومات (تلقائية + يدوية)
                var allDiscounts = new List<InvoiceDiscount>();

                if (calc != null && calc.Adjustments != null)
                    allDiscounts.AddRange(calc.Adjustments);

                allDiscounts.AddRange(manualApplied);

                decimal newDiscountTotal = allDiscounts.Sum(d => d.DiscountAmount);
                decimal newNet = Math.Max(0m, newTotal - newDiscountTotal);

                // ---------- التحقق: المجموع لا يقل عن المدفوعات ----------
                decimal paidTotal = db.Payments
                    .Where(p => p.InvoiceId == invoice.Id)
                    .Select(p => (decimal?)p.Amount)
                    .Sum() ?? 0m;

                if (newNet < paidTotal)
                {
                    ShowError("الصافي الجديد (" + newNet.ToString("N2") +
                        " د.ل) أقل من إجمالي المدفوعات المسجلة (" +
                        paidTotal.ToString("N2") + " د.ل). لا يمكن الحفظ.");
                    return;
                }

                // ---------- تطبيق التعديلات ----------
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        // بنود الفاتورة
                        foreach (var upd in lineUpdates)
                            upd.Item1.Amount = upd.Item2;

                        // إزالة الخصومات القديمة
                        if (invoice.Discounts != null && invoice.Discounts.Any())
                            db.InvoiceDiscounts.RemoveRange(invoice.Discounts.ToList());

                        // إضافة الخصومات الجديدة
                        foreach (var d in allDiscounts)
                        {
                            d.InvoiceId = invoice.Id;
                            invoice.Discounts.Add(d);
                        }

                        // التحديثات
                        invoice.TotalAmount = newTotal;
                        invoice.DiscountAmount = newDiscountTotal;
                        invoice.NetAmount = newNet;
                        invoice.PaidAmount = Math.Min(newNet, paidTotal);

                        if (newNet <= 0)
                            invoice.Status = InvoiceStatus.Paid;
                        else if (paidTotal <= 0)
                            invoice.Status = InvoiceStatus.Issued;
                        else if (paidTotal >= newNet)
                            invoice.Status = InvoiceStatus.Paid;
                        else
                            invoice.Status = InvoiceStatus.PartiallyPaid;

                        invoice.InvoiceDate = invoiceDate;
                        invoice.Notes = txtNotes.Text == null ? "" : txtNotes.Text.Trim();

                        db.SaveChanges();
                        tx.Commit();

                        // ✅ إعادة التوجيه إلى صفحة الطالب مع رسالة النجاح
                        RedirectToList("تم حفظ التعديلات بنجاح.");
                        return;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException("تعذر حفظ تعديلات الفاتورة.", ex);
            }
        }

        // =========================================================
        // دوال مساعدة
        // =========================================================
        private void RedirectToList(string successMessage = "")
        {
            int studentId = StudentId;
            if (studentId <= 0 && _invoice != null)
                studentId = _invoice.StudentId;

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
            lblMessage.CssClass = "inv-message danger";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = "inv-message success";
        }

        private void ClearMessage()
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "inv-message";
        }

        private void HandleException(string friendlyMessage, Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Invoices.Edit Error: " + ex);

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
