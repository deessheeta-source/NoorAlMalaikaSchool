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
        private string CurrentUserName()
        {
            return Context != null && Context.User != null && Context.User.Identity != null
                ? Context.User.Identity.Name : "system";
        }

        private void SelectKindFromCategory(string category)
        {
            string value = "Announcement";
            if (category == "مالي") value = "Financial";
            else if (category == "حضور") value = "Attendance";
            else if (category == "أكاديمي") value = "Academic";
            else if (category == "إداري") value = "Administrative";
            if (ddlMessageKind.Items.FindByValue(value) != null) ddlMessageKind.SelectedValue = value;
        }

        protected void btnResetCompose_Click(object sender, EventArgs e)
        {
            txtMessage.Text = string.Empty;
            ddlTemplate.SelectedIndex = 0;
            ddlAudienceType.SelectedValue = "Class";
            if (ddlComposeGrade.Items.Count > 1)
            {
                ddlComposeGrade.SelectedIndex = 1;
                BindComposeDivisions();
            }
            chkIncludeBalance.Checked = false;
            chkIncludeAttendance.Checked = false;
            chkIncludeGrades.Checked = false;
            chkAppendDailyLessons.Checked = false;
            chkIncludeAnnouncement.Checked = false;
            txtAnnouncement.Text = string.Empty;
            ddlMessageKind.SelectedValue = "Announcement";
            Session.Remove(EditingDraftKey);
            SetAudienceVisibility();
            SetInitialDateTime();
            UpdateRecipientSummary();
            BindDailyPreview();
            SetAnnouncementVisibility();
        }

        protected void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            if (ddlTemplateCategory.SelectedValue == "مالي" &&
                SecurityHelper.CurrentRole != UserRole.Admin && SecurityHelper.CurrentRole != UserRole.Accountant)
            {
                ShowNotice("إنشاء قوالب مالية متاح لمدير النظام أو المحاسب فقط.", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTemplateName.Text) || string.IsNullOrWhiteSpace(txtTemplateBody.Text))
            {
                ShowNotice("اسم القالب ونصه مطلوبان.", true);
                return;
            }
            if (chkTemplateApproved.Checked && string.IsNullOrWhiteSpace(txtProviderTemplateName.Text))
            {
                ShowNotice("أدخل الاسم المطابق للقالب المعتمد لدى Meta.", true);
                return;
            }
            int id;
            WhatsAppTemplate row = int.TryParse(hdnTemplateId.Value, out id)
                ? db.WhatsAppTemplates.Find(id) : null;
            if (row == null)
            {
                row = new WhatsAppTemplate { CreatedAt = DateTime.UtcNow, CreatedBy = CurrentUserName() };
                db.WhatsAppTemplates.Add(row);
            }
            row.Name = txtTemplateName.Text.Trim();
            row.Category = ddlTemplateCategory.SelectedValue;
            row.Body = txtTemplateBody.Text.Trim();
            row.ProviderTemplateName = string.IsNullOrWhiteSpace(txtProviderTemplateName.Text)
                ? null : txtProviderTemplateName.Text.Trim();
            row.LanguageCode = string.IsNullOrWhiteSpace(txtTemplateLanguage.Text) ? "ar" : txtTemplateLanguage.Text.Trim();
            row.IsMetaApproved = chkTemplateApproved.Checked;
            row.IsActive = true;
            db.SaveChanges();
            ClearTemplateForm();
            BindTemplates();
            BindTemplateSelector();
            ShowNotice("حُفظ القالب. تذكر أن الاعتماد يجب أن يطابق قالب Meta حرفياً.", false);
        }

        private void BindTemplates()
        {
            var rows = db.WhatsAppTemplates.AsNoTracking().OrderBy(x => x.Category).ThenBy(x => x.Name).ToList();
            SetGridPage(gvTemplates, rows.Count, gvTemplates.PageIndex);
            gvTemplates.DataSource = rows;
            gvTemplates.DataBind();
        }

        protected void gvTemplates_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTemplates.PageIndex = e.NewPageIndex;
            BindTemplates();
        }

        protected void gvTemplates_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            var row = db.WhatsAppTemplates.Find(id);
            if (row == null) return;
            if (e.CommandName == "edititem")
            {
                hdnTemplateId.Value = row.Id.ToString(CultureInfo.InvariantCulture);
                txtTemplateName.Text = row.Name;
                ddlTemplateCategory.SelectedValue = row.Category;
                txtTemplateBody.Text = row.Body;
                txtProviderTemplateName.Text = row.ProviderTemplateName;
                txtTemplateLanguage.Text = row.LanguageCode;
                chkTemplateApproved.Checked = row.IsMetaApproved;
                return;
            }
            if (e.CommandName == "toggle")
            {
                row.IsActive = !row.IsActive;
                db.SaveChanges();
                BindTemplates();
                BindTemplateSelector();
            }
        }

        protected void btnClearTemplate_Click(object sender, EventArgs e)
        {
            ClearTemplateForm();
        }

        private void ClearTemplateForm()
        {
            hdnTemplateId.Value = string.Empty;
            txtTemplateName.Text = string.Empty;
            txtTemplateBody.Text = string.Empty;
            txtProviderTemplateName.Text = string.Empty;
            txtTemplateLanguage.Text = "ar";
            chkTemplateApproved.Checked = false;
            if (ddlTemplateCategory.Items.Count > 0) ddlTemplateCategory.SelectedIndex = 0;
        }

        protected void ddlConsentClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvContacts.PageIndex = 0;
            BindContacts();
        }

        protected void ConsentSearch_TextChanged(object sender, EventArgs e)
        {
            gvContacts.PageIndex = 0;
            BindContacts();
        }

        protected void ClearConsentSearch_Click(object sender, EventArgs e)
        {
            txtConsentSearch.Text = string.Empty;
            gvContacts.PageIndex = 0;
            BindContacts();
        }

        private List<ContactRow> LoadContactRows()
        {
            int classId;
            if (!int.TryParse(ddlConsentClass.SelectedValue, out classId)) return new List<ContactRow>();
            string search = (txtConsentSearch.Text ?? string.Empty).Trim();
            var students = db.Students.AsNoTracking().Include(x => x.Parent)
                .Where(x => x.ClassRoomId == classId && x.IsActive && x.ParentId.HasValue)
                .ToList().Where(x => x.Parent != null && !string.IsNullOrWhiteSpace(x.Parent.Phone)).ToList();
            var consentMap = LoadConsentMap();
            return students.GroupBy(x => x.ParentId.Value).Select(group =>
            {
                Student first = group.First();
                WhatsAppConsent consent;
                consentMap.TryGetValue(group.Key, out consent);
                bool hasConsent = consent != null && consent.IsOptedIn &&
                    WhatsAppCloudApi.NormalizeRecipientNumber(consent.PhoneNumber) ==
                    WhatsAppCloudApi.NormalizeRecipientNumber(first.Parent.Phone);
                string state = hasConsent ? "موافق" :
                    consent != null && consent.RevokedAt.HasValue ? "ألغى الموافقة" : "غير مسجل";
                return new ContactRow
                {
                    ParentId = group.Key,
                    ParentName = first.Parent.Name,
                    StudentNames = string.Join("، ", group.Select(x => x.Name).Distinct()),
                    Phone = first.Parent.Phone,
                    IsOptedIn = hasConsent,
                    ConsentState = state,
                    ConsentSource = consent == null ? "—" : (string.IsNullOrWhiteSpace(consent.ConsentSource) ? "—" : consent.ConsentSource),
                    ConsentDate = FormatConsentDate(consent),
                    UpdatedBy = consent == null || string.IsNullOrWhiteSpace(consent.UpdatedBy) ? "—" : consent.UpdatedBy
                };
            }).Where(r => search.Length == 0 ||
                    r.ParentName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.Phone.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.StudentNames.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
              .OrderBy(x => x.ParentName).ToList();
        }

        private static string FormatConsentDate(WhatsAppConsent consent)
        {
            if (consent == null) return "—";
            if (consent.RevokedAt.HasValue)
                return "أُلغي: " + consent.RevokedAt.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            if (consent.ConsentedAt.HasValue)
                return "وُثّق: " + consent.ConsentedAt.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            return "—";
        }

        private void BindContacts()
        {
            var rows = LoadContactRows();
            SetGridPage(gvContacts, rows.Count, gvContacts.PageIndex);
            gvContacts.DataSource = rows;
            gvContacts.DataBind();
        }

        protected void gvContacts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvContacts.PageIndex = e.NewPageIndex;
            BindContacts();
        }

        protected void gvContacts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "optin" && e.CommandName != "optout") return;
            int parentId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out parentId)) return;
            var parent = db.Parents.Find(parentId);
            if (parent == null || string.IsNullOrWhiteSpace(parent.Phone)) return;

            // التحقق الآن يعتمد فقط على تشييك التأكيد
            if (e.CommandName == "optin" && !chkConsentVerified.Checked)
            {
                ShowNotice("لتسجيل الموافقة، يجب تأكيد توثيقها أولاً بوضع علامة في المربع.", true);
                return;
            }

            var consent = db.WhatsAppConsents.FirstOrDefault(x => x.ParentId == parentId);
            if (consent == null)
            {
                consent = new WhatsAppConsent { ParentId = parentId };
                db.WhatsAppConsents.Add(consent);
            }

            // مصدر افتراضي يتم حفظه في قاعدة البيانات
            string source = "تسجيل إداري عبر النظام";

            if (e.CommandName == "optin")
            {
                consent.IsOptedIn = true;
                consent.ConsentedAt = DateTime.UtcNow;
                consent.RevokedAt = null;
                consent.RevokeReason = null;
                consent.ConsentSource = source;
            }
            else
            {
                consent.IsOptedIn = false;
                consent.ConsentedAt = null;
                consent.RevokedAt = DateTime.UtcNow;
                consent.RevokeReason = "إلغاء موافقة عبر النظام";
                consent.ConsentSource = "إلغاء موافقة عبر النظام";
            }

            consent.PhoneNumber = WhatsAppCloudApi.NormalizeRecipientNumber(parent.Phone);
            consent.UpdatedAt = DateTime.UtcNow;
            consent.UpdatedBy = CurrentUserName();

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp consent save failed: " + ex);
                ShowNotice("تعذر حفظ الموافقة. أعد المحاولة.", true);
                return;
            }

            BindContacts();
            ShowNotice(e.CommandName == "optin"
                ? "سُجلت الموافقة للرقم الحالي."
                : "أُلغيت الموافقة؛ ستُلغى الرسائل المجدولة لهذا الرقم عند المعالجة.", false);
        }

        private void BindDrafts()
        {
            var classNames = LoadClasses().ToDictionary(x => x.Id, x => x.Name);
            var campaigns = db.WhatsAppCampaigns.AsNoTracking()
                .Where(x => x.Status == "Draft" || x.Status == "Scheduled" || x.Status == "Queued")
                .OrderByDescending(x => x.CreatedAt).Take(200).ToList();
            List<int> ids = campaigns.Select(x => x.Id).ToList();
            var counts = new Dictionary<int, int>();
            if (ids.Count > 0)
            {
                var grouped = db.WhatsAppMessages.AsNoTracking()
                    .Where(m => m.CampaignId.HasValue && ids.Contains(m.CampaignId.Value))
                    .GroupBy(m => m.CampaignId.Value)
                    .Select(g => new { CampaignId = g.Key, Total = g.Count() }).ToList();
                foreach (var item in grouped) counts[item.CampaignId] = item.Total;
            }
            var rows = campaigns.Select(x => new DraftRow
            {
                Id = x.Id,
                Title = x.Title,
                ClassName = x.AudienceType == "School"
                    ? "كل المدرسة"
                    : x.ClassRoomId.HasValue && classNames.ContainsKey(x.ClassRoomId.Value) ? classNames[x.ClassRoomId.Value] : "—",
                CreatedAt = x.CreatedAt.ToLocalTime(),
                ScheduledAt = x.ScheduledAt.HasValue ? (DateTime?)x.ScheduledAt.Value.ToLocalTime() : null,
                RecipientCount = counts.ContainsKey(x.Id) ? counts[x.Id] : 0,
                Status = StatusText(x.Status)
            }).ToList();
            SetGridPage(gvDrafts, rows.Count, gvDrafts.PageIndex);
            gvDrafts.DataSource = rows;
            gvDrafts.DataBind();
        }

        protected void gvDrafts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDrafts.PageIndex = e.NewPageIndex;
            BindDrafts();
        }

        protected void gvDrafts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            if (e.CommandName == "cancelitem")
            {
                var campaign = db.WhatsAppCampaigns.Find(id);
                if (campaign == null || (campaign.Status != "Draft" && campaign.Status != "Queued" && campaign.Status != "Scheduled")) return;
                var queuedMessages = db.WhatsAppMessages.Where(x => x.CampaignId == id &&
                    (x.Status == "Queued" || x.Status == "Scheduled")).ToList();
                foreach (WhatsAppMessage message in queuedMessages)
                {
                    message.Status = "Cancelled";
                    message.ErrorDetails = "ألغى المستخدم الحملة قبل الإرسال.";
                }
                campaign.Status = "Cancelled";
                db.SaveChanges();
                BindDrafts();
                ShowNotice("أُلغيت الحملة والرسائل التي لم يبدأ إرسالها.", false);
                return;
            }
            if (e.CommandName != "open") return;
            Session[PendingDraftKey] = id;
            Response.Redirect(ViewUrl("compose"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void RestorePendingDraft()
        {
            object raw = Session[PendingDraftKey];
            int id;
            if (raw == null || !int.TryParse(Convert.ToString(raw), out id)) return;
            Session.Remove(PendingDraftKey);
            var campaign = db.WhatsAppCampaigns.AsNoTracking().FirstOrDefault(x => x.Id == id && x.Status == "Draft");
            if (campaign == null) return;
            txtMessage.Text = campaign.MessageBody;
            Session[EditingDraftKey] = campaign.Id;

            if (campaign.ClassRoomId.HasValue)
            {
                var room = db.ClassRooms.AsNoTracking().FirstOrDefault(x => x.Id == campaign.ClassRoomId.Value);
                if (room != null)
                {
                    ListItem gradeItem = ddlComposeGrade.Items.FindByValue(room.GradeLevelId.ToString(CultureInfo.InvariantCulture));
                    if (gradeItem != null) ddlComposeGrade.SelectedValue = gradeItem.Value;
                    BindComposeDivisions();
                    if (!string.IsNullOrEmpty(room.DivisionName))
                    {
                        ListItem divItem = ddlComposeDivision.Items.FindByValue(room.DivisionName);
                        if (divItem != null) ddlComposeDivision.SelectedValue = divItem.Value;
                    }
                    BindComposeSections();
                    ListItem classItem = ddlClass.Items.FindByValue(campaign.ClassRoomId.Value.ToString(CultureInfo.InvariantCulture));
                    if (classItem != null) ddlClass.SelectedValue = classItem.Value;
                }
            }
            else
            {
                if (ddlComposeGrade.Items.Count > 1) ddlComposeGrade.SelectedIndex = 1;
                BindComposeDivisions();
            }

            BindStudentSelector();

            if (campaign.TargetStudentId.HasValue && ddlStudent.Items.FindByValue(campaign.TargetStudentId.Value.ToString(CultureInfo.InvariantCulture)) != null)
                ddlStudent.SelectedValue = campaign.TargetStudentId.Value.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(campaign.AudienceType) &&
                ddlAudienceType.Items.FindByValue(campaign.AudienceType) != null)
                ddlAudienceType.SelectedValue = campaign.AudienceType;
            else
                ddlAudienceType.SelectedValue = campaign.TargetStudentId.HasValue ? "Individual" : "Class";
            if (ddlMessageKind.Items.FindByValue(campaign.MessageKind) != null)
                ddlMessageKind.SelectedValue = campaign.MessageKind;
            if (campaign.WhatsAppTemplateId.HasValue && ddlTemplate.Items.FindByValue(campaign.WhatsAppTemplateId.Value.ToString(CultureInfo.InvariantCulture)) != null)
                ddlTemplate.SelectedValue = campaign.WhatsAppTemplateId.Value.ToString(CultureInfo.InvariantCulture);
            chkIncludeBalance.Checked = campaign.IncludeBalance && CanManageFinancial;
            chkIncludeAttendance.Checked = campaign.IncludeAttendance;
            chkIncludeGrades.Checked = campaign.IncludeGrades;
            chkAppendDailyLessons.Checked = campaign.AppendDailyLessons;
            SetAudienceVisibility();
            SetVisibleView();
            pnlCompose.Visible = true;
            BindDailyPreview();
            ShowNotice("فُتحت المسودة للتعديل. راجع الفصل والموافقات قبل الإرسال.", false);
        }

        protected void OutboxSearch_TextChanged(object sender, EventArgs e)
        {
            gvOutbox.PageIndex = 0;
            BindOutbox();
        }

        protected void ClearOutboxSearch_Click(object sender, EventArgs e)
        {
            txtOutboxSearch.Text = string.Empty;
            gvOutbox.PageIndex = 0;
            BindOutbox();
        }

    }
}