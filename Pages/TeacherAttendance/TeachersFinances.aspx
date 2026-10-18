<%@ Page Title="الشؤون المالية للمعلمين والمعلمات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TeachersFinances.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.TeachersFinances" %>

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
        .tf-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; flex-shrink: 0; }
        .tf-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }

        /* ---------- BODY ---------- */
        .tf-body { max-width: 1280px; margin: -56px auto 0; padding: 0 24px 40px; position: relative; z-index: 3; }

        /* ---------- FILTER BAR ---------- */
        .tf-filters { background: #ffffff; border: 1px solid #b7e4c9; border-radius: 18px; padding: 16px 20px; display: flex; align-items: flex-end; gap: 14px; flex-wrap: wrap; margin-bottom: 18px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
        .tf-filter { display: flex; flex-direction: column; gap: 5px; min-width: 190px; }
        .tf-filter label { font-size: 11.5px; font-weight: 800; color: #6c757d; }
        .tf-filter select { border: 1.5px solid #dee2e6; border-radius: 10px; padding: 8px 10px; font-size: 13px; font-family: inherit; background: #fff; }
        .tf-filter select:focus { outline: none; border-color: #1b7f4d; }

        /* ---------- KPI GRID ---------- */
        .tf-kpis { display: grid; grid-template-columns: repeat(5, 1fr); gap: 14px; margin-bottom: 18px; }
        .tf-kpi { background: #ffffff; border: 1px solid #b7e4c9; border-radius: 18px; padding: 18px 20px; display: flex; align-items: center; gap: 14px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); position: relative; overflow: hidden; transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1); }
        .tf-kpi::before { content: ''; position: absolute; top: 0; right: 0; width: 4px; height: 100%; background: currentColor; opacity: 0.85; }
        .tf-kpi:hover { transform: translateY(-4px); box-shadow: 0 16px 32px rgba(27,127,77,0.12); }
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
        .tf-info-line { display: flex; align-items: center; gap: 10px; padding: 12px 18px; background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%); border: 1.5px solid rgba(0,177,79,0.2); border-radius: 12px; margin-bottom: 18px; font-size: 12.5px; font-weight: 600; color: #1f3d33; }
        .tf-info-line i { width: 30px; height: 30px; border-radius: 9px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 13px; flex-shrink: 0; }

        /* ---------- CARD ---------- */
        .tf-card { background: #ffffff; border-radius: 20px; border: 1px solid #b7e4c9; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 18px; }
        .tf-card-header { display: flex; align-items: center; gap: 14px; padding: 20px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); }
        .tf-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; }
        .tf-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .tf-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .tf-count-pill { margin-right: auto; display: inline-flex; align-items: center; gap: 6px; padding: 6px 14px; border-radius: 999px; font-size: 12px; font-weight: 800; background: rgba(0,177,79,0.1); color: #007a33; border: 1px solid rgba(0,177,79,0.25); flex-shrink: 0; }
        .tf-card-body { padding: 16px; }

        /* ---------- TABLE ---------- */
        .tf-table-wrap { border-radius: 16px; overflow: hidden; border: 1px solid #e8eef0; background: #fff; box-shadow: 0 2px 8px rgba(0,0,0,0.02); }
        .tf-table-scroll { max-height: 780px; overflow-y: auto; overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .tf-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 12.5px; min-width: 780px; }
        .tf-table thead th { background: linear-gradient(180deg, #f8fdfa 0%, #edf7f1 100%); color: #1f3d33; font-size: 11px; font-weight: 800; padding: 14px 12px !important; text-align: right; border-bottom: 2px solid #c8e5d5; white-space: nowrap; position: sticky; top: 0; z-index: 5; letter-spacing: 0.4px; }
        .tf-table thead th:first-child { padding-right: 20px !important; }
        .tf-table thead th:last-child  { padding-left: 20px !important; }
        .tf-table tbody td { font-size: 12.5px; padding: 14px 12px !important; vertical-align: middle; border-bottom: 1px solid #f1f5f3; color: #212529; background: #fff; text-align: right; transition: background 0.15s ease; }
        .tf-table tbody tr:nth-child(even) td { background: #fbfdfc; }
        .tf-table tbody tr:hover td { background: #f0f8f3 !important; }
        .tf-table tbody tr:last-child td { border-bottom: none; }
        .tf-table tbody td:first-child { padding-right: 20px !important; }
        .tf-table tbody td:last-child  { padding-left: 20px !important; }

        /* ---------- MONEY ---------- */
        .money { font-family: 'Courier New', 'Consolas', monospace; font-variant-numeric: tabular-nums; font-weight: 700; color: #1f3d33; letter-spacing: -0.3px; direction: ltr; display: inline-block; font-size: 12.5px; }
        .money.pos { color: #007a33; }
        .money.neg { color: #a51c1c; }
        .money.big { font-size: 14px; font-weight: 800; }

        /* ---------- TEACHER ---------- */
        .tf-teacher-name { font-size: 13px; font-weight: 800; color: #1f3d33; display: block; }

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

        /* ---------- ROW NUM ---------- */
        .row-num { display: inline-flex; align-items: center; justify-content: center; width: 28px; height: 28px; border-radius: 9px; background: #f8faf9; color: #94a3a8; font-weight: 800; font-size: 11.5px; font-family: 'Courier New', monospace; border: 1px solid #eef1f3; }

        /* ---------- PAGINATION ---------- */
        .table-pagination { padding: 16px 20px !important; background: #fbfdfc; border-top: 1px solid #eef1f3; text-align: center; }
        .table-pagination a, .table-pagination span { display: inline-flex; align-items: center; justify-content: center; min-width: 36px; height: 36px; padding: 0 10px; margin: 0 3px; border-radius: 10px; font-size: 12.5px; font-weight: 800; color: #495057; background: #fff; border: 1.5px solid #e9ecef; text-decoration: none; transition: all 0.15s ease; font-family: 'Droid Arabic Kufi', sans-serif; vertical-align: middle; }
        .table-pagination a:hover { background: #f0f8f3; border-color: #00b14f; color: #007a33; text-decoration: none; transform: translateY(-1px); }
        .table-pagination span { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; border-color: transparent; box-shadow: 0 4px 10px rgba(0,177,79,0.3); }

        /* ---------- EMPTY ---------- */
        .tf-empty { text-align: center; padding: 50px 20px; }
        .tf-empty-icon { width: 72px; height: 72px; border-radius: 20px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 32px; margin-bottom: 16px; }
        .tf-empty h4 { color: #1f3d33; font-weight: 800; font-size: 15px; margin: 0 0 6px; }
        .tf-empty p { color: #6c757d; font-size: 12.5px; margin: 0; }

        .tf-note { display: block; font-size: 11.5px; color: #6c757d; line-height: 1.9; padding: 14px 18px 4px; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 1200px) { .tf-kpis { grid-template-columns: repeat(3, 1fr); } }
        @media (max-width: 992px) {
            .tf-hero-section { padding: 26px 20px 72px; }
            .tf-hero-titles h1 { font-size: 19px; }
            .tf-body { padding: 0 16px 40px; }
            .tf-kpis { grid-template-columns: repeat(2, 1fr); gap: 10px; }
            .tf-filter { min-width: 100%; }
        }
        @media (max-width: 768px) { .tf-table { font-size: 11px; } .tf-table thead th, .tf-table tbody td { padding: 10px 6px !important; } }
        @media (max-width: 576px) {
            .tf-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .tf-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .tf-hero-titles h1 { font-size: 16px; }
            .tf-btn-glass span { display: none; }
            .tf-kpis { grid-template-columns: 1fr; }
            .tf-card-header { padding: 16px; gap: 10px; }
            .tf-card-header-text p { display: none; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .tf-hero-section, .tf-btn-glass, .tf-filters, .no-print { display: none !important; }
            .tf-body { margin: 0 !important; padding: 0 !important; }
            .tf-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .tf-kpis { grid-template-columns: repeat(5, 1fr); }
            .tf-kpi { border: 1px solid #ddd !important; box-shadow: none !important; }
            .tf-table { font-size: 10.5px !important; }
            .tf-table-scroll { max-height: none !important; overflow: visible !important; }
            .tf-table thead th { position: static !important; }
            body { background: #fff !important; }
        }
        th {
            text-align: revert;
            padding-top: 20px;
            padding-bottom: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="tf-page" dir="rtl">

        <%-- HERO --%>
        <div class="tf-hero-section">
            <div class="tf-hero-content">
                <div class="tf-hero-left">
                    <div class="tf-hero-icon">
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="tf-hero-titles">
                        <h1>الشؤون المالية للمعلمين والمعلمات</h1>
                        <p>الراتب الشهري بالساعات، الخصومات، التسويات، وصافي المستحق لكل معلم ومعلمة</p>
                        <span class="tf-hero-chip">
                            <i class="fas fa-coins"></i>
                            <asp:Label ID="lblStage" runat="server" />
                        </span>
                    </div>
                </div>
                <a href="Home.aspx" class="tf-btn-glass">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة</span>
                </a>
            </div>
        </div>

        <%-- BODY --%>
        <div class="tf-body">

            <%-- FILTERS --%>
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
                <asp:Button ID="btnPrint" runat="server" Text="طباعة التقرير" CssClass="tf-btn-glass"
                    OnClientClick="window.print(); return false;" UseSubmitBehavior="false" />
            </div>

            <%-- KPI CARDS --%>
            <div class="tf-kpis">
                <div class="tf-kpi kpi-blue-wrap">
                    <div class="kpi-icon kpi-blue"><i class="fas fa-users"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">عدد المعلمين</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTeachersCount" runat="server" Text="0" />
                            <span class="kpi-unit">معلم/ة</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-teal-wrap">
                    <div class="kpi-icon kpi-teal"><i class="fas fa-clock"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">ساعات الفترة</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalHours" runat="server" Text="0" />
                            <span class="kpi-unit">ساعة</span>
                        </span>
                    </div>
                </div>

                <div class="tf-kpi kpi-green-wrap">
                    <div class="kpi-icon kpi-green"><i class="fas fa-coins"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي المستحق</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalEarned" runat="server" Text="0.00" />
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

                <div class="tf-kpi kpi-purple-wrap">
                    <div class="kpi-icon kpi-purple"><i class="fas fa-file-invoice-dollar"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">صافي المستحق</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalNet" runat="server" Text="0.00" />
                            <span class="kpi-unit">د.ل</span>
                        </span>
                    </div>
                </div>
            </div>

            <%-- INFO LINE --%>
            <div class="tf-info-line">
                <i class="fas fa-info-circle"></i>
                <asp:Label ID="lblInfo" runat="server" />
            </div>

            <%-- GRID --%>
            <div class="tf-card">
                <div class="tf-card-header">
                    <div class="tf-card-header-icon">
                        <i class="fas fa-money-bill-wave"></i>
                    </div>
                    <div class="tf-card-header-text">
                        <div>
                            <h3>التفاصيل المالية للمعلمين</h3>
                            <p>الراتب الشهري بالساعات مقابل المستحق الفعلي</p>
                        </div>
                    </div>
                    <span class="tf-count-pill">
                        <i class="fas fa-layer-group"></i>
                        <asp:Label ID="lblRowsCount" runat="server" Text="0" /> صف
                    </span>
                </div>
                <div class="tf-card-body">

                    <div class="tf-table-wrap">
                        <div class="tf-table-scroll">
                            <asp:GridView ID="gvFinances" runat="server"
                                CssClass="tf-table"
                                AutoGenerateColumns="false"
                                EmptyDataText=""
                                GridLines="None"
                                ShowHeaderWhenEmpty="true"
                                AllowPaging="true"
                                PageSize="25"
                                OnPageIndexChanging="gvFinances_PageIndexChanging">
                                <Columns>
                                    <%-- # --%>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="60px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="row-num"><%# Container.DataItemIndex + 1 %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- المعلم / المعلمة --%>
                                    <asp:TemplateField HeaderText="المعلم / المعلمة" ItemStyle-Width="250px">
                                        <ItemTemplate>
                                            <span class="tf-teacher-name"><%#: Eval("TeacherName") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- نوع التعاقد --%>
                                    <asp:TemplateField HeaderText="نوع التعاقد" ItemStyle-Width="180px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="badge-soft blue">
                                                <i class="fas fa-id-badge" style="font-size:9px;"></i>
                                                <%#: Eval("EmploymentText") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- المستحق --%>
                                    <asp:TemplateField HeaderText="المستحق" ItemStyle-Width="130px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="money pos"><%# Eval("EarnedAmountText") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- الخصومات --%>
                                    <asp:TemplateField HeaderText="الخصومات" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="money neg"><%# Eval("DeductionAmountText") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- التسويات --%>
                                    <asp:TemplateField HeaderText="التسويات" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
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

                                    <%-- الحالة --%>
                                    <asp:TemplateField HeaderText="الحالة" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class='<%# Eval("StatusClass") %>'>
                                                <span class="dot"></span>
                                                <%# Eval("StatusText") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- كشف --%>
                                    <asp:TemplateField HeaderText="كشف" ItemStyle-Width="110px" ItemStyle-CssClass="text-center">
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

                    <asp:Label ID="lblMessage" runat="server" />

                    <span class="tf-note">
                        <i class="fas fa-calculator"></i>
                        <b>طريقة الاحتساب:</b>
                        «المستحق» للمعلم الدائم والمشرف الدائم = الراتب الأساسي الشهري، ولمعلم الحصة = مجموع الحصص حسب سعر كل مادة ودقائق الحضور،
                        ثم تُطرح الخصومات وتُضاف التسويات للحصول على «الصافي».
                    </span>
                </div>
            </div>

        </div>
    </div>
</asp:Content>