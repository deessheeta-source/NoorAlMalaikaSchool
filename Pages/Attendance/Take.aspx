<%@ Page Title="تسجيل الحضور" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Take.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Attendance.Take" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Take Attendance
           ========================================================= */
        .take-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .take-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .take-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .take-hero::after {
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
        .take-hero-content {
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
        .take-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .take-hero-icon {
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
        .take-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .take-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .take-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .take-btn-glass {
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
        .take-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .take-btn-glass-primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .take-btn-glass-primary:hover {
            background: #f8f9fa;
            color: #007a33;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .take-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .take-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .take-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .take-card-header-icon {
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
        .take-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .take-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .take-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 200px;
        }
        .take-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .take-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .take-count-pill {
            margin-right: auto;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
            background: rgba(0,177,79,0.1);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
            flex-shrink: 0;
        }

        .take-card-body {
            padding: 20px 24px;
        }

        /* ---------- FILTERS ROW ---------- */
        .take-filters {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            align-items: end;
            margin-bottom: 20px;
        }

        .take-filters .take-field {
            flex: 1 1 180px;
            min-width: 0;
        }

        .take-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .take-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .take-field label i {
            color: #00b14f;
            font-size: 12px;
        }
        .take-field .form-control,
        .take-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 12.5px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px;
            width: 100%;
        }
        .take-field .form-control:focus,
        .take-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- BUTTONS ---------- */
        .take-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 10px 22px;
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
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .take-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }

        .take-btn-outline {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 10px 18px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #dee2e6;
            color: #495057;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 42px;
            white-space: nowrap;
            text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .take-btn-outline:hover {
            border-color: #00b14f;
            color: #00b14f;
            background: rgba(0,177,79,0.04);
            text-decoration: none;
        }

        /* Save button — larger */
        .take-btn-save {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            padding: 12px 32px;
            border-radius: 12px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            color: #fff;
            font-size: 14px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 8px 20px rgba(0,177,79,0.35);
            height: 48px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .take-btn-save:hover {
            transform: translateY(-2px);
            box-shadow: 0 12px 28px rgba(0,177,79,0.45);
            color: #fff;
        }

        /* ---------- TABLE ---------- */
        .take-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .take-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .take-table thead th {
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
        .take-table tbody td {
            font-size: 13px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .take-table tbody tr {
            transition: all 0.15s ease;
        }
        .take-table tbody tr:hover td {
            background: #f8fdfa;
        }
        .take-table tbody tr:last-child td {
            border-bottom: none;
        }

        /* Student row cell */
        .student-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 700;
            color: #212529;
        }
        .student-avatar {
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

        /* Row number badge */
        .row-num {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 30px;
            height: 30px;
            border-radius: 9px;
            background: #f8f9fa;
            color: #6c757d;
            font-weight: 800;
            font-size: 12px;
            border: 1px solid #e9ecef;
        }

        /* Checkbox — Premium */
        .take-checkbox {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            position: relative;
        }
        .take-checkbox input[type="checkbox"] {
            appearance: none;
            -webkit-appearance: none;
            width: 24px;
            height: 24px;
            border-radius: 8px;
            border: 2px solid #ced4da;
            background: #fff;
            cursor: pointer;
            transition: all 0.2s ease;
            position: relative;
            margin: 0;
        }
        .take-checkbox input[type="checkbox"]:hover {
            border-color: #00b14f;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.1);
        }
        .take-checkbox input[type="checkbox"]:checked {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border-color: #00b14f;
            box-shadow: 0 4px 10px rgba(0,177,79,0.3);
        }
        .take-checkbox input[type="checkbox"]:checked::after {
            content: '✓';
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            color: #fff;
            font-weight: 900;
            font-size: 14px;
            line-height: 1;
        }

        /* Notes input inside table */
        .take-table .notes-input {
            width: 100%;
            border: 1.5px solid #e9ecef;
            border-radius: 9px;
            padding: 7px 12px;
            font-size: 12.5px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            min-width: 180px;
        }
        .take-table .notes-input:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- SUMMARY BOX ---------- */
        .take-summary {
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
            padding: 14px 18px;
            border-radius: 12px;
            background: linear-gradient(135deg, rgba(0,177,79,0.06) 0%, rgba(0,177,79,0.02) 100%);
            border: 1.5px solid rgba(0,177,79,0.15);
            margin-bottom: 16px;
        }
        .take-summary-item {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: 12.5px;
            font-weight: 700;
            color: #007a33;
        }
        .take-summary-item i {
            font-size: 13px;
        }

        /* ---------- EMPTY STATE ---------- */
        .take-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .take-empty-icon {
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
        .take-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .take-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0;
        }

        /* Validation message */
        .take-validation {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 8px 16px;
            border-radius: 10px;
            font-size: 13px;
            font-weight: 700;
            margin-top: 12px;
        }
        .take-validation.text-danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .take-validation.text-success {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }

        /* ---------- SAVE ROW ---------- */
        .take-save-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            flex-wrap: wrap;
            padding-top: 20px;
            margin-top: 8px;
            border-top: 1px solid #f1f3f5;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .take-hero { padding: 26px 20px 72px; }
            .take-hero-titles h1 { font-size: 19px; }
            .take-body { padding: 0 16px 40px; }
            .take-filters .take-field { flex: 1 1 45%; }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .take-table { font-size: 11px; min-width: 720px; }
            .take-table thead th,
            .take-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .take-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .take-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .take-hero-titles h1 { font-size: 16px; }
            .take-hero-titles p { font-size: 12px; }
            .take-btn-glass { padding: 8px 14px; font-size: 12px; }
            .take-filters .take-field { flex: 1 1 100%; }
        }

        /* عرض كل سجل تسجيل حضور كبطاقة على الهاتف بدلاً من تمرير الجدول أفقياً */
        @media (max-width: 767.98px) {
            .take-table-wrap {
                overflow: visible !important;
                border: 0;
                border-radius: 0;
                padding: 0;
            }
            .take-table,
            .take-table tbody { display: block; width: 100%; min-width: 0 !important; }
            .take-table thead { display: none; }
            .take-table tbody tr.take-table-header-row { display: none !important; }
            .take-table tbody tr {
                display: block;
                width: 100%;
                margin: 0 0 12px;
                padding: 10px 14px;
                border: 1px solid #e5eee9;
                border-radius: 14px;
                background: #fff;
                box-shadow: 0 4px 14px rgba(31, 61, 51, 0.06);
            }
            .take-table tbody tr:hover td { background: #fff; }
            .take-table tbody td {
                display: grid;
                grid-template-columns: 82px minmax(0, 1fr);
                align-items: center;
                gap: 10px;
                width: 100% !important;
                padding: 8px 0 !important;
                border-bottom: 1px solid #f1f3f5;
                text-align: right !important;
                white-space: normal;
            }
            .take-table tbody td::before {
                content: attr(data-label);
                color: #6c757d;
                font-size: 11px;
                font-weight: 800;
                line-height: 1.5;
                text-align: right;
            }
            .take-table tbody td:first-child {
                display: block;
                padding-top: 4px !important;
                padding-bottom: 12px !important;
            }
            .take-table tbody td:first-child::before { display: block; margin-bottom: 8px; }
            .take-table tbody td:last-child {
                display: block;
                padding-bottom: 2px !important;
                border-bottom: 0;
            }
            .take-table tbody td:last-child::before { display: block; margin-bottom: 8px; }
            .take-table tbody td > *,
            .student-name-cell,
            .take-checkbox { min-width: 0; max-width: 100%; overflow-wrap: anywhere; }
            .take-table .notes-input { width: 100%; min-width: 0; max-width: 100%; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .take-filters,
            .no-print,
            .take-hero-actions,
            .take-save-row {
                display: none !important;
            }
            .take-page { background: #fff !important; }
            .take-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .take-body { margin-top: 0 !important; padding: 0 !important; }
            .take-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .take-table-wrap { overflow: visible !important; border: 1px solid #ccc !important; }
            .take-table { min-width: 0 !important; font-size: 11px !important; }
            .take-table thead th {
                background: #f0f8f3 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .take-table .notes-input { border: none !important; padding: 0 !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="take-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="take-hero">
            <div class="take-hero-content">
                <div class="take-hero-left">
                    <div class="take-hero-icon">
                        <i class="fas fa-clipboard-check"></i>
                    </div>
                    <div class="take-hero-titles">
                        <h1>تسجيل الحضور والغياب</h1>
                        <p>حدد الصف والشعبة ثم قم بتسجيل حضور الطلاب</p>
                    </div>
                </div>
                <div class="take-hero-actions">
                    <a href='<%= ResolveUrl("~/Pages/Attendance/Default.aspx") %>'
                       class="take-btn-glass take-btn-glass-primary">
                        <i class="fas fa-arrow-right"></i> العودة للقائمة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="take-body">

            <div class="take-card">

                <div class="take-card-header">
                    <div class="take-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="take-card-header-text">
                        <div>
                            <h3>تصفية الطلاب</h3>
                            <p>اختر الصف والشعبة والتاريخ لعرض قائمة الطلاب</p>
                        </div>
                    </div>
                    <span class="take-count-pill">
                        <i class="fas fa-users"></i>
                        <asp:Label ID="lblCountPill" runat="server" Text="0" />
                    </span>
                </div>

                <div class="take-card-body">

                    <!-- Filters -->
                    <div class="take-filters no-print">

                        <div class="take-field">
                            <label><i class="fas fa-layer-group"></i> الصف الدراسي</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="-- اختر الصف --" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="take-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الأقسام" Value="" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="take-field">
                            <label><i class="fas fa-chair"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الشعب" Value="" Selected="True" />
                                <asp:ListItem Text="شعبة أ" Value="أ" />
                                <asp:ListItem Text="شعبة ب" Value="ب" />
                                <asp:ListItem Text="شعبة ج" Value="ج" />
                                <asp:ListItem Text="شعبة د" Value="د" />
                            </asp:DropDownList>
                        </div>

                        <div class="take-field">
                            <label><i class="fas fa-calendar-alt"></i> التاريخ</label>
                            <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                        <div class="take-field" style="flex: 0 0 auto;">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnLoad" runat="server" Text="عرض الطلاب"
                                CssClass="take-btn-primary"
                                OnClick="btnLoad_Click" />
                        </div>

                    </div>

                    <!-- Students Panel -->
                    <asp:Panel ID="pnlStudents" runat="server" Visible="false">

                        <div class="table-responsive take-table-wrap">
                            <asp:GridView ID="gvStudents" runat="server"
                                CssClass="take-table"
                                AutoGenerateColumns="false"
                                DataKeyNames="Id"
                                EmptyDataText=""
                                GridLines="None"
                                ShowHeaderWhenEmpty="true"
                                OnRowCreated="gvStudents_RowCreated"
                                OnRowDataBound="gvStudents_RowDataBound">

                                <Columns>

                                    <asp:TemplateField HeaderText="ر.ت" ItemStyle-Width="60px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="row-num"><%# Container.DataItemIndex + 1 %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="اسم الطالب">
                                        <ItemTemplate>
                                            <div class="student-name-cell">
                                                <span class="student-avatar">
                                                    <i class="fas fa-user-graduate"></i>
                                                </span>
                                                <span><%#: Eval("Name") %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="الحضور" ItemStyle-Width="100px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <div class="take-checkbox">
                                                <asp:CheckBox ID="chkPresent" runat="server" Checked="true" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="ملاحظات">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtNotes" runat="server"
                                                CssClass="notes-input"
                                                placeholder="أضف ملاحظة..." />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>
                        </div>

                        <!-- Save Row -->
                        <div class="take-save-row no-print">
                            <div style="display:flex;align-items:center;gap:12px;flex-wrap:wrap;">
                                <asp:Button ID="btnSave" runat="server" Text="حفظ السجلات"
                                    CssClass="take-btn-save"
                                    UseSubmitBehavior="false"
                                    OnClick="btnSave_Click"
                                    OnClientClick="if (typeof(Page_ClientValidate)=='function' &amp;&amp; !Page_ClientValidate('')) { return false; } var b=this; b.value='جارٍ الحفظ...'; b.disabled=true; setTimeout(function(){ b.disabled=false; b.value='حفظ السجلات'; }, 8000);" />

                                <asp:Label ID="lblMessage" runat="server" CssClass="take-validation" />
                            </div>

                            <a href='<%= ResolveUrl("~/Pages/Attendance/Default.aspx") %>'
                               class="take-btn-outline">
                                <i class="fas fa-times"></i> إلغاء
                            </a>
                        </div>

                    </asp:Panel>

                    <!-- Empty State -->
                    <asp:Panel ID="pnlEmpty" runat="server" Visible="true" CssClass="take-empty">
                        <div class="take-empty-icon">
                            <i class="fas fa-clipboard-list"></i>
                        </div>
                        <h4>لم يتم عرض الطلاب بعد</h4>
                        <p>اختر الصف الدراسي والتاريخ ثم اضغط على "عرض الطلاب"</p>
                    </asp:Panel>

                </div>
            </div>

        </div>
    </div>

    <script type="text/javascript">
        function setTodayDate() {
            var dateInput = document.getElementById('<%= txtDate.ClientID %>');
            if (dateInput && !dateInput.value) {
                var today = new Date().toISOString().split('T')[0];
                dateInput.value = today;
            }
        }
        window.onload = setTodayDate;
    </script>
</asp:Content>
