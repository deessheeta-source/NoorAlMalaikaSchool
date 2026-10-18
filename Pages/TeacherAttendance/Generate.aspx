<%@ Page Title="توليد الحصص اليومية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Generate.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.Generate" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Generate Daily Lessons
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
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
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
            padding: 14px 18px;
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1px solid rgba(0,177,79,0.2);
            border-radius: 12px;
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
            gap: 16px;
            flex-wrap: wrap;
        }
        .ta-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 200px;
            flex: 1 1 200px;
            max-width: 260px;
        }
        .ta-field label {
            font-size: 11.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
        }
        .ta-field .form-control {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
        }
        .ta-field .form-control:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
        }

        .ta-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 24px;
            border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            color: #fff;
            font-size: 13.5px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
            flex: 1 1 220px;
            max-width: 300px;
            height: 42px;
        }
        .ta-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
        }
        .ta-btn-primary i {
            font-size: 14px;
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
            font-size: 12px;
            font-weight: 800;
            padding: 14px 12px !important;
            text-align: right;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
            letter-spacing: 0.2px;
        }
        .ta-table tbody td {
            font-size: 12.5px;
            padding: 14px 12px !important;
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

        .ta-time-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 5px 12px;
            border-radius: 8px;
            background: #f8f9fa;
            border: 1px solid #eef1f3;
            font-size: 11.5px;
            font-weight: 700;
            color: #495057;
            font-family: 'Courier New', monospace;
            direction: ltr;
        }

        .ta-duration-badge {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 4px 10px;
            border-radius: 8px;
            background: linear-gradient(135deg, rgba(13,110,253,0.1), rgba(13,110,253,0.05));
            color: #0d6efd;
            font-size: 11.5px;
            font-weight: 700;
            border: 1px solid rgba(13,110,253,0.15);
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

        @media (max-width: 576px) {
            .ta-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .ta-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .ta-hero-titles h1 { font-size: 16px; }
            .ta-hero-titles p { font-size: 12px; }
            .ta-btn-back { padding: 8px 14px; font-size: 12px; }
            .ta-btn-back span { display: none; }

            .ta-field { max-width: 100%; flex: 1 1 100%; }
            .ta-btn-primary { max-width: 100%; flex: 1 1 100%; }

            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .ta-table { font-size: 11.5px; min-width: 720px; }
            .ta-table thead th { padding: 10px 8px !important; }
            .ta-table tbody td { padding: 10px 8px !important; }
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
                        <i class="fas fa-cogs"></i>
                    </div>
                    <div class="ta-hero-titles">
                        <h1>توليد الحصص اليومية</h1>
                        <p>إنشاء ومزامنة الحصص اليومية من الجدول المدرسي المعتمد — يتم تحديث الحصص تلقائياً دون المساس بالسجلات التاريخية.</p>
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
                    يتم <strong>إنشاء ومزامنة</strong> الحصص اليومية من الجدول المدرسي المعتمد.
                    عند تعديل أوقات الحصص، تتم مزامنة الحصص التي <strong>لم يُسجل عليها حضور</strong> بعد،
                    بينما تُحفظ الحصص التي تحتوي على حضور <strong>كسجل تاريخي مالي</strong>.
                </span>
            </div>

            <!-- Toolbar Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon">
                        <i class="fas fa-sliders-h"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <h3>خيارات التوليد</h3>
                        <p>اختر التاريخ ثم اضغط "توليد / مزامنة" لإنشاء حصص اليوم</p>
                    </div>
                </div>
                <div class="ta-card-body">
                    <div class="ta-toolbar">

                        <div class="ta-field">
                            <label>
                                <i class="fas fa-calendar-alt me-1" style="color:#00b14f;"></i>
                                التاريخ
                            </label>
                            <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                        <asp:Button ID="btnGenerate" runat="server"
                            Text="توليد / مزامنة حصص اليوم"
                            CssClass="ta-btn-primary"
                            OnClick="btnGenerate_Click" />

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
                        <p>قائمة الحصص المُولّدة أو المُزامَنة لهذا اليوم</p>
                    </div>
                </div>
                <div class="ta-card-body" style="padding:16px;">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvLessons" runat="server"
                            CssClass="ta-table"
                            AutoGenerateColumns="false"
                            EmptyDataText=""
                            GridLines="None"
                            ShowHeaderWhenEmpty="true">
                            <Columns>
                                <asp:TemplateField HeaderText="#" ItemStyle-Width="50px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span style="color:#adb5bd;font-weight:700;font-size:12px;">
                                            <%# Container.DataItemIndex + 1 %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحصة" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="ta-period-badge"><%# Eval("PeriodNumber") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المعلم/ة">
                                    <ItemTemplate>
                                        <div class="ta-teacher">
                                            <span class="ta-avatar">
                                                <i class="fas fa-chalkboard-teacher"></i>
                                            </span>
                                            <span><%# Eval("TeacherName") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="SubjectName" HeaderText="المادة" />

                                <asp:BoundField DataField="ClassName" HeaderText="الفصل" />

                                <asp:TemplateField HeaderText="الوقت" ItemStyle-Width="140px">
                                    <ItemTemplate>
                                        <span class="ta-time-badge">
                                            <i class="fas fa-clock" style="font-size:10px;"></i>
                                            <%# Eval("StartTimeText") %> - <%# Eval("EndTimeText") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الدقائق" ItemStyle-Width="90px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="ta-duration-badge">
                                            <i class="fas fa-hourglass-half" style="font-size:10px;"></i>
                                            <%# Eval("DurationMinutes") %>
                                        </span>
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
                        <p>اضغط "توليد / مزامنة حصص اليوم" لإنشاء الحصص من الجدول المدرسي</p>
                    </asp:Panel>

                </div>
            </div>

        </div>
    </div>
</asp:Content>