using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class PayrollPeriod
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "السنة")]
        public int Year { get; set; }

        [Display(Name = "الشهر")]
        public int Month { get; set; }

        [Display(Name = "تاريخ البداية")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاريخ النهاية")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "الحالة")]
        public PayrollPeriodStatus Status { get; set; }

        [Display(Name = "تاريخ الفتح")]
        public DateTime OpenedAt { get; set; }

        [Display(Name = "فتح بواسطة")]
        public string OpenedBy { get; set; }

        [Display(Name = "تاريخ الإغلاق")]
        public DateTime? ClosedAt { get; set; }

        [Display(Name = "أغلق بواسطة")]
        public string ClosedBy { get; set; }

        public virtual ICollection<TeacherPayroll> TeacherPayrolls { get; set; }
    }
}