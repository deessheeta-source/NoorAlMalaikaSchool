using System;

namespace NoorAlMalaikaSchool.Models
{
    public class WhatsAppCampaign
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string MessageBody { get; set; }
        public string MessageKind { get; set; }
        public string Status { get; set; }
        public string AudienceType { get; set; }
        public int? ClassRoomId { get; set; }
        public int? TargetStudentId { get; set; }
        public int? WhatsAppTemplateId { get; set; }
        public bool IncludeBalance { get; set; }
        public bool IncludeAttendance { get; set; }
        public bool IncludeGrades { get; set; }
        public bool AppendDailyLessons { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
