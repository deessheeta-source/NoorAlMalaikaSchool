<%@ Page Title="مراجعة واعتماد اليوم" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Review.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.Review" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Review & Approve Day
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
            background: linear-gradient(135deg, #ec9342 0%, #fd841d 100%);
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
        .ta-card-header-icon.orange {
            background: linear-gradient(135deg, #fd7e14 0%, #ffb066 100%);
            box-shadow: 0 6px 16px rgba(253,126,20,0.3);
        }
        .ta-card-header-icon.red {
            background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%);
            box-shadow: 0 6px 16px rgba(220,53,69,0.3);
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

        /* ---------- WORKFLOW STATE BAR ---------- */
        .ta-workflow {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 14px 20px;
            background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 14px;
            margin-bottom: 16px;
            font-size: 12.5px;
            font-weight: 600;
            color: #1f3d33;
            flex-wrap: wrap;
        }
        .ta-workflow i.ta-workflow-icon {
            width: 34px;
            height: 34px;
            border-radius: 10px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            flex-shrink: 0;
        }
        .ta-workflow .stat-item {
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
        .ta-workflow .stat-item.green {
            color: #007a33;
            border-color: rgba(0,177,79,0.3);
            background: rgba(0,177,79,0.06);
        }
        .ta-workflow .stat-item.orange {
            color: #b45309;
            border-color: rgba(253,126,20,0.3);
            background: rgba(253,126,20,0.06);
        }
        .ta-workflow .stat-item.red {
            color: #a51c1c;
            border-color: rgba(220,53,69,0.3);
            background: rgba(220,53,69,0.06);
        }

        /* ---------- APPROVAL STATE BADGE ---------- */
        .approval-state-wrap {
            margin-bottom: 16px;
        }
        .approval-state-wrap .badge-green,
        .approval-state-wrap .badge-yellow,
        .approval-state-wrap .badge-red,
        .approval-state-wrap .badge-gray {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 12px 20px;
            border-radius: 14px;
            font-size: 14px;
            font-weight: 800;
            letter-spacing: 0.2px;
        }
        .approval-state-wrap .badge-green {
            background: linear-gradient(135deg, rgba(0,177,79,0.12), rgba(0,177,79,0.06));
            color: #007a33;
            border: 1.5px solid rgba(0,177,79,0.3);
        }
        .approval-state-wrap .badge-yellow {
            background: linear-gradient(135deg, rgba(253,126,20,0.12), rgba(253,126,20,0.06));
            color: #b45309;
            border: 1.5px solid rgba(253,126,20,0.3);
        }
        .approval-state-wrap .badge-red {
            background: linear-gradient(135deg, rgba(220,53,69,0.12), rgba(220,53,69,0.06));
            color: #a51c1c;
            border: 1.5px solid rgba(220,53,69,0.3);
        }
        .approval-state-wrap .badge-gray {
            background: #f8f9fa;
            color: #495057;
            border: 1.5px solid #e9ecef;
        }

        /* ---------- CONFLICT PANEL ---------- */
        .conflict-panel {
            background: linear-gradient(180deg, #ffffff 0%, #fef8f8 100%);
            border: 1.5px solid rgba(220,53,69,0.3);
            border-radius: 16px;
            padding: 20px 22px;
            margin-bottom: 20px;
            box-shadow: 0 8px 24px rgba(220,53,69,0.06);
        }
        .conflict-title {
            font-weight: 800;
            color: #a51c1c;
            font-size: 15px;
            margin-bottom: 6px;
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .conflict-title i {
            width: 32px;
            height: 32px;
            border-radius: 9px;
            background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            flex-shrink: 0;
        }
        .conflict-help {
            font-size: 12.5px;
            color: #6c757d;
            margin-bottom: 14px;
            padding-right: 40px;
        }
        .conflict-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }
        .conflict-item {
            display: flex;
            gap: 12px;
            align-items: flex-start;
            border: 1px solid #f0d0d0;
            border-radius: 12px;
            background: #fff;
            padding: 12px 16px;
            font-size: 12.5px;
            transition: all 0.15s ease;
        }
        .conflict-item:hover {
            border-color: #dc3545;
            box-shadow: 0 4px 12px rgba(220,53,69,0.08);
        }
        .conflict-index {
            width: 28px;
            height: 28px;
            border-radius: 50%;
            background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%);
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: center;
            flex: 0 0 28px;
            font-weight: 800;
            font-size: 12px;
            box-shadow: 0 4px 8px rgba(220,53,69,0.25);
        }
        .conflict-body {
            flex: 1;
            line-height: 1.9;
            color: #495057;
        }
        .conflict-body strong {
            color: #212529;
            font-weight: 800;
        }
        .conflict-message {
            margin-top: 6px;
            color: #a51c1c;
            font-weight: 600;
            background: rgba(220,53,69,0.05);
            padding: 6px 12px;
            border-radius: 8px;
            border-right: 3px solid #dc3545;
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

        .ta-minutes-badge {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 4px 10px;
            border-radius: 8px;
            background: linear-gradient(135deg, rgba(13,110,253,0.1), rgba(13,110,253,0.05));
            color: #0d6efd;
            font-size: 11.5px;
            font-weight: 800;
            border: 1px solid rgba(13,110,253,0.15);
            font-family: 'Courier New', monospace;
        }

        /* Status badges in table */
        .ta-table .badge-green,
        .ta-table .badge-yellow,
        .ta-table .badge-red,
        .ta-table .badge-gray {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
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
        .ta-table .badge-gray {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }

        /* ---------- SUMMARY ---------- */
        .ta-summary {
            display: flex;
            align-items: center;
            gap: 8px;
            padding: 12px 18px;
            background: #f8f9fa;
            border: 1px solid #eef1f3;
            border-radius: 12px;
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            margin-top: 16px;
        }
        .ta-summary i {
            color: #00b14f;
            font-size: 14px;
        }

        /* ---------- ACTIONS BAR ---------- */
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
            padding: 12px 26px;
            border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            color: #fff;
            font-size: 13.5px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            height: 44px;
            text-decoration: none;
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
        }
        .ta-btn-success:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
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

            .approval-state-wrap .badge-green,
            .approval-state-wrap .badge-yellow,
            .approval-state-wrap .badge-red,
            .approval-state-wrap .badge-gray {
                width: 100%;
                justify-content: center;
                font-size: 13px;
            }
        }

        /* Print */
        @media print {
            .ta-hero-section,
            .ta-btn-back,
            .ta-actions,
            .ta-toolbar,
            .no-print { display: none !important; }
            .ta-body { margin: 0 !important; padding: 0 !important; }
            .ta-card { box-shadow: none !important; border: 1px solid #ddd !important; }
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
                        <i class="fas fa-clipboard-check"></i>
                    </div>
                    <div class="ta-hero-titles">
                        <h1>مراجعة واعتماد اليوم مالياً</h1>
                        <p><strong>الخطوة 3 من 3</strong> — راجع السجلات ثم اعتمدها مالياً لتصبح محسوبة في الاستحقاق الشهري</p>
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

            <!-- Toolbar Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon orange">
                        <i class="fas fa-sliders-h"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <h3>خيارات العرض</h3>
                        <p>اختر التاريخ والمعلم ثم اضغط "عرض" لمراجعة السجلات</p>
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

                        <asp:LinkButton ID="btnPrint" runat="server" CssClass="ta-btn-outline"
                            OnClientClick="window.print();return false;">
                            <i class="fas fa-print"></i> طباعة
                        </asp:LinkButton>

                    </div>
                </div>
            </div>

            <!-- Workflow State Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon">
                        <i class="fas fa-tasks"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <h3>حالة اعتماد اليوم</h3>
                        <p>ملخص سريع لحالة اليوم الحالية وتفاصيل النواقص</p>
                    </div>
                </div>
                <div class="ta-card-body">

                    <!-- Workflow stats bar -->
                    <div class="ta-workflow">
                        <i class="fas fa-info-circle ta-workflow-icon"></i>
                        <asp:Label ID="lblWorkflowDetail" runat="server" />
                    </div>

                    <!-- Approval State Badge -->
                    <div class="approval-state-wrap">
                        <asp:Label ID="lblApprovalState" runat="server" />
                    </div>

                </div>
            </div>

            <!-- Conflict Panel -->
           <div class="conflict-panel" id="conflictPanel" runat="server" visible="false">
                <asp:Label ID="lblConflictDetails" runat="server" />
            </div>

            <!-- Records Table Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon red">
                        <i class="fas fa-list-ul"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <h3>سجلات الحضور المسجلة</h3>
                        <p>راجع الحالات والدقائق ثم اعتمد</p>
                    </div>
                </div>
                <div class="ta-card-body" style="padding:16px;">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvReview" runat="server"
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

                                <asp:TemplateField HeaderText="الحالة">
                                    <ItemTemplate>
                                        <span class="<%# StatusClass((int)Eval("Status")) %>">
                                            <%# StatusName((int)Eval("Status")) %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الدقائق" ItemStyle-Width="100px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="ta-minutes-badge">
                                            <i class="fas fa-hourglass-half" style="font-size:10px;"></i>
                                            <%# Eval("ActualMinutes") %>/<%# Eval("DurationMinutes") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الاعتماد" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# (bool)Eval("IsApproved") ? "badge-green" : "badge-gray" %>'>
                                            <i class='fas <%# (bool)Eval("IsApproved") ? "fa-check-circle" : "fa-hourglass-half" %>'></i>
                                            <%# (bool)Eval("IsApproved") ? "معتمد" : "غير معتمد" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="Notes" HeaderText="ملاحظات" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon">
                            <i class="fas fa-inbox"></i>
                        </div>
                        <h4>لا توجد سجلات حضور لهذا اليوم</h4>
                        <p>يجب تسجيل ساعات الحضور أولاً من صفحة «تسجيل الساعات»</p>
                    </asp:Panel>

                    <!-- Summary -->
                    <div class="ta-summary">
                        <i class="fas fa-chart-bar"></i>
                        <asp:Label ID="lblReviewSummary" runat="server" />
                    </div>

                </div>
            </div>

                       <!-- Actions -->
            <div class="ta-actions">

                <a href="Record.aspx" class="ta-btn-outline">
                    <i class="fas fa-pen"></i> العودة للتسجيل
                </a>

                <%-- ✅ زر إصلاح الأسعار المفقودة --%>
                <asp:Button ID="btnFixRates" runat="server"
                    Text="إصلاح أسعار الحصص تلقائياً"
                    CssClass="ta-btn-outline"
                    OnClick="btnFixRates_Click"
                    OnClientClick="return confirm('سيتم تحديث قيم الأسعار المفقودة من بطاقات المعلمين الحالية. هل تريد المتابعة؟');" />

                <asp:Button ID="btnApprove" runat="server" Text="اعتماد ساعات اليوم مالياً"
                    CssClass="ta-btn-success" OnClick="btnApprove_Click"
                    OnClientClick="return confirm('سيتم اعتماد جميع سجلات الحضور لهذا اليوم مالياً. هل راجعت الحالات والدقائق؟');" />

            </div>

        </div>
    </div>
</asp:Content>