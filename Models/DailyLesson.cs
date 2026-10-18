using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class DailyLesson
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "التاريخ")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "رقم الحصة")]
        public int PeriodNumber { get; set; }

        [Display(Name = "وقت البداية")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "وقت النهاية")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "المدة (دقيقة)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "المعلم")]
        public int TeacherId { get; set; }

        [Display(Name = "المادة")]
        public int SubjectId { get; set; }

        [Display(Name = "الفصل")]
        public int ClassRoomId { get; set; }

        [Display(Name = "سجل الجدول")]
        public int? ScheduleEntryId { get; set; }

        [Display(Name = "الحالة")]
        public LessonStatus Status { get; set; }

        [Display(Name = "المعلم")]
        public virtual Teacher Teacher { get; set; }

        [Display(Name = "المادة")]
        public virtual Subject Subject { get; set; }

        [Display(Name = "الفصل")]
        public virtual ClassRoom ClassRoom { get; set; }

        [Display(Name = "سجل الجدول")]
        public virtual TimetableEntry ScheduleEntry { get; set; }
    }
}