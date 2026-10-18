using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Attendance
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الطالب")]
        public int StudentId { get; set; }

        [Display(Name = "التاريخ")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "حاضر")]
        public bool IsPresent { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "الطالب")]
        public virtual Student Student { get; set; }
    }
}
