<%@ Page Title="جاهزية الحضور للاستحقاق المالي" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Readiness.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.Readiness" %>

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
            background: linear-gradient(135deg, #e87a83 0%, #e35965 100%);
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
            background: linear-gradient(135deg, #e87a83 0%, #e35965 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
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
            margin-bottom: 20px;
        }
        .ta-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 180px;
            flex: 1 1 180px;
        }
        .ta-field label {
            font-size: 11.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
        }
        .ta-field label i { color: #00b14f; }
        .ta-field .form-select,
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
        .ta-field .form-select:focus,
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
            font-size: 12.5px;
            color: #1f3d33;
            line-height: 1.7;
            font-weight: 500;
        }
        .ta-note i {
            color: #00b14f;
            font-size: 16px;
            flex-shrink: 0;
            margin-top: 2px;
        }
        .ta-note strong {
            color: #007a33;
            font-weight: 800;
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

        .ta-alert.warning {
            background: linear-gradient(180deg, #ffffff 0%, #fffaf0 100%);
            border: 1.5px solid rgba(253,126,20,0.35);
            color: #7a4a05;
        }
        .ta-alert.warning i { background: linear-gradient(135deg, #fd7e14, #ffb066); }

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

        /* ---------- KPI GRID (6 cards) ---------- */
        .ta-kpis {
            display: grid;
            grid-template-columns: repeat(6, 1fr);
            gap: 12px;
            margin-bottom: 22px;
        }
        .ta-kpi {
            background: #ffffff;
            border: 1px solid #99f6c3;
            border-radius: 16px;
            padding: 16px 14px;
            display: flex;
            flex-direction: column;
            gap: 6px;
            position: relative;
            overflow: hidden;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 4px 12px rgba(0,0,0,0.03);
        }
        .ta-kpi::before {
            content: '';
            position: absolute;
            top: 0;
            right: 0;
            width: 100%;
            height: 3px;
            background: currentColor;
            opacity: 0.9;
        }
        .ta-kpi:hover {
            transform: translateY(-4px);
            box-shadow: 0 12px 28px rgba(0,177,79,0.1);
        }
        .ta-kpi-icon {
            width: 36px;
            height: 36px;
            border-radius: 11px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 15px;
            color: #fff;
            margin-bottom: 6px;
            transition: transform 0.3s ease;
        }
        .ta-kpi:hover .ta-kpi-icon {
            transform: scale(1.08) rotate(-4deg);
        }
        .ta-kpi small {
            color: #6c757d;
            font-size: 11.5px;
            font-weight: 700;
            letter-spacing: 0.2px;
        }
        .ta-kpi .ta-kpi-val {
            font-size: 22px;
            font-weight: 800;
            color: #212529;
            font-family: 'Courier New', monospace;
            line-height: 1;
            letter-spacing: -0.5px;
        }
        .ta-kpi .ta-kpi-val.text-danger { color: #dc3545; }
        .ta-kpi .ta-kpi-val.text-warning { color: #fd7e14; }
        .ta-kpi .ta-kpi-val.text-success { color: #00b14f; }

        /* KPI color variants */
        .ta-kpi.kpi-green  { color: #00b14f; }
        .ta-kpi.kpi-green  .ta-kpi-icon { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 14px rgba(0,177,79,0.3); }

        .ta-kpi.kpi-blue   { color: #0d6efd; }
        .ta-kpi.kpi-blue   .ta-kpi-icon { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 14px rgba(13,110,253,0.3); }

        .ta-kpi.kpi-teal   { color: #0dcaf0; }
        .ta-kpi.kpi-teal   .ta-kpi-icon { background: linear-gradient(135deg, #0dcaf0 0%, #63e1f5 100%); box-shadow: 0 6px 14px rgba(13,202,240,0.3); }

        .ta-kpi.kpi-orange { color: #fd7e14; }
        .ta-kpi.kpi-orange .ta-kpi-icon { background: linear-gradient(135deg, #fd7e14 0%, #ffb066 100%); box-shadow: 0 6px 14px rgba(253,126,20,0.3); }

        .ta-kpi.kpi-yellow { color: #f59e0b; }
        .ta-kpi.kpi-yellow .ta-kpi-icon { background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%); box-shadow: 0 6px 14px rgba(245,158,11,0.3); }

        .ta-kpi.kpi-red    { color: #dc3545; }
        .ta-kpi.kpi-red    .ta-kpi-icon { background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%); box-shadow: 0 6px 14px rgba(220,53,69,0.3); }

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

        /* Teacher badge */
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

        /* Severity badges */
        .ta-severity {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            white-space: nowrap;
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
            margin: 0;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 1200px) {
            .ta-kpis { grid-template-columns: repeat(3, 1fr); }
        }

        @media (max-width: 992px) {
            .ta-hero-section { padding: 26px 20px 72px; }
            .ta-hero-titles h1 { font-size: 19px; }
            .ta-body { padding: 0 16px 40px; }
            .ta-card-body { padding: 18px; }
            .ta-kpis { grid-template-columns: repeat(2, 1fr); gap: 10px; }
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
            .ta-btn-glass { padding: 8px 14px; font-size: 12px; }
            .ta-btn-glass span { display: none; }

            .ta-field { max-width: 100%; flex: 1 1 100%; }
            .ta-toolbar > .ta-btn-primary { flex: 1 1 100%; max-width: 100%; }

            .ta-kpis { grid-template-columns: 1fr 1fr; gap: 8px; }
            .ta-kpi { padding: 12px 10px; }
            .ta-kpi .ta-kpi-val { font-size: 18px; }
            .ta-kpi-icon { width: 30px; height: 30px; font-size: 13px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .ta-hero-section,
            .ta-btn-glass,
            .ta-toolbar,
            .no-print { display: none !important; }
            .ta-body { margin: 0 !important; padding: 0 !important; }
            .ta-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .ta-kpis { grid-template-columns: repeat(6, 1fr); }
            .ta-kpi { box-shadow: none !important; border: 1px solid #ddd !important; }
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
                        <i class="fas fa-shield-alt"></i>
                    </div>
                    <div class="ta-hero-titles">
                        <h1>جاهزية الحضور للاستحقاق المالي</h1>
                        <p>افحص الفترة قبل الحساب أو الاعتماد لضمان عدم وجود نواقص أو تعارضات</p>
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

            <!-- Toolbar Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon blue">
                        <i class="fas fa-search"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>فحص الجاهزية</h3>
                            <p>اختر الفترة المالية ثم اضغط "فحص الجاهزية" لبدء الفحص</p>
                        </div>
                    </div>
                </div>
                <div class="ta-card-body">

                    <div class="ta-toolbar">
                        <div class="ta-field" style="flex:2 1 300px;">
                            <label><i class="fas fa-calendar-alt me-1"></i> الفترة المالية</label>
                            <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="form-select" />
                        </div>

                        <asp:Button ID="btnCheck" runat="server" Text="فحص الجاهزية"
                            CssClass="ta-btn-primary" OnClick="btnCheck_Click" />
                    </div>

                    <!-- Info Note -->
                    <div class="ta-note">
                        <i class="fas fa-info-circle"></i>
                        <span>
                            إذا كانت الفترة <strong>مفتوحة</strong> أو <strong>قيد المراجعة</strong>،
                            يتم فحص الأيام حتى تاريخ اليوم فقط. <strong>الأيام المستقبلية</strong>
                            لا تدخل في النقص المالي.
                        </span>
                    </div>

                    <!-- Summary Alert -->
                    <asp:Label ID="lblSummary" runat="server" CssClass="d-none" />

                </div>
            </div>

            <!-- KPI Cards -->
            <div class="ta-kpis">

                <div class="ta-kpi kpi-blue">
                    <div class="ta-kpi-icon"><i class="fas fa-calendar-check"></i></div>
                    <small>الحصص المجدولة</small>
                    <div class="ta-kpi-val"><asp:Label ID="lblScheduled" runat="server" Text="0" /></div>
                </div>

                <div class="ta-kpi kpi-teal">
                    <div class="ta-kpi-icon"><i class="fas fa-pen"></i></div>
                    <small>المسجلة</small>
                    <div class="ta-kpi-val"><asp:Label ID="lblRecorded" runat="server" Text="0" /></div>
                </div>

                <div class="ta-kpi kpi-green">
                    <div class="ta-kpi-icon"><i class="fas fa-check-circle"></i></div>
                    <small>المعتمدة</small>
                    <div class="ta-kpi-val text-success"><asp:Label ID="lblApproved" runat="server" Text="0" /></div>
                </div>

                <div class="ta-kpi kpi-red">
                    <div class="ta-kpi-icon"><i class="fas fa-exclamation-circle"></i></div>
                    <small>الناقصة</small>
                    <div class="ta-kpi-val text-danger"><asp:Label ID="lblMissing" runat="server" Text="0" /></div>
                </div>

                <div class="ta-kpi kpi-orange">
                    <div class="ta-kpi-icon"><i class="fas fa-hourglass-half"></i></div>
                    <small>غير معتمدة</small>
                    <div class="ta-kpi-val text-warning"><asp:Label ID="lblUnapproved" runat="server" Text="0" /></div>
                </div>

                <div class="ta-kpi kpi-yellow">
                    <div class="ta-kpi-icon"><i class="fas fa-shield-alt"></i></div>
                    <small>التعارضات</small>
                    <div class="ta-kpi-val text-danger"><asp:Label ID="lblConflicts" runat="server" Text="0" /></div>
                </div>

            </div>

            <!-- Issues Table Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon orange">
                        <i class="fas fa-exclamation-triangle"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>قائمة المخالفات والتعارضات</h3>
                            <p>تفاصيل المشاكل التي يجب حلها قبل الحساب المالي</p>
                        </div>
                    </div>
                </div>
                <div class="ta-card-body" style="padding:16px;">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvIssues" runat="server"
                            CssClass="ta-table"
                            AutoGenerateColumns="false"
                            EmptyDataText=""
                            GridLines="None"
                            ShowHeaderWhenEmpty="true">
                            <Columns>
                                <asp:TemplateField HeaderText="النوع" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# (string)Eval("Severity") == "خطأ" ? "ta-severity error" : "ta-severity warn" %>'>
                                            <i class='fas <%# (string)Eval("Severity") == "خطأ" ? "fa-times-circle" : "fa-exclamation-triangle" %>'></i>
                                            <%# Eval("Severity") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="DateText" HeaderText="التاريخ"
                                    ItemStyle-Width="130px" ItemStyle-CssClass="text-center" />

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
                                    ItemStyle-Width="90px" ItemStyle-CssClass="text-center" />

                                <asp:BoundField DataField="Message" HeaderText="الملاحظة" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon">
                            <i class="fas fa-check-circle"></i>
                        </div>
                        <h4>لا توجد مخالفات أو تعارضات</h4>
                        <p>الفترة المالية جاهزة للحساب والاعتماد</p>
                    </asp:Panel>

                </div>
            </div>

        </div>
    </div>
</asp:Content>