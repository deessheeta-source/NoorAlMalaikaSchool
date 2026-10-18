using System;
using System.Data.SqlClient;
using System.Linq;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>Claims queued messages atomically so overlapping IIS workers cannot send duplicates.</summary>
    public static class WhatsAppQueueService
    {
        public static bool TrySendNow(int messageId, bool allowFailedRetry)
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (settings.ManualLinkMode || !settings.IsConfigured) return false;
            using (var db = new SchoolDbContext())
            {
                string statusFilter = allowFailedRetry
                    ? "Status IN ('Queued','Failed')"
                    : "Status = 'Queued'";
                int claimedId = Claim(db, messageId, statusFilter);
                if (claimedId <= 0) return false;
            }

            SendClaimed(messageId);
            return true;
        }

        public static int DispatchDue(int maximumMessages)
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (settings.ManualLinkMode || !settings.IsConfigured) return 0;
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
    FROM dbo.WhatsAppMessages WITH (UPDLOCK, READPAST, ROWLOCK)
    WHERE Direction = 'Outbound'
      AND Status IN ('Queued','Scheduled')
      AND (ScheduledAt IS NULL OR ScheduledAt <= SYSUTCDATETIME())
    ORDER BY ISNULL(ScheduledAt, CreatedAt), Id
)
UPDATE m
   SET Status = 'Sending', Attempts = Attempts + 1
OUTPUT inserted.Id
  FROM dbo.WhatsAppMessages AS m
  JOIN nextMessage AS n ON n.Id = m.Id;").FirstOrDefault();
                }
                if (id <= 0) break;
                if (SendClaimed(id)) sentToProvider++;
            }
            return sentToProvider;
        }

        private static int Claim(SchoolDbContext db, int messageId, string statusFilter)
        {
            return db.Database.SqlQuery<int>(@"
UPDATE dbo.WhatsAppMessages
   SET Status = 'Sending', Attempts = Attempts + 1, ProviderMessageId = NULL, ErrorDetails = NULL
OUTPUT inserted.Id
 WHERE Id = @id AND Direction = 'Outbound' AND " + statusFilter + @";",
                new SqlParameter("@id", messageId)).FirstOrDefault();
        }

        private static bool SendClaimed(int id)
        {
            WhatsAppMessage message;
            using (var db = new SchoolDbContext())
                message = db.WhatsAppMessages.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (message == null) return false;
            try
            {
                bool replyNeedsConsent = message.IsReplyToIncoming &&
                    WhatsAppApiSettings.Load().RepliesRequireConsent;
                if ((!message.IsReplyToIncoming || replyNeedsConsent) && message.ParentId.HasValue)
                {
                    using (var db = new SchoolDbContext())
                    {
                        bool consentStillValid = db.WhatsAppConsents.Any(x =>
                            x.ParentId == message.ParentId.Value && x.IsOptedIn &&
                            x.PhoneNumber == message.PhoneNumber);
                        if (!consentStillValid)
                        {
                            var row = db.WhatsAppMessages.Find(id);
                            if (row != null)
                            {
                                row.Status = "Cancelled";
                                row.ErrorDetails = replyNeedsConsent
                                    ? "لا توجد موافقة فعّالة تسمح بالرد على هذا الرقم."
                                    : "أُلغيت الموافقة أو تغيّر رقم الهاتف قبل الإرسال.";
                                db.SaveChanges();
                                UpdateCampaignStatus(db, row.CampaignId);
                            }
                            return false;
                        }
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
                                row.ErrorDetails = "القالب لم يعد مفعّلاً أو معتمداً لدى Meta.";
                                db.SaveChanges();
                                UpdateCampaignStatus(db, row.CampaignId);
                            }
                            return false;
                        }
                    }
                }
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
                        row.ErrorDetails = null;
                        db.SaveChanges();
                        UpdateCampaignStatus(db, row.CampaignId);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                using (var db = new SchoolDbContext())
                {
                    var row = db.WhatsAppMessages.Find(id);
                    if (row != null)
                    {
                        row.Status = "Failed";
                        row.ErrorDetails = ex.Message.Length > 1500 ? ex.Message.Substring(0, 1500) : ex.Message;
                        db.SaveChanges();
                        UpdateCampaignStatus(db, row.CampaignId);
                    }
                }
                return false;
            }
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
                campaign.Status = "Completed";
                db.SaveChanges();
            }
        }
    }
}
