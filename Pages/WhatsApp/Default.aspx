<%@ Page Title="واتساب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.WhatsApp.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/whatsapp-module.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <main class="wa-page" dir="rtl">
        <section class="wa-hero">
            <div class="wa-hero-mark"><i class="fab fa-whatsapp" aria-hidden="true"></i></div>
            <div class="wa-hero-copy">
                <span class="wa-eyebrow">التواصل المدرسي</span>
                <h1>واتساب</h1>
                <p>رسائل أولياء الأمور، الإعلانات، القوالب والمتابعة من مكان واحد.</p>
            </div>
            <div class="wa-connection">
                <span class="wa-status-dot"></span>
                <asp:Literal ID="litConnectionStatus" runat="server" Text="غير مهيأ" />
                <small><asp:Literal ID="litConnectionNumber" runat="server" Text="" /></small>
                <small><asp:Literal ID="litWebhookStatus" runat="server" Text="" /></small>
                <asp:Button ID="btnTestConnection" runat="server" Text="اختبار الاتصال" CssClass="btn wa-light-btn wa-connection-btn" OnClick="btnTestConnection_Click" CausesValidation="false" />
            </div>
        </section>

        <asp:Panel ID="pnlNotice" runat="server" CssClass="wa-notice" Visible="false" role="status">
            <i class="fas fa-info-circle" aria-hidden="true"></i>
            <asp:Literal ID="litNotice" runat="server" Mode="Encode" />
        </asp:Panel>
        <asp:Panel ID="pnlDbError" runat="server" CssClass="wa-notice wa-notice-warning" Visible="false" role="alert">
            <i class="fas fa-plug" aria-hidden="true"></i>
            <div><strong>تعذر الوصول إلى قاعدة البيانات</strong><p><asp:Literal ID="litDbError" runat="server" Mode="Encode" Text="تحقق من اتصال الخادم بقاعدة البيانات ثم أعد فتح الصفحة." /></p></div>
        </asp:Panel>
        <asp:Panel ID="pnlSchemaWarning" runat="server" CssClass="wa-notice wa-notice-warning" Visible="false" role="alert">
            <i class="fas fa-database" aria-hidden="true"></i>
            <div><strong>يلزم تجهيز جداول الوحدة</strong><p>نفّذ ملفي التحديث <code>Database/Migrations/20261015_AddWhatsAppModule.sql</code> ثم <code>Database/Migrations/20261016_EnhanceWhatsAppModule.sql</code> على قاعدة البيانات، ثم أعد فتح الصفحة.</p></div>
        </asp:Panel>
        <asp:Panel ID="pnlSchedulerWarning" runat="server" CssClass="wa-notice wa-notice-warning" Visible="false" role="alert">
            <i class="fas fa-clock" aria-hidden="true"></i>
            <div><strong>متابعة الإرسال المجدول</strong><p><asp:Literal ID="litSchedulerWarning" runat="server" Mode="Encode" /></p></div>
        </asp:Panel>

        <nav class="wa-tabs" aria-label="أقسام واتساب">
            <a class='<%= TabCss("compose") %>' href='<%= ViewUrl("compose") %>'><i class="fas fa-bullhorn"></i> إعلان ورسالة</a>
            <a class='<%= TabCss("groups") %>' href='<%= ViewUrl("groups") %>'><i class="fas fa-users"></i> ربط الشعب</a>
            <a class='<%= TabCss("drafts") %>' href='<%= ViewUrl("drafts") %>'><i class="fas fa-file-alt"></i> المسودات</a>
            <a class='<%= TabCss("outbox") %>' href='<%= ViewUrl("outbox") %>'><i class="fas fa-paper-plane"></i> الإرسال اليدوي</a>
            <a class='<%= TabCss("inbox") %>' href='<%= ViewUrl("inbox") %>'><i class="fas fa-comments"></i> الاستفسارات</a>
            <a class='<%= TabCss("log") %>' href='<%= ViewUrl("log") %>'><i class="fas fa-history"></i> السجل</a>
            <a class='<%= TabCss("templates") %>' href='<%= ViewUrl("templates") %>'><i class="fas fa-copy"></i> القوالب</a>
            <a class='<%= TabCss("consent") %>' href='<%= ViewUrl("consent") %>'><i class="fas fa-user-shield"></i> الموافقات</a>
        </nav>

        <asp:Panel ID="pnlCompose" runat="server" CssClass="wa-panel">
            <div class="wa-panel-heading">
                <div><span class="wa-eyebrow">إنشاء حملة</span><h2>رسالة جديدة</h2><p>تُرسل الرسالة منفصلة لكل ولي أمر لديه رقم وموافقة مسجلة. ولي الأمر الذي لديه أكثر من طالب في نفس الفصل تصله رسالة واحدة تجمع بياناتهم.</p></div>
                <span class="wa-heading-icon"><i class="fab fa-whatsapp"></i></span>
            </div>
            <div class="wa-form-grid">
                <div class="wa-field">
                    <asp:Label ID="lblCampaignTitle" runat="server" AssociatedControlID="txtCampaignTitle" Text="عنوان داخلي للحملة" />
                    <asp:TextBox ID="txtCampaignTitle" runat="server" CssClass="form-control" MaxLength="200" placeholder="مثال: تنبيه غياب يوم الأحد" />
                </div>
                <div class="wa-field">
                    <asp:Label ID="lblClass" runat="server" AssociatedControlID="ddlClass" Text="الفصل / قائمة المستلمين" />
                    <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlClass_SelectedIndexChanged" />
                </div>
                <div class="wa-field">
                    <asp:Label ID="lblAudienceType" runat="server" AssociatedControlID="ddlAudienceType" Text="نوع الإرسال" />
                    <asp:DropDownList ID="ddlAudienceType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAudienceType_SelectedIndexChanged">
                        <asp:ListItem Value="Class" Text="لكل أولياء الفصل" />
                        <asp:ListItem Value="Individual" Text="لولي أمر طالب واحد" />
                        <asp:ListItem Value="School" Text="لجميع أولياء المدرسة (النطاق الحالي)" />
                    </asp:DropDownList>
                </div>
                <div class="wa-field">
                    <asp:Label ID="lblKind" runat="server" AssociatedControlID="ddlMessageKind" Text="تصنيف الرسالة" />
                    <asp:DropDownList ID="ddlMessageKind" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlMessageKind_SelectedIndexChanged">
                        <asp:ListItem Value="Announcement" Text="إعلان عام" />
                        <asp:ListItem Value="Attendance" Text="الحضور والغياب" />
                        <asp:ListItem Value="Academic" Text="أكاديمي ودرجات" />
                        <asp:ListItem Value="Financial" Text="مالي وأقساط" />
                        <asp:ListItem Value="Administrative" Text="إداري" />
                        <asp:ListItem Value="Free" Text="رسالة حرة" />
                    </asp:DropDownList>
                </div>
                <div class="wa-field">
                    <asp:Label ID="lblTemplate" runat="server" AssociatedControlID="ddlTemplate" Text="قالب جاهز" />
                    <asp:DropDownList ID="ddlTemplate" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplate_SelectedIndexChanged" />
                </div>
            </div>

            <div class="wa-recipient-summary">
                <i class="fas fa-user-check" aria-hidden="true"></i>
                <asp:Literal ID="litRecipientSummary" runat="server" Text="اختر الفصل لعرض عدد أولياء الأمور المستعدين للإرسال." />
                <span class="wa-summary-separator">•</span>
                <span>لا تُرسل إلى رقم دون موافقة فعّالة.</span>
            </div>
            <asp:Panel ID="pnlIndividualRecipient" runat="server" CssClass="wa-individual-recipient" Visible="false">
                <div class="wa-field"><asp:Label ID="lblStudent" runat="server" AssociatedControlID="ddlStudent" Text="الطالب وولي الأمر" /><asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged" /></div>
            </asp:Panel>

            <div class="wa-field wa-message-field">
                <asp:Label ID="lblMessage" runat="server" AssociatedControlID="txtMessage" Text="نص الرسالة" />
                <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control wa-message-box" TextMode="MultiLine" Rows="6" MaxLength="4000" placeholder="اكتب الرسالة أو اختر قالباً. يمكنك استخدام {guardianName} و{studentName} و{class} و{date} و{balance} و{attendanceSummary} و{latestGrade} و{extra}. القوالب المعتمدة تدعم أيضاً {{1}} و{{2}} بالترتيب." />
                <small class="wa-help">المتغيرات تُملأ لكل طالب. القالب المحلي لا يصبح معتمداً تلقائياً لدى Meta.</small>
            </div>

            <div class="wa-options-grid">
                <asp:PlaceHolder ID="pnlFinancialOption" runat="server"><label class="wa-option"><asp:CheckBox ID="chkIncludeBalance" runat="server" /><span><strong>الوضع المالي</strong><small>تعبئة {balance} عند استخدامه في نص الرسالة</small></span></label></asp:PlaceHolder>
                <label class="wa-option"><asp:CheckBox ID="chkIncludeAttendance" runat="server" /><span><strong>ملخص الحضور</strong><small>تعبئة {attendanceSummary} عند استخدامه</small></span></label>
                <label class="wa-option"><asp:CheckBox ID="chkIncludeGrades" runat="server" /><span><strong>آخر الدرجات</strong><small>تعبئة {latestGrade} عند استخدامه</small></span></label>
                <label class="wa-option"><asp:CheckBox ID="chkAppendDailyLessons" runat="server" AutoPostBack="true" OnCheckedChanged="chkAppendDailyLessons_CheckedChanged" /><span><strong>ملخص دروس اليوم</strong><small>إضافة دروس الفصل المحدد تلقائياً</small></span></label>
            </div>
            <asp:Panel ID="pnlDailyLessons" runat="server" CssClass="wa-daily-preview" Visible="false">
                <strong><i class="fas fa-calendar-day" aria-hidden="true"></i> دروس اليوم</strong>
                <asp:Literal ID="litDailyLessons" runat="server" Mode="Encode" />
            </asp:Panel>

            <asp:Panel ID="pnlManagerSummary" runat="server" CssClass="wa-manager-summary" Visible="false">
                <div><strong><i class="fas fa-user-tie" aria-hidden="true"></i> ملخص اليوم للمدير</strong><small>ملخص مجمّع للفصل المحدد: عدد الطلاب والغياب ودروس اليوم، دون أسماء الطلاب.</small><small>رقم المدير: <asp:Literal ID="litManagerPhone" runat="server" Mode="Encode" /></small></div>
                <label class="wa-manager-confirm"><asp:CheckBox ID="chkManagerConfirmed" runat="server" /><span>أؤكد أن الرقم المهيأ يخص المدير ووافق على استلام الملخص.</span></label>
                <asp:Button ID="btnSendManagerSummary" runat="server" Text="إرسال ملخص اليوم للمدير" CssClass="btn wa-secondary-btn" OnClick="btnSendManagerSummary_Click" />
            </asp:Panel>

            <div class="wa-schedule-row">
                <div class="wa-field">
                    <asp:Label ID="lblSchedule" runat="server" AssociatedControlID="txtScheduledAt" Text="موعد الإرسال (اختياري)" />
                    <asp:TextBox ID="txtScheduledAt" runat="server" CssClass="form-control" TextMode="DateTimeLocal" step="60" />
                    <small class="wa-help">تُحفظ الرسالة في الطابور، ويعالجها المجدول الموجود على الخادم.</small>
                </div>
                <div class="wa-actions">
                    <asp:Button ID="btnSendNow" runat="server" Text="إرسال الآن" CssClass="btn wa-primary-btn" OnClick="btnSendNow_Click" />
                    <asp:Button ID="btnSchedule" runat="server" Text="جدولة الإرسال" CssClass="btn wa-secondary-btn" OnClick="btnSchedule_Click" />
                    <asp:Button ID="btnSaveDraft" runat="server" Text="حفظ كمسودة" CssClass="btn wa-light-btn" OnClick="btnSaveDraft_Click" CausesValidation="false" />
                    <asp:Button ID="btnResetCompose" runat="server" Text="تفريغ" CssClass="btn wa-text-btn" OnClick="btnResetCompose_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlGroups" runat="server" CssClass="wa-panel" Visible="false">
            <div class="wa-panel-heading"><div><span class="wa-eyebrow">قوائم المستلمين</span><h2>ربط الشعب الدراسية</h2><p>أسماء تنظيمية لقوائم أولياء الأمور. يرسل النظام لكل رقم على حدة ولا ينشر داخل مجموعة واتساب فعلية.</p></div></div>
            <div class="wa-inline-form">
                <div class="wa-field"><asp:Label ID="lblMappingClass" runat="server" AssociatedControlID="ddlMappingClass" Text="الفصل" /><asp:DropDownList ID="ddlMappingClass" runat="server" CssClass="form-select" /></div>
                <div class="wa-field"><asp:Label ID="lblMappingName" runat="server" AssociatedControlID="txtMappingName" Text="اسم القائمة" /><asp:TextBox ID="txtMappingName" runat="server" CssClass="form-control" MaxLength="150" placeholder="مثال: أول ثانوي - أ" /></div>
                <asp:Button ID="btnSaveMapping" runat="server" Text="حفظ الربط" CssClass="btn wa-primary-btn" OnClick="btnSaveMapping_Click" />
            </div>
            <asp:GridView ID="gvMappings" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لم تُسجل قوائم بعد." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvMappings_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvMappings_RowCommand" OnRowDataBound="Grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="ClassName" HeaderText="الفصل / الشعبة" />
                    <asp:BoundField DataField="DisplayName" HeaderText="اسم القائمة" />
                    <asp:BoundField DataField="Recipients" HeaderText="أولياء لديهم موافقة" />
                    <asp:TemplateField HeaderText="الحالة"><ItemTemplate><span class='wa-badge <%# (bool)Eval("IsActive") ? "is-ok" : "is-muted" %>'><%# (bool)Eval("IsActive") ? "فعّالة" : "موقوفة" %></span></ItemTemplate></asp:TemplateField>
                    <asp:TemplateField HeaderText="إجراء"><ItemTemplate><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="toggle" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"><%# (bool)Eval("IsActive") ? "إيقاف" : "تفعيل" %></asp:LinkButton></ItemTemplate></asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlDrafts" runat="server" CssClass="wa-panel" Visible="false">
            <div class="wa-panel-heading"><div><span class="wa-eyebrow">المحتوى غير المرسل</span><h2>المسودات والرسائل المجدولة</h2><p>يمكنك فتح المسودة لتعديلها، ومراجعة الرسائل التي تنتظر موعدها.</p></div></div>
            <asp:GridView ID="gvDrafts" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد مسودات أو رسائل مجدولة." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvDrafts_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvDrafts_RowCommand" OnRowDataBound="Grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="الحملة" />
                    <asp:BoundField DataField="ClassName" HeaderText="الفصل" />
                    <asp:BoundField DataField="CreatedAt" HeaderText="تاريخ الإنشاء" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:BoundField DataField="ScheduledAt" HeaderText="موعد الإرسال" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:BoundField DataField="RecipientCount" HeaderText="عدد الرسائل" />
                    <asp:BoundField DataField="Status" HeaderText="الحالة" />
                    <asp:TemplateField HeaderText="إجراء"><ItemTemplate><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="open" CommandArgument='<%# Eval("Id") %>' Visible='<%# Convert.ToString(Eval("Status")) == "مسودة" %>' CausesValidation="false">فتح</asp:LinkButton><asp:LinkButton runat="server" CssClass="wa-row-action wa-row-danger" CommandName="cancel" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return waConfirm('هل تريد إلغاء هذه الحملة ورسائلها غير المرسلة؟');">إلغاء</asp:LinkButton></ItemTemplate></asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlOutbox" runat="server" CssClass="wa-panel" Visible="false">
            <div class="wa-panel-heading"><div><span class="wa-eyebrow">بدون رسوم — إرسال عبر واتساب</span><h2>صندوق الإرسال اليدوي</h2><p>النظام في هذا الوضع لا يُرسل أي رسالة تلقائياً؛ أنت من يرسل عبر واتساب بنفسك.</p><small class="wa-stats"><asp:Literal ID="litOutboxHint" runat="server" Mode="Encode" /></small></div>
                <div class="wa-toolbar">
                    <asp:TextBox ID="txtOutboxSearch" runat="server" CssClass="form-control wa-search" placeholder="بحث بالرقم أو الاسم أو النص" AutoPostBack="true" OnTextChanged="OutboxSearch_TextChanged" />
                    <asp:LinkButton ID="btnClearOutboxSearch" runat="server" CssClass="wa-row-action" OnClick="ClearOutboxSearch_Click" CausesValidation="false">مسح</asp:LinkButton>
                </div>
            </div>
            <div class="wa-warning">
                <strong><i class="fas fa-triangle-exclamation" aria-hidden="true"></i> خطوات الإرسال الصحيحة</strong>
                <ol>
                    <li>اضغط «فتح في واتساب ↗» بجانب الرسالة لتفتح واتساب بالنص جاهزاً.</li>
                    <li>اضغط «إرسال» داخل تطبيق واتساب نفسه.</li>
                    <li>عُد إلى هذه الصفحة واضغط «تسجيل كمُرسل» لتحديث السجل.</li>
                </ol>
                <small class="wa-help">«تسجيل كمُرسل» لا يُرسل شيئاً؛ إنه يحدّث الحالة فقط، ولن يعمل قبل أن تفتح رابط واتساب للرسالة. الرسائل تُرسل من حساب واتساب الموظف الذي يفتح الرابط.</small>
            </div>
            <asp:HiddenField ID="hfOpenedOutboxIds" runat="server" />
            <asp:GridView ID="gvOutbox" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد رسائل بانتظار الإرسال اليدوي." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvOutbox_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvOutbox_RowCommand" OnRowDataBound="Grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="RecipientName" HeaderText="المستلم" />
                    <asp:BoundField DataField="PhoneNumber" HeaderText="رقم واتساب" />
                    <asp:BoundField DataField="MessagePreview" HeaderText="نص الرسالة" />
                    <asp:BoundField DataField="ScheduledAt" HeaderText="موعد الإرسال" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:TemplateField HeaderText="إجراء"><ItemTemplate><asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("IsBlocked") %>'><span class="wa-badge is-danger"><%# Eval("BlockReason") %></span></asp:PlaceHolder><asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("IsScheduledFuture") %>'><span class="wa-badge is-muted"><%# Eval("BlockReason") %></span></asp:PlaceHolder><asp:PlaceHolder runat="server" Visible='<%# !(bool)Eval("IsBlocked") %>'><a class="wa-row-action wa-row-send" href='<%# Eval("WhatsAppLink") %>' target="_blank" rel="noopener" onclick="waRecordOutboxOpen(<%# Eval("Id") %>);">فتح في واتساب ↗</a><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="sent" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" Visible='<%# !(bool)Eval("IsScheduledFuture") %>' OnClientClick="return waConfirm('تنبيه: هذا الزر لا يُرسل الرسالة، بل يسجّل أنها أُرسلت فقط. تأكد أنك ضغطت «إرسال» داخل واتساب قبل المتابعة. هل أرسلتها فعلاً؟');">تسجيل كمُرسل</asp:LinkButton></asp:PlaceHolder><asp:LinkButton runat="server" CssClass="wa-row-action wa-row-danger" CommandName="cancel" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return waConfirm('إلغاء هذه الرسالة دون إرسال؟');">إلغاء</asp:LinkButton></ItemTemplate></asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlInbox" runat="server" CssClass="wa-panel" Visible="false">
            <div class="wa-panel-heading"><div><span class="wa-eyebrow">محادثات أولياء الأمور</span><h2>الاستفسارات والردود</h2><p>تظهر الرسائل الواردة بعد إعداد Webhook. الرد الحر متاح ضمن نافذة الخدمة التي تسمح بها Meta.</p></div>
                <div class="wa-toolbar">
                    <asp:TextBox ID="txtInboxSearch" runat="server" CssClass="form-control wa-search" placeholder="بحث بالرقم أو الاسم" AutoPostBack="true" OnTextChanged="InboxSearch_TextChanged" />
                    <asp:LinkButton ID="btnClearInboxSearch" runat="server" CssClass="wa-row-action" OnClick="ClearInboxSearch_Click" CausesValidation="false">مسح</asp:LinkButton>
                </div>
            </div>
            <asp:GridView ID="gvInbox" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد رسائل واردة حتى الآن." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvInbox_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvInbox_RowCommand" OnRowDataBound="Grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="CreatedAtLocal" HeaderText="الوقت" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:BoundField DataField="RecipientName" HeaderText="ولي الأمر" />
                    <asp:BoundField DataField="PhoneNumber" HeaderText="الرقم" />
                    <asp:BoundField DataField="MessageBody" HeaderText="الرسالة" />
                    <asp:TemplateField HeaderText="إجراء"><ItemTemplate><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="reply" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">رد</asp:LinkButton></ItemTemplate></asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Panel ID="pnlReply" runat="server" CssClass="wa-reply-box" Visible="false">
                <asp:HiddenField ID="hdnReplyParentId" runat="server" />
                <asp:HiddenField ID="hdnReplyPhone" runat="server" />
                <strong>الرد على <asp:Literal ID="litReplyTo" runat="server" Mode="Encode" /></strong>
                <small class="wa-help"><asp:Literal ID="litReplyPolicy" runat="server" Mode="Encode" /></small>
                <asp:DropDownList ID="ddlReplyTemplate" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlReplyTemplate_SelectedIndexChanged" />
                <asp:TextBox ID="txtReply" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="4000" />
                <asp:Button ID="btnSendReply" runat="server" Text="إرسال الرد" CssClass="btn wa-primary-btn" OnClick="btnSendReply_Click" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlLog" runat="server" CssClass="wa-panel" Visible="false">
            <div class="wa-panel-heading"><div><span class="wa-eyebrow">التتبع</span><h2>سجل الرسائل</h2><p>يعرض حالة قبول الرسالة وتسليمها وقراءتها أو فشلها، مع إمكانية إعادة محاولة الفاشلة.</p><small class="wa-stats"><asp:Literal ID="litLogStats" runat="server" Mode="Encode" /></small></div>
                <div class="wa-toolbar">
                    <asp:TextBox ID="txtLogSearch" runat="server" CssClass="form-control wa-search" placeholder="بحث بالرقم أو الاسم أو النص" AutoPostBack="true" OnTextChanged="LogSearch_TextChanged" />
                    <asp:LinkButton ID="btnClearLogSearch" runat="server" CssClass="wa-row-action" OnClick="ClearLogSearch_Click" CausesValidation="false">مسح</asp:LinkButton>
                    <asp:DropDownList ID="ddlLogDirection" runat="server" CssClass="form-select wa-filter" AutoPostBack="true" OnSelectedIndexChanged="ddlLogDirection_SelectedIndexChanged"><asp:ListItem Value="" Text="صادر ووارد" /><asp:ListItem Value="Outbound" Text="صادر فقط" /><asp:ListItem Value="Inbound" Text="وارد فقط" /></asp:DropDownList>
                    <asp:DropDownList ID="ddlLogStatus" runat="server" CssClass="form-select wa-filter" AutoPostBack="true" OnSelectedIndexChanged="ddlLogStatus_SelectedIndexChanged"><asp:ListItem Value="" Text="كل الحالات" /><asp:ListItem Value="Sent" Text="قُبلت للإرسال" /><asp:ListItem Value="Delivered" Text="تم التسليم" /><asp:ListItem Value="Read" Text="مقروءة" /><asp:ListItem Value="Failed" Text="فشلت" /><asp:ListItem Value="Scheduled" Text="مجدولة" /><asp:ListItem Value="Received" Text="واردة" /></asp:DropDownList>
                    <asp:LinkButton ID="btnExportLog" runat="server" CssClass="wa-row-action" OnClick="btnExportLog_Click" CausesValidation="false">تصدير CSV</asp:LinkButton>
                </div>
            </div>
            <asp:GridView ID="gvLog" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد رسائل في السجل." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvLog_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvLog_RowCommand" OnRowDataBound="Grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="CreatedAtLocal" HeaderText="الوقت" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:BoundField DataField="DirectionText" HeaderText="الاتجاه" />
                    <asp:BoundField DataField="RecipientName" HeaderText="المستلم" />
                    <asp:BoundField DataField="PhoneNumber" HeaderText="رقم الهاتف" />
                    <asp:BoundField DataField="MessageBody" HeaderText="نص الرسالة" />
                    <asp:TemplateField HeaderText="الحالة"><ItemTemplate><span class='wa-badge <%# StatusCss(Convert.ToString(Eval("Status"))) %>'><%# StatusText(Convert.ToString(Eval("Status"))) %></span><asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("ErrorDetails"))) %>'><span class="wa-error"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("ErrorDetails") %>' /></span></asp:PlaceHolder></ItemTemplate></asp:TemplateField>
                    <asp:BoundField DataField="CreatedBy" HeaderText="بواسطة" />
                    <asp:TemplateField HeaderText="إجراء"><ItemTemplate><asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("WhatsAppLink"))) %>'><a class="wa-row-action wa-row-send" href='<%# Eval("WhatsAppLink") %>' target="_blank" rel="noopener">فتح في واتساب ↗</a></asp:PlaceHolder><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="retry" CommandArgument='<%# Eval("Id") %>' Visible='<%# (bool)Eval("CanRetry") %>' CausesValidation="false">إعادة المحاولة</asp:LinkButton><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="cancel" CommandArgument='<%# Eval("Id") %>' Visible='<%# (bool)Eval("CanCancel") %>' CausesValidation="false" OnClientClick="return waConfirm('هل تريد إلغاء هذه الرسالة قبل إرسالها؟');">إلغاء</asp:LinkButton><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="resend" CommandArgument='<%# Eval("Id") %>' Visible='<%# (bool)Eval("CanResend") %>' CausesValidation="false" OnClientClick="return waConfirm('سيتم إنشاء نسخة جديدة وإرسالها. متابعة؟');">إعادة إرسال</asp:LinkButton><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="requeue" CommandArgument='<%# Eval("Id") %>' Visible='<%# (bool)Eval("CanRequeue") %>' CausesValidation="false" OnClientClick="return waConfirm('ستعود الرسالة إلى صندوق الإرسال اليدوي لإعادة إرسالها من واتساب. متابعة؟');">إعادة إلى الصندوق</asp:LinkButton></ItemTemplate></asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlTemplates" runat="server" CssClass="wa-panel" Visible="false">
            <div class="wa-panel-heading"><div><span class="wa-eyebrow">رسائل قابلة لإعادة الاستخدام</span><h2>قوالب الرسائل</h2><p>القوالب المحلية تساعد على الصياغة. الإرسال خارج نافذة الخدمة يتطلب قالباً معتمداً من Meta.</p></div></div>
            <asp:HiddenField ID="hdnTemplateId" runat="server" />
            <div class="wa-inline-form wa-template-form">
                <div class="wa-field"><asp:Label ID="lblTemplateName" runat="server" AssociatedControlID="txtTemplateName" Text="اسم القالب" /><asp:TextBox ID="txtTemplateName" runat="server" CssClass="form-control" MaxLength="150" /></div>
                <div class="wa-field"><asp:Label ID="lblTemplateCategory" runat="server" AssociatedControlID="ddlTemplateCategory" Text="التصنيف" /><asp:DropDownList ID="ddlTemplateCategory" runat="server" CssClass="form-select"><asp:ListItem Text="إعلان" Value="إعلان" /><asp:ListItem Text="مالي" Value="مالي" /><asp:ListItem Text="حضور" Value="حضور" /><asp:ListItem Text="إداري" Value="إداري" /><asp:ListItem Text="أكاديمي" Value="أكاديمي" /></asp:DropDownList></div>
                <div class="wa-field"><asp:Label ID="lblProviderTemplateName" runat="server" AssociatedControlID="txtProviderTemplateName" Text="اسم القالب المعتمد لدى Meta (إن وجد)" /><asp:TextBox ID="txtProviderTemplateName" runat="server" CssClass="form-control" MaxLength="100" /></div>
                <div class="wa-field"><asp:Label ID="lblTemplateLanguage" runat="server" AssociatedControlID="txtTemplateLanguage" Text="لغة القالب" /><asp:TextBox ID="txtTemplateLanguage" runat="server" CssClass="form-control" MaxLength="20" Text="ar" /></div>
                <div class="wa-field wa-field-wide"><asp:Label ID="lblTemplateBody" runat="server" AssociatedControlID="txtTemplateBody" Text="النص ومتغيراته" /><asp:TextBox ID="txtTemplateBody" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="4000" /></div>
                <label class="wa-option wa-approval-option"><asp:CheckBox ID="chkTemplateApproved" runat="server" /><span><strong>اعتماد Meta مؤكد</strong><small>فعّلها فقط بعد اعتماد الاسم واللغة والمحتوى في WhatsApp Manager.</small></span></label>
                <div class="wa-actions"><asp:Button ID="btnSaveTemplate" runat="server" Text="حفظ القالب" CssClass="btn wa-primary-btn" OnClick="btnSaveTemplate_Click" /><asp:Button ID="btnClearTemplate" runat="server" Text="قالب جديد" CssClass="btn wa-light-btn" OnClick="btnClearTemplate_Click" CausesValidation="false" /></div>
            </div>
            <asp:GridView ID="gvTemplates" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد قوالب." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvTemplates_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvTemplates_RowCommand" OnRowDataBound="Grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="القالب" />
                    <asp:BoundField DataField="Category" HeaderText="التصنيف" />
                    <asp:BoundField DataField="ProviderTemplateName" HeaderText="اسم Meta" />
                    <asp:TemplateField HeaderText="اعتماد Meta"><ItemTemplate><span class='wa-badge <%# (bool)Eval("IsMetaApproved") ? "is-ok" : "is-muted" %>'><%# (bool)Eval("IsMetaApproved") ? "معتمد" : "محلي فقط" %></span></ItemTemplate></asp:TemplateField>
                    <asp:TemplateField HeaderText="إجراء"><ItemTemplate><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="edit" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">تعديل</asp:LinkButton><asp:LinkButton runat="server" CssClass="wa-row-action wa-row-danger" CommandName="toggle" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"><%# (bool)Eval("IsActive") ? "إيقاف" : "تفعيل" %></asp:LinkButton></ItemTemplate></asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlConsent" runat="server" CssClass="wa-panel" Visible="false">
            <div class="wa-panel-heading"><div><span class="wa-eyebrow">حماية خصوصية الأسرة</span><h2>موافقات أولياء الأمور</h2><p>سجّل موافقة ولي الأمر ومصدرها قبل أي إرسال. إلغاء الموافقة يوقف الرسائل المستقبلية فوراً.</p></div>
                <div class="wa-toolbar">
                    <asp:TextBox ID="txtConsentSearch" runat="server" CssClass="form-control wa-search" placeholder="بحث بالاسم أو الرقم" AutoPostBack="true" OnTextChanged="ConsentSearch_TextChanged" />
                    <asp:LinkButton ID="btnClearConsentSearch" runat="server" CssClass="wa-row-action" OnClick="ClearConsentSearch_Click" CausesValidation="false">مسح</asp:LinkButton>
                    <asp:LinkButton ID="btnExportConsents" runat="server" CssClass="wa-row-action" OnClick="btnExportConsents_Click" CausesValidation="false">تصدير CSV</asp:LinkButton>
                </div>
            </div>
            <div class="wa-inline-form wa-consent-form">
                <div class="wa-field"><asp:Label ID="lblConsentClass" runat="server" AssociatedControlID="ddlConsentClass" Text="الفصل" /><asp:DropDownList ID="ddlConsentClass" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlConsentClass_SelectedIndexChanged" /></div>
                <div class="wa-field"><asp:Label ID="lblConsentSource" runat="server" AssociatedControlID="txtConsentSource" Text="مصدر التحقق من الموافقة" /><asp:TextBox ID="txtConsentSource" runat="server" CssClass="form-control" MaxLength="300" placeholder="مثال: نموذج موافقة ورقي بتاريخ 2026-10-15" /></div>
                <label class="wa-option"><asp:CheckBox ID="chkConsentVerified" runat="server" /><span><strong>تحققت من وجود موافقة موثقة</strong><small>لا تفعّلها اعتماداً على مجرد وجود رقم هاتف في الملف.</small></span></label>
            </div>
            <asp:GridView ID="gvContacts" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا يوجد أولياء أمور بأرقام هاتف في هذا الفصل." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvContacts_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvContacts_RowCommand" OnRowDataBound="Grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="ParentName" HeaderText="ولي الأمر" />
                    <asp:BoundField DataField="StudentNames" HeaderText="الطالب / الطلاب" />
                    <asp:BoundField DataField="Phone" HeaderText="رقم الهاتف" />
                    <asp:TemplateField HeaderText="حالة الموافقة"><ItemTemplate><span class='wa-badge <%# (bool)Eval("IsOptedIn") ? "is-ok" : Convert.ToString(Eval("ConsentState")) == "ألغى الموافقة" ? "is-danger" : "is-muted" %>'><%# Eval("ConsentState") %></span></ItemTemplate></asp:TemplateField>
                    <asp:BoundField DataField="ConsentSource" HeaderText="مصدر الموافقة" />
                    <asp:BoundField DataField="ConsentDate" HeaderText="التاريخ" />
                    <asp:BoundField DataField="UpdatedBy" HeaderText="آخر تحديث بواسطة" />
                    <asp:TemplateField HeaderText="إجراء"><ItemTemplate><asp:LinkButton runat="server" CssClass="wa-row-action" CommandName="optin" CommandArgument='<%# Eval("ParentId") %>' CausesValidation="false">تسجيل الموافقة</asp:LinkButton><asp:LinkButton runat="server" CssClass="wa-row-action wa-row-danger" CommandName="optout" CommandArgument='<%# Eval("ParentId") %>' CausesValidation="false" Visible='<%# (bool)Eval("IsOptedIn") %>' OnClientClick="return waConfirm('هل تريد إلغاء الموافقة لهذا الرقم؟');">إلغاء</asp:LinkButton></ItemTemplate></asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
    </main>
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        function waConfirm(message) {
            return window.confirm(message || 'هل أنت متأكد؟');
        }
        function waGuard(button, message) {
            if (!window.confirm(message || 'هل أنت متأكد؟')) return false;
            if (button && !button.getAttribute('data-wa-busy')) {
                button.setAttribute('data-wa-busy', '1');
                window.setTimeout(function () { button.disabled = true; }, 0);
            }
            return true;
        }
        function waRecordOutboxOpen(id) {
            var field = document.getElementById('<%= hfOpenedOutboxIds.ClientID %>');
            if (!field) return true;
            var ids = (field.value || '').split(',');
            var key = String(id);
            if (ids.indexOf(key) === -1) { ids.push(key); field.value = ids.join(','); }
            return true;
        }
    </script>
</asp:Content>
