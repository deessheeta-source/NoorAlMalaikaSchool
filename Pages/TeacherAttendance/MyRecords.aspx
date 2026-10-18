<%@ Page Title="سجل الساعات والتعويضيات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyRecords.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.MyRecords" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — My Records
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
            background: linear-gradient(135deg,#cf4553,#cd3847);
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

        .ta-hero-chip {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 5px 14px;
            border-radius: 999px;
            background: rgba(255,255,255,0.22);
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255,255,255,0.3);
            color: #fff;
            font-weight: 700;
            font-size: 12.5px;
            margin-top: 8px;
        }
        .ta-hero-chip i { font-size: 11px; }

        .ta-btn-glass {
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
            flex-shrink: 0;
        }
        .ta-btn-glass:hover {
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

        /* ---------- KPI GRID ---------- */
        .ta-kpis {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 14px;
            margin-bottom: 20px;
        }
        .ta-kpi {
            background: #ffffff;
            border: 1px solid #99f6c3;
            border-radius: 18px;
            padding: 18px 20px;
            display: flex;
            align-items: center;
            gap: 14px;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 4px 12px rgba(0,0,0,0.03);
            position: relative;
            overflow: hidden;
        }
        .ta-kpi::before {
            content: '';
            position: absolute;
            top: 0;
            right: 0;
            width: 4px;
            height: 100%;
            background: currentColor;
            opacity: 0.85;
        }
        .ta-kpi:hover {
            transform: translateY(-4px);
            box-shadow: 0 16px 32px rgba(0,177,79,0.1);
        }
        .ta-kpi .kpi-icon {
            width: 48px;
            height: 48px;
            border-radius: 14px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            color: #fff;
            flex-shrink: 0;
            transition: transform 0.3s ease;
        }
        .ta-kpi:hover .kpi-icon {
            transform: scale(1.08) rotate(-4deg);
        }
        .ta-kpi .kpi-body {
            display: flex;
            flex-direction: column;
            gap: 4px;
            min-width: 0;
        }
        .ta-kpi .kpi-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
        }
        .ta-kpi .kpi-value {
            font-size: 22px;
            font-weight: 800;
            color: #212529;
            line-height: 1;
            font-family: 'Courier New', monospace;
            letter-spacing: -0.5px;
        }
        .ta-kpi .kpi-unit {
            font-size: 11px;
            color: #94a3a8;
            font-weight: 700;
            font-family: 'Droid Arabic Kufi', sans-serif;
            margin-right: 3px;
        }

        .kpi-green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .kpi-yellow { background: linear-gradient(135deg, #fd7e14 0%, #ffb066 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .kpi-purple { background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .kpi-blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }

        .ta-kpi.kpi-green-wrap  { color: #00b14f; }
        .ta-kpi.kpi-yellow-wrap { color: #fd7e14; }
        .ta-kpi.kpi-purple-wrap { color: #6f42c1; }
        .ta-kpi.kpi-blue-wrap   { color: #0d6efd; }

        /* ---------- INFO LINE ---------- */
        .ta-info-line {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 12px 18px;
            background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 12px;
            margin-bottom: 20px;
            font-size: 12.5px;
            font-weight: 600;
            color: #1f3d33;
        }
        .ta-info-line i {
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
        .ta-info-line .text-muted { color: #1f3d33 !important; }

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
        .ta-card-header-icon.purple {
            background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%);
            box-shadow: 0 6px 16px rgba(111,66,193,0.3);
        }
        .ta-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 0;
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
        .ta-count-pill {
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
        .ta-count-pill.purple {
            background: rgba(111,66,193,0.1);
            color: #5a2fa0;
            border-color: rgba(111,66,193,0.25);
        }

        .ta-card-body {
            padding: 16px;
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

        /* Badges — soft style */
        .badge-soft {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
        }
        .badge-soft.green {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .badge-soft.yellow {
            background: rgba(253,126,20,0.12);
            color: #b45309;
            border: 1px solid rgba(253,126,20,0.25);
        }
        .badge-soft.red {
            background: rgba(220,53,69,0.12);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .badge-soft.gray {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }

        /* ---------- EMPTY STATE ---------- */
        .ta-empty {
            text-align: center;
            padding: 50px 20px;
        }
        .ta-empty-icon {
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
        .ta-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 15px;
            margin: 0 0 6px;
        }
        .ta-empty p {
            color: #6c757d;
            font-size: 12.5px;
            margin: 0;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .ta-hero-section { padding: 26px 20px 72px; }
            .ta-hero-titles h1 { font-size: 19px; }
            .ta-body { padding: 0 16px 40px; }
            .ta-kpis { grid-template-columns: repeat(2, 1fr); gap: 10px; }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .ta-table { font-size: 11px; min-width: 860px; }
            .ta-table thead th,
            .ta-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .ta-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .ta-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .ta-hero-titles h1 { font-size: 16px; }
            .ta-hero-titles p { font-size: 12px; }
            .ta-btn-glass { padding: 8px 14px; font-size: 12px; }
            .ta-btn-glass span { display: none; }

            .ta-kpis { grid-template-columns: 1fr; }
            .ta-kpi { padding: 14px 16px; }
            .ta-kpi .kpi-icon { width: 42px; height: 42px; font-size: 18px; }
            .ta-kpi .kpi-value { font-size: 19px; }

            .ta-card-header { padding: 16px; gap: 10px; }
            .ta-card-header-icon { width: 38px; height: 38px; font-size: 16px; }
            .ta-card-header-text h3 { font-size: 14px; }
            .ta-card-header-text p { display: none; }
            .ta-count-pill { font-size: 11px; padding: 5px 10px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .ta-hero-section,
            .ta-btn-glass,
            .no-print { display: none !important; }
            .ta-body { margin: 0 !important; padding: 0 !important; }
            .ta-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .ta-kpis { grid-template-columns: repeat(4, 1fr); }
            .ta-kpi { border: 1px solid #ddd !important; box-shadow: none !important; }
            .ta-table { font-size: 10.5px !important; }
            .table-responsive { overflow: visible !important; }
            body { background: #fff !important; }
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
                        <i class="fas fa-stopwatch"></i>
                    </div>
                    <div class="ta-hero-titles">
                        <h1>سجل الحضور والغياب والتعويضيات</h1>
                        <p>عرض تفصيلي لسجل الساعات والتعويضيات الخاصة بك</p>
                        <span class="ta-hero-chip">
                            <i class="fas fa-layer-group"></i>
                            <asp:Label ID="lblStage" runat="server" />
                        </span>
                    </div>
                </div>
                <a href="Home.aspx" class="ta-btn-glass">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة</span>
                </a>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="ta-body">

            <!-- ================== KPI CARDS ================== -->
            <div class="ta-kpis">

                <div class="ta-kpi kpi-green-wrap">
                    <div class="kpi-icon kpi-green"><i class="fas fa-clock"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي ساعات الحضور</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalPresentHours" runat="server" Text="0" />
                            <span class="kpi-unit">ساعة</span>
                        </span>
                    </div>
                </div>

                <div class="ta-kpi kpi-yellow-wrap">
                    <div class="kpi-icon kpi-yellow"><i class="fas fa-user-slash"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">إجمالي ساعات الغياب</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblTotalAbsentHours" runat="server" Text="0" />
                            <span class="kpi-unit">ساعة</span>
                        </span>
                    </div>
                </div>

                <div class="ta-kpi kpi-purple-wrap">
                    <div class="kpi-icon kpi-purple"><i class="fas fa-exchange-alt"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">عدد التعويضيات</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblSubstitutionsCount" runat="server" Text="0" />
                            <span class="kpi-unit">تعويضية</span>
                        </span>
                    </div>
                </div>

                <div class="ta-kpi kpi-blue-wrap">
                    <div class="kpi-icon kpi-blue"><i class="fas fa-list-ol"></i></div>
                    <div class="kpi-body">
                        <span class="kpi-label">عدد الحصص المسجلة</span>
                        <span class="kpi-value">
                            <asp:Label ID="lblLessonsCount" runat="server" Text="0" />
                            <span class="kpi-unit">حصة</span>
                        </span>
                    </div>
                </div>

            </div>

            <!-- ================== INFO LINE ================== -->
            <div class="ta-info-line">
                <i class="fas fa-info-circle"></i>
                <asp:Label ID="lblInfo" runat="server" />
            </div>

            <!-- ================== SECTION 1: ATTENDANCE ================== -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon">
                        <i class="fas fa-clock"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>سجل الحضور والغياب بالساعات</h3>
                            <p>عرض تجميعي لكل يوم على حِدة</p>
                        </div>
                    </div>
                    <span class="ta-count-pill">
                        <i class="fas fa-layer-group"></i>
                        <asp:Label ID="lblAttendanceCount" runat="server" Text="0" /> يوم
                    </span>
                </div>
                <div class="ta-card-body">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvAttendance" runat="server"
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

                                <asp:BoundField DataField="Date" HeaderText="التاريخ"
                                    DataFormatString="{0:yyyy-MM-dd}" ItemStyle-CssClass="text-center" />

                                <asp:BoundField DataField="LessonsCount" HeaderText="الحصص"
                                    ItemStyle-CssClass="text-center" />

                                <asp:TemplateField HeaderText="دقائق الحضور" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="badge-soft green">
                                            <i class="fas fa-check" style="font-size:10px;"></i>
                                            <%# Eval("PresentMinutes") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="PresentHours" HeaderText="ساعات الحضور"
                                    DataFormatString="{0:0.##}" ItemStyle-CssClass="text-center" />

                                <asp:TemplateField HeaderText="دقائق الغياب" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="badge-soft yellow">
                                            <i class="fas fa-hourglass-half" style="font-size:10px;"></i>
                                            <%# Eval("AbsentMinutes") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="AbsentHours" HeaderText="ساعات الغياب"
                                    DataFormatString="{0:0.##}" ItemStyle-CssClass="text-center" />

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-Width="150px">
                                    <ItemTemplate>
                                        <span class='<%# Eval("StatusClass") %>'>
                                            <%# Eval("StatusText") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الاعتماد" ItemStyle-Width="130px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# (bool)Eval("AllApproved") ? "badge-soft green" : "badge-soft gray" %>'>
                                            <i class='<%# (bool)Eval("AllApproved") ? "fas fa-check-circle" : "fas fa-lock" %>' style="font-size:10px;"></i>
                                            <%# (bool)Eval("AllApproved") ? "معتمد" : "غير معتمد" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlAttendanceEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon">
                            <i class="fas fa-clock"></i>
                        </div>
                        <h4>لا توجد سجلات حضور أو غياب</h4>
                        <p>لم يتم تسجيل أي حصة لهذه المرحلة بعد</p>
                    </asp:Panel>

                </div>
            </div>

            <!-- ================== SECTION 2: SUBSTITUTIONS ================== -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon purple">
                        <i class="fas fa-exchange-alt"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>التعويضيات الخاصة بي</h3>
                            <p>تغطية الحصص لمعلمين آخرين</p>
                        </div>
                    </div>
                    <span class="ta-count-pill purple">
                        <i class="fas fa-sync"></i>
                        <asp:Label ID="lblSubstitutionsTotal" runat="server" Text="0" /> تعويضية
                    </span>
                </div>
                <div class="ta-card-body">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvSubstitutions" runat="server"
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

                                <asp:BoundField DataField="Date" HeaderText="التاريخ"
                                    DataFormatString="{0:yyyy-MM-dd}" ItemStyle-CssClass="text-center" />

                                <asp:BoundField DataField="PeriodNumber" HeaderText="الحصة"
                                    ItemStyle-CssClass="text-center" />

                                <asp:BoundField DataField="ClassName" HeaderText="الفصل" />
                                <asp:BoundField DataField="SubjectName" HeaderText="المادة" />

                                <asp:TemplateField HeaderText="المعلم الأصلي">
                                    <ItemTemplate>
                                        <span class="ta-teacher" style="font-weight:600;">
                                            <span class="ta-avatar" style="width:28px;height:28px;font-size:12px;">
                                                <i class="fas fa-chalkboard-teacher"></i>
                                            </span>
                                            <%#: Eval("OriginalTeacher") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="دوري" ItemStyle-Width="130px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# Eval("Role").ToString() == "أصلي" ? "badge-soft red" : "badge-soft green" %>'>
                                            <i class='<%# Eval("Role").ToString() == "أصلي" ? "fas fa-user" : "fas fa-user-check" %>' style="font-size:10px;"></i>
                                            <%#: Eval("Role") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-Width="150px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# (bool)Eval("IsApproved") ? "badge-soft green" : "badge-soft yellow" %>'>
                                            <i class='<%# (bool)Eval("IsApproved") ? "fas fa-check-circle" : "fas fa-hourglass-half" %>' style="font-size:10px;"></i>
                                            <%# (bool)Eval("IsApproved") ? "معتمد" : "قيد الاعتماد" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlSubstitutionsEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon" style="background: linear-gradient(135deg, #eee6f8 0%, #ddd0f0 100%); color: #6f42c1;">
                            <i class="fas fa-exchange-alt"></i>
                        </div>
                        <h4>لا توجد تعويضيات</h4>
                        <p>لم يتم تسجيل أي تعويضية خاصة بك بعد</p>
                    </asp:Panel>

                </div>
            </div>

        </div>
    </div>
</asp:Content>