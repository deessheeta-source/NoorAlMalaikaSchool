using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class Substitution
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "الحصة اليومية")
        ]
        public int? DailyLessonId { get; set; }

        [Display(Name = "التاريخ")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "الفصل")]
        public int ClassRoomId { get; set; }

        [Display(Name = "رقم الحصة")]
        public int PeriodNumber { get; set; }

        [Display(Name = "المادة")]
        public int SubjectId { get; set; }

        [Display(Name = "المعلم الأصلي")]
        public int OriginalTeacherId { get; set; }

        [Display(Name = "المعلم المُعوِّض")]
        public int SubstituteTeacherId { get; set; }

        [Display(Name = "نوع التعويض")]
        public AttendanceStatus Status { get; set; }

        [Display(Name = "معتمد")]
        public bool IsApproved { get; set; }

        [Display(Name = "اعتمد بواسطة")]
        [StringLength(100)]
        public string ApprovedBy { get; set; }

        [Display(Name = "وقت الاعتماد")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "سجّل بواسطة")]
        [StringLength(100)]
        public string RecordedBy { get; set; }

        [Display(Name = "وقت التسجيل")]
        public DateTime? RecordedAt { get; set; }

        [Display(Name = "الفصل")]
        public virtual DailyLesson DailyLesson { get; set; }

        public virtual ClassRoom ClassRoom { get; set; }

        [Display(Name = "المادة")]
        public virtual Subject Subject { get; set; }

        [Display(Name = "المعلم الأصلي")]
        public virtual Teacher OriginalTeacher { get; set; }

        [Display(Name = "المعلم المُعوِّض")]
        public virtual Teacher SubstituteTeacher { get; set; }
    }
}