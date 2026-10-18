<%@ Page Title="كشف استحقاق شهري" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Statement.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Payroll.Statement" %>

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

        /* ---------- INFO CARDS ---------- */
        .tf-info-cards { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; margin-bottom: 18px; }
        .tf-info-card { background: #ffffff; border: 1px solid #b7e4c9; border-radius: 18px; padding: 18px 20px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
        .tf-info-card h6 { margin: 0 0 12px; font-size: 13px; font-weight: 800; color: #1f3d33; display: flex; align-items: center; gap: 8px; }
        .tf-info-card h6 i { width: 26px; height: 26px; border-radius: 8px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 11px; }
        .tf-info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px 20px; }
        .tf-info-row { display: flex; flex-direction: column; gap: 3px; }
        .tf-info-label { font-size: 11px; color: #6c757d; font-weight: 700; }
        .tf-info-value { font-size: 13px; font-weight: 800; color: #1f3d33; }
        .tf-info-value.green { color: #007a33; }
        .tf-info-value.red { color: #a51c1c; }

        .tf-net-row { display: flex; justify-content: space-between; align-items: center; padding-top: 10px; margin-top: 10px; border-top: 1.5px dashed #b7e4c9; }
        .tf-net-row .tf-info-label { font-size: 13px; color: #1f3d33; font-weight: 800; }
        .tf-net-row .tf-info-value { font-size: 20px; font-family: 'Courier New', monospace; color: #6f42c1; }

        /* ---------- CARD ---------- */
        .tf-card { background: #ffffff; border-radius: 20px; border: 1px solid #b7e4c9; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 18px; }
        .tf-card-header { display: flex; align-items: center; gap: 14px; padding: 20px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .tf-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; }
        .tf-card-header-icon.orange { background: linear-gradient(135deg, #e67e22 0%, #f39c12 100%); }
        .tf-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .tf-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .tf-count-pill { margin-right: auto; display: inline-flex; align-items: center; gap: 6px; padding: 6px 14px; border-radius: 999px; font-size: 12px; font-weight: 800; background: rgba(0,177,79,0.1); color: #007a33; border: 1px solid rgba(0,177,79,0.25); flex-shrink: 0; }
        .tf-card-body { padding: 16px; }

        /* ---------- TABLE ---------- */
        .tf-table-wrap { border-radius: 16px; overflow: hidden; border: 1px solid #e8eef0; background: #fff; box-shadow: 0 2px 8px rgba(0,0,0,0.02); }
        .tf-table-scroll { max-height: 780px; overflow-y: auto; overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .tf-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 12.5px; min-width: 1000px; }
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
        .money { font-family: 'Courier New', 'Consolas', monospace; font-variant-numeric: tabular-nums; font-weight: 700; color: #1f3d33; letter-spacing: -0.3px; direction: ltr; display: inline-block; font-size: 14px; }
        .money.pos { color: #007a33; }
        .money.neg { color: #a51c1c; }
        .money.neutral { color: #6c757d; }

        /* ---------- BADGE ---------- */
        .badge-soft { display: inline-flex; align-items: center; gap: 5px; padding: 5px 11px; border-radius: 8px; font-size: 11px; font-weight: 800; white-space: nowrap; letter-spacing: 0.1px; }
        .badge-soft.green  { background: rgba(0,177,79,0.10); color: #007a33; border: 1px solid rgba(0,177,79,0.22); }
        .badge-soft.yellow { background: rgba(255,193,7,0.14); color: #8a6d00; border: 1px solid rgba(255,193,7,0.30); }
        .badge-soft.red    { background: rgba(220,53,69,0.10); color: #a51c1c; border: 1px solid rgba(220,53,69,0.22); }
        .badge-soft.gray   { background: #f8f9fa; color: #6c757d; border: 1px solid #e9ecef; }
        .badge-soft.blue   { background: rgba(13,110,253,0.10); color: #0a58ca; border: 1px solid rgba(13,110,253,0.22); }

        /* legacy badge classes mapping (from StatusClass in code-behind) */
        .badge-green  { background: rgba(0,177,79,0.10); color: #007a33; border: 1px solid rgba(0,177,79,0.22); display: inline-flex; align-items: center; gap: 5px; padding: 5px 11px; border-radius: 8px; font-size: 11px; font-weight: 800; }
        .badge-yellow { background: rgba(255,193,7,0.14); color: #8a6d00; border: 1px solid rgba(255,193,7,0.30); display: inline-flex; align-items: center; gap: 5px; padding: 5px 11px; border-radius: 8px; font-size: 11px; font-weight: 800; }
        .badge-red    { background: rgba(220,53,69,0.10); color: #a51c1c; border: 1px solid rgba(220,53,69,0.22); display: inline-flex; align-items: center; gap: 5px; padding: 5px 11px; border-radius: 8px; font-size: 11px; font-weight: 800; }
        .badge-gray   { background: #f8f9fa; color: #6c757d; border: 1px solid #e9ecef; display: inline-flex; align-items: center; gap: 5px; padding: 5px 11px; border-radius: 8px; font-size: 11px; font-weight: 800; }

        /* ---------- ROW NUM ---------- */
        .row-num { display: inline-flex; align-items: center; justify-content: center; width: 28px; height: 28px; border-radius: 9px; background: #f8faf9; color: #94a3a8; font-weight: 800; font-size: 11.5px; font-family: 'Courier New', monospace; border: 1px solid #eef1f3; }

        /* ---------- PAGINATION ---------- */
        .table-pagination { padding: 16px 20px !important; background: #fbfdfc; border-top: 1px solid #eef1f3; text-align: center; }
        .table-pagination a, .table-pagination span { display: inline-flex; align-items: center; justify-content: center; min-width: 36px; height: 36px; padding: 0 10px; margin: 0 3px; border-radius: 10px; font-size: 12.5px; font-weight: 800; color: #495057; background: #fff; border: 1.5px solid #e9ecef; text-decoration: none; transition: all 0.15s ease; font-family: 'Droid Arabic Kufi', sans-serif; vertical-align: middle; }
        .table-pagination a:hover { background: #f0f8f3; border-color: #00b14f; color: #007a33; text-decoration: none; transform: translateY(-1px); }
        .table-pagination span { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; border-color: transparent; box-shadow: 0 4px 10px rgba(0,177,79,0.3); }

        /* ---------- INFO MESSAGE ---------- */
        .tf-info-msg { display: inline-flex; align-items: center; gap: 8px; padding: 10px 16px; border-radius: 10px; font-size: 12.5px; font-weight: 700; margin-top: 14px; }
        .tf-info-msg.success { background: rgba(0,177,79,0.10); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .tf-info-msg.danger  { background: rgba(220,53,69,0.10); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }
        .tf-info-msg.neutral { background: #f8f9fa; color: #6c757d; border: 1px solid #e9ecef; }

        /* ---------- EMPTY ---------- */
        .tf-empty { text-align: center; padding: 40px 20px; }
        .tf-empty-icon { width: 72px; height: 72px; border-radius: 20px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 32px; margin-bottom: 16px; }
        .tf-empty h4 { color: #1f3d33; font-weight: 800; font-size: 15px; margin: 0 0 6px; }
        .tf-empty p { color: #6c757d; font-size: 12.5px; margin: 0; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .tf-hero-section { padding: 26px 20px 72px; }
            .tf-hero-titles h1 { font-size: 19px; }
            .tf-body { padding: 0 16px 40px; }
            .tf-info-cards { grid-template-columns: 1fr; }
            .tf-info-grid { grid-template-columns: 1fr 1fr; }
        }
        @media (max-width: 768px) { .tf-table { font-size: 11px; } .tf-table thead th, .tf-table tbody td { padding: 10px 6px !important; } }
        @media (max-width: 576px) {
            .tf-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .tf-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .tf-hero-titles h1 { font-size: 16px; }
            .tf-btn-glass span { display: none; }
            .tf-info-grid { grid-template-columns: 1fr; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .tf-hero-section, .tf-btn-glass, .no-print { display: none !important; }
            .tf-body { margin: 0 !important; padding: 0 !important; }
            .tf-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .tf-table-scroll { max-height: none !important; overflow: visible !important; }
            .tf-table thead th { position: static !important; }
        }
        th {
              text-align: revert;
              background-color: #e5f0e9;
              padding-bottom: 15px;
              padding-top: 15px;
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
                        <i class="fas fa-file-alt"></i>
                    </div>
                    <div class="tf-hero-titles">
                        <h1>كشف الاستحقاق الشهري</h1>
                        <p>تفاصيل احتساب الراتب الشهري للمعلم مع الحصص والمكافآت والخصومات</p>
                        <span class="tf-hero-chip">
                            <i class="fas fa-user"></i>
                            <asp:Label ID="lblTeacherName" runat="server" />
                            &nbsp;·&nbsp;
                            <asp:Label ID="lblPeriod" runat="server" />
                        </span>
                    </div>
                </div>
                <a href='<%= "Details.aspx?id=" + PeriodId %>' class="tf-btn-glass">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة للتفاصيل</span>
                </a>
            </div>
        </div>

        <%-- ================== BODY ================== --%>
        <div class="tf-body">

            <%-- ================== INFO CARDS ================== --%>
            <div class="tf-info-cards">

                <%-- بطاقة معلومات المعلم --%>
                <div class="tf-info-card">
                    <h6><i class="fas fa-user-tie"></i> بيانات المعلم والعقد</h6>
                    <div class="tf-info-grid">
                        <div class="tf-info-row">
                            <span class="tf-info-label">المعلم</span>
                            <span class="tf-info-value"><asp:Label ID="lblTeacherName2" runat="server" /></span>
                        </div>
                        <div class="tf-info-row">
                            <span class="tf-info-label">الفترة</span>
                            <span class="tf-info-value"><asp:Label ID="lblPeriod2" runat="server" /></span>
                        </div>
                        <div class="tf-info-row">
                            <span class="tf-info-label">نوع التعاقد</span>
                            <span class="tf-info-value"><asp:Label ID="lblEmployment" runat="server" /></span>
                        </div>
                        <div class="tf-info-row">
                            <span class="tf-info-label">قيمة الحصة / الراتب</span>
                            <span class="tf-info-value"><asp:Label ID="lblLessonRate" runat="server" /></span>
                        </div>
                    </div>
                </div>

                <%-- بطاقة الملخص المالي --%>
                <div class="tf-info-card">
                    <h6><i class="fas fa-coins"></i> الملخص المالي</h6>

                    <%-- المستحق --%>
                    <div class="tf-info-row" style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 6px;">
                        <span class="tf-info-label">المستحق</span>
                        <span class="tf-info-value green"><asp:Label ID="lblEarned" runat="server" /></span>
                    </div>

                    <%-- ⭐ التعويضات --%>
                    <div class="tf-info-row" style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 6px;">
                        <span class="tf-info-label">التعويضات</span>
                        <span class="tf-info-value green"><asp:Label ID="lblSubstitutions" runat="server" /></span>
                    </div>

                    <%-- الخصومات --%>
                    <div class="tf-info-row" style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 6px;">
                        <span class="tf-info-label">الخصومات</span>
                        <span class="tf-info-value red"><asp:Label ID="lblDeductions" runat="server" /></span>
                    </div>

                    <%-- المكافآت والتسويات --%>
                    <div class="tf-info-row" style="flex-direction: row; justify-content: space-between; align-items: center;">
                        <span class="tf-info-label">المكافآت والتسويات</span>
                        <span class="tf-info-value green"><asp:Label ID="lblAdjustments" runat="server" /></span>
                    </div>

                    <%-- الصافي --%>
                    <div class="tf-net-row">
                        <span class="tf-info-label">الصافي المستحق</span>
                        <span class="tf-info-value"><asp:Label ID="lblNet" runat="server" /></span>
                    </div>
                </div>

            </div>

            <%-- ================== CARD 1: تفصيل الحصص ================== --%>
            <div class="tf-card">
                <div class="tf-card-header">
                    <div class="tf-card-header-icon">
                        <i class="fas fa-calendar-check"></i>
                    </div>
                    <div class="tf-card-header-text">
                        <h3>تفصيل الحصص</h3>
                        <p>سجلات الحضور والحصص المعتمدة خلال الفترة</p>
                    </div>
                </div>
                <div class="tf-card-body">

                    <asp:UpdatePanel ID="upStatement" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="tf-table-wrap">
                                <div class="tf-table-scroll">
                                    <asp:GridView ID="gvRecords" runat="server"
                                        CssClass="tf-table"
                                        AutoGenerateColumns="false"
                                        EmptyDataText=""
                                        GridLines="None"
                                        ShowHeaderWhenEmpty="true"
                                        AllowPaging="true"
                                        PageSize="100"
                                        OnPageIndexChanging="gvRecords_PageIndexChanging">
                                        <Columns>
                                            <%-- # --%>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="60px" ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="row-num"><%# Container.DataItemIndex + 1 %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- التاريخ --%>
                                            <asp:BoundField DataField="Date" HeaderText="التاريخ"
                                                DataFormatString="{0:yyyy-MM-dd}" ItemStyle-CssClass="text-center" />

                                            <%-- الحصة --%>
                                            <asp:BoundField DataField="PeriodNumber" HeaderText="الحصة"
                                                ItemStyle-CssClass="text-center" ItemStyle-Width="70px" />

                                            <%-- الوقت --%>
                                            <asp:TemplateField HeaderText="الوقت" ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="money"><%# Eval("StartTimeText") %></span>
                                                    <span style="color:#ced4da;">—</span>
                                                    <span class="money"><%# Eval("EndTimeText") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- المادة --%>
                                            <asp:BoundField DataField="SubjectName" HeaderText="المادة" />

                                            <%-- الصف/الشعبة --%>
                                            <asp:BoundField DataField="ClassRoomName" HeaderText="الصف/الشعبة" />

                                            <%-- الحالة --%>
                                            <asp:TemplateField HeaderText="الحالة" ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class='<%# StatusClass((int)Eval("Status")) %>'>
                                                        <%# StatusName((int)Eval("Status")) %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- الدقائق --%>
                                            <asp:TemplateField HeaderText="الدقائق" ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="money"><%# Eval("ActualMinutes") %></span>
                                                    <span style="color:#ced4da;">/</span>
                                                    <span class="money" style="color:#6c757d;"><%# Eval("DurationMinutes") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- قيمة الحصة --%>
                                            <asp:TemplateField HeaderText="قيمة الحصة" ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class="money"><%# Eval("RateDisplay") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- حساب البند --%>
                                            <asp:TemplateField HeaderText="حساب البند" ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <span class='money <%# GetLineClass(Eval("LineDisplay")) %>'>
                                                        <%# Eval("LineDisplay") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="table-pagination" />
                                    </asp:GridView>
                                </div>
                            </div>

                            <asp:Label ID="lblInfo" runat="server" CssClass="tf-info-msg neutral" Visible="false" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </div>

            <%-- ================== CARD 2: المكافآت/الخصومات/التسويات ================== --%>
            <div class="tf-card">
                <div class="tf-card-header">
                    <div class="tf-card-header-icon orange">
                        <i class="fas fa-coins"></i>
                    </div>
                    <div class="tf-card-header-text">
                        <h3>المكافآت / الخصومات / التسويات</h3>
                        <p>البنود الإضافية التي تمت إضافتها على استحقاق هذه الفترة</p>
                    </div>
                </div>
                <div class="tf-card-body">
                    <div class="tf-table-wrap">
                        <div class="tf-table-scroll">
                            <asp:GridView ID="gvItems" runat="server"
                                CssClass="tf-table"
                                AutoGenerateColumns="false"
                                EmptyDataText=""
                                GridLines="None"
                                ShowHeaderWhenEmpty="true">
                                <Columns>
                                    <%-- # --%>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="60px" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="row-num"><%# Container.DataItemIndex + 1 %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- النوع --%>
                                    <asp:TemplateField HeaderText="النوع" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class="badge-soft blue">
                                                <i class="fas fa-tag" style="font-size:9px;"></i>
                                                <%# ItemTypeName((int)Eval("Type")) %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- السبب --%>
                                    <asp:BoundField DataField="Reason" HeaderText="السبب" />

                                    <%-- القيمة --%>
                                    <asp:TemplateField HeaderText="القيمة" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <span class='money <%# (int)Eval("Type") == 1 ? "neg" : "pos" %>'>
                                                <%# ((decimal)Eval("Amount")).ToString("N2") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%-- أضيف بواسطة --%>
                                    <asp:BoundField DataField="ApprovedBy" HeaderText="أضيف بواسطة" ItemStyle-CssClass="text-center" />

                                    <%-- التاريخ --%>
                                    <asp:BoundField DataField="CreatedAt" HeaderText="التاريخ"
                                        DataFormatString="{0:yyyy-MM-dd HH:mm}" ItemStyle-CssClass="text-center" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>