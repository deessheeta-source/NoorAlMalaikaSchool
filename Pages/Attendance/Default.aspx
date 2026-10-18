<%@ Page Title="الحضور والغياب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Attendance.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Attendance
           ========================================================= */
        .attendance-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .attendance-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .attendance-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .attendance-hero::after {
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
        .attendance-hero-content {
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
        .attendance-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .attendance-hero-icon {
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
        .attendance-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .attendance-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .attendance-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .attendance-btn-glass {
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
        .attendance-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .attendance-btn-glass-primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .attendance-btn-glass-primary:hover {
            background: #f8f9fa;
            color: #007a33;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .attendance-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .attendance-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .attendance-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .attendance-card-header-icon {
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
        .attendance-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .attendance-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .attendance-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 200px;
        }
        .attendance-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .attendance-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .attendance-count-pill {
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

        .attendance-card-body {
            padding: 20px 24px;
        }

        /* ---------- FILTERS ROW (Flex - صف واحد) ---------- */
        .attendance-filters {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            align-items: end;
            margin-bottom: 20px;
        }

        .attendance-filters .attendance-field {
            flex: 1 1 160px;
            min-width: 0;
        }

        .attendance-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .attendance-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .attendance-field label i {
            color: #00b14f;
            font-size: 12px;
        }
        .attendance-field .form-control,
        .attendance-field .form-select {
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
        .attendance-field .form-control:focus,
        .attendance-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- BUTTONS ---------- */
        .attendance-btn-primary {
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
        .attendance-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }

        .attendance-btn-outline {
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
        .attendance-btn-outline:hover {
            border-color: #00b14f;
            color: #00b14f;
            background: rgba(0,177,79,0.04);
            text-decoration: none;
        }

        /* ---------- TABLE ---------- */
        .attendance-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .attendance-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .attendance-table thead th {
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
        .attendance-table tbody td {
            font-size: 13px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .attendance-table tbody tr {
            transition: all 0.15s ease;
        }
        .attendance-table tbody tr:hover td {
            background: #f8fdfa;
        }
        .attendance-table tbody tr:last-child td {
            border-bottom: none;
        }

        /* Student name cell */
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

        /* Class badge */
        .class-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            color: #495057;
            font-size: 13px;
            font-weight: 500;
        }

        /* Section badge */
        .section-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 11px;
            font-size: 12px;
            font-weight: 700;
            border-radius: 999px;
        }
        .section-badge.sci {
            background: rgba(13,110,253,0.1);
            color: #0d6efd;
            border: 1px solid rgba(13,110,253,0.2);
        }
        .section-badge.lit {
            background: rgba(111,66,193,0.1);
            color: #5a2fa0;
            border: 1px solid rgba(111,66,193,0.2);
        }

        /* Division badge */
        .division-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 11px;
            font-size: 12px;
            font-weight: 700;
            border-radius: 999px;
            background: #f8f9fa;
            color: #495057;
            border: 1px solid #e9ecef;
        }

        /* Status badge */
        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
        }
        .status-badge.present {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .status-badge.absent {
            background: rgba(220,53,69,0.1);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }

        /* Date badge */
        .date-cell {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-family: 'Courier New', monospace;
            font-weight: 600;
            font-size: 12px;
            color: #495057;
        }
        .date-cell i {
            color: #00b14f;
            font-size: 10px;
        }

        /* Notes cell */
        .notes-cell {
            color: #6c757d;
            font-size: 12.5px;
        }

        /* ---------- ACTION BUTTONS ---------- */
        .action-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 5px;
            padding: 7px 11px;
            border-radius: 9px;
            font-size: 12px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.15s ease;
            text-decoration: none;
            border: 1.5px solid transparent;
            white-space: nowrap;
            background: #fff;
        }
        .action-btn i { font-size: 12px; }
        .action-btn.edit {
            color: #00b14f;
            border-color: rgba(0,177,79,0.3);
        }
        .action-btn.edit:hover {
            background: #00b14f;
            color: #fff;
            box-shadow: 0 4px 10px rgba(0,177,79,0.25);
            text-decoration: none;
        }

        /* Pagination */
        .attendance-table .pagination,
        .attendance-table .table-pagination {
            display: flex;
            gap: 6px;
            justify-content: center;
            padding: 16px 0 4px;
            margin: 0;
            list-style: none;
        }
        .attendance-table .table-pagination a,
        .attendance-table .table-pagination span {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 34px;
            height: 34px;
            padding: 0 10px;
            border-radius: 8px;
            background: #fff;
            border: 1.5px solid #e9ecef;
            color: #495057;
            font-size: 12.5px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.15s ease;
        }
        .attendance-table .table-pagination a:hover {
            border-color: #00b14f;
            color: #00b14f;
            background: rgba(0,177,79,0.04);
        }
        .attendance-table .table-pagination span {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            border-color: #00b14f;
        }

        /* ---------- EMPTY STATE ---------- */
        .attendance-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .attendance-empty-icon {
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
        .attendance-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .attendance-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .attendance-hero { padding: 26px 20px 72px; }
            .attendance-hero-titles h1 { font-size: 19px; }
            .attendance-body { padding: 0 16px 40px; }

            .attendance-filters .attendance-field {
                flex: 1 1 45%;
            }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .attendance-table { font-size: 11px; min-width: 900px; }
            .attendance-table thead th,
            .attendance-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .attendance-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .attendance-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .attendance-hero-titles h1 { font-size: 16px; }
            .attendance-hero-titles p { font-size: 12px; }
            .attendance-btn-glass { padding: 8px 14px; font-size: 12px; }

            .attendance-filters .attendance-field {
                flex: 1 1 100%;
            }
        }

        /* عرض كل سجل حضور كبطاقة على الهاتف بدلاً من تمرير الجدول أفقياً */
        @media (max-width: 767.98px) {
            .attendance-table-wrap {
                overflow: visible !important;
                border: 0;
                border-radius: 0;
                padding: 0;
            }
            .attendance-table,
            .attendance-table tbody { display: block; width: 100%; min-width: 0 !important; }
            .attendance-table thead { display: none; }
            .attendance-table tbody tr.attendance-table-header-row { display: none !important; }
            .attendance-table tbody tr {
                display: block;
                width: 100%;
                margin: 0 0 12px;
                padding: 10px 14px;
                border: 1px solid #e5eee9;
                border-radius: 14px;
                background: #fff;
                box-shadow: 0 4px 14px rgba(31, 61, 51, 0.06);
            }
            .attendance-table tbody tr.attendance-table-pager-row {
                display: block;
                margin: 0;
                padding: 12px 0 0;
                border: 0;
                border-radius: 0;
                background: transparent;
                box-shadow: none;
            }
            .attendance-table tbody tr.attendance-table-pager-row td {
                display: block;
                width: 100% !important;
                padding: 0 !important;
                border: 0;
                background: transparent;
                text-align: center !important;
            }
            .attendance-table tbody tr.attendance-table-pager-row td::before { content: none; display: none; }
            .attendance-table tbody tr.attendance-table-pager-row .table-pagination { flex-wrap: wrap; }
            .attendance-table tbody tr:hover td { background: #fff; }
            .attendance-table tbody td {
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
            .attendance-table tbody td::before {
                content: attr(data-label);
                color: #6c757d;
                font-size: 11px;
                font-weight: 800;
                line-height: 1.5;
                text-align: right;
            }
            .attendance-table tbody td:first-child {
                display: block;
                padding-top: 4px !important;
                padding-bottom: 12px !important;
            }
            .attendance-table tbody td:first-child::before { display: block; margin-bottom: 8px; }
            .attendance-table tbody td:last-child {
                display: block;
                padding-bottom: 2px !important;
                border-bottom: 0;
            }
            .attendance-table tbody td:last-child::before { display: block; margin-bottom: 8px; }
            .attendance-table tbody td > *,
            .student-name-cell,
            .notes-cell { min-width: 0; max-width: 100%; overflow-wrap: anywhere; }
            .attendance-table tbody td:last-child .action-btn { width: 100%; margin: 0; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .attendance-filters,
            .no-print {
                display: none !important;
            }
            .attendance-hero-actions {
                display: none !important;
            }
            .pagination,
            .attendance-table .pagination,
            .attendance-table .table-pagination {
                display: none !important;
            }
            .action-btn {
                display: none !important;
            }
            .attendance-page { background: #fff !important; }
            .attendance-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .attendance-body {
                margin-top: 0 !important;
                padding: 0 !important;
            }
            .attendance-card {
                box-shadow: none !important;
                border: 1px solid #ddd !important;
            }
            .attendance-table-wrap {
                overflow: visible !important;
                border: 1px solid #ccc !important;
            }
            .attendance-table {
                min-width: 0 !important;
                font-size: 11px !important;
            }
            .attendance-table thead th {
                background: #f0f8f3 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .attendance-table tr { page-break-inside: avoid; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="attendance-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="attendance-hero">
            <div class="attendance-hero-content">
                <div class="attendance-hero-left">
                    <div class="attendance-hero-icon">
                        <i class="fas fa-clipboard-check"></i>
                    </div>
                    <div class="attendance-hero-titles">
                        <h1>الحضور والغياب</h1>
                        <p>متابعة سجلات حضور وغياب الطلاب</p>
                    </div>
                </div>
                <div class="attendance-hero-actions">
                    <asp:Button ID="btnExportWord" runat="server" Text="تصدير ورد"
                        CssClass="attendance-btn-glass"
                        OnClick="btnExportWord_Click" />
                    <a href='<%= "Take.aspx?stage=" + (Session["Stage"] ?? "") %>'
                       class="attendance-btn-glass attendance-btn-glass-primary">
                        <i class="fas fa-plus"></i> تسجيل الحضور
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="attendance-body">

            <div class="attendance-card">

                <div class="attendance-card-header">
                    <div class="attendance-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="attendance-card-header-text">
                        <div>
                            <h3>سجلات الحضور والغياب</h3>
                            <p>استخدم الفلاتر للبحث والتصفية</p>
                        </div>
                    </div>
                    <span class="attendance-count-pill">
                        <i class="fas fa-list"></i>
                        <asp:Label ID="lblCountPill" runat="server" Text="0" />
                    </span>
                </div>

                <div class="attendance-card-body">

                    <!-- Filters -->
                    <div class="attendance-filters no-print">

                        <div class="attendance-field">
                            <label><i class="fas fa-calendar-alt"></i> التاريخ</label>
                            <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                        <div class="attendance-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlClassRoom" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlClassRoom_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="attendance-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الأقسام" Value="" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="attendance-field">
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

                        <div class="attendance-field" style="flex: 0 0 auto;">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnFilter" runat="server" Text="بحث"
                                CssClass="attendance-btn-primary"
                                OnClick="btnFilter_Click" />
                        </div>

                        <div class="attendance-field" style="flex: 0 0 auto;">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnReset" runat="server" Text="إعادة"
                                CssClass="attendance-btn-outline"
                                OnClick="btnReset_Click" />
                        </div>

                    </div>

                    <asp:UpdatePanel ID="upAttendance" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <!-- Empty State -->
                            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="attendance-empty">
                                <div class="attendance-empty-icon">
                                    <i class="fas fa-clipboard-list"></i>
                                </div>
                                <h4>لا توجد سجلات حضور</h4>
                                <p>لم يتم تسجيل أي حضور في هذا التاريخ أو ضمن الفلاتر المحددة</p>
                            </asp:Panel>

                            <!-- Table -->
                            <div class="table-responsive attendance-table-wrap">
                                <asp:GridView ID="gvAttendance" runat="server"
                                    CssClass="attendance-table"
                                    AutoGenerateColumns="false"
                                    EmptyDataText=""
                                    GridLines="None"
                                    AllowPaging="true"
                                    PageSize="20"
                                    OnPageIndexChanging="gvAttendance_PageIndexChanging"
                                    OnRowCreated="gvAttendance_RowCreated"
                                    OnRowDataBound="gvAttendance_RowDataBound"
                                    ShowHeaderWhenEmpty="true">

                                    <Columns>

                                        <asp:TemplateField HeaderText="#" ItemStyle-Width="50px" ItemStyle-CssClass="text-center">
                                            <ItemTemplate>
                                                <span style="color:#adb5bd;font-weight:700;font-size:12px;">
                                                    <%# Container.DataItemIndex + 1 %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="اسم الطالب">
                                            <ItemTemplate>
                                                <div class="student-name-cell">
                                                    <span class="student-avatar">
                                                        <i class="fas fa-user-graduate"></i>
                                                    </span>
                                                    <span><%#: Eval("StudentName") %></span>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="الصف" ItemStyle-CssClass="text-center">
                                            <ItemTemplate>
                                                <span class="class-badge">
                                                    <%#: Eval("ClassName") %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="القسم" ItemStyle-CssClass="text-center">
                                            <ItemTemplate>
                                                <%# FormatSectionBadge(Eval("Section")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="الشعبة" ItemStyle-CssClass="text-center">
                                            <ItemTemplate>
                                                <%# FormatDivisionBadge(Eval("Division")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="التاريخ" ItemStyle-CssClass="text-center">
                                            <ItemTemplate>
                                                <span class="date-cell">
                                                    <i class="fas fa-calendar-day"></i>
                                                    <%# Eval("Date", "{0:yyyy-MM-dd}") %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="الحالة" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                            <ItemTemplate>
                                                <%# (bool)Eval("IsPresent")
                                                    ? "<span class='status-badge present'><i class='fas fa-check-circle' style='font-size:10px;'></i> حاضر</span>"
                                                    : "<span class='status-badge absent'><i class='fas fa-times-circle' style='font-size:10px;'></i> غائب</span>" %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="ملاحظات">
                                            <ItemTemplate>
                                                <span class="notes-cell">
                                                    <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("Notes")))
                                                        ? "—"
                                                        : HttpUtility.HtmlEncode(Convert.ToString(Eval("Notes"))) %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="تعديل" ItemStyle-Width="90px" ItemStyle-CssClass="text-center">
                                            <ItemTemplate>
                                                <a class="action-btn edit"
                                                   href='<%# "Take.aspx?class=" + Eval("ClassRoomId") + "&date=" + Eval("Date", "{0:yyyy-MM-dd}") %>'
                                                   title="تعديل">
                                                    <i class="fas fa-pen"></i> تعديل
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>

                                    <PagerStyle CssClass="table-pagination" />
                                </asp:GridView>
                            </div>

                            <asp:Label ID="lblInfo" runat="server"
                                CssClass="text-muted mt-3 d-block"
                                Style="font-size:12.5px;text-align:center;" />

                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </div>

        </div>
    </div>
</asp:Content>
