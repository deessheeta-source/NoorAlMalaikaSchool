using System;
using System.Web;

namespace NoorAlMalaikaSchool.Services
{
    public static class ErrorMessageHelper
    {
        public static string GetUserMessage(Exception exception, string source)
        {
            AppErrorLogger.Log(exception, source);
            var safe = exception as InvalidOperationException;
            if (safe != null && LooksLikeUserMessage(safe.Message))
                return HttpUtility.HtmlEncode(safe.Message);
            return "حدث خطأ غير متوقع أثناء تنفيذ العملية. حاول مرة أخرى.";
        }

        private static bool LooksLikeUserMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return false;
            if (message.Length > 500) return false;
            if (message.IndexOf(" at ", StringComparison.Ordinal) >= 0) return false;
            if (message.IndexOf("System.", StringComparison.Ordinal) >= 0) return false;
            if (message.IndexOf("<", StringComparison.Ordinal) >= 0) return false;
            return true;
        }
    }
}
