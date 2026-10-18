using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class TimetableEntry
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الصف الدراسي")]
        public int ClassRoomId { get; set; }

        [Display(Name = "اليوم")]
        public WeekDay DayOfWeek { get; set; }

        [Display(Name = "رقم الحصة")]
        public int PeriodNumber { get; set; }

        [Display(Name = "المادة")]
        public int SubjectId { get; set; }

        [Display(Name = "المعلم")]
        public int TeacherId { get; set; }

        [Display(Name = "وقت البداية")]
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        [Display(Name = "وقت النهاية")]
        [DataType(DataType.Time)]
        public TimeSpan? EndTime { get; set; }

        [Display(Name = "المدة (دقيقة)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "الحالة")]
        public LessonStatus Status { get; set; }

        [Display(Name = "الصف الدراسي")]
        public virtual ClassRoom ClassRoom { get; set; }

        [Display(Name = "المادة")]
        public virtual Subject Subject { get; set; }

        [Display(Name = "المعلم")]
        public virtual Teacher Teacher { get; set; }
    }
}