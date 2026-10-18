using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class ClassRoom
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Display(Name = "المستوى الدراسي")]
        public int GradeLevelId { get; set; }

        [Display(Name = "السنة الدراسية")]
        public int AcademicYearId { get; set; }

        [Display(Name = "القسم")]
        public string SectionName { get; set; }

        /// <summary>
        /// الشعبة (أ / ب / ج / د) — تُستخدم لتقسيم الفصل إلى شعب لجدول الحصص
        /// </summary>
        [StringLength(10)]
        [Display(Name = "الشعبة")]
        public string DivisionName { get; set; }

        [Display(Name = "السعة")]
        public int Capacity { get; set; }

        [Display(Name = "اسم المعلم")]
        public string TeacherName { get; set; }

        [Display(Name = "المستوى الدراسي")]
        public virtual GradeLevel GradeLevel { get; set; }

        [Display(Name = "السنة الدراسية")]
        public virtual AcademicYear AcademicYear { get; set; }

        [Display(Name = "الطلاب")]
        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<Fee> Fees { get; set; }
    }
}