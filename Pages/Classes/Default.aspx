<%@ Page Title="الفصول" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Classes.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Classes List
           ========================================================= */
        .cls-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .cls-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .cls-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .cls-hero::after {
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
        .cls-hero-content {
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
        .cls-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .cls-hero-icon {
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
        .cls-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .cls-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .cls-stage-label {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            margin-inline-start: 8px;
            padding: 3px 12px;
            border-radius: 999px;
            background: rgba(255,255,255,0.15);
            border: 1px solid rgba(255,255,255,0.3);
            font-size: 12px;
            font-weight: 700;
            color: #fff;
        }
        .cls-stage-label i { font-size: 11px; }
        .cls-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .cls-btn-glass {
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
        .cls-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .cls-btn-glass.primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .cls-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .cls-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- STATS CARDS ---------- */
        .cls-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 14px;
            margin-bottom: 20px;
        }
        .cls-stat-card {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 16px 18px;
            border-radius: 16px;
            background: #ffffff;
            border: 1px solid #99f6c3;
            box-shadow: 0 4px 12px rgba(0,0,0,0.03);
            transition: all 0.2s ease;
        }
        .cls-stat-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.06);
        }
        .cls-stat-icon {
            width: 46px;
            height: 46px;
            border-radius: 12px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            flex-shrink: 0;
            color: #fff;
        }
        .cls-stat-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .cls-stat-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .cls-stat-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .cls-stat-info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
        .cls-stat-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
        }
        .cls-stat-value {
            font-size: 19px;
            font-weight: 800;
            color: #212529;
            line-height: 1.1;
        }

        /* ---------- CARD ---------- */
        .cls-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .cls-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .cls-card-header-icon {
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
        }
        .cls-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .cls-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .cls-card-body { padding: 0; }

        /* ---------- TABLE ---------- */
        .cls-table-wrap { overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .cls-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
            min-width: 900px;
        }
        .cls-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33;
            font-size: 11.5px;
            font-weight: 800;
            padding: 14px 12px !important;
            text-align: center;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
        }
        .cls-table tbody td {
            font-size: 13px;
            padding: 12px 12px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: center;
        }
        .cls-table tbody tr:hover td { background: #f8fdfa; }
        .cls-table tbody tr:last-child td { border-bottom: none; }

        /* Class badge */
        .cls-class-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.2);
            white-space: nowrap;
        }
        .cls-section-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            background: rgba(111,66,193,0.08);
            color: #6f42c1;
            border: 1px solid rgba(111,66,193,0.2);
            white-space: nowrap;
        }
        .cls-division-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            background: rgba(253,126,20,0.08);
            color: #d97706;
            border: 1px solid rgba(253,126,20,0.2);
            white-space: nowrap;
        }
        .cls-row-num {
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

        /* ---------- ACTION BUTTONS ---------- */
        .cls-btn-action {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 4px;
            padding: 7px 12px;
            border-radius: 8px;
            font-size: 12px;
            font-weight: 700;
            text-decoration: none;
            border: 1.5px solid;
            transition: all 0.15s ease;
            cursor: pointer;
            background: #fff;
            font-family: 'Droid Arabic Kufi', sans-serif;
            white-space: nowrap;
        }
        .cls-btn-action.edit {
            border-color: #00b14f;
            color: #00b14f;
        }
        .cls-btn-action.edit:hover {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            text-decoration: none;
        }
        .cls-btn-action.delete {
            border-color: #dc3545;
            color: #dc3545;
        }
        .cls-btn-action.delete:hover {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            text-decoration: none;
        }

        /* ---------- EMPTY STATE ---------- */
        .cls-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .cls-empty-icon {
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
        .cls-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .cls-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0 0 18px;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .cls-hero { padding: 26px 20px 72px; }
            .cls-hero-titles h1 { font-size: 19px; }
            .cls-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .cls-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .cls-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .cls-hero-titles h1 { font-size: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .cls-hero, .cls-hero-actions, .cls-btn-glass,
            .cls-stats, .cls-btn-action, .no-print { display: none !important; }
            .cls-page { background: #fff !important; }
            .cls-body { margin: 0 !important; padding: 0 !important; }
            .cls-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .cls-table { font-size: 10px !important; min-width: 0 !important; }
            .cls-table thead th,
            .cls-table tbody td { padding: 6px 4px !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cls-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="cls-hero">
            <div class="cls-hero-content">
                <div class="cls-hero-left">
                    <div class="cls-hero-icon">
                        <i class="fas fa-chalkboard"></i>
                    </div>
                    <div class="cls-hero-titles">
                        <h1>
                            إدارة الفصول
                            <asp:Panel ID="pnlStageLabel" runat="server" CssClass="cls-stage-label" Visible="false">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:Label ID="lblStage" runat="server" />
                            </asp:Panel>
                        </h1>
                        <p>عرض وإدارة الفصول الدراسية للمرحلة الحالية</p>
                    </div>
                </div>
                <div class="cls-hero-actions no-print">
                    <asp:Button ID="btnAdd" runat="server" Text="إضافة فصل جديد"
                        CssClass="cls-btn-glass primary"
                        OnClick="btnAddClass_Click" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="cls-body">

            <!-- الجدول -->
            <div class="cls-card">
                <div class="cls-card-header">
                    <div class="cls-card-header-icon">
                        <i class="fas fa-list-ul"></i>
                    </div>
                    <div class="cls-card-header-text">
                        <h3>قائمة الفصول</h3>
                        <p>جميع الفصول المسجلة للمرحلة الحالية</p>
                    </div>
                </div>

                <div class="cls-card-body">
                    <div class="cls-table-wrap">
                        <asp:GridView ID="gvClasses" runat="server" CssClass="cls-table"
                            AutoGenerateColumns="false" EmptyDataText="" GridLines="None"
                            AllowPaging="true" PageSize="15"
                            OnRowCommand="gvClasses_RowCommand"
                            OnPageIndexChanging="gvClasses_PageIndexChanging"
                            ShowHeaderWhenEmpty="true">
                            <Columns>

                                <asp:TemplateField HeaderText="#" ItemStyle-Width="60px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="cls-row-num"><%# Container.DataItemIndex + 1 %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="السنة الدراسية">
                                    <ItemTemplate>
                                        <span class="cls-class-badge">
                                            <i class="fas fa-calendar-alt" style="font-size:10px;"></i>
                                            <%# Eval("AcademicYear.Name") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الصف">
                                    <ItemTemplate>
                                        <span class="cls-class-badge">
                                            <i class="fas fa-graduation-cap" style="font-size:10px;"></i>
                                            <%# Eval("GradeLevel.Name") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="القسم" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# FormatSection(Eval("SectionName")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الشعبة" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# FormatDivision(Eval("DivisionName")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="إجراءات" ItemStyle-CssClass="text-center" ItemStyle-Width="200px">
                                    <ItemTemplate>
                                        <div class="d-flex gap-1 justify-content-center flex-wrap">
                                            <asp:LinkButton ID="lnkEdit" runat="server"
                                                CommandName="EditClass"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CssClass="cls-btn-action edit"
                                                ToolTip="تعديل">
                                                <i class="fas fa-edit"></i> تعديل
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="lnkDelete" runat="server"
                                                CommandName="DeleteClass"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CssClass="cls-btn-action delete"
                                                ToolTip="حذف"
                                               >
                                                <i class="fas fa-trash"></i> حذف
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                            <PagerStyle CssClass="pagination justify-content-center" />
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="cls-empty">
                        <div class="cls-empty-icon">
                            <i class="fas fa-chalkboard"></i>
                        </div>
                        <h4>لا توجد فصول مسجلة</h4>
                        <p>لم يتم إضافة أي فصول لهذه المرحلة بعد</p>
                        <asp:Button ID="btnAddFromEmpty" runat="server" Text="إضافة فصل جديد"
                            CssClass="cls-btn-glass primary"
                            OnClick="btnAddClass_Click"
                            style="display:inline-flex;" />
                    </asp:Panel>
                </div>
            </div>

        </div>
    </div>
</asp:Content>