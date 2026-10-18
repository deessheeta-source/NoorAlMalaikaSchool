using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class InvoiceLine
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الفاتورة")]
        public int InvoiceId { get; set; }

        [Required]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Display(Name = "الرسم")]
        public int? FeeId { get; set; }

        [Display(Name = "بند يدوي")]
        public bool IsManual { get; set; }

        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        [Display(Name = "الفاتورة")]
        public virtual Invoice Invoice { get; set; }

        [Display(Name = "الرسم")]
        public virtual Fee Fee { get; set; }
    }
}
