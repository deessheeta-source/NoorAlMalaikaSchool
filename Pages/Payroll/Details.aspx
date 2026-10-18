<%@ Page Title="تفاصيل الفترة المالية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Payroll.Details" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        .tf-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; }

        /* ---------- HERO ---------- */
        .tf-hero-section { position: relative; background: linear-gradient(135deg,#1b7f4d,#0f5f38); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .tf-hero-section::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%); pointer-events: none; }
        .tf-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .tf-hero-left { display: flex; align-items: center; gap: 20px; }
        .tf-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; }
        .tf-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; }
        .tf-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; max-width: 640px; }
        .tf-hero-chip { display: inline-flex; align-items: center; gap: 6px; padding: 5px 14px; border-radius: 999px; background: rgba(255,255,255,0.22); border: 1px solid rgba(255,255,255,0.3); color: #fff; font-weight: 700; font-size: 12.5px; margin-top: 8px; }
        .tf-hero-chip.badge-blue   { background: rgba(13,110,253,0.35); border-color: rgba(13,110,253,0.55); }
        .tf-hero-chip.badge-yellow { background: rgba(255,193,7,0.35); border-color: rgba(255,193,7,0.55); }
        .tf-hero-chip.badge-green  { background: rgba(76,201,120,0.35); border-color: rgba(76,201,120,0.55); }
        .tf-hero-chip.badge-red    { background: rgba(220,53,69,0.35); border-color: rgba(220,53,69,0.55); }
        .tf-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; flex-shrink: 0; }
        .tf-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }

        /* ---------- BODY ---------- */
        .tf-body { max-width: 1280px; margin: -56px auto 0; padding: 0 24px 40px; position: relative; z-index: 3; }

        /* ---------- FILTER BAR ---------- */
        .tf-filters { background: #ffffff; border: 1px solid #b7e4c9; border-radius: 18px; padding: 16px 20px; display: flex; align-items: flex-end; gap: 14px; flex-wrap: wrap; margin-bottom: 18px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
        .tf-filter { display: flex; flex-direction: column; gap: 5px; min-width: 220px; }
        .tf-filter label { font-size: 11.5px; font-weight: 800; color: #6c757d; }
        .tf-filter select.tf-select { border: 1.5px solid #dee2e6; border-radius: 10px; padding: 9px 12px; font-size: 13px; font-family: inherit; background: #fff; color: #1f3d33; font-weight: 700; min-width: 220px; cursor: pointer; transition: all 0.15s ease; }
        .tf-filter select.tf-select:focus { outline: none; border-color: #1b7f4d; box-shadow: 0 0 0 3px rgba(27,127,77,0.10); }

        /* ---------- KPI GRID (6 بطاقات) ---------- */
        .tf-kpis { display: grid; grid-template-columns: repeat(6, 1fr); gap: 12px; margin-bottom: 18px; }
        .tf-kpi { background: #ffffff; border: 1px solid #b7e4c9; border-radius: 18px; padding: 15px 12px; display: flex; align-items: center; gap: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); position: relative; overflow: hidden; transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1); }
        .tf-kpi::before { content: ''; position: absolute; top: 0; right: 0; width: 4px; height: 100%; background: currentColor; opacity: 0.85; }
        .tf-kpi:hover { transform: translateY(-4px); box-shadow: 0 16px 32px rgba(27,127,77,0.12); }
        .tf-kpi .kpi-icon { width: 38px; height: 38px; border-radius: 12px; display: inline-flex; align-items: center; justify-content: center; font-size: 16px; color: #fff; flex-shrink: 0; }
        .tf-kpi .kpi-body { display: flex; flex-direction: column; gap: 4px; min-width: 0; }
        .tf-kpi .kpi-label { font-size: 11px; font-weight: 700; color: #6c757d; line-height: 1.3; }
        .tf-kpi .kpi-value { font-size: 18px; font-weight: 800; color: #212529; line-height: 1; font-family: 'Courier New', monospace; }
        .tf-kpi .kpi-unit { font-size: 10px; font-weight: 700; color: #94a3a8; }

        .kpi-green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); }
        .kpi-blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); }
        .kpi-purple { background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%); }
        .kpi-red    { background: linear-gradient(135deg, #dc3545 0%, #f06572 100%); }
        .kpi-teal   { background: linear-gradient(135deg, #0c7a7a 0%, #35a8a8 100%); }
        .kpi-orange { background: linear-gradient(135deg, #e67e22 0%, #f39c12 100%); }

        .tf-kpi.kpi-green-wrap  { color: #00b14f; }
        .tf-kpi.kpi-blue-wrap   { color: #0d6efd; }
        .tf-kpi.kpi-purple-wrap { color: #6f42c1; }
        .tf-kpi.kpi-red-wrap    { color: #dc3545; }
        .tf-kpi.kpi-teal-wrap   { color: #0c7a7a; }
        .tf-kpi.kpi-orange-wrap { color: #e67e22; }

        /* ---------- INFO LINE ---------- */
        .tf-info-line { display: flex; align-items: center; gap: 10px; padding: 12px 18px; background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%); border: 1.5px solid rgba(0,177,79,0.2); border-radius: 12px; margin-bottom: 18px; font-size: 12.5px; font-weight: 600; color: #1f3d33; }
        .tf-info-line i { width: 30px; height: 30px; border-radius: 9px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 13px; flex-shrink: 0; }

        /* ---------- CARD ---------- */
        .tf-card { background: #ffffff; border-radius: 20px; border: 1px solid #b7e4c9; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 18px; }
        .tf-card-header { display: flex; align-items: center; gap: 14px; padding: 20px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .tf-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; }
        .tf-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .tf-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .tf-count-pill { margin-right: auto; display: inline-flex; align-items: center; gap: 6px; padding: 6px 14px; border-radius: 999px; font-size: 12px; font-weight: 800; background: rgba(0,177,79,0.1); color: #007a33; border: 1px solid rgba(0,177,79,0.25); flex-shrink: 0; }
        .tf-card-body { padding: 16px; }

        /* ---------- TABLE ---------- */
        .tf-table-wrap { border-radius: 16px; overflow: hidden; border: 1px solid #e8eef0; background: #fff; box-shadow: 0 2px 8px rgba(0,0,0,0.02); }
        .tf-table-scroll { max-height: 780px; overflow-y: auto; overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .tf-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 12.5px;  }
        .tf-table thead th { background: linear-gradient(180deg, #f8fdfa 0%, #edf7f1 100%); color: #1f3d33; font-size: 11px; font-weight: 800; padding: 14px 12px !important; text-align: right; border-bottom: 2px solid #c8e5d5; white-space: nowrap; position: sticky; top: 0; z-index: 5; letter-spacing: 0.4px; }
        .tf-table thead th:first-child { padding-right: 20px !important; }
        .tf-table thead th:last-child  { padding-left: 20px !important; }
        .tf-table tbody td { font-size: 12.5px; padding: 14px 12px !important; vertical-align: middle; border-bottom: 1px solid #f1f5f3; color: #212529; background: #fff; text-align: right; transition: background 0.15s ease; }
        .tf-table tbody tr:nth-child(even) td { background: #fbfdfc; }
        .tf-table tbody tr:hover td { background: #f0f8f3 !important; }
        .tf-table tbody tr:last-child td { border-bottom: none; }
        .tf-table tbody td:first-child { padding-right: 5px !important; }
        .tf-table tbody td:last-child  { padding-left: 20px !important; }

        /* ---------- MONEY ---------- */
        .money { font-variant-numeric: tabular-nums; font-weight: 700; color: #1f3d33; letter-spacing: -0.3px; direction: ltr; display: inline-block; font-size: 13px; }
        .money.pos { color: #007a33; }
        .money.neg { color: #a51c1c; }
        .money.neutral { color: #94a3a8; font-weight: 600; }
        .money.big { font-size: 13px; font-weight: 700; }

        /* ---------- TEACHER ---------- */
        .tf-teacher { display: flex; align-items: center; gap: 12px; font-weight: 700; color: #212529; min-width: 0; }
        .tf-avatar { width: 40px; height: 40px; border-radius: 12px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 15px; font-weight: 800; flex-shrink: 0; box-shadow: 0 4px 10px rgba(0,177,79,0.25); letter-spacing: 0.5px; font-family: 'Droid Arabic Kufi', sans-serif; }
        .tf-teacher-info { display: flex; flex-direction: column; gap: 3px; min-width: 0; text-align: right; }
        .tf-teacher-name { font-size: 13px; font-weight: 800; color: #1f3d33; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 190px; }
        .tf-teacher-meta { font-size: 10.5px; color: #94a3a8; font-weight: 700; direction: ltr; text-align: right; letter-spacing: 0.2px; }

        /* ---------- STATUS ---------- */
        .status-indicator { display: inline-flex; align-items: center; gap: 7px; padding: 6px 13px; border-radius: 999px; font-size: 11px; font-weight: 800; white-space: nowrap; letter-spacing: 0.1px; }
        .status-indicator .dot { width: 7px; height: 7px; border-radius: 50%; display: inline-block; flex-shrink: 0; }
        .status-indicator.green { background: rgba(0,177,79,0.10); color: #007a33; border: 1px solid rgba(0,177,79,0.22); }
        .status-indicator.green .dot { background: #00b14f; box-shadow: 0 0 0 3px rgba(0,177,79,0.20); }
        .status-indicator.gray { background: #f8f9fa; color: #6c757d; border: 1px solid #e9ecef; }
        .status-indicator.gray .dot { background: #adb5bd; }

        /* ---------- ACTION BTN ---------- */
        .action-btn { display: inline-flex; align-items: center; justify-content: center; gap: 6px; padding: 8px 14px; border-radius: 10px; font-size: 11.5px; font-weight: 800; text-decoration: none; transition: all 0.2s ease; white-space: nowrap; border: 1.5px solid transparent; font-family: 'Droid Arabic Kufi', sans-serif; cursor: pointer; }
        .action-btn.primary { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; box-shadow: 0 3px 8px rgba(0,177,79,0.25); }
        .action-btn.primary:hover { transform: translateY(-1px); box-shadow: 0 6px 14px rgba(0,177,79,0.35); color: #fff; text-decoration: none; }
        .action-btn.disabled { background: #f1f3f5; color: #adb5bd; cursor: not-allowed; border-color: #e9ecef; box-shadow: none; pointer-events: none; }

        /* ---------- BADGE ---------- */
        .badge-soft { display: inline-flex; align-items: center; gap: 5px; padding: 5px 11px; border-radius: 8px; font-size: 11px; font-weight: 800; white-space: nowrap; letter-spacing: 0.1px; }
        .badge-soft.blue { background: rgba(13,110,253,0.10); color: #0a58ca; border: 1px solid rgba(13,110,253,0.22); }
        .badge-soft.green { background: rgba(0,177,79,0.10); color: #007a33; border: 1px solid rgba(0,177,79,0.22); }
        .badge-soft.orange { background: rgba(230,126,34,0.10); color: #b45309; border: 1px solid rgba(230,126,34,0.22); }

        /* ---------- ROW NUM ---------- */
        .row-num { display: inline-flex; align-items: center; justify-content: center; width: 28px; height: 28px; border-radius: 9px; background: #f8faf9; color: #94a3a8; font-weight: 800; font-size: 11.5px; font-family: 'Courier New', monospace; border: 1px solid #eef1f3; }

        /* ---------- PAGINATION ---------- */
        .table-pagination { padding: 16px 20px !important; background: #fbfdfc; border-top: 1px solid #eef1f3; text-align: center; }
        .table-pagination a, .table-pagination span { display: inline-flex; align-items: center; justify-content: center; min-width: 36px; height: 36px; padding: 0 10px; margin: 0 3px; border-radius: 10px; font-size: 12.5px; font-weight: 800; color: #495057; background: #fff; border: 1.5px solid #e9ecef; text-decoration: none; transition: all 0.15s ease; font-family: 'Droid Arabic Kufi', sans-serif; vertical-align: middle; }
        .table-pagination a:hover { background: #f0f8f3; border-color: #00b14f; color: #007a33; text-decoration: none; transform: translateY(-1px); }
        .table-pagination span { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; border-color: transparent; box-shadow: 0 4px 10px rgba(0,177,79,0.3); }

        /* ---------- ADD ITEM FORM ---------- */
        .tf-add-section { background: linear-gradient(180deg, #fbfdfc 0%, #f5faf7 100%); border: 1.5px dashed #b7e4c9; border-radius: 16px; padding: 18px 20px; margin-top: 18px; }
        .tf-add-title { display: flex; align-items: center; gap: 10px; margin-bottom: 14px; }
        .tf-add-title-icon { width: 36px; height: 36px; border-radius: 10px; background: linear-gradient(135deg, #e67e22 0%, #f39c12 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 15px; flex-shrink: 0; }
        .tf-add-title h6 { margin: 0; font-size: 14px; font-weight: 800; color: #1f3d33; }
        .tf-add-grid { display: grid; grid-template-columns: 1.4fr 1fr 1fr 1.6fr auto; gap: 12px; align-items: end; }
        .tf-form-group { display: flex; flex-direction: column; gap: 5px; min-width: 0; }
        .tf-form-group label { font-size: 11.5px; font-weight: 800; color: #6c757d; }
        .tf-input { border: 1.5px solid #dee2e6; border-radius: 10px; padding: 9px 12px; font-size: 13px; font-family: inherit; background: #fff; color: #212529; width: 100%; transition: all 0.15s ease; }
        .tf-input:focus { outline: none; border-color: #1b7f4d; box-shadow: 0 0 0 3px rgba(27,127,77,0.10); }
        .tf-submit { padding: 9px 22px; border-radius: 10px; border: none; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; font-size: 13px; font-weight: 800; font-family: inherit; cursor: pointer; box-shadow: 0 4px 10px rgba(0,177,79,0.25); transition: all 0.2s ease; white-space: nowrap; }
        .tf-submit:hover { transform: translateY(-1px); box-shadow: 0 6px 14px rgba(0,177,79,0.35); }

        /* ---------- INFO MESSAGE ---------- */
        .tf-info-msg { display: inline-flex; align-items: center; gap: 8px; padding: 10px 16px; border-radius: 10px; font-size: 12.5px; font-weight: 700; margin-top: 14px; }
        .tf-info-msg.success { background: rgba(0,177,79,0.10); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .tf-info-msg.danger  { background: rgba(220,53,69,0.10); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }
        .tf-info-msg.neutral { background: #f8f9fa; color: #6c757d; border: 1px solid #e9ecef; }

        /* ---------- EMPTY ---------- */
        .tf-empty { text-align: center; padding: 50px 20px; }
        .tf-empty-icon { width: 72px; height: 72px; border-radius: 20px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 32px; margin-bottom: 16px; }
        .tf-empty h4 { color: #1f3d33; font-weight: 800; font-size: 15px; margin: 0 0 6px; }
        .tf-empty p { color: #6c757d; font-size: 12.5px; margin: 0; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 1400px) { .tf-kpis { grid-template-columns: repeat(3, 1fr); } }
        @media (max-width: 992px) {
            .tf-hero-section { padding: 26px 20px 72px; }
            .tf-hero-titles h1 { font-size: 19px; }
            .tf-body { padding: 0 16px 40px; }
            .tf-kpis { grid-template-columns: 1fr 1fr; gap: 10px; }
            .tf-add-grid { grid-template-columns: 1fr 1fr; }
            .tf-filter { min-width: 100%; }
        }
        @media (max-width: 768px) { .tf-table { font-size: 11px; } .tf-table thead th, .tf-table tbody td { padding: 10px 6px !important; } }
        @media (max-width: 576px) {
            .tf-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .tf-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .tf-hero-titles h1 { font-size: 16px; }
            .tf-btn-glass span { display: none; }
            .tf-kpis { grid-template-columns: 1fr; }
            .tf-add-grid { grid-template-columns: 1fr; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .tf-hero-section, .tf-btn-glass, .tf-filters, .tf-add-section, .no-print { display: none !important; }
            .tf-body { margin: 0 !important; padding: 0 !important; }
            .tf-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .tf-table-scroll { max-height: none !important; overflow: visible !important; }
            .tf-table thead th { position: static !important; }
        }
        th {
            text-align: revert;
            padding-bottom: 15px;
            padding-top: 15px;
            background-color: #e5f0e9;
            padding-right: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="tf-page" dir="rtl">

        <%-- ================== HERO ================== --%>
        <div class="tf-hero-section">
            <div class="tf-hero-content">
                <div class="tf-hero-left">
                    <div class="tf-hero-icon">
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="tf-hero-titles">
                        <h1>تفاصيل الفترة المالية</h1>
                        <p>كشف احتساب الاستحقاق لكل معلم في الفترة المالية المحددة</p>
                        <span class="tf-hero-chip">
                            <i class="fas fa-calendar-alt"></i>
                            الفترة: <asp:Label ID="lblPeriodTitle" runat="server" />
                            &nbsp;·&nbsp;
                            الحالة: <asp:Label ID="lblStatus" runat="server" />
                        </span>
                    </div>
                </div>
            </div>
        </div>

        <%-- ================== BODY ================== --%>
        <div class="tf-body">

            <%-- ================== FILTERS ================== --%>
            <div class="tf-filters no-print">
                <div class="tf-filter">
                    <label for="ddlPeriod">الفترة المالية</label>
                    <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="tf-select" AppendDataBoundItems="true"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="-- اختر الفترة --" />
                    </asp:DropDownList>
                </div>
                <div class="tf-filter">
                    <label for="ddlStageFilter">المرحلة الدراسية</label>
                    <asp:DropDownList ID="ddlStageFilter" runat="server" CssClass="tf-select" AppendDataBoundItems="true"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlStageFilter_SelectedIndexChanged">
                        <asp:ListItem Value="Basic" Text="المرحلة الأساسية" />
                        <asp:ListItem Value="Secondary" Text="المرحلة الثانوية" />
                        <asp:ListItem Value="all" Text="كل المراحل" />
                    </asp:DropDownList>
                </div>
                <div class="tf-filter">
                    <label for="ddlEmploymentFilter">نوع التعاقد</label>
                    <asp:DropDownList ID="ddlEmploymentFilter" runat="server" CssClass="tf-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlEmploymentFilter_SelectedIndexChanged">
                        <asp:ListItem Value="all" Text="كل الأنواع" />
                        <asp:ListItem Value="PerLesson" Text="معلم بالحصة" />
                        <asp:ListItem Value="Permanent" Text="معلم براتب شهري" />
                        <asp:ListItem Value="PermanentSupervisor" Text="مشرف براتب شهري" />
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnPrint" runat="server" Text="طباعة التقرير" CssClass="tf-btn-glass"
                    OnClientClick="window.print(); return false;" UseSubmitBehavior="false" />
            </div>

            <%-- ================== KPI CARDS (6 بطاقات) ================== --%>
            <div class="tf-kpis">
                <div class="tf-kpi kpi-blue-wrap">
                    <div class="kpi-icon kpi-blue"><i class="fas fa-users"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">عدد المعلمين المحسوبين</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTeacherCount" runat="server" Text="0" />
                            <span class="kpi-unit">معلم/ة</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-purple-wrap">
                    <div class="kpi-icon kpi-purple"><i class="fas fa-wallet"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي المرتبات الشهرية</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalSalary" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-teal-wrap">
                    <div class="kpi-icon kpi-teal"><i class="fas fa-book-open"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي مرتب الحصص</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblLessonsSalary" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-orange-wrap">
                    <div class="kpi-icon kpi-orange"><i class="fas fa-gift"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي المكافآت والتسويات</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalAdjustments" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-red-wrap">
                    <div class="kpi-icon kpi-red"><i class="fas fa-minus-circle"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي الخصومات</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalDeductions" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-green-wrap">
                    <div class="kpi-icon kpi-green"><i class="fas fa-coins"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي صافي المرتبات</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalNet" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>
            </div>

            <%-- ================== INFO LINE ================== --%>
            <div class="tf-info-line">
                <i class="fas fa-info-circle"></i>
                <asp:Label ID="lblInfoLine" runat="server" />
            </div>

            <%-- ================== CARD WITH GRID + ADD FORM ================== --%>
            <div class="tf-card">
                <div class="tf-card-header">
                    <div class="tf-card-header-icon">
                        <i class="fas fa-money-bill-wave"></i>
                    </div>
                    <div class="tf-card-header-text">
                        <h3>كشف احتساب الاستحقاق</h3>
                        <p>تفاصيل كل معلم في هذه الفترة المالية</p>
                    </div>
                    <span class="tf-count-pill">
                        <i class="fas fa-layer-group"></i>
                        <asp:Label ID="lblRowsCount" runat="server" Text="0" /> صف
                    </span>
                </div>
                <div class="tf-card-body">

                    <asp:UpdatePanel ID="upDetails" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="tf-table-wrap">
                                <div class="tf-table-scroll">
                                    <asp:GridView ID="gvPayroll" runat="server"
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
                                            <%-- # --%>
                                            <asp:TemplateField HeaderText="#"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="row-num"><%# Container.DataItemIndex + 1 %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- المعلم / المعلمة --%>
                                            <asp:TemplateField HeaderText="المعلم / المعلمة" >
                                                <ItemTemplate>
                                                    <span class="tf-teacher-name"><%#: Eval("TeacherName") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- نوع التعاقد --%>
                                            <asp:TemplateField HeaderText="نوع التعاقد"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="badge-soft blue">
                                                        <i class="fas fa-id-badge" style="font-size:9px;"></i>
                                                        <%#: Eval("EmploymentText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- المستحق --%>
                                            <asp:TemplateField HeaderText="المستحق"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="money pos"><%# Eval("EarnedAmountText") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- الخصومات --%>
                                            <asp:TemplateField HeaderText="الخصومات"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="money neg"><%# Eval("DeductionAmountText") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- ⭐ التعويضات --%>
                                            <asp:TemplateField HeaderText="التعويضات"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class='<%# Eval("SubstitutionAmountClass") %>'><%# Eval("SubstitutionAmountText") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- التسويات --%>
                                            <asp:TemplateField HeaderText="التسويات"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="money pos"><%# Eval("AdjustmentAmountText") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- الصافي --%>
                                            <asp:TemplateField HeaderText="الصافي"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="money big pos"><%# Eval("NetAmountText") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- الحالة --%>
                                            <asp:TemplateField HeaderText="الحالة"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class='<%# Eval("StatusClass") %>'>
                                                        <span class="dot"></span>
                                                        <%# Eval("StatusText") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- كشف --%>
                                            <asp:TemplateField HeaderText="كشف"  ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <a class='<%# Eval("ActionClass") %>'
                                                       href='<%# Eval("StatementUrl") %>'>
                                                        <i class="fas fa-file-invoice"></i>
                                                        عرض
                                                    </a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="table-pagination" />
                                    </asp:GridView>
                                </div>
                            </div>

                            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="tf-empty">
                                <div class="tf-empty-icon">
                                    <i class="fas fa-file-invoice"></i>
                                </div>
                                <h4>لا توجد بيانات مالية لعرضها</h4>
                                <p>اختر فترة مالية أخرى، أو احسب استحقاقات الفترة من صفحة الفترات المالية أولاً</p>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <%-- ADD ITEM FORM --%>
                    <div id="divItems" runat="server" class="tf-add-section no-print">
                        <div class="tf-add-title">
                            <div class="tf-add-title-icon"><i class="fas fa-coins"></i></div>
                            <h6>إضافة مكافأة / خصم / تصحيح مالي</h6>
                        </div>

                        <div class="tf-add-grid">
                            <div class="tf-form-group">
                                <label>المعلم</label>
                                <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="tf-input" AppendDataBoundItems="true">
                                    <asp:ListItem Value="" Text="-- اختر --" />
                                </asp:DropDownList>
                            </div>
                            <div class="tf-form-group">
                                <label>النوع</label>
                                <asp:DropDownList ID="ddlItemType" runat="server" CssClass="tf-input">
                                    <asp:ListItem Value="0" Text="مكافأة" />
                                    <asp:ListItem Value="1" Text="خصم إضافي" />
                                    <asp:ListItem Value="2" Text="بدل" />
                                    <asp:ListItem Value="3" Text="تصحيح مالي" />
                                    <asp:ListItem Value="4" Text="تسوية" />
                                </asp:DropDownList>
                            </div>
                            <div class="tf-form-group">
                                <label>القيمة (د.ل)</label>
                                <asp:TextBox ID="txtAmount" runat="server" TextMode="Number" CssClass="tf-input" min="0" step="0.01" />
                            </div>
                            <div class="tf-form-group">
                                <label>السبب</label>
                                <asp:TextBox ID="txtReason" runat="server" CssClass="tf-input" />
                            </div>
                            <div class="tf-form-group">
                                <asp:Button ID="btnAddItem" runat="server" Text="إضافة" CssClass="tf-submit" OnClick="btnAddItem_Click" />
                            </div>
                        </div>
                    </div>

                    <asp:Label ID="lblInfo" runat="server" CssClass="tf-info-msg neutral" Visible="false" />

                </div>
            </div>

        </div>
    </div>
</asp:Content>