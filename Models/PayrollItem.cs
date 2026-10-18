using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class PayrollItem
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "استحقاق المعلم")]
        public int TeacherPayrollId { get; set; }

        [Display(Name = "النوع")]
        public PayrollItemType Type { get; set; }

        [Display(Name = "القيمة")]
        public decimal Amount { get; set; }

        [Display(Name = "السبب")]
        public string Reason { get; set; }

        [Display(Name = "اعتمد بواسطة")]
        public string ApprovedBy { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "استحقاق المعلم")]
        public virtual TeacherPayroll TeacherPayroll { get; set; }
    }
}