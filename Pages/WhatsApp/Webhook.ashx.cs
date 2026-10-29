using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.WhatsApp
{
    public class Webhook : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            if (string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                Verify(context);
                return;
            }
            if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405;
                context.Response.End();
                return;
            }

            const int maxWebhookBytes = 1024 * 1024;
            byte[] body;
            if (context.Request.ContentLength > maxWebhookBytes ||
                !TryReadBounded(context.Request.InputStream, maxWebhookBytes, out body))
            {
                context.Response.StatusCode = 413;
                return;
            }
            if (!IsValidSignature(body, context.Request.Headers["X-Hub-Signature-256"]))
            {
                context.Response.StatusCode = 401;
                context.Response.End();
                return;
            }

            try
            {
                var serializer = new JavaScriptSerializer { MaxJsonLength = 1024 * 1024 };
                object payload = serializer.DeserializeObject(Encoding.UTF8.GetString(body));
                ProcessPayload(payload);
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/plain";
                context.Response.Write("EVENT_RECEIVED");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp webhook processing failed: " + ex);
                context.Response.StatusCode = 500;
            }
        }

        private static void Verify(HttpContext context)
        {
            string expected = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_WEBHOOK_VERIFY_TOKEN", "WhatsAppWebhookVerifyToken");
            string mode = context.Request.QueryString["hub.mode"];
            string supplied = context.Request.QueryString["hub.verify_token"];
            string challenge = context.Request.QueryString["hub.challenge"];
            if (string.Equals(mode, "subscribe", StringComparison.Ordinal) &&
                !string.IsNullOrEmpty(expected) && SecureEquals(expected, supplied) &&
                !string.IsNullOrEmpty(challenge))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/plain";
                context.Response.Write(challenge);
            }
            else context.Response.StatusCode = 403;
        }

        private static bool IsValidSignature(byte[] body, string signature)
        {
            string secret = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_APP_SECRET", "WhatsAppAppSecret");
            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(signature) ||
                !signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase)) return false;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] digest = hmac.ComputeHash(body);
                string expected = "sha256=" + BitConverter.ToString(digest).Replace("-", string.Empty).ToLowerInvariant();
                return SecureEquals(expected, signature.ToLowerInvariant());
            }
        }

        private static bool TryReadBounded(Stream input, int maximumBytes, out byte[] body)
        {
            using (var buffer = new MemoryStream())
            {
                byte[] chunk = new byte[81920];
                while (true)
                {
                    int remaining = maximumBytes + 1 - (int)buffer.Length;
                    if (remaining <= 0)
                    {
                        body = null;
                        return false;
                    }
                    int read = input.Read(chunk, 0, Math.Min(chunk.Length, remaining));
                    if (read == 0) break;
                    if (buffer.Length + read > maximumBytes)
                    {
                        body = null;
                        return false;
                    }
                    buffer.Write(chunk, 0, read);
                }
                body = buffer.ToArray();
                return true;
            }
        }

        private static bool SecureEquals(string left, string right)
        {
            if (left == null || right == null) return false;
            byte[] a = Encoding.UTF8.GetBytes(left);
            byte[] b = Encoding.UTF8.GetBytes(right);
            int difference = a.Length ^ b.Length;
            int length = Math.Min(a.Length, b.Length);
            for (int i = 0; i < length; i++) difference |= a[i] ^ b[i];
            return difference == 0;
        }

        private static void ProcessPayload(object payload)
        {
            var root = payload as Dictionary<string, object>;
            object[] entries;
            if (root == null || !TryArray(root, "entry", out entries)) return;
            foreach (object entryValue in entries)
            {
                var entry = entryValue as Dictionary<string, object>;
                object[] changes;
                if (entry == null || !TryArray(entry, "changes", out changes)) continue;
                foreach (object changeValue in changes)
                {
                    var change = changeValue as Dictionary<string, object>;
                    var value = GetDictionary(change, "value");
                    if (value == null) continue;
                    SaveIncoming(value);
                    UpdateStatuses(value);
                }
            }
        }

        private static void SaveIncoming(Dictionary<string, object> value)
        {
            object[] messages;
            if (!TryArray(value, "messages", out messages)) return;
            object[] contacts;
            TryArray(value, "contacts", out contacts);
            foreach (object messageValue in messages)
            {
                var message = messageValue as Dictionary<string, object>;
                if (message == null) continue;
                string providerId = GetString(message, "id");
                string phone = GetString(message, "from");
                if (string.IsNullOrWhiteSpace(providerId) || string.IsNullOrWhiteSpace(phone)) continue;
                using (var db = new SchoolDbContext())
                using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    string messageText = ReadMessageText(message);
                    int? parentId = FindParentId(db, phone);
                    if (db.WhatsAppMessages.Any(x => x.ProviderMessageId == providerId))
                    {
                        if (parentId.HasValue && IsOptOutMessage(messageText))
                            ApplyOptOut(db, parentId.Value, phone);
                        transaction.Commit();
                        continue;
                    }
                    string contactName = GetContactName(contacts, phone);
                    db.WhatsAppMessages.Add(new WhatsAppMessage
                    {
                        ProviderMessageId = providerId,
                        ParentId = parentId,
                        RecipientName = contactName,
                        PhoneNumber = phone,
                        MessageBody = messageText,
                        Direction = "Inbound",
                        Status = "Received",
                        CreatedAt = DateTime.UtcNow
                    });
                    db.SaveChanges();
                    if (parentId.HasValue && IsOptOutMessage(messageText))
                        ApplyOptOut(db, parentId.Value, phone);
                    else
                    {
                        // إرسال رد تلقائي بعدم استقبال الردود
                        db.WhatsAppMessages.Add(new WhatsAppMessage
                        {
                            ParentId = parentId,
                            RecipientName = contactName,
                            PhoneNumber = phone,
                            MessageBody = "عذراً، هذا الرقم مخصص للرسائل الإعلانية فقط ولا يتم مراقبة الردود عليه. يرجى التواصل مع الإدارة عبر الرقم الرسمي للمدرسة.",
                            Direction = "Outbound",
                            Status = "Queued",
                            IsReplyToIncoming = true,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "WhatsAppWebhook"
                        });
                        db.SaveChanges();
                    }
                    transaction.Commit();
                }
            }
        }

        private static void UpdateStatuses(Dictionary<string, object> value)
        {
            object[] statuses;
            if (!TryArray(value, "statuses", out statuses)) return;
            foreach (object statusValue in statuses)
            {
                var status = statusValue as Dictionary<string, object>;
                if (status == null) continue;
                string providerId = GetString(status, "id");
                if (string.IsNullOrWhiteSpace(providerId)) continue;
                string state = GetString(status, "status");
                string mapped = MapStatus(state);
                if (mapped == null) continue;
                string errors = ReadErrors(status);
                using (var db = new SchoolDbContext())
                using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var row = db.WhatsAppMessages.FirstOrDefault(x => x.ProviderMessageId == providerId);
                    if (row == null)
                    {
                        transaction.Commit();
                        continue;
                    }
                    if (ShouldApplyStatus(row.Status, mapped)) row.Status = mapped;
                    if (!string.IsNullOrWhiteSpace(errors)) row.ErrorDetails = errors;
                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }

        private static void ApplyOptOut(SchoolDbContext db, int parentId, string phone)
        {
            var consent = db.WhatsAppConsents.FirstOrDefault(x => x.ParentId == parentId);
            if (consent == null)
            {
                consent = new WhatsAppConsent { ParentId = parentId };
                db.WhatsAppConsents.Add(consent);
            }
            consent.IsOptedIn = false;
            consent.ConsentedAt = null;
            consent.RevokedAt = DateTime.UtcNow;
            consent.RevokeReason = "طلب إلغاء وارد عبر WhatsApp";
            consent.ConsentSource = "طلب إلغاء وارد عبر WhatsApp";
            consent.PhoneNumber = WhatsAppCloudApi.NormalizeRecipientNumber(phone);
            consent.UpdatedAt = DateTime.UtcNow;
            consent.UpdatedBy = "WhatsApp";
            db.SaveChanges();
        }

        private static int? FindParentId(SchoolDbContext db, string phone)
        {
            string normalized = WhatsAppCloudApi.NormalizeRecipientNumber(phone);
            if (normalized.Length < 8) return null;
            try
            {
                return db.Parents.AsNoTracking()
                    .Where(x => x.PhoneNormalized == normalized)
                    .Select(x => (int?)x.Id)
                    .FirstOrDefault();
            }
            catch (SqlException ex)
            {
                if (ex.Number != 207 && ex.Number != 208 && ex.Number != 209) throw;
                // Column not deployed yet: fall back to the legacy in-memory scan.
                foreach (Parent parent in db.Parents.AsNoTracking().Where(x => x.Phone != null).ToList())
                    if (WhatsAppCloudApi.NormalizeRecipientNumber(parent.Phone) == normalized) return parent.Id;
                return null;
            }
        }

        private static readonly string[] OptOutKeywords =
        {
            "stop", "stop all", "stop messages", "stop all messages", "unsubscribe",
            "إلغاء", "الغاء", "إيقاف", "ايقاف",
            "إلغاء الاشتراك", "الغاء الاشتراك", "إيقاف الاشتراك", "ايقاف الاشتراك",
            "إلغاء الرسائل", "الغاء الرسائل", "إيقاف الرسائل", "ايقاف الرسائل",
            "إلغاء جميع الرسائل", "الغاء جميع الرسائل", "إيقاف جميع الرسائل", "ايقاف جميع الرسائل"
        };

        private static bool IsOptOutMessage(string text)
        {
            string normalized = NormalizeKeywordText(text);
            if (normalized.Length == 0 || normalized.Length > 60) return false;
            foreach (string keyword in OptOutKeywords)
                if (string.Equals(normalized, keyword, StringComparison.Ordinal)) return true;
            return false;
        }

        private static string NormalizeKeywordText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var builder = new StringBuilder(text.Length);
            bool pendingSpace = false;
            foreach (char character in text.Trim().ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(character))
                {
                    if (pendingSpace && builder.Length > 0) builder.Append(' ');
                    pendingSpace = false;
                    builder.Append(character);
                }
                else
                {
                    pendingSpace = true;
                }
            }
            return builder.ToString();
        }

        private static string GetContactName(object[] contacts, string phone)
        {
            if (contacts == null) return phone;
            foreach (object item in contacts)
            {
                var contact = item as Dictionary<string, object>;
                if (contact == null || GetString(contact, "wa_id") != phone) continue;
                var profile = GetDictionary(contact, "profile");
                string name = GetString(profile, "name");
                if (!string.IsNullOrWhiteSpace(name)) return name;
            }
            return phone;
        }

        private static string ReadMessageText(Dictionary<string, object> message)
        {
            string type = GetString(message, "type");
            var content = GetDictionary(message, type);
            string text = GetString(content, "body");
            if (!string.IsNullOrWhiteSpace(text)) return text;
            return "رسالة واردة من نوع: " + (string.IsNullOrWhiteSpace(type) ? "غير معروف" : type);
        }

        private static string MapStatus(string value)
        {
            switch ((value ?? string.Empty).ToLowerInvariant())
            {
                case "sent": return "Sent";
                case "delivered": return "Delivered";
                case "read": return "Read";
                case "failed": return "Failed";
                default: return null;
            }
        }

        private static bool ShouldApplyStatus(string current, string incoming)
        {
            if (incoming == "Failed")
                return current != "Delivered" && current != "Read" && current != "Failed";
            return StatusRank(incoming) > StatusRank(current);
        }

        private static int StatusRank(string value)
        {
            switch (value)
            {
                case "Sent": return 1;
                case "Delivered": return 2;
                case "Read": return 3;
                case "Failed": return 1;
                default: return 0;
            }
        }

        private static string ReadErrors(Dictionary<string, object> status)
        {
            object[] errors;
            if (!TryArray(status, "errors", out errors) || errors.Length == 0) return null;
            var error = errors[0] as Dictionary<string, object>;
            if (error == null) return null;
            var data = GetDictionary(error, "error_data");
            return GetString(data, "details") ?? GetString(error, "title") ?? GetString(error, "message");
        }

        private static bool TryArray(Dictionary<string, object> dictionary, string key, out object[] array)
        {
            array = null;
            object value;
            if (dictionary == null || !dictionary.TryGetValue(key, out value)) return false;
            array = value as object[];
            return array != null;
        }

        private static Dictionary<string, object> GetDictionary(Dictionary<string, object> source, string key)
        {
            object value;
            return source != null && source.TryGetValue(key, out value)
                ? value as Dictionary<string, object> : null;
        }

        private static string GetString(Dictionary<string, object> source, string key)
        {
            object value;
            return source != null && source.TryGetValue(key, out value) && value != null
                ? Convert.ToString(value) : null;
        }
    }
}
