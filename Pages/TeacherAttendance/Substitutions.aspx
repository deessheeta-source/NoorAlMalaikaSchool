<%@ Page Title="سجل التعويضيات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Substitutions.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.Substitutions" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        .ta-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .ta-hero-section {
            position: relative;
            background: linear-gradient(135deg, #ef6fa7 0%, #dd5999 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .ta-hero-section::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .ta-hero-section::after {
            content: '';
            position: absolute;
            top: -40%;
            left: -10%;
            width: 340px;
            height: 340px;
            border-radius: 50%;
            background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%);
            pointer-events: none;
        }

        .ta-hero-content {
            position: relative;
            z-index: 2;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 24px;
            flex-wrap: wrap;
            max-width: 1280px;
            margin: 0 auto;
        }

        .ta-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .ta-hero-icon {
            width: 68px;
            height: 68px;
            border-radius: 20px;
            background: rgba(255,255,255,0.18);
            backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,0.3);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 30px;
            flex-shrink: 0;
            box-shadow: 0 8px 24px rgba(0,0,0,0.12);
        }
        .ta-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .ta-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
            max-width: 620px;
        }
        .ta-hero-titles p strong {
            color: #fff;
            font-weight: 800;
        }

        .ta-btn-glass {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 10px 20px;
            border-radius: 12px;
            background: rgba(255,255,255,0.15);
            backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,0.3);
            color: #fff;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.2s ease;
            cursor: pointer;
            flex-shrink: 0;
        }
        .ta-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .ta-body {
            max-width: 1280px;
            margin: -56px auto 0;
            padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- ALERT ---------- */
        .ta-alert {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 14px 20px;
            border-radius: 14px;
            margin-bottom: 20px;
            font-size: 13.5px;
            font-weight: 700;
            box-shadow: 0 8px 24px rgba(0,0,0,0.06);
            animation: slideDown 0.4s ease;
        }
        .ta-alert i {
            width: 36px;
            height: 36px;
            border-radius: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
            color: #fff;
        }
        .ta-alert.success {
            background: linear-gradient(180deg, #ffffff 0%, #f8fdfa 100%);
            border: 1.5px solid rgba(0,177,79,0.35);
            color: #1f3d33;
        }
        .ta-alert.success i { background: linear-gradient(135deg, #00b14f, #4ed17e); }

        .ta-alert.danger {
            background: linear-gradient(180deg, #ffffff 0%, #fef8f8 100%);
            border: 1.5px solid rgba(220,53,69,0.35);
            color: #7a1a26;
        }
        .ta-alert.danger i { background: linear-gradient(135deg, #dc3545, #ff6b7a); }

        @keyframes slideDown {
            from { opacity: 0; transform: translateY(-10px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        /* ---------- CARD ---------- */
        .ta-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .ta-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .ta-card-header-icon {
            width: 44px;
            height: 44px;
            border-radius: 13px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            flex-shrink: 0;
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
            position: relative;
        }
        .ta-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .ta-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .ta-card-header-icon.purple {
            background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%);
            box-shadow: 0 6px 16px rgba(111,66,193,0.3);
        }
        .ta-card-header-icon.orange {
            background: linear-gradient(135deg, #fd7e14 0%, #ffb066 100%);
            box-shadow: 0 6px 16px rgba(253,126,20,0.3);
        }
        .ta-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 200px;
        }
        .ta-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .ta-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }

        .ta-card-body {
            padding: 20px 24px;
        }

        /* ---------- TOOLBAR ---------- */
        .ta-toolbar {
            display: flex;
            align-items: flex-end;
            gap: 12px;
            flex-wrap: wrap;
        }
        .ta-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 160px;
            flex: 1 1 160px;
        }
        .ta-field label {
            font-size: 11.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
        }
        .ta-field label i { color: #00b14f; }
        .ta-field .form-control,
        .ta-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
        }
        .ta-field .form-control:focus,
        .ta-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
        }

        .ta-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 22px;
            border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            color: #fff;
            font-size: 13px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
            height: 42px;
            text-decoration: none;
            flex: 0 0 auto;
        }
        .ta-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }

        /* ---------- TABLE ---------- */
        .ta-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .ta-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
        }
        .ta-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33;
            font-size: 11.5px;
            font-weight: 800;
            padding: 14px 10px !important;
            text-align: right;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
            letter-spacing: 0.2px;
        }
        .ta-table tbody td {
            font-size: 12.5px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .ta-table tbody tr {
            transition: all 0.15s ease;
        }
        .ta-table tbody tr:hover td {
            background: #f8fdfa;
        }
        .ta-table tbody tr:last-child td {
            border-bottom: none;
        }

        /* Badges */
        .badge-green,
        .badge-gray,
        .badge-purple,
        .badge-orange {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
        }
        .badge-green {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .badge-gray {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }
        .badge-purple {
            background: rgba(111,66,193,0.12);
            color: #5a2fa0;
            border: 1px solid rgba(111,66,193,0.25);
        }
        .badge-orange {
            background: rgba(253,126,20,0.12);
            color: #b45309;
            border: 1px solid rgba(253,126,20,0.25);
        }

        /* Action Buttons */
        .ta-action-btn {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 6px 12px;
            border-radius: 8px;
            font-size: 11.5px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.15s ease;
            text-decoration: none;
            border: 1.5px solid transparent;
            white-space: nowrap;
            background: #fff;
        }
        .ta-action-btn i { font-size: 10px; }
        .ta-action-btn.approve {
            color: #00b14f;
            border-color: rgba(0,177,79,0.3);
        }
        .ta-action-btn.approve:hover {
            background: #00b14f;
            color: #fff;
            text-decoration: none;
            box-shadow: 0 4px 10px rgba(0,177,79,0.25);
        }
        .ta-action-btn.unapprove {
            color: #d97706;
            border-color: rgba(253,126,20,0.3);
        }
        .ta-action-btn.unapprove:hover {
            background: #fd7e14;
            color: #fff;
            text-decoration: none;
            box-shadow: 0 4px 10px rgba(253,126,20,0.25);
        }

        /* ⭐ زر الحذف (أحمر) */
        .ta-action-btn.delete {
            color: #dc3545;
            border-color: rgba(220,53,69,0.3);
            margin-right: 4px;
        }
        .ta-action-btn.delete:hover {
            background: #dc3545;
            color: #fff;
            text-decoration: none;
            box-shadow: 0 4px 10px rgba(220,53,69,0.25);
        }

        /* ---------- EMPTY STATE ---------- */
        .ta-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .ta-empty-icon {
            width: 80px;
            height: 80px;
            border-radius: 22px;
            background: linear-gradient(135deg, #eee6f8 0%, #ddd0f0 100%);
            color: #6f42c1;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            margin-bottom: 18px;
        }
        .ta-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .ta-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .ta-hero-section { padding: 26px 20px 72px; }
            .ta-hero-titles h1 { font-size: 19px; }
            .ta-body { padding: 0 16px 40px; }
            .ta-card-body { padding: 18px; }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .ta-table { font-size: 11px; min-width: 1000px; }
            .ta-table thead th,
            .ta-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .ta-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .ta-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .ta-hero-titles h1 { font-size: 16px; }
            .ta-hero-titles p { font-size: 12px; }
            .ta-btn-glass { padding: 8px 14px; font-size: 12px; }
            .ta-btn-glass span { display: none; }

            .ta-field { max-width: 100%; flex: 1 1 100%; }
            .ta-toolbar > .ta-btn-primary { flex: 1 1 100%; max-width: 100%; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .ta-hero-section,
            .ta-btn-glass,
            .ta-toolbar,
            .ta-action-btn,
            .no-print { display: none !important; }
            .ta-body { margin: 0 !important; padding: 0 !important; }
            .ta-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .ta-table { font-size: 10.5px !important; }
            .table-responsive { overflow: visible !important; }
            body { background: #fff !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ta-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="ta-hero-section">
            <div class="ta-hero-content">
                <div class="ta-hero-left">
                    <div class="ta-hero-icon">
                        <i class="fas fa-exchange-alt"></i>
                    </div>
                    <div class="ta-hero-titles">
                        <h1>سجل التعويضيات</h1>
                        <p>من غطّى حصة من؟ — سجّل التعويضيات واعتمدها <strong>لتدخل في الرواتب</strong></p>
                    </div>
                </div>
                <a href="Home.aspx" class="ta-btn-glass">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة</span>
                </a>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="ta-body">

            <!-- Alert -->
            <asp:Label ID="lblMessage" runat="server" CssClass="d-none" />

            <!-- ============ FILTER CARD ============ -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>تصفية وعرض التعويضيات</h3>
                            <p>اختر التاريخ والفصل ثم اضغط "عرض"</p>
                        </div>
                    </div>
                </div>
                <div class="ta-card-body">
                    <div class="ta-toolbar">

                        <div class="ta-field" style="max-width:200px;">
                            <label><i class="fas fa-calendar-alt me-1"></i> التاريخ</label>
                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>

                        <div class="ta-field" style="max-width:280px;">
                            <label><i class="fas fa-chalkboard me-1"></i> الفصل</label>
                            <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-select" />
                        </div>

                        <asp:Button ID="btnLoad" runat="server" Text="عرض"
                            CssClass="ta-btn-primary" OnClick="btnLoad_Click" />

                    </div>
                </div>
            </div>

            <!-- ============ ADD SUBSTITUTION CARD ============ -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon purple">
                        <i class="fas fa-plus-circle"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>تسجيل تعويضية جديدة</h3>
                            <p>اختر المعلم الأصلي أولاً، ثم ستظهر حصصه فقط (قد يكون لديه أكثر من مادة)</p>
                        </div>
                    </div>
                </div>
                <div class="ta-card-body">
                    <div class="ta-toolbar">

                        <%-- 1) المعلم الأصلي --%>
                        <div class="ta-field" style="flex:1 1 240px;">
                            <label><i class="fas fa-user me-1"></i> المعلم الأصلي</label>
                            <asp:DropDownList ID="ddlOriginalTeacher" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlOriginalTeacher_SelectedIndexChanged" />
                        </div>

                        <%-- 2) الحصة --%>
                        <div class="ta-field" style="flex:2 1 320px;">
                            <label><i class="fas fa-book me-1"></i> الحصة (المادة - رقم الحصة - الفصل)</label>
                            <asp:DropDownList ID="ddlLesson" runat="server" CssClass="form-select" />
                        </div>

                        <%-- 3) المعلم المُعوِّض --%>
                        <div class="ta-field" style="flex:1 1 220px;">
                            <label><i class="fas fa-user-check me-1"></i> المعلم المُعوِّض</label>
                            <asp:DropDownList ID="ddlSubstituteTeacher" runat="server" CssClass="form-select" />
                        </div>

                        <%-- 4) النوع --%>
                        <div class="ta-field" style="max-width:200px;">
                            <label><i class="fas fa-tag me-1"></i> النوع</label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                                <asp:ListItem Value="SubstitutionFull" Text="تعويضية كاملة" />
                                <asp:ListItem Value="SubstitutionPartial" Text="تعويضية جزئية" />
                            </asp:DropDownList>
                        </div>

                        <%-- 5) ملاحظات --%>
                        <div class="ta-field" style="flex:1 1 220px;">
                            <label><i class="fas fa-sticky-note me-1"></i> ملاحظات</label>
                            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" />
                        </div>

                        <%-- زر التسجيل --%>
                        <asp:Button ID="btnAdd" runat="server" Text="تسجيل تعويضية"
                            CssClass="ta-btn-primary" OnClick="btnAdd_Click" />

                    </div>
                </div>
            </div>

            <!-- ============ TABLE CARD ============ -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon orange">
                        <i class="fas fa-list-ul"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>قائمة التعويضيات</h3>
                            <p>جميع التعويضيات المسجلة لهذا اليوم</p>
                        </div>
                    </div>
                </div>
                <div class="ta-card-body" style="padding:16px;">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvSubstitutions" runat="server"
                            CssClass="ta-table"
                            AutoGenerateColumns="false"
                            EmptyDataText=""
                            GridLines="None"
                            ShowHeaderWhenEmpty="true"
                            DataKeyNames="Id"
                            OnRowCommand="gvSubstitutions_RowCommand"
                            OnRowDeleting="gvSubstitutions_RowDeleting">
                            <Columns>
                                <asp:TemplateField HeaderText="#" ItemStyle-Width="50px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span style="color:#adb5bd;font-weight:700;font-size:12px;">
                                            <%# Container.DataItemIndex + 1 %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="DateText" HeaderText="التاريخ"
                                    ItemStyle-Width="110px" ItemStyle-CssClass="text-center" />

                                <asp:BoundField DataField="ClassName" HeaderText="الفصل" />

                                <asp:BoundField DataField="PeriodNumber" HeaderText="الحصة"
                                    ItemStyle-Width="80px" ItemStyle-CssClass="text-center" />

                                <asp:BoundField DataField="SubjectName" HeaderText="المادة" />

                                <asp:TemplateField HeaderText="المعلم الأصلي">
                                    <ItemTemplate>
                                        <div class="d-flex align-items-center gap-2">
                                            <span style="width:28px;height:28px;border-radius:8px;background:linear-gradient(135deg,#e6f7ec,#d0eedd);color:#00b14f;display:inline-flex;align-items:center;justify-content:center;font-size:12px;flex-shrink:0;">
                                                <i class="fas fa-user"></i>
                                            </span>
                                            <span style="font-weight:600;"><%#: Eval("OriginalTeacherName") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المعلم المُعوِّض">
                                    <ItemTemplate>
                                        <div class="d-flex align-items-center gap-2">
                                            <span style="width:28px;height:28px;border-radius:8px;background:rgba(111,66,193,0.12);color:#6f42c1;display:inline-flex;align-items:center;justify-content:center;font-size:12px;flex-shrink:0;">
                                                <i class="fas fa-user-check"></i>
                                            </span>
                                            <span style="font-weight:700;color:#5a2fa0;"><%#: Eval("SubstituteTeacherName") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="النوع" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="badge-purple">
                                            <i class="fas fa-exchange-alt" style="font-size:10px;"></i>
                                            <%#: Eval("StatusText") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-Width="130px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# (bool)Eval("IsApproved") ? "badge-green" : "badge-gray" %>'>
                                            <i class='fas <%# (bool)Eval("IsApproved") ? "fa-check-circle" : "fa-hourglass-half" %>' style="font-size:10px;"></i>
                                            <%# (bool)Eval("IsApproved") ? "معتمدة" : "غير معتمدة" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- ⭐ إجراءات: اعتماد/تراجع + حذف --%>
                                <asp:TemplateField HeaderText="إجراءات" ItemStyle-Width="240px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnApproveItem" runat="server"
                                            CssClass='<%# (bool)Eval("IsApproved") ? "ta-action-btn unapprove" : "ta-action-btn approve" %>'
                                            CommandName='<%# (bool)Eval("IsApproved") ? "Unapprove" : "Approve" %>'
                                            CommandArgument='<%# Eval("Id") %>'
                                            CausesValidation="false">
                                            <i class='fas <%# (bool)Eval("IsApproved") ? "fa-undo" : "fa-check" %>'></i>
                                            <%# (bool)Eval("IsApproved") ? "تراجع" : "اعتماد" %>
                                        </asp:LinkButton>

                                        <%-- ⭐ زر الحذف — CommandName="RemoveSub" لتجنب التعارض مع Delete المحجوز --%>
                                        <asp:LinkButton ID="btnDeleteItem" runat="server"
                                            CssClass="ta-action-btn delete"
                                            CommandName="RemoveSub"
                                            CommandArgument='<%# Eval("Id") %>'
                                            CausesValidation="false"
                                            OnClientClick="return confirm('هل أنت متأكد من حذف هذه التعويضية؟ لا يمكن التراجع عن هذا الإجراء.');">
                                            <i class="fas fa-trash"></i>
                                            حذف
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon">
                            <i class="fas fa-exchange-alt"></i>
                        </div>
                        <h4>لا توجد تعويضيات مسجلة</h4>
                        <p>سجّل تعويضية جديدة من النموذج بالأعلى</p>
                    </asp:Panel>

                </div>
            </div>

        </div>
    </div>
</asp:Content>