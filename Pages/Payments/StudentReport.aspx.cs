using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Data.Services;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Payments
{
    public partial class StudentReport : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private Student _student;

        // ✅ إجمالي المبالغ (Gross)
        private decimal _totalGrossDue;

        // ✅ إجمالي الخصومات
        private decimal _totalDiscount;

        // ✅ إجمالي المستحق بعد الخصم (Net)
        private decimal _totalDue;

        // ✅ إجمالي المدفوع
        private decimal _totalSubscriptionPaid;

        private int _subscriptionFeeId;
        private EducationStage _stage;

        public string CurrentStage
        {
            get { return _stage.ToString(); }
        }

        protected int StudentId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["studentId"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();

            // دور View يستطيع قراءة تقرير الطالب، لكنه لا يستطيع تسجيل دفعات.
            if (SecurityHelper.CurrentRole == UserRole.View)
                paymentModal.Visible = false;

            _stage = EnumHelper.GetCurrentStage();

            lblStage.Text =
                EnumHelper.GetStageArabic(
                    Session["Stage"] as string ??
                    _stage.ToString());

            if (StudentId == 0)
            {
                Response.Redirect(
                    "Default.aspx?stage=" +
                    Uri.EscapeDataString(
                        _stage.ToString()));

                return;
            }

            _student = db.Students
                .Include(s => s.ClassRoom.GradeLevel)
                .FirstOrDefault(s =>
                    s.Id == StudentId &&
                    s.IsActive);

            if (_student == null)
            {
                Response.Redirect(
                    "Default.aspx?stage=" +
                    Uri.EscapeDataString(
                        _stage.ToString()));

                return;
            }

            ApplyMessage();
            LoadReport();
        }

        // =========================================================
        // الرسائل
        // =========================================================
        private void ApplyMessage()
        {
            string msg = Request.QueryString["msg"];
            string err = Request.QueryString["err"];

            if (!string.IsNullOrEmpty(err))
            {
                pnlError.Visible = true;
                lblError.Text = Server.HtmlEncode(err);
            }
            else if (!string.IsNullOrEmpty(msg))
            {
                pnlMessage.Visible = true;
                lblMessage.Text = Server.HtmlEncode(msg);
            }
        }

        // =========================================================
        // تحميل تقرير الطالب
        // =========================================================
        private void LoadReport()
        {
            if (_student.ClassRoom == null)
            {
                pnlNoStudent.Visible = true;
                pnlReport.Visible = false;
                return;
            }

            pnlNoStudent.Visible = false;
            pnlReport.Visible = true;

            int yearId = _student.ClassRoom.AcademicYearId;

            // =====================================================
            // تسجيل الطالب في السنة الدراسية
            // =====================================================
            StudentFinancialService.EnsureEnrollment(db, _student, yearId);
            db.SaveChanges();

            // =====================================================
            // الرسوم المطبقة على الطالب
            // =====================================================
            var fees = StudentFinancialService.GetApplicableFees(db, _student, yearId);

            // =====================================================
            // فواتير الطالب
            // =====================================================
            var invoices = db.Invoices
                .Include(i => i.Lines)
                .Where(i =>
                    i.StudentId == StudentId &&
                    i.AcademicYearId == yearId &&
                    i.Status != InvoiceStatus.Cancelled)
                .ToList();

            var lineContexts = StudentFinancialService.LoadLineNetContexts(db, invoices);

            // =====================================================
            // دفعات الطالب
            // =====================================================
            var payments = db.Payments
                .Include(p => p.Fee)
                .Where(p => p.StudentId == StudentId)
                .OrderBy(p => p.PaymentDate)
                .ThenBy(p => p.Id)
                .ToList();

            // =====================================================
            // الاشتراكات السنوية
            // =====================================================
            var subscriptionFees = fees
                .Where(f => f.Type == FeeType.AnnualSubscription)
                .ToList();

            var subscriptionFeeIds = subscriptionFees
                .Select(f => f.Id)
                .ToList();

            var subscriptionRowsSource = new List<Tuple<Fee, decimal, decimal>>();

            foreach (var fee in subscriptionFees)
            {
                var lines = invoices
                    .SelectMany(i => i.Lines
                        .Where(l => l.FeeId.HasValue && l.FeeId.Value == fee.Id)
                        .Select(l => new { Invoice = i, Line = l }))
                    .ToList();

                // ✅ حساب المبلغ الإجمالي (Gross) والمبلغ الصافي (Net)
                decimal grossDue;
                decimal netDue;

                if (lines.Count == 0)
                {
                    grossDue = fee.Amount;
                    netDue = fee.Amount;
                }
                else
                {
                    grossDue = lines.Sum(x => x.Line.Amount);

                    netDue = lines
                        .GroupBy(x => x.Invoice.Id)
                        .Sum(g => g.Sum(x =>
                            StudentFinancialService.GetLineNetAmount(
                                x.Invoice,
                                x.Line,
                                lineContexts[x.Invoice.Id])));
                }

                decimal discount = Math.Max(0m, grossDue - netDue);

                _totalGrossDue += grossDue;
                _totalDiscount += discount;
                _totalDue += netDue;

                decimal paid = payments
                    .Where(p => p.FeeId == fee.Id)
                    .Sum(p => p.Amount);

                subscriptionRowsSource.Add(Tuple.Create(fee, netDue, paid));
            }

            _totalSubscriptionPaid = subscriptionRowsSource.Sum(x => x.Item3);

            // =====================================================
            // تحديد الاشتراك السنوي الذي سيتم التحصيل عليه
            // =====================================================
            var preferred = subscriptionRowsSource
                .FirstOrDefault(x =>
                    x.Item2 - x.Item3 > 0 &&
                    invoices.Any(i => i.Lines.Any(l =>
                        l.FeeId.HasValue && l.FeeId.Value == x.Item1.Id)));

            if (preferred == null)
            {
                preferred = subscriptionRowsSource
                    .FirstOrDefault(x =>
                        invoices.Any(i => i.Lines.Any(l =>
                            l.FeeId.HasValue && l.FeeId.Value == x.Item1.Id)));
            }

            _subscriptionFeeId = preferred == null ? 0 : preferred.Item1.Id;

            // =====================================================
            // التحقق من تعدد الاشتراكات
            // =====================================================
            bool hasSubscriptionInvoice = invoices.Any(i =>
                i.Lines.Any(l =>
                    l.FeeId.HasValue &&
                    subscriptionFeeIds.Contains(l.FeeId.Value)));

            if (subscriptionFees.Count > 1 && !hasSubscriptionInvoice)
            {
                pnlError.Visible = true;
                lblError.Text =
                    "يوجد أكثر من اشتراك سنوي متاح لهذا الصف ولم تحدد " +
                    "الفاتورة الرسم المناسب. يجب إصدار الفاتورة حسب مسار " +
                    "الطالب قبل التحصيل.";
            }

            // =====================================================
            // دفعات الاشتراكات السنوية
            // =====================================================
            var subscriptionPayments = payments
                .Where(p => subscriptionFeeIds.Contains(p.FeeId))
                .ToList();

            decimal runningPaid = 0m;

            var subscriptionRows = subscriptionPayments
                .Select(p =>
                {
                    runningPaid += p.Amount;

                    return new
                    {
                        p.ReceiptNumber,

                        FeeName = p.Fee != null
                            ? p.Fee.Name
                            : "الاشتراك السنوي",

                        p.PaymentDate,
                        p.Amount,

                        AmountText = p.Amount.ToString("N2"),
                        GrossAmountText = _totalGrossDue.ToString("N2"),
                        DiscountText = _totalDiscount.ToString("N2"),
                        DueText = _totalDue.ToString("N2"),

                        Remaining = Math.Max(0m, _totalDue - runningPaid),

                        MethodName = EnumHelper.GetPaymentMethodName(p.Method)
                    };
                })
                .ToList();

            rptSubPayments.DataSource = subscriptionRows;
            rptSubPayments.DataBind();

            pnlSubTable.Visible = subscriptionRows.Count > 0;
            pnlSubEmpty.Visible = subscriptionRows.Count == 0;

            // =====================================================
            // ✅ الرسوم الأخرى - فقط التي لها فاتورة صادرة
            // =====================================================
            var otherFees = fees
                .Where(f =>
                    f.Type != FeeType.AnnualSubscription &&
                    invoices.Any(i => i.Lines.Any(l =>
                        l.FeeId.HasValue && l.FeeId.Value == f.Id)))
                .ToList();

            var otherItems = otherFees
                .Select(f =>
                {
                    var lines = invoices
                        .SelectMany(i => i.Lines
                            .Where(l => l.FeeId.HasValue && l.FeeId.Value == f.Id)
                            .Select(l => new { Invoice = i, Line = l }))
                        .ToList();

                    decimal due;

                    if (lines.Count == 0)
                    {
                        due = f.Amount;
                    }
                    else
                    {
                        due = lines
                            .GroupBy(x => x.Invoice.Id)
                            .Sum(g => g.Sum(x =>
                                StudentFinancialService.GetLineNetAmount(
                                    x.Invoice,
                                    x.Line,
                                    lineContexts[x.Invoice.Id])));
                    }

                    decimal paid = payments
                        .Where(p => p.FeeId == f.Id)
                        .Sum(p => p.Amount);

                    return new
                    {
                        f.Id,
                        FeeName = f.Name,
                        AmountText = due.ToString("N2"),
                        Paid = paid,
                        Remaining = Math.Max(0m, due - paid)
                    };
                })
                .ToList();

            rptOtherFees.DataSource = otherItems;
            rptOtherFees.DataBind();

            pnlOtherTable.Visible = otherItems.Count > 0;
            pnlOtherEmpty.Visible = otherItems.Count == 0;

            // =====================================================
            // بيانات الطالب
            // =====================================================
            lblStudentName.Text = _student.Name;
            lblNationalId.Text = _student.NationalId;

            lblClass.Text = _student.ClassRoom.GradeLevel != null
                ? _student.ClassRoom.GradeLevel.Name
                : _student.ClassRoom.Name;

            string sectionText = "—";
            if (!string.IsNullOrWhiteSpace(_student.ClassRoom.SectionName))
            {
                if (_student.ClassRoom.SectionName == "علمي")
                    sectionText = "القسم العلمي";
                else if (_student.ClassRoom.SectionName == "أدبي")
                    sectionText = "القسم الأدبي";
                else
                    sectionText = _student.ClassRoom.SectionName;
            }
            lblSection.Text = sectionText;

            lblDivision.Text = string.IsNullOrWhiteSpace(_student.ClassRoom.DivisionName)
                ? "—"
                : "شعبة " + _student.ClassRoom.DivisionName;

            lblGrade.Text = EnumHelper.GetStageArabic(_student.Stage.ToString());

            // =====================================================
            // إجماليات الاشتراك السنوي (البطاقات العلوية)
            // =====================================================
            lblTotalGrossDue.Text = _totalGrossDue.ToString("N2");
            lblTotalDiscount.Text = _totalDiscount.ToString("N2");
            lblTotalDue.Text = _totalDue.ToString("N2");
            lblTotalPaid.Text = _totalSubscriptionPaid.ToString("N2");
            lblRemaining.Text = Math.Max(0m, _totalDue - _totalSubscriptionPaid).ToString("N2");

            // =====================================================
            // حقول الطباعة
            // =====================================================
            lblPrintStage.Text = EnumHelper.GetStageArabic(_student.Stage.ToString());
            lblPrintDate.Text = DateTime.Now.ToString("yyyy/MM/dd - HH:mm");
            lblPrintStudent.Text = _student.Name;
            lblPrintFooterDate.Text = DateTime.Now.ToString("yyyy/MM/dd");

            string userName =
                (Session["DisplayName"] as string)
                ?? (Session["Username"] as string)
                ?? "النظام";

            lblPrintUser.Text = userName;
        }

        // =========================================================
        // عرض المتبقي
        // =========================================================
        protected string RenderRemaining(object value)
        {
            decimal x = value == null ? 0m : Convert.ToDecimal(value);

            if (x > 0)
            {
                return "<span class=\"fw-bold\" style=\"color:#e74c3c;\">"
                    + x.ToString("N2") + " د.ل</span>";
            }

            return "<span class=\"fw-bold\" style=\"color:#27ae60;\">"
                + "0.00 د.ل</span>";
        }

        // =========================================================
        // عرض المدفوع
        // =========================================================
        protected string RenderPaid(object value)
        {
            decimal x = value == null ? 0m : Convert.ToDecimal(value);

            if (x > 0)
            {
                return "<span class=\"fw-bold\" style=\"color:#27ae60;\">"
                    + x.ToString("N2") + " د.ل</span>";
            }

            return "<span class=\"text-muted\">0.00 د.ل</span>";
        }

        // =========================================================
        // تأكيد تسجيل الدفعة
        // =========================================================
        protected void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            // تحقق خادمي مستقل حتى لا يمكن تجاوز منع الدفع بطلب POST مباشر.
            if (SecurityHelper.CurrentRole == UserRole.View)
            {
                SecurityHelper.Deny();
                return;
            }

            int feeId;
            decimal amount;

            if (!int.TryParse(hdnFeeId.Value, out feeId) || feeId <= 0)
            {
                RedirectWithError("يرجى تحديد الرسم المطلوب");
                return;
            }

            if (!decimal.TryParse(txtPaymentAmount.Text, out amount) || amount <= 0)
            {
                RedirectWithError("المبلغ يجب أن يكون أكبر من صفر");
                return;
            }

            int yearId = _student.ClassRoom.AcademicYearId;

            var invoices = db.Invoices
                .Include(i => i.Lines)
                .Where(i =>
                    i.StudentId == StudentId &&
                    i.AcademicYearId == yearId &&
                    i.Status != InvoiceStatus.Cancelled &&
                    i.Status != InvoiceStatus.Draft &&
                    i.Lines.Any(l =>
                        l.FeeId.HasValue &&
                        l.FeeId.Value == feeId))
                .OrderByDescending(i => i.InvoiceDate)
                .ThenByDescending(i => i.Id)
                .ToList();

            var paidInvoicesContexts = StudentFinancialService.LoadLineNetContexts(db, invoices);

            var invoice = invoices.FirstOrDefault(i =>
                i.Lines.Any(l =>
                {
                    if (!l.FeeId.HasValue || l.FeeId.Value != feeId)
                        return false;

                    decimal lineNet = StudentFinancialService.GetLineNetAmount(
                        i, l, paidInvoicesContexts[i.Id]);

                    decimal paid = db.Payments
                        .Where(p => p.InvoiceId == i.Id && p.FeeId == feeId)
                        .Sum(p => (decimal?)p.Amount) ?? 0m;

                    return lineNet - paid > 0;
                }));

            if (invoice == null)
            {
                RedirectWithError(
                    "لا توجد فاتورة صادرة ذات رصيد متبقٍ لهذا الرسم. " +
                    "أنشئ الفاتورة أولاً.");
                return;
            }

            try
            {
                PaymentMethod method;
                if (!Enum.TryParse(ddlPaymentMethod.SelectedValue, out method))
                    method = PaymentMethod.Cash;

                var payment = FinancialService.CreatePayment(
                    db,
                    invoice.Id,
                    feeId,
                    amount,
                    method,
                    DateTime.Today,
                    "دفعة مرتبطة بالفاتورة " + invoice.InvoiceNumber);

                string stage = Uri.EscapeDataString(_stage.ToString());
                string msg = Uri.EscapeDataString(
                    "تم تسجيل الدفعة " + amount.ToString("N2") +
                    " د.ل بنجاح - رقم الإيصال: " + payment.ReceiptNumber);

                Response.Redirect(
                    "StudentReport.aspx?studentId=" + StudentId +
                    "&stage=" + stage +
                    "&msg=" + msg,
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (InvalidOperationException ex)
            {
                RedirectWithError(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "StudentReport payment failed: " + ex);

                RedirectWithError("تعذر تسجيل الدفعة. لم يتم حفظ أي تغيير.");
            }
        }

        // =========================================================
        // إعادة التوجيه مع رسالة خطأ
        // =========================================================
        private void RedirectWithError(string message)
        {
            string stage = Uri.EscapeDataString(_stage.ToString());
            string err = Uri.EscapeDataString(message ?? "حدث خطأ غير متوقع");

            Response.Redirect(
                "StudentReport.aspx?studentId=" + StudentId +
                "&stage=" + stage +
                "&err=" + err,
                false);

            Context.ApplicationInstance.CompleteRequest();
        }

        // =========================================================
        // التخلص من DbContext
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (db != null)
                db.Dispose();

            base.OnUnload(e);
        }
    }
}