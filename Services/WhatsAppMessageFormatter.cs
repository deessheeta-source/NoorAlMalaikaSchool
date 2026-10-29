using System;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// Central formatting for outbound WhatsApp messages. Every message starts with
    /// the school name so parents can identify the sender before reading the body.
    /// The routine is idempotent: a body that already starts with the school name
    /// is returned unchanged, which prevents duplicate headers in templates and
    /// re-sent messages.
    /// </summary>
    public static class WhatsAppMessageFormatter
    {
        public const string SchoolName = "مدرسة نور الملائكة";
        public const string Greeting = "السلام عليكم";

        public static string EnsureSchoolHeader(string body)
        {
            string text = (body ?? string.Empty).Trim();
            if (text.Length == 0) return string.Empty;

            if (text.StartsWith(SchoolName, StringComparison.Ordinal))
                return text;

            if (text.StartsWith(Greeting, StringComparison.Ordinal))
                return SchoolName + Environment.NewLine + text;

            return SchoolName + Environment.NewLine + Greeting + Environment.NewLine + text;
        }
    }
}
