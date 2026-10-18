using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class TeacherAttendance
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "المعلم/ة")]
        public int TeacherId { get; set; }

        [Display(Name = "التاريخ")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "حاضر")]
        public bool IsPresent { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "المعلم/ة")]
        public virtual Teacher Teacher { get; set; }
    }
}