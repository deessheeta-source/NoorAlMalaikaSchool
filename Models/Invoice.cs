using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Invoice
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "رقم الفاتورة")]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Display(Name = "الطالب")]
        public int StudentId { get; set; }

        [Display(Name = "السنة الدراسية")]
        public int AcademicYearId { get; set; }

        [Display(Name = "تاريخ الفاتورة")]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        [Display(Name = "إجمالي البنود")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "إجمالي الخصومات")]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "الصافي المستحق")]
        public decimal NetAmount { get; set; }

        [Display(Name = "المبلغ المدفوع")]
        public decimal PaidAmount { get; set; }

        [Display(Name = "الحالة")]
        public InvoiceStatus Status { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "أنشئ بواسطة")]
        public string CreatedBy { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "الطالب")]
        public virtual Student Student { get; set; }

        [Display(Name = "السنة الدراسية")]
        public virtual AcademicYear AcademicYear { get; set; }

        [Display(Name = "البنود")]
        public virtual ICollection<InvoiceLine> Lines { get; set; }

        [Display(Name = "الخصومات")]
        public virtual ICollection<InvoiceDiscount> Discounts { get; set; }
    }
}
