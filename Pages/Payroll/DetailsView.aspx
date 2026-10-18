<%@ Page Title="عرض تفاصيل الفترة المالية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DetailsView.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Payroll.DetailsView" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">

    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />

    <style>
        .tf-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
        }

        /* HERO */
        .tf-hero-section {
            position: relative;
            background: linear-gradient(135deg, #1b7f4d, #0f5f38);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }

        .tf-hero-section::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%,
                    rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%,
                    rgba(255,255,255,0.08) 0%, transparent 40%);
            pointer-events: none;
        }

        .tf-hero-content {
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

        .tf-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }

        .tf-hero-icon {
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
        }

        .tf-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
        }

        .tf-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
            max-width: 640px;
        }

        .tf-hero-chip {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 5px 14px;
            border-radius: 999px;
            background: rgba(255,255,255,0.22);
            border: 1px solid rgba(255,255,255,0.3);
            color: #fff;
            font-weight: 700;
            font-size: 12.5px;
            margin-top: 8px;
        }

        .tf-hero-chip.badge-blue {
            background: rgba(13,110,253,0.35);
            border-color: rgba(13,110,253,0.55);
        }

        .tf-hero-chip.badge-yellow {
            background: rgba(255,193,7,0.35);
            border-color: rgba(255,193,7,0.55);
        }

        .tf-hero-chip.badge-green {
            background: rgba(76,201,120,0.35);
            border-color: rgba(76,201,120,0.55);
        }

        .tf-hero-chip.badge-red {
            background: rgba(220,53,69,0.35);
            border-color: rgba(220,53,69,0.55);
        }

        .tf-btn-glass {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 10px 20px;
            border-radius: 12px;
            background: rgba(255,255,255,0.15);
            border: 1.5px solid rgba(255,255,255,0.3);
            color: #fff;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.2s ease;
            cursor: pointer;
            flex-shrink: 0;
            font-family: inherit;
        }

        .tf-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* BODY */
        .tf-body {
            max-width: 1280px;
            margin: -56px auto 0;
            padding: 0 24px 40px;
            position: relative;
            z-index: 3;
        }

        /* FILTERS */
        .tf-filters {
            background: #fff;
            border: 1px solid #b7e4c9;
            border-radius: 18px;
            padding: 16px 20px;
            display: flex;
            align-items: flex-end;
            gap: 14px;
            flex-wrap: wrap;
            margin-bottom: 18px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.03);
        }

        .tf-filter {
            display: flex;
            flex-direction: column;
            gap: 5px;
            min-width: 220px;
        }

        .tf-filter label {
            font-size: 11.5px;
            font-weight: 800;
            color: #6c757d;
        }

        .tf-filter select.tf-select {
            border: 1.5px solid #dee2e6;
            border-radius: 10px;
            padding: 9px 12px;
            font-size: 13px;
            font-family: inherit;
            background: #fff;
            color: #1f3d33;
            font-weight: 700;
            min-width: 220px;
            cursor: pointer;
            transition: all 0.15s ease;
        }

        .tf-filter select.tf-select:focus {
            outline: none;
            border-color: #1b7f4d;
            box-shadow: 0 0 0 3px rgba(27,127,77,0.10);
        }

        /* KPI */
        .tf-kpis {
            display: grid;
            grid-template-columns: repeat(6, minmax(0, 1fr));
            gap: 12px;
            margin-bottom: 18px;
        }

        .tf-kpi {
            background: #fff;
            border: 1px solid #b7e4c9;
            border-radius: 18px;
            padding: 15px 12px;
            display: flex;
            align-items: center;
            gap: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.03);
            position: relative;
            overflow: hidden;
            transition: all 0.25s ease;
        }

        .tf-kpi::before {
            content: '';
            position: absolute;
            top: 0;
            right: 0;
            width: 4px;
            height: 100%;
            background: currentColor;
            opacity: 0.85;
        }

        .tf-kpi:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 25px rgba(27,127,77,0.10);
        }

        .tf-kpi .kpi-icon {
            width: 38px;
            height: 38px;
            border-radius: 12px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            color: #fff;
            flex-shrink: 0;
        }

        .tf-kpi .kpi-body {
            display: flex;
            flex-direction: column;
            gap: 4px;
            min-width: 0;
        }

        .tf-kpi .kpi-label {
            font-size: 11px;
            font-weight: 700;
            color: #6c757d;
            line-height: 1.5;
        }

        .tf-kpi .kpi-value {
            font-size: 18px;
            font-weight: 800;
            color: #212529;
            line-height: 1.3;
            font-family: 'Courier New', monospace;
        }

        .kpi-green {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
        }

        .kpi-blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
        }

        .kpi-purple {
            background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%);
        }

        .kpi-red {
            background: linear-gradient(135deg, #dc3545 0%, #f06572 100%);
        }

        .kpi-teal {
            background: linear-gradient(135deg, #0c7a7a 0%, #35a8a8 100%);
        }

        .kpi-orange {
            background: linear-gradient(135deg, #e67e22 0%, #f39c12 100%);
        }

        .kpi-green-wrap  { color: #00b14f; }
        .kpi-blue-wrap   { color: #0d6efd; }
        .kpi-purple-wrap { color: #6f42c1; }
        .kpi-red-wrap    { color: #dc3545; }
        .kpi-teal-wrap   { color: #0c7a7a; }
        .kpi-orange-wrap { color: #e67e22; }

        .kpi-unit {
            font-size: 10px;
            font-weight: 700;
            color: #94a3a8;
        }

        /* INFO */
        .tf-info-line {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 12px 18px;
            background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 12px;
            margin-bottom: 18px;
            font-size: 12.5px;
            font-weight: 600;
            color: #1f3d33;
        }

        .tf-info-line i {
            width: 30px;
            height: 30px;
            border-radius: 9px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            flex-shrink: 0;
        }

        /* CARD */
        .tf-card {
            background: #fff;
            border-radius: 20px;
            border: 1px solid #b7e4c9;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 18px;
        }

        .tf-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #fff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }

        .tf-card-header-icon {
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
        }

        .tf-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }

        .tf-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }

        .tf-count-pill {
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

        .tf-card-body {
            padding: 16px;
        }

        /* TABLE */
        .tf-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #e8eef0;
            background: #fff;
            box-shadow: 0 2px 8px rgba(0,0,0,0.02);
        }

        .tf-table-scroll {
            max-height: 780px;
            overflow-y: auto;
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }

        .tf-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 12.5px;
        }

        .tf-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #edf7f1 100%);
            color: #1f3d33;
            font-size: 11px;
            font-weight: 800;
            padding: 14px 12px !important;
            text-align: right;
            border-bottom: 2px solid #c8e5d5;
            white-space: nowrap;
            position: sticky;
            top: 0;
            z-index: 5;
        }

        .tf-table tbody td {
            font-size: 12.5px;
            padding: 14px 12px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f5f3;
            color: #212529;
            background: #fff;
            text-align: right;
        }

        .tf-table tbody tr:nth-child(even) td {
            background: #fbfdfc;
        }

        .tf-table tbody tr:hover td {
            background: #f0f8f3 !important;
        }

        .tf-table tbody tr:last-child td {
            border-bottom: none;
        }

        .money {
            font-variant-numeric: tabular-nums;
            font-weight: 700;
            color: #1f3d33;
            direction: ltr;
            display: inline-block;
            font-size: 13px;
        }

        .money.pos { color: #007a33; }
        .money.neg { color: #a51c1c; }
        .money.neutral {
            color: #94a3a8;
            font-weight: 600;
        }

        .money.big {
            font-size: 13px;
            font-weight: 700;
        }

        .tf-teacher-name {
            font-size: 13px;
            font-weight: 800;
            color: #1f3d33;
        }

        .status-indicator {
            display: inline-flex;
            align-items: center;
            gap: 7px;
            padding: 6px 13px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 800;
            white-space: nowrap;
        }

        .status-indicator .dot {
            width: 7px;
            height: 7px;
            border-radius: 50%;
            display: inline-block;
            flex-shrink: 0;
        }

        .status-indicator.green {
            background: rgba(0,177,79,0.10);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.22);
        }

        .status-indicator.green .dot {
            background: #00b14f;
            box-shadow: 0 0 0 3px rgba(0,177,79,0.20);
        }

        .badge-soft {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 11px;
            border-radius: 8px;
            font-size: 11px;
            font-weight: 800;
            white-space: nowrap;
        }

        .badge-soft.blue {
            background: rgba(13,110,253,0.10);
            color: #0a58ca;
            border: 1px solid rgba(13,110,253,0.22);
        }

        .row-num {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 28px;
            height: 28px;
            border-radius: 9px;
            background: #f8faf9;
            color: #94a3a8;
            font-weight: 800;
            font-size: 11.5px;
            font-family: 'Courier New', monospace;
            border: 1px solid #eef1f3;
        }

        /* PAGINATION */
        .table-pagination {
            padding: 16px 20px !important;
            background: #fbfdfc;
            border-top: 1px solid #eef1f3;
            text-align: center;
        }

        .table-pagination a,
        .table-pagination span {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 36px;
            height: 36px;
            padding: 0 10px;
            margin: 0 3px;
            border-radius: 10px;
            font-size: 12.5px;
            font-weight: 800;
            color: #495057;
            background: #fff;
            border: 1.5px solid #e9ecef;
            text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
            vertical-align: middle;
        }

        .table-pagination a:hover {
            background: #f0f8f3;
            border-color: #00b14f;
            color: #007a33;
        }

        .table-pagination span {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            border-color: transparent;
            box-shadow: 0 4px 10px rgba(0,177,79,0.3);
        }

        /* EMPTY */
        .tf-empty {
            text-align: center;
            padding: 50px 20px;
        }

        .tf-empty-icon {
            width: 72px;
            height: 72px;
            border-radius: 20px;
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            color: #00b14f;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 32px;
            margin-bottom: 16px;
        }

        .tf-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 15px;
            margin: 0 0 6px;
        }

        .tf-empty p {
            color: #6c757d;
            font-size: 12.5px;
            margin: 0;
        }

        .tf-info-msg {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 10px 16px;
            border-radius: 10px;
            font-size: 12.5px;
            font-weight: 700;
            margin-top: 14px;
        }

        .tf-info-msg.danger {
            background: rgba(220,53,69,0.10);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }

        .tf-info-msg.neutral {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }

        /* RESPONSIVE */
        @media (max-width: 1400px) {
            .tf-kpis {
                grid-template-columns: repeat(3, minmax(0, 1fr));
            }
        }

        @media (max-width: 992px) {
            .tf-hero-section {
                padding: 26px 20px 72px;
            }

            .tf-hero-titles h1 {
                font-size: 19px;
            }

            .tf-body {
                padding: 0 16px 40px;
            }

            .tf-kpis {
                grid-template-columns: repeat(2, minmax(0, 1fr));
                gap: 10px;
            }

            .tf-filter {
                min-width: 100%;
            }

            .tf-filter select.tf-select {
                width: 100%;
            }
        }

        @media (max-width: 576px) {
            .tf-hero-section {
                padding: 20px 16px 66px;
                border-radius: 0 0 24px 24px;
            }

            .tf-hero-left {
                align-items: flex-start;
                gap: 12px;
            }

            .tf-hero-icon {
                width: 54px;
                height: 54px;
                font-size: 24px;
            }

            .tf-hero-titles h1 {
                font-size: 16px;
            }

            .tf-kpis {
                grid-template-columns: 1fr;
            }
        }

        /* PRINT */
        @media print {
            .tf-hero-section,
            .tf-filters,
            .no-print {
                display: none !important;
            }

            .tf-page {
                background: #fff !important;
                min-height: auto !important;
            }

            .tf-body {
                max-width: none !important;
                margin: 0 !important;
                padding: 0 !important;
            }

            .tf-card {
                box-shadow: none !important;
                border: 1px solid #ddd !important;
                break-inside: avoid;
            }

            .tf-table-scroll {
                max-height: none !important;
                overflow: visible !important;
            }

            .tf-table thead th {
                position: static !important;
            }

            .tf-kpis {
                grid-template-columns: repeat(3, minmax(0, 1fr));
            }

            body {
                background: #fff !important;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="tf-page" dir="rtl">

        <!-- HERO -->
        <div class="tf-hero-section">
            <div class="tf-hero-content">
                <div class="tf-hero-left">
                    <div class="tf-hero-icon">
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>

                    <div class="tf-hero-titles">
                        <h1>عرض تفاصيل الفترة المالية</h1>

                        <p>
                            عرض كشف احتساب الاستحقاق للمعلمين في الفترة المالية المحددة.
                        </p>

                        <span class="tf-hero-chip">
                            <i class="fas fa-calendar-alt"></i>
                            الفترة:
                            <asp:Label ID="lblPeriodTitle" runat="server" />
                            &nbsp;·&nbsp;
                            الحالة:
                            <asp:Label ID="lblStatus" runat="server" />
                        </span>
                    </div>
                </div>
            </div>
        </div>

        <!-- BODY -->
        <div class="tf-body">

            <!-- FILTERS -->
            <div class="tf-filters no-print">

                <div class="tf-filter">
                    <label for="<%= ddlPeriod.ClientID %>">الفترة المالية</label>

                    <asp:DropDownList
                        ID="ddlPeriod"
                        runat="server"
                        CssClass="tf-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">

                        <asp:ListItem Value="0" Text="-- اختر الفترة --" />
                    </asp:DropDownList>
                </div>

                <div class="tf-filter">
                    <label for="<%= ddlStageFilter.ClientID %>">المرحلة الدراسية</label>

                    <asp:DropDownList
                        ID="ddlStageFilter"
                        runat="server"
                        CssClass="tf-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlStageFilter_SelectedIndexChanged">

                        <asp:ListItem Value="Basic" Text="المرحلة الأساسية" />
                        <asp:ListItem Value="Secondary" Text="المرحلة الثانوية" />
                        <asp:ListItem Value="all" Text="كل المراحل" />
                    </asp:DropDownList>
                </div>

                <div class="tf-filter">
                    <label for="<%= ddlEmploymentFilter.ClientID %>">نوع التعاقد</label>

                    <asp:DropDownList
                        ID="ddlEmploymentFilter"
                        runat="server"
                        CssClass="tf-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlEmploymentFilter_SelectedIndexChanged">

                        <asp:ListItem Value="all" Text="كل الأنواع" />
                        <asp:ListItem Value="PerLesson" Text="معلم بالحصة" />
                        <asp:ListItem Value="Permanent" Text="معلم براتب شهري" />
                        <asp:ListItem Value="PermanentSupervisor" Text="مشرف براتب شهري" />
                    </asp:DropDownList>
                </div>

                <asp:Button
                    ID="btnPrint"
                    runat="server"
                    Text="طباعة التقرير"
                    CssClass="tf-btn-glass"
                    OnClientClick="window.print(); return false;"
                    UseSubmitBehavior="false" />
            </div>

            <!-- KPI CARDS -->
            <div class="tf-kpis">

                <div class="tf-kpi kpi-blue-wrap">
                    <div class="kpi-icon kpi-blue">
                        <i class="fas fa-users"></i>
                    </div>

                    <div class="kpi-body">
                        <span class="kpi-label">عدد المعلمين المحسوبين</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTeacherCount" runat="server" Text="0" />
                            <span class="kpi-unit">معلم/ة</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-purple-wrap">
                    <div class="kpi-icon kpi-purple">
                        <i class="fas fa-wallet"></i>
                    </div>

                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي المرتبات الشهرية</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalSalary" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-teal-wrap">
                    <div class="kpi-icon kpi-teal">
                        <i class="fas fa-book-open"></i>
                    </div>

                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي مرتب الحصص</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblLessonsSalary" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-orange-wrap">
                    <div class="kpi-icon kpi-orange">
                        <i class="fas fa-gift"></i>
                    </div>

                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي المكافآت والتسويات</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalAdjustments" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-red-wrap">
                    <div class="kpi-icon kpi-red">
                        <i class="fas fa-minus-circle"></i>
                    </div>

                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي الخصومات</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalDeductions" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-green-wrap">
                    <div class="kpi-icon kpi-green">
                        <i class="fas fa-coins"></i>
                    </div>

                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي صافي المرتبات</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalNet" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

            </div>

            <!-- INFO LINE -->
            <div class="tf-info-line">
                <i class="fas fa-info-circle"></i>
                <asp:Label ID="lblInfoLine" runat="server" />
            </div>

            <!-- PAYROLL TABLE -->
            <div class="tf-card">

                <div class="tf-card-header">
                    <div class="tf-card-header-icon">
                        <i class="fas fa-money-bill-wave"></i>
                    </div>

                    <div class="tf-card-header-text">
                        <h3>كشف احتساب الاستحقاق</h3>
                        <p>تفاصيل الاستحقاقات المالية لكل معلم في الفترة المحددة</p>
                    </div>

                    <span class="tf-count-pill">
                        <i class="fas fa-layer-group"></i>
                        <asp:Label ID="lblRowsCount" runat="server" Text="0" />
                        صف
                    </span>
                </div>

                <div class="tf-card-body">

                    <asp:UpdatePanel
                        ID="upDetails"
                        runat="server"
                        UpdateMode="Conditional">

                        <ContentTemplate>

                            <div class="tf-table-wrap">
                                <div class="tf-table-scroll">

                                    <asp:GridView
                                        ID="gvPayroll"
                                        runat="server"
                                        CssClass="tf-table"
                                        AutoGenerateColumns="false"
                                        EmptyDataText=""
                                        GridLines="None"
                                        ShowHeaderWhenEmpty="true"
                                        AllowPaging="true"
                                        PageSize="50"
                                        OnPageIndexChanging="gvPayroll_PageIndexChanging"
                                        DataKeyNames="Id">

                                        <Columns>

                                            <asp:TemplateField HeaderText="#">
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class="row-num">
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="المعلم / المعلمة">
                                                <ItemTemplate>
                                                    <span class="tf-teacher-name">
                                                        <%#: Eval("TeacherName") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="نوع التعاقد">
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class="badge-soft blue">
                                                        <i class="fas fa-id-badge"
                                                            style="font-size:9px;"></i>
                                                        <%#: Eval("EmploymentText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="المستحق">
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class="money pos">
                                                        <%# Eval("EarnedAmountText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="الخصومات">
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class="money neg">
                                                        <%# Eval("DeductionAmountText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="التعويضات">
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Eval("SubstitutionAmountClass") %>'>
                                                        <%# Eval("SubstitutionAmountText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="التسويات">
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class="money pos">
                                                        <%# Eval("AdjustmentAmountText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="الصافي">
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class="money big pos">
                                                        <%# Eval("NetAmountText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="الحالة">
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Eval("StatusClass") %>'>
                                                        <span class="dot"></span>
                                                        <%# Eval("StatusText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>

                                        <PagerStyle CssClass="table-pagination" />

                                    </asp:GridView>

                                </div>
                            </div>

                            <asp:Panel
                                ID="pnlEmpty"
                                runat="server"
                                Visible="false"
                                CssClass="tf-empty">

                                <div class="tf-empty-icon">
                                    <i class="fas fa-file-invoice"></i>
                                </div>

                                <h4>لا توجد بيانات مالية لعرضها</h4>

                                <p>
                                    اختر فترة مالية أخرى، أو تأكد من احتساب استحقاقات الفترة
                                    من صفحة الفترات المالية.
                                </p>

                            </asp:Panel>

                        </ContentTemplate>

                    </asp:UpdatePanel>

                    <asp:Label
                        ID="lblInfo"
                        runat="server"
                        CssClass="tf-info-msg neutral"
                        Visible="false" />

                </div>
            </div>

        </div>
    </div>

</asp:Content>