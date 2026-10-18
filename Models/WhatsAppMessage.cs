using System;

namespace NoorAlMalaikaSchool.Models
{
    public class WhatsAppMessage
    {
        public int Id { get; set; }
        public int? CampaignId { get; set; }
        public int? ParentId { get; set; }
        public int? StudentId { get; set; }
        public string RecipientName { get; set; }
        public string PhoneNumber { get; set; }
        public string MessageBody { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }
        public string ProviderMessageId { get; set; }
        public string ProviderTemplateName { get; set; }
        public string TemplateLanguage { get; set; }
        public string TemplateParametersJson { get; set; }
        public string ErrorDetails { get; set; }
        public int Attempts { get; set; }
        public bool IsReplyToIncoming { get; set; }
        public bool IsInternalRecipient { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
