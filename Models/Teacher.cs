using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Teacher
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الرقم التعريفي")]
        public string EmployeeNo { get; set; }

        [Display(Name = "الاسم الأول")]
        public string FirstName { get; set; }

        [Display(Name = "اسم الأب")]
        public string FatherName { get; set; }

        [Display(Name = "اسم الجد")]
        public string GrandfatherName { get; set; }

        [Display(Name = "اسم العائلة")]
        public string FamilyName { get; set; }

        [Required]
        [Display(Name = "الاسم الكامل")]
        public string Name { get; set; }

        [Display(Name = "الجنس")]
        public Gender Gender { get; set; }

        [Display(Name = "التخصص")]
        public string Specialty { get; set; }

        [Display(Name = "الهاتف")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "العنوان")]
        public string Address { get; set; }

        [Display(Name = "رقم الهوية")]
        public string NationalId { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "تاريخ بداية العمل")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [Display(Name = "نوع التعاقد")]
        public EmploymentType EmploymentType { get; set; }

        [Display(Name = "الراتب الأساسي")]
        [DataType(DataType.Currency)]
        public decimal BaseSalary { get; set; }

        [Display(Name = "قيمة الحصة")]
        [DataType(DataType.Currency)]
        public decimal LessonRate { get; set; }

        [Display(Name = "قيمة الساعة")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }

        [Display(Name = "عدد الساعات الشهري")]
        public decimal MonthlyHours { get; set; }

        [Display(Name = "رقم الحساب البنكي")]
        public string BankAccount { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }

        [Display(Name = "المستوى الدراسي")]
        public EducationStage Stage { get; set; }

        [Display(Name = "السنة الدراسية")]
        public int? AcademicYearId { get; set; }

        public virtual AcademicYear AcademicYear { get; set; }

        public virtual ICollection<TeacherAttendance> Attendances { get; set; }

        public virtual ICollection<TeacherSubjectAssignment> SubjectAssignments { get; set; }
    }
}
