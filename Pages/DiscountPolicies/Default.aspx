<%@ Page Title="سياسات الخصم والإعفاءات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.DiscountPolicies.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Discount Policies List
           ========================================================= */
        .dp-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .dp-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .dp-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .dp-hero::after {
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
        .dp-hero-content {
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
        .dp-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .dp-hero-icon {
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
        .dp-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .dp-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .dp-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .dp-btn-glass {
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
        .dp-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .dp-btn-glass.primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .dp-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .dp-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- STATS CARDS ---------- */
        .dp-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 14px;
            margin-bottom: 20px;
        }
        .dp-stat-card {
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
        .dp-stat-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.06);
        }
        .dp-stat-icon {
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
        .dp-stat-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .dp-stat-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .dp-stat-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .dp-stat-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .dp-stat-info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
        .dp-stat-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
        }
        .dp-stat-value {
            font-size: 19px;
            font-weight: 800;
            color: #212529;
            line-height: 1.1;
        }

        /* ---------- CARD ---------- */
        .dp-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .dp-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .dp-card-header-icon {
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
        .dp-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .dp-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .dp-count-pill {
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
        .dp-card-body { padding: 0; }

        /* ---------- TABLE ---------- */
        .dp-table-wrap {
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }
        .dp-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
            min-width: 1000px;
        }
        .dp-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33;
            font-size: 11.5px;
            font-weight: 800;
            padding: 14px 12px !important;
            text-align: center;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
        }
        .dp-table tbody td {
            font-size: 13px;
            padding: 12px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: center;
        }
        .dp-table tbody tr:hover td { background: #f8fdfa; }
        .dp-table tbody tr:last-child td { border-bottom: none; }

        /* Policy name cell */
        .policy-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 700;
            color: #212529;
            text-align: start;
        }
        .policy-name-icon {
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

        /* Type badge */
        .policy-type-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            background: rgba(13,110,253,0.08);
            color: #0d6efd;
            border: 1px solid rgba(13,110,253,0.2);
            white-space: nowrap;
        }

        /* Value badge */
        .policy-value-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 14px;
            border-radius: 999px;
            font-size: 12.5px;
            font-weight: 800;
            background: linear-gradient(135deg, rgba(0,177,79,0.1) 0%, rgba(0,177,79,0.04) 100%);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
            font-family: 'Courier New', monospace;
            white-space: nowrap;
        }

        /* Scope badge */
        .policy-scope-badge {
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

        /* Auto badge */
        .policy-auto-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            white-space: nowrap;
        }
        .policy-auto-badge.yes {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .policy-auto-badge.no {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }

        /* Status badge */
        .policy-status-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            white-space: nowrap;
        }
        .policy-status-badge.active {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .policy-status-badge.inactive {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }

        /* Row number */
        .policy-row-num {
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
        .policy-btn-action {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 6px 12px;
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
        .policy-btn-action.edit {
            border-color: #00b14f;
            color: #00b14f;
        }
        .policy-btn-action.edit:hover {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            text-decoration: none;
        }
        .policy-btn-action.delete {
            border-color: #dc3545;
            color: #dc3545;
        }
        .policy-btn-action.delete:hover {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            text-decoration: none;
        }

        /* ---------- EMPTY STATE ---------- */
        .dp-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .dp-empty-icon {
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
        .dp-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .dp-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0 0 18px;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .dp-hero { padding: 26px 20px 72px; }
            .dp-hero-titles h1 { font-size: 19px; }
            .dp-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .dp-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .dp-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .dp-hero-titles h1 { font-size: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .dp-hero, .dp-hero-actions, .dp-btn-glass,
            .dp-stats, .policy-btn-action, .no-print { display: none !important; }
            .dp-page { background: #fff !important; }
            .dp-body { margin: 0 !important; padding: 0 !important; }
            .dp-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .dp-table { font-size: 10px !important; min-width: 0 !important; }
            .dp-table thead th,
            .dp-table tbody td { padding: 6px 4px !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dp-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="dp-hero">
            <div class="dp-hero-content">
                <div class="dp-hero-left">
                    <div class="dp-hero-icon">
                        <i class="fas fa-percent"></i>
                    </div>
                    <div class="dp-hero-titles">
                        <h1>سياسات الخصم والإعفاءات</h1>
                        <p>إدارة سياسات الخصم والإعفاءات المطبقة على الرسوم</p>
                    </div>
                </div>
                <div class="dp-hero-actions no-print">
                    <a href="Create.aspx" class="dp-btn-glass primary">
                        <i class="fas fa-plus"></i> إضافة سياسة جديدة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="dp-body">

            <!-- بطاقات الإحصائيات -->
            <div class="dp-stats no-print">
                <div class="dp-stat-card">
                    <div class="dp-stat-icon green"><i class="fas fa-percent"></i></div>
                    <div class="dp-stat-info">
                        <span class="dp-stat-label">إجمالي السياسات</span>
                        <span class="dp-stat-value"><asp:Label ID="lblTotalPolicies" runat="server" Text="0" /></span>
                    </div>
                </div>
                <div class="dp-stat-card">
                    <div class="dp-stat-icon blue"><i class="fas fa-check-circle"></i></div>
                    <div class="dp-stat-info">
                        <span class="dp-stat-label">السياسات النشطة</span>
                        <span class="dp-stat-value"><asp:Label ID="lblActivePolicies" runat="server" Text="0" /></span>
                    </div>
                </div>
                <div class="dp-stat-card">
                    <div class="dp-stat-icon purple"><i class="fas fa-bolt"></i></div>
                    <div class="dp-stat-info">
                        <span class="dp-stat-label">تطبيق تلقائي</span>
                        <span class="dp-stat-value"><asp:Label ID="lblAutoPolicies" runat="server" Text="0" /></span>
                    </div>
                </div>
            </div>

            <!-- الجدول -->
            <asp:Panel ID="pnlTable" runat="server">
                <div class="dp-card">
                    <div class="dp-card-header">
                        <div class="dp-card-header-icon">
                            <i class="fas fa-list-ul"></i>
                        </div>
                        <div class="dp-card-header-text">
                            <h3>قائمة السياسات</h3>
                            <p>جميع سياسات الخصم والإعفاءات المسجلة</p>
                        </div>
                        <span class="dp-count-pill">
                            <i class="fas fa-list-ol"></i>
                            <asp:Label ID="lblCountPill" runat="server" Text="0" />
                        </span>
                    </div>

                    <div class="dp-card-body">
                        <div class="dp-table-wrap">
                            <table class="dp-table">
                                <thead>
                                    <tr>
                                        <th style="width:60px;">#</th>
                                        <th style="text-align:right;">اسم السياسة</th>
                                        <th>النوع</th>
                                        <th>القيمة</th>
                                        <th>تلقائي</th>
                                        <th>نطاق الرسم</th>
                                        <th>الحالة</th>
                                        <th style="width:200px;">إجراءات</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptPolicies" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <span class="policy-row-num"><%# Container.ItemIndex + 1 %></span>
                                                </td>
                                                <td>
                                                    <div class="policy-name-cell">
                                                       
                                                        <span><%# Eval("Name") %></span>
                                                    </div>
                                                </td>
                                                <td>
                                                    <span class="policy-type-badge">
                                                        <i class="fas fa-tag" style="font-size:10px;"></i>
                                                        <%# Eval("TypeName") %>
                                                    </span>
                                                </td>
                                                <td>
                                                    <span class="policy-value-badge">
                                                        <%# Eval("KindValue") %>
                                                    </span>
                                                </td>
                                                <td>
                                                    <span class='<%# (bool)Eval("ApplyAutomatically") ? "policy-auto-badge yes" : "policy-auto-badge no" %>'>
                                                        <%# (bool)Eval("ApplyAutomatically") ? "نعم" : "لا" %>
                                                    </span>
                                                </td>
                                                <td>
                                                    <span class="policy-scope-badge">
                                                        <i class="fas fa-bullseye" style="font-size:10px;"></i>
                                                        <%# Eval("ScopeName") %>
                                                    </span>
                                                </td>
                                                <td>
                                                    <span class='<%# (bool)Eval("IsActive") ? "policy-status-badge active" : "policy-status-badge inactive" %>'>
                                                        <i class='<%# (bool)Eval("IsActive") ? "fas fa-check-circle" : "fas fa-pause-circle" %>' style="font-size:10px;"></i>
                                                        <%# (bool)Eval("IsActive") ? "نشطة" : "موقوفة" %>
                                                    </span>
                                                </td>
                                                <td>
                                                    <div class="d-flex gap-1 justify-content-center flex-wrap">
                                                        <a href='<%# "Edit.aspx?id=" + Eval("Id") %>'
                                                           class="policy-btn-action edit" title="تعديل">
                                                            <i class="fas fa-edit"></i> تعديل
                                                        </a>
                                                        <a href='<%# "Delete.aspx?id=" + Eval("Id") %>'
                                                           class="policy-btn-action delete" title="حذف">
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
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="dp-card">
                    <div class="dp-card-body">
                        <div class="dp-empty">
                            <div class="dp-empty-icon">
                                <i class="fas fa-percent"></i>
                            </div>
                            <h4>لا توجد سياسات</h4>
                            <p>لم يتم إضافة أي سياسات خصم أو إعفاء بعد</p>
                            <a href="Create.aspx" class="dp-btn-glass primary" style="display:inline-flex;">
                                <i class="fas fa-plus"></i> إضافة سياسة جديدة
                            </a>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>
</asp:Content>