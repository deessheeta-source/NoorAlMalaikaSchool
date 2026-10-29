<%@ Page Title="واتساب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.WhatsApp.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* =========================================================
           Design System & Variables
           ========================================================= */
        :root {
            --wa-primary: #00b14f;
            --wa-primary-dark: #00923f;
            --wa-primary-light: #eaf8ef;
            --wa-secondary: #6f42c1;
            --wa-bg: #f4f7f6;
            --wa-card-bg: #ffffff;
            --wa-text-main: #1e293b;
            --wa-text-muted: #64748b;
            --wa-border: #e2e8f0;
            --wa-danger: #ef4444;
            --wa-warning: #f59e0b;
            --wa-radius-card: 16px;
            --wa-radius-input: 10px;
            --wa-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03);
            --wa-font: 'Droid Arabic Kufi', Tahoma, Arial, sans-serif;
        }

        body { background-color: var(--wa-bg); font-family: var(--wa-font); }
        .wa-page { min-height: 100vh; padding: 0; direction: rtl; }

        /* ---------- HERO HEADER ---------- */
        .wa-hero {
            position: relative;
            background: linear-gradient(135deg, var(--wa-primary) 0%, var(--wa-primary-dark) 100%);
            padding: 30px 30px 70px;
            border-radius: 0 0 30px 30px;
            color: #fff;
            overflow: hidden;
        }
        .wa-hero::before {
            content: ''; position: absolute; inset: 0;
            background: radial-gradient(circle at 10% 20%, rgba(255,255,255,0.1) 0%, transparent 40%),
                        radial-gradient(circle at 90% 80%, rgba(255,255,255,0.1) 0%, transparent 40%);
        }
        .wa-hero-content {
            position: relative; z-index: 2;
            display: flex; justify-content: space-between; align-items: center;
            max-width: 1300px; margin: 0 auto; flex-wrap: wrap; gap: 20px;
        }
        .wa-hero-left { display: flex; align-items: center; gap: 16px; }
        .wa-hero-icon {
            width: 60px; height: 60px; border-radius: 16px;
            background: rgba(255,255,255,0.2); backdrop-filter: blur(8px);
            border: 1px solid rgba(255,255,255,0.3);
            display: flex; align-items: center; justify-content: center;
            font-size: 28px; box-shadow: 0 8px 16px rgba(0,0,0,0.1);
        }
        .wa-hero-titles h1 { margin: 0 0 6px; font-size: 22px; font-weight: 800; display: flex; align-items: center; gap: 10px; }
        .wa-hero-titles p { margin: 0; font-size: 13px; color: rgba(255,255,255,0.85); }
        
        .wa-status-pill {
            display: inline-flex; align-items: center; gap: 6px;
            padding: 4px 12px; border-radius: 20px;
            background: rgba(255,255,255,0.15); border: 1px solid rgba(255,255,255,0.3);
            font-size: 11px; font-weight: 700;
        }
        .wa-status-dot { width: 8px; height: 8px; border-radius: 50%; background: #cbd5e1; }
        
        .wa-btn-glass {
            display: inline-flex; align-items: center; gap: 8px;
            padding: 10px 20px; border-radius: 10px;
            background: rgba(255,255,255,0.15); backdrop-filter: blur(8px);
            border: 1px solid rgba(255,255,255,0.3); color: #fff;
            font-size: 13px; font-weight: 700; cursor: pointer;
            transition: all 0.2s; text-decoration: none; font-family: var(--wa-font);
        }
        .wa-btn-glass:hover { background: rgba(255,255,255,0.25); transform: translateY(-2px); }

        /* ---------- MAIN LAYOUT ---------- */
        .wa-body {
            max-width: 1300px; margin: -50px auto 40px;
            padding: 0 20px; position: relative; z-index: 3;
        }

        /* ---------- NOTICES ---------- */
        .wa-notice {
            padding: 16px 20px; border-radius: var(--wa-radius-input);
            font-size: 13px; font-weight: 600; margin-bottom: 20px;
            display: flex; align-items: flex-start; gap: 12px;
            background: var(--wa-primary-light); color: var(--wa-primary-dark);
            border: 1px solid #bbf7d0;
        }
        .wa-notice-warning { background: #fffbeb; color: #b45309; border-color: #fde68a; }
        .wa-notice i { font-size: 18px; margin-top: 2px; }

        /* ---------- TABS ---------- */
        .wa-tabs-container {
            background: #fff; border-radius: 12px; box-shadow: var(--wa-shadow);
            margin-bottom: 24px; overflow-x: auto; white-space: nowrap;
        }
        .wa-tabs { display: flex; padding: 0 10px; }
        .wa-tabs a {
            display: inline-flex; align-items: center; gap: 8px;
            padding: 16px 20px; font-size: 14px; font-weight: 700;
            color: var(--wa-text-muted); text-decoration: none;
            border-bottom: 3px solid transparent; transition: all 0.2s;
        }
        .wa-tabs a:hover { color: var(--wa-primary); background: #f8fafc; }
        .wa-tabs a.active { color: var(--wa-primary); border-bottom-color: var(--wa-primary); }
        .wa-tabs a[href*="view=manager"].active { color: var(--wa-secondary); border-bottom-color: var(--wa-secondary); }

        /* ---------- CARDS ---------- */
        .wa-card {
            background: var(--wa-card-bg); border-radius: var(--wa-radius-card);
            border: 1px solid var(--wa-border); box-shadow: var(--wa-shadow);
            margin-bottom: 24px; overflow: hidden;
        }
        .wa-card-header {
            padding: 20px 24px; border-bottom: 1px solid var(--wa-border);
            background: #fdfdfd; display: flex; align-items: center; gap: 16px;
            flex-wrap: wrap;
        }
        .wa-card-header-icon {
            width: 42px; height: 42px; border-radius: 12px;
            background: var(--wa-primary-light); color: var(--wa-primary);
            display: flex; align-items: center; justify-content: center; font-size: 20px;
        }
        .wa-card-header-text h2 { margin: 0 0 4px; font-size: 16px; font-weight: 800; color: var(--wa-text-main); }
        .wa-card-header-text p { margin: 0; font-size: 12px; color: var(--wa-text-muted); }
        .wa-card-body { padding: 24px; }

        /* ---------- FORMS & GRIDS ---------- */
        .wa-form-grid {
            display: grid; grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
            gap: 20px; margin-bottom: 24px;
        }
        .wa-field { display: flex; flex-direction: column; gap: 8px; }
        .wa-field-wide { grid-column: 1 / -1; }
        
        .wa-field label {
            font-size: 13px; font-weight: 700; color: var(--wa-text-main);
            display: flex; align-items: center; gap: 6px;
        }
        
        /* Clean form controls */
        .form-control, .form-select {
            width: 100%; padding: 10px 14px; font-size: 14px;
            font-family: var(--wa-font); color: var(--wa-text-main);
            background-color: #f8fafc; border: 1.5px solid var(--wa-border);
            border-radius: var(--wa-radius-input); transition: all 0.2s;
            height: 44px;
        }
        textarea.form-control { height: auto; min-height: 120px; resize: vertical; line-height: 1.6; }
        
        .form-control:focus, .form-select:focus {
            background-color: #fff; border-color: var(--wa-primary);
            box-shadow: 0 0 0 3px var(--wa-primary-light); outline: none;
        }

        /* Dropdown fixes */
        .form-select { cursor: pointer; }
        .form-select option { direction: rtl; text-align: right; padding: 8px; font-family: var(--wa-font); font-size: 14px; }

        .wa-help { font-size: 11px; color: var(--wa-text-muted); line-height: 1.5; }

        /* Highlighted Guardian Box */
        .wa-selected-guardian {
            margin-top: 10px; padding: 12px 16px; border-radius: 10px;
            background: var(--wa-primary-light); border: 1px solid #a7f3d0;
            color: var(--wa-primary-dark);
        }
        .wa-selected-guardian-label { display: block; font-size: 11px; font-weight: 700; opacity: 0.8; margin-bottom: 4px; }
        .wa-selected-guardian strong { display: block; font-size: 15px; font-weight: 800; }

        /* Inline form group */
        .wa-inline-group {
            display: flex; gap: 16px; align-items: flex-start; flex-wrap: wrap;
            padding: 16px; background: #f8fafc; border: 1px solid var(--wa-border);
            border-radius: var(--wa-radius-input); margin-bottom: 24px;
        }
        .wa-inline-group .wa-field { flex: 1; min-width: 200px; }

        /* ---------- OPTIONS (Checkboxes) ---------- */
        .wa-options-grid {
            display: grid; grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
            gap: 16px; margin-bottom: 24px;
        }
        .wa-option {
            display: flex; align-items: flex-start; gap: 12px; padding: 14px;
            background: #fff; border: 1.5px solid var(--wa-border); border-radius: 10px;
            cursor: pointer; transition: all 0.2s;
        }
        .wa-option:hover { border-color: var(--wa-primary); background: #f8fafc; }
        .wa-option input[type="checkbox"] { width: 18px; height: 18px; margin-top: 2px; accent-color: var(--wa-primary); cursor: pointer; }
        .wa-option span { display: flex; flex-direction: column; gap: 4px; }
        .wa-option strong { font-size: 13px; color: var(--wa-text-main); }
        .wa-option small { font-size: 11px; color: var(--wa-text-muted); }

        /* ---------- ACTION BAR ---------- */
        .wa-action-bar {
            display: flex; justify-content: flex-end; align-items: center;
            gap: 12px; padding-top: 20px; border-top: 1px solid var(--wa-border);
            flex-wrap: wrap;
        }
        .wa-action-bar .wa-field { margin-right: auto; max-width: 300px; } /* Push schedule field to left */

        .wa-btn {
            display: inline-flex; align-items: center; justify-content: center; gap: 8px;
            padding: 0 24px; height: 44px; border-radius: var(--wa-radius-input);
            font-size: 14px; font-weight: 700; font-family: var(--wa-font);
            cursor: pointer; transition: all 0.2s; border: none;
        }
        .wa-btn-primary { background: var(--wa-primary); color: #fff; box-shadow: 0 4px 12px rgba(0,177,79,0.3); }
        .wa-btn-primary:hover { background: var(--wa-primary-dark); transform: translateY(-1px); }
        .wa-btn-outline { background: #fff; border: 1.5px solid var(--wa-primary); color: var(--wa-primary); }
        .wa-btn-outline:hover { background: var(--wa-primary-light); }
        .wa-btn-light { background: #f1f5f9; color: var(--wa-text-main); border: 1px solid var(--wa-border); }
        .wa-btn-light:hover { background: #e2e8f0; }
        .wa-btn-text { background: transparent; color: var(--wa-text-muted); padding: 0 16px; }
        .wa-btn-text:hover { color: var(--wa-danger); background: #fef2f2; }

        /* ---------- TABLES ---------- */
        .wa-table-wrapper { overflow-x: auto; border-radius: 12px; border: 1px solid var(--wa-border); }
        .wa-grid { width: 100%; border-collapse: collapse; font-size: 13px; background: #fff; }
        .wa-grid th {
            background: #f8fafc; color: var(--wa-text-main); font-weight: 800;
            padding: 14px 16px; text-align: right; border-bottom: 2px solid var(--wa-border);
            white-space: nowrap;
        }
        .wa-grid td { padding: 14px 8px; border-bottom: 1px solid var(--wa-border); color: var(--wa-text-main); vertical-align: middle; }
        .wa-grid tr:last-child td { border-bottom: none; }
        .wa-grid tr:hover td { background: #f8fafc; }
        
        .wa-pager { padding: 16px; text-align: center; border-top: 1px solid var(--wa-border); background: #f8fafc; }
        .wa-pager a, .wa-pager span { display: inline-block; padding: 6px 12px; margin: 0 2px; border-radius: 6px; background: #fff; border: 1px solid var(--wa-border); color: var(--wa-text-main); text-decoration: none; font-weight: 700; font-size: 12px; }
        .wa-pager span { background: var(--wa-primary); color: #fff; border-color: var(--wa-primary); }

        /* ---------- BADGES & ROW ACTIONS ---------- */
        .wa-badge { display: inline-flex; align-items: center; padding: 4px 10px; border-radius: 6px; font-size: 11px; font-weight: 700; }
        .wa-badge.is-ok { background: #dcfce7; color: #166534; }
        .wa-badge.is-danger { background: #fee2e2; color: #991b1b; }
        .wa-badge.is-muted { background: #f1f5f9; color: #475569; }

        .wa-row-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 11px;
            font-weight: 700;
            text-decoration: none;
            background: #eef1fb;
            border: 1px solid #c3d8f4;
            color: #0024b1;
            cursor: pointer;
            transition: all 0.15s;
            font-family: var(--wa-font);
        }
        .wa-row-btn:hover { border-color: var(--wa-primary); background: var(--wa-primary-light); }
        .wa-row-btn.danger { color: var(--wa-danger); }
        .wa-row-btn.danger:hover { border-color: var(--wa-danger); background: #fef2f2; }
        .wa-row-btn.send { color: #128c7e; }
        .wa-row-btn.send:hover { border-color: #128c7e; background: #e6f7f3; }

        /* ---------- SPECIAL BOXES ---------- */
        .wa-info-box {
            padding: 16px; border-radius: 10px; font-size: 13px;
            background: #f8fafc; border: 1px solid var(--wa-border);
            margin-bottom: 24px; line-height: 1.8;
        }
        .wa-info-box strong { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; color: var(--wa-text-main); }
        .wa-info-box.warning { background: #fffbeb; border-color: #fde68a; }
        .wa-info-box.warning strong { color: #b45309; }

        /* ---------- TOOLBARS ---------- */
        .wa-toolbar { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }
        .wa-toolbar .form-control, .wa-toolbar .form-select { height: 38px; font-size: 12px; width: auto; min-width: 150px; }

        @media (max-width: 768px) {
            .wa-hero { padding: 20px 20px 60px; border-radius: 0 0 20px 20px; }
            .wa-body { padding: 0 15px; margin-top: -40px; }
            .wa-action-bar { flex-direction: column-reverse; align-items: stretch; }
            .wa-action-bar .wa-field { max-width: 100%; }
            .wa-btn { width: 100%; }
        }

        .students-field .form-control,
.students-field .form-select {
    border: 1.5px solid #e9ecef;
    border-radius: 11px;
    padding: 10px 14px;
    font-size: 13px;
    font-weight: 500;
    color: #212529;
    transition: all 0.2s ease;
    background: #f9fafb;
    font-family: 'Droid Arabic Kufi', sans-serif;
    height: 42px;
}
.students-field .form-control:focus,
.students-field .form-select:focus {
    border-color: #00b14f;
    background: #fff;
    box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
    outline: none;
}
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <main class="wa-page">
        
        <!-- ================== HERO ================== -->
        <section class="wa-hero">
            <div class="wa-hero-content">
                <div class="wa-hero-left">
                    <div class="wa-hero-icon">
                        <i class="fab fa-whatsapp"></i>
                    </div>
                    <div class="wa-hero-titles">
                        <h1>
                            نظام المراسلات
                            <span class="wa-status-pill">
                                <span class="wa-status-dot"></span>
                                <asp:Literal ID="litConnectionStatus" runat="server" Text="غير مهيأ" />
                            </span>
                        </h1>
                        <p>
                            <asp:Literal ID="litConnectionNumber" runat="server" Text="" />
                            <asp:Literal ID="litWebhookStatus" runat="server" Text="" />
                        </p>
                    </div>
                </div>
                <div class="wa-hero-actions">
                    <asp:Button ID="btnTestConnection" runat="server" Text="اختبار الاتصال" CssClass="wa-btn-glass" OnClick="btnTestConnection_Click" CausesValidation="false" />
                </div>
            </div>
        </section>

        <!-- ================== BODY ================== -->
        <div class="wa-body">

            <!-- Notices -->
            <asp:Panel ID="pnlNotice" runat="server" CssClass="wa-notice" Visible="false" role="status">
                <i class="fas fa-info-circle"></i>
                <asp:Literal ID="litNotice" runat="server" Mode="Encode" />
            </asp:Panel>
            <asp:Panel ID="pnlDbError" runat="server" CssClass="wa-notice wa-notice-warning" Visible="false" role="alert">
                <i class="fas fa-plug"></i>
                <div><strong>تعذر الوصول إلى قاعدة البيانات</strong><p><asp:Literal ID="litDbError" runat="server" Mode="Encode" Text="تحقق من اتصال الخادم بقاعدة البيانات ثم أعد فتح الصفحة." /></p></div>
            </asp:Panel>
            <asp:Panel ID="pnlSchemaWarning" runat="server" CssClass="wa-notice wa-notice-warning" Visible="false" role="alert">
                <i class="fas fa-database"></i>
                <div><strong>يلزم تجهيز جداول الوحدة</strong><p>نفّذ ملفي التحديث <code>Database/Migrations/20261015_AddWhatsAppModule.sql</code> ثم <code>Database/Migrations/20261016_EnhanceWhatsAppModule.sql</code> على قاعدة البيانات، ثم أعد فتح الصفحة.</p></div>
            </asp:Panel>
            <asp:Panel ID="pnlSchedulerWarning" runat="server" CssClass="wa-notice wa-notice-warning" Visible="false" role="alert">
                <i class="fas fa-clock"></i>
                <div><strong>متابعة الإرسال المجدول</strong><p><asp:Literal ID="litSchedulerWarning" runat="server" Mode="Encode" /></p></div>
            </asp:Panel>

            <!-- Tabs -->
            <div class="wa-tabs-container">
                <nav class="wa-tabs" aria-label="أقسام واتساب">
                    <a class='<%= TabCss("compose") %>' href='<%= ViewUrl("compose") %>'><i class="fas fa-bullhorn"></i> إعلان ورسالة</a>
                    <asp:PlaceHolder ID="phManagerTab" runat="server" Visible="false">
                        <a class='<%= TabCss("manager") %>' href='<%= ViewUrl("manager") %>'><i class="fas fa-user-tie"></i> ملخص المدير</a>
                    </asp:PlaceHolder>
                    <a class='<%= TabCss("drafts") %>' href='<%= ViewUrl("drafts") %>'><i class="fas fa-file-alt"></i> المسودات</a>
                    <a class='<%= TabCss("outbox") %>' href='<%= ViewUrl("outbox") %>'><i class="fas fa-paper-plane"></i> الإرسال اليدوي</a>
                    <a class='<%= TabCss("log") %>' href='<%= ViewUrl("log") %>'><i class="fas fa-history"></i> السجل</a>
                    <a class='<%= TabCss("templates") %>' href='<%= ViewUrl("templates") %>'><i class="fas fa-copy"></i> القوالب</a>
                    <a class='<%= TabCss("consent") %>' href='<%= ViewUrl("consent") %>'><i class="fas fa-user-shield"></i> الموافقات</a>
                    <asp:PlaceHolder ID="phSettingsTab" runat="server" Visible="false">
                        <a class='<%= TabCss("settings") %>' href='<%= ViewUrl("settings") %>'><i class="fas fa-sliders-h"></i> الإعدادات</a>
                    </asp:PlaceHolder>
                </nav>
            </div>

            <!-- ================== PANEL: COMPOSE ================== -->
            <asp:Panel ID="pnlCompose" runat="server" CssClass="wa-card">
                <div class="wa-card-header">
                    <div class="wa-card-header-icon"><i class="fab fa-whatsapp"></i></div>
                    <div class="wa-card-header-text">
                        <h2>إنشاء حملة جديدة</h2>
                        <p>أرسل رسالة مخصصة لأولياء الأمور بناءً على الفصل أو الطالب.</p>
                    </div>
                </div>
                <div class="wa-card-body">
                    
                    <!-- 1. Target Audience -->
                    <div class="wa-form-grid">
                        <div class="students-field">
                            <asp:Label ID="lblAudienceType" runat="server" AssociatedControlID="ddlAudienceType" Text="نوع الإرسال" Font-Size="13px" Font-Bold="true"  />
                            <asp:DropDownList ID="ddlAudienceType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAudienceType_SelectedIndexChanged">
                                <asp:ListItem Value="Class" Text="لكل أولياء الفصل" />
                                <asp:ListItem Value="Individual" Text="لولي أمر طالب واحد" />
                                <asp:ListItem Value="School" Text="لجميع أولياء المدرسة" />
                            </asp:DropDownList>
                        </div>
                        
                        <div class="students-field">
                            <asp:Label ID="lblComposeGrade" runat="server" Text="الفصل"  Font-Size="13px" Font-Bold="true" />
                            <asp:DropDownList ID="ddlComposeGrade" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlComposeGrade_SelectedIndexChanged" />
                        </div>
                        
                        <div class="students-field">
                            <asp:Label ID="lblComposeDivision" runat="server" Text="القسم" Font-Size="13px" Font-Bold="true"/>
                            <asp:DropDownList ID="ddlComposeDivision" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlComposeDivision_SelectedIndexChanged" />
                        </div>
                        
                        <div class="students-field">
                            <asp:Label ID="lblClass" runat="server" AssociatedControlID="ddlClass" Text="الشعبة"  Font-Size="13px" Font-Bold="true"/>
                            <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlClass_SelectedIndexChanged" />
                        </div>
                        
                        <div class="students-field">
                            <asp:Label ID="lblKind" runat="server" AssociatedControlID="ddlMessageKind" Text="تصنيف الرسالة"  Font-Size="13px" Font-Bold="true"/>
                            <asp:DropDownList ID="ddlMessageKind" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlMessageKind_SelectedIndexChanged">
                                <asp:ListItem Value="Announcement" Text="إعلان عام" />
                                <asp:ListItem Value="Attendance" Text="الحضور والغياب" />
                                <asp:ListItem Value="Academic" Text="أكاديمي ودرجات" />
                                <asp:ListItem Value="Financial" Text="مالي وأقساط" />
                                <asp:ListItem Value="Administrative" Text="إداري" />
                                <asp:ListItem Value="Free" Text="رسالة حرة" />
                            </asp:DropDownList>
                        </div>
                        
                        <div class="students-field">
                            <asp:Label ID="lblTemplate" runat="server" AssociatedControlID="ddlTemplate" Text="قالب جاهز (اختياري)"  Font-Size="13px" Font-Bold="true"/>
                            <asp:DropDownList ID="ddlTemplate" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplate_SelectedIndexChanged" />
                        </div>
                    </div>

                    <!-- Recipient Summary -->
                    <div class="wa-info-box" style="background:var(--wa-primary-light); border-color:#bbf7d0; color:var(--wa-primary-dark);">
                        <i class="fas fa-users" style="float:right; margin-left:10px; font-size:18px;"></i>
                        <div>
                            <asp:Literal ID="litRecipientSummary" runat="server" Text="اختر الفصل أو الشعبة لعرض عدد أولياء الأمور المستعدين للإرسال." />
                            <br /><span style="font-size:11px; opacity:0.8;">ملاحظة: لا تُرسل الرسائل إلا لأولياء الأمور الذين لديهم موافقة فعّالة مسجلة.</span>
                        </div>
                    </div>

                    <!-- Individual Guardian Selection -->
                    <asp:Panel ID="pnlIndividualRecipient" runat="server" CssClass="students-field" Visible="false" style="margin-bottom: 24px;">
                        <asp:Label ID="lblStudent" runat="server" AssociatedControlID="ddlStudent" Text="اختيار الطالب وولي الأمر" />
                        <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged" />
                        <small class="wa-help">يظهر اسم الطالب أولاً ثم اسم ولي الأمر. تُرسل الرسالة إلى الرقم المسجل لولي الأمر المحدد.</small>
                        <div id="waSelectedGuardian" class="wa-selected-guardian" role="status" aria-live="polite" style="display:none;">
                            <span class="wa-selected-guardian-label">المستلم المحدد:</span>
                            <strong id="waSelectedGuardianName"></strong>
                        </div>
                    </asp:Panel>

                    <!-- 2. Message Content -->
                    <div class="wa-field wa-field-wide" style="margin-bottom:24px;">
                        <asp:Label ID="lblMessage" runat="server" AssociatedControlID="txtMessage" Text="نص الرسالة" />
                        <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" MaxLength="4000" placeholder="اكتب نص الرسالة هنا..." />
                        <small class="wa-help">المتغيرات المتاحة: {guardianName}, {studentName}, {class}, {date}, {balance}, {attendanceSummary}, {latestGrade}, {dailyLessons}. تُضاف بداية الرسالة تلقائياً باسم المدرسة.</small>
                    </div>

                    <!-- Options -->
                    <div class="wa-options-grid">
                        <asp:PlaceHolder ID="pnlFinancialOption" runat="server">
                            <label class="wa-option">
                                <asp:CheckBox ID="chkIncludeBalance" runat="server" />
                                <span><strong>تضمين الوضع المالي</strong><small>تعبئة المتغير {balance}</small></span>
                            </label>
                        </asp:PlaceHolder>
                        <label class="wa-option">
                            <asp:CheckBox ID="chkIncludeAttendance" runat="server" />
                            <span><strong>ملخص الحضور</strong><small>تعبئة المتغير {attendanceSummary}</small></span>
                        </label>
                        <label class="wa-option">
                            <asp:CheckBox ID="chkIncludeGrades" runat="server" />
                            <span><strong>آخر الدرجات</strong><small>تعبئة المتغير {latestGrade}</small></span>
                        </label>
                        <label class="wa-option">
                            <asp:CheckBox ID="chkAppendDailyLessons" runat="server" AutoPostBack="true" OnCheckedChanged="chkAppendDailyLessons_CheckedChanged" />
                            <span><strong>ملخص دروس اليوم</strong><small>إضافة جدول دروس الفصل تلقائياً</small></span>
                        </label>
                        <label class="wa-option">
                            <asp:CheckBox ID="chkIncludeAnnouncement" runat="server" AutoPostBack="true" OnCheckedChanged="chkIncludeAnnouncement_CheckedChanged" />
                            <span><strong>إضافة إعلان مخصص</strong><small>فتح حقل لكتابة إعلان عطلة أو غيره</small></span>
                        </label>
                    </div>

                    <asp:Panel ID="pnlDailyLessons" runat="server" CssClass="wa-info-box" Visible="false" style="background:#eff6ff; border-color:#bfdbfe;">
                        <strong style="color:#1d4ed8;"><i class="fas fa-calendar-day"></i> دروس اليوم:</strong>
                        <asp:Literal ID="litDailyLessons" runat="server" Mode="Encode" />
                    </asp:Panel>

                    <asp:Panel ID="pnlAnnouncement" runat="server" CssClass="wa-field wa-field-wide" Visible="false" style="margin-bottom:24px;">
                        <asp:Label ID="lblAnnouncement" runat="server" AssociatedControlID="txtAnnouncement" Text="نص الإعلان الإضافي" />
                        <asp:TextBox ID="txtAnnouncement" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="1500" placeholder="اكتب الإعلان هنا..." />
                        <small class="wa-help">سيتم إضافة هذا النص إلى نهاية الرسالة الأساسية.</small>
                    </asp:Panel>

                    <!-- 3. Action Bar -->
                    <div class="wa-action-bar">
                        <div class="wa-field">
                            <asp:Label ID="lblSchedule" runat="server" AssociatedControlID="txtScheduledAt" Text="موعد الإرسال (اختياري)" />
                            <asp:TextBox ID="txtScheduledAt" runat="server" CssClass="form-control" TextMode="DateTimeLocal" step="60" />
                        </div>
                        <div style="display:flex; gap:10px; flex-wrap:wrap;">
                            <asp:Button ID="btnSendNow" runat="server" Text="إرسال الآن" CssClass="wa-btn wa-btn-primary" OnClick="btnSendNow_Click" />
                            <asp:Button ID="btnSchedule" runat="server" Text="جدولة الإرسال" CssClass="wa-btn wa-btn-outline" OnClick="btnSchedule_Click" />
                            <asp:Button ID="btnSaveDraft" runat="server" Text="حفظ كمسودة" CssClass="wa-btn wa-btn-light" OnClick="btnSaveDraft_Click" CausesValidation="false" />
                            <asp:Button ID="btnResetCompose" runat="server" Text="تفريغ" CssClass="wa-btn wa-btn-text" OnClick="btnResetCompose_Click" CausesValidation="false" />
                        </div>
                    </div>

                </div>
            </asp:Panel>

            <!-- ================== PANEL: MANAGER ================== -->
            <asp:Panel ID="pnlManager" runat="server" CssClass="wa-card" Visible="false">
                <div class="wa-card-header">
                    <div class="wa-card-header-icon" style="background:#f3e8ff; color:var(--wa-secondary);"><i class="fas fa-user-tie"></i></div>
                    <div class="wa-card-header-text">
                        <h2>ملخص اليوم للمدير</h2>
                        <p>تقرير مجمّع يُرسل إلى رقم المدير المعتمد.</p>
                    </div>
                </div>
                <div class="wa-card-body">
                    
                    <div class="wa-info-box warning">
                        <strong><i class="fas fa-circle-info"></i> آلية التصفية</strong>
                        اختر <strong>المستوى الدراسي</strong> ثم الصف، وبعده القسم (علمي أو أدبي) والشعبة (أ، ب).
                    </div>

                    <div class="wa-form-grid">
                        <div class="wa-field">
                            <asp:Label ID="lblManagerStage" runat="server" AssociatedControlID="ddlManagerStage" Text="المستوى الدراسي" />
                            <asp:DropDownList ID="ddlManagerStage" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlManagerStage_SelectedIndexChanged" />
                        </div>
                        <div class="wa-field">
                            <asp:Label ID="lblManagerGrade" runat="server" AssociatedControlID="ddlManagerGrade" Text="الصف" />
                            <asp:DropDownList ID="ddlManagerGrade" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlManagerGrade_SelectedIndexChanged" />
                        </div>
                        <div class="wa-field">
                            <asp:Label ID="lblManagerDivision" runat="server" AssociatedControlID="ddlManagerDivision" Text="القسم" />
                            <asp:DropDownList ID="ddlManagerDivision" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlManagerDivision_SelectedIndexChanged" />
                        </div>
                        <div class="wa-field">
                            <asp:Label ID="lblManagerClass" runat="server" AssociatedControlID="ddlManagerClass" Text="الشعبة" />
                            <asp:DropDownList ID="ddlManagerClass" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlManagerClass_SelectedIndexChanged" />
                        </div>
                        <div class="wa-field">
                            <asp:Label ID="lblManagerPhoneView" runat="server" Text="رقم المدير المعتمد" />
                            <div class="form-control" style="background:#f1f5f9; display:flex; align-items:center; gap:8px; cursor:default; border-color:transparent;">
                                <i class="fas fa-phone" style="color:var(--wa-secondary);"></i>
                                <asp:Literal ID="litManagerPhone" runat="server" Mode="Encode" />
                            </div>
                            <small class="wa-help">يُعدَّل من تبويب «الإعدادات».</small>
                        </div>
                    </div>

                    <div class="wa-options-grid" style="margin-top:10px;">
                        <label class="wa-option">
                            <asp:CheckBox ID="chkManagerIncludeDaily" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="chkManagerIncludeDaily_CheckedChanged" />
                            <span><strong>تضمين ملخص اليوم</strong><small>الطلاب، الحضور، والغياب.</small></span>
                        </label>
                        <label class="wa-option">
                            <asp:CheckBox ID="chkManagerIncludeFinancial" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="chkManagerIncludeFinancial_CheckedChanged" />
                            <span><strong>تضمين التقرير المالي للطلاب</strong><small>المستحقات، المسدد، والمتأخرات.</small></span>
                        </label>
                        <label class="wa-option">
                            <asp:CheckBox ID="chkManagerIncludePayroll" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="chkManagerIncludePayroll_CheckedChanged" />
                            <span><strong>تضمين التقرير المالي للمعلمين</strong><small>الرواتب، الحصص، التسويات، والخصومات.</small></span>
                        </label>
                    </div>

                    <asp:Panel ID="pnlManagerPreview" runat="server" CssClass="wa-info-box" Visible="false" style="background:#f8fafc; border-color:#e2e8f0;">
                        <strong style="color:var(--wa-secondary);"><i class="fas fa-eye"></i> معاينة الملخص:</strong>
                        <div style="white-space:pre-line; font-size:13px; margin-top:8px;"><asp:Literal ID="litManagerPreview" runat="server" Mode="Encode" /></div>
                    </asp:Panel>

                    <label class="wa-option" style="margin-top:10px; background:#fffbeb; border-color:#fde68a;">
                        <asp:CheckBox ID="chkManagerConfirmed" runat="server" />
                        <span><strong style="color:#b45309;">أؤكد أن الرقم يخص المدير وقد وافق على استلام الملخص</strong></span>
                    </label>

                    <div class="wa-action-bar" style="margin-top:20px;">
                        <div></div>
                        <div style="display:flex; gap:10px;">
                            <asp:Button ID="btnPreviewManagerSummary" runat="server" Text="تحديث المعاينة" CssClass="wa-btn wa-btn-light" OnClick="btnPreviewManagerSummary_Click" CausesValidation="false" />
                            <asp:Button ID="btnSendManagerSummary" runat="server" Text="إرسال الملخص للمدير" CssClass="wa-btn wa-btn-primary" OnClick="btnSendManagerSummary_Click" />
                        </div>
                    </div>

                </div>
            </asp:Panel>

            <!-- ================== PANEL: DRAFTS ================== -->
            <asp:Panel ID="pnlDrafts" runat="server" CssClass="wa-card" Visible="false">
                <div class="wa-card-header">
                    <div class="wa-card-header-icon"><i class="fas fa-file-alt"></i></div>
                    <div class="wa-card-header-text">
                        <h2>المسودات والرسائل المجدولة</h2>
                        <p>مراجعة وتعديل الرسائل التي لم تُرسل بعد.</p>
                    </div>
                </div>
                <div class="wa-card-body">
                    <div class="wa-table-wrapper">
                        <asp:GridView ID="gvDrafts" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد مسودات أو رسائل مجدولة." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvDrafts_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvDrafts_RowCommand" OnRowDataBound="Grid_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="Title" HeaderText="الحملة" />
                                <asp:BoundField DataField="ClassName" HeaderText="الفصل" />
                                <asp:BoundField DataField="CreatedAt" HeaderText="تاريخ الإنشاء" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                <asp:BoundField DataField="ScheduledAt" HeaderText="موعد الإرسال" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                <asp:BoundField DataField="RecipientCount" HeaderText="الرسائل" />
                                <asp:BoundField DataField="Status" HeaderText="الحالة" />
                                <asp:TemplateField HeaderText="إجراء">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn" CommandName="open" CommandArgument='<%# Eval("Id") %>' Visible='<%# Convert.ToString(Eval("Status")) == "مسودة" %>' CausesValidation="false"><i class="fas fa-edit"></i> فتح</asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn danger" CommandName="cancelitem" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return waConfirm('هل تريد إلغاء هذه الحملة ورسائلها؟');"><i class="fas fa-trash"></i> إلغاء</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>

            <!-- ================== PANEL: OUTBOX ================== -->
            <asp:Panel ID="pnlOutbox" runat="server" CssClass="wa-card" Visible="false">
                <div class="wa-card-header">
                    <div class="wa-card-header-icon"><i class="fas fa-paper-plane"></i></div>
                    <div class="wa-card-header-text">
                        <h2>صندوق الإرسال اليدوي</h2>
                        <p>الرسائل الجاهزة للإرسال عبر تطبيق واتساب.</p>
                    </div>
                    <div class="wa-toolbar" style="margin-right:auto;">
                        <asp:TextBox ID="txtOutboxSearch" runat="server" CssClass="form-control" placeholder="بحث..." AutoPostBack="true" OnTextChanged="OutboxSearch_TextChanged" />
                        <asp:LinkButton ID="btnClearOutboxSearch" runat="server" CssClass="wa-row-btn" OnClick="ClearOutboxSearch_Click" CausesValidation="false"><i class="fas fa-times"></i></asp:LinkButton>
                    </div>
                </div>
                <div class="wa-card-body">
                    <div class="wa-info-box">
                        <strong><i class="fas fa-info-circle" style="color:var(--wa-primary);"></i> خطوات الإرسال:</strong>
                        <ol style="margin:0; padding-right:20px;">
                            <li>اضغط «فتح في واتساب» بجانب الرسالة.</li>
                            <li>اضغط «إرسال» داخل تطبيق واتساب.</li>
                            <li>عُد هنا واضغط «تسجيل كمُرسل» لتحديث السجل.</li>
                        </ol>
                        <div style="margin-top:10px; font-weight:700; color:var(--wa-primary-dark);"><asp:Literal ID="litOutboxHint" runat="server" Mode="Encode" /></div>
                    </div>
                    
                    <asp:HiddenField ID="hfOpenedOutboxIds" runat="server" />
                    
                    <div class="wa-table-wrapper">
                        <asp:GridView ID="gvOutbox" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد رسائل بانتظار الإرسال." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvOutbox_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvOutbox_RowCommand" OnRowDataBound="Grid_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="RecipientName" HeaderText="المستلم" />
                                <asp:BoundField DataField="PhoneNumber" HeaderText="رقم واتساب" />
                                <asp:BoundField DataField="MessagePreview" HeaderText="نص الرسالة" />
                                <asp:BoundField DataField="ScheduledAt" HeaderText="الموعد" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                <asp:TemplateField HeaderText="إجراء">
                                    <ItemTemplate>
                                        <a class="wa-row-btn send" href='<%# Eval("WhatsAppLink") %>' target="_blank" rel="noopener" onclick="waRecordOutboxOpen(<%# Eval("Id") %>);"><i class="fab fa-whatsapp"></i> فتح</a>
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn" CommandName="sent" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return waConfirm('هل أرسلت الرسالة فعلاً؟');"><i class="fas fa-check"></i> تسجيل</asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn danger" CommandName="cancelitem" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return waConfirm('إلغاء الرسالة؟');"><i class="fas fa-times"></i></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>

            <!-- ================== PANEL: LOG ================== -->
            <asp:Panel ID="pnlLog" runat="server" CssClass="wa-card" Visible="false">
                <div class="wa-card-header">
                    <div class="wa-card-header-icon"><i class="fas fa-history"></i></div>
                    <div class="wa-card-header-text">
                        <h2>سجل الرسائل</h2>
                        <p>متابعة حالة الإرسال والتسليم.</p>
                    </div>
                    <div class="wa-toolbar" style="margin-right:auto;">
                        <asp:TextBox ID="txtLogSearch" runat="server" CssClass="form-control" placeholder="بحث..." AutoPostBack="true" OnTextChanged="LogSearch_TextChanged" />
                        <asp:LinkButton ID="btnClearLogSearch" runat="server" CssClass="wa-row-btn" OnClick="ClearLogSearch_Click" CausesValidation="false"><i class="fas fa-times"></i></asp:LinkButton>
                        <asp:DropDownList ID="ddlLogDirection" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlLogDirection_SelectedIndexChanged">
                            <asp:ListItem Value="" Text="الكل" />
                            <asp:ListItem Value="Outbound" Text="صادر" />
                            <asp:ListItem Value="Inbound" Text="وارد" />
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlLogStatus" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlLogStatus_SelectedIndexChanged">
                            <asp:ListItem Value="" Text="كل الحالات" />
                            <asp:ListItem Value="Sent" Text="قُبلت" />
                            <asp:ListItem Value="Delivered" Text="تم التسليم" />
                            <asp:ListItem Value="Read" Text="مقروءة" />
                            <asp:ListItem Value="Failed" Text="فشلت" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="wa-card-body">
                    <div style="margin-bottom:16px; font-size:13px; font-weight:700; color:var(--wa-text-muted);">
                        <asp:Literal ID="litLogStats" runat="server" Mode="Encode" />
                    </div>
                    <div class="wa-table-wrapper">
                        <asp:GridView ID="gvLog" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد سجلات." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvLog_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvLog_RowCommand" OnRowDataBound="Grid_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="CreatedAtLocal" HeaderText="الوقت" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                <asp:BoundField DataField="RecipientName" HeaderText="المستلم" />
                                <asp:BoundField DataField="PhoneNumber" HeaderText="الرقم" />
                                <asp:BoundField DataField="MessageBody" HeaderText="النص" />
                                <asp:TemplateField HeaderText="الحالة">
                                    <ItemTemplate>
                                        <span class='wa-badge <%# StatusCss(Convert.ToString(Eval("Status"))) %>'><%# StatusText(Convert.ToString(Eval("Status"))) %></span>
                                        <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("ErrorDetails"))) %>'>
                                            <span style="display:block; margin-top:4px; font-size:10px; color:var(--wa-danger);"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("ErrorDetails") %>' /></span>
                                        </asp:PlaceHolder>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="CreatedBy" HeaderText="بواسطة" />
                                <asp:TemplateField HeaderText="إجراء">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn" CommandName="retry" CommandArgument='<%# Eval("Id") %>' Visible='<%# (bool)Eval("CanRetry") %>' CausesValidation="false">إعادة</asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn danger" CommandName="deleteitem" CommandArgument='<%# Eval("Id") %>' Visible='<%# (bool)Eval("CanDelete") %>' CausesValidation="false" OnClientClick="return waConfirm('حذف السجل نهائياً؟');">حذف</asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn danger" CommandName="cancelitem" CommandArgument='<%# Eval("Id") %>' Visible='<%# (bool)Eval("CanCancel") %>' CausesValidation="false" OnClientClick="return waConfirm('إلغاء الرسالة؟');">إلغاء</asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn" CommandName="resend" CommandArgument='<%# Eval("Id") %>' Visible='<%# (bool)Eval("CanResend") %>' CausesValidation="false" OnClientClick="return waConfirm('إعادة إرسال نسخة جديدة؟');">إعادة إرسال</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>

            <!-- ================== PANEL: TEMPLATES ================== -->
            <asp:Panel ID="pnlTemplates" runat="server" CssClass="wa-card" Visible="false">
                <div class="wa-card-header">
                    <div class="wa-card-header-icon"><i class="fas fa-copy"></i></div>
                    <div class="wa-card-header-text">
                        <h2>قوالب الرسائل</h2>
                        <p>القوالب المحلية تساعد على الصياغة السريعة.</p>
                    </div>
                </div>
                <div class="wa-card-body">
                    <asp:HiddenField ID="hdnTemplateId" runat="server" />
                    <div class="wa-inline-group" style="display:grid; grid-template-columns:1fr 1fr; gap:16px;">
                        <div class="wa-field"><asp:Label ID="lblTemplateName" runat="server" AssociatedControlID="txtTemplateName" Text="اسم القالب" /><asp:TextBox ID="txtTemplateName" runat="server" CssClass="form-control" MaxLength="150" /></div>
                        <div class="wa-field"><asp:Label ID="lblTemplateCategory" runat="server" AssociatedControlID="ddlTemplateCategory" Text="التصنيف" /><asp:DropDownList ID="ddlTemplateCategory" runat="server" CssClass="form-select"><asp:ListItem Text="إعلان" Value="إعلان" /><asp:ListItem Text="مالي" Value="مالي" /><asp:ListItem Text="حضور" Value="حضور" /><asp:ListItem Text="إداري" Value="إداري" /><asp:ListItem Text="أكاديمي" Value="أكاديمي" /></asp:DropDownList></div>
                        <div class="wa-field"><asp:Label ID="lblProviderTemplateName" runat="server" AssociatedControlID="txtProviderTemplateName" Text="اسم Meta (إن وجد)" /><asp:TextBox ID="txtProviderTemplateName" runat="server" CssClass="form-control" MaxLength="100" /></div>
                        <div class="wa-field"><asp:Label ID="lblTemplateLanguage" runat="server" AssociatedControlID="txtTemplateLanguage" Text="اللغة" /><asp:TextBox ID="txtTemplateLanguage" runat="server" CssClass="form-control" MaxLength="20" Text="ar" /></div>
                        <div class="wa-field" style="grid-column: 1 / -1;"><asp:Label ID="lblTemplateBody" runat="server" AssociatedControlID="txtTemplateBody" Text="النص والمتغيرات" /><asp:TextBox ID="txtTemplateBody" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="4000" /></div>
                        
                        <label class="wa-option" style="grid-column: 1 / -1;">
                            <asp:CheckBox ID="chkTemplateApproved" runat="server" />
                            <span><strong>اعتماد Meta مؤكد</strong><small>فعّلها فقط بعد الاعتماد الرسمي.</small></span>
                        </label>
                        <div style="grid-column: 1 / -1; display:flex; gap:10px;">
                            <asp:Button ID="btnSaveTemplate" runat="server" Text="حفظ القالب" CssClass="wa-btn wa-btn-primary" OnClick="btnSaveTemplate_Click" />
                            <asp:Button ID="btnClearTemplate" runat="server" Text="قالب جديد" CssClass="wa-btn wa-btn-light" OnClick="btnClearTemplate_Click" CausesValidation="false" />
                        </div>
                    </div>

                    <div class="wa-table-wrapper" style="margin-top:24px;">
                        <asp:GridView ID="gvTemplates" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا توجد قوالب." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvTemplates_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvTemplates_RowCommand" OnRowDataBound="Grid_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="القالب" />
                                <asp:BoundField DataField="Category" HeaderText="التصنيف" />
                                <asp:BoundField DataField="ProviderTemplateName" HeaderText="اسم Meta" />
                                <asp:TemplateField HeaderText="الاعتماد"><ItemTemplate><span class='wa-badge <%# (bool)Eval("IsMetaApproved") ? "is-ok" : "is-muted" %>'><%# (bool)Eval("IsMetaApproved") ? "معتمد" : "محلي" %></span></ItemTemplate></asp:TemplateField>
                                <asp:TemplateField HeaderText="إجراء"><ItemTemplate><asp:LinkButton runat="server" CssClass="wa-row-btn" CommandName="edititem" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">تعديل</asp:LinkButton><asp:LinkButton runat="server" CssClass="wa-row-btn danger" CommandName="toggle" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"><%# (bool)Eval("IsActive") ? "إيقاف" : "تفعيل" %></asp:LinkButton></ItemTemplate></asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>

            <!-- ================== PANEL: CONSENT ================== -->
            <asp:Panel ID="pnlConsent" runat="server" CssClass="wa-card" Visible="false">
                <div class="wa-card-header">
                    <div class="wa-card-header-icon"><i class="fas fa-user-shield"></i></div>
                    <div class="wa-card-header-text">
                        <h2>موافقات أولياء الأمور</h2>
                        <p>إدارة ومراجعة الموافقات قبل الإرسال.</p>
                    </div>
                    <div class="wa-toolbar" style="margin-right:auto;">
                        <asp:TextBox ID="txtConsentSearch" runat="server" CssClass="form-control" placeholder="بحث..." AutoPostBack="true" OnTextChanged="ConsentSearch_TextChanged" />
                        <asp:LinkButton ID="btnClearConsentSearch" runat="server" CssClass="wa-row-btn" OnClick="ClearConsentSearch_Click" CausesValidation="false"><i class="fas fa-times"></i></asp:LinkButton>
                    </div>
                </div>
                <div class="wa-card-body">
                    <div class="wa-inline-group" style="align-items:center;">
                        <div class="wa-field" style="flex:1;">
                            <asp:Label ID="lblConsentClass" runat="server" AssociatedControlID="ddlConsentClass" Text="الفصل" />
                            <asp:DropDownList ID="ddlConsentClass" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlConsentClass_SelectedIndexChanged" />
                        </div>
                       <div class="wa-field" style="flex:1;">
                          <strong style="font-size:12px">تاكيد التسجيل</strong>
                        <label class="wa-option" style="flex:1; margin:0; height:44px; align-items:center;">
                            <asp:CheckBox ID="chkConsentVerified" runat="server" />
                            <span><strong style="font-weight: 500;">أؤكد توثيق الموافقة (مطلوب للتسجيل)</strong></span>
                        </label>
                           </div>
                    </div>

                    <div class="wa-table-wrapper">
                        <asp:GridView ID="gvContacts" runat="server" AutoGenerateColumns="false" CssClass="wa-grid" GridLines="None" EmptyDataText="لا يوجد أولياء أمور بأرقام في هذا الفصل." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvContacts_PageIndexChanging" PagerStyle-CssClass="wa-pager" OnRowCommand="gvContacts_RowCommand" OnRowDataBound="Grid_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="ParentName" HeaderText="ولي الأمر" />
                                <asp:BoundField DataField="StudentNames" HeaderText="الطلاب" />
                                <asp:BoundField DataField="Phone" HeaderText="الهاتف" />
                                <asp:TemplateField HeaderText="الحالة"><ItemTemplate><span class='wa-badge <%# (bool)Eval("IsOptedIn") ? "is-ok" : Convert.ToString(Eval("ConsentState")) == "ألغى الموافقة" ? "is-danger" : "is-muted" %>'><%# Eval("ConsentState") %></span></ItemTemplate></asp:TemplateField>
                                <asp:BoundField DataField="ConsentSource" HeaderText="المصدر" />
                                <asp:BoundField DataField="ConsentDate" HeaderText="التاريخ" />
                                <asp:TemplateField HeaderText="إجراء">
                                    <ItemTemplate>
                                        <!-- زر التسجيل يظهر فقط إذا لم يكن مسجلاً -->
                                        <asp:LinkButton runat="server" CssClass="wa-row-btn" CommandName="optin" CommandArgument='<%# Eval("ParentId") %>' CausesValidation="false" Visible='<%# !(bool)Eval("IsOptedIn") %>'>تسجيل</asp:LinkButton>
                                        
                                        <!-- كلمة "مسجل" تظهر فقط إذا كان مسجلاً (غير قابلة للضغط) -->
                                        <span runat="server" Visible='<%# (bool)Eval("IsOptedIn") %>' class="wa-row-btn" style="color:var(--wa-primary); background:var(--wa-primary-light); border-color:#bbf7d0; cursor:default; padding:6px 12px;">
                                            <i class="fas fa-check"></i> مسجل
                                        </span>

                                        <asp:LinkButton runat="server" CssClass="wa-row-btn danger" CommandName="optout" CommandArgument='<%# Eval("ParentId") %>' CausesValidation="false" Visible='<%# (bool)Eval("IsOptedIn") %>' OnClientClick="return waConfirm('هل تريد إلغاء الموافقة لهذا الرقم؟');">إلغاء</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>

            <!-- ================== PANEL: SETTINGS ================== -->
            <asp:Panel ID="pnlSettings" runat="server" CssClass="wa-card" Visible="false">
                <div class="wa-card-header">
                    <div class="wa-card-header-icon"><i class="fas fa-sliders-h"></i></div>
                    <div class="wa-card-header-text">
                        <h2>إعدادات النظام</h2>
                        <p>التحكم في رقم المدير وسياسات الإرسال.</p>
                    </div>
                </div>
                <div class="wa-card-body">
                    <div class="wa-form-grid">
                        <div class="wa-field">
                            <asp:Label ID="lblSettingsManagerPhone" runat="server" AssociatedControlID="txtSettingsManagerPhone" Text="رقم المدير" />
                            <asp:TextBox ID="txtSettingsManagerPhone" runat="server" CssClass="form-control" MaxLength="30" placeholder="+218..." />
                        </div>
                        <div class="wa-field">
                            <asp:Label ID="lblSettingsCountryCode" runat="server" AssociatedControlID="txtSettingsCountryCode" Text="رمز الدولة" />
                            <asp:TextBox ID="txtSettingsCountryCode" runat="server" CssClass="form-control" MaxLength="4" placeholder="218" />
                        </div>
                    </div>
                    <div class="wa-options-grid" style="margin-top:10px;">
                        <label class="wa-option">
                            <asp:CheckBox ID="chkSettingsReplyConsent" runat="server" />
                            <span><strong>الرد يتطلب موافقة</strong><small>منع الردود على أرقام بلا موافقة.</small></span>
                        </label>
                        <label class="wa-option">
                            <asp:CheckBox ID="chkSettingsManualLinks" runat="server" />
                            <span><strong>وضع الإرسال اليدوي</strong><small>تجهيز روابط بدل الإرسال الآلي المدفوع.</small></span>
                        </label>
                    </div>
                    
                    <div class="wa-info-box" style="margin-top:20px; font-size:12px;">
                        <asp:Literal ID="litReplyPolicy" runat="server" Mode="Encode" />
                    </div>

                    <div class="wa-action-bar" style="margin-top:20px;">
                        <div></div>
                        <div style="display:flex; gap:10px;">
                            <asp:Button ID="btnSaveSettings" runat="server" Text="حفظ الإعدادات" CssClass="wa-btn wa-btn-primary" OnClick="btnSaveSettings_Click" />
                            <asp:Button ID="btnResetSettings" runat="server" Text="استعادة الافتراضي" CssClass="wa-btn wa-btn-light" OnClick="btnResetSettings_Click" CausesValidation="false" OnClientClick="return waConfirm('استعادة القيم الافتراضية؟');" />
                        </div>
                    </div>

                    <div style="margin-top:30px; padding-top:20px; border-top:1px solid var(--wa-border);">
                        <strong style="display:block; margin-bottom:10px; font-size:14px; color:var(--wa-text-main);">حالة إعدادات الخادم (الأسرار)</strong>
                        <div style="font-size:13px; line-height:2;"><asp:Literal ID="litSettingsSecretStatus" runat="server" Mode="PassThrough" /></div>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </main>
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        (function () {
            var ddl = document.getElementById('<%= ddlStudent.ClientID %>');
            var panel = document.getElementById('waSelectedGuardian');
            var name = document.getElementById('waSelectedGuardianName');
            if (!ddl || !panel || !name) return;
            
            function showSelectedGuardian() {
                var option = ddl.options[ddl.selectedIndex];
                if (!option || !option.value) {
                    panel.style.display = 'none';
                    name.textContent = '';
                    return;
                }
                name.textContent = option.text;
                panel.style.display = 'block';
            }
            ddl.addEventListener('change', showSelectedGuardian);
            showSelectedGuardian();
        })();
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