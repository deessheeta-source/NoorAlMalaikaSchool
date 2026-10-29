using System;

namespace NoorAlMalaikaSchool.Models
{
    /// <summary>
    /// Key/value store for non-secret runtime settings that must travel with the
    /// database when the site is moved to another server. Secrets must stay in
    /// IIS environment variables or a server-side secret store.
    /// </summary>
    public class SystemSetting
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}
