<%@ Page Title="تقرير الطالب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentReport.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Payments.StudentReport" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
       
        .sr-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- PRINT HEADER (hidden on screen) ---------- */
        .print-header,
        .print-footer {
            display: none;
        }

        /* ---------- HERO ---------- */
        .sr-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .sr-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .sr-hero::after {
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
        .sr-hero-content {
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
        .sr-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .sr-hero-icon {
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
        .sr-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .sr-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .sr-stage-label {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            margin-inline-start: 8px;
            padding: 3px 12px;
            border-radius: 999px;
            background: rgba(255,255,255,0.15);
            border: 1px solid rgba(255,255,255,0.3);
            font-size: 12px;
            font-weight: 700;
            color: #fff;
        }
        .sr-stage-label i { font-size: 11px; }
        .sr-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .sr-btn-glass {
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
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .sr-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .sr-btn-glass.primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .sr-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .sr-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .sr-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .sr-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .sr-card-header-icon {
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
        }
        .sr-card-header-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .sr-card-header-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .sr-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .sr-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .sr-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .sr-card-body { padding: 20px 24px; }

        /* ---------- STUDENT INFO TABLE ---------- */
        .sr-info-table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            border: 1px solid #eef1f3;
            border-radius: 14px;
            overflow: hidden;
        }
        .sr-info-table td {
            padding: 14px 18px;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            border-inline-start: 1px solid #f1f3f5;
        }
        .sr-info-table td:first-child {
            border-inline-start: none;
        }
        .sr-info-table tr:last-child td {
            border-bottom: none;
        }
        .sr-info-table .info-label {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            margin-bottom: 5px;
            letter-spacing: 0.2px;
        }
        .sr-info-table .info-label i {
            color: #00b14f;
            font-size: 11px;
        }
        .sr-info-table .info-value {
            font-size: 11px;
            font-weight: 700;
            color: #212529;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        .sr-info-table .info-value.small {
            font-size: 13px;
            font-weight: 700;
        }
        .sr-info-table .info-value.mono {
            
            letter-spacing: 0.5px;
        }
        .sr-info-table .info-cell-accent {
            background: linear-gradient(135deg, rgba(0,177,79,0.05) 0%, rgba(0,177,79,0.02) 100%);
        }

        /* ---------- STAT CARDS (تم التعديل لاستيعاب 5 بطاقات) ---------- */
        .sr-stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 16px;
            margin-bottom: 20px;
        }
        .sr-stat-card {
            display: flex;
            align-items: center;
            gap: 16px;
            padding: 20px 10px;
            border-radius: 18px;
            background: #ffffff;
            border: 1px solid #99f6c3;
            box-shadow: 0 4px 12px rgba(0,0,0,0.03);
            transition: all 0.2s ease;
            position: relative;
            overflow: hidden;
        }
        .sr-stat-card::before {
            content: '';
            position: absolute;
            top: 0;
            inset-inline-start: 0;
            width: 4px;
            height: 100%;
            border-radius: 0 4px 4px 0;
        }
        .sr-stat-card.blue::before   { background: linear-gradient(180deg, #0d6efd 0%, #3d8bfd 100%); }
        .sr-stat-card.green::before  { background: linear-gradient(180deg, #00b14f 0%, #4ed17e 100%); }
        .sr-stat-card.red::before    { background: linear-gradient(180deg, #dc3545 0%, #f0616d 100%); }
        .sr-stat-card.orange::before { background: linear-gradient(180deg, #fd7e14 0%, #ffa94d 100%); }
        .sr-stat-card.slate::before  { background: linear-gradient(180deg, #6c757d 0%, #adb5bd 100%); }
        .sr-stat-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 24px rgba(0,0,0,0.08);
        }
        .sr-stat-icon {
            width: 35px;
            height: 35px;
            border-radius: 16px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            flex-shrink: 0;
            color: #fff;
        }
        .sr-stat-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .sr-stat-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .sr-stat-icon.red    { background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); box-shadow: 0 6px 16px rgba(220,53,69,0.3); }
        .sr-stat-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .sr-stat-icon.slate  { background: linear-gradient(135deg, #6c757d 0%, #adb5bd 100%); box-shadow: 0 6px 16px rgba(108,117,125,0.3); }
        .sr-stat-info { display: flex; flex-direction: column; gap: 4px; min-width: 0; }
        .sr-stat-label {
            font-size: 12px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        .sr-stat-value {
            font-size: 18px;
            font-weight: 700;
            line-height: 1.1;
            
        }
        .sr-stat-value.blue  { color: #0d6efd; }
        .sr-stat-value.green { color: #00b14f; }
        .sr-stat-value.red   { color: #dc3545; }
        .sr-stat-value.orange{ color: #fd7e14; }
        .sr-stat-value.slate { color: #495057; }
        .sr-stat-value small { font-size: 14px; opacity: 0.8; font-family: 'Droid Arabic Kufi', sans-serif; }

        /* ---------- TABLE ---------- */
        .sr-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .sr-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .sr-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33;
            font-size: 11.5px;
            font-weight: 800;
            padding: 14px 10px !important;
            text-align: right;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
        }
        .sr-table tbody td {
            font-size: 13px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .sr-table tbody tr:hover td { background: #f8fdfa; }
        .sr-table tbody tr:last-child td { border-bottom: none; }

        /* Money values */
        .money-value { font-weight: 800;  font-size: 13px; }
        .money-value.green  { color: #00b14f; }
        .money-value.red    { color: #dc3545; }
        .money-value.blue   { color: #0d6efd; }
        .money-value.gray   { color: #495057; }

        /* Receipt badge */
        .receipt-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 10px;
            border-radius: 8px;
            background: rgba(0,177,79,0.08);
            color: #007a33;
            font-size: 12px;
            font-weight: 700;
            border: 1px solid rgba(0,177,79,0.2);
            
        }

        /* Method badge */
        .method-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            background: rgba(0,177,79,0.1);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.2);
            white-space: nowrap;
        }

        /* Row number */
        .row-num {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 28px;
            height: 28px;
            border-radius: 8px;
            background: #f8f9fa;
            color: #6c757d;
            font-weight: 800;
            font-size: 11px;
            border: 1px solid #e9ecef;
        }

        /* Empty state */
        .sr-empty {
            text-align: center;
            padding: 40px 20px;
        }
        .sr-empty-icon {
            width: 68px; height: 68px; border-radius: 20px;
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            color: #00b14f;
            display: inline-flex; align-items: center; justify-content: center;
            font-size: 28px; margin-bottom: 14px;
        }
        .sr-empty h4 { color: #1f3d33; font-weight: 800; font-size: 15px; margin: 0 0 4px; }
        .sr-empty p { color: #6c757d; font-size: 12.5px; margin: 0; }

        /* ---------- MESSAGE ---------- */
        .sr-message {
            display: block;
            padding: 14px 18px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 16px;
            position: relative;
        }
        .sr-message.success {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .sr-message.danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }

        /* ---------- MODAL ---------- */
        .sr-modal .modal-content {
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 20px 60px rgba(0,0,0,0.15);
            overflow: hidden;
        }
        .sr-modal .modal-header {
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
        }
        .sr-modal .modal-title {
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .sr-modal .modal-title-icon {
            width: 36px;
            height: 36px;
            border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 15px;
            box-shadow: 0 4px 12px rgba(0,177,79,0.3);
        }
        .sr-modal .modal-body { padding: 24px; }
        .sr-modal .modal-footer {
            padding: 16px 24px;
            border-top: 1px solid #f1f3f5;
            background: #fbfdfc;
        }
        .sr-modal .form-label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
        }
        .sr-modal .form-control,
        .sr-modal .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 13px;
            font-weight: 600;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px;
            transition: all 0.2s ease;
        }
        .sr-modal .form-control:focus,
        .sr-modal .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }
        .sr-modal .remaining-box {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 14px 18px;
            border-radius: 12px;
            background: linear-gradient(135deg, rgba(220,53,69,0.06) 0%, rgba(220,53,69,0.02) 100%);
            border: 1px solid rgba(220,53,69,0.2);
        }
        .sr-modal .remaining-box .label {
            font-size: 12.5px;
            font-weight: 700;
            color: #6c757d;
        }
        .sr-modal .remaining-box .value {
            font-size: 20px;
            font-weight: 800;
            color: #dc3545;
            
        }

        /* ---------- BUTTONS ---------- */
        .sr-btn-primary {
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
            height: 44px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .sr-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }
        .sr-btn-primary:disabled {
            opacity: 0.5;
            cursor: not-allowed;
            transform: none;
        }
        .sr-btn-outline {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 24px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #dee2e6;
            color: #495057;
            font-size: 13.5px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 44px;
            text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .sr-btn-outline:hover {
            border-color: #6c757d;
            background: #f8f9fa;
            color: #343a40;
            text-decoration: none;
        }

        /* Responsive */
        @media (max-width: 992px) {
            .sr-hero { padding: 26px 20px 72px; }
            .sr-hero-titles h1 { font-size: 19px; }
            .sr-body { padding: 0 16px 40px; }
            .sr-stats-grid { grid-template-columns: 1fr 1fr; }
        }
        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .sr-table { font-size: 11px; min-width: 900px; }
            .sr-info-table td { padding: 10px 12px; }
            .sr-stats-grid { grid-template-columns: 1fr; }
        }
        @media (max-width: 576px) {
            .sr-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .sr-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .sr-hero-titles h1 { font-size: 16px; }
            .sr-card-body { padding: 16px; }
        }

        @media (max-width: 767.98px) {
            .sr-info-table .info-value.student-name {
                white-space: normal;
                overflow: visible;
                text-overflow: clip;
                overflow-wrap: anywhere;
            }
        }

        /* =========================================================
           PRINT — تنسيق احترافي للطباعة (A4 Portrait)
           ========================================================= */
        @page {
            size: A4 portrait;
            margin: 12mm 10mm;
        }

        @media print {

            /* إخفاء عناصر التحكم */
            .sr-hero,
            .sr-hero-actions,
            .no-print,
            .fin-nav,
            .sr-message,
            .sr-btn-glass,
            .sr-btn-primary,
            .sr-btn-outline,
            .modal,
            .modal-backdrop {
                display: none !important;
            }

            /* إخفاء أي أيقونة كبيرة داخل بطاقات الإحصائيات */
            html, body {
                background: #fff !important;
                font-size: 11pt;
            }

            .sr-page {
                background: #fff !important;
                min-height: 0 !important;
            }

            .sr-body {
                max-width: 100% !important;
                margin: 0 !important;
                padding: 0 !important;
            }

            /* ========= Header خاص بالطباعة ========= */
            .print-header {
                display: block !important;
                border-bottom: 2.5px solid #00b14f;
                padding-bottom: 10px;
                margin-bottom: 14px;
                page-break-after: avoid;
            }
            .print-header-top {
                display: flex;
                align-items: center;
                justify-content: space-between;
                gap: 16px;
            }
            .print-header-brand {
                display: flex;
                align-items: center;
                gap: 12px;
            }
            .print-header-logo {
                width: 48px;
                height: 48px;
                border-radius: 12px;
                background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
                color: #fff;
                display: inline-flex;
                align-items: center;
                justify-content: center;
                font-size: 22px;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .print-header-title {
                display: flex;
                flex-direction: column;
                gap: 2px;
            }
            .print-header-title .school-name {
                font-size: 18px;
                font-weight: 800;
                color: #007a33;
            }
            .print-header-title .report-name {
                font-size: 12px;
                font-weight: 700;
                color: #495057;
            }
            .print-header-meta {
                display: flex;
                flex-direction: column;
                align-items: flex-end;
                gap: 3px;
                font-size: 10.5px;
                color: #6c757d;
                font-weight: 700;
            }
            .print-header-meta .meta-item {
                display: flex;
                align-items: center;
                gap: 5px;
            }
            .print-header-meta .meta-item strong {
                color: #212529;
            }

            /* ========= Footer خاص بالطباعة ========= */
            .print-footer {
                display: block !important;
                margin-top: 18px;
                padding-top: 10px;
                border-top: 1.5px solid #d8e6dd;
                font-size: 10px;
                color: #6c757d;
                text-align: center;
                page-break-before: avoid;
            }
            .print-footer .footer-line {
                display: flex;
                justify-content: space-between;
                align-items: center;
                flex-wrap: wrap;
                gap: 8px;
            }
            .print-footer .footer-line .lbl { font-weight: 700; }
            .print-footer .footer-line .val { color: #212529; font-weight: 800; }

            /* ========= البطاقات ========= */
            .sr-card {
                border: 1px solid #d8e6dd !important;
                border-radius: 10px !important;
                box-shadow: none !important;
                margin-bottom: 12px !important;
                page-break-inside: avoid;
                overflow: visible !important;
            }
            .sr-card-header {
                background: #f0f8f3 !important;
                border-bottom: 1.5px solid #d8e6dd !important;
                padding: 10px 14px !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .sr-card-header-icon {
                width: 32px !important;
                height: 32px !important;
                border-radius: 8px !important;
                font-size: 14px !important;
                box-shadow: none !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .sr-card-header-text h3 {
                font-size: 12pt !important;
                color: #007a33 !important;
            }
            .sr-card-header-text p { display: none !important; }
            .sr-card-body { padding: 12px 14px !important; }

            /* ========= جدول بيانات الطالب ========= */
            .sr-info-table {
                border: 1px solid #d8e6dd !important;
                border-radius: 8px !important;
                font-size: 10.5pt;
            }
            .sr-info-table td {
                padding: 8px 12px !important;
                border-color: #eef1f3 !important;
            }
            .sr-info-table .info-label {
                font-size: 9pt !important;
                color: #6c757d !important;
                margin-bottom: 3px !important;
            }
            .sr-info-table .info-value {
                font-size: 11pt !important;
                color: #000 !important;
            }
            .sr-info-table .info-cell-accent {
                background: #f0f8f3 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }

            /* ========= بطاقات الإحصائيات (تعديل لاستيعاب 5 بطاقات) ========= */
            .sr-stats-grid {
                display: grid !important;
                grid-template-columns: repeat(3, 1fr) !important;
                gap: 8px !important;
                margin-bottom: 12px !important;
            }
            .sr-stat-card {
                display: flex !important;
                flex-direction: column !important;
                align-items: center !important;
                border: 1px solid #d8e6dd !important;
                border-radius: 10px !important;
                box-shadow: none !important;
                padding: 10px 8px !important;
                text-align: center !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                background: #f9fdfa !important;
            }
            .sr-stat-card::before { display: none !important; }
            .sr-stat-card:hover { transform: none !important; box-shadow: none !important; }
            .sr-stat-icon {
                width: 32px !important;
                height: 32px !important;
                border-radius: 8px !important;
                font-size: 14px !important;
                box-shadow: none !important;
                margin: 0 auto 6px !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .sr-stat-info {
                align-items: center !important;
                text-align: center !important;
            }
            .sr-stat-label {
                font-size: 8.5pt !important;
                color: #495057 !important;
                white-space: normal !important;
            }
            .sr-stat-value {
                font-size: 12pt !important;
                font-weight: 800 !important;
            }
            .sr-stat-value small {
                font-size: 9pt !important;
            }

            /* ========= الجداول ========= */
            .sr-table-wrap {
                border: 1px solid #d8e6dd !important;
                border-radius: 8px !important;
                overflow: visible !important;
            }
            .sr-table {
                font-size: 10pt !important;
                border-collapse: collapse !important;
                width: 100% !important;
            }
            .sr-table thead th {
                background: #e8f4ec !important;
                color: #007a33 !important;
                font-size: 10pt !important;
                font-weight: 800 !important;
                padding: 8px 6px !important;
                border: 1px solid #d8e6dd !important;
                text-align: center !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .sr-table tbody td {
                font-size: 10pt !important;
                padding: 7px 6px !important;
                border: 1px solid #eef1f3 !important;
                text-align: center !important;
                color: #000 !important;
            }
            .sr-table tbody tr:nth-child(even) td {
                background: #f9fdfa !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .sr-table tbody tr:hover td {
                background: inherit !important;
            }
            .sr-table .money-value {
                font-size: 10pt !important;
                font-family: 'Courier New', monospace !important;
                font-weight: 800 !important;
            }
            .receipt-badge {
                background: none !important;
                border: none !important;
                padding: 0 !important;
                color: #000 !important;
                font-size: 9.5pt !important;
            }
            .receipt-badge i { display: none !important; }
            .method-badge {
                background: none !important;
                border: none !important;
                padding: 0 !important;
                color: #000 !important;
                font-size: 9.5pt !important;
            }
            .row-num {
                background: none !important;
                border: none !important;
                color: #495057 !important;
                font-weight: 800 !important;
                width: auto !important;
                height: auto !important;
                padding: 0 !important;
            }

            /* ========= الحالة الفارغة ========= */
            .sr-empty {
                padding: 16px 12px !important;
            }
            .sr-empty-icon {
                width: 40px !important;
                height: 40px !important;
                border-radius: 10px !important;
                font-size: 18px !important;
                margin-bottom: 6px !important;
            }
            .sr-empty h4 { font-size: 11pt !important; }
            .sr-empty p { font-size: 9.5pt !important; }

            /* ========= فواصل الصفحات ========= */
            .sr-card {
                page-break-inside: avoid;
            }
            .sr-table tr {
                page-break-inside: avoid;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="sr-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="sr-hero">
            <div class="sr-hero-content">
                <div class="sr-hero-left">
                    <div class="sr-hero-icon">
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="sr-hero-titles">
                        <h1>
                            تقرير الطالب المالي
                            <span class="sr-stage-label">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:Literal ID="lblStage" runat="server" />
                            </span>
                        </h1>
                        <p>عرض تفاصيل الرسوم والمدفوعات والمتبقي</p>
                    </div>
                </div>
                <div class="sr-hero-actions no-print">
                    <button type="button" onclick="window.print()" class="sr-btn-glass">
                        <i class="fas fa-print"></i> طباعة
                    </button>
                    <a href='<%= ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx") + "?studentId=" + StudentId + "&stage=" + Server.UrlEncode(CurrentStage) %>'
                       class="sr-btn-glass primary">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="sr-body">

            <!-- رسائل -->
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="sr-message success no-print">
                <i class="fas fa-check-circle me-2"></i><asp:Literal ID="lblMessage" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="sr-message danger no-print">
                <i class="fas fa-exclamation-triangle me-2"></i><asp:Literal ID="lblError" runat="server" />
            </asp:Panel>

            <!-- ═══════════════ PRINT HEADER (يظهر عند الطباعة فقط) ═══════════════ -->
            <div class="print-header">
                <div class="print-header-top">
                    <div class="print-header-brand">
                        <div class="print-header-logo">
                            <i class="fas fa-school"></i>
                        </div>
                        <div class="print-header-title">
                            <span class="school-name">مدرسة نور الملائكة</span>
                            <span class="report-name">تقرير الطالب المالي — <asp:Literal ID="lblPrintStage" runat="server" /></span>
                        </div>
                    </div>
                    <div class="print-header-meta">
                        <div class="meta-item">
                            <i class="fas fa-calendar-alt"></i>
                            <span>تاريخ الطباعة: <strong><asp:Literal ID="lblPrintDate" runat="server" /></strong></span>
                        </div>
                        <div class="meta-item">
                            <i class="fas fa-user-graduate"></i>
                            <span>الطالب: <strong><asp:Literal ID="lblPrintStudent" runat="server" /></strong></span>
                        </div>
                    </div>
                </div>
            </div>
            <!-- ═══════════════════════════════════════════════════════════════════ -->

            <asp:Panel ID="pnlReport" runat="server">

                <!-- بيانات الطالب -->
                <div class="sr-card">
                    <div class="sr-card-header">
                        <div class="sr-card-header-icon blue">
                            <i class="fas fa-user-graduate"></i>
                        </div>
                        <div class="sr-card-header-text">
                            <h3>بيانات الطالب</h3>
                            <p>المعلومات الأساسية للطالب</p>
                        </div>
                    </div>
                    <div class="sr-card-body">

                        <table class="sr-info-table">
                            <tr>
                                <td class="info-cell-accent" style="width: 40%;">
                                    <div class="info-label"><i class="fas fa-user"></i> اسم الطالب</div>
                                    <div class="info-value student-name"><asp:Literal ID="lblStudentName" runat="server" /></div>
                                </td>
                                <td style="width: 30%;">
                                    <div class="info-label"><i class="fas fa-id-card"></i> الرقم الوطني</div>
                                    <div class="info-value mono small"><asp:Literal ID="lblNationalId" runat="server" /></div>
                                </td>
                                <td style="width: 30%;">
                                    <div class="info-label"><i class="fas fa-graduation-cap"></i> المرحلة</div>
                                    <div class="info-value small"><asp:Literal ID="lblGrade" runat="server" /></div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="info-label"><i class="fas fa-layer-group"></i> الصف</div>
                                    <div class="info-value small"><asp:Literal ID="lblClass" runat="server" /></div>
                                </td>
                                <td>
                                    <div class="info-label"><i class="fas fa-stream"></i> القسم</div>
                                    <div class="info-value small"><asp:Literal ID="lblSection" runat="server" /></div>
                                </td>
                                <td>
                                    <div class="info-label"><i class="fas fa-chair"></i> الشعبة</div>
                                    <div class="info-value small"><asp:Literal ID="lblDivision" runat="server" /></div>
                                </td>
                            </tr>
                        </table>

                    </div>
                </div>

                <!-- بطاقات الإحصائيات (5 بطاقات) -->
                <div class="sr-stats-grid">

                    <!-- 1. إجمالي مبلغ الاشتراك (Gross) -->
                    <div class="sr-stat-card slate">
                        <div class="sr-stat-icon slate">
                            <i class="fas fa-file-invoice"></i>
                        </div>
                        <div class="sr-stat-info">
                            <span class="sr-stat-label">اجمالي مبلغ الاشتراك</span>
                            <span class="sr-stat-value slate">
                                <asp:Literal ID="lblTotalGrossDue" runat="server" /> <small>د.ل</small>
                            </span>
                        </div>
                    </div>

                    <!-- 2. مبلغ الخصم -->
                    <div class="sr-stat-card orange">
                        <div class="sr-stat-icon orange">
                            <i class="fas fa-tags"></i>
                        </div>
                        <div class="sr-stat-info">
                            <span class="sr-stat-label">مبلغ الخصم</span>
                            <span class="sr-stat-value orange">
                                <asp:Literal ID="lblTotalDiscount" runat="server" /> <small>د.ل</small>
                            </span>
                        </div>
                    </div>

                    <!-- 3. إجمالي المبلغ المستحق (Net) -->
                    <div class="sr-stat-card blue">
                        <div class="sr-stat-icon blue">
                            <i class="fas fa-file-invoice-dollar"></i>
                        </div>
                        <div class="sr-stat-info">
                            <span class="sr-stat-label">اجمالي المبلغ المستحق</span>
                            <span class="sr-stat-value blue">
                                <asp:Literal ID="lblTotalDue" runat="server" /> <small>د.ل</small>
                            </span>
                        </div>
                    </div>

                    <!-- 4. المبلغ المدفوع -->
                    <div class="sr-stat-card green">
                        <div class="sr-stat-icon green">
                            <i class="fas fa-check-circle"></i>
                        </div>
                        <div class="sr-stat-info">
                            <span class="sr-stat-label">المبلغ المدفوع</span>
                            <span class="sr-stat-value green">
                                <asp:Literal ID="lblTotalPaid" runat="server" /> <small>د.ل</small>
                            </span>
                        </div>
                    </div>

                    <!-- 5. المبلغ المتبقي -->
                    <div class="sr-stat-card red">
                        <div class="sr-stat-icon red">
                            <i class="fas fa-exclamation-circle"></i>
                        </div>
                        <div class="sr-stat-info">
                            <span class="sr-stat-label">المبلغ المتبقي</span>
                            <span class="sr-stat-value red">
                                <asp:Literal ID="lblRemaining" runat="server" /> <small>د.ل</small>
                            </span>
                        </div>
                    </div>

                </div>

                <!-- سجل مدفوعات الاشتراك -->
                <div class="sr-card">
                    <div class="sr-card-header">
                        <div class="sr-card-header-icon">
                            <i class="fas fa-money-bill-wave"></i>
                        </div>
                        <div class="sr-card-header-text">
                            <h3>سجل المدفوعات</h3>
                            <p>جميع دفعات الاشتراك السنوي</p>
                        </div>
                    </div>
                    <div class="sr-card-body">

                        <asp:Panel ID="pnlSubTable" runat="server">
                            <div class="table-responsive sr-table-wrap">
                                <table class="sr-table">
                                    <thead>
                                        <tr>
                                            <th style="width:50px;text-align:center;">#</th>
                                            <th>رقم الإيصال</th>
                                            <th>نوع القسط</th>
                                            <th>التاريخ</th>
                                            <th style="text-align:center;">المبلغ الإجمالي</th>
                                            <th style="text-align:center;">المبلغ المخصوم</th>
                                            <th style="text-align:center;">المبلغ المستحق</th>
                                            <th style="text-align:center;">المبلغ المدفوع</th>
                                            <th style="text-align:center;">المبلغ المتبقي</th>
                                            <th>طريقة الدفع</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptSubPayments" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td style="text-align:center;">
                                                        <span class="row-num"><%# Container.ItemIndex + 1 %></span>
                                                    </td>
                                                    <td>
                                                        <span class="receipt-badge">
                                                            <i class="fas fa-receipt" style="font-size:10px;"></i>
                                                            <%# Eval("ReceiptNumber") %>
                                                        </span>
                                                    </td>
                                                    <td style="font-weight:700;"><%# Eval("FeeName") %></td>
                                                    <td>
                                                        <span style="color:#6c757d;font-size:12.5px;">
                                                            <i class="fas fa-calendar-alt" style="color:#00b14f;font-size:11px;margin-inline-end:4px;"></i>
                                                            <%# Eval("PaymentDate", "{0:yyyy/MM/dd}") %>
                                                        </span>
                                                    </td>
                                                    <td style="text-align:center;">
                                                        <span class="money-value gray"><%# Eval("GrossAmountText") %> د.ل</span>
                                                    </td>
                                                    <td style="text-align:center;">
                                                        <span class="money-value red"><%# Eval("DiscountText") %> د.ل</span>
                                                    </td>
                                                    <td style="text-align:center;">
                                                        <span class="money-value blue"><%# Eval("DueText") %> د.ل</span>
                                                    </td>
                                                    <td style="text-align:center;">
                                                        <span class="money-value green"><%# Eval("AmountText") %> د.ل</span>
                                                    </td>
                                                    <td style="text-align:center;">
                                                        <%# RenderRemaining(Eval("Remaining")) %>
                                                    </td>
                                                    <td>
                                                        <span class="method-badge"><%# Eval("MethodName") %></span>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlSubEmpty" runat="server">
                            <div class="sr-empty">
                                <div class="sr-empty-icon"><i class="fas fa-inbox"></i></div>
                                <h4>لا توجد مدفوعات اشتراك بعد</h4>
                                <p>لم يتم تسجيل أي دفعة اشتراك سنوي لهذا الطالب</p>
                            </div>
                        </asp:Panel>

                    </div>
                </div>

                <!-- سجل المدفوعات - الأنواع الأخرى -->
                <div class="sr-card">
                    <div class="sr-card-header">
                        <div class="sr-card-header-icon purple">
                            <i class="fas fa-list-alt"></i>
                        </div>
                        <div class="sr-card-header-text">
                            <h3>سجل المدفوعات - الأنواع الأخرى</h3>
                            <p>الرسوم غير المرتبطة بالاشتراك السنوي</p>
                        </div>
                    </div>
                    <div class="sr-card-body">

                        <asp:Panel ID="pnlOtherTable" runat="server">
                            <div class="table-responsive sr-table-wrap">
                                <table class="sr-table">
                                    <thead>
                                        <tr>
                                            <th style="width:50px;text-align:center;">#</th>
                                            <th>نوع الرسم</th>
                                            <th style="text-align:center;">المبلغ المستحق</th>
                                            <th style="text-align:center;">المبلغ المدفوع</th>
                                            <th style="text-align:center;">المبلغ المتبقي</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptOtherFees" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td style="text-align:center;">
                                                        <span class="row-num"><%# Container.ItemIndex + 1 %></span>
                                                    </td>
                                                    <td style="font-weight:700;"><%# Eval("FeeName") %></td>
                                                    <td style="text-align:center;">
                                                        <span class="money-value gray"><%# Eval("AmountText") %> د.ل</span>
                                                    </td>
                                                    <td style="text-align:center;">
                                                        <%# RenderPaid(Eval("Paid")) %>
                                                    </td>
                                                    <td style="text-align:center;">
                                                        <%# RenderRemaining(Eval("Remaining")) %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlOtherEmpty" runat="server">
                            <div class="sr-empty">
                                <div class="sr-empty-icon"><i class="fas fa-inbox"></i></div>
                                <h4>لا توجد رسوم أخرى</h4>
                                <p>لا توجد رسوم إضافية غير الاشتراك السنوي</p>
                            </div>
                        </asp:Panel>

                    </div>
                </div>

            </asp:Panel>

            <!-- ═══════════════ PRINT FOOTER (يظهر عند الطباعة فقط) ═══════════════ -->
            <div class="print-footer">
                <div class="footer-line">
                    <span>
                        <span class="lbl">النظام:</span>
                        <span class="val">نظام إدارة مدرسة نور الملائكة</span>
                    </span>
                    <span>
                        <span class="lbl">تم الإنشاء بواسطة:</span>
                        <span class="val"><asp:Literal ID="lblPrintUser" runat="server" /></span>
                    </span>
                    <span>
                        <span class="lbl">التاريخ:</span>
                        <span class="val"><asp:Literal ID="lblPrintFooterDate" runat="server" /></span>
                    </span>
                </div>
            </div>
            <!-- ═══════════════════════════════════════════════════════════════════ -->

            <!-- لا يوجد طالب -->
            <asp:Panel ID="pnlNoStudent" runat="server" Visible="false">
                <div class="sr-card">
                    <div class="sr-card-body">
                        <div class="sr-empty">
                            <div class="sr-empty-icon"><i class="fas fa-user-slash"></i></div>
                            <h4>لم يتم اختيار طالب</h4>
                            <p>يرجى اختيار طالب للانتقال إلى تقرير المدفوعات</p>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>

    <!-- ============ MODAL: إضافة دفعة ============ -->
    <div class="modal fade sr-modal" id="paymentModal" tabindex="-1" aria-hidden="true" runat="server" ClientIDMode="Static">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title">
                        <span class="modal-title-icon"><i class="fas fa-credit-card"></i></span>
                        إضافة دفعة
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">
                    <asp:HiddenField ID="hdnFeeId" runat="server" />
                    <asp:HiddenField ID="hdnMaxAmount" runat="server" />

                    <div class="mb-3">
                        <label class="form-label" id="modalFeeLabel"></label>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">المبلغ المتبقي</label>
                        <div class="remaining-box">
                            <span class="label"><i class="fas fa-hourglass-half" style="color:#dc3545;"></i> المتبقي</span>
                            <div>
                                <span class="value" id="modalRemaining">0.00</span>
                                <span style="font-size:13px;color:#dc3545;"> د.ل</span>
                            </div>
                        </div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">دفع المبلغ المتبقي <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtPaymentAmount" runat="server" CssClass="form-control"
                            TextMode="Number" step="0.01" min="0.01" placeholder="0.00" />
                        <small class="text-muted d-block mt-1" style="font-size:11px;">
                            لا يمكن أن يتجاوز المبلغ المتبقي
                        </small>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">طريقة الدفع <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-select">
                            <asp:ListItem Value="Cash" Text="نقدي" />
                            <asp:ListItem Value="BankTransfer" Text="تحويل بنكي" />
                            <asp:ListItem Value="Check" Text="شيك" />
                            <asp:ListItem Value="Electronic" Text="بطاقة مصرفية" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="sr-btn-outline" data-bs-dismiss="modal">
                        <i class="fas fa-times"></i> إلغاء
                    </button>
                    <asp:Button ID="btnConfirmPayment" runat="server" CssClass="sr-btn-primary"
                        Text="تأكيد الدفع" OnClick="btnConfirmPayment_Click" ClientIDMode="Static" />
                </div>

            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        function openPaymentModal(feeId, remaining, feeName) {
            document.getElementById('<%= hdnFeeId.ClientID %>').value = feeId;
            document.getElementById('<%= hdnMaxAmount.ClientID %>').value = remaining;
            document.getElementById('modalFeeLabel').textContent = 'نوع الرسم: ' + feeName;
            document.getElementById('modalRemaining').textContent = Number(remaining).toFixed(2);
            var amountInput = document.getElementById('<%= txtPaymentAmount.ClientID %>');
            amountInput.value = '';
            var btn = document.getElementById('btnConfirmPayment');
            btn.disabled = false;
            var modal = new bootstrap.Modal(document.getElementById('paymentModal'));
            modal.show();
        }

        function checkAmount() {
            var max = parseFloat(document.getElementById('<%= hdnMaxAmount.ClientID %>').value);
            var val = parseFloat(document.getElementById('<%= txtPaymentAmount.ClientID %>').value);
            var btn = document.getElementById('btnConfirmPayment');
            if (!isNaN(val) && val > max) {
                btn.disabled = true;
            } else {
                btn.disabled = false;
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            var input = document.getElementById('<%= txtPaymentAmount.ClientID %>');
            if (input) input.addEventListener('input', checkAmount);
        });
    </script>
</asp:Content>
