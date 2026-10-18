<%@ Page Title="تقرير الحضور" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AttendanceReport.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Reports.AttendanceReport" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Attendance Report
           ========================================================= */
        .report-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .report-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .report-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .report-hero::after {
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
        .report-hero-content {
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
        .report-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .report-hero-icon {
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
        .report-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .report-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .report-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .report-btn-glass {
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
        .report-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .report-btn-glass-primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .report-btn-glass-primary:hover {
            background: #f8f9fa;
            color: #007a33;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .report-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .report-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .report-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .report-card-header-icon {
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
        .report-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .report-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .report-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 200px;
        }
        .report-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .report-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .report-count-pill {
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

        .report-card-body {
            padding: 20px 24px;
        }

        /* ---------- FILTERS ROW ---------- */
        .report-filters {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            align-items: end;
            margin-bottom: 20px;
        }

        .report-filters .report-field {
            flex: 1 1 180px;
            min-width: 0;
        }

        .report-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .report-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .report-field label i {
            color: #00b14f;
            font-size: 12px;
        }
        .report-field .form-control,
        .report-field .form-select {
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
        .report-field .form-control:focus,
        .report-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- BUTTONS ---------- */
        .report-btn-primary {
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
        .report-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }

        /* ---------- SUMMARY CARDS ---------- */
        .report-summary-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 14px;
            margin-bottom: 20px;
        }
        .report-summary-card {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 16px 18px;
            border-radius: 14px;
            background: #ffffff;
            border: 1px solid #99f6c3;
            box-shadow: 0 4px 12px rgba(0,0,0,0.03);
            transition: all 0.2s ease;
        }
        .report-summary-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.06);
        }
        .report-summary-icon {
            width: 46px;
            height: 46px;
            border-radius: 12px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            flex-shrink: 0;
        }
        .report-summary-icon.green {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
        }
        .report-summary-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            color: #fff;
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .report-summary-icon.red {
            background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%);
            color: #fff;
            box-shadow: 0 6px 16px rgba(220,53,69,0.3);
        }
        .report-summary-icon.orange {
            background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%);
            color: #fff;
            box-shadow: 0 6px 16px rgba(253,126,20,0.3);
        }
        .report-summary-info {
            display: flex;
            flex-direction: column;
            gap: 2px;
            min-width: 0;
        }
        .report-summary-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
        }
        .report-summary-value {
            font-size: 20px;
            font-weight: 800;
            color: #212529;
            line-height: 1.1;
        }

        /* ---------- TABLE ---------- */
        .report-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .report-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .report-table thead th {
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
        .report-table tbody td {
            font-size: 13px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .report-table tbody tr {
            transition: all 0.15s ease;
        }
        .report-table tbody tr:hover td {
            background: #f8fdfa;
        }
        .report-table tbody tr:last-child td {
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

        /* Row number */
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

        /* Class badge */
        .class-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            color: #495057;
            font-size: 13px;
            font-weight: 500;
        }

        /* Count badge */
        .count-badge {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 40px;
            padding: 5px 12px;
            border-radius: 9px;
            font-size: 13px;
            font-weight: 800;
            font-family: 'Courier New', monospace;
        }
        .count-badge.present {
            background: rgba(0,177,79,0.1);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.2);
        }
        .count-badge.absent {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.2);
        }

        /* Percentage bar */
        .percentage-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            min-width: 120px;
        }
        .percentage-value {
            font-size: 13px;
            font-weight: 800;
            color: #212529;
            font-family: 'Courier New', monospace;
            min-width: 50px;
        }
        .percentage-bar {
            flex: 1;
            height: 8px;
            border-radius: 999px;
            background: #f1f3f5;
            overflow: hidden;
            position: relative;
        }
        .percentage-bar-fill {
            height: 100%;
            border-radius: 999px;
            transition: width 0.4s ease;
            background: linear-gradient(90deg, #00b14f 0%, #4ed17e 100%);
        }
        .percentage-bar-fill.high {
            background: linear-gradient(90deg, #00b14f 0%, #4ed17e 100%);
        }
        .percentage-bar-fill.medium {
            background: linear-gradient(90deg, #fd7e14 0%, #ffa94d 100%);
        }
        .percentage-bar-fill.low {
            background: linear-gradient(90deg, #dc3545 0%, #f0616d 100%);
        }

        /* ---------- EMPTY STATE ---------- */
        .report-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .report-empty-icon {
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
        .report-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .report-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0;
        }

        /* Message label */
        .report-message {
            display: none;
            padding: 12px 18px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 700;
            margin-top: 16px;
        }
        .report-message.text-success {
            display: block;
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .report-message.text-danger {
            display: block;
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .report-message.text-info {
            display: block;
            background: rgba(13,110,253,0.08);
            color: #0d6efd;
            border: 1px solid rgba(13,110,253,0.25);
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .report-hero { padding: 26px 20px 72px; }
            .report-hero-titles h1 { font-size: 19px; }
            .report-body { padding: 0 16px 40px; }
            .report-filters .report-field { flex: 1 1 45%; }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .report-table { font-size: 11px; min-width: 900px; }
            .report-table thead th,
            .report-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .report-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .report-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .report-hero-titles h1 { font-size: 16px; }
            .report-hero-titles p { font-size: 12px; }
            .report-btn-glass { padding: 8px 14px; font-size: 12px; }
            .report-filters .report-field { flex: 1 1 100%; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .report-filters,
            .no-print,
            .report-hero-actions,
            .report-message {
                display: none !important;
            }
            .report-page { background: #fff !important; }
            .report-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .report-body { margin-top: 0 !important; padding: 0 !important; }
            .report-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .report-table-wrap { overflow: visible !important; border: 1px solid #ccc !important; }
            .report-table { min-width: 0 !important; font-size: 11px !important; }
            .report-table thead th {
                background: #f0f8f3 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .report-table tr { page-break-inside: avoid; }
            .report-summary-grid { grid-template-columns: repeat(4, 1fr) !important; }
            .report-summary-card { border: 1px solid #ccc !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="report-page" dir="rtl">

        <div class="report-hero">
            <div class="report-hero-content">
                <div class="report-hero-left">
                    <div class="report-hero-icon">
                        <i class="fas fa-clipboard-check"></i>
                    </div>
                    <div class="report-hero-titles">
                        <h1>تقرير الحضور والغياب</h1>
                        <p>تقرير شامل لحضور الطلاب خلال فترة محددة</p>
                    </div>
                </div>
                <div class="report-hero-actions">
                    <asp:Button ID="btnExportWord" runat="server" Text="تصدير ورد"
                        CssClass="report-btn-glass"
                        OnClick="btnExportWord_Click" />
                </div>
            </div>
        </div>

        <div class="report-body">

            <div class="report-card">

                <div class="report-card-header">
                    <div class="report-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="report-card-header-text">
                        <div>
                            <h3>معايير التقرير</h3>
                            <p>اختر الفترة الزمنية والصف والشعبة ثم اضغط "عرض التقرير"</p>
                        </div>
                    </div>
                    <span class="report-count-pill">
                        <i class="fas fa-users"></i>
                        <asp:Label ID="lblCountPill" runat="server" Text="0" />
                    </span>
                </div>

                <div class="report-card-body">

                    <div class="report-filters no-print">

                        <div class="report-field">
                            <label><i class="fas fa-calendar-alt"></i> من تاريخ</label>
                            <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                        <div class="report-field">
                            <label><i class="fas fa-calendar-check"></i> إلى تاريخ</label>
                            <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                        <div class="report-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="report-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="جميع الأقسام" />
                                <asp:ListItem Value="علمي" Text="القسم العلمي" />
                                <asp:ListItem Value="أدبي" Text="القسم الأدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="report-field">
                            <label><i class="fas fa-chair"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlClassRoom" runat="server" CssClass="form-select">
                                <asp:ListItem Value="" Text="جميع الشعب" />
                            </asp:DropDownList>
                        </div>

                        <div class="report-field" style="flex: 0 0 auto;">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnFilter" runat="server" Text="عرض التقرير"
                                CssClass="report-btn-primary"
                                OnClick="btnFilter_Click" />
                        </div>

                    </div>

                    <asp:Panel ID="pnlSummary" runat="server" Visible="false">
                        <div class="report-summary-grid">

                            <div class="report-summary-card">
                                <div class="report-summary-icon blue">
                                    <i class="fas fa-users"></i>
                                </div>
                                <div class="report-summary-info">
                                    <span class="report-summary-label">عدد الطلاب</span>
                                    <span class="report-summary-value">
                                        <asp:Label ID="lblTotalStudents" runat="server" Text="0" />
                                    </span>
                                </div>
                            </div>

                            <div class="report-summary-card">
                                <div class="report-summary-icon green">
                                    <i class="fas fa-check-circle"></i>
                                </div>
                                <div class="report-summary-info">
                                    <span class="report-summary-label">إجمالي الحضور</span>
                                    <span class="report-summary-value">
                                        <asp:Label ID="lblTotalPresent" runat="server" Text="0" />
                                    </span>
                                </div>
                            </div>

                            <div class="report-summary-card">
                                <div class="report-summary-icon red">
                                    <i class="fas fa-times-circle"></i>
                                </div>
                                <div class="report-summary-info">
                                    <span class="report-summary-label">إجمالي الغياب</span>
                                    <span class="report-summary-value">
                                        <asp:Label ID="lblTotalAbsent" runat="server" Text="0" />
                                    </span>
                                </div>
                            </div>

                            <div class="report-summary-card">
                                <div class="report-summary-icon orange">
                                    <i class="fas fa-percentage"></i>
                                </div>
                                <div class="report-summary-info">
                                    <span class="report-summary-label">نسبة الحضور العامة</span>
                                    <span class="report-summary-value">
                                        <asp:Label ID="lblOverallPercentage" runat="server" Text="0%" />
                                    </span>
                                </div>
                            </div>

                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlEmpty" runat="server" Visible="true" CssClass="report-empty">
                        <div class="report-empty-icon">
                            <i class="fas fa-chart-bar"></i>
                        </div>
                        <h4>لم يتم إنشاء التقرير بعد</h4>
                        <p>اختر الفترة الزمنية والصف ثم اضغط على "عرض التقرير"</p>
                    </asp:Panel>

                    <asp:Panel ID="pnlReport" runat="server" Visible="false">
                        <div class="table-responsive report-table-wrap">
                            <asp:GridView ID="gvReport" runat="server"
                                CssClass="report-table"
                                AutoGenerateColumns="false"
                                EmptyDataText=""
                                GridLines="None"
                                ShowHeaderWhenEmpty="true"
                                OnRowDataBound="gvReport_RowDataBound">

                                <Columns>

                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="60px" ItemStyle-CssClass="text-center">
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
                                            <span class="class-badge">
                                                <%#: Eval("SectionText") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="الشعبة" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="class-badge">
                                                <%#: Eval("DivisionText") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="أيام الحضور" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="count-badge present">
                                                <%# Eval("TotalPresent") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="أيام الغياب" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="count-badge absent">
                                                <%# Eval("TotalAbsent") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="نسبة الحضور" ItemStyle-Width="200px">
                                        <ItemTemplate>
                                            <div class="percentage-cell">
                                                <span class="percentage-value">
                                                    <%# Convert.ToDecimal(Eval("Percentage")).ToString("F1") %>%
                                                </span>
                                                <div class="percentage-bar">
                                                    <div id="barFill" runat="server" class="percentage-bar-fill"></div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>
                        </div>
                    </asp:Panel>

                    <asp:Label ID="lblMessage" runat="server" CssClass="report-message" />

                </div>
            </div>

        </div>
    </div>
</asp:Content>