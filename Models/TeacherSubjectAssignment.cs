using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    /// <summary>
    /// يربط المعلم بأكثر من مادة دون إنشاء سجل معلم جديد لكل مادة.
    /// الأسعار هنا هي أسعار التكليف الحالية، بينما السعر التاريخي للحصة
    /// يُحفظ في AttendanceRecord/PayrollLessonLine.
    /// </summary>
    public class TeacherSubjectAssignment
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public decimal LessonRate { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
