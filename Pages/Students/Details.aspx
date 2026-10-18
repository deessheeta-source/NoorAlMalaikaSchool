<%@ Page Title="تفاصيل الطالب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Students.Details" %>
<%@ Import Namespace="NoorAlMalaikaSchool.Data.Helpers" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Student Details
           ========================================================= */
        .details-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .details-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .details-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .details-hero::after {
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
        .details-hero-content {
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
        .details-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .details-hero-icon {
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
        .details-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .details-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .details-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .details-btn-glass {
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
        .details-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .details-btn-glass-primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .details-btn-glass-primary:hover {
            background: #f8f9fa;
            color: #007a33;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .details-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .details-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .details-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .details-card-header-icon {
            width: 40px;
            height: 40px;
            border-radius: 12px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
        }
        .details-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .details-card-header-icon.pink {
            background: linear-gradient(135deg, #d63384 0%, #e685b5 100%);
            box-shadow: 0 6px 16px rgba(214,51,132,0.3);
        }
        .details-card-header-icon.orange {
            background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%);
            box-shadow: 0 6px 16px rgba(253,126,20,0.3);
        }
        .details-card-header-text h3 {
            margin: 0;
            font-size: 15px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .details-card-header-text p {
            margin: 3px 0 0;
            font-size: 12px;
            color: #6c757d;
            font-weight: 500;
        }

        .details-card-body {
            padding: 20px 24px;
        }

        /* ---------- STUDENT HERO CARD ---------- */
        .student-hero-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .student-hero-body {
            display: flex;
            align-items: center;
            gap: 24px;
            padding: 24px 28px;
            flex-wrap: wrap;
        }
        .student-hero-avatar {
            width: 100px;
            height: 120px;
            border-radius: 16px;
            object-fit: cover;
            border: 4px solid #fff;
            box-shadow: 0 8px 20px rgba(0,0,0,0.12);
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            flex-shrink: 0;
        }
        .student-hero-info {
            flex: 1;
            min-width: 250px;
        }
        .student-hero-name {
            font-size: 20px;
            font-weight: 800;
            color: #212529;
            margin: 0 0 10px;
            line-height: 1.3;
        }
        .student-hero-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            align-items: center;
        }
        .student-hero-meta-item {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            background: #f8f9fa;
            color: #495057;
            border: 1px solid #e9ecef;
        }
        .student-hero-meta-item i {
            color: #00b14f;
            font-size: 11px;
        }

        /* Status badges */
        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 6px 14px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
        }
        .status-badge.active {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .status-badge.inactive {
            background: rgba(245,158,11,0.14);
            color: #d97706;
            border: 1px solid rgba(245,158,11,0.3);
        }

        /* ---------- INFO ROWS ---------- */
        .info-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 16px 24px;
        }

        /* ---------- Parent Info Row (5 أعمدة في صف واحد) ---------- */
        .info-row-parent {
            display: grid;
            grid-template-columns: 1.2fr 1.2fr 1fr 0.8fr 2fr;
            gap: 16px 20px;
            align-items: start;
        }

        .info-item {
            display: flex;
            flex-direction: column;
            gap: 4px;
            min-width: 0;
        }
        .info-item-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #868e96;
            display: flex;
            align-items: center;
            gap: 6px;
            letter-spacing: 0.2px;
        }
        .info-item-label i {
            font-size: 10px;
            color: #00b14f;
        }
        .info-item-value {
            font-size: 14px;
            font-weight: 700;
            color: #212529;
            line-height: 1.5;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }
        .info-item-value.empty {
            color: #ced4da;
            font-weight: 500;
            font-style: italic;
        }

        /* Section badge */
        .section-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            font-size: 12.5px;
            font-weight: 700;
            border-radius: 999px;
        }
        /*.section-badge.sci {
            background: rgba(13,110,253,0.1);
            color: #0d6efd;
            border: 1px solid rgba(13,110,253,0.2);
        }*/
      /*  .section-badge.lit {
            background: rgba(111,66,193,0.1);
            color: #5a2fa0;
            border: 1px solid rgba(111,66,193,0.2);
        }*/

        /* Notes box */
        .notes-box {
            padding: 16px 20px;
            background: #fbfdfc;
            border-right: 4px solid #00b14f;
            border-radius: 10px;
            color: #495057;
            font-size: 13.5px;
            line-height: 1.8;
            white-space: pre-wrap;
        }

        /* ---------- NOT FOUND ---------- */
        .details-notfound {
            text-align: center;
            padding: 60px 20px;
        }
        .details-notfound-icon {
            width: 80px;
            height: 80px;
            border-radius: 22px;
            background: linear-gradient(135deg, #fff4e5 0%, #ffe6c7 100%);
            color: #fd7e14;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            margin-bottom: 18px;
        }
        .details-notfound h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .details-notfound p {
            color: #6c757d;
            font-size: 13px;
            margin: 0 0 18px;
        }

        /* Button */
        .details-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 12px 28px;
            border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            color: #fff;
            font-size: 14px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
            height: 46px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .details-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }

        /* Placeholder for missing student photo */
        .student-hero-avatar-placeholder {
            width: 100px;
            height: 120px;
            border-radius: 16px;
            border: 4px solid #fff;
            box-shadow: 0 8px 20px rgba(0,0,0,0.12);
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            display: inline-flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 6px;
            color: #00b14f;
            flex-shrink: 0;
        }
        .student-hero-avatar-placeholder i {
            font-size: 34px;
            opacity: 0.7;
        }
        .student-hero-avatar-placeholder span {
            font-size: 10px;
            font-weight: 700;
            color: #4b7a5f;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .details-hero { padding: 26px 20px 72px; }
            .details-hero-titles h1 { font-size: 19px; }
            .details-body { padding: 0 16px 40px; }

            .info-row-parent {
                grid-template-columns: repeat(2, 1fr);
            }
        }
        @media (max-width: 768px) {
            .student-hero-body {
                flex-direction: column;
                align-items: center;
                text-align: center;
            }
            .student-hero-meta { justify-content: center; }
            .info-row { grid-template-columns: 1fr; }
        }
        @media (max-width: 576px) {
            .details-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .details-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .details-hero-titles h1 { font-size: 16px; }
            .details-hero-titles p { font-size: 12px; }
            .details-btn-glass { padding: 8px 14px; font-size: 12px; }
            .details-card-body { padding: 16px; }
            .student-hero-body { padding: 20px; }

            .info-row-parent {
                grid-template-columns: 1fr;
            }
        }

        /* =========================================================
           PRINT HEADER / FOOTER (hidden on screen)
           ========================================================= */
        .print-header,
        .print-footer {
            display: none;
        }

        /* =========================================================
           PRINT STYLES — احترافي (صفحة واحدة)
           ========================================================= */
        @media print {
            /* إعدادات الصفحة */
            @page {
                size: A4 portrait;
                margin: 8mm 8mm 8mm 8mm;
            }

            /* إعادة تعيين عام */
            html, body {
                background: #fff !important;
                color: #000 !important;
                font-size: 9pt !important;
                font-family: 'Droid Arabic Kufi', sans-serif !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }

            /* إخفاء عناصر التحكم + قسم Hero */
            .details-hero-actions,
            .details-hero,
            .no-print,
            .sidebar,
            .sidebar-overlay,
            .top-header,
            .footer,
            #sidebarToggle,
            .pagination {
                display: none !important;
            }

            .main-content,
            .content-wrapper {
                margin: 0 !important;
                padding: 0 !important;
            }
            .details-page {
                background: #fff !important;
                padding: 0 !important;
                min-height: auto !important;
            }

            /* رأس الطباعة - مضغوط */
            .print-header {
                display: block !important;
                text-align: center;
                padding-bottom: 2mm;
                margin-bottom: 3mm;
                border-bottom: 1pt solid #00b14f;
            }
            .print-header h1 {
                margin: 0 0 1mm;
                font-size: 14pt;
                color: #00b14f;
                font-weight: 800;
                letter-spacing: -0.3pt;
            }
            .print-header p {
                margin: 0;
                font-size: 8.5pt;
                color: #555;
            }

            /* Body */
            .details-body {
                margin: 0 !important;
                padding: 0 !important;
                max-width: 100% !important;
            }

            /* البطاقات - مضغوطة */
            .details-card,
            .student-hero-card {
                background: #fff !important;
                box-shadow: none !important;
                border: 0.5pt solid #bbb !important;
                border-radius: 0 !important;
                margin-bottom: 2.5mm !important;
                page-break-inside: avoid;
                overflow: visible !important;
            }

            /* رأس البطاقة - مضغوط */
            .details-card-header {
                background: #f5f5f5 !important;
                background-image: none !important;
                border-bottom: 0.5pt solid #bbb !important;
                padding: 1.5mm 3mm !important;
                gap: 2mm !important;
            }

            .details-card-header-icon {
                background: #00b14f !important;
                background-image: none !important;
                box-shadow: none !important;
                width: 6mm !important;
                height: 6mm !important;
                border-radius: 1mm !important;
                font-size: 3mm !important;
            }
            .details-card-header-icon.blue { background: #0d6efd !important; }
            .details-card-header-icon.pink { background: #d63384 !important; }
            .details-card-header-icon.orange { background: #fd7e14 !important; }

            .details-card-header-text h3 {
                color: #000 !important;
                font-size: 9.5pt !important;
                font-weight: 800 !important;
                margin: 0 !important;
            }
            /* إخفاء العناوين الفرعية لتوفير مساحة */
            .details-card-header-text p {
                display: none !important;
            }

            .details-card-body {
                padding: 2mm 3mm !important;
            }

            /* بطاقة الطالب الرئيسية - مضغوطة */
            .student-hero-body {
                padding: 2mm 3mm !important;
                gap: 3mm !important;
                flex-wrap: nowrap !important;
            }

            .student-hero-avatar,
            .student-hero-avatar-placeholder {
                width: 20mm !important;
                height: 24mm !important;
                border: 0.5pt solid #999 !important;
                border-radius: 1.5mm !important;
                box-shadow: none !important;
                page-break-inside: avoid;
            }
            .student-hero-avatar-placeholder i { font-size: 6mm !important; }
            .student-hero-avatar-placeholder span { font-size: 6pt !important; }

            .student-hero-info { min-width: 0 !important; }

            .student-hero-name {
                font-size: 12pt !important;
                color: #000 !important;
                margin: 0 0 1.5mm !important;
            }

            .student-hero-meta { gap: 1.5mm !important; }

            .student-hero-meta-item {
                background: #fff !important;
                border: 0.5pt solid #999 !important;
                padding: 0.5mm 2mm !important;
                font-size: 8pt !important;
                color: #000 !important;
                border-radius: 1mm !important;
            }
            .student-hero-meta-item i {
                color: #00b14f !important;
                font-size: 7pt !important;
            }

            .status-badge {
                padding: 0.5mm 2mm !important;
                font-size: 8pt !important;
                border: 0.5pt solid currentColor !important;
                border-radius: 1mm !important;
            }

            /* ✅ الشبكات — 4 أعمدة لضغط الصفوف */
            .info-row {
                gap: 2mm 4mm !important;
                grid-template-columns: repeat(4, 1fr) !important;
            }

            .info-row-parent {
                gap: 2mm 3mm !important;
                grid-template-columns: 1fr 1fr 1fr 0.7fr 1.3fr !important;
            }

            /* ✅ إلغاء امتداد full-width (العنوان) → يندمج مع الشبكة */
            .info-row > .info-item[style*="grid-column"] {
                grid-column: auto !important;
            }

            .info-item { gap: 0.3mm !important; }

            .info-item-label {
                font-size: 7.5pt !important;
                color: #666 !important;
            }
            .info-item-label i {
                color: #00b14f !important;
                font-size: 6.5pt !important;
            }

            .info-item-value {
                font-size: 9pt !important;
                color: #000 !important;
                font-weight: 700 !important;
                line-height: 1.2 !important;
            }

            /* شارات القسم */
            .section-badge {
                border: 0.5pt solid currentColor !important;
                padding: 0.3mm 1.5mm !important;
                font-size: 8pt !important;
                border-radius: 1mm !important;
            }
            .section-badge.sci { background: #eaf3ff !important; color: #0d6efd !important; }
            .section-badge.lit { background: #f3edff !important; color: #5a2fa0 !important; }

            /* الملاحظات */
            .notes-box {
                background: #f9f9f9 !important;
                border-right: 2pt solid #00b14f !important;
                padding: 1.5mm 3mm !important;
                font-size: 8.5pt !important;
                border-radius: 0 !important;
                line-height: 1.4 !important;
            }

            /* الحقول الفارغة */
            .info-item-value.empty {
                color: #999 !important;
                font-style: italic !important;
                font-weight: 500 !important;
            }

            /* ✅ تذييل الطباعة - مضغوط */
            .print-footer {
                display: block !important;
                text-align: center;
                padding-top: 3mm;
                margin-top: 4mm;
                border-top: 0.5pt solid #bbb;
                font-size: 7.5pt;
                color: #777;
                page-break-inside: avoid;
            }

            .print-footer .signature-row {
                display: flex;
                justify-content: space-between;
                margin-top: 5mm;
                padding: 0 5mm;
            }

            .print-footer .signature-box {
                text-align: center;
                font-size: 8pt;
                color: #333;
            }

            .print-footer .signature-line {
                border-top: 0.5pt solid #333;
                width: 35mm;
                margin-top: 8mm;
                padding-top: 0.5mm;
            }

            /* منع انقسام العناصر */
            .details-card,
            .student-hero-card,
            .info-item {
                page-break-inside: avoid;
            }

            /* إزالة الخلفيات */
            * {
                background-image: none !important;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="details-page" dir="rtl">

        <!-- ================== PRINT HEADER (يظهر في الطباعة فقط) ================== -->
        <div class="print-header">
            <h1>مدرسة نور الملائكة</h1>
            <p>بطاقة بيانات الطالب — العام الدراسي 2026/2027</p>
        </div>

        <!-- ================== HERO (يختفي في الطباعة) ================== -->
        <div class="details-hero">
            <div class="details-hero-content">
                <div class="details-hero-left">
                    <div class="details-hero-icon">
                        <i class="fas fa-id-card"></i>
                    </div>
                    <div class="details-hero-titles">
                        <h1>تفاصيل الطالب</h1>
                        <p>عرض شامل لبيانات الطالب وولي الأمر</p>
                    </div>
                </div>
                <div class="details-hero-actions">
                    <asp:Button ID="btnPrint" runat="server" Text="طباعة"
                        CssClass="details-btn-glass"
                        OnClientClick="window.print(); return false;"
                        CausesValidation="false" />
                    <asp:Button ID="btnBackToListTop" runat="server" Text="العودة للقائمة"
                        CssClass="details-btn-glass details-btn-glass-primary"
                        OnClick="btnBackToList_Click"
                        CausesValidation="false" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="details-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="alert d-none" />
            <asp:HiddenField ID="hfStudentId" runat="server" />

            <!-- Not Found -->
            <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
                <div class="details-card">
                    <div class="details-card-body">
                        <div class="details-notfound">
                            <div class="details-notfound-icon">
                                <i class="fas fa-exclamation-triangle"></i>
                            </div>
                            <h4>الطالب غير موجود</h4>
                            <p>ربما تم حذفه أو أن الرابط غير صالح</p>
                            <asp:Button ID="btnBackToList" runat="server" Text="العودة للقائمة"
                                CssClass="details-btn-primary"
                                OnClick="btnBackToList_Click"
                                CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Details -->
            <asp:Panel ID="pnlDetails" runat="server" Visible="true">

                <!-- 1) بطاقة الطالب الرئيسية -->
                <div class="student-hero-card">
                    <div class="student-hero-body">
                        <asp:Panel ID="pnlStudentPhoto" runat="server" Visible="false">
                            <asp:Image ID="imgStudent" runat="server" CssClass="student-hero-avatar"
                                AlternateText="صورة الطالب" />
                        </asp:Panel>

                        <asp:Panel ID="pnlStudentPhotoPlaceholder" runat="server" Visible="true" CssClass="student-hero-avatar-placeholder">
                            <i class="fas fa-user-graduate"></i>
                            <span>لا توجد صورة</span>
                        </asp:Panel>

                        <div class="student-hero-info">
                            <h2 class="student-hero-name">
                                <asp:Label ID="lblFullName" runat="server" />
                            </h2>

                            <div class="student-hero-meta">
                                <asp:Label ID="lblStatus" runat="server" CssClass="status-badge status-active" />
                                <span class="student-hero-meta-item">
                                    <i class="fas fa-graduation-cap"></i>
                                    <asp:Label ID="lblHeroGradeLevel" runat="server" />
                                </span>
                                <span class="student-hero-meta-item">
                                    <i class="fas fa-hashtag"></i>
                                    <asp:Label ID="lblHeroRegistrationNo" runat="server" />
                                </span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 2) بيانات الطالب -->
                <div class="details-card">
                    <div class="details-card-header">
                        <div class="details-card-header-icon">
                            <i class="fas fa-user-graduate"></i>
                        </div>
                        <div class="details-card-header-text">
                            <h3>بيانات الطالب</h3>
                            <p>المعلومات الأساسية والدراسية</p>
                        </div>
                    </div>
                    <div class="details-card-body">
                        <div class="info-row">

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-id-card"></i> الرقم الوطني</div>
                                <div class="info-item-value"><asp:Label ID="lblNationalId2" runat="server" /></div>
                            </div>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-hashtag"></i> رقم التسجيل</div>
                                <div class="info-item-value"><asp:Label ID="lblRegistrationNo2" runat="server" /></div>
                            </div>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-birthday-cake"></i> تاريخ الميلاد</div>
                                <div class="info-item-value"><asp:Label ID="lblBirthDate" runat="server" /></div>
                            </div>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-venus-mars"></i> الجنس</div>
                                <div class="info-item-value"><asp:Label ID="lblGender" runat="server" /></div>
                            </div>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-globe"></i> الجنسية</div>
                                <div class="info-item-value"><asp:Label ID="lblNationality" runat="server" /></div>
                            </div>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-graduation-cap"></i> الصف</div>
                                <div class="info-item-value"><asp:Label ID="lblGradeLevel" runat="server" /></div>
                            </div>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-layer-group"></i> القسم</div>
                                <div class="info-item-value"><asp:Label ID="lblSection" runat="server" /></div>
                            </div>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-chair"></i> الشعبة</div>
                                <div class="info-item-value"><asp:Label ID="lblSectionRoom" runat="server" /></div>
                            </div>

                           <%-- <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-school"></i> المرحلة</div>
                                <div class="info-item-value"><asp:Label ID="lblStage" runat="server" /></div>
                            </div>--%>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-phone"></i> الهاتف</div>
                                <div class="info-item-value"><asp:Label ID="lblPhone" runat="server" /></div>
                            </div>

                            <div class="info-item" style="grid-column: 1 / -1;">
                                <div class="info-item-label"><i class="fas fa-map-marker-alt"></i> العنوان</div>
                                <div class="info-item-value"><asp:Label ID="lblAddress" runat="server" /></div>
                            </div>

                        </div>
                    </div>
                </div>

                <!-- 3) بيانات ولي الأمر -->
                <asp:Panel ID="pnlParent" runat="server" Visible="true">
                    <div class="details-card">
                        <div class="details-card-header">
                            <div class="details-card-header-icon blue">
                                <i class="fas fa-user-shield"></i>
                            </div>
                            <div class="details-card-header-text">
                                <h3>بيانات ولي الأمر</h3>
                                <p>المعلومات الخاصة بولي الأمر المسجل</p>
                            </div>
                        </div>
                        <div class="details-card-body">
                            <div class="info-row-parent">

                               <div class="info-item">
                                    <div class="info-item-label">
                                        <i class="fas fa-user"></i> اسم ولي الأمر
                                    </div>
                                    <div class="info-item-value">
                                        <asp:Label ID="lblParentName" runat="server" />
                                    </div>
                                </div>
                                 <div class="info-item">
                                     <div class="info-item-label">
                                         <i class="fas fa-id-card"></i> الرقم الوطني
                                     </div>
                                     <div class="info-item-value">
                                         <asp:Label ID="lblParentNationalId" runat="server" />
                                     </div>
                                 </div>

                                <div class="info-item">
                                    <div class="info-item-label">
                                        <i class="fas fa-phone"></i> هاتف ولي الأمر
                                    </div>
                                    <div class="info-item-value">
                                        <asp:Label ID="lblParentPhone" runat="server" />
                                    </div>
                                </div>

                                <div class="info-item">
                                    <div class="info-item-label">
                                        <i class="fas fa-link"></i> صلة القرابة
                                    </div>
                                    <div class="info-item-value">
                                        <asp:Label ID="lblParentRelation" runat="server" />
                                    </div>
                                </div>

                                <div class="info-item">
                                    <div class="info-item-label">
                                        <i class="fas fa-map-marker-alt"></i> عنوان ولي الأمر
                                    </div>
                                    <div class="info-item-value">
                                        <asp:Label ID="lblParentAddress" runat="server" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- 4) بيانات الأم -->
                <div class="details-card">
                    <div class="details-card-header">
                        <div class="details-card-header-icon pink">
                            <i class="fas fa-female"></i>
                        </div>
                        <div class="details-card-header-text">
                            <h3>بيانات الأم</h3>
                            <p>معلومات الاتصال بالأم</p>
                        </div>
                    </div>
                    <div class="details-card-body">
                        <div class="info-row">

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-user"></i> اسم الأم</div>
                                <div class="info-item-value"><asp:Label ID="lblMotherName" runat="server" /></div>
                            </div>

                            <div class="info-item">
                                <div class="info-item-label"><i class="fas fa-phone"></i> هاتف الأم</div>
                                <div class="info-item-value"><asp:Label ID="lblMotherPhone" runat="server" /></div>
                            </div>

                        </div>
                    </div>
                </div>

                <!-- 5) ملاحظات -->
                <asp:Panel ID="pnlNotes" runat="server" Visible="false">
                    <div class="details-card">
                        <div class="details-card-header">
                            <div class="details-card-header-icon orange">
                                <i class="fas fa-sticky-note"></i>
                            </div>
                            <div class="details-card-header-text">
                                <h3>ملاحظات</h3>
                                <p>ملاحظات إضافية مسجلة على الطالب</p>
                            </div>
                        </div>
                        <div class="details-card-body">
                            <div class="notes-box"><asp:Label ID="lblNotes" runat="server" /></div>
                        </div>
                    </div>
                </asp:Panel>

            </asp:Panel>
        </div>

        <!-- ================== PRINT FOOTER (يظهر في الطباعة فقط) ================== -->
        <div class="print-footer">
            <div class="signature-row">
                <div class="signature-box">
                    <div class="signature-line">توقيع ولي الأمر</div>
                </div>
                <div class="signature-box">
                    <div class="signature-line">توقيع الموظف المسؤول</div>
                </div>
                <div class="signature-box">
                    <div class="signature-line">ختم المدرسة</div>
                </div>
            </div>
            <div style="margin-top:6mm;">
                تم إنشاء هذا التقرير تلقائياً من نظام مدرسة نور الملائكة
                &nbsp;•&nbsp;
                تاريخ الطباعة: <%= DateTime.Now.ToString("yyyy-MM-dd HH:mm") %>
            </div>
        </div>

    </div>
</asp:Content>