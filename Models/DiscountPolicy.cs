using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class DiscountPolicy
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Display(Name = "النوع")]
        public DiscountType Type { get; set; }

        [Display(Name = "نوع القيمة")]
        public DiscountKind Kind { get; set; }

        [Required]
        [Display(Name = "القيمة")]
        public decimal Value { get; set; }

        [Display(Name = "يُطبق تلقائياً")]
        public bool ApplyAutomatically { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Display(Name = "ترتيب التطبيق")]
        public int SortOrder { get; set; }
        [Display(Name = "ينطبق على نوع رسم")] public FeeType? AppliesToFeeType { get; set; }
    }
}
