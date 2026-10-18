<%@ Page Title="رسوم الإشتراك" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Fees.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Fees List
           ========================================================= */
        .fees-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .fees-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .fees-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .fees-hero::after {
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
        .fees-hero-content {
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
        .fees-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .fees-hero-icon {
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
        .fees-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .fees-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .fees-stage-label {
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
        .fees-stage-label i { font-size: 11px; }
        .fees-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .fees-btn-glass {
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
        .fees-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .fees-btn-glass.primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .fees-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .fees-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- STATS CARDS ---------- */
        .fees-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 14px;
            margin-bottom: 20px;
        }
        .fees-stat-card {
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
        .fees-stat-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.06);
        }
        .fees-stat-icon {
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
        .fees-stat-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .fees-stat-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .fees-stat-info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
        .fees-stat-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
        }
        .fees-stat-value {
            font-size: 19px;
            font-weight: 800;
            color: #212529;
            line-height: 1.1;
        }

        /* ---------- CARD ---------- */
        .fees-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .fees-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .fees-card-header-icon {
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
        .fees-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .fees-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .fees-count-pill {
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
        .fees-card-body { padding: 0; }

        /* ---------- TABLE ---------- */
        .fees-table-wrap {
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }
        .fees-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
            min-width: 1050px;
        }
        .fees-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33;
            font-size: 13px;
            font-weight: 700;
            padding: 14px 10px !important;
            text-align: center;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
        }
        .fees-table tbody td {
            font-size: 13px;
            padding: 10px 5px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: center;
        }
        .fees-table tbody tr:hover td { background: #f8fdfa; }
        .fees-table tbody tr:last-child td { border-bottom: none; }

        /* Amount */
        .fee-amount {
            font-weight: 800;
            color: #00b14f;
            font-size: 13px;
        }

        /* Type badge */
        .fee-type-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 5px;
            font-size: 12px;
            font-weight: 700;
            color: #0d6efd;
            white-space: nowrap;
        }

        /* Grade badge */
        .fee-grade-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 5px;
            font-size: 12px;
            font-weight: 700;
            color: #007a33;
            white-space: nowrap;
        }
        .fee-grade-badge.all {
            background: rgba(108,117,125,0.1);
            color: #6c757d;
            border-color: rgba(108,117,125,0.2);
        }

        /* Section badge */
        .fee-section-badge {
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

        /* Division badge */
        .fee-division-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 5px;
            font-size: 12px;
            font-weight: 700;
            color: #d97706;
            white-space: nowrap;
        }

        /* Row number */
        .fee-row-num {
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
        .fee-btn-action {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 6px 10px;
            border-radius: 8px;
            font-size: 11.5px;
            font-weight: 700;
            text-decoration: none;
            border: 1.5px solid;
            transition: all 0.15s ease;
            cursor: pointer;
            background: #fff;
            font-family: 'Droid Arabic Kufi', sans-serif;
            white-space: nowrap;
        }
        .fee-btn-action.students {
            border-color: #0d6efd;
            color: #0d6efd;
        }
        .fee-btn-action.students:hover {
            background: rgba(13,110,253,0.08);
            color: #0d6efd;
            text-decoration: none;
        }
        .fee-btn-action.edit {
            border-color: #00b14f;
            color: #00b14f;
        }
        .fee-btn-action.edit:hover {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            text-decoration: none;
        }
        .fee-btn-action.delete {
            border-color: #dc3545;
            color: #dc3545;
        }
        .fee-btn-action.delete:hover {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            text-decoration: none;
        }

        /* ---------- EMPTY STATE ---------- */
        .fees-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .fees-empty-icon {
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
        .fees-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .fees-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0 0 18px;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .fees-hero { padding: 26px 20px 72px; }
            .fees-hero-titles h1 { font-size: 19px; }
            .fees-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .fees-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .fees-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .fees-hero-titles h1 { font-size: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .fees-hero, .fees-hero-actions, .fees-btn-glass,
            .fees-stats, .fee-btn-action, .no-print { display: none !important; }
            .fees-page { background: #fff !important; }
            .fees-body { margin: 0 !important; padding: 0 !important; }
            .fees-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .fees-table { font-size: 10px !important; min-width: 0 !important; }
            .fees-table thead th,
            .fees-table tbody td { padding: 6px 4px !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fees-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="fees-hero">
            <div class="fees-hero-content">
                <div class="fees-hero-left">
                    <div class="fees-hero-icon">
                        <i class="fas fa-money-bill-wave"></i>
                    </div>
                    <div class="fees-hero-titles">
                        <h1>
                            رسوم الإشتراك
                            <span class="fees-stage-label">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:Literal ID="lblStage" runat="server" />
                            </span>
                        </h1>
                        <p>عرض وإدارة جميع رسوم الإشتراك الخاصة بالمرحلة الحالية</p>
                    </div>
                </div>
                <div class="fees-hero-actions no-print">
                    <a href="Create.aspx" class="fees-btn-glass primary">
                        <i class="fas fa-plus"></i> إضافة رسم إشتراك
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="fees-body">

            <!-- بطاقات الإحصائيات -->
            <div class="fees-stats no-print">
                <div class="fees-stat-card">
                    <div class="fees-stat-icon green"><i class="fas fa-file-invoice-dollar"></i></div>
                    <div class="fees-stat-info">
                        <span class="fees-stat-label">عدد الرسوم</span>
                        <span class="fees-stat-value"><asp:Label ID="lblTotalFees" runat="server" Text="0" /></span>
                    </div>
                </div>
                <div class="fees-stat-card">
                    <div class="fees-stat-icon purple"><i class="fas fa-coins"></i></div>
                    <div class="fees-stat-info">
                        <span class="fees-stat-label">إجمالي المبالغ</span>
                        <span class="fees-stat-value"><asp:Label ID="lblTotalAmount" runat="server" Text="0.00" /></span>
                    </div>
                </div>
            </div>

            <!-- الجدول -->
            <asp:Panel ID="pnlTable" runat="server">
                <div class="fees-card">
                    <div class="fees-card-header">
                        <div class="fees-card-header-icon">
                            <i class="fas fa-list-ul"></i>
                        </div>
                        <div class="fees-card-header-text">
                            <h3>قائمة رسوم الإشتراك</h3>
                            <p>جميع الرسوم المسجلة للمرحلة الحالية</p>
                        </div>
                        <span class="fees-count-pill">
                            <i class="fas fa-list-ol"></i>
                            <asp:Label ID="lblCountPill" runat="server" Text="0" />
                        </span>
                    </div>

                    <div class="fees-card-body">
                        <div class="fees-table-wrap">
                            <table class="fees-table">
                                <thead>
                                    <tr>
                                        <th style="width:60px;">#</th>
                                        <th>نوع الرسم</th>
                                        <th>المبلغ</th>
                                        <th>السنة الدراسية</th>
                                        <th>الصف</th>
                                        <th>القسم</th>
                                        <th>الشعبة</th>
                                        <th>ملاحظات</th>
                                        <th style="width:260px;">الإجراءات</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptFees" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <span class="fee-row-num"><%# Container.ItemIndex + 1 %></span>
                                                </td>
                                                <td>
                                                    <span class="fee-type-badge">
                                                        <%# Eval("TypeName") %>
                                                    </span>
                                                </td>
                                                <td>
                                                    <span class="fee-amount">
                                                        <%# FormatAmount((decimal)Eval("Amount")) %>
                                                    </span>
                                                </td>
                                                <td style="color:#6c757d;font-weight:600;">
                                                    <%# Eval("AcademicYearName") %>
                                                </td>
                                                <td>
                                                    <%# RenderGradeLevelBadge(Eval("GradeLevelId"), Eval("GradeLevelName")) %>
                                                </td>
                                                <td>
                                                    <%# RenderSection(Eval("GradeLevelName"), Eval("SectionName")) %>
                                                </td>
                                                <td>
                                                    <%# RenderDivision(Eval("DivisionName")) %>
                                                </td>
                                                <td style="color:#6c757d;font-size:12.5px;">
                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("Notes"))) ? "—" : Eval("Notes") %>
                                                </td>
                                                <td>
                                                    <div class="d-flex gap-1 justify-content-center flex-wrap">
                                                        <a href='<%# BuildStudentFeesUrl(Eval("AcademicYearId"), Eval("GradeLevelId"), Eval("SectionName")) %>'
                                                           class="fee-btn-action students" title="رسوم الطلاب">
                                                            رسوم الطلاب
                                                        </a>
                                                        <a href='<%# "Edit.aspx?id=" + Eval("Id") %>'
                                                           class="fee-btn-action edit" title="تعديل">
                                                            <i class="fas fa-edit"></i> تعديل
                                                        </a>
                                                        <a href='<%# "Delete.aspx?id=" + Eval("Id") %>'
                                                           class="fee-btn-action delete" title="حذف">
                                                            <i class="fas fa-trash"></i> حذف
                                                        </a>
                                                    </div>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- الحالة الفارغة -->
            <asp:Panel ID="pnlEmpty" runat="server">
                <div class="fees-card">
                    <div class="fees-card-body">
                        <div class="fees-empty">
                            <div class="fees-empty-icon">
                                <i class="fas fa-inbox"></i>
                            </div>
                            <h4>لا توجد رسوم</h4>
                            <p>لم يتم إضافة أي رسوم لهذه المرحلة بعد</p>
                            <a href="Create.aspx" class="fees-btn-glass primary" style="display:inline-flex;">
                                <i class="fas fa-plus"></i> إضافة رسم إشتراك
                            </a>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>
</asp:Content>