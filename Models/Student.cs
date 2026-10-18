using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoorAlMalaikaSchool.Models
{
    public class Student
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "رقم الهوية")]
        public string NationalId { get; set; }

        [Required]
        [Display(Name = "الاسم الكامل")]
        public string Name { get; set; }

        [StringLength(100)]
        [Display(Name = "الاسم")]
        public string FirstName { get; set; }

        [StringLength(100)]
        [Display(Name = "الأب")]
        public string FatherName { get; set; }

        [StringLength(100)]
        [Display(Name = "الجد")]
        public string GrandfatherName { get; set; }

        [StringLength(100)]
        [Display(Name = "اللقب")]
        public string Nickname { get; set; }

        // توافق مؤقت مع الصفحات القديمة التي تستخدم الأسماء السابقة.
        [NotMapped]
        public string GrandFatherName
        {
            get { return GrandfatherName; }
            set { GrandfatherName = value; }
        }

        [NotMapped]
        public string Surname
        {
            get { return Nickname; }
            set { Nickname = value; }
        }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "الجنس")]
        public Gender Gender { get; set; }

        [Display(Name = "الجنسية")]
        public string Nationality { get; set; }

        [Display(Name = "الفصل الدراسي")]
        public int ClassRoomId { get; set; }

        [Display(Name = "القسم")]
        [StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "ولي الأمر")]
        public int? ParentId { get; set; }

        [Display(Name = "الهاتف")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "العنوان")]
        public string Address { get; set; }

        [StringLength(20)]
        [Display(Name = "رقم التسجيل")]
        public string RegistrationNo { get; set; }

        [StringLength(100)]
        [Display(Name = "اسم الأم")]
        public string MotherName { get; set; }

        [StringLength(20)]
        [Display(Name = "هاتف الأم")]
        [DataType(DataType.PhoneNumber)]
        public string MotherPhone { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "تاريخ التسجيل")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "الصورة")]
        public string PhotoPath { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }

        [Display(Name = "المستوى الدراسي")]
        public EducationStage Stage { get; set; }

        [Display(Name = "الفصل الدراسي")]
        public virtual ClassRoom ClassRoom { get; set; }

        [Display(Name = "ولي الأمر")]
        public virtual Parent Parent { get; set; }

        [Display(Name = "الحضور")]
        public virtual ICollection<Attendance> Attendances { get; set; }

        [Display(Name = "المدفوعات")]
        public virtual ICollection<Payment> Payments { get; set; }

        [Display(Name = "درجات الصحيفة الشهرية")]
        public virtual ICollection<MonthlySheetGrade> MonthlySheetGrades { get; set; }

        [Display(Name = "تقييم الصحيفة الشهرية")]
        public virtual ICollection<MonthlySheetEvaluation> MonthlySheetEvaluations { get; set; }

        [Display(Name = "الفواتير")]
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<StudentAcademicYear> AcademicYearEnrollments { get; set; }
        public virtual ICollection<StudentSubscription> Subscriptions { get; set; }
        public virtual ICollection<StudentDiscount> Discounts { get; set; }
        public virtual ICollection<StudentExemption> Exemptions { get; set; }
        public virtual ICollection<StudentStaffRelation> StaffRelations { get; set; }
    }
}
