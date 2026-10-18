using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Payment
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الطالب")]
        public int StudentId { get; set; }

        [Display(Name = "الرسوم")]
        public int FeeId { get; set; }

        [Display(Name = "الفاتورة")]
        public int? InvoiceId { get; set; }

        [Required]
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        [Display(Name = "تاريخ الدفع")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "رقم الإيصال")]
        [StringLength(50)]
        public string ReceiptNumber { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "طريقة الدفع")]
        public PaymentMethod Method { get; set; }

        [Display(Name = "أنشأ بواسطة")]
        public string CreatedBy { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "الطالب")]
        public virtual Student Student { get; set; }

        [Display(Name = "الرسوم")]
        public virtual Fee Fee { get; set; }

        [Display(Name = "الفاتورة")]
        public virtual Invoice Invoice { get; set; }
    }
}
