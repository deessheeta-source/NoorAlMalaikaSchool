using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class InvoiceDiscount
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الفاتورة")]
        public int InvoiceId { get; set; }

        [Display(Name = "سياسة الخصم")]
        public int? DiscountPolicyId { get; set; }

        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Display(Name = "النوع")]
        public DiscountType Type { get; set; }

        [Display(Name = "القيمة")]
        public decimal Value { get; set; }

        [Display(Name = "مبلغ الخصم")]
        public decimal DiscountAmount { get; set; }
        [Display(Name = "نوع الرسم المطبق عليه")] public FeeType? AppliedToFeeType { get; set; }
        public int? StudentDiscountId { get; set; }
        public int? StudentExemptionId { get; set; }
        [Display(Name = "السبب")] public string Reason { get; set; }

        [Display(Name = "الفاتورة")]
        public virtual Invoice Invoice { get; set; }

        [Display(Name = "سياسة الخصم")]
        public virtual DiscountPolicy DiscountPolicy { get; set; }
        public virtual StudentDiscount StudentDiscount { get; set; }
        public virtual StudentExemption StudentExemption { get; set; }
    }
}
