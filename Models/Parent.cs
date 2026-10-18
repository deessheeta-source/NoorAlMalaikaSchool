using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Parent
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "رقم الهوية")]
        public string NationalId { get; set; }

        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "الهاتف")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "العنوان")]
        public string Address { get; set; }

        [Display(Name = "صلة القرابة")]
        public string Relation { get; set; }

        [Display(Name = "الطلاب")]
        public virtual ICollection<Student> Students { get; set; }
    }
}
