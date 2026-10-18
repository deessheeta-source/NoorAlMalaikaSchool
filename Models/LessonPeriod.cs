using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class LessonPeriod
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "رقم الحصة")]
        public int PeriodNumber { get; set; }

        [Display(Name = "وقت البداية")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "وقت النهاية")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "المدة (دقيقة)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "ليس يوم دراسي")]
        public bool IsActive { get; set; }
    }
}