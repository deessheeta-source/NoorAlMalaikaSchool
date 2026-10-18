using System;
using System.Text;
using System.Web;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.WhatsApp
{
    public class Dispatch : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405;
                return;
            }
            string secret = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_DISPATCH_KEY", "WhatsAppDispatchKey");
            string authorization = context.Request.Headers["Authorization"] ?? string.Empty;
            string supplied = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorization.Substring(7).Trim() : string.Empty;
            if (string.IsNullOrWhiteSpace(secret) || !SecureEquals(secret, supplied))
            {
                context.Response.StatusCode = 401;
                return;
            }
            try
            {
                int count = WhatsAppQueueService.DispatchDue(50);
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.Write("processed=" + count);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp dispatch failed: " + ex);
                context.Response.StatusCode = 500;
                context.Response.Write("dispatch failed");
            }
        }

        private static bool SecureEquals(string left, string right)
        {
            if (left == null || right == null) return false;
            byte[] a = Encoding.UTF8.GetBytes(left);
            byte[] b = Encoding.UTF8.GetBytes(right);
            int diff = a.Length ^ b.Length;
            int length = Math.Min(a.Length, b.Length);
            for (int i = 0; i < length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
