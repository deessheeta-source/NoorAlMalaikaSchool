using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoorAlMalaikaSchool.Models
{
    public class StudentExemption
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

        public FeeType? FeeType { get; set; }

        public DiscountKind Kind { get; set; }

        public decimal Value { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(100)]
        public string DecisionNo { get; set; }

        public DateTime? DecisionDate { get; set; }

        [MaxLength(100)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public string Reason { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; }
    }
}