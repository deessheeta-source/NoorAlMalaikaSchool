using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Data.Services
{
    public static class StudentFinancialService
    {
        public sealed class FeeAmount
        {
            public Fee Fee { get; set; }
            public decimal Amount { get; set; }
        }

        public sealed class BenefitCalculation
        {
            public List<InvoiceDiscount> Adjustments { get; private set; }
            public decimal TotalDiscount { get; set; }
            public decimal NetAmount { get; set; }
            public BenefitCalculation() { Adjustments = new List<InvoiceDiscount>(); }
        }

        public sealed class LineNetContext
        {
            public IList<InvoiceDiscount> Discounts { get; set; }
            public IDictionary<int, FeeType> FeeTypes { get; set; }
        }

        public static LineNetContext LoadLineNetContext(SchoolDbContext db, Invoice invoice)
        {
            if (db == null || invoice == null) return null;
            var discounts = db.InvoiceDiscounts.Where(d => d.InvoiceId == invoice.Id).ToList();
            var feeTypes = new Dictionary<int, FeeType>();
            var lineFeeIds = invoice.Lines == null
                ? new List<int>()
                : invoice.Lines.Where(l => l.FeeId.HasValue).Select(l => l.FeeId.Value).Distinct().ToList();
            if (lineFeeIds.Count > 0)
            {
                foreach (var fee in db.Fees.Where(f => lineFeeIds.Contains(f.Id)).Select(f => new { f.Id, f.Type }).ToList())
                    feeTypes[fee.Id] = fee.Type;
            }
            return new LineNetContext { Discounts = discounts, FeeTypes = feeTypes };
        }

        public static IDictionary<int, LineNetContext> LoadLineNetContexts(SchoolDbContext db, IEnumerable<Invoice> invoices)
        {
            var result = new Dictionary<int, LineNetContext>();
            var invoiceList = invoices == null
                ? new List<Invoice>()
                : invoices.Where(i => i != null).ToList();
            if (db == null || invoiceList.Count == 0) return result;

            var invoiceIds = invoiceList.Select(i => i.Id).ToList();
            var discountsByInvoice = db.InvoiceDiscounts
                .Where(d => invoiceIds.Contains(d.InvoiceId))
                .ToList()
                .GroupBy(d => d.InvoiceId)
                .ToDictionary(g => g.Key, g => (IList<InvoiceDiscount>)g.ToList());

            var lineFeeIds = invoiceList
                .SelectMany(i => i.Lines == null ? new List<InvoiceLine>() : i.Lines)
                .Where(l => l.FeeId.HasValue)
                .Select(l => l.FeeId.Value)
                .Distinct()
                .ToList();

            var feeTypes = new Dictionary<int, FeeType>();
            if (lineFeeIds.Count > 0)
            {
                foreach (var fee in db.Fees.Where(f => lineFeeIds.Contains(f.Id)).Select(f => new { f.Id, f.Type }).ToList())
                    feeTypes[fee.Id] = fee.Type;
            }

            foreach (var invoice in invoiceList)
            {
                result[invoice.Id] = new LineNetContext
                {
                    Discounts = discountsByInvoice.ContainsKey(invoice.Id)
                        ? discountsByInvoice[invoice.Id]
                        : new List<InvoiceDiscount>(),
                    FeeTypes = feeTypes
                };
            }
            return result;
        }

        public static int GetCurrentAcademicYearId(SchoolDbContext db)
        {
            var current = db.AcademicYears.FirstOrDefault(y => y.IsCurrent);
            if (current != null) return current.Id;
            var latest = db.AcademicYears.OrderByDescending(y => y.StartDate).FirstOrDefault();
            return latest == null ? 0 : latest.Id;
        }

        public static StudentAcademicYear EnsureEnrollment(SchoolDbContext db, Student student, int academicYearId)
        {
            if (student == null) throw new ArgumentNullException("student");
            var existing = db.StudentAcademicYears.FirstOrDefault(x => x.StudentId == student.Id && x.AcademicYearId == academicYearId);
            if (existing != null)
            {
                if (existing.ClassRoomId != student.ClassRoomId)
                {
                    existing.ClassRoomId = student.ClassRoomId;
                    existing.IsActive = true;
                }
                return existing;
            }
            var enrollment = new StudentAcademicYear
            {
                StudentId = student.Id,
                AcademicYearId = academicYearId,
                ClassRoomId = student.ClassRoomId,
                RegistrationDate = student.EnrollmentDate == default(DateTime) ? DateTime.Today : student.EnrollmentDate,
                Status = StudentEnrollmentStatus.Active,
                IsActive = student.IsActive,
                Notes = "تم إنشاؤه تلقائياً من تسجيل الطالب الحالي."
            };
            db.StudentAcademicYears.Add(enrollment);
            return enrollment;
        }

        public static List<Fee> GetApplicableFees(SchoolDbContext db, Student student, int academicYearId)
        {
            if (student == null || student.ClassRoom == null) return new List<Fee>();
            int classId = student.ClassRoomId;
            int gradeId = student.ClassRoom.GradeLevelId;
            var exact = db.Fees.Where(f => f.AcademicYearId == academicYearId && f.ClassRoomId == classId).ToList();
            var generic = db.Fees.Where(f => f.AcademicYearId == academicYearId && !f.ClassRoomId.HasValue &&
                                              (!f.GradeLevelId.HasValue || f.GradeLevelId.Value == gradeId)).ToList();
            var result = exact.ToList();
            foreach (var fee in generic)
            {
                if (!result.Any(x => x.Type == fee.Type && x.Name == fee.Name)) result.Add(fee);
            }
            return result.OrderBy(f => f.Type).ThenBy(f => f.Name).ThenBy(f => f.Id).ToList();
        }

        public static bool HasSibling(SchoolDbContext db, Student student)
        {
            return student != null && student.ParentId.HasValue &&
                   db.Students.Any(s => s.Id != student.Id && s.ParentId == student.ParentId && s.IsActive);
        }

        public static bool HasStaffRelation(SchoolDbContext db, Student student)
        {
            return student != null && db.StudentStaffRelations.Any(r => r.StudentId == student.Id && r.IsActive && r.Employee.IsActive);
        }

        // =========================================================
        // ✅ NEW: التعرف على السياسات ذات المبلغ اليدوي
        //         (الإخوة / التسجيل المبكر / أي سياسة بقيمة صفر)
        // =========================================================
        private static bool IsManualDiscountPolicy(DiscountPolicy p)
        {
            if (p == null) return false;

            // سياسة بقيمة صفر → مبلغها يُدخَل يدوياً
            if (p.Value <= 0m) return true;

            // سياسات محددة بالاسم
            string name = (p.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) return false;

            return name.Contains("إخوة")
                || name.Contains("اخوة")
                || name.Contains("أخوة")
                || name.Contains("تسجيل المبكر")
                || name.Contains("تسجيل مبكر")
                || name.ToLower().Contains("sibling")
                || name.ToLower().Contains("early");
        }

        // =========================================================
        // ✅ سياسات مؤهَّلة للتطبيق التلقائي
        //    (بعد التعديل: استُبعدت الإخوة والتسجيل المبكر)
        // =========================================================
        public static List<DiscountPolicy> GetEligiblePolicies(SchoolDbContext db, Student student, int academicYearId)
        {
            var policies = db.DiscountPolicies
                .Where(p => p.IsActive && p.ApplyAutomatically)
                .OrderBy(p => p.SortOrder)
                .ToList();

            var result = new List<DiscountPolicy>();

            foreach (var p in policies)
            {
                // ✅ لا تُطبَّق السياسات اليدوية تلقائياً
                if (IsManualDiscountPolicy(p)) continue;

                bool eligible = false;
                if (p.Type == DiscountType.StaffChild)
                    eligible = HasStaffRelation(db, student);

                if (eligible) result.Add(p);
            }

            return result;
        }

        private static bool InDateRange(DateTime start, DateTime? end, DateTime date)
        {
            return start.Date <= date.Date && (!end.HasValue || end.Value.Date >= date.Date);
        }

        private static decimal ScopeBase(IList<FeeAmount> lines, FeeType? scope)
        {
            return lines.Where(x => !scope.HasValue || (x.Fee != null && x.Fee.Type == scope.Value)).Sum(x => Math.Max(0m, x.Amount));
        }

        private static decimal PriorDiscountForScope(BenefitCalculation result, IList<FeeAmount> lines, FeeType? scope)
        {
            decimal gross = ScopeBase(lines, null);
            decimal scopeBase = ScopeBase(lines, scope);
            if (scopeBase <= 0m) return 0m;
            decimal total = 0m;
            foreach (var a in result.Adjustments)
            {
                if (!a.AppliedToFeeType.HasValue) total += gross <= 0m ? 0m : a.DiscountAmount * (scopeBase / gross);
                else if (scope.HasValue && a.AppliedToFeeType.Value == scope.Value) total += a.DiscountAmount;
            }
            return Math.Min(scopeBase, total);
        }

        public static BenefitCalculation Calculate(SchoolDbContext db, Student student, int academicYearId,
            IList<FeeAmount> lines, IEnumerable<int> selectedPolicyIds, DateTime calculationDate)
        {
            var result = new BenefitCalculation();
            if (lines == null) return result;

            var policies = db.DiscountPolicies.Where(p => p.IsActive).ToList();
            var selected = new HashSet<int>((selectedPolicyIds ?? Enumerable.Empty<int>()).ToList());

            foreach (var auto in GetEligiblePolicies(db, student, academicYearId))
                selected.Add(auto.Id);

            var studentDiscounts = db.StudentDiscounts
                .Include(d => d.DiscountPolicy)
                .Where(d => d.StudentId == student.Id && d.AcademicYearId == academicYearId && d.IsActive)
                .ToList()
                .Where(d => InDateRange(d.StartDate, d.EndDate, calculationDate))
                .ToList();

            var exemptions = db.StudentExemptions
                .Where(e => e.StudentId == student.Id && e.AcademicYearId == academicYearId && e.IsActive)
                .ToList()
                .Where(e => InDateRange(e.StartDate, e.EndDate, calculationDate))
                .ToList();

            decimal gross = lines.Sum(x => Math.Max(0m, x.Amount));
            decimal running = 0m;

            // Exemptions first
            foreach (var ex in exemptions)
            {
                decimal baseAmount = ScopeBase(lines, ex.FeeType);
                decimal available = Math.Max(0m, baseAmount - PriorDiscountForScope(result, lines, ex.FeeType));
                if (available <= 0m) continue;
                decimal amount = ex.Kind == DiscountKind.Percentage
                    ? Math.Round(available * Math.Min(100m, Math.Max(0m, ex.Value)) / 100m, 2)
                    : Math.Min(Math.Max(0m, ex.Value), available);
                if (amount <= 0m) continue;
                result.Adjustments.Add(new InvoiceDiscount
                {
                    Name = string.IsNullOrWhiteSpace(ex.Reason) ? "إعفاء" : "إعفاء - " + ex.Reason,
                    Type = DiscountType.General,
                    Value = ex.Value,
                    DiscountAmount = amount,
                    AppliedToFeeType = ex.FeeType,
                    StudentExemptionId = ex.Id,
                    Reason = ex.Reason
                });
                running += amount;
            }

            // Student discounts (assigned individually)
            foreach (var sd in studentDiscounts)
            {
                decimal baseAmount = ScopeBase(lines, sd.FeeType);
                decimal available = Math.Max(0m, baseAmount - PriorDiscountForScope(result, lines, sd.FeeType));
                if (available <= 0m) continue;
                decimal amount = sd.Kind == DiscountKind.Percentage
                    ? Math.Round(available * Math.Min(100m, Math.Max(0m, sd.Value)) / 100m, 2)
                    : Math.Min(Math.Max(0m, sd.Value), available);
                if (amount <= 0m) continue;
                result.Adjustments.Add(new InvoiceDiscount
                {
                    DiscountPolicyId = sd.DiscountPolicyId,
                    Name = sd.DiscountPolicy != null ? sd.DiscountPolicy.Name : "خصم خاص",
                    Type = sd.Type,
                    Value = sd.Value,
                    DiscountAmount = amount,
                    AppliedToFeeType = sd.FeeType,
                    StudentDiscountId = sd.Id,
                    Reason = sd.Reason
                });
                running += amount;
            }

            var assignedPolicyIds = new HashSet<int>(
                studentDiscounts.Where(d => d.DiscountPolicyId.HasValue).Select(d => d.DiscountPolicyId.Value));

            foreach (var policy in policies
                .Where(p => selected.Contains(p.Id) && !assignedPolicyIds.Contains(p.Id))
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Id))
            {
                // ============================================================
                // ✅ حماية: السياسات اليدوية لا تُطبَّق تلقائياً من هنا
                //    (يجب أن تُضاف يدوياً في Create.aspx.cs)
                // ============================================================
                if (IsManualDiscountPolicy(policy))
                {
                    // إذا كان المقدار = 0 → تجاهل
                    if (policy.Value <= 0m) continue;
                    // إذا كان المقدار > 0 (بيانات قديمة) → نسمح بالحساب كتوافقية
                }
                // ============================================================

                decimal baseAmount = ScopeBase(lines, policy.AppliesToFeeType);
                decimal available = Math.Max(0m, baseAmount - PriorDiscountForScope(result, lines, policy.AppliesToFeeType));
                if (available <= 0m) continue;
                decimal amount = policy.Kind == DiscountKind.Percentage
                    ? Math.Round(available * Math.Min(100m, Math.Max(0m, policy.Value)) / 100m, 2)
                    : Math.Min(Math.Max(0m, policy.Value), available);
                if (amount <= 0m) continue;
                result.Adjustments.Add(new InvoiceDiscount
                {
                    DiscountPolicyId = policy.Id,
                    Name = policy.Name,
                    Type = policy.Type,
                    Value = policy.Value,
                    DiscountAmount = amount,
                    AppliedToFeeType = policy.AppliesToFeeType
                });
                running += amount;
            }

            result.TotalDiscount = Math.Min(gross, Math.Max(0m, running));
            result.NetAmount = Math.Max(0m, gross - result.TotalDiscount);
            return result;
        }

        public static decimal GetLineNetAmount(SchoolDbContext db, Invoice invoice, InvoiceLine line)
        {
            if (invoice == null || line == null) return 0m;
            if (invoice.TotalAmount <= 0m) return Math.Max(0m, line.Amount);
            if (invoice.Lines == null && line.InvoiceId == invoice.Id && db.Entry(invoice).State != System.Data.Entity.EntityState.Detached)
                db.Entry(invoice).Collection(i => i.Lines).Load();
            return GetLineNetAmount(invoice, line, LoadLineNetContext(db, invoice));
        }

        public static decimal GetLineNetAmount(Invoice invoice, InvoiceLine line, LineNetContext context)
        {
            if (invoice == null || line == null) return 0m;
            if (invoice.TotalAmount <= 0m) return Math.Max(0m, line.Amount);

            decimal allocated = 0m;

            if (context != null && context.Discounts != null)
            {
                foreach (var d in context.Discounts)
                {
                    if (!d.AppliedToFeeType.HasValue)
                    {
                        allocated += d.DiscountAmount * (line.Amount / invoice.TotalAmount);
                    }
                    else if (line.FeeId.HasValue && context.FeeTypes != null)
                    {
                        var matchingFeeIds = new HashSet<int>();
                        foreach (var kv in context.FeeTypes)
                            if (kv.Value == d.AppliedToFeeType.Value) matchingFeeIds.Add(kv.Key);
                        if (!matchingFeeIds.Contains(line.FeeId.Value)) continue;

                        var eligibleLines = invoice.Lines == null
                            ? new List<InvoiceLine>()
                            : invoice.Lines.Where(l => l.FeeId.HasValue && matchingFeeIds.Contains(l.FeeId.Value)).ToList();
                        decimal eligibleBase = eligibleLines.Sum(l => l.Amount);
                        if (eligibleBase > 0m && eligibleLines.Any(l => l.Id == line.Id))
                            allocated += d.DiscountAmount * (line.Amount / eligibleBase);
                    }
                }
            }

            return Math.Max(0m, Math.Round(line.Amount - allocated, 2));
        }
    }
}