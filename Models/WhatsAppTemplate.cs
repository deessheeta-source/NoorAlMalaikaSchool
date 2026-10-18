using System;

namespace NoorAlMalaikaSchool.Models
{
    public class WhatsAppTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Body { get; set; }
        public string ProviderTemplateName { get; set; }
        public string LanguageCode { get; set; }
        public bool IsMetaApproved { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
