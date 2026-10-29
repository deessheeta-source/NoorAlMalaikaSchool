using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    public static class FinancialService
    {
        public static string GetNextInvoiceNumber(SchoolDbContext db, DateTime invoiceDate)
        {
            long sequence = db.Database.SqlQuery<long>("SELECT NEXT VALUE FOR dbo.InvoiceNumberSequence").Single();
            return "INV-" + invoiceDate.Year + "-" + sequence.ToString("D6");
        }

        public static string GetNextReceiptNumber(SchoolDbContext db, DateTime paymentDate)
        {
            long sequence = db.Database.SqlQuery<long>("SELECT NEXT VALUE FOR dbo.ReceiptNumberSequence").Single();
            return "NMS-" + paymentDate.Year + "-" + sequence.ToString("D6");
        }

        public static void ValidateInvoiceForPayment(SchoolDbContext db, Invoice invoice, decimal amount)
        {
            if (invoice == null) throw new InvalidOperationException("الفاتورة غير موجودة.");
            if (invoice.Status == InvoiceStatus.Cancelled) throw new InvalidOperationException("لا يمكن تسجيل دفعة على فاتورة ملغاة.");
            if (invoice.Status == InvoiceStatus.Draft) throw new InvalidOperationException("لا يمكن تسجيل دفعة على فاتورة مسودة.");
            if (amount <= 0) throw new InvalidOperationException("يجب أن يكون مبلغ الدفع أكبر من صفر.");

            decimal paid = db.Payments.Where(p => p.InvoiceId == invoice.Id).Select(p => (decimal?)p.Amount).Sum() ?? 0m;
            decimal remaining = Math.Max(0m, invoice.NetAmount - paid);
            if (amount > remaining)
                throw new InvalidOperationException("المبلغ يتجاوز الرصيد المتبقي للفاتورة: " + remaining.ToString("N2") + " د.ل.");
        }

        public static void SynchronizeInvoice(SchoolDbContext db, Invoice invoice)
        {
            decimal paid = db.Payments.Where(p => p.InvoiceId == invoice.Id).Select(p => (decimal?)p.Amount).Sum() ?? 0m;
            invoice.PaidAmount = Math.Min(invoice.NetAmount, Math.Max(0m, paid));
            invoice.Status = InvoiceCalculator.ResolveStatus(invoice.NetAmount, invoice.PaidAmount, invoice.Status);
        }

        public static Payment CreatePayment(SchoolDbContext db, int invoiceId, int feeId, decimal amount,
            PaymentMethod method, DateTime paymentDate, string notes)
        {
            using (var tx = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var invoice = db.Invoices.FirstOrDefault(i => i.Id == invoiceId);
                if (invoice == null) throw new InvalidOperationException("الفاتورة غير موجودة.");
                if (paymentDate.Date < invoice.InvoiceDate.Date)
                    throw new InvalidOperationException("تاريخ الدفع لا يمكن أن يسبق تاريخ الفاتورة.");
                ValidateInvoiceForPayment(db, invoice, amount);

                var line = db.InvoiceLines.FirstOrDefault(l => l.InvoiceId == invoice.Id && l.FeeId == feeId);
                if (line == null) throw new InvalidOperationException("الرسم المحدد غير موجود ضمن بنود الفاتورة.");
                var linePaid = db.Payments.Where(p => p.InvoiceId == invoice.Id && p.FeeId == feeId)
                    .Select(p => (decimal?)p.Amount).Sum() ?? 0m;
                var lineNetAmount = StudentFinancialService.GetLineNetAmount(db, invoice, line);
                var lineRemaining = Math.Max(0m, lineNetAmount - linePaid);
                if (amount > lineRemaining)
                    throw new InvalidOperationException("المبلغ يتجاوز المتبقي من بند الرسم: " + lineRemaining.ToString("N2") + " د.ل.");

                var payment = new Payment
                {
                    StudentId = invoice.StudentId,
                    FeeId = feeId,
                    InvoiceId = invoice.Id,
                    Amount = amount,
                    PaymentDate = paymentDate,
                    ReceiptNumber = GetNextReceiptNumber(db, paymentDate),
                    Method = method,
                    Notes = notes,
                    CreatedBy = NoorAlMalaikaSchool.Data.Helpers.EnumHelper.CurrentUserName,
                    CreatedAt = DateTime.Now
                };
                db.Payments.Add(payment);
                db.SaveChanges();
                SynchronizeInvoice(db, invoice);
                db.SaveChanges();
                tx.Commit();
                return payment;
            }
        }
    }
}
