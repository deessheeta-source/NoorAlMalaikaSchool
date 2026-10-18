using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class TeacherPayroll
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الفترة المالية")]
        public int PayrollPeriodId { get; set; }

        [Display(Name = "المعلم")]
        public int TeacherId { get; set; }

        [Display(Name = "نوع التعاقد وقت الحساب")]
        public EmploymentType EmploymentTypeApplied { get; set; }

        [Display(Name = "الراتب الأساسي المطبق")]
        public decimal BaseSalaryApplied { get; set; }

        [Display(Name = "الساعات الشهرية المطبقة")]
        public decimal MonthlyHoursApplied { get; set; }

        [Display(Name = "الحصص المقررة")]
        public int ScheduledLessons { get; set; }

        [Display(Name = "حضور كامل")]
        public int FullAttendance { get; set; }

        [Display(Name = "حضور جزئي")]
        public int PartialAttendance { get; set; }

        [Display(Name = "غياب بعذر")]
        public int AbsentExcused { get; set; }

        [Display(Name = "غياب بدون عذر")]
        public int AbsentUnexcused { get; set; }

        [Display(Name = "حصة ملغاة")]
        public int Cancelled { get; set; }

        [Display(Name = "تعويضات كاملة")]
        public int SubstitutionFull { get; set; }

        [Display(Name = "تعويضات جزئية")]
        public int SubstitutionPartial { get; set; }

        [Display(Name = "إجمالي الدقائق")]
        public int TotalMinutes { get; set; }

        [Display(Name = "قيمة الحصة المطبقة")]
        public decimal LessonRateApplied { get; set; }

        [Display(Name = "قيمة الساعة المطبقة")]
        public decimal HourlyRateApplied { get; set; }

        [Display(Name = "قيمة الحضور المستحق")]
        public decimal EarnedAmount { get; set; }

        [Display(Name = "قيمة الخصومات")]
        public decimal DeductionAmount { get; set; }

        [Display(Name = "قيمة التسويات")]
        public decimal AdjustmentAmount { get; set; }

        [Display(Name = "صافي المستحق")]
        public decimal NetAmount { get; set; }

        [Display(Name = "الفترة المالية")]
        public virtual PayrollPeriod PayrollPeriod { get; set; }

        [Display(Name = "المعلم")]
        public virtual Teacher Teacher { get; set; }

        public virtual ICollection<PayrollItem> Items { get; set; }

        public virtual ICollection<PayrollLessonLine> LessonLines { get; set; }
    }
}