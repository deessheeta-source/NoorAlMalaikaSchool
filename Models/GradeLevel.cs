using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class GradeLevel
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Display(Name = "الترتيب")]
        public int SortOrder { get; set; }

        [Required]
        [Display(Name = "المرحلة التعليمية")]
        public EducationStage Stage { get; set; }

        public virtual ICollection<ClassRoom> ClassRooms { get; set; }
        public virtual ICollection<Fee> Fees { get; set; }
    }
}
