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
        private const string FlashNoticeKey = "WhatsAppFlashNotice";
        private const string EditingDraftKey = "WhatsAppEditingDraftId";
        private const string PendingDraftKey = "WhatsAppPendingDraftId";
        private const int GridPageSize = 20;
        private const int LogRowLimit = 1000;
        private const int InboxRowLimit = 500;
        private const int OutboxRowLimit = 500;

        private readonly SchoolDbContext db = new SchoolDbContext();

        private bool IsAdmin { get { return SecurityHelper.CurrentRole == UserRole.Admin; } }
        private bool CanManageFinancial
        {
            get
            {
                return SecurityHelper.CurrentRole == UserRole.Admin ||
                       SecurityHelper.CurrentRole == UserRole.Accountant;
            }
        }

        private string CurrentView
        {
            get
            {
                string value = (Request.QueryString["view"] ?? "compose").Trim().ToLowerInvariant();
                return new[] { "compose", "groups", "drafts", "outbox", "inbox", "log", "templates", "consent" }.Contains(value)
                    ? value : "compose";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin, UserRole.Accountant, UserRole.Supervisor, UserRole.DataEntry);
            SetVisibleView();
            SetAudienceVisibility();
            SetConnectionState();
            pnlManagerSummary.Visible = IsAdmin;
            string managerPhone = WhatsAppApiSettings.ReadSetting("NOOR_WHATSAPP_MANAGER_PHONE", "WhatsAppManagerPhone");
            litManagerPhone.Text = string.IsNullOrWhiteSpace(managerPhone) ? "غير مهيأ" : managerPhone;
            pnlFinancialOption.Visible = CanManageFinancial;
            if (!CanManageFinancial)
            {
                ListItem financialKind = ddlMessageKind.Items.FindByValue("Financial");
                if (financialKind != null) ddlMessageKind.Items.Remove(financialKind);
                ListItem financialTemplateCategory = ddlTemplateCategory.Items.FindByValue("مالي");
                if (financialTemplateCategory != null) ddlTemplateCategory.Items.Remove(financialTemplateCategory);
            }
            litReplyPolicy.Text = WhatsAppApiSettings.Load().RepliesRequireConsent
                ? "سياسة المدرسة الحالية: الرد يتطلب موافقة فعّالة مسجلة لولي الأمر."
                : "سياسة المدرسة الحالية: الرد الحر متاح ضمن نافذة خدمة Meta.";
            if (!ModuleSchemaReady()) return;

            if (!IsPostBack)
            {
                ConsumeFlashNotice();
                SetSchedulerState();
                SetWebhookState();
                ClearStaleDraftEditingSession();
                BindClassSelectors();
                BindStudentSelector();
                BindTemplateSelector();
                BindReplyTemplates();
                SetInitialDateTime();
                RestorePendingDraft();
                BindCurrentView();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (pnlCompose.Visible)
            {
                string summary = litRecipientSummary.Text;
                btnSendNow.OnClientClick = "return waGuard(this, '" +
                    HttpUtility.JavaScriptStringEncode("هل تريد إرسال الرسالة الآن؟ " + summary) + "');";
                btnSchedule.OnClientClick =
                    "return waGuard(this, 'هل تريد جدولة إرسال هذه الرسالة؟');";
            }
            btnResetCompose.OnClientClick =
                "return waConfirm('هل تريد تفريغ الحقول؟ التغييرات غير المحفوظة ستُفقد.');";
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            db.Dispose();
        }

        public string ViewUrl(string view)
        {
            string stage = Request.QueryString["stage"];
            if (string.IsNullOrWhiteSpace(stage)) stage = Session["Stage"] as string;
            string url = ResolveUrl("~/Pages/WhatsApp/Default.aspx") + "?view=" + HttpUtility.UrlEncode(view);
            if (!string.IsNullOrWhiteSpace(stage)) url += "&stage=" + HttpUtility.UrlEncode(stage);
            return url;
        }

        public string TabCss(string view)
        {
            return CurrentView == view ? "active" : string.Empty;
        }

        public string StatusText(string status)
        {
            switch (status ?? string.Empty)
            {
                case "Queued": return "في طابور الإرسال";
                case "Draft": return "مسودة";
                case "Scheduled": return "مجدولة";
                case "Sending": return "قيد الإرسال";
                case "Sent": return "قبلتها Meta";
                case "Delivered": return "تم التسليم";
                case "Read": return "مقروءة";
                case "Failed": return "فشلت";
                case "Cancelled": return "أُلغيت";
                case "Received": return "واردة";
                default: return status ?? "—";
            }
        }

        public string StatusCss(string status)
        {
            if (status == "Failed" || status == "Cancelled") return "is-danger";
            if (status == "Sent" || status == "Delivered" || status == "Read" || status == "Received") return "is-ok";
            return "is-muted";
        }

        private void SetVisibleView()
        {
            pnlCompose.Visible = CurrentView == "compose";
            pnlGroups.Visible = CurrentView == "groups";
            pnlDrafts.Visible = CurrentView == "drafts";
            pnlOutbox.Visible = CurrentView == "outbox";
            pnlInbox.Visible = CurrentView == "inbox";
            pnlLog.Visible = CurrentView == "log";
            pnlTemplates.Visible = CurrentView == "templates";
            pnlConsent.Visible = CurrentView == "consent";
        }

        private void SetConnectionState()
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (settings.ManualLinkMode)
            {
                litConnectionStatus.Text = "وضع الإرسال اليدوي";
                litConnectionNumber.Text = "بدون Cloud API مدفوع — تُجهَّز الرسالة ويُرسلها الموظف عبر رابط واتساب";
                btnTestConnection.Visible = false;
                return;
            }
            litConnectionStatus.Text = settings.IsConfigured ? "بيانات الربط موجودة" : "غير مهيأ";
            litConnectionNumber.Text = settings.IsConfigured
                ? "معرّف الرقم: " + HttpUtility.HtmlEncode(settings.PhoneNumberId) + " · لم يُختبر الاتصال بعد"
                : "أضف إعدادات Cloud API على الخادم";
        }

        private void SetWebhookState()
        {
            try
            {
                DateTime? last = db.WhatsAppMessages.AsNoTracking()
                    .Where(x => x.Direction == "Inbound")
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => (DateTime?)x.CreatedAt)
                    .FirstOrDefault();
                litWebhookStatus.Text = last.HasValue
                    ? "آخر رسالة واردة: " + last.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
                    : "لم تصل رسائل واردة بعد — تحقق من إعداد Webhook.";
            }
            catch (Exception ex)
            {
                litWebhookStatus.Text = "تعذر قراءة حالة الرسائل الواردة.";
                System.Diagnostics.Trace.TraceWarning("WhatsApp webhook state failed: " + ex.Message);
            }
        }

        private void SetSchedulerState()
        {
            var warnings = new List<string>();
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (settings.ManualLinkMode)
            {
                pnlSchedulerWarning.Visible = false;
                litSchedulerWarning.Text = string.Empty;
                return;
            }
            string dispatchUrl = WhatsAppApiSettings.ReadSetting("NOOR_WHATSAPP_DISPATCH_URL", "WhatsAppDispatchUrl");
            int stale = 0;
            try
            {
                DateTime threshold = DateTime.UtcNow.AddMinutes(-15);
                stale = db.WhatsAppMessages.AsNoTracking().Count(x => x.Direction == "Outbound" &&
                    (x.Status == "Queued" || x.Status == "Scheduled") &&
                    (x.ScheduledAt ?? x.CreatedAt) < threshold);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("WhatsApp scheduler state failed: " + ex.Message);
            }
            if (settings.IsConfigured && string.IsNullOrWhiteSpace(dispatchUrl))
                warnings.Add("لم يُضبط مسار استدعاء المجدول (NOOR_WHATSAPP_DISPATCH_URL)؛ الرسائل المجدولة والمعلّقة لن تُرسل حتى تُجدول مهمة تستدعي Dispatch.ashx.");
            if (stale > 0)
                warnings.Add("يوجد " + stale + " رسالة مستحقة منذ أكثر من 15 دقيقة ولم تُعالج. تحقق من تشغيل المجدول.");
            pnlSchedulerWarning.Visible = warnings.Count > 0;
            litSchedulerWarning.Text = string.Join(" ", warnings);
        }

        private bool ModuleSchemaReady()
        {
            try
            {
                db.WhatsAppTemplates.AsNoTracking().Select(x => x.Id).Take(1).ToList();
                db.WhatsAppMessages.AsNoTracking().Select(x => x.Id).Take(1).ToList();
                db.WhatsAppConsents.AsNoTracking().Select(x => new { x.Id, x.RevokedAt }).Take(1).ToList();
                db.WhatsAppCampaigns.AsNoTracking()
                    .Select(x => new { x.Id, x.AudienceType, x.IncludeBalance }).Take(1).ToList();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 207 || ex.Number == 208 || ex.Number == 209)
                {
                    pnlSchemaWarning.Visible = true;
                    System.Diagnostics.Trace.TraceWarning("WhatsApp tables are not ready: " + ex.Message);
                }
                else
                {
                    pnlDbError.Visible = true;
                    System.Diagnostics.Trace.TraceError("WhatsApp database error: " + ex);
                }
                return false;
            }
            catch (Exception ex)
            {
                pnlDbError.Visible = true;
                System.Diagnostics.Trace.TraceError("WhatsApp database error: " + ex);
                return false;
            }
        }

        private void ConsumeFlashNotice()
        {
            object raw = Session[FlashNoticeKey];
            if (raw == null) return;
            Session.Remove(FlashNoticeKey);
            string text = Convert.ToString(raw) ?? string.Empty;
            if (text.Length < 2) return;
            ShowNotice(text.Substring(2), text[0] == '1');
        }

        private void FlashNotice(string message, bool isError)
        {
            Session[FlashNoticeKey] = (isError ? "1" : "0") + "|" + message;
        }

        private void ClearStaleDraftEditingSession()
        {
            if (CurrentView != "compose") return;
            if (Session[PendingDraftKey] != null) return;
            if (Session[EditingDraftKey] != null) Session.Remove(EditingDraftKey);
        }

        private void BindClassSelectors()
        {
            var classes = LoadClasses();
            BindClassDropDown(ddlClass, classes, "اختر الفصل المستهدف");
            BindClassDropDown(ddlMappingClass, classes, "اختر الفصل");
            BindClassDropDown(ddlConsentClass, classes, "اختر الفصل");
            if (classes.Count > 0)
            {
                ddlClass.SelectedValue = classes[0].Id.ToString(CultureInfo.InvariantCulture);
                ddlConsentClass.SelectedValue = classes[0].Id.ToString(CultureInfo.InvariantCulture);
                ddlMappingClass.SelectedValue = classes[0].Id.ToString(CultureInfo.InvariantCulture);
            }
        }

        private List<ClassOption> LoadClasses()
        {
            EducationStage? stage = ResolveStage();
            var query = db.ClassRooms.AsNoTracking().Include(x => x.GradeLevel).AsQueryable();
            if (stage.HasValue) query = query.Where(x => x.GradeLevel.Stage == stage.Value);
            return query.OrderBy(x => x.GradeLevel.SortOrder).ThenBy(x => x.Name)
                .ThenBy(x => x.DivisionName)
                .ToList()
                .Select(x => new ClassOption
                {
                    Id = x.Id,
                    Name = GetClassName(x),
                    Division = x.DivisionName
                }).ToList();
        }

        private EducationStage? ResolveStage()
        {
            string stage = Request.QueryString["stage"] ?? Session["Stage"] as string;
            EducationStage parsed;
            return Enum.TryParse(stage, true, out parsed) ? (EducationStage?)parsed : null;
        }

        private static string GetClassName(ClassRoom room)
        {
            string name = room.GradeLevel == null ? room.Name : room.GradeLevel.Name;
            if (!string.IsNullOrWhiteSpace(room.DivisionName)) name += " - " + room.DivisionName;
            else if (!string.IsNullOrWhiteSpace(room.SectionName)) name += " - " + room.SectionName;
            return name;
        }

        private static void BindClassDropDown(DropDownList ddl, List<ClassOption> classes, string prompt)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem(prompt, string.Empty));
            foreach (ClassOption item in classes)
                ddl.Items.Add(new ListItem(item.Name, item.Id.ToString(CultureInfo.InvariantCulture)));
        }

        private void BindTemplateSelector()
        {
            ddlTemplate.Items.Clear();
            ddlTemplate.Items.Add(new ListItem("رسالة حرة", string.Empty));
            foreach (WhatsAppTemplate template in db.WhatsAppTemplates.AsNoTracking()
                .Where(x => x.IsActive).OrderBy(x => x.Category).ThenBy(x => x.Name).ToList())
            {
                string label = template.Name + (template.IsMetaApproved ? " · معتمد" : " · محلي");
                ddlTemplate.Items.Add(new ListItem(label, template.Id.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private void BindReplyTemplates()
        {
            ddlReplyTemplate.Items.Clear();
            ddlReplyTemplate.Items.Add(new ListItem("إدراج قالب جاهز…", string.Empty));
            foreach (WhatsAppTemplate template in db.WhatsAppTemplates.AsNoTracking()
                .Where(x => x.IsActive).OrderBy(x => x.Name).ToList())
            {
                ddlReplyTemplate.Items.Add(new ListItem(template.Name, template.Id.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private void SetInitialDateTime()
        {
            txtScheduledAt.Text = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        }

        private void BindCurrentView()
        {
            switch (CurrentView)
            {
                case "compose": UpdateRecipientSummary(); BindDailyPreview(); break;
                case "groups": BindMappings(); break;
                case "drafts": BindDrafts(); break;
                case "outbox": BindOutbox(); break;
                case "inbox": BindInbox(); break;
                case "log": BindLog(); break;
                case "templates": BindTemplates(); break;
                case "consent": BindContacts(); break;
            }
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
                    litRecipientSummary.Text = "اختر الفصل لعرض عدد أولياء الأمور المستعدين للإرسال.";
                    return;
                }
                targetClass = classId;
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
            ddlStudent.Items.Add(new ListItem("اختر الطالب وولي الأمر", string.Empty));
            int classId;
            if (!int.TryParse(ddlClass.SelectedValue, out classId)) return;
            var students = db.Students.AsNoTracking().Include(x => x.Parent)
                .Where(x => x.ClassRoomId == classId && x.IsActive && x.ParentId.HasValue)
                .ToList().Where(x => x.Parent != null && !string.IsNullOrWhiteSpace(x.Parent.Phone))
                .OrderBy(x => x.Name).ToList();
            foreach (Student student in students)
                ddlStudent.Items.Add(new ListItem(student.Name + " — " + student.Parent.Name,
                    student.Id.ToString(CultureInfo.InvariantCulture)));
        }

        protected void chkAppendDailyLessons_CheckedChanged(object sender, EventArgs e)
        {
            BindDailyPreview();
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

        private void SaveCampaign(string status, DateTime? scheduledAtUtc, bool createMessages)
        {
            try
            {
                EnforceFinancialPermission();
                string audience = ddlAudienceType.SelectedValue;
                int classId;
                int? targetClass = null;
                if (audience != "School")
                {
                    if (!int.TryParse(ddlClass.SelectedValue, out classId))
                    {
                        ShowNotice("اختر الفصل أو قائمة المستلمين أولاً.", true);
                        return;
                    }
                    targetClass = classId;
                }
                string body = txtMessage.Text == null ? string.Empty : txtMessage.Text.Trim();
                if (body.Length == 0)
                {
                    ShowNotice("اكتب نص الرسالة أو اختر قالباً.", true);
                    return;
                }
                if (body.Length > 4000)
                {
                    ShowNotice("نص الرسالة أطول من الحد المسموح.", true);
                    return;
                }
                if ((body.IndexOf("{balance}", StringComparison.OrdinalIgnoreCase) >= 0 && !chkIncludeBalance.Checked) ||
                    (body.IndexOf("{attendanceSummary}", StringComparison.OrdinalIgnoreCase) >= 0 && !chkIncludeAttendance.Checked) ||
                    (body.IndexOf("{latestGrade}", StringComparison.OrdinalIgnoreCase) >= 0 && !chkIncludeGrades.Checked))
                {
                    ShowNotice("فعّل خيارات البيانات التي تظهر متغيراتها في نص الرسالة قبل الحفظ أو الإرسال.", true);
                    return;
                }
                string campaignTitle = string.IsNullOrWhiteSpace(txtCampaignTitle.Text)
                    ? body.Length > 80 ? body.Substring(0, 80) : body
                    : txtCampaignTitle.Text.Trim();
                WhatsAppTemplate selectedTemplate = GetSelectedTemplate();
                string kind = ddlMessageKind.SelectedValue;
                int targetStudentId;
                int? targetStudent = audience == "Individual" &&
                    int.TryParse(ddlStudent.SelectedValue, out targetStudentId) ? (int?)targetStudentId : null;
                if (audience == "Individual" && !targetStudent.HasValue)
                {
                    ShowNotice("اختر الطالب الذي تريد مراسلة ولي أمره.", true);
                    return;
                }
                List<Recipient> recipients = createMessages ? LoadEligibleRecipients(targetClass, targetStudent) : new List<Recipient>();
                if (createMessages && recipients.Count == 0)
                {
                    ShowNotice("لا يوجد ولي أمر لديه رقم وموافقة واتساب فعّالة ضمن النطاق المحدد. راجع تبويب الموافقات.", true);
                    return;
                }

                int editingId;
                WhatsAppCampaign campaign = int.TryParse(Convert.ToString(Session[EditingDraftKey]), out editingId)
                    ? db.WhatsAppCampaigns.FirstOrDefault(x => x.Id == editingId && x.Status == "Draft")
                    : null;
                if (campaign == null)
                {
                    campaign = new WhatsAppCampaign
                    {
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = CurrentUserName()
                    };
                    db.WhatsAppCampaigns.Add(campaign);
                }
                campaign.Title = campaignTitle;
                campaign.MessageBody = body;
                campaign.MessageKind = kind;
                campaign.Status = status;
                campaign.AudienceType = audience;
                campaign.ClassRoomId = targetClass;
                campaign.TargetStudentId = targetStudent;
                campaign.WhatsAppTemplateId = selectedTemplate == null ? (int?)null : selectedTemplate.Id;
                campaign.IncludeBalance = chkIncludeBalance.Checked;
                campaign.IncludeAttendance = chkIncludeAttendance.Checked;
                campaign.IncludeGrades = chkIncludeGrades.Checked;
                campaign.AppendDailyLessons = chkAppendDailyLessons.Checked;
                campaign.ScheduledAt = scheduledAtUtc;
                db.SaveChanges();
                Session.Remove(EditingDraftKey);

                if (createMessages)
                {
                    foreach (Recipient recipient in recipients)
                    {
                        List<string> parameters;
                        string rendered = RenderMessage(body, recipient, selectedTemplate, out parameters);
                        db.WhatsAppMessages.Add(new WhatsAppMessage
                        {
                            CampaignId = campaign.Id,
                            ParentId = recipient.Parent.Id,
                            StudentId = recipient.Students.Count == 1 ? recipient.Students[0].Id : (int?)null,
                            RecipientName = recipient.Parent.Name,
                            PhoneNumber = WhatsAppCloudApi.NormalizeRecipientNumber(recipient.Parent.Phone),
                            MessageBody = rendered,
                            Direction = "Outbound",
                            Status = status,
                            ProviderTemplateName = selectedTemplate != null && selectedTemplate.IsMetaApproved
                                ? selectedTemplate.ProviderTemplateName : null,
                            TemplateLanguage = selectedTemplate == null ? null : selectedTemplate.LanguageCode,
                            TemplateParametersJson = new JavaScriptSerializer().Serialize(parameters),
                            ScheduledAt = scheduledAtUtc,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = CurrentUserName()
                        });
                    }
                    db.SaveChanges();
                }
                bool manual = WhatsAppApiSettings.Load().ManualLinkMode;
                string message;
                if (status == "Draft")
                    message = "حُفظت المسودة. لم يُنشأ طابور إرسال.";
                else if (manual)
                    message = "جُهّزت " + recipients.Count + " رسالة في صندوق الإرسال اليدوي. افتح كل رابط، أرسل من واتساب، ثم علّمها كمُرسلة.";
                else if (status == "Scheduled")
                    message = "جُدولت الحملة لعدد " + recipients.Count + " وليّ أمر. سيُعاد التحقق من الموافقة عند موعد الإرسال.";
                else
                    message = "أُضيفت " + recipients.Count + " رسالة إلى طابور الإرسال. سيعالجها المجدول على الخادم.";
                ShowNotice(message, false);
                if (status == "Draft")
                {
                    FlashNotice(message, false);
                    Response.Redirect(ViewUrl("drafts"), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else if (manual && createMessages)
                {
                    FlashNotice(message, false);
                    Response.Redirect(ViewUrl("outbox"), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowNotice(ex.Message, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp campaign save failed: " + ex);
                ShowNotice("تعذر حفظ الحملة. راجع سجل أخطاء التطبيق.", true);
            }
        }

        private void EnforceFinancialPermission()
        {
            if (ddlMessageKind.SelectedValue == "Financial" &&
                SecurityHelper.CurrentRole != UserRole.Admin &&
                SecurityHelper.CurrentRole != UserRole.Accountant)
            {
                ddlMessageKind.SelectedValue = "Announcement";
                chkIncludeBalance.Checked = false;
                throw new UnauthorizedAccessException("إرسال البيانات المالية متاح لمدير النظام أو المحاسب فقط.");
            }
            if (chkIncludeBalance.Checked &&
                SecurityHelper.CurrentRole != UserRole.Admin &&
                SecurityHelper.CurrentRole != UserRole.Accountant)
            {
                chkIncludeBalance.Checked = false;
                throw new UnauthorizedAccessException("إظهار الرصيد المالي متاح لمدير النظام أو المحاسب فقط.");
            }
        }

        private Dictionary<int, WhatsAppConsent> LoadConsentMap()
        {
            var map = new Dictionary<int, WhatsAppConsent>();
            foreach (WhatsAppConsent consent in db.WhatsAppConsents.AsNoTracking().ToList())
                map[consent.ParentId] = consent;
            return map;
        }

        private static bool IsConsentValid(Dictionary<int, WhatsAppConsent> consents, int parentId, string phone)
        {
            WhatsAppConsent consent;
            if (consents == null || !consents.TryGetValue(parentId, out consent) || !consent.IsOptedIn) return false;
            string currentPhone = WhatsAppCloudApi.NormalizeRecipientNumber(phone);
            if (currentPhone.Length < 10) return false;
            return currentPhone == WhatsAppCloudApi.NormalizeRecipientNumber(consent.PhoneNumber);
        }

        private List<Recipient> LoadEligibleRecipients(int? classId, int? targetStudentId = null)
        {
            var query = db.Students.AsNoTracking().Include(x => x.Parent)
                .Include(x => x.ClassRoom).Include(x => x.ClassRoom.GradeLevel)
                .Where(x => x.IsActive && x.ParentId.HasValue);
            if (classId.HasValue)
            {
                query = query.Where(x => x.ClassRoomId == classId.Value);
            }
            else
            {
                EducationStage? stage = ResolveStage();
                if (stage.HasValue) query = query.Where(x => x.ClassRoom.GradeLevel.Stage == stage.Value);
            }
            if (targetStudentId.HasValue) query = query.Where(x => x.Id == targetStudentId.Value);
            var students = query.ToList()
                .Where(x => x.Parent != null && !string.IsNullOrWhiteSpace(x.Parent.Phone)).ToList();
            var consents = LoadConsentMap();
            var result = new List<Recipient>();
            foreach (var group in students.GroupBy(x => x.ParentId.Value))
            {
                Parent parent = group.First().Parent;
                if (!IsConsentValid(consents, group.Key, parent.Phone)) continue;
                result.Add(new Recipient { Parent = parent, Students = group.OrderBy(x => x.Name).ToList() });
            }
            return result;
        }

        private Dictionary<int, int> CountEligiblePerClass()
        {
            var result = new Dictionary<int, int>();
            var consents = LoadConsentMap();
            var rows = db.Students.AsNoTracking()
                .Where(x => x.IsActive && x.ParentId.HasValue)
                .Select(x => new { x.ClassRoomId, x.ParentId, x.Parent.Phone }).ToList();
            var seen = new Dictionary<int, HashSet<int>>();
            foreach (var row in rows)
            {
                if (!row.ParentId.HasValue) continue;
                if (!IsConsentValid(consents, row.ParentId.Value, row.Phone)) continue;
                HashSet<int> set;
                if (!seen.TryGetValue(row.ClassRoomId, out set))
                {
                    set = new HashSet<int>();
                    seen[row.ClassRoomId] = set;
                }
                set.Add(row.ParentId.Value);
            }
            foreach (var pair in seen) result[pair.Key] = pair.Value.Count;
            return result;
        }

        private string RenderMessage(string body, Recipient recipient, WhatsAppTemplate selectedTemplate, out List<string> orderedParameters)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "guardianName", recipient.Parent == null || string.IsNullOrWhiteSpace(recipient.Parent.Name) ? "ولي الأمر" : recipient.Parent.Name },
                { "studentName", JoinNames(recipient.Students) },
                { "class", ClassNames(recipient.Students) },
                { "date", DateTime.Today.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) },
                { "time", string.Empty },
                { "extra", string.Empty },
                { "balance", chkIncludeBalance.Checked ? BuildBalanceSummary(recipient.Students) : "يرجى التواصل مع الإدارة" },
                { "attendanceSummary", chkIncludeAttendance.Checked ? BuildAttendanceSummary(recipient.Students) : "يرجى التواصل مع الإدارة" },
                { "latestGrade", chkIncludeGrades.Checked ? BuildLatestGradeSummary(recipient.Students) : "يرجى التواصل مع الإدارة" }
            };
            if (chkAppendDailyLessons.Checked && recipient.Students.Count > 0 && recipient.Students[0].ClassRoom != null)
                values["extra"] = BuildDailyLessonSummary(recipient.Students[0].ClassRoomId);
            return WhatsAppTemplateRenderer.Render(body, values, out orderedParameters);
        }

        private static string JoinNames(List<Student> students)
        {
            var names = students.Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return names.Count == 0 ? string.Empty : string.Join(" و ", names);
        }

        private static string ClassNames(List<Student> students)
        {
            var names = students.Select(x => x.ClassRoom == null ? string.Empty : GetClassName(x.ClassRoom))
                .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            return string.Join("، ", names);
        }

        private decimal StudentBalance(int studentId)
        {
            return db.Invoices.AsNoTracking().Where(x => x.StudentId == studentId &&
                    x.Status != InvoiceStatus.Draft && x.Status != InvoiceStatus.Cancelled && x.AcademicYear.IsCurrent)
                .Select(x => new { x.NetAmount, x.PaidAmount }).ToList()
                .Sum(x => Math.Max(0m, x.NetAmount - x.PaidAmount));
        }

        private string BuildBalanceSummary(List<Student> students)
        {
            if (students.Count <= 1)
                return StudentBalance(students.Count == 0 ? 0 : students[0].Id).ToString("N2", CultureInfo.CurrentCulture) + " د.ل";
            decimal total = 0m;
            var parts = new List<string>();
            foreach (Student student in students)
            {
                decimal balance = StudentBalance(student.Id);
                total += balance;
                parts.Add(student.Name + ": " + balance.ToString("N2", CultureInfo.CurrentCulture) + " د.ل");
            }
            return "الإجمالي " + total.ToString("N2", CultureInfo.CurrentCulture) + " د.ل (" + string.Join("؛ ", parts) + ")";
        }

        private string BuildAttendanceSummary(List<Student> students)
        {
            DateTime from = DateTime.Today.AddDays(-6);
            DateTime to = DateTime.Today.AddDays(1);
            if (students.Count <= 1)
            {
                int single = students.Count == 0 ? 0 : AbsenceCount(students[0].Id, from, to);
                return "الغياب خلال آخر 7 أيام: " + single + " يوم";
            }
            var parts = new List<string>();
            foreach (Student student in students)
                parts.Add(student.Name + ": " + AbsenceCount(student.Id, from, to) + " يوم");
            return "الغياب خلال آخر 7 أيام (" + string.Join("؛ ", parts) + ")";
        }

        private int AbsenceCount(int studentId, DateTime from, DateTime to)
        {
            return db.Attendances.AsNoTracking()
                .Count(x => x.StudentId == studentId && !x.IsPresent && x.Date >= from && x.Date < to);
        }

        private string BuildLatestGradeSummary(List<Student> students)
        {
            if (students.Count <= 1)
                return BuildLatestGrade(students.Count == 0 ? 0 : students[0].Id);
            var parts = new List<string>();
            foreach (Student student in students)
                parts.Add(student.Name + " — " + BuildLatestGrade(student.Id));
            return string.Join("؛ ", parts);
        }

        private string BuildLatestGrade(int studentId)
        {
            var grade = db.MonthlySheetGrades.AsNoTracking().Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.MonthNumber).ThenByDescending(x => x.Id)
                .Select(x => new { x.Subject, x.Total, x.MonthNumber })
                .FirstOrDefault();
            return grade == null ? "لا توجد درجة حديثة مسجلة" :
                grade.Subject + ": " + (grade.Total ?? "—") + " (الشهر " + grade.MonthNumber + ")";
        }

        private string BuildDailyLessonSummary(int classId)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            var lessons = db.DailyLessons.AsNoTracking().Include(x => x.Subject).Include(x => x.Teacher)
                .Where(x => x.ClassRoomId == classId && x.Date >= today && x.Date < tomorrow && x.Status == LessonStatus.Approved)
                .OrderBy(x => x.PeriodNumber).ToList();
            return lessons.Count == 0 ? "لا يوجد جدول مسجل لهذا الفصل اليوم" :
                "دروس اليوم: " + string.Join("، ", lessons.Select(x =>
                    x.PeriodNumber + "-" + (x.Subject == null ? "حصة" : x.Subject.Name) +
                    (x.Teacher == null ? string.Empty : " مع " + x.Teacher.Name)));
        }

        private WhatsAppTemplate GetSelectedTemplate()
        {
            int id;
            return int.TryParse(ddlTemplate.SelectedValue, out id)
                ? db.WhatsAppTemplates.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive)
                : null;
        }

        private static bool TryParseLocalSchedule(string input, out DateTime local)
        {
            string[] formats = { "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss" };
            return DateTime.TryParseExact((input ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out local);
        }

        private void ShowNotice(string message, bool isError)
        {
            pnlNotice.Visible = true;
            pnlNotice.CssClass = isError ? "wa-notice wa-notice-warning" : "wa-notice";
            litNotice.Text = message;
        }

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
            txtCampaignTitle.Text = string.Empty;
            txtMessage.Text = string.Empty;
            ddlTemplate.SelectedIndex = 0;
            ddlAudienceType.SelectedValue = "Class";
            chkIncludeBalance.Checked = false;
            chkIncludeAttendance.Checked = false;
            chkIncludeGrades.Checked = false;
            chkAppendDailyLessons.Checked = false;
            ddlMessageKind.SelectedValue = "Announcement";
            Session.Remove(EditingDraftKey);
            SetAudienceVisibility();
            SetInitialDateTime();
            UpdateRecipientSummary();
            BindDailyPreview();
        }

        protected void btnSendManagerSummary_Click(object sender, EventArgs e)
        {
            if (!IsAdmin)
            {
                ShowNotice("إرسال الملخص الداخلي متاح لمدير النظام فقط.", true);
                return;
            }
            if (!chkManagerConfirmed.Checked)
            {
                ShowNotice("أكد أن الرقم المهيأ يخص المدير وأنه وافق على استلام الملخص.", true);
                return;
            }
            if (!WhatsAppApiSettings.Load().IsConfigured && !WhatsAppApiSettings.Load().ManualLinkMode)
            {
                ShowNotice("إعداد WhatsApp Cloud API غير مكتمل.", true);
                return;
            }
            string managerPhone = WhatsAppApiSettings.ReadSetting("NOOR_WHATSAPP_MANAGER_PHONE", "WhatsAppManagerPhone");
            string normalizedPhone = WhatsAppCloudApi.NormalizeRecipientNumber(managerPhone);
            if (normalizedPhone.Length < 10)
            {
                ShowNotice("أضف رقم المدير بصيغة دولية أو اضبط رمز الدولة في إعدادات الخادم.", true);
                return;
            }
            int classId;
            if (!int.TryParse(ddlClass.SelectedValue, out classId))
            {
                ShowNotice("اختر الفصل الذي تريد تلخيص يومه.", true);
                return;
            }

            try
            {
                string summary = BuildManagerDailySummary(classId);
                var campaign = new WhatsAppCampaign
                {
                    Title = "ملخص اليوم للمدير · " + DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    MessageBody = summary,
                    MessageKind = "ManagerSummary",
                    Status = "Queued",
                    AudienceType = "Class",
                    ClassRoomId = classId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = CurrentUserName()
                };
                db.WhatsAppCampaigns.Add(campaign);
                db.SaveChanges();

                WhatsAppTemplate selectedTemplate = GetSelectedTemplate();
                List<string> parameters = new List<string>();
                string rendered = summary;
                if (selectedTemplate != null && selectedTemplate.IsMetaApproved && !string.IsNullOrWhiteSpace(selectedTemplate.ProviderTemplateName))
                {
                    var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "guardianName", "مدير النظام" }, { "studentName", "ملخص اليوم" },
                        { "class", GetClassName(db.ClassRooms.Include(x => x.GradeLevel).First(x => x.Id == classId)) },
                        { "date", DateTime.Today.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) },
                        { "time", string.Empty }, { "extra", summary }, { "attendanceSummary", summary },
                        { "latestGrade", string.Empty }, { "balance", string.Empty }
                    };
                    rendered = WhatsAppTemplateRenderer.Render(selectedTemplate.Body, values, out parameters);
                }
                db.WhatsAppMessages.Add(new WhatsAppMessage
                {
                    CampaignId = campaign.Id,
                    RecipientName = "مدير النظام",
                    PhoneNumber = normalizedPhone,
                    MessageBody = rendered,
                    Direction = "Outbound",
                    Status = "Queued",
                    IsInternalRecipient = true,
                    ProviderTemplateName = selectedTemplate != null && selectedTemplate.IsMetaApproved
                        ? selectedTemplate.ProviderTemplateName : null,
                    TemplateLanguage = selectedTemplate == null ? null : selectedTemplate.LanguageCode,
                    TemplateParametersJson = new JavaScriptSerializer().Serialize(parameters),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = CurrentUserName()
                });
                db.SaveChanges();
                ShowNotice(WhatsAppApiSettings.Load().ManualLinkMode
                    ? "جُهّز ملخص اليوم في صندوق الإرسال اليدوي. افتح الرابط وأرسله للمدير ثم علّمه كمُرسل."
                    : "أُضيف ملخص اليوم إلى طابور الإرسال للمدير. راقب حالته في سجل الرسائل.", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp manager summary failed: " + ex);
                ShowNotice("تعذر إنشاء ملخص المدير. راجع سجل أخطاء التطبيق.", true);
            }
        }

        private string BuildManagerDailySummary(int classId)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            var room = db.ClassRooms.AsNoTracking().Include(x => x.GradeLevel).FirstOrDefault(x => x.Id == classId);
            if (room == null) throw new InvalidOperationException("الفصل غير موجود.");
            List<int> studentIds = db.Students.AsNoTracking()
                .Where(x => x.ClassRoomId == classId && x.IsActive).Select(x => x.Id).ToList();
            int recordedAttendance = db.Attendances.AsNoTracking().Count(x => studentIds.Contains(x.StudentId) && x.Date >= today && x.Date < tomorrow);
            int absences = db.Attendances.AsNoTracking().Count(x => studentIds.Contains(x.StudentId) && !x.IsPresent && x.Date >= today && x.Date < tomorrow);
            var lessons = db.DailyLessons.AsNoTracking().Include(x => x.Subject).Include(x => x.Teacher)
                .Where(x => x.ClassRoomId == classId && x.Date >= today && x.Date < tomorrow && x.Status == LessonStatus.Approved)
                .OrderBy(x => x.PeriodNumber).ToList();
            string lessonSummary = lessons.Count == 0 ? "لا توجد دروس معتمدة مسجلة" : string.Join("، ", lessons.Select(x =>
                x.PeriodNumber + "-" + (x.Subject == null ? "حصة" : x.Subject.Name) +
                (x.Teacher == null ? string.Empty : " مع " + x.Teacher.Name)));
            return "ملخص اليوم — " + today.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) +
                "\nالفصل: " + GetClassName(room) +
                "\nعدد الطلاب النشطين: " + studentIds.Count +
                "\nحضور مسجل: " + recordedAttendance + "، غياب: " + absences +
                "\nدروس اليوم: " + lessonSummary;
        }

        protected void btnSaveMapping_Click(object sender, EventArgs e)
        {
            int classId;
            if (!int.TryParse(ddlMappingClass.SelectedValue, out classId) || string.IsNullOrWhiteSpace(txtMappingName.Text))
            {
                ShowNotice("اختر فصلاً واكتب اسماً للقائمة.", true);
                return;
            }
            var row = db.WhatsAppClassMappings.FirstOrDefault(x => x.ClassRoomId == classId);
            if (row == null)
            {
                row = new WhatsAppClassMapping { ClassRoomId = classId };
                db.WhatsAppClassMappings.Add(row);
            }
            row.DisplayName = txtMappingName.Text.Trim();
            row.IsActive = true;
            row.UpdatedAt = DateTime.UtcNow;
            row.UpdatedBy = CurrentUserName();
            db.SaveChanges();
            txtMappingName.Text = string.Empty;
            BindMappings();
            ShowNotice("حُفظ ربط قائمة المستلمين.", false);
        }

        private void BindMappings()
        {
            List<ClassOption> classes = LoadClasses();
            var classNames = classes.ToDictionary(x => x.Id, x => x.Name);
            var recipients = CountEligiblePerClass();
            var rows = db.WhatsAppClassMappings.AsNoTracking().OrderBy(x => x.DisplayName).ToList()
                .Where(x => classNames.ContainsKey(x.ClassRoomId))
                .Select(x => new MappingRow
                {
                    Id = x.Id, ClassName = classNames[x.ClassRoomId], DisplayName = x.DisplayName,
                    IsActive = x.IsActive, Recipients = recipients.ContainsKey(x.ClassRoomId) ? recipients[x.ClassRoomId] : 0
                }).ToList();
            SetGridPage(gvMappings, rows.Count, gvMappings.PageIndex);
            gvMappings.DataSource = rows;
            gvMappings.DataBind();
        }

        protected void gvMappings_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMappings.PageIndex = e.NewPageIndex;
            BindMappings();
        }

        protected void gvMappings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "toggle") return;
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            var row = db.WhatsAppClassMappings.Find(id);
            if (row == null) return;
            row.IsActive = !row.IsActive;
            row.UpdatedAt = DateTime.UtcNow;
            row.UpdatedBy = CurrentUserName();
            db.SaveChanges();
            BindMappings();
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
            BindReplyTemplates();
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
            if (e.CommandName == "edit")
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
                BindReplyTemplates();
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
            if (e.CommandName == "optin" &&
                (!chkConsentVerified.Checked || string.IsNullOrWhiteSpace(txtConsentSource.Text)))
            {
                ShowNotice("لتسجيل الموافقة، أكّد توثيقها واكتب مصدر التحقق.", true);
                return;
            }
            var consent = db.WhatsAppConsents.FirstOrDefault(x => x.ParentId == parentId);
            if (consent == null)
            {
                consent = new WhatsAppConsent { ParentId = parentId };
                db.WhatsAppConsents.Add(consent);
            }
            string source = (txtConsentSource.Text ?? string.Empty).Trim();
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
                consent.RevokeReason = string.IsNullOrWhiteSpace(source) ? "إلغاء موافقة عبر النظام" : source;
                consent.ConsentSource = "إلغاء موافقة عبر النظام" +
                    (string.IsNullOrWhiteSpace(source) ? string.Empty : ": " + source);
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

        protected void btnExportConsents_Click(object sender, EventArgs e)
        {
            var rows = LoadContactRows();
            var sb = new StringBuilder();
            WriteCsvRow(sb, "ولي الأمر", "الطالب/الطلاب", "رقم الهاتف", "حالة الموافقة", "مصدر الموافقة", "التاريخ", "آخر تحديث بواسطة");
            foreach (ContactRow row in rows)
                WriteCsvRow(sb, row.ParentName, row.StudentNames, row.Phone, row.ConsentState, row.ConsentSource, row.ConsentDate, row.UpdatedBy);
            SendCsv(sb.ToString(), "whatsapp-consents");
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
            if (e.CommandName == "cancel")
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
            txtCampaignTitle.Text = campaign.Title;
            txtMessage.Text = campaign.MessageBody;
            Session[EditingDraftKey] = campaign.Id;
            if (campaign.ClassRoomId.HasValue && ddlClass.Items.FindByValue(campaign.ClassRoomId.Value.ToString(CultureInfo.InvariantCulture)) != null)
                ddlClass.SelectedValue = campaign.ClassRoomId.Value.ToString(CultureInfo.InvariantCulture);
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
            if (e.CommandName == "cancel")
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

        /// <summary>
        /// Returns whether this outbound message may be sent from the manual outbox right now.
        /// A message is blocked when the parent has no valid consent, or when it is scheduled
        /// for a future time. Internal recipients (no ParentId) are never consent-blocked.
        /// </summary>
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

        /// <summary>
        /// Returns messages left in Sending (e.g. an interrupted prior run) back to the queue
        /// so they are not stuck forever. Only unconfirmed rows are recovered.
        /// </summary>
        private int RecoverStuckSending()
        {
            var stuck = db.WhatsAppMessages
                .Where(x => x.Status == "Sending" && x.ProviderMessageId == null && x.SentAt == null)
                .ToList();
            if (stuck.Count == 0) return 0;
            foreach (WhatsAppMessage message in stuck)
            {
                message.Status = "Queued";
                message.ErrorDetails = "أُعيدت الرسالة إلى الطابور بعد أن بقيت في حالة الإرسال دون نتيجة مؤكدة.";
            }
            db.SaveChanges();
            return stuck.Count;
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
            return "https://wa.me/" + number + "?text=" + Uri.EscapeDataString(body ?? string.Empty);
        }

        protected void InboxSearch_TextChanged(object sender, EventArgs e)
        {
            gvInbox.PageIndex = 0;
            BindInbox();
        }

        protected void ClearInboxSearch_Click(object sender, EventArgs e)
        {
            txtInboxSearch.Text = string.Empty;
            gvInbox.PageIndex = 0;
            BindInbox();
        }

        private void BindInbox()
        {
            string search = (txtInboxSearch.Text ?? string.Empty).Trim();
            var query = db.WhatsAppMessages.AsNoTracking().Where(x => x.Direction == "Inbound");
            if (search.Length > 0)
                query = query.Where(x => x.PhoneNumber.Contains(search) ||
                    x.RecipientName.Contains(search) || x.MessageBody.Contains(search));
            var rows = query.OrderByDescending(x => x.CreatedAt).Take(InboxRowLimit).ToList()
                .Select(x => new InboxRow
                {
                    Id = x.Id,
                    CreatedAtLocal = x.CreatedAt.ToLocalTime(),
                    RecipientName = x.RecipientName,
                    PhoneNumber = x.PhoneNumber,
                    MessageBody = x.MessageBody
                }).ToList();
            SetGridPage(gvInbox, rows.Count, gvInbox.PageIndex);
            gvInbox.DataSource = rows;
            gvInbox.DataBind();
        }

        protected void gvInbox_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInbox.PageIndex = e.NewPageIndex;
            BindInbox();
        }

        protected void gvInbox_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "reply") return;
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            var message = db.WhatsAppMessages.AsNoTracking().FirstOrDefault(x => x.Id == id && x.Direction == "Inbound");
            if (message == null) return;
            hdnReplyPhone.Value = message.PhoneNumber;
            hdnReplyParentId.Value = message.ParentId.HasValue ? message.ParentId.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            litReplyTo.Text = message.RecipientName + " · " + message.PhoneNumber;
            pnlReply.Visible = true;
        }

        protected void ddlReplyTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(ddlReplyTemplate.SelectedValue, out id)) return;
            var template = db.WhatsAppTemplates.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive);
            if (template != null) txtReply.Text = template.Body;
        }

        protected void btnSendReply_Click(object sender, EventArgs e)
        {
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (!settings.IsConfigured && !settings.ManualLinkMode)
            {
                ShowNotice("إعداد Cloud API غير مكتمل؛ لم يُرسل الرد.", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(hdnReplyPhone.Value) || string.IsNullOrWhiteSpace(txtReply.Text))
            {
                ShowNotice("اكتب نص الرد أولاً.", true);
                return;
            }
            int? parentId = null;
            int parsedParent;
            if (int.TryParse(hdnReplyParentId.Value, out parsedParent)) parentId = parsedParent;
            if (settings.RepliesRequireConsent)
            {
                if (!parentId.HasValue || !IsConsentValid(LoadConsentMap(), parentId.Value, hdnReplyPhone.Value))
                {
                    ShowNotice("الرد محظور وفق سياسة المدرسة: لا توجد موافقة فعّالة لهذا الرقم.", true);
                    return;
                }
            }
            db.WhatsAppMessages.Add(new WhatsAppMessage
            {
                ParentId = parentId,
                PhoneNumber = WhatsAppCloudApi.NormalizeNumber(hdnReplyPhone.Value),
                RecipientName = litReplyTo.Text,
                MessageBody = txtReply.Text.Trim(),
                Direction = "Outbound",
                Status = "Queued",
                IsReplyToIncoming = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = CurrentUserName()
            });
            db.SaveChanges();
            txtReply.Text = string.Empty;
            ddlReplyTemplate.SelectedIndex = 0;
            pnlReply.Visible = false;
            ShowNotice(settings.ManualLinkMode
                ? "جُهّز الرد في صندوق الإرسال اليدوي. أرسله عبر رابط واتساب ثم علّمه كمُرسل."
                : "أُضيف الرد إلى طابور الإرسال. إذا انتهت نافذة الخدمة، سيظهر سبب الرفض في السجل.", false);
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

        protected void btnExportLog_Click(object sender, EventArgs e)
        {
            string status = ddlLogStatus.SelectedValue;
            string direction = ddlLogDirection.SelectedValue;
            string search = (txtLogSearch.Text ?? string.Empty).Trim();
            var query = db.WhatsAppMessages.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.Status == status);
            if (!string.IsNullOrWhiteSpace(direction)) query = query.Where(x => x.Direction == direction);
            if (search.Length > 0)
                query = query.Where(x => x.PhoneNumber.Contains(search) ||
                    x.RecipientName.Contains(search) || x.MessageBody.Contains(search));
            var rows = query.OrderByDescending(x => x.CreatedAt).Take(5000).ToList();
            var sb = new StringBuilder();
            WriteCsvRow(sb, "الوقت", "الاتجاه", "المستلم", "الرقم", "الحالة", "بواسطة", "النص", "تفاصيل الخطأ");
            foreach (WhatsAppMessage row in rows)
                WriteCsvRow(sb, row.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    row.Direction == "Outbound" ? "صادر" : "وارد",
                    row.RecipientName, row.PhoneNumber, StatusText(row.Status), row.CreatedBy, row.MessageBody, row.ErrorDetails);
            SendCsv(sb.ToString(), "whatsapp-log");
        }

        protected void gvLog_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
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
            if (e.CommandName == "cancel")
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

        private static void WriteCsvRow(StringBuilder builder, params string[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (i > 0) builder.Append(',');
                string value = cells[i] ?? string.Empty;
                value = value.Replace("\r", " ").Replace("\n", " ");
                if (value.Length > 0 && (value[0] == '=' || value[0] == '+' || value[0] == '-' || value[0] == '@'))
                    value = "'" + value;
                builder.Append('"').Append(value.Replace("\"", "\"\"")).Append('"');
            }
            builder.Append("\r\n");
        }

        private void SendCsv(string content, string fileNamePrefix)
        {
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" +
                fileNamePrefix + "-" + DateTime.Now.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture) + ".csv\"");
            Response.Write("\uFEFF");
            Response.Write(content);
            Response.Flush();
            Context.ApplicationInstance.CompleteRequest();
        }

        private sealed class ClassOption { public int Id { get; set; } public string Name { get; set; } public string Division { get; set; } }
        private sealed class Recipient { public Parent Parent { get; set; } public List<Student> Students { get; set; } }
        private sealed class MappingRow { public int Id { get; set; } public string ClassName { get; set; } public string DisplayName { get; set; } public bool IsActive { get; set; } public int Recipients { get; set; } }
        private sealed class ContactRow { public int ParentId { get; set; } public string ParentName { get; set; } public string StudentNames { get; set; } public string Phone { get; set; } public bool IsOptedIn { get; set; } public string ConsentState { get; set; } public string ConsentSource { get; set; } public string ConsentDate { get; set; } public string UpdatedBy { get; set; } }
        private sealed class DraftRow { public int Id { get; set; } public string Title { get; set; } public string ClassName { get; set; } public DateTime CreatedAt { get; set; } public DateTime? ScheduledAt { get; set; } public int RecipientCount { get; set; } public string Status { get; set; } }
        private sealed class InboxRow { public int Id { get; set; } public DateTime CreatedAtLocal { get; set; } public string RecipientName { get; set; } public string PhoneNumber { get; set; } public string MessageBody { get; set; } }
        private sealed class LogRow { public int Id { get; set; } public DateTime CreatedAtLocal { get; set; } public string RecipientName { get; set; } public string PhoneNumber { get; set; } public string MessageBody { get; set; } public string Status { get; set; } public string ErrorDetails { get; set; } public string CreatedBy { get; set; } public bool CanRetry { get; set; } public bool CanCancel { get; set; } public bool CanResend { get; set; } public bool CanRequeue { get; set; } public string WhatsAppLink { get; set; } public string DirectionText { get; set; } }
        private sealed class OutboxRow { public int Id { get; set; } public string RecipientName { get; set; } public string PhoneNumber { get; set; } public string MessagePreview { get; set; } public DateTime CreatedAtLocal { get; set; } public DateTime? ScheduledAt { get; set; } public string WhatsAppLink { get; set; } public bool IsReply { get; set; } public bool IsBlocked { get; set; } public bool IsScheduledFuture { get; set; } public string BlockReason { get; set; } }
        private sealed class StatusCountItem { public string Status { get; set; } public int Total { get; set; } }
    }
}
