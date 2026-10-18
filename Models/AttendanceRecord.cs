using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class AttendanceRecord
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الحصة اليومية")]
        public int DailyLessonId { get; set; }

        [Display(Name = "التاريخ")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "المعلم")]
        public int TeacherId { get; set; }

        [Display(Name = "رقم الحصة")]
        public int PeriodNumber { get; set; }

        [Display(Name = "حالة الحضور")]
        public AttendanceStatus Status { get; set; }

        [Display(Name = "دقائق الحضور الفعلية")]
        public int ActualMinutes { get; set; }

        [Display(Name = "دقائق الغياب")]
        public int AbsentMinutes { get; set; }

        [Display(Name = "قيمة الحصة المطبقة")]
        public decimal LessonRateApplied { get; set; }

        [Display(Name = "قيمة الساعة المطبقة")]
        public decimal HourlyRateApplied { get; set; }

        [Display(Name = "مصدر التعويض") ]
        public int? SourceSubstitutionId { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "سجّل بواسطة")]
        public string RecordedBy { get; set; }

        [Display(Name = "وقت التسجيل")]
        public DateTime? RecordedAt { get; set; }

        [Display(Name = "معتمد")]
        public bool IsApproved { get; set; }

        [Display(Name = "اعتمد بواسطة")]
        public string ApprovedBy { get; set; }

        [Display(Name = "وقت الاعتماد")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "الحصة اليومية")]
        public virtual DailyLesson DailyLesson { get; set; }

        [Display(Name = "المعلم")]
        public virtual Teacher Teacher { get; set; }
    }
}