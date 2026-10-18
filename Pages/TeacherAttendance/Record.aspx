<%@ Page Title="تسجيل الساعات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Record.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.Record" %>
<%@ Import Namespace="NoorAlMalaikaSchool.Models" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Record Daily Hours
           ========================================================= */
        .ta-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .ta-hero-section {
              position: relative;
              background: linear-gradient(135deg, #5083cb 0%, #0d66e9 100%);
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

        .ta-btn-back {
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
            flex-shrink: 0;
        }
        .ta-btn-back:hover {
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
            padding: 24px;
        }

        /* ---------- INFO NOTE ---------- */
        .ta-note {
            display: flex;
            align-items: flex-start;
            gap: 12px;
            padding: 16px 20px;
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 14px;
            margin-bottom: 20px;
        }
        .ta-note i {
            color: #00b14f;
            font-size: 18px;
            flex-shrink: 0;
            margin-top: 2px;
        }
        .ta-note span {
            font-size: 13px;
            color: #1f3d33;
            line-height: 1.7;
            font-weight: 500;
        }
        .ta-note strong {
            color: #007a33;
            font-weight: 800;
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
            padding: 11px 20px;
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
            flex: 0 0 auto;
        }
        .ta-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }

        .ta-btn-outline {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 20px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #dee2e6;
            color: #495057;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 42px;
            flex: 0 0 auto;
            text-decoration: none;
        }
        .ta-btn-outline:hover {
            border-color: #00b14f;
            color: #00b14f;
            background: rgba(0,177,79,0.04);
            text-decoration: none;
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
            padding: 12px 10px !important;
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

        .table-row-approved td {
            background: #f8fdfa !important;
            opacity: 0.9;
        }
        .table-row-approved:hover td {
            background: #f0f8f3 !important;
        }

        /* Row badges */
        .ta-period-badge {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 32px;
            height: 32px;
            border-radius: 9px;
            background: linear-gradient(135deg, rgba(0,177,79,0.12) 0%, rgba(0,177,79,0.06) 100%);
            color: #00b14f;
            font-weight: 800;
            font-size: 13px;
            border: 1px solid rgba(0,177,79,0.2);
        }

        .ta-teacher {
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 700;
            color: #212529;
        }
        .ta-avatar {
            width: 34px;
            height: 34px;
            border-radius: 10px;
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            color: #00b14f;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            flex-shrink: 0;
        }

        .ta-time-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 5px 10px;
            border-radius: 8px;
            background: #f8f9fa;
            border: 1px solid #eef1f3;
            font-size: 11px;
            font-weight: 700;
            color: #495057;
            font-family: 'Courier New', monospace;
            direction: ltr;
        }

        /* ---------- FORM CONTROLS INSIDE TABLE ---------- */
        .ta-table .form-select,
        .ta-table .form-control {
            border: 1.5px solid #e9ecef;
            border-radius: 9px;
            font-size: 12px;
            font-weight: 600;
            padding: 7px 10px;
            background: #fff;
            transition: all 0.15s ease;
        }
        .ta-table .form-select:focus,
        .ta-table .form-control:focus {
            border-color: #00b14f;
            box-shadow: 0 0 0 3px rgba(0,177,79,0.12);
        }
        .minutes-field {
            text-align: center;
            font-weight: 800;
            color: #212529;
        }
        .minutes-field.is-locked {
            background: #f0f2f5 !important;
            cursor: not-allowed;
            color: #495057;
        }
        .minutes-field.has-error {
            border: 2px solid #dc3545 !important;
            background: #fff5f5;
            color: #dc3545;
        }
        .field-error {
            display: none;
            color: #dc3545;
            font-size: 10.5px;
            margin-top: 4px;
            text-align: center;
            font-weight: 700;
            line-height: 1.4;
        }

        /* ---------- ACTION BAR ---------- */
        .ta-actions {
            display: flex;
            align-items: center;
            justify-content: flex-end;
            gap: 10px;
            flex-wrap: wrap;
            margin-top: 20px;
            padding: 16px 20px;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            border: 1px solid #99f6c3;
            border-radius: 16px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.02);
        }

        .ta-btn-success {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 22px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #00b14f;
            color: #00b14f;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 42px;
            text-decoration: none;
        }
        .ta-btn-success:hover {
            background: #00b14f;
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(0,177,79,0.3);
        }

        .ta-btn-warning {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 20px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #f59e0b;
            color: #d97706;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 42px;
            text-decoration: none;
        }
        .ta-btn-warning:hover {
            background: rgba(245,158,11,0.08);
            text-decoration: none;
            color: #d97706;
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
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            color: #00b14f;
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
            .ta-card-body { padding: 20px; }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .ta-table { font-size: 11px; min-width: 900px; }
            .ta-table thead th,
            .ta-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .ta-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .ta-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .ta-hero-titles h1 { font-size: 16px; }
            .ta-hero-titles p { font-size: 12px; }
            .ta-btn-back { padding: 8px 14px; font-size: 12px; }
            .ta-btn-back span { display: none; }

            .ta-field { max-width: 100%; flex: 1 1 100%; }
            .ta-toolbar > div,
            .ta-toolbar > .ta-btn-primary,
            .ta-toolbar > .ta-btn-outline { flex: 1 1 100%; max-width: 100%; }

            .ta-actions { justify-content: stretch; }
            .ta-actions > * { flex: 1 1 100%; }
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
                        <i class="fas fa-pen"></i>
                    </div>
                    <div class="ta-hero-titles">
                        <h1>تسجيل ساعات حضور المعلمين</h1>
                        <p><strong>الخطوة 2 من 3</strong> — سجّل حالة كل معلم ودقائق الحضور الفعلية لكل حصة</p>
                    </div>
                </div>
                <a href="Home.aspx" class="ta-btn-back">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة</span>
                </a>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="ta-body">

            <!-- Alert -->
            <asp:Label ID="lblMessage" runat="server" CssClass="d-none" />

            <!-- Info Note -->
            <div class="ta-note">
                <i class="fas fa-info-circle"></i>
                <span>
                    سجّل حالة كل معلم وعدد دقائق الحضور الفعلية، ثم اضغط <strong>«حفظ الساعات»</strong>.
                    الحفظ لا يعني <strong>الاعتماد المالي</strong> — انتقل بعدها إلى الخطوة التالية «مراجعة واعتماد اليوم».
                </span>
            </div>

            <!-- Toolbar Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon blue">
                        <i class="fas fa-sliders-h"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <h3>خيارات العرض</h3>
                        <p>اختر التاريخ والمعلم ثم اضغط "عرض" لعرض الحصص</p>
                    </div>
                </div>
                <div class="ta-card-body">
                    <div class="ta-toolbar">

                        <div class="ta-field" style="max-width:180px;">
                            <label><i class="fas fa-calendar-alt me-1"></i> التاريخ</label>
                            <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                        <div class="ta-field" style="max-width:260px;">
                            <label><i class="fas fa-chalkboard-teacher me-1"></i> المعلم / المعلمة</label>
                            <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlTeacher_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="جميع المعلمين" />
                            </asp:DropDownList>
                        </div>

                        <asp:Button ID="btnLoad" runat="server" Text="عرض"
                            CssClass="ta-btn-primary" OnClick="btnLoad_Click" />

                        <asp:Button ID="btnGenerateDay" runat="server" Text="توليد الحصص"
                            CssClass="ta-btn-outline" OnClick="btnGenerateDay_Click" />

                    </div>
                </div>
            </div>

            <!-- Lessons Table Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon">
                        <i class="fas fa-list-ul"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <h3>حصص اليوم</h3>
                        <p>سجل حضور كل معلم لكل حصة ثم اضغط "حفظ الساعات"</p>
                    </div>
                </div>
                <div class="ta-card-body" style="padding:16px;">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvLessons" runat="server"
                            CssClass="ta-table"
                            AutoGenerateColumns="false"
                            EmptyDataText=""
                            GridLines="None"
                            ShowHeaderWhenEmpty="true"
                            DataKeyNames="Id"
                            OnRowDataBound="gvLessons_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="PeriodNumber" HeaderText="الحصة"
                                    ItemStyle-Width="70px" ItemStyle-CssClass="text-center" />

                                <asp:TemplateField HeaderText="المعلم / المعلمة">
                                    <ItemTemplate>
                                        <div class="ta-teacher">
                                            <span class="ta-avatar">
                                                <i class="fas fa-chalkboard-teacher"></i>
                                            </span>
                                            <span><%#: Eval("TeacherName") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="SubjectName" HeaderText="المادة" />
                                <asp:BoundField DataField="ClassName" HeaderText="الفصل" />

                                <asp:TemplateField HeaderText="الوقت" ItemStyle-Width="130px">
                                    <ItemTemplate>
                                        <span class="ta-time-badge">
                                            <i class="fas fa-clock" style="font-size:10px;"></i>
                                            <%# Eval("StartTimeText") %> - <%# Eval("EndTimeText") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select form-select-sm">
                                            <asp:ListItem Value="PresentFull" Text="حضور كامل" />
                                            <asp:ListItem Value="PresentPartial" Text="حضور جزئي" />
                                            <asp:ListItem Value="AbsentExcused" Text="غياب بعذر" />
                                            <asp:ListItem Value="AbsentUnexcused" Text="غياب بدون عذر" />
                                            <asp:ListItem Value="Cancelled" Text="حصة ملغاة" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحضور الفعلي (دقيقة)" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMinutes" runat="server" TextMode="Number"
                                            CssClass="form-control form-control-sm text-center" Text="45" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="ملاحظات">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control form-control-sm" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon">
                            <i class="fas fa-calendar-times"></i>
                        </div>
                        <h4>لا توجد حصص لهذا اليوم</h4>
                        <p>استخدم زر "توليد الحصص" أو ارجع للصفحة الرئيسية</p>
                    </asp:Panel>

                </div>
            </div>

            <!-- Actions -->
            <div class="ta-actions">

                <a href="Home.aspx" class="ta-btn-outline">
                    <i class="fas fa-times"></i> إلغاء
                </a>

                <asp:Button ID="btnReset" runat="server" Text="إعادة تعيين"
                    CssClass="ta-btn-warning"
                    OnClick="btnReset_Click"
                    OnClientClick="return confirm('سيتم إرجاع جميع الصفوف إلى الحالة الافتراضية (حضور كامل + مدة الحصة). أي تعديلات غير محفوظة ستفقد. متابعة؟');" />

                <asp:Button ID="btnSave" runat="server" Text="حفظ الساعات"
                    CssClass="ta-btn-primary" OnClick="btnSave_Click"
                    OnClientClick="return validateAllRows();" />

                <a href="Review.aspx" class="ta-btn-success">
                    <i class="fas fa-arrow-left"></i> الخطوة التالية: المراجعة والاعتماد
                </a>

            </div>

        </div>
    </div>

    <script type="text/javascript">
        function syncMinutesWithStatus(ddl) {
            if (!ddl) return;

            var row = ddl.closest('tr');
            if (!row) return;

            var txt = row.querySelector('.minutes-field');
            if (!txt) return;
            if (txt.disabled) return;

            var duration = parseInt(txt.getAttribute('data-duration')) || 45;
            var status = ddl.value;

            clearFieldError(txt);

            switch (status) {
                case 'PresentFull':
                    // الحضور لا يعني اكتمال المدة: نتيح تسجيل دقائق أقل من
                    // المدة، وتُدفع الحصة بنسبة وقتها الفعلي.
                    txt.readOnly = false;
                    txt.classList.remove('is-locked');
                    txt.min = 1;
                    txt.max = duration;
                    if (parseInt(txt.value) > duration) txt.value = duration;
                    if (parseInt(txt.value) < 1) txt.value = duration;
                    enableFieldValidators(txt, duration, false);
                    break;

                case 'PresentPartial':
                    txt.readOnly = false;
                    txt.classList.remove('is-locked');
                    txt.min = 1;
                    txt.max = duration - 1;
                    enableFieldValidators(txt, duration, true);
                    break;

                case 'AbsentExcused':
                    txt.readOnly = false;
                    txt.classList.remove('is-locked');
                    txt.min = 0;
                    txt.max = duration;
                    enableFieldValidators(txt, duration, false);
                    break;

                case 'AbsentUnexcused':
                    txt.value = 0;
                    txt.readOnly = true;
                    txt.classList.add('is-locked');
                    txt.min = 0;
                    txt.max = 0;
                    break;

                case 'Cancelled':
                    txt.readOnly = false;
                    txt.classList.remove('is-locked');
                    txt.min = 0;
                    txt.max = duration;
                    enableFieldValidators(txt, duration, false);
                    break;
            }
        }

        function enableFieldValidators(txt, duration, strictLess) {
            txt.oninput = function () {
                var v = parseInt(this.value);
                if (isNaN(v)) return;

                if (v < 0) { this.value = 0; v = 0; }
                if (v > duration) { this.value = duration; v = duration; }

                if (strictLess && v >= duration) {
                    showFieldError(this, "يجب أن يكون أقل من " + duration + " دقيقة");
                } else {
                    clearFieldError(this);
                }
            };

            txt.onblur = function () {
                var v = parseInt(this.value);
                if (isNaN(v)) { this.value = 0; v = 0; }
                if (v < 0) { this.value = 0; v = 0; }
                if (v > duration) { this.value = duration; v = duration; }

                if (strictLess && v >= duration) {
                    showFieldError(this, "الحضور الجزئي يجب أن يكون أقل من " + duration + " دقيقة");
                } else {
                    clearFieldError(this);
                }
            };

            txt.oninput();
        }

        function showFieldError(txt, message) {
            txt.classList.add('has-error');

            var row = txt.closest('tr');
            if (!row) return;

            var err = row.querySelector('.field-error');
            if (!err) {
                err = document.createElement('div');
                err.className = 'field-error';
                txt.parentNode.appendChild(err);
            }
            err.textContent = message;
            err.style.display = 'block';
        }

        function clearFieldError(txt) {
            txt.classList.remove('has-error');

            var row = txt.closest('tr');
            if (!row) return;

            var err = row.querySelector('.field-error');
            if (err) {
                err.textContent = '';
                err.style.display = 'none';
            }
        }

        function validateAllRows() {
            var rows = document.querySelectorAll('.status-dropdown');
            for (var i = 0; i < rows.length; i++) {
                var ddl = rows[i];
                var row = ddl.closest('tr');
                var txt = row.querySelector('.minutes-field');

                if (!txt || txt.disabled) continue;

                var duration = parseInt(txt.getAttribute('data-duration')) || 45;
                var v = parseInt(txt.value);

                if (ddl.value === 'PresentPartial' && (isNaN(v) || v <= 0 || v >= duration)) {
                    showFieldError(txt, "الحضور الجزئي يجب أن يكون بين 1 و " + (duration - 1) + " دقيقة");
                    txt.focus();
                    return false;
                }
            }
            return true;
        }

        document.addEventListener('DOMContentLoaded', function () {
            var ddlList = document.querySelectorAll('.status-dropdown');
            for (var i = 0; i < ddlList.length; i++) {
                syncMinutesWithStatus(ddlList[i]);
            }
        });
    </script>
</asp:Content>