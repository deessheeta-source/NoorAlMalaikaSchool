using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    /// <summary>
    /// لقطة مالية تفصيلية لكل حصة دخلت في استحقاق معلم بالحصة.
    /// تحفظ المادة والصف والمدة والسعر والمبلغ حتى لا تتغير الرواتب التاريخية
    /// عند تعديل بيانات المعلم لاحقاً.
    /// </summary>
    public class PayrollLessonLine
    {
        public int Id { get; set; }
        public int TeacherPayrollId { get; set; }
        public int AttendanceRecordId { get; set; }
        public int? SubjectId { get; set; }
        public int? ClassRoomId { get; set; }
        public DateTime Date { get; set; }
        public int PeriodNumber { get; set; }
        public int DurationMinutes { get; set; }
        public int ActualMinutes { get; set; }
        public int Status { get; set; }
        public decimal LessonRate { get; set; }
        public decimal EarnedAmount { get; set; }
        public decimal DeductionAmount { get; set; }

        public virtual TeacherPayroll TeacherPayroll { get; set; }
        public virtual AttendanceRecord AttendanceRecord { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ClassRoom ClassRoom { get; set; }
    }
}
