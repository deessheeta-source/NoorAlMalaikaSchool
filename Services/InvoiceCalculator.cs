using System;
using System.Collections.Generic;
using System.Linq;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// Pure invoice arithmetic shared by the Create/Edit pages and FinancialService.
    /// Keeping it here makes the rules testable without a database or a page lifecycle.
    ///
    /// Rules (unchanged from the original page code):
    ///   NetAmount   = max(0, TotalAmount - DiscountAmount)
    ///   PaidAmount  = clamp(paidTotal, 0, NetAmount)
    ///   Status      = Paid when NetAmount is zero or fully paid,
    ///                 Issued when nothing was paid,
    ///                 PartiallyPaid otherwise.
    ///   Cancelled / Draft invoices keep their status (they are not recalculated).
    /// </summary>
    public static class InvoiceCalculator
    {
        public struct Totals
        {
            public decimal TotalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal NetAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public InvoiceStatus Status { get; set; }

            public decimal Remaining
            {
                get { return Math.Max(0m, NetAmount - PaidAmount); }
            }
        }

        public static Totals Calculate(
            IEnumerable<decimal> lineAmounts,
            decimal discountTotal,
            decimal paidTotal,
            InvoiceStatus currentStatus)
        {
            decimal total = lineAmounts == null ? 0m : lineAmounts.Sum();
            return Calculate(total, discountTotal, paidTotal, currentStatus);
        }

        public static Totals Calculate(
            decimal totalAmount,
            decimal discountTotal,
            decimal paidTotal,
            InvoiceStatus currentStatus)
        {
            decimal total = Math.Max(0m, totalAmount);
            decimal net = Math.Max(0m, total - Math.Max(0m, discountTotal));
            decimal paid = Math.Min(net, Math.Max(0m, paidTotal));

            return new Totals
            {
                TotalAmount = total,
                DiscountAmount = Math.Max(0m, discountTotal),
                NetAmount = net,
                PaidAmount = paid,
                Status = ResolveStatus(net, paid, currentStatus)
            };
        }

        public static InvoiceStatus ResolveStatus(
            decimal netAmount,
            decimal paidAmount,
            InvoiceStatus currentStatus)
        {
            if (currentStatus == InvoiceStatus.Cancelled || currentStatus == InvoiceStatus.Draft)
            {
                return currentStatus;
            }

            if (netAmount <= 0m) return InvoiceStatus.Paid;
            if (paidAmount <= 0m) return InvoiceStatus.Issued;
            if (paidAmount >= netAmount) return InvoiceStatus.Paid;
            return InvoiceStatus.PartiallyPaid;
        }

        /// <summary>Applies the calculated values to an existing Invoice entity.</summary>
        public static void Apply(Invoice invoice, decimal totalAmount, decimal discountTotal, decimal paidTotal)
        {
            if (invoice == null) throw new ArgumentNullException("invoice");

            Totals totals = Calculate(totalAmount, discountTotal, paidTotal, invoice.Status);
            invoice.TotalAmount = totals.TotalAmount;
            invoice.DiscountAmount = totals.DiscountAmount;
            invoice.NetAmount = totals.NetAmount;
            invoice.PaidAmount = totals.PaidAmount;
            invoice.Status = totals.Status;
        }
    }
}
