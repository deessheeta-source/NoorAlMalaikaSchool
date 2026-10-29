using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Services;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Pages.WhatsApp
{
    public partial class WhatsAppDispatchPage : Page
    {
        protected global::System.Web.UI.WebControls.Literal lblPendingCount;
        protected global::System.Web.UI.WebControls.Literal lblDispatchResult;
        protected global::System.Web.UI.WebControls.Repeater rptMessages;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            var messages = WhatsAppQueueService.GetPendingMessages();
            rptMessages.DataSource = messages;
            rptMessages.DataBind();
            lblPendingCount.Text = messages.Count.ToString();
        }

        protected void rptMessages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "ConfirmSent") return;

            int messageId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out messageId))
            {
                lblDispatchResult.Text = "<div class='alert alert-warning'>تعذر تحديد الرسالة. حدّث الصفحة وحاول مجدداً.</div>";
                BindData();
                return;
            }

            try
            {
                if (WhatsAppQueueService.MarkAsSent(messageId))
                    lblDispatchResult.Text = "<div class='alert alert-success'>تم تسجيل تأكيد الإرسال اليدوي.</div>";
                else
                    lblDispatchResult.Text = "<div class='alert alert-warning'>لم تُسجّل الرسالة: ربما تغيّرت حالتها أو لم تعد هناك موافقة سارية.</div>";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp manual confirmation failed: " + ex);
                lblDispatchResult.Text = "<div class='alert alert-danger'>تعذر تسجيل الإرسال. راجع سجل النظام ثم حدّث الصفحة.</div>";
            }

            BindData();
        }

        protected string GetWhatsAppLink(object phone, object message)
        {
            if (phone == null || message == null) return "#";
            string encodedMessage = Uri.EscapeDataString(Convert.ToString(message));
            string cleanPhone = WhatsAppCloudApi.NormalizeRecipientNumber(Convert.ToString(phone));
            return cleanPhone.Length >= 8
                ? "https://wa.me/" + cleanPhone + "?text=" + encodedMessage
                : "#";
        }
    }
}