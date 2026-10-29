using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.WhatsApp
{
    public partial class Default : Page
    {
        private void BindOutbox()
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            int recovered = RecoverStuckSending();
            if (recovered > 0)
                ShowNotice("أُعيدت " + recovered.ToString(CultureInfo.InvariantCulture) +
                    " رسالة كانت عالقة في حالة الإرسال إلى طابور الإرسال اليدوي.", false);
            string search = (txtOutboxSearch.Text ?? string.Empty).Trim();
            var query = db.WhatsAppMessages.AsNoTracking()
                .Where(x => x.Direction == "Outbound" && (x.Status == "Queued" || x.Status == "Scheduled"));
            if (search.Length > 0)
                query = query.Where(x => x.PhoneNumber.Contains(search) ||
                    x.RecipientName.Contains(search) || x.MessageBody.Contains(search));
            Dictionary<int, WhatsAppConsent> consents = LoadConsentMap();
            var source = query.OrderBy(x => x.ScheduledAt ?? x.CreatedAt).ThenBy(x => x.Id)
                .Take(OutboxRowLimit).ToList();
            var rows = new List<OutboxRow>();
            foreach (WhatsAppMessage message in source)
            {
                bool isBlocked;
                bool isScheduledFuture;
                string blockReason;
                ApplyOutboxGuard(message, consents, settings, out isBlocked, out isScheduledFuture, out blockReason);
                rows.Add(new OutboxRow
                {
                    Id = message.Id,
                    RecipientName = message.RecipientName,
                    PhoneNumber = WhatsAppCloudApi.NormalizeRecipientNumber(message.PhoneNumber),
                    MessagePreview = Preview(message.MessageBody),
                    CreatedAtLocal = message.CreatedAt.ToLocalTime(),
                    ScheduledAt = message.ScheduledAt.HasValue ? (DateTime?)message.ScheduledAt.Value.ToLocalTime() : null,
                    WhatsAppLink = isBlocked ? string.Empty : BuildWhatsAppLink(message.PhoneNumber, message.MessageBody),
                    IsReply = message.IsReplyToIncoming,
                    IsBlocked = isBlocked,
                    IsScheduledFuture = isScheduledFuture,
                    BlockReason = blockReason
                });
            }
            SetGridPage(gvOutbox, rows.Count, gvOutbox.PageIndex);
            gvOutbox.DataSource = rows;
            gvOutbox.DataBind();
            litOutboxHint.Text = "عدد الرسائل الجاهزة للإرسال اليدوي: " + rows.Count +
                (settings.ManualLinkMode
                    ? " — افتح كل رابط، أرسل من واتساب، ثم اضغط «تم الإرسال»."
                    : " — تنبيه: النظام في وضع Cloud API؛ استخدم الإرسال الآلي بدلاً من ذلك.");
        }

        protected void gvOutbox_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOutbox.PageIndex = e.NewPageIndex;
            BindOutbox();
        }

        protected void gvOutbox_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            var row = db.WhatsAppMessages.Find(id);
            if (row == null || row.Direction != "Outbound") return;
            if (e.CommandName == "sent")
            {
                if (row.Status != "Queued" && row.Status != "Scheduled") return;
                bool isBlocked;
                bool isScheduledFuture;
                string blockReason;
                ApplyOutboxGuard(row, LoadConsentMap(), WhatsAppApiSettings.Load(),
                    out isBlocked, out isScheduledFuture, out blockReason);
                if (isBlocked || isScheduledFuture)
                {
                    BindOutbox();
                    ShowNotice("لم تُسجَّل الرسالة: " + blockReason + ".", true);
                    return;
                }
                if (!WasOutboxLinkOpened(id))
                {
                    BindOutbox();
                    ShowNotice("لم تُسجَّل الرسالة: افتح رابط واتساب أولاً، أرسل الرسالة من واتساب، ثم اضغط «تسجيل كمُرسل».", true);
                    return;
                }
                row.Status = "Sent";
                row.SentAt = DateTime.UtcNow;
                row.ErrorDetails = null;
                db.SaveChanges();
                UpdateCampaignCompletion(row.CampaignId);
                BindOutbox();
                ShowNotice("عُلّمت الرسالة كمُرسلة.", false);
                return;
            }
            if (e.CommandName == "cancelitem")
            {
                if (row.Status != "Queued" && row.Status != "Scheduled") return;
                row.Status = "Cancelled";
                row.ErrorDetails = "ألغى المستخدم الرسالة في صندوق الإرسال اليدوي.";
                db.SaveChanges();
                UpdateCampaignCompletion(row.CampaignId);
                BindOutbox();
                ShowNotice("أُلغيت الرسالة.", false);
            }
        }

        private void UpdateCampaignCompletion(int? campaignId)
        {
            if (!campaignId.HasValue) return;
            bool pending = db.WhatsAppMessages.Any(x => x.CampaignId == campaignId.Value &&
                (x.Status == "Queued" || x.Status == "Scheduled" || x.Status == "Sending"));
            if (pending) return;
            var campaign = db.WhatsAppCampaigns.Find(campaignId.Value);
            if (campaign != null && campaign.Status != "Cancelled")
            {
                campaign.Status = "Completed";
                db.SaveChanges();
            }
        }

        private bool WasOutboxLinkOpened(int messageId)
        {
            string raw = hfOpenedOutboxIds == null ? null : hfOpenedOutboxIds.Value;
            if (string.IsNullOrWhiteSpace(raw)) return false;
            foreach (string token in raw.Split(','))
            {
                int openedId;
                if (int.TryParse(token.Trim(), out openedId) && openedId == messageId) return true;
            }
            return false;
        }

        private void ApplyOutboxGuard(WhatsAppMessage message, Dictionary<int, WhatsAppConsent> consents,
            WhatsAppApiSettings settings, out bool isBlocked, out bool isScheduledFuture, out string blockReason)
        {
            isBlocked = false;
            isScheduledFuture = false;
            blockReason = null;
            if (message == null) return;

            if (message.Status == "Scheduled" && message.ScheduledAt.HasValue &&
                message.ScheduledAt.Value > DateTime.UtcNow)
            {
                isScheduledFuture = true;
                blockReason = "مجدولة — لا تُرسل قبل موعدها";
            }

            if (message.ParentId.HasValue)
            {
                bool replyNeedsConsent = message.IsReplyToIncoming && settings.RepliesRequireConsent;
                if (!message.IsReplyToIncoming || replyNeedsConsent)
                {
                    if (!IsConsentValid(consents, message.ParentId.Value, message.PhoneNumber))
                    {
                        isBlocked = true;
                        blockReason = "محجوبة — لا موافقة فعّالة";
                    }
                }
            }
        }

        private int RecoverStuckSending()
        {
            return WhatsAppQueueService.RecoverStuckSending(10);
        }

        private static string Preview(string body)
        {
            string value = (body ?? string.Empty).Replace("\r", " ").Replace("\n", " ").Trim();
            return value.Length <= 160 ? value : value.Substring(0, 160) + "…";
        }

        private static string BuildWhatsAppLink(string phone, string body)
        {
            string number = WhatsAppCloudApi.NormalizeRecipientNumber(phone);
            if (number.Length < 8) return string.Empty;
            return "https://wa.me/" + number + "?text=" +
                Uri.EscapeDataString(WhatsAppMessageFormatter.EnsureSchoolHeader(body));
        }

        protected void LogSearch_TextChanged(object sender, EventArgs e)
        {
            gvLog.PageIndex = 0;
            BindLog();
        }

        protected void ClearLogSearch_Click(object sender, EventArgs e)
        {
            txtLogSearch.Text = string.Empty;
            gvLog.PageIndex = 0;
            BindLog();
        }

        private void BindLog()
        {
            string status = ddlLogStatus.SelectedValue;
            string direction = ddlLogDirection.SelectedValue;
            string search = (txtLogSearch.Text ?? string.Empty).Trim();
            bool manual = WhatsAppApiSettings.Load().ManualLinkMode;
            var query = db.WhatsAppMessages.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.Status == status);
            if (!string.IsNullOrWhiteSpace(direction)) query = query.Where(x => x.Direction == direction);
            if (search.Length > 0)
                query = query.Where(x => x.PhoneNumber.Contains(search) ||
                    x.RecipientName.Contains(search) || x.MessageBody.Contains(search));
            var rows = query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                .Take(LogRowLimit).ToList()
                .Select(x => new LogRow
                {
                    Id = x.Id,
                    CreatedAtLocal = x.CreatedAt.ToLocalTime(),
                    RecipientName = x.RecipientName,
                    PhoneNumber = x.PhoneNumber,
                    MessageBody = x.MessageBody,
                    Status = x.Status,
                    ErrorDetails = x.ErrorDetails,
                    CreatedBy = x.CreatedBy,
                    CanRetry = x.Status == "Failed",
                    CanDelete = x.Status != "Sending",
                    CanCancel = x.Status == "Queued" || x.Status == "Scheduled",
                    CanResend = x.Status == "Failed" || x.Status == "Cancelled",
                    CanRequeue = manual && x.Direction == "Outbound" &&
                        (x.Status == "Sent" || x.Status == "Delivered" || x.Status == "Read"),
                    WhatsAppLink = x.Direction == "Outbound" ? BuildWhatsAppLink(x.PhoneNumber, x.MessageBody) : string.Empty,
                    DirectionText = x.Direction == "Outbound" ? "صادر" : "وارد"
                }).ToList();
            SetGridPage(gvLog, rows.Count, gvLog.PageIndex);
            gvLog.DataSource = rows;
            gvLog.DataBind();
            BindLogStats();
        }

        private void BindLogStats()
        {
            try
            {
                DateTime from = DateTime.UtcNow.AddDays(-30);
                var counts = db.WhatsAppMessages.AsNoTracking()
                    .Where(x => x.CreatedAt >= from && x.Direction == "Outbound")
                    .GroupBy(x => x.Status)
                    .Select(g => new StatusCountItem { Status = g.Key, Total = g.Count() }).ToList();
                int pending = StatusCount(counts, "Queued") + StatusCount(counts, "Scheduled") + StatusCount(counts, "Sending");
                litLogStats.Text = "آخر 30 يوماً — مقبولة: " + StatusCount(counts, "Sent") +
                    " · مُسلّمة: " + StatusCount(counts, "Delivered") +
                    " · مقروءة: " + StatusCount(counts, "Read") +
                    " · فاشلة: " + StatusCount(counts, "Failed") +
                    " · غير مؤكدة: " + StatusCount(counts, "Unknown") +
                    " · ملغاة: " + StatusCount(counts, "Cancelled") +
                    " · في الانتظار: " + pending;
            }
            catch (Exception ex)
            {
                litLogStats.Text = "تعذر حساب الإحصاءات.";
                System.Diagnostics.Trace.TraceWarning("WhatsApp log stats failed: " + ex.Message);
            }
        }

        private static int StatusCount(List<StatusCountItem> counts, string status)
        {
            StatusCountItem item = counts.FirstOrDefault(x => x.Status == status);
            return item == null ? 0 : item.Total;
        }

        protected void ddlLogStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvLog.PageIndex = 0;
            BindLog();
        }

        protected void ddlLogDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvLog.PageIndex = 0;
            BindLog();
        }

        protected void gvLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLog.PageIndex = e.NewPageIndex;
            BindLog();
        }

        protected void gvLog_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            if (e.CommandName == "deleteitem")
            {
                bool exists = db.WhatsAppMessages.AsNoTracking().Any(x => x.Id == id);
                if (!exists)
                {
                    BindLog();
                    ShowNotice("لم يعد سجل الرسالة موجوداً.", true);
                    return;
                }

                db.Database.ExecuteSqlCommand(
                    "DELETE FROM dbo.WhatsAppMessages WHERE Id = @p0 AND Status <> @p1", id, "Sending");
                bool remains = db.WhatsAppMessages.AsNoTracking().Any(x => x.Id == id);
                BindLog();
                ShowNotice(remains
                    ? "تعذر حذف الرسالة لأنها قيد الإرسال. حدّث السجل وحاول بعد ظهور نتيجتها."
                    : "حُذف سجل الرسالة نهائياً.", remains);
                return;
            }
            if (e.CommandName == "retry")
            {
                if (WhatsAppApiSettings.Load().ManualLinkMode)
                {
                    var manualRow = db.WhatsAppMessages.Find(id);
                    if (manualRow == null) return;
                    manualRow.Status = "Queued";
                    manualRow.ErrorDetails = null;
                    db.SaveChanges();
                    BindLog();
                    ShowNotice("أُعيدت الرسالة إلى صندوق الإرسال اليدوي.", false);
                    return;
                }
                bool queued = WhatsAppQueueService.TrySendNow(id, true);
                if (!queued)
                    ShowNotice("تعذر بدء الإعادة. تحقق من إعداد API ومن موافقة ولي الأمر.", true);
                else ShowNotice("بدأت إعادة المحاولة. ستظهر النتيجة النهائية في السجل.", false);
                BindLog();
                return;
            }
            if (e.CommandName == "cancelitem")
            {
                var message = db.WhatsAppMessages.Find(id);
                if (message == null || (message.Status != "Queued" && message.Status != "Scheduled")) return;
                message.Status = "Cancelled";
                message.ErrorDetails = "ألغى المستخدم الرسالة قبل الإرسال.";
                db.SaveChanges();
                BindLog();
                ShowNotice("أُلغيت الرسالة.", false);
                return;
            }
            if (e.CommandName == "requeue")
            {
                if (!WhatsAppApiSettings.Load().ManualLinkMode)
                {
                    ShowNotice("الإعادة إلى الصندوق متاحة في وضع الإرسال اليدوي فقط.", true);
                    return;
                }
                var row = db.WhatsAppMessages.Find(id);
                if (row == null || row.Direction != "Outbound") return;
                if (row.Status != "Sent" && row.Status != "Delivered" && row.Status != "Read") return;
                row.Status = "Queued";
                row.SentAt = null;
                row.ProviderMessageId = null;
                row.ErrorDetails = null;
                if (row.CampaignId.HasValue)
                {
                    var campaign = db.WhatsAppCampaigns.Find(row.CampaignId.Value);
                    if (campaign != null && campaign.Status == "Completed") campaign.Status = "Queued";
                }
                db.SaveChanges();
                BindLog();
                ShowNotice("أُعيدت الرسالة إلى صندوق الإرسال اليدوي. افتحه لإرسالها من واتساب.", false);
                return;
            }
            if (e.CommandName == "resend")
            {
                var source = db.WhatsAppMessages.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (source == null) return;
                db.WhatsAppMessages.Add(new WhatsAppMessage
                {
                    CampaignId = source.CampaignId,
                    ParentId = source.ParentId,
                    StudentId = source.StudentId,
                    RecipientName = source.RecipientName,
                    PhoneNumber = source.PhoneNumber,
                    MessageBody = source.MessageBody,
                    Direction = "Outbound",
                    Status = "Queued",
                    IsReplyToIncoming = source.IsReplyToIncoming,
                    ProviderTemplateName = source.ProviderTemplateName,
                    TemplateLanguage = source.TemplateLanguage,
                    TemplateParametersJson = source.TemplateParametersJson,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = CurrentUserName()
                });
                db.SaveChanges();
                BindLog();
                ShowNotice("أُضيفت نسخة جديدة من الرسالة إلى طابور الإرسال.", false);
            }
        }

        protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            GridView grid = sender as GridView;
            if (grid == null || grid.HeaderRow == null) return;
            for (int i = 0; i < e.Row.Cells.Count && i < grid.HeaderRow.Cells.Count; i++)
            {
                string header = HttpUtility.HtmlDecode(grid.HeaderRow.Cells[i].Text ?? string.Empty);
                if (!string.IsNullOrWhiteSpace(header)) e.Row.Cells[i].Attributes["data-label"] = header;
            }
        }

        private static void SetGridPage(GridView grid, int rowCount, int requestedIndex)
        {
            int maxIndex = rowCount <= 0 ? 0 : (rowCount - 1) / GridPageSize;
            int index = requestedIndex < 0 ? 0 : requestedIndex;
            if (index > maxIndex) index = maxIndex;
            grid.PageIndex = index;
        }

        private sealed class ClassOption { public int Id { get; set; } public string Name { get; set; } public string Division { get; set; } }
        private sealed class Recipient { public Parent Parent { get; set; } public List<Student> Students { get; set; } }
        private sealed class ContactRow { public int ParentId { get; set; } public string ParentName { get; set; } public string StudentNames { get; set; } public string Phone { get; set; } public bool IsOptedIn { get; set; } public string ConsentState { get; set; } public string ConsentSource { get; set; } public string ConsentDate { get; set; } public string UpdatedBy { get; set; } }
        private sealed class DraftRow { public int Id { get; set; } public string Title { get; set; } public string ClassName { get; set; } public DateTime CreatedAt { get; set; } public DateTime? ScheduledAt { get; set; } public int RecipientCount { get; set; } public string Status { get; set; } }
        private sealed class LogRow { public int Id { get; set; } public DateTime CreatedAtLocal { get; set; } public string RecipientName { get; set; } public string PhoneNumber { get; set; } public string MessageBody { get; set; } public string Status { get; set; } public string ErrorDetails { get; set; } public string CreatedBy { get; set; } public bool CanRetry { get; set; } public bool CanDelete { get; set; } public bool CanCancel { get; set; } public bool CanResend { get; set; } public bool CanRequeue { get; set; } public string WhatsAppLink { get; set; } public string DirectionText { get; set; } }
        private sealed class OutboxRow { public int Id { get; set; } public string RecipientName { get; set; } public string PhoneNumber { get; set; } public string MessagePreview { get; set; } public DateTime CreatedAtLocal { get; set; } public DateTime? ScheduledAt { get; set; } public string WhatsAppLink { get; set; } public bool IsReply { get; set; } public bool IsBlocked { get; set; } public bool IsScheduledFuture { get; set; } public string BlockReason { get; set; } }
        private sealed class StatusCountItem { public string Status { get; set; } public int Total { get; set; } }
    }
}