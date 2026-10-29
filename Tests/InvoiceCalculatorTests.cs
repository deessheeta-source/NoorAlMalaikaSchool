using System;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Tests
{
    /// <summary>
    /// اختبارات حساب الفواتير. تُشغَّل بأمر:
    ///     powershell -ExecutionPolicy Bypass -File Tests\Run-Tests.ps1
    /// لا تحتاج قاعدة بيانات ولا Internet.
    /// </summary>
    internal static class InvoiceCalculatorTests
    {
        private static int _failures;
        private static int _checks;

        private static int Main()
        {
            NoPayment_IsIssued();
            FullPayment_IsPaid();
            PartialPayment_IsPartiallyPaid();
            DiscountGreaterThanTotal_IsClampedToZero();
            Overpayment_IsClampedToNet();
            Cancelled_KeepsStatus();
            Draft_KeepsStatus();
            ZeroLines_IsPaid();
            Remaining_IsNeverNegative();
            MultipleLines_AreSummed();

            Console.WriteLine();
            Console.WriteLine(_failures == 0
                ? string.Format("PASS - {0} checks.", _checks)
                : string.Format("FAIL - {0} failure(s) out of {1} checks.", _failures, _checks));

            return _failures == 0 ? 0 : 1;
        }

        private static void NoPayment_IsIssued()
        {
            var totals = InvoiceCalculator.Calculate(new[] { 1000m, 250m }, 0m, 0m, InvoiceStatus.Issued);
            Expect("total", 1250m, totals.TotalAmount);
            Expect("net", 1250m, totals.NetAmount);
            Expect("paid", 0m, totals.PaidAmount);
            Expect("status", InvoiceStatus.Issued, totals.Status);
        }

        private static void FullPayment_IsPaid()
        {
            var totals = InvoiceCalculator.Calculate(1000m, 100m, 900m, InvoiceStatus.Issued);
            Expect("net", 900m, totals.NetAmount);
            Expect("paid", 900m, totals.PaidAmount);
            Expect("status", InvoiceStatus.Paid, totals.Status);
        }

        private static void PartialPayment_IsPartiallyPaid()
        {
            var totals = InvoiceCalculator.Calculate(1000m, 0m, 400m, InvoiceStatus.Issued);
            Expect("status", InvoiceStatus.PartiallyPaid, totals.Status);
            Expect("remaining", 600m, totals.Remaining);
        }

        private static void DiscountGreaterThanTotal_IsClampedToZero()
        {
            var totals = InvoiceCalculator.Calculate(500m, 800m, 0m, InvoiceStatus.Issued);
            Expect("net", 0m, totals.NetAmount);
            Expect("paid", 0m, totals.PaidAmount);
            Expect("status", InvoiceStatus.Paid, totals.Status);
        }

        private static void Overpayment_IsClampedToNet()
        {
            var totals = InvoiceCalculator.Calculate(1000m, 0m, 1500m, InvoiceStatus.Issued);
            Expect("paid", 1000m, totals.PaidAmount);
            Expect("status", InvoiceStatus.Paid, totals.Status);
            Expect("remaining", 0m, totals.Remaining);
        }

        private static void Cancelled_KeepsStatus()
        {
            var totals = InvoiceCalculator.Calculate(1000m, 0m, 500m, InvoiceStatus.Cancelled);
            Expect("status", InvoiceStatus.Cancelled, totals.Status);
        }

        private static void Draft_KeepsStatus()
        {
            var totals = InvoiceCalculator.Calculate(1000m, 0m, 0m, InvoiceStatus.Draft);
            Expect("status", InvoiceStatus.Draft, totals.Status);
        }

        private static void ZeroLines_IsPaid()
        {
            var totals = InvoiceCalculator.Calculate(new decimal[0], 0m, 0m, InvoiceStatus.Issued);
            Expect("total", 0m, totals.TotalAmount);
            Expect("status", InvoiceStatus.Paid, totals.Status);
        }

        private static void Remaining_IsNeverNegative()
        {
            var totals = InvoiceCalculator.Calculate(100m, 0m, 500m, InvoiceStatus.Issued);
            Expect("remaining", 0m, totals.Remaining);
        }

        private static void MultipleLines_AreSummed()
        {
            var totals = InvoiceCalculator.Calculate(new[] { 10.5m, 20.25m, 0m }, 0.75m, 0m, InvoiceStatus.Issued);
            Expect("total", 30.75m, totals.TotalAmount);
            Expect("net", 30.00m, totals.NetAmount);
        }

        private static void Expect(string label, decimal expected, decimal actual)
        {
            _checks++;
            if (expected != actual)
            {
                _failures++;
                Console.WriteLine("FAIL {0}: expected {1}, got {2}", label, expected, actual);
            }
        }

        private static void Expect(string label, InvoiceStatus expected, InvoiceStatus actual)
        {
            _checks++;
            if (expected != actual)
            {
                _failures++;
                Console.WriteLine("FAIL {0}: expected {1}, got {2}", label, expected, actual);
            }
        }
    }
}
