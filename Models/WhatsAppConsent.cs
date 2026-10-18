using System;

namespace NoorAlMalaikaSchool.Models
{
    public class WhatsAppConsent
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsOptedIn { get; set; }
        public DateTime? ConsentedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string ConsentSource { get; set; }
        public string RevokeReason { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
