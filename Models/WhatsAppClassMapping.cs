using System;

namespace NoorAlMalaikaSchool.Models
{
    // A classroom recipient cohort. It is deliberately not represented as a real
    // WhatsApp group: Cloud API delivery is addressed to opted-in parents one-to-one.
    public class WhatsAppClassMapping
    {
        public int Id { get; set; }
        public int ClassRoomId { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}
