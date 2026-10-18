<%@ Page Title="المعلمون الدائمون والمشرفون" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PermanentStaff.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.PermanentStaff" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        .tf-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; }

        /* ---------- HERO ---------- */
        .tf-hero-section { position: relative; background: linear-gradient(135deg,#0d6efd,#0a58ca); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .tf-hero-section::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%); pointer-events: none; }
        .tf-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .tf-hero-left { display: flex; align-items: center; gap: 20px; }
        .tf-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; }
        .tf-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; }
        .tf-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; max-width: 640px; }
        .tf-hero-chip { display: inline-flex; align-items: center; gap: 6px; padding: 5px 14px; border-radius: 999px; background: rgba(255,255,255,0.22); border: 1px solid rgba(255,255,255,0.3); color: #fff; font-weight: 700; font-size: 12.5px; margin-top: 8px; }
        .tf-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; flex-shrink: 0; }
        .tf-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }

        /* ---------- BODY ---------- */
        .tf-body { max-width: 1280px; margin: -56px auto 0; padding: 0 24px 40px; position: relative; z-index: 3; }

        /* ---------- FILTER BAR ---------- */
        .tf-filters { background: #ffffff; border: 1px solid #b7d9f0; border-radius: 18px; padding: 16px 20px; display: flex; align-items: flex-end; gap: 14px; flex-wrap: wrap; margin-bottom: 18px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
        .tf-filter { display: flex; flex-direction: column; gap: 5px; min-width: 220px; }
        .tf-filter label { font-size: 11.5px; font-weight: 800; color: #6c757d; }
        .tf-filter select.tf-select { border: 1.5px solid #dee2e6; border-radius: 10px; padding: 9px 12px; font-size: 13px; font-family: inherit; background: #fff; color: #1f3d33; font-weight: 700; min-width: 220px; cursor: pointer; transition: all 0.15s ease; }
        .tf-filter select.tf-select:focus { outline: none; border-color: #0d6efd; box-shadow: 0 0 0 3px rgba(13,110,253,0.10); }

        /* ---------- KPI GRID ---------- */
        .tf-kpis { display: grid; grid-template-columns: repeat(4, 1fr); gap: 14px; margin-bottom: 18px; }
        .tf-kpi { background: #ffffff; border: 1px solid #b7d9f0; border-radius: 18px; padding: 18px 20px; display: flex; align-items: center; gap: 14px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); position: relative; overflow: hidden; transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1); }
        .tf-kpi::before { content: ''; position: absolute; top: 0; right: 0; width: 4px; height: 100%; background: currentColor; opacity: 0.85; }
        .tf-kpi:hover { transform: translateY(-4px); box-shadow: 0 16px 32px rgba(13,110,253,0.12); }
        .tf-kpi .kpi-icon { width: 46px; height: 46px; border-radius: 14px; display: inline-flex; align-items: center; justify-content: center; font-size: 19px; color: #fff; flex-shrink: 0; }
        .tf-kpi .kpi-body { display: flex; flex-direction: column; gap: 4px; min-width: 0; }
        .tf-kpi .kpi-label { font-size: 11.5px; font-weight: 700; color: #6c757d; }
        .tf-kpi .kpi-value { font-size: 20px; font-weight: 800; color: #212529; line-height: 1; font-family: 'Courier New', monospace; }
        .tf-kpi .kpi-unit { font-size: 10.5px; font-weight: 700; color: #94a3a8; }

        .kpi-green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); }
        .kpi-blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); }
        .kpi-purple { background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%); }
        .kpi-red    { background: linear-gradient(135deg, #dc3545 0%, #f06572 100%); }
        .kpi-teal   { background: linear-gradient(135deg, #0c7a7a 0%, #35a8a8 100%); }

        .tf-kpi.kpi-green-wrap  { color: #00b14f; }
        .tf-kpi.kpi-blue-wrap   { color: #0d6efd; }
        .tf-kpi.kpi-purple-wrap { color: #6f42c1; }
        .tf-kpi.kpi-red-wrap    { color: #dc3545; }
        .tf-kpi.kpi-teal-wrap   { color: #0c7a7a; }

        /* ---------- INFO LINE ---------- */
        .tf-info-line { display: flex; align-items: center; gap: 10px; padding: 12px 18px; background: linear-gradient(135deg, #f8fbff 0%, #eef5ff 100%); border: 1.5px solid rgba(13,110,253,0.2); border-radius: 12px; margin-bottom: 18px; font-size: 12.5px; font-weight: 600; color: #1f3d33; }
        .tf-info-line i { width: 30px; height: 30px; border-radius: 9px; background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 13px; flex-shrink: 0; }

        /* ---------- CARD ---------- */
        .tf-card { background: #ffffff; border-radius: 20px; border: 1px solid #b7d9f0; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 18px; }
        .tf-card-header { display: flex; align-items: center; gap: 14px; padding: 20px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdff 100%); flex-wrap: wrap; }
        .tf-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; }
        .tf-card-header-icon.orange { background: linear-gradient(135deg, #e67e22 0%, #f39c12 100%); }
        .tf-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .tf-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .tf-count-pill { margin-right: auto; display: inline-flex; align-items: center; gap: 6px; padding: 6px 14px; border-radius: 999px; font-size: 12px; font-weight: 800; background: rgba(13,110,253,0.1); color: #0a58ca; border: 1px solid rgba(13,110,253,0.25); flex-shrink: 0; }
        .tf-card-body { padding: 16px; }

        /* ---------- TABLE ---------- */
        .tf-table-wrap { border-radius: 16px; overflow: hidden; border: 1px solid #e8eef0; background: #fff; box-shadow: 0 2px 8px rgba(0,0,0,0.02); }
        .tf-table-scroll { max-height: 780px; overflow-y: auto; overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .tf-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 12.5px; min-width: 900px; }
        .tf-table thead th { background: linear-gradient(180deg, #f8fbff 0%, #eef5ff 100%); color: #1f3d33; font-size: 11px; font-weight: 800; padding: 14px 12px !important; text-align: right; border-bottom: 2px solid #c5daf0; white-space: nowrap; position: sticky; top: 0; z-index: 5; letter-spacing: 0.4px; }
        .tf-table thead th:first-child { padding-right: 20px !important; }
        .tf-table thead th:last-child  { padding-left: 20px !important; }
        .tf-table tbody td { font-size: 12.5px; padding: 14px 12px !important; vertical-align: middle; border-bottom: 1px solid #f1f5f3; color: #212529; background: #fff; text-align: right; transition: background 0.15s ease; }
        .tf-table tbody tr:nth-child(even) td { background: #fbfdff; }
        .tf-table tbody tr:hover td { background: #eff5ff !important; }
        .tf-table tbody tr:last-child td { border-bottom: none; }
        .tf-table tbody td:first-child { padding-right: 20px !important; }
        .tf-table tbody td:last-child  { padding-left: 20px !important; }

        /* ---------- MONEY ---------- */
        .money { font-family: 'Courier New', 'Consolas', monospace; font-variant-numeric: tabular-nums; font-weight: 700; color: #1f3d33; letter-spacing: -0.3px; direction: ltr; display: inline-block; font-size: 13px; }
        .money.pos { color: #007a33; }
        .money.neg { color: #a51c1c; }
        .money.big { font-size: 13.5px; font-weight: 800; }

        /* ---------- TEACHER ---------- */
        .tf-teacher { display: flex; align-items: center; gap: 12px; font-weight: 700; color: #212529; min-width: 0; }
        .tf-avatar { width: 40px; height: 40px; border-radius: 12px; background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 15px; font-weight: 800; flex-shrink: 0; box-shadow: 0 4px 10px rgba(13,110,253,0.25); letter-spacing: 0.5px; font-family: 'Droid Arabic Kufi', sans-serif; }
        .tf-teacher-info { display: flex; flex-direction: column; gap: 3px; min-width: 0; text-align: right; }
        .tf-teacher-name { font-size: 13px; font-weight: 800; color: #1f3d33; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 190px; }
        .tf-teacher-meta { font-size: 10.5px; color: #94a3a8; font-weight: 700; direction: ltr; text-align: right; letter-spacing: 0.2px; }

        /* ---------- BADGE ---------- */
        .badge-soft { display: inline-flex; align-items: center; gap: 5px; padding: 5px 11px; border-radius: 8px; font-size: 11px; font-weight: 800; white-space: nowrap; letter-spacing: 0.1px; }
        .badge-soft.blue { background: rgba(13,110,253,0.10); color: #0a58ca; border: 1px solid rgba(13,110,253,0.22); }
        .badge-soft.green { background: rgba(0,177,79,0.10); color: #007a33; border: 1px solid rgba(0,177,79,0.22); }
        .badge-soft.purple { background: rgba(111,66,193,0.10); color: #6f42c1; border: 1px solid rgba(111,66,193,0.22); }

        /* ---------- ACTION BTN ---------- */
        .action-btn { display: inline-flex; align-items: center; justify-content: center; gap: 6px; padding: 8px 14px; border-radius: 10px; font-size: 11.5px; font-weight: 800; text-decoration: none; transition: all 0.2s ease; white-space: nowrap; border: 1.5px solid transparent; font-family: 'Droid Arabic Kufi', sans-serif; cursor: pointer; }
        .action-btn.primary { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); color: #fff; box-shadow: 0 3px 8px rgba(13,110,253,0.25); }
        .action-btn.primary:hover { transform: translateY(-1px); box-shadow: 0 6px 14px rgba(13,110,253,0.35); color: #fff; text-decoration: none; }

        /* ---------- ROW NUM ---------- */
        .row-num { display: inline-flex; align-items: center; justify-content: center; width: 28px; height: 28px; border-radius: 9px; background: #f8faf9; color: #94a3a8; font-weight: 800; font-size: 11.5px; font-family: 'Courier New', monospace; border: 1px solid #eef1f3; }

        /* ---------- ADD ITEM FORM ---------- */
        .tf-add-section { background: linear-gradient(180deg, #fbfdfc 0%, #f5faf7 100%); border: 1.5px dashed #b7e4c9; border-radius: 16px; padding: 18px 20px; margin-top: 18px; }
        .tf-add-title { display: flex; align-items: center; gap: 10px; margin-bottom: 14px; }
        .tf-add-title-icon { width: 36px; height: 36px; border-radius: 10px; background: linear-gradient(135deg, #e67e22 0%, #f39c12 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 15px; flex-shrink: 0; }
        .tf-add-title h6 { margin: 0; font-size: 14px; font-weight: 800; color: #1f3d33; }
        .tf-add-title p { margin: 3px 0 0; font-size: 11.5px; color: #6c757d; font-weight: 500; }
        .tf-add-grid { display: grid; grid-template-columns: 1.2fr 1.4fr 1fr 0.9fr 1.4fr auto; gap: 12px; align-items: end; }
        .tf-form-group { display: flex; flex-direction: column; gap: 5px; min-width: 0; }
        .tf-form-group label { font-size: 11.5px; font-weight: 800; color: #6c757d; }
        .tf-input { border: 1.5px solid #dee2e6; border-radius: 10px; padding: 9px 12px; font-size: 13px; font-family: inherit; background: #fff; color: #212529; width: 100%; transition: all 0.15s ease; }
        .tf-input:focus { outline: none; border-color: #0d6efd; box-shadow: 0 0 0 3px rgba(13,110,253,0.10); }
        .tf-submit { padding: 9px 22px; border-radius: 10px; border: none; background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); color: #fff; font-size: 13px; font-weight: 800; font-family: inherit; cursor: pointer; box-shadow: 0 4px 10px rgba(13,110,253,0.25); transition: all 0.2s ease; white-space: nowrap; }
        .tf-submit:hover { transform: translateY(-1px); box-shadow: 0 6px 14px rgba(13,110,253,0.35); }

        /* ---------- INFO MESSAGE ---------- */
        .tf-info-msg { display: inline-flex; align-items: center; gap: 8px; padding: 10px 16px; border-radius: 10px; font-size: 12.5px; font-weight: 700; margin-top: 14px; }
        .tf-info-msg.success { background: rgba(0,177,79,0.10); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .tf-info-msg.danger  { background: rgba(220,53,69,0.10); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }
        .tf-info-msg.neutral { background: #f8f9fa; color: #6c757d; border: 1px solid #e9ecef; }

        /* ---------- EMPTY ---------- */
        .tf-empty { text-align: center; padding: 50px 20px; }
        .tf-empty-icon { width: 72px; height: 72px; border-radius: 20px; background: linear-gradient(135deg, #e6f0ff 0%, #d0e0ff 100%); color: #0d6efd; display: inline-flex; align-items: center; justify-content: center; font-size: 32px; margin-bottom: 16px; }
        .tf-empty h4 { color: #1f3d33; font-weight: 800; font-size: 15px; margin: 0 0 6px; }
        .tf-empty p { color: #6c757d; font-size: 12.5px; margin: 0; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 1200px) { .tf-kpis { grid-template-columns: repeat(2, 1fr); } }
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
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="tf-page" dir="rtl">

        <%-- ================== HERO ================== --%>
        <div class="tf-hero-section">
            <div class="tf-hero-content">
                <div class="tf-hero-left">
                    <div class="tf-hero-icon">
                        <i class="fas fa-user-tie"></i>
                    </div>
                    <div class="tf-hero-titles">
                        <h1>المعلمون الدائمون والمشرفون</h1>
                        <p>رواتب شهرية ثابتة — مع إمكانية إضافة الخصومات والمكافآت والتسويات</p>
                        <span class="tf-hero-chip">
                            <i class="fas fa-layer-group"></i>
                            <asp:Label ID="lblCurrentStage" runat="server" />
                        </span>
                    </div>
                </div>
                <a href="Home.aspx" class="tf-btn-glass">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة</span>
                </a>
            </div>
        </div>

        <%-- ================== BODY ================== --%>
        <div class="tf-body">

            <%-- ================== FILTERS ================== --%>
            <div class="tf-filters no-print">
                <div class="tf-filter">
                    <label for="ddlPeriod">الفترة المالية</label>
                    <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="tf-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged" />
                </div>
                <div class="tf-filter">
                    <label for="ddlStaffType">نوع الكادر</label>
                    <asp:DropDownList ID="ddlStaffType" runat="server" CssClass="tf-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlStaffType_SelectedIndexChanged">
                        <asp:ListItem Value="all" Text="كل الأنواع" />
                        <asp:ListItem Value="Permanent" Text="معلم دائم - راتب شهري" />
                        <asp:ListItem Value="PermanentSupervisor" Text="مشرف دائم - راتب شهري" />
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnPrint" runat="server" Text="طباعة التقرير" CssClass="tf-btn-glass"
                    OnClientClick="window.print(); return false;" UseSubmitBehavior="false" />
            </div>

            <%-- ================== KPI CARDS ================== --%>
            <div class="tf-kpis">
                <div class="tf-kpi kpi-blue-wrap">
                    <div class="kpi-icon kpi-blue"><i class="fas fa-user-tie"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">معلمون دائمون</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblPermanentTeachersCount" runat="server" Text="0" />
                            <span class="kpi-unit">معلم/ة</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-teal-wrap">
                    <div class="kpi-icon kpi-teal"><i class="fas fa-user-shield"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">مشرفون دائمون</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblPermanentSupervisorsCount" runat="server" Text="0" />
                            <span class="kpi-unit">مشرف/ة</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-purple-wrap">
                    <div class="kpi-icon kpi-purple"><i class="fas fa-money-bill-wave"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي الرواتب الشهرية</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblMonthlySalaryTotal" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-green-wrap">
                    <div class="kpi-icon kpi-green"><i class="fas fa-coins"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي الصافي للفترة</span>
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

            <%-- ================== CARD: GRID ================== --%>
            <div class="tf-card">
                <div class="tf-card-header">
                    <div class="tf-card-header-icon">
                        <i class="fas fa-users"></i>
                    </div>
                    <div class="tf-card-header-text">
                        <h3>قائمة الدائمين والمشرفين</h3>
                        <p>تفاصيل استحقاق كل معلم دائم ومشرف في الفترة المالية المحددة</p>
                    </div>
                    <span class="tf-count-pill">
                        <i class="fas fa-layer-group"></i>
                        <asp:Label ID="lblRowsCount" runat="server" Text="0" /> صف
                    </span>
                </div>
                <div class="tf-card-body">

                    <div class="tf-table-wrap">
                        <div class="tf-table-scroll">
                            <asp:GridView ID="gvStaff" runat="server"
                                CssClass="tf-table"
                                AutoGenerateColumns="false"
                                EmptyDataText=""
                                GridLines="None"
                                ShowHeaderWhenEmpty="true"
                                AllowPaging="true"
                                PageSize="25"
                                OnPageIndexChanging="gvStaff_PageIndexChanging"
                                DataKeyNames="Id">
                                <Columns>
                                    <%-- # --%>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="60px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="row-num"><%# Container.DataItemIndex + 1 %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- الاسم --%>
                                    <asp:TemplateField HeaderText="الاسم" ItemStyle-Width="260px">
                                        <ItemTemplate>
                                            <div class="tf-teacher">
                                                <span class="tf-avatar"><%# Eval("Initials") %></span>
                                                <div class="tf-teacher-info">
                                                    <span class="tf-teacher-name"><%#: Eval("TeacherName") %></span>
                                                    <span class="tf-teacher-meta"><%#: Eval("EmployeeNoText") %></span>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- نوع الكادر --%>
                                    <asp:TemplateField HeaderText="نوع الكادر" ItemStyle-Width="180px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class='<%# Eval("EmploymentClass") %>'>
                                                <i class="fas fa-id-badge" style="font-size:9px;"></i>
                                                <%#: Eval("EmploymentText") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- الراتب الشهري --%>
                                    <asp:TemplateField HeaderText="الراتب الشهري" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="money pos"><%# Eval("BaseSalaryText") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- المستحق --%>
                                    <asp:TemplateField HeaderText="المستحق" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="money pos"><%# Eval("EarnedAmountText") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- الخصومات --%>
                                    <asp:TemplateField HeaderText="الخصومات" ItemStyle-Width="110px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="money neg"><%# Eval("DeductionAmountText") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- المكافآت/التسويات --%>
                                    <asp:TemplateField HeaderText="مكافآت/تسويات" ItemStyle-Width="130px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="money pos"><%# Eval("AdjustmentAmountText") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- الصافي --%>
                                    <asp:TemplateField HeaderText="الصافي" ItemStyle-Width="140px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="money big pos"><%# Eval("NetAmountText") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- كشف --%>
                                    <asp:TemplateField HeaderText="كشف" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <a class="action-btn primary"
                                               href='<%# Eval("StatementUrl") %>'>
                                                <i class="fas fa-file-invoice"></i>
                                                عرض الكشف
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
                            <i class="fas fa-user-tie"></i>
                        </div>
                        <h4>لا يوجد معلمون دائمون أو مشرفون</h4>
                        <p>لم يتم العثور على كادر دائم في هذه المرحلة أو في الفترة المحددة</p>
                    </asp:Panel>

                </div>
            </div>

            <%-- ================== CARD: FINANCIAL ITEM FORM ================== --%>
            <asp:Panel ID="pnlAddItem" runat="server" Visible="false" CssClass="tf-card">
                <div class="tf-card-header">
                    <div class="tf-card-header-icon orange">
                        <i class="fas fa-coins"></i>
                    </div>
                    <div class="tf-card-header-text">
                        <h3>إضافة خصم / مكافأة / تسوية مالية</h3>
                        <p>تُضاف مباشرة على استحقاق المعلم أو المشرف في الفترة المالية المحددة</p>
                    </div>
                </div>
                <div class="tf-card-body">
                    <div class="tf-add-section no-print">
                        <div class="tf-add-grid">
                            <div class="tf-form-group">
                                <label for="ddlItemTeacher">المعلم / المشرف</label>
                                <asp:DropDownList ID="ddlItemTeacher" runat="server" CssClass="tf-input">
                                    <asp:ListItem Value="" Text="-- اختر --" />
                                </asp:DropDownList>
                            </div>
                            <div class="tf-form-group">
                                <label for="ddlItemType">النوع</label>
                                <asp:DropDownList ID="ddlItemType" runat="server" CssClass="tf-input">
                                    <asp:ListItem Value="0" Text="مكافأة" />
                                    <asp:ListItem Value="1" Text="خصم" />
                                    <asp:ListItem Value="2" Text="بدل" />
                                    <asp:ListItem Value="3" Text="تصحيح مالي" />
                                    <asp:ListItem Value="4" Text="تسوية" />
                                </asp:DropDownList>
                            </div>
                            <div class="tf-form-group">
                                <label for="txtAmount">القيمة (د.ل)</label>
                                <asp:TextBox ID="txtAmount" runat="server" TextMode="Number"
                                    CssClass="tf-input" min="0" step="0.01" />
                            </div>
                            <div class="tf-form-group">
                                <label for="txtReason">السبب</label>
                                <asp:TextBox ID="txtReason" runat="server" CssClass="tf-input" />
                            </div>
                            <div class="tf-form-group">
                                <asp:Button ID="btnAddItem" runat="server" Text="إضافة"
                                    CssClass="tf-submit" OnClick="btnAddItem_Click" />
                            </div>
                        </div>

                        <asp:Label ID="lblItemInfo" runat="server"
                            CssClass="tf-info-msg neutral" Visible="false" />
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>
</asp:Content>