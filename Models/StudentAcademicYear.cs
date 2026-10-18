using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoorAlMalaikaSchool.Models
{
    public class StudentAcademicYear
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [ForeignKey(nameof(AcademicYearId))]
        public virtual AcademicYear AcademicYear { get; set; }

        [Required]
        public int ClassRoomId { get; set; }

        [ForeignKey(nameof(ClassRoomId))]
        public virtual ClassRoom ClassRoom { get; set; }

        public System.DateTime RegistrationDate { get; set; }

        public StudentEnrollmentStatus Status { get; set; }

        public bool IsActive { get; set; }

        public string Notes { get; set; }
    }
}