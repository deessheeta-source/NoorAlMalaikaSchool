using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    public sealed class WhatsAppApiSettings
    {
        public string AccessToken { get; private set; }
        public string PhoneNumberId { get; private set; }
        public string GraphVersion { get; private set; }
        public bool RepliesRequireConsent { get; private set; }
        public bool ManualLinks { get; private set; }

        public bool IsConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AccessToken) &&
                       !string.IsNullOrWhiteSpace(PhoneNumberId) &&
                       !string.IsNullOrWhiteSpace(GraphVersion);
            }
        }

        /// <summary>
        /// When true the module does not call Meta. It stages messages in the manual outbox
        /// and the operator sends each one via a click-to-chat wa.me link. This is used when
        /// Cloud API credentials are absent or when the school opts out of paid messaging.
        /// </summary>
        public bool ManualLinkMode
        {
            get { return ManualLinks || !IsConfigured; }
        }

        public static WhatsAppApiSettings Load()
        {
            string graphVersion = ReadSetting("NOOR_WHATSAPP_GRAPH_VERSION", "WhatsAppGraphVersion");
            if (!Regex.IsMatch(graphVersion, @"^v[0-9]+\.[0-9]+$", RegexOptions.CultureInvariant))
                graphVersion = string.Empty;
            return new WhatsAppApiSettings
            {
                AccessToken = ReadSetting("NOOR_WHATSAPP_ACCESS_TOKEN", "WhatsAppAccessToken"),
                PhoneNumberId = ReadSetting("NOOR_WHATSAPP_PHONE_NUMBER_ID", "WhatsAppPhoneNumberId"),
                GraphVersion = graphVersion,
                RepliesRequireConsent = IsTrue(ReadSetting("NOOR_WHATSAPP_REPLY_REQUIRE_CONSENT", "WhatsAppReplyRequireConsent")),
                ManualLinks = IsTrue(ReadSetting("NOOR_WHATSAPP_MANUAL_LINKS", "WhatsAppManualLinks"))
            };
        }

        private static bool IsTrue(string value)
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(value, "1", StringComparison.Ordinal) ||
                   string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
        }

        public static string ReadSetting(string environmentName, string appSettingName)
        {
            string value = Environment.GetEnvironmentVariable(environmentName);
            if (string.IsNullOrWhiteSpace(value))
                value = ConfigurationManager.AppSettings[appSettingName];
            return (value ?? string.Empty).Trim();
        }
    }

    /// <summary>Official WhatsApp Cloud API sender. Secrets are read from the IIS process environment.</summary>
    public static class WhatsAppCloudApi
    {
        public static string Send(WhatsAppMessage message)
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (!settings.IsConfigured)
                throw new InvalidOperationException("إعدادات WhatsApp Cloud API غير مكتملة.");

            if (message == null || string.IsNullOrWhiteSpace(message.PhoneNumber))
                throw new InvalidOperationException("رقم المستلم غير صالح.");

            var serializer = new JavaScriptSerializer { MaxJsonLength = 1024 * 1024 };
            var payload = new Dictionary<string, object>
            {
                { "messaging_product", "whatsapp" },
                { "recipient_type", "individual" },
                { "to", NormalizeNumber(message.PhoneNumber) }
            };

            if (!string.IsNullOrWhiteSpace(message.ProviderTemplateName))
            {
                var parameters = serializer.Deserialize<List<string>>(
                    message.TemplateParametersJson ?? "[]") ?? new List<string>();
                var components = new List<object>();
                if (parameters.Count > 0)
                {
                    var bodyParameters = new List<object>();
                    foreach (string value in parameters)
                        bodyParameters.Add(new Dictionary<string, object>
                        {
                            { "type", "text" }, { "text", value ?? string.Empty }
                        });
                    components.Add(new Dictionary<string, object>
                    {
                        { "type", "body" }, { "parameters", bodyParameters }
                    });
                }

                payload["type"] = "template";
                var templatePayload = new Dictionary<string, object>
                {
                    { "name", message.ProviderTemplateName },
                    { "language", new Dictionary<string, object>
                        { { "code", string.IsNullOrWhiteSpace(message.TemplateLanguage) ? "ar" : message.TemplateLanguage } } }
                };
                if (components.Count > 0) templatePayload["components"] = components;
                payload["template"] = templatePayload;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(message.MessageBody))
                    throw new InvalidOperationException("نص الرسالة فارغ.");
                payload["type"] = "text";
                payload["text"] = new Dictionary<string, object>
                {
                    { "preview_url", false },
                    { "body", message.MessageBody }
                };
            }

            byte[] body = Encoding.UTF8.GetBytes(serializer.Serialize(payload));
            string url = "https://graph.facebook.com/" + settings.GraphVersion + "/" +
                         Uri.EscapeDataString(settings.PhoneNumberId) + "/messages";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json";
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + settings.AccessToken;
            request.Timeout = 25000;
            request.ReadWriteTimeout = 25000;
            request.ContentLength = body.Length;

            using (Stream stream = request.GetRequestStream())
                stream.Write(body, 0, body.Length);

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var result = serializer.Deserialize<Dictionary<string, object>>(reader.ReadToEnd());
                    object[] messages = result != null && result.ContainsKey("messages")
                        ? result["messages"] as object[] : null;
                    var first = messages != null && messages.Length > 0
                        ? messages[0] as Dictionary<string, object> : null;
                    object id;
                    return first != null && first.TryGetValue("id", out id)
                        ? Convert.ToString(id) : null;
                }
            }
            catch (WebException ex)
            {
                string details = ReadError(ex);
                throw new InvalidOperationException(details, ex);
            }
        }

        /// <summary>Reads the phone number profile from Graph API to confirm the token and id are valid.</summary>
        public static string TestConnection()
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (!settings.IsConfigured)
                throw new InvalidOperationException("إعدادات WhatsApp Cloud API غير مكتملة.");

            string url = "https://graph.facebook.com/" + settings.GraphVersion + "/" +
                         Uri.EscapeDataString(settings.PhoneNumberId) +
                         "?fields=display_phone_number,verified_name,quality_rating,status";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json";
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + settings.AccessToken;
            request.Timeout = 20000;
            request.ReadWriteTimeout = 20000;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var serializer = new JavaScriptSerializer { MaxJsonLength = 1024 * 1024 };
                    var result = serializer.Deserialize<Dictionary<string, object>>(reader.ReadToEnd());
                    if (result == null) return "استجابت واجهة Meta دون بيانات.";
                    var parts = new List<string>();
                    object value;
                    if (result.TryGetValue("display_phone_number", out value) && value != null)
                        parts.Add("الرقم: " + Convert.ToString(value));
                    if (result.TryGetValue("verified_name", out value) && value != null)
                        parts.Add("الاسم: " + Convert.ToString(value));
                    if (result.TryGetValue("quality_rating", out value) && value != null)
                        parts.Add("الجودة: " + Convert.ToString(value));
                    if (result.TryGetValue("status", out value) && value != null)
                        parts.Add("الحالة: " + Convert.ToString(value));
                    return parts.Count == 0 ? "الاتصال ناجح." : string.Join(" · ", parts);
                }
            }
            catch (WebException ex)
            {
                throw new InvalidOperationException(ReadError(ex), ex);
            }
        }

        public static string NormalizeNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var digits = new StringBuilder();
            foreach (char c in value)
                if (char.IsDigit(c)) digits.Append(c);
            string result = digits.ToString();
            if (result.StartsWith("00", StringComparison.Ordinal)) result = result.Substring(2);
            return result;
        }

        public static string NormalizeRecipientNumber(string value)
        {
            string result = NormalizeNumber(value);
            if (result.Length == 0) return result;
            string countryCode = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_DEFAULT_COUNTRY_CODE", "WhatsAppDefaultCountryCode");
            if (string.IsNullOrWhiteSpace(countryCode)) countryCode = "218";
            countryCode = NormalizeNumber(countryCode);
            if ((value ?? string.Empty).TrimStart().StartsWith("+", StringComparison.Ordinal) ||
                (value ?? string.Empty).TrimStart().StartsWith("00", StringComparison.Ordinal) ||
                (!string.IsNullOrWhiteSpace(countryCode) && result.StartsWith(countryCode, StringComparison.Ordinal)))
                return result;
            if (result.StartsWith("0", StringComparison.Ordinal))
                result = result.Substring(1);
            return countryCode + result;
        }

        private static string ReadError(WebException ex)
        {
            try
            {
                if (ex.Response == null) return ex.Message;
                using (var reader = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8))
                {
                    string text = reader.ReadToEnd();
                    if (text.Length > 1500) text = text.Substring(0, 1500);
                    return string.IsNullOrWhiteSpace(text) ? ex.Message : text;
                }
            }
            catch { return ex.Message; }
        }
    }
}
