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
                context.Response.Headers["Allow"] = "POST";
                return;
            }
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            if (!context.Request.IsSecureConnection && !context.Request.IsLocal)
            {
                context.Response.StatusCode = 403;
                return;
            }
            if (context.Request.ContentLength > 0)
            {
                context.Response.StatusCode = 413;
                return;
            }
            string secret = Environment.GetEnvironmentVariable("NOOR_WHATSAPP_DISPATCH_KEY");
            string authorization = context.Request.Headers["Authorization"] ?? string.Empty;
            string supplied = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorization.Substring(7).Trim() : string.Empty;
            if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
            {
                context.Response.StatusCode = 503;
                return;
            }
            if (!SecureEquals(secret, supplied))
            {
                context.Response.SuppressFormsAuthenticationRedirect = true;
                context.Response.StatusCode = 401;
                return;
            }
            try
            {
                context.Server.ScriptTimeout = 180;
                int count = WhatsAppQueueService.DispatchDue(5);
                System.Diagnostics.Trace.TraceInformation("WhatsApp dispatch completed; processed=" + count);
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
            for (int i = 0; i < a.Length; i++)
            {
                byte other = i < b.Length ? b[i] : (byte)0;
                diff |= a[i] ^ other;
            }
            return diff == 0;
        }
    }
}
