<%@ Page Title="الفترات المالية" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Periods.aspx.cs"
    Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.Periods" %><%@ Import Namespace="NoorAlMalaikaSchool.Models" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Financial Periods
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
            background: linear-gradient(135deg,#8a67d8 0%,#825ec2 100%);
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

        .ta-hero-steps {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            margin-top: 10px;
            padding: 5px 12px;
            border-radius: 999px;
            background: rgba(255,255,255,0.18);
            border: 1px solid rgba(255,255,255,0.28);
            font-size: 11.5px;
            font-weight: 700;
            color: #fff;
            flex-wrap: wrap;
        }
        .ta-hero-steps i {
            font-size: 9px;
            opacity: 0.7;
        }
        .ta-hero-steps span {
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        .ta-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
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
            white-space: nowrap;
        }
        .ta-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .ta-btn-glass-primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .ta-btn-glass-primary:hover {
            background: #f8f9fa;
            color: #007a33;
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
        .ta-card-header-icon.purple {
            background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%);
            box-shadow: 0 6px 16px rgba(111,66,193,0.3);
        }
        .ta-card-header-icon.orange {
            background: linear-gradient(135deg, #fd7e14 0%, #ffb066 100%);
            box-shadow: 0 6px 16px rgba(253,126,20,0.3);
        }
        .ta-card-header-icon.red {
            background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%);
            box-shadow: 0 6px 16px rgba(220,53,69,0.3);
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

        /* ---------- INFO NOTE ---------- */
        .ta-note {
            display: flex;
            align-items: flex-start;
            gap: 12px;
            padding: 14px 18px;
            background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 14px;
            margin-bottom: 20px;
            font-size: 13px;
            color: #1f3d33;
            line-height: 1.7;
            font-weight: 500;
        }
        .ta-note i {
            color: #00b14f;
            font-size: 18px;
            flex-shrink: 0;
            margin-top: 2px;
        }
        .ta-note strong {
            color: #007a33;
            font-weight: 800;
        }

        /* ---------- BUTTONS ---------- */
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
            padding: 10px 18px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #dee2e6;
            color: #495057;
            font-size: 12.5px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 40px;
            text-decoration: none;
        }
        .ta-btn-outline:hover {
            border-color: #00b14f;
            color: #00b14f;
            background: rgba(0,177,79,0.04);
            text-decoration: none;
            transform: translateY(-1px);
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

        .ta-period-name {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-weight: 800;
            color: #212529;
            font-size: 13px;
        }
        .ta-period-icon {
            width: 32px;
            height: 32px;
            border-radius: 9px;
            background: linear-gradient(135deg, rgba(0,177,79,0.12) 0%, rgba(0,177,79,0.06) 100%);
            color: #00b14f;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            flex-shrink: 0;
            border: 1px solid rgba(0,177,79,0.2);
        }

        .ta-total-amount {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 9px;
            background: linear-gradient(135deg, rgba(0,177,79,0.1), rgba(0,177,79,0.05));
            color: #007a33;
            font-weight: 800;
            font-size: 12.5px;
            border: 1px solid rgba(0,177,79,0.18);
            font-family: 'Courier New', monospace;
        }
        .ta-total-amount small {
            font-size: 10px;
            color: #4b7a5f;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }

        /* Status badges */
        .ta-table .badge-blue,
        .ta-table .badge-green,
        .ta-table .badge-yellow,
        .ta-table .badge-red {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
        }
        .ta-table .badge-blue {
            background: rgba(13,110,253,0.1);
            color: #0d6efd;
            border: 1px solid rgba(13,110,253,0.2);
        }
        .ta-table .badge-green {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .ta-table .badge-yellow {
            background: rgba(253,126,20,0.12);
            color: #b45309;
            border: 1px solid rgba(253,126,20,0.25);
        }
        .ta-table .badge-red {
            background: rgba(220,53,69,0.12);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }

        /* Action buttons */
        .ta-actions-cell {
            display: flex;
            flex-wrap: wrap;
            gap: 5px;
            justify-content: flex-start;
        }
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
            border: 1px solid transparent;
            white-space: nowrap;
            background: #fff;
        }
        .ta-action-btn i { font-size: 10px; }

        .ta-action-btn.details {
            color: #495057;
            border-color: #dee2e6;
        }
        .ta-action-btn.details:hover {
            color: #0d6efd;
            border-color: #0d6efd;
            background: rgba(13,110,253,0.04);
            text-decoration: none;
        }
        .ta-action-btn.check {
            color: #6f42c1;
            border-color: rgba(111,66,193,0.3);
        }
        .ta-action-btn.check:hover {
            background: rgba(111,66,193,0.08);
            text-decoration: none;
            color: #6f42c1;
        }
        .ta-action-btn.calc {
            color: #00b14f;
            border-color: rgba(0,177,79,0.3);
        }
        .ta-action-btn.calc:hover {
            background: rgba(0,177,79,0.08);
            text-decoration: none;
            color: #00b14f;
        }
        .ta-action-btn.approve {
            color: #fff;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            box-shadow: 0 4px 10px rgba(0,177,79,0.25);
        }
        .ta-action-btn.approve:hover {
            box-shadow: 0 6px 16px rgba(0,177,79,0.35);
            transform: translateY(-1px);
            color: #fff;
            text-decoration: none;
        }
        .ta-action-btn.close {
            color: #dc3545;
            border-color: rgba(220,53,69,0.3);
        }
        .ta-action-btn.close:hover {
            background: rgba(220,53,69,0.08);
            text-decoration: none;
            color: #dc3545;
        }

        /* ---------- READINESS PANEL ---------- */
        .ta-readiness-summary {
            display: flex;
            align-items: center;
            gap: 8px;
            flex-wrap: wrap;
            padding: 14px 18px;
            background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 12px;
            margin-bottom: 16px;
            font-size: 12.5px;
            font-weight: 600;
            color: #495057;
        }
        .ta-readiness-summary .stat {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 3px 10px;
            border-radius: 999px;
            background: #fff;
            border: 1px solid #e9ecef;
            font-size: 11.5px;
            font-weight: 700;
        }
        .ta-readiness-summary .stat.green { color: #007a33; border-color: rgba(0,177,79,0.3); }
        .ta-readiness-summary .stat.orange { color: #b45309; border-color: rgba(253,126,20,0.3); }
        .ta-readiness-summary .stat.red { color: #a51c1c; border-color: rgba(220,53,69,0.3); }

        .ta-readiness-summary .badge-green,
        .ta-readiness-summary .badge-red {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            border-radius: 999px;
            font-size: 12.5px;
            font-weight: 800;
        }
        .ta-readiness-summary .badge-green {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .ta-readiness-summary .badge-red {
            background: rgba(220,53,69,0.12);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }

        /* Issues severity */
        .ta-severity {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
        }
        .ta-severity.error {
            background: rgba(220,53,69,0.12);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .ta-severity.warn {
            background: rgba(253,126,20,0.12);
            color: #b45309;
            border: 1px solid rgba(253,126,20,0.25);
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
            margin: 0 0 18px;
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
            .ta-actions-cell { gap: 3px; }
            .ta-action-btn { padding: 5px 8px; font-size: 10.5px; }
        }

        @media (max-width: 576px) {
            .ta-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .ta-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .ta-hero-titles h1 { font-size: 16px; }
            .ta-hero-titles p { font-size: 12px; }
            .ta-hero-steps { display: none; }
            .ta-btn-glass { padding: 8px 14px; font-size: 12px; }
            .ta-btn-glass span { display: none; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .ta-hero-section,
            .ta-btn-glass,
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
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="ta-hero-titles">
                        <h1>الفترات المالية والاستحقاق الشهري</h1>
                        <p>إدارة دورة الاستحقاق المالي الشهرية للمعلمين والمعلمات</p>
                        <span class="ta-hero-steps">
                            <span><i class="fas fa-sync-alt"></i> مزامنة</span>
                            <i class="fas fa-chevron-left"></i>
                            <span><i class="fas fa-stethoscope"></i> فحص</span>
                            <i class="fas fa-chevron-left"></i>
                            <span><i class="fas fa-calculator"></i> حساب</span>
                            <i class="fas fa-chevron-left"></i>
                            <span><i class="fas fa-check-circle"></i> اعتماد</span>
                            <i class="fas fa-chevron-left"></i>
                            <span><i class="fas fa-lock"></i> إغلاق</span>
                        </span>
                    </div>
                </div>
                <div class="ta-hero-actions">
                    <a href="<%= ResolveUrl("~/Pages/Payroll/Create.aspx") %>" class="ta-btn-glass ta-btn-glass-primary">
                        <i class="fas fa-calendar-plus"></i>
                        <span>فتح فترة جديدة</span>
                    </a>
                    <a href="Home.aspx" class="ta-btn-glass">
                        <i class="fas fa-arrow-right"></i>
                        <span>عودة</span>
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="ta-body">

            <!-- Alert -->
            <asp:Label ID="lblMessage" runat="server" CssClass="d-none" />

            <!-- Info Note + Sync -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon purple">
                        <i class="fas fa-info-circle"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>دورة العمل المالي</h3>
                            <p>سير المرحلة: فحص الجاهزية ← احسب الاستحقاق ← اعتماد الفترة ← إغلاق نهائي</p>
                        </div>
                    </div>
                    <asp:LinkButton ID="btnSyncAll" runat="server"
                        CssClass="ta-btn-outline"
                        OnClick="btnSyncAll_Click"
                        OnClientClick="return confirm('سيتم مزامنة حصص الفترات المفتوحة من الجدول المعتمد. لن تُلمس الحصص التي عليها حضور مسجل. متابعة؟');">
                        <i class="fas fa-sync-alt"></i> مزامنة الفترات المفتوحة
                    </asp:LinkButton>
                </div>
            </div>

            <!-- Periods Table Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon">
                        <i class="fas fa-list-ul"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>الفترات المالية</h3>
                            <p>قائمة الفترات المفتوحة والمعتمدة والمغلقة</p>
                        </div>
                    </div>
                </div>
                <div class="ta-card-body" style="padding:16px;">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvPeriods" runat="server"
                            CssClass="ta-table"
                            AutoGenerateColumns="false"
                            EmptyDataText=""
                            GridLines="None"
                            ShowHeaderWhenEmpty="true"
                            OnRowCommand="gvPeriods_RowCommand"
                            DataKeyNames="Id">
                            <Columns>
                                <asp:TemplateField HeaderText="#" ItemStyle-Width="50px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span style="color:#adb5bd;font-weight:700;font-size:12px;">
                                            <%# Container.DataItemIndex + 1 %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الفترة">
                                    <ItemTemplate>
                                        <div class="ta-period-name">
                                            <span class="ta-period-icon">
                                                <i class="fas fa-calendar-alt"></i>
                                            </span>
                                            <span><%# GetMonthName(Eval("Month") as int?) + " " + Eval("Year") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="StartDate" HeaderText="من"
                                    DataFormatString="{0:yyyy-MM-dd}" ItemStyle-CssClass="text-center" />

                                <asp:BoundField DataField="EndDate" HeaderText="إلى"
                                    DataFormatString="{0:yyyy-MM-dd}" ItemStyle-CssClass="text-center" />

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# GetStatusClass(Eval("Status") as int?) %>'>
                                            <%# GetStatusName(Eval("Status") as int?) %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="إجمالي المستحق" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="ta-total-amount">
                                            <%# GetPeriodTotal(Eval("Id") as int?) %>
                                            <small>د.ل</small>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="إجراءات">
                                    <ItemTemplate>
                                        <div class="ta-actions-cell">

                                            <a class="ta-action-btn details"
                                               href='<%# "../Payroll/Details.aspx?id=" + Eval("Id") %>'>
                                                <i class="fas fa-list"></i> التفاصيل
                                            </a>

                                            <asp:LinkButton ID="btnCheck" runat="server"
                                                CssClass="ta-action-btn check"
                                                CommandName="Check"
                                                CommandArgument='<%# Eval("Id") %>'>
                                                <i class="fas fa-stethoscope"></i> فحص
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnCalc" runat="server"
                                                CssClass="ta-action-btn calc"
                                                CommandName="Calc"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClientClick="return confirm('إعادة حساب الاستحقاق؟');">
                                                <i class="fas fa-calculator"></i> احسب
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnApprovePeriod" runat="server"
                                                CssClass="ta-action-btn approve"
                                                CommandName="Approve"
                                                CommandArgument='<%# Eval("Id") %>'
                                                Visible='<%# (Eval("Status") as int?) == 1 %>'
                                                OnClientClick="return confirm('اعتماد هذه الفترة؟');">
                                                <i class="fas fa-check-circle"></i> اعتماد
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnClosePeriod" runat="server"
                                                CssClass="ta-action-btn close"
                                                CommandName="Close"
                                                CommandArgument='<%# Eval("Id") %>'
                                                Visible='<%# (Eval("Status") as int?) == 2 %>'
                                                OnClientClick="return confirm('إغلاق نهائي؟ لن يمكن التعديل بعدها.');">
                                                <i class="fas fa-lock"></i> إغلاق
                                            </asp:LinkButton>

                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon">
                            <i class="fas fa-calendar-times"></i>
                        </div>
                        <h4>لا توجد فترات مالية</h4>
                        <p>ابدأ بفتح فترة مالية جديدة للشهر الحالي</p>
                        <a href="<%= ResolveUrl("~/Pages/Payroll/Create.aspx") %>" class="ta-btn-primary">
                            <i class="fas fa-calendar-plus"></i> فتح فترة جديدة
                        </a>
                    </asp:Panel>

                </div>
            </div>

            <!-- Readiness Panel -->
            <asp:Panel ID="pnlReadiness" runat="server" Visible="false" CssClass="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon orange">
                        <i class="fas fa-stethoscope"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>تقرير الجاهزية المالية</h3>
                            <p>فحص شامل للسجلات قبل الحساب المالي والاعتماد</p>
                        </div>
                    </div>
                </div>
                <div class="ta-card-body" style="padding:16px;">

                    <div class="ta-readiness-summary">
                        <asp:Label ID="lblReadinessInfo" runat="server" />
                    </div>

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvIssues" runat="server"
                            CssClass="ta-table"
                            AutoGenerateColumns="false"
                            EmptyDataText=""
                            GridLines="None"
                            ShowHeaderWhenEmpty="true">
                            <Columns>
                                <asp:TemplateField HeaderText="النوع" ItemStyle-Width="100px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# (string)Eval("Severity") == "خطأ" ? "ta-severity error" : "ta-severity warn" %>'>
                                            <i class='fas <%# (string)Eval("Severity") == "خطأ" ? "fa-times-circle" : "fa-exclamation-triangle" %>'></i>
                                            <%# Eval("Severity") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="DateText" HeaderText="التاريخ"
                                    ItemStyle-Width="120px" ItemStyle-CssClass="text-center" />

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

                                <asp:BoundField DataField="PeriodNumber" HeaderText="الحصة"
                                    ItemStyle-Width="80px" ItemStyle-CssClass="text-center" />

                                <asp:BoundField DataField="Message" HeaderText="الوصف" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlIssuesEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon">
                            <i class="fas fa-check-circle"></i>
                        </div>
                        <h4>لا توجد مشاكل</h4>
                        <p>الفترة جاهزة للحساب المالي والاعتماد</p>
                    </asp:Panel>

                </div>
            </asp:Panel>

        </div>
    </div>
</asp:Content>