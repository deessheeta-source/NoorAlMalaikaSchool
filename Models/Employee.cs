using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required, StringLength(30)] public string EmployeeNo { get; set; }
        [Required, StringLength(200)] public string Name { get; set; }
        [StringLength(20)] public string NationalId { get; set; }
        public bool IsActive { get; set; }
    }
}
