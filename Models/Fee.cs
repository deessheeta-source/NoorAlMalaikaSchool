using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Fee
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        [Display(Name = "نوع الرسم")]
        public FeeType Type { get; set; }

        [Display(Name = "السنة الدراسية")]
        public int AcademicYearId { get; set; }

        [Display(Name = "المستوى الدراسي")]
        public int? GradeLevelId { get; set; }

        [Display(Name = "الفصل الدراسي")]
        public int? ClassRoomId { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "السنة الدراسية")]
        public virtual AcademicYear AcademicYear { get; set; }

        [Display(Name = "المستوى الدراسي")]
        public virtual GradeLevel GradeLevel { get; set; }

        [Display(Name = "الفصل الدراسي")]
        public virtual ClassRoom ClassRoom { get; set; }

        [Display(Name = "المدفوعات")]
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
