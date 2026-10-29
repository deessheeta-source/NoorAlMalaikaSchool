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
        private void BindSettings()
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            string managerPhone = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_MANAGER_PHONE", "WhatsAppManagerPhone");
            string countryCode = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_DEFAULT_COUNTRY_CODE", "WhatsAppDefaultCountryCode");

            txtSettingsManagerPhone.Text = managerPhone;
            txtSettingsCountryCode.Text = string.IsNullOrWhiteSpace(countryCode) ? "218" : countryCode;
            chkSettingsReplyConsent.Checked = settings.RepliesRequireConsent;
            chkSettingsManualLinks.Checked = settings.ManualLinks;
            litSettingsSecretStatus.Text = BuildSecretStatusHtml();
        }

        private string BuildSecretStatusHtml()
        {
            var items = new[]
            {
                new { Label = "رمز الوصول (Access Token)", Environment = "NOOR_WHATSAPP_ACCESS_TOKEN", AppSetting = "WhatsAppAccessToken" },
                new { Label = "معرّف رقم واتساب (Phone Number ID)", Environment = "NOOR_WHATSAPP_PHONE_NUMBER_ID", AppSetting = "WhatsAppPhoneNumberId" },
                new { Label = "إصدار Graph API", Environment = "NOOR_WHATSAPP_GRAPH_VERSION", AppSetting = "WhatsAppGraphVersion" },
                new { Label = "App Secret (توقيع Webhook)", Environment = "NOOR_WHATSAPP_APP_SECRET", AppSetting = "WhatsAppAppSecret" },
                new { Label = "Webhook Verify Token", Environment = "NOOR_WHATSAPP_WEBHOOK_VERIFY_TOKEN", AppSetting = "WhatsAppWebhookVerifyToken" },
                new { Label = "Dispatch Key", Environment = "NOOR_WHATSAPP_DISPATCH_KEY", AppSetting = "WhatsAppDispatchKey" },
                new { Label = "Dispatch URL", Environment = "NOOR_WHATSAPP_DISPATCH_URL", AppSetting = "WhatsAppDispatchUrl" }
            };

            var sb = new StringBuilder();
            foreach (var item in items)
            {
                bool configured = !string.IsNullOrWhiteSpace(
                    WhatsAppApiSettings.ReadSetting(item.Environment, item.AppSetting));
                sb.Append("<div style=\"margin:2px 0;\"><span class=\"wa-badge ")
                  .Append(configured ? "is-ok" : "is-muted")
                  .Append("\">").Append(configured ? "مهيأ" : "غير مهيأ")
                  .Append("</span> ").Append(HttpUtility.HtmlEncode(item.Label))
                  .Append("</div>");
            }
            return sb.ToString();
        }

        protected void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (!IsAdmin)
            {
                ShowNotice("إعدادات الوحدة متاحة لمدير النظام فقط.", true);
                return;
            }

            string countryCode = (txtSettingsCountryCode.Text ?? string.Empty).Trim();
            if (!Regex.IsMatch(countryCode, @"^[0-9]{1,4}$"))
            {
                ShowNotice("رمز الدولة يجب أن يكون من 1 إلى 4 أرقام، مثال: 218.", true);
                return;
            }

            string normalizedPhone;
            string error;
            if (!TryNormalizeManagerPhone(txtSettingsManagerPhone.Text, countryCode, out normalizedPhone, out error))
            {
                ShowNotice(error, true);
                return;
            }

            try
            {
                string user = CurrentUserName();
                SystemSettingsService.Set("WhatsAppManagerPhone", normalizedPhone, user);
                SystemSettingsService.Set("WhatsAppDefaultCountryCode", countryCode, user);
                SystemSettingsService.Set("WhatsAppReplyRequireConsent", chkSettingsReplyConsent.Checked ? "true" : "false", user);
                SystemSettingsService.Set("WhatsAppManualLinks", chkSettingsManualLinks.Checked ? "true" : "false", user);

                BindSettings();
                SetConnectionState();
                litManagerPhone.Text = normalizedPhone;
                litReplyPolicy.Text = chkSettingsReplyConsent.Checked
                    ? "سياسة المدرسة الحالية: الرد يتطلب موافقة فعّالة مسجلة لولي الأمر."
                    : "سياسة المدرسة الحالية: الرد الحر متاح ضمن نافذة خدمة Meta.";
                ShowNotice("حُفظت إعدادات واتساب. ستُستخدم القيم الجديدة فوراً.", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp settings save failed: " + ex);
                ShowNotice("تعذر حفظ الإعدادات. تأكد من تطبيق ترحيل جدول SystemSettings.", true);
            }
        }

        protected void btnResetSettings_Click(object sender, EventArgs e)
        {
            if (!IsAdmin)
            {
                ShowNotice("إعدادات الوحدة متاحة لمدير النظام فقط.", true);
                return;
            }

            try
            {
                foreach (string key in new[]
                {
                    "WhatsAppManagerPhone",
                    "WhatsAppDefaultCountryCode",
                    "WhatsAppReplyRequireConsent",
                    "WhatsAppManualLinks"
                })
                {
                    SystemSettingsService.Remove(key);
                }

                BindSettings();
                SetConnectionState();
                litManagerPhone.Text = WhatsAppApiSettings.ReadSetting(
                    "NOOR_WHATSAPP_MANAGER_PHONE", "WhatsAppManagerPhone");
                litReplyPolicy.Text = WhatsAppApiSettings.Load().RepliesRequireConsent
                    ? "سياسة المدرسة الحالية: الرد يتطلب موافقة فعّالة مسجلة لولي الأمر."
                    : "سياسة المدرسة الحالية: الرد الحر متاح ضمن نافذة خدمة Meta.";
                ShowNotice("أُعيدت القيم الافتراضية من Web.config.", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp settings reset failed: " + ex);
                ShowNotice("تعذرت استعادة القيم. راجع سجل أخطاء التطبيق.", true);
            }
        }

        private static bool TryNormalizeManagerPhone(string input, string countryCode,
            out string normalized, out string error)
        {
            normalized = string.Empty;
            error = null;

            string digits = new string((input ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length == 0)
            {
                error = "أدخل رقم المدير.";
                return false;
            }

            if (digits.StartsWith("00", StringComparison.Ordinal))
                digits = digits.Substring(2);

            string cc = new string((countryCode ?? string.Empty).Where(char.IsDigit).ToArray());
            if (cc.Length == 0) cc = "218";

            if (!digits.StartsWith(cc, StringComparison.Ordinal))
            {
                if (digits.StartsWith("0", StringComparison.Ordinal))
                    digits = digits.Substring(1);
                digits = cc + digits;
            }

            if (digits.Length < 10 || digits.Length > 15)
            {
                error = "رقم المدير غير صالح. أدخل رقماً دولياً مثل +218944684046 أو محلياً مثل 0944684046.";
                return false;
            }

            normalized = "+" + digits;
            return true;
        }

        private void UpdateRecipientSummary()
        {
            string audience = ddlAudienceType.SelectedValue;
            pnlIndividualRecipient.Visible = audience == "Individual";
            int classId;
            int? targetClass = null;
            if (audience != "School")
            {
                if (!int.TryParse(ddlClass.SelectedValue, out classId))
                {
                    if (audience == "Class" && ddlComposeGrade.SelectedValue != "")
                    {
                        // Valid, but targetClass is null
                    }
                    else
                    {
                        litRecipientSummary.Text = "اختر الفصل أو الشعبة لعرض عدد أولياء الأمور المستعدين للإرسال.";
                        return;
                    }
                }
                else
                {
                    targetClass = classId;
                }
            }
            int studentId;
            int? targetStudent = audience == "Individual" &&
                int.TryParse(ddlStudent.SelectedValue, out studentId) ? (int?)studentId : null;
            if (audience == "Individual" && !targetStudent.HasValue)
            {
                litRecipientSummary.Text = "اختر الطالب. لا يُدرج ولي الأمر إلا إذا كان رقمه وموافقته صالحين.";
                return;
            }
            List<Recipient> eligibleRecipients = LoadEligibleRecipients(targetClass, targetStudent);
            if (audience == "Individual")
                litRecipientSummary.Text = eligibleRecipients.Count == 0
                    ? "ولي الأمر غير مهيأ للإرسال أو لا توجد موافقة فعّالة."
                    : "مستلم واحد جاهز للإرسال.";
            else if (audience == "School")
                litRecipientSummary.Text = "إرسال لكل المدرسة (النطاق: " + StageLabel() +
                    ") — أولياء لديهم رقم وموافقة فعّالة: " + eligibleRecipients.Count;
            else
                litRecipientSummary.Text = "أولياء لديهم رقم وموافقة فعّالة: " + eligibleRecipients.Count;
        }

        private string StageLabel()
        {
            EducationStage? stage = ResolveStage();
            return stage.HasValue ? stage.Value.ToString() : "كل المراحل";
        }

        private void BindDailyPreview()
        {
            int classId;
            if (!int.TryParse(ddlClass.SelectedValue, out classId) || ddlAudienceType.SelectedValue == "School")
            {
                pnlDailyLessons.Visible = false;
                return;
            }
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            var lessons = db.DailyLessons.AsNoTracking().Include(x => x.Subject).Include(x => x.Teacher)
                .Where(x => x.ClassRoomId == classId && x.Date >= today && x.Date < tomorrow && x.Status == LessonStatus.Approved)
                .OrderBy(x => x.PeriodNumber).ToList();
            pnlDailyLessons.Visible = chkAppendDailyLessons.Checked;
            litDailyLessons.Text = lessons.Count == 0
                ? "لا يوجد جدول مسجل لهذا الفصل اليوم."
                : string.Join("؛ ", lessons.Select(x => x.PeriodNumber + ". " +
                    (x.Subject == null ? "حصة" : x.Subject.Name) +
                    (x.Teacher == null ? string.Empty : " — " + x.Teacher.Name)));
        }

        protected void ddlClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindStudentSelector();
            UpdateRecipientSummary();
            BindDailyPreview();
        }

        protected void ddlAudienceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAudienceVisibility();
            UpdateRecipientSummary();
            BindDailyPreview();
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRecipientSummary();
        }

        private void SetAudienceVisibility()
        {
            if (pnlIndividualRecipient != null && ddlAudienceType != null)
                pnlIndividualRecipient.Visible = ddlAudienceType.SelectedValue == "Individual";
        }

        private void BindStudentSelector()
        {
            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("— اختر الطالب —", string.Empty));

            var query = db.Students.AsNoTracking().Include(x => x.Parent).Include(x => x.ClassRoom)
                .Where(x => x.IsActive && x.ParentId.HasValue);

            int classId;
            if (int.TryParse(ddlClass.SelectedValue, out classId))
            {
                query = query.Where(x => x.ClassRoomId == classId);
            }
            else
            {
                int gradeId;
                if (int.TryParse(ddlComposeGrade.SelectedValue, out gradeId))
                {
                    query = query.Where(x => x.ClassRoom.GradeLevelId == gradeId);
                    if (!string.IsNullOrEmpty(ddlComposeDivision.SelectedValue))
                    {
                        query = query.Where(x => x.ClassRoom.DivisionName == ddlComposeDivision.SelectedValue);
                    }
                }
                else
                {
                    return;
                }
            }

            var students = query.ToList()
                .Where(x => x.Parent != null && !string.IsNullOrWhiteSpace(x.Parent.Phone))
                .OrderBy(x => x.Name).ThenBy(x => x.Parent.Name).ToList();

            // خيار مستقل لكل طالب، مع إظهار اسم الطالب قبل ولي الأمر.
            foreach (Student student in students)
            {
                Parent parent = student.Parent;
                string parentName = string.IsNullOrWhiteSpace(parent.Name) ? "ولي أمر غير مسجل" : parent.Name;
                string studentName = string.IsNullOrWhiteSpace(student.Name) ? "اسم الطالب غير مسجل" : student.Name;
                string displayText = studentName + " — ولي الأمر: " + parentName;

                ListItem item = new ListItem(displayText, student.Id.ToString(CultureInfo.InvariantCulture));
                item.Attributes.Add("title", displayText);
                ddlStudent.Items.Add(item);
            }
        }
        private void ApplyComposeQuerySelection()
        {
            if (CurrentView != "compose") return;

            string audience = Request.QueryString["audience"];
            if (!string.IsNullOrWhiteSpace(audience) &&
                ddlAudienceType.Items.FindByValue(audience) != null)
            {
                ddlAudienceType.SelectedValue = audience;
            }

            int studentId;
            bool hasStudent = int.TryParse(Request.QueryString["studentId"], out studentId);
            int classId;
            bool hasClass = int.TryParse(Request.QueryString["classId"], out classId);

            if (!hasClass && hasStudent)
            {
                var student = db.Students.AsNoTracking()
                    .Where(x => x.Id == studentId)
                    .Select(x => new { x.ClassRoomId })
                    .FirstOrDefault();
                if (student != null)
                {
                    classId = student.ClassRoomId;
                    hasClass = true;
                }
            }

            if (hasClass)
            {
                var room = db.ClassRooms.AsNoTracking().Include(x => x.GradeLevel).FirstOrDefault(x => x.Id == classId);
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
                    ListItem classItem = ddlClass.Items.FindByValue(classId.ToString(CultureInfo.InvariantCulture));
                    if (classItem != null) ddlClass.SelectedValue = classItem.Value;
                }
            }

            if (hasStudent)
            {
                ListItem studentItem = ddlStudent.Items.FindByValue(
                    studentId.ToString(CultureInfo.InvariantCulture));
                if (studentItem != null) ddlStudent.SelectedValue = studentItem.Value;
            }

            SetAudienceVisibility();
        }

        protected void chkAppendDailyLessons_CheckedChanged(object sender, EventArgs e)
        {
            BindDailyPreview();
        }

        protected void chkIncludeAnnouncement_CheckedChanged(object sender, EventArgs e)
        {
            SetAnnouncementVisibility();
        }

        private void SetAnnouncementVisibility()
        {
            if (pnlAnnouncement != null && chkIncludeAnnouncement != null)
                pnlAnnouncement.Visible = chkIncludeAnnouncement.Checked;
        }

        protected void ddlMessageKind_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { EnforceFinancialPermission(); }
            catch (UnauthorizedAccessException ex) { ShowNotice(ex.Message, true); }
        }

        protected void ddlTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(ddlTemplate.SelectedValue, out id))
            {
                var template = db.WhatsAppTemplates.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive);
                if (template != null)
                {
                    txtMessage.Text = template.Body;
                    SelectKindFromCategory(template.Category);
                }
            }
        }

        protected void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                string result = WhatsAppCloudApi.TestConnection();
                ShowNotice("نجح الاتصال بواجهة Meta. " + result, false);
            }
            catch (Exception ex)
            {
                ShowNotice("فشل الاتصال: " + ex.Message, true);
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            SaveCampaign("Draft", null, false);
        }

        protected void btnSendNow_Click(object sender, EventArgs e)
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (!settings.ManualLinkMode && !settings.IsConfigured)
            {
                ShowNotice("لم تُرسل أي رسالة: إعداد Cloud API غير مكتمل. جهّز بيانات الربط أولاً أو استخدم وضع الإرسال اليدوي.", true);
                return;
            }
            SaveCampaign("Queued", null, true);
        }

        protected void btnSchedule_Click(object sender, EventArgs e)
        {
            DateTime scheduledAt;
            if (!TryParseLocalSchedule(txtScheduledAt.Text, out scheduledAt) || scheduledAt <= DateTime.Now.AddMinutes(1))
            {
                ShowNotice("حدد موعداً مستقبلياً صحيحاً بعد دقيقة على الأقل.", true);
                return;
            }
            SaveCampaign("Scheduled", scheduledAt.ToUniversalTime(), true);
        }

    }
}