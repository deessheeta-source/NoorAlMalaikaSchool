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
                return new[] { "compose", "manager", "drafts", "outbox", "log", "templates", "consent", "settings" }.Contains(value)
                    ? value : "compose";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin, UserRole.Accountant, UserRole.Supervisor, UserRole.DataEntry);
            if (CurrentView == "settings" && !IsAdmin)
            {
                Response.Redirect(ViewUrl("compose"), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            if (CurrentView == "manager" && !IsAdmin)
            {
                Response.Redirect(ViewUrl("compose"), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            SetVisibleView();
            SetAudienceVisibility();
            SetAnnouncementVisibility();
            SetConnectionState();
            phManagerTab.Visible = IsAdmin;
            phSettingsTab.Visible = IsAdmin;
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
                ApplyComposeQuerySelection();
                BindTemplateSelector();
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
                case "Unknown": return "نتيجة الإرسال غير مؤكدة — راجع سجل Meta قبل الإعادة";
                case "NeedsReview": return "تحتاج مراجعة يدوية";
                case "Cancelled": return "أُلغيت";
                case "Received": return "واردة";
                default: return status ?? "—";
            }
        }

        public string StatusCss(string status)
        {
            if (status == "Failed" || status == "Unknown" || status == "Cancelled") return "is-danger";
            if (status == "Sent" || status == "Delivered" || status == "Read" || status == "Received") return "is-ok";
            return "is-muted";
        }

        private void SetVisibleView()
        {
            pnlCompose.Visible = CurrentView == "compose";
            pnlManager.Visible = CurrentView == "manager";
            pnlDrafts.Visible = CurrentView == "drafts";
            pnlOutbox.Visible = CurrentView == "outbox";
            pnlLog.Visible = CurrentView == "log";
            pnlTemplates.Visible = CurrentView == "templates";
            pnlConsent.Visible = CurrentView == "consent";
            pnlSettings.Visible = CurrentView == "settings";
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
            // Populate Compose Grade
            ddlComposeGrade.Items.Clear();
            ddlComposeGrade.Items.Add(new ListItem("اختر الفصل...", ""));
            var grades = db.GradeLevels.AsNoTracking().OrderBy(x => x.SortOrder).ToList();
            foreach (var g in grades) ddlComposeGrade.Items.Add(new ListItem(g.Name, g.Id.ToString()));

            // Consent class (keep as is)
            var classes = LoadClasses();
            BindClassDropDown(ddlConsentClass, classes, "اختر الفصل");

            if (ddlComposeGrade.Items.Count > 1)
            {
                ddlComposeGrade.SelectedIndex = 1;
                BindComposeDivisions();
            }

            if (classes.Count > 0) ddlConsentClass.SelectedValue = classes[0].Id.ToString(CultureInfo.InvariantCulture);
        }

        private void BindComposeDivisions()
        {
            ddlComposeDivision.Items.Clear();
            ddlComposeDivision.Items.Add(new ListItem("كل الأقسام", ""));
            ddlClass.Items.Clear();
            ddlClass.Items.Add(new ListItem("كل الشعب", ""));

            int gradeId;
            if (!int.TryParse(ddlComposeGrade.SelectedValue, out gradeId)) return;

            var divisions = db.ClassRooms.AsNoTracking()
                .Where(x => x.GradeLevelId == gradeId && !string.IsNullOrEmpty(x.DivisionName))
                .Select(x => x.DivisionName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (var div in divisions)
            {
                ddlComposeDivision.Items.Add(new ListItem(div, div));
            }

            BindComposeSections();
        }

        private void BindComposeSections()
        {
            ddlClass.Items.Clear();
            ddlClass.Items.Add(new ListItem("كل الشعب", ""));

            int gradeId;
            if (!int.TryParse(ddlComposeGrade.SelectedValue, out gradeId)) return;

            string division = ddlComposeDivision.SelectedValue;

            var query = db.ClassRooms.AsNoTracking().Where(x => x.GradeLevelId == gradeId);
            if (!string.IsNullOrEmpty(division)) query = query.Where(x => x.DivisionName == division);

            var rooms = query.OrderBy(x => x.SectionName).ToList();
            foreach (var room in rooms)
            {
                string name = string.IsNullOrWhiteSpace(room.SectionName) ? "شعبة 1" : room.SectionName;
                ddlClass.Items.Add(new ListItem(name, room.Id.ToString(CultureInfo.InvariantCulture)));
            }

            if (ddlClass.Items.Count > 1) ddlClass.SelectedIndex = 1;
            ddlClass_SelectedIndexChanged(null, null);
        }

        protected void ddlComposeGrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindComposeDivisions();
            UpdateRecipientSummary();
            BindDailyPreview();
        }

        protected void ddlComposeDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindComposeSections();
            UpdateRecipientSummary();
            BindDailyPreview();
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

        private void SetInitialDateTime()
        {
            txtScheduledAt.Text = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        }

        private void BindCurrentView()
        {
            switch (CurrentView)
            {
                case "compose": UpdateRecipientSummary(); BindDailyPreview(); break;
                case "manager": BindManagerSummary(); break;
                case "drafts": BindDrafts(); break;
                case "outbox": BindOutbox(); break;
                case "log": BindLog(); break;
                case "templates": BindTemplates(); break;
                case "consent": BindContacts(); break;
                case "settings": BindSettings(); break;
            }
        }

        // ============================================================
        // MANAGER SUMMARY TAB (cascading & aggregation)
        // ============================================================
    }
}