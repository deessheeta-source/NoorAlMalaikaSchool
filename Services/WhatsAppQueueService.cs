using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>Claims queued messages atomically so overlapping IIS workers cannot send duplicates.</summary>
    public static class WhatsAppQueueService
    {
        private const int MaxAttempts = 5;
        private const int StuckSendingMinutes = 10;
        private static readonly TimeSpan[] RetryDelays =
        {
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(15),
            TimeSpan.FromMinutes(60),
            TimeSpan.FromMinutes(180)
        };

        public static bool TrySendNow(int messageId, bool allowFailedRetry)
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (settings.ManualLinkMode || !settings.IsConfigured) return false;
            using (var db = new SchoolDbContext())
            {
                string statusFilter = allowFailedRetry
                    ? "Status IN ('Queued','Failed')"
                    : "Status = 'Queued'";
                int claimedId = Claim(db, messageId, statusFilter, allowFailedRetry);
                if (claimedId <= 0) return false;
            }

            SendClaimed(messageId);
            return true;
        }

        public static int DispatchDue(int maximumMessages)
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (settings.ManualLinkMode || !settings.IsConfigured) return 0;
            RecoverStuckSending(StuckSendingMinutes);
            int sentToProvider = 0;
            int limit = Math.Max(1, Math.Min(maximumMessages, 100));
            for (int i = 0; i < limit; i++)
            {
                int id;
                using (var db = new SchoolDbContext())
                {
                    id = db.Database.SqlQuery<int>(@"
;WITH nextMessage AS
(
    SELECT TOP (1) Id
    FROM dbo.WhatsAppMessages WITH (UPDLOCK, READPAST, ROWLOCK, READCOMMITTEDLOCK)
    WHERE Direction = 'Outbound'
      AND Status IN ('Queued','Scheduled')
      AND (ScheduledAt IS NULL OR ScheduledAt <= SYSUTCDATETIME())
      AND (NextAttemptAt IS NULL OR NextAttemptAt <= SYSUTCDATETIME())
    ORDER BY ISNULL(ScheduledAt, CreatedAt), Id
)
UPDATE m
   SET Status = 'Sending', Attempts = Attempts + 1, ClaimedAt = SYSUTCDATETIME(),
       NextAttemptAt = NULL, ProviderMessageId = NULL, ErrorDetails = NULL, SendStartedAt = NULL
OUTPUT inserted.Id
  FROM dbo.WhatsAppMessages AS m
  JOIN nextMessage AS n ON n.Id = m.Id;").FirstOrDefault();
                }
                if (id <= 0) break;
                if (SendClaimed(id)) sentToProvider++;
            }
            return sentToProvider;
        }

        /// <summary>
        /// Returns rows left in Sending (interrupted worker) to Queued. Only claims older
        /// than the threshold are recovered, so an in-flight send is never stolen.
        /// </summary>
        public static int RecoverStuckSending(int olderThanMinutes)
        {
            int minutes = Math.Max(1, olderThanMinutes);
            using (var db = new SchoolDbContext())
            {
                int recovered = db.Database.ExecuteSqlCommand(@"
UPDATE dbo.WhatsAppMessages
   SET Status = CASE WHEN SendStartedAt IS NULL THEN 'Queued' ELSE 'Unknown' END,
       ClaimedAt = NULL, NextAttemptAt = NULL,
       ErrorDetails = CASE WHEN SendStartedAt IS NULL
           THEN N'أُعيدت الرسالة إلى الطابور بعد انقطاع قبل بدء طلب الإرسال.'
           ELSE N'انقطع العامل بعد بدء طلب الإرسال؛ راجع سجل Meta قبل أي إعادة يدوية.' END
 WHERE Status = 'Sending' AND SentAt IS NULL AND ProviderMessageId IS NULL
   AND ((ClaimedAt IS NOT NULL AND ClaimedAt < DATEADD(MINUTE, -@minutes, SYSUTCDATETIME()))
        OR (ClaimedAt IS NULL AND CreatedAt < DATEADD(MINUTE, -@minutes, SYSUTCDATETIME())));",
                    new SqlParameter("@minutes", minutes));
                db.Database.ExecuteSqlCommand(@"
UPDATE c
   SET Status = 'NeedsReview'
  FROM dbo.WhatsAppCampaigns AS c
 WHERE EXISTS
       (SELECT 1 FROM dbo.WhatsAppMessages AS m
        WHERE m.CampaignId = c.Id AND m.Status = 'Unknown')
   AND NOT EXISTS
       (SELECT 1 FROM dbo.WhatsAppMessages AS m
        WHERE m.CampaignId = c.Id
          AND m.Status IN ('Queued','Scheduled','Sending'));");
                return recovered;
            }
        }

        private static int Claim(SchoolDbContext db, int messageId, string statusFilter, bool resetAttempts)
        {
            return db.Database.SqlQuery<int>(@"
UPDATE dbo.WhatsAppMessages
   SET Status = 'Sending',
       Attempts = CASE WHEN @resetAttempts = 1 THEN 1 ELSE Attempts + 1 END,
       ClaimedAt = SYSUTCDATETIME(), NextAttemptAt = NULL,
       ProviderMessageId = NULL, ErrorDetails = NULL, SendStartedAt = NULL
OUTPUT inserted.Id
 WHERE Id = @id AND Direction = 'Outbound' AND " + statusFilter + @";",
                new SqlParameter("@id", messageId),
                new SqlParameter("@resetAttempts", resetAttempts ? 1 : 0)).FirstOrDefault();
        }

        private static bool SendClaimed(int id)
        {
            WhatsAppMessage message;
            using (var db = new SchoolDbContext())
                message = db.WhatsAppMessages.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (message == null) return false;
            bool sendMayHaveReachedProvider = false;
            try
            {
                bool replyNeedsConsent = message.IsReplyToIncoming &&
                    WhatsAppApiSettings.Load().RepliesRequireConsent;
                bool consentRequired = !message.IsReplyToIncoming || replyNeedsConsent;
                if (!message.IsInternalRecipient && (consentRequired || !message.ParentId.HasValue))
                {
                    bool consentStillValid = false;
                    if (message.ParentId.HasValue)
                    {
                        using (var db = new SchoolDbContext())
                        {
                            var consent = db.WhatsAppConsents.AsNoTracking().FirstOrDefault(x =>
                                x.ParentId == message.ParentId.Value && x.IsOptedIn);
                            string recipientPhone = WhatsAppCloudApi.NormalizeRecipientNumber(message.PhoneNumber);
                            consentStillValid = consent != null && recipientPhone.Length >= 10 &&
                                recipientPhone == WhatsAppCloudApi.NormalizeRecipientNumber(consent.PhoneNumber);
                        }
                    }
                    if (!consentStillValid)
                    {
                        using (var db = new SchoolDbContext())
                        {
                            var row = db.WhatsAppMessages.Find(id);
                            if (row != null)
                            {
                                row.Status = "Cancelled";
                                row.ClaimedAt = null;
                                row.NextAttemptAt = null;
                                row.ErrorDetails = replyNeedsConsent || !message.ParentId.HasValue
                                    ? "لا توجد موافقة فعّالة تسمح بالإرسال إلى هذا الرقم."
                                    : "أُلغيت الموافقة أو تغيّر رقم الهاتف قبل الإرسال.";
                                db.SaveChanges();
                                UpdateCampaignStatus(db, row.CampaignId);
                            }
                        }
                        return false;
                    }
                }
                if (!string.IsNullOrWhiteSpace(message.ProviderTemplateName))
                {
                    using (var db = new SchoolDbContext())
                    {
                        bool templateStillApproved = db.WhatsAppTemplates.Any(x => x.IsActive && x.IsMetaApproved &&
                            x.ProviderTemplateName == message.ProviderTemplateName &&
                            x.LanguageCode == message.TemplateLanguage);
                        if (!templateStillApproved)
                        {
                            var row = db.WhatsAppMessages.Find(id);
                            if (row != null)
                            {
                                row.Status = "Cancelled";
                                row.ClaimedAt = null;
                                row.NextAttemptAt = null;
                                row.ErrorDetails = "القالب لم يعد مفعّلاً أو معتمداً لدى Meta.";
                                db.SaveChanges();
                                UpdateCampaignStatus(db, row.CampaignId);
                            }
                            return false;
                        }
                    }
                }
                using (var db = new SchoolDbContext())
                {
                    int started = db.Database.ExecuteSqlCommand(@"
UPDATE dbo.WhatsAppMessages
   SET SendStartedAt = SYSUTCDATETIME()
 WHERE Id = @id AND Status = 'Sending' AND ClaimedAt IS NOT NULL
   AND SendStartedAt IS NULL;",
                        new SqlParameter("@id", id));
                    if (started != 1) return false;
                }
                sendMayHaveReachedProvider = true;
                string providerId = WhatsAppCloudApi.Send(message);
                if (string.IsNullOrWhiteSpace(providerId))
                    throw new InvalidOperationException("قبلت واجهة Meta الطلب دون إرجاع معرف رسالة.");
                using (var db = new SchoolDbContext())
                {
                    var row = db.WhatsAppMessages.Find(id);
                    if (row != null)
                    {
                        row.Status = "Sent"; // API accepted; final delivery status is set by webhook.
                        row.ProviderMessageId = providerId;
                        row.SentAt = DateTime.UtcNow;
                        row.ClaimedAt = null;
                        row.NextAttemptAt = null;
                        row.ErrorDetails = null;
                        db.SaveChanges();
                        UpdateCampaignStatus(db, row.CampaignId);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var sendError = ex as WhatsAppSendException;
                bool outcomeUnknown = sendError != null ? sendError.IsOutcomeUnknown : sendMayHaveReachedProvider;
                bool retryable = sendError != null && sendError.IsRetryable;
                using (var db = new SchoolDbContext())
                {
                    var row = db.WhatsAppMessages.Find(id);
                    if (row != null)
                    {
                        string details = string.IsNullOrWhiteSpace(ex.Message)
                            ? "فشل غير معروف." : ex.Message;
                        if (details.Length > 1500) details = details.Substring(0, 1500);
                        row.ClaimedAt = null;
                        if (outcomeUnknown)
                        {
                            row.Status = "Unknown";
                            row.NextAttemptAt = null;
                            row.ErrorDetails = "نتيجة الإرسال غير مؤكدة؛ لا تُعِد الإرسال قبل مراجعة سجل Meta. " + details;
                        }
                        else if (retryable && row.Attempts < MaxAttempts)
                        {
                            TimeSpan delay = RetryDelay(row.Attempts, sendError);
                            row.Status = "Queued";
                            row.NextAttemptAt = DateTime.UtcNow.Add(delay);
                            row.ErrorDetails = "محاولة " + row.Attempts +
                                " فشلت مؤقتاً؛ ستُعاد تلقائياً بعد " +
                                Math.Max(1, (int)Math.Round(delay.TotalMinutes)) + " دقيقة. " + details;
                        }
                        else
                        {
                            row.Status = "Failed";
                            row.NextAttemptAt = null;
                            row.ErrorDetails = details;
                        }
                        db.SaveChanges();
                        UpdateCampaignStatus(db, row.CampaignId);
                    }
                }
                return false;
            }
        }

        private static TimeSpan RetryDelay(int attempts, WhatsAppSendException sendError)
        {
            if (sendError != null && sendError.RetryAfter.HasValue)
            {
                TimeSpan requested = sendError.RetryAfter.Value;
                if (requested < TimeSpan.FromMinutes(1)) requested = TimeSpan.FromMinutes(1);
                if (requested > TimeSpan.FromMinutes(180)) requested = TimeSpan.FromMinutes(180);
                return requested;
            }
            int index = attempts - 1;
            if (index < 0) index = 0;
            if (index >= RetryDelays.Length) index = RetryDelays.Length - 1;
            return RetryDelays[index];
        }

        private static void UpdateCampaignStatus(SchoolDbContext db, int? campaignId)
        {
            if (!campaignId.HasValue) return;
            var campaign = db.WhatsAppCampaigns.Find(campaignId.Value);
            if (campaign == null) return;
            bool pending = db.WhatsAppMessages.Any(x => x.CampaignId == campaignId.Value &&
                (x.Status == "Queued" || x.Status == "Scheduled" || x.Status == "Sending"));
            if (!pending)
            {
                bool needsReview = db.WhatsAppMessages.Any(x =>
                    x.CampaignId == campaignId.Value && x.Status == "Unknown");
                campaign.Status = needsReview ? "NeedsReview" : "Completed";
                db.SaveChanges();
            }
        }
        public static List<WhatsAppMessage> GetPendingMessages()
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (!settings.ManualLinkMode && settings.IsConfigured)
                return new List<WhatsAppMessage>();

            using (var db = new SchoolDbContext())
            {
                var pending = db.WhatsAppMessages.AsNoTracking()
                    .Where(x => x.Direction == "Outbound" && (x.Status == "Queued" || x.Status == "Failed"))
                    .OrderBy(x => x.CreatedAt)
                    .ToList();
                if (pending.Count == 0) return pending;

                int[] parentIds = pending
                    .Where(x => !x.IsInternalRecipient && x.ParentId.HasValue)
                    .Select(x => x.ParentId.Value)
                    .Distinct()
                    .ToArray();
                var validConsents = parentIds.Length == 0
                    ? new Dictionary<int, WhatsAppConsent>()
                    : db.WhatsAppConsents.AsNoTracking()
                        .Where(x => x.IsOptedIn && parentIds.Contains(x.ParentId))
                        .ToList()
                        .ToDictionary(x => x.ParentId);

                return pending.Where(message =>
                {
                    if (message.IsInternalRecipient) return true;
                    if (!message.ParentId.HasValue) return false;
                    bool consentRequired = !message.IsReplyToIncoming || settings.RepliesRequireConsent;
                    if (!consentRequired) return true;

                    WhatsAppConsent consent;
                    if (!validConsents.TryGetValue(message.ParentId.Value, out consent)) return false;
                    string recipientPhone = WhatsAppCloudApi.NormalizeRecipientNumber(message.PhoneNumber);
                    return recipientPhone.Length >= 10 &&
                        recipientPhone == WhatsAppCloudApi.NormalizeRecipientNumber(consent.PhoneNumber);
                }).ToList();
            }
        }

        public static bool MarkAsSent(int messageId)
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (!settings.ManualLinkMode && settings.IsConfigured) return false;

            using (var db = new SchoolDbContext())
            using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var message = db.WhatsAppMessages.Find(messageId);
                if (message == null || message.Direction != "Outbound" ||
                    (message.Status != "Queued" && message.Status != "Failed") ||
                    message.SendStartedAt.HasValue || !string.IsNullOrEmpty(message.ProviderMessageId))
                    return false;

                if (!message.IsInternalRecipient)
                {
                    if (!message.ParentId.HasValue) return false;
                    bool consentRequired = !message.IsReplyToIncoming || settings.RepliesRequireConsent;
                    if (consentRequired)
                    {
                        var consent = db.WhatsAppConsents.AsNoTracking()
                            .FirstOrDefault(x => x.ParentId == message.ParentId.Value && x.IsOptedIn);
                        string recipientPhone = WhatsAppCloudApi.NormalizeRecipientNumber(message.PhoneNumber);
                        if (consent == null || recipientPhone.Length < 10 ||
                            recipientPhone != WhatsAppCloudApi.NormalizeRecipientNumber(consent.PhoneNumber))
                            return false;
                    }
                }

                message.Status = "Sent";
                message.SentAt = DateTime.UtcNow;
                message.ClaimedAt = null;
                message.NextAttemptAt = null;
                message.ErrorDetails = null;
                db.SaveChanges();
                transaction.Commit();
                return true;
            }
        }
    }
}
