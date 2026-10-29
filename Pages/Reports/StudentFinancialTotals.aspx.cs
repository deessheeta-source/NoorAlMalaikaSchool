using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Services;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Pages.Reports
{
    public partial class StudentFinancialTotals : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private EducationStage CurrentStage
        {
            get { return EnumHelper.GetCurrentStage(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
               SecurityHelper.HideViewExportControls(this);

            if (!IsPostBack)
            {
                LoadYears();
                LoadGrades();
                LoadTotals();
            }
        }

        // =========================================================
        // تحميل السنوات الدراسية
        // =========================================================
        private void LoadYears()
        {
            var years = db.AcademicYears
                .AsNoTracking()
                .OrderByDescending(y => y.StartDate)
                .ToList();

            ddlYear.Items.Clear();

            ddlYear.Items.Add(
                new ListItem("جميع السنوات الدراسية", ""));

            foreach (var year in years)
            {
                ddlYear.Items.Add(
                    new ListItem(
                        year.Name,
                        year.Id.ToString()));
            }

            var current = years.FirstOrDefault(y => y.IsCurrent);

            if (current != null)
            {
                ddlYear.SelectedValue = current.Id.ToString();
            }
        }

        // =========================================================
        // تحميل الصفوف التابعة للمرحلة الحالية
        // =========================================================
        private void LoadGrades()
        {
            var grades = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == CurrentStage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();

            ddlGradeLevel.Items.Add(
                new ListItem("جميع الصفوف", ""));

            foreach (var grade in grades)
            {
                ddlGradeLevel.Items.Add(
                    new ListItem(
                        grade.Name,
                        grade.Id.ToString()));
            }
        }

        // =========================================================
        // تغيير عوامل التصفية
        // =========================================================
        protected void FilterChanged(object sender, EventArgs e)
        {
            LoadTotals();
        }

        // =========================================================
        // تحديث التقرير
        // =========================================================
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTotals();
        }

        // =========================================================
        // التحقق من الصف المرتبط بالرسوم
        // القيمة صفر تعني أن الرسوم غير مقيدة بصف معين.
        // =========================================================
        private bool MatchesGrade(int? feeGradeId, int studentGradeId)
        {
            return !feeGradeId.HasValue
                || feeGradeId.Value == 0
                || feeGradeId.Value == studentGradeId;
        }

        private bool MatchesGrade(int feeGradeId, int studentGradeId)
        {
            return feeGradeId == 0
                || feeGradeId == studentGradeId;
        }

        // =========================================================
        // التحقق من الفصل المرتبط بالرسوم
        // القيمة صفر تعني أن الرسوم غير مقيدة بفصل معين.
        // =========================================================
        private bool MatchesClass(int? feeClassRoomId, int studentClassRoomId)
        {
            return !feeClassRoomId.HasValue
                || feeClassRoomId.Value == 0
                || feeClassRoomId.Value == studentClassRoomId;
        }

        private bool MatchesClass(int feeClassRoomId, int studentClassRoomId)
        {
            return feeClassRoomId == 0
                || feeClassRoomId == studentClassRoomId;
        }

        // =========================================================
        // حساب إجماليات التقرير
        // =========================================================
        private void LoadTotals()
        {
            int yearId = 0;
            int gradeId = 0;

            bool hasYear =
                int.TryParse(ddlYear.SelectedValue, out yearId)
                && yearId > 0;

            bool hasGrade =
                int.TryParse(ddlGradeLevel.SelectedValue, out gradeId)
                && gradeId > 0;

            DateTime today = DateTime.Today;

            // -----------------------------------------------------
            // 1. جلب الطلاب النشطين في المرحلة الحالية
            // -----------------------------------------------------
            var studentsQuery = db.Students
                .AsNoTracking()
                .Where(s =>
                    s.IsActive
                    && s.Stage == CurrentStage
                    && s.ClassRoom != null);

            if (hasYear)
            {
                studentsQuery = studentsQuery.Where(
                    s => s.ClassRoom.AcademicYearId == yearId);
            }

            if (hasGrade)
            {
                studentsQuery = studentsQuery.Where(
                    s => s.ClassRoom.GradeLevelId == gradeId);
            }

            var students = studentsQuery
                .Select(s => new
                {
                    s.Id,
                    s.ClassRoomId,
                    GradeLevelId = s.ClassRoom.GradeLevelId,
                    AcademicYearId = s.ClassRoom.AcademicYearId
                })
                .ToList();

            var studentIds = students
                .Select(s => s.Id)
                .ToList();

            var yearIds = students
                .Select(s => s.AcademicYearId)
                .Distinct()
                .ToList();

            // لا توجد بيانات طلاب ضمن النطاق المحدد.
            if (students.Count == 0)
            {
                SetTotals(0m, 0m, 0m, 0m, 0m);

                UpdateScope(0);

                return;
            }

            // -----------------------------------------------------
            // 2. جلب الرسوم مباشرة من جدول Fees
            // يشمل ذلك الطلاب الذين لا توجد لهم فواتير.
            // -----------------------------------------------------
            var feesQuery = db.Fees
                .AsNoTracking()
                .Where(f => yearIds.Contains(f.AcademicYearId));

            if (hasYear)
            {
                feesQuery = feesQuery.Where(
                    f => f.AcademicYearId == yearId);
            }

            var fees = feesQuery.ToList();

            // -----------------------------------------------------
            // 3. جلب اشتراكات الطلاب النشطة
            // -----------------------------------------------------
            var subscriptions = db.StudentSubscriptions
                .AsNoTracking()
                .Where(x =>
                    studentIds.Contains(x.StudentId)
                    && yearIds.Contains(x.AcademicYearId)
                    && x.Status == SubscriptionStatus.Active)
                .ToList();

            decimal subscriptionTotal = 0m;

            var studentFeeTotals = new Dictionary<int, decimal>();

            foreach (var student in students)
            {
                decimal studentTotal = 0m;

                var applicableFees = fees.Where(f =>
                    f.AcademicYearId == student.AcademicYearId
                    && MatchesGrade(
                        f.GradeLevelId,
                        student.GradeLevelId)
                    && MatchesClass(
                        f.ClassRoomId,
                        student.ClassRoomId));

                foreach (var fee in applicableFees)
                {
                    var subscription = subscriptions
                        .Where(x =>
                            x.StudentId == student.Id
                            && x.AcademicYearId == student.AcademicYearId
                            && x.FeeId == fee.Id)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();

                    decimal amount =
                        subscription != null
                        && subscription.AmountOverride.HasValue
                            ? subscription.AmountOverride.Value
                            : fee.Amount;

                    studentTotal += amount;
                }

                studentFeeTotals[student.Id] = studentTotal;
                subscriptionTotal += studentTotal;
            }

            // -----------------------------------------------------
            // 4. جلب الفواتير غير الملغاة وغير المسودة
            // -----------------------------------------------------
            var invoicesQuery = db.Invoices
                .AsNoTracking()
                .Where(i =>
                    studentIds.Contains(i.StudentId)
                    && i.Status != InvoiceStatus.Cancelled
                    && i.Status != InvoiceStatus.Draft);

            if (hasYear)
            {
                invoicesQuery = invoicesQuery.Where(
                    i => i.AcademicYearId == yearId);
            }

            var invoices = invoicesQuery
                .Select(i => new
                {
                    i.Id,
                    i.StudentId,
                    i.DiscountAmount,
                    i.AcademicYearId
                })
                .ToList();

            var invoicedStudentIds = new HashSet<int>(
                invoices.Select(i => i.StudentId));

            // الخصومات المثبتة على الفواتير.
            decimal discountTotal = invoices.Sum(
                i => i.DiscountAmount);

            // -----------------------------------------------------
            // 5. خصومات الطلاب الذين لا توجد لهم فواتير
            // -----------------------------------------------------
            var noInvoiceStudentIds = studentIds
                .Where(id => !invoicedStudentIds.Contains(id))
                .ToList();

            var studentDiscounts = noInvoiceStudentIds.Count == 0
                ? new List<StudentDiscount>()
                : db.StudentDiscounts
                    .AsNoTracking()
                    .Where(d =>
                        noInvoiceStudentIds.Contains(d.StudentId)
                        && yearIds.Contains(d.AcademicYearId)
                        && d.IsActive
                        && d.StartDate <= today
                        && (!d.EndDate.HasValue
                            || d.EndDate.Value >= today))
                    .ToList();

            foreach (var student in students.Where(
                s => noInvoiceStudentIds.Contains(s.Id)))
            {
                decimal baseAmount =
                    studentFeeTotals.ContainsKey(student.Id)
                        ? studentFeeTotals[student.Id]
                        : 0m;

                decimal studentDiscountTotal = 0m;

                var discountsForStudent = studentDiscounts.Where(d =>
                    d.StudentId == student.Id
                    && d.AcademicYearId == student.AcademicYearId);

                foreach (var discount in discountsForStudent)
                {
                    decimal discountBase = baseAmount;

                    // إذا كان الخصم مرتبطًا بنوع رسوم محدد،
                    // تُحسب النسبة على الرسوم من النوع نفسه.
                    if (discount.FeeType.HasValue)
                    {
                        var applicableFees = fees.Where(f =>
                            f.AcademicYearId == student.AcademicYearId
                            && f.Type == discount.FeeType.Value
                            && MatchesGrade(
                                f.GradeLevelId,
                                student.GradeLevelId)
                            && MatchesClass(
                                f.ClassRoomId,
                                student.ClassRoomId));

                        discountBase = applicableFees.Sum(f =>
                        {
                            var subscription = subscriptions
                                .Where(x =>
                                    x.StudentId == student.Id
                                    && x.AcademicYearId == student.AcademicYearId
                                    && x.FeeId == f.Id)
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();

                            return subscription != null
                                && subscription.AmountOverride.HasValue
                                    ? subscription.AmountOverride.Value
                                    : f.Amount;
                        });
                    }

                    decimal value =
                        discount.Kind == DiscountKind.Percentage
                            ? discountBase * discount.Value / 100m
                            : discount.Value;

                    studentDiscountTotal += Math.Max(0m, value);
                }

                // لا يمكن أن يتجاوز الخصم الإجمالي مبلغ الرسوم
                // المحتسب لهذا الطالب.
                discountTotal += Math.Min(
                    baseAmount,
                    studentDiscountTotal);
            }

            // -----------------------------------------------------
            // 6. جلب المدفوعات
            // -----------------------------------------------------
            var allPayments = db.Payments
                .AsNoTracking()
                .Where(p => studentIds.Contains(p.StudentId))
                .ToList();

            var validInvoiceIds = new HashSet<int>(
                invoices.Select(i => i.Id));

            // المدفوعات المرتبطة بفواتير ضمن نطاق التقرير.
            decimal invoicePaid = allPayments
                .Where(p =>
                    p.InvoiceId.HasValue
                    && validInvoiceIds.Contains(p.InvoiceId.Value))
                .Sum(p => p.Amount);

            // الرسوم ضمن السنة الدراسية المحددة للطلاب.
            var feeIds = new HashSet<int>(
                fees.Select(f => f.Id));

            // Payment.FeeId من النوع int وليس int?.
            // لذلك نستخدمه مباشرة دون HasValue أو Value.
            decimal directPaid = allPayments
                .Where(p =>
                    !p.InvoiceId.HasValue
                    && feeIds.Contains(p.FeeId))
                .Sum(p => p.Amount);

            decimal paidTotal = invoicePaid + directPaid;

            // -----------------------------------------------------
            // 7. حساب المبالغ المستحقة والمتبقية
            // -----------------------------------------------------
            decimal dueTotal = Math.Max(
                0m,
                subscriptionTotal - discountTotal);

            decimal remainingTotal = Math.Max(
                0m,
                dueTotal - paidTotal);

            // -----------------------------------------------------
            // 8. عرض الإجماليات
            // -----------------------------------------------------
            SetTotals(
                subscriptionTotal,
                discountTotal,
                dueTotal,
                paidTotal,
                remainingTotal);

            UpdateScope(students.Count);
        }

        // =========================================================
        // عرض الإجماليات في الصفحة
        // =========================================================
        private void SetTotals(
            decimal subscription,
            decimal discount,
            decimal due,
            decimal paid,
            decimal remaining)
        {
            litSubscription.Text = subscription.ToString("N2");
            litDiscount.Text = discount.ToString("N2");
            litDue.Text = due.ToString("N2");
            litPaid.Text = paid.ToString("N2");
            litRemaining.Text = remaining.ToString("N2");
        }

        // =========================================================
        // عرض نطاق التقرير
        // =========================================================
        private void UpdateScope(int studentCount)
        {
            string yearText = ddlYear.SelectedItem != null
                ? ddlYear.SelectedItem.Text
                : "جميع السنوات الدراسية";

            string gradeText = ddlGradeLevel.SelectedItem != null
                ? ddlGradeLevel.SelectedItem.Text
                : "جميع الصفوف";

            litScope.Text =
                "نطاق التقرير: "
                + Server.HtmlEncode(yearText)
                + " — "
                + Server.HtmlEncode(gradeText)
                + " | عدد الطلاب ضمن النطاق: "
                + studentCount.ToString("N0")
                + " | تُحتسب الرسوم مباشرة من جدول الرسوم، بما يشمل الطلاب الذين لا توجد لهم فواتير.";
        }

        // =========================================================
        // تحرير موارد قاعدة البيانات
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (db != null)
            {
                db.Dispose();
            }

            base.OnUnload(e);
        }
    }
}