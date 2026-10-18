using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Subject
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Display(Name = "الرمز")]
        [StringLength(20)]
        public string Code { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Display(Name = "المرحلة")]
        public EducationStage Stage { get; set; }

        [Display(Name = "الصف/الفرع")]
        public SubjectCategory Category { get; set; }
    }
}
