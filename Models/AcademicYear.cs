using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class AcademicYear
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Display(Name = "تاريخ البداية")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاريخ النهاية")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "السنة الحالية")]
        public bool IsCurrent { get; set; }

        public virtual ICollection<ClassRoom> ClassRooms { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
        public virtual ICollection<Fee> Fees { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
