<%@ Page Title="الطلاب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Students.Default" %>

<%@ Import Namespace="NoorAlMalaikaSchool.Models" %>
<%@ Import Namespace="NoorAlMalaikaSchool.Data.Helpers" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        .students-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .students-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .students-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .students-hero::after {
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

        .students-hero-content {
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

        .students-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .students-hero-icon {
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
        .students-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .students-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }

        .students-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }

        .students-btn-glass {
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
        }
        .students-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .students-btn-glass-primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .students-btn-glass-primary:hover {
            background: #f8f9fa;
            color: #007a33;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .students-body {
            max-width: 1280px;
            margin: -56px auto 0;
            padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .students-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .students-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .students-card-header-icon {
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
        .students-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .students-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .students-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 200px;
        }
        .students-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .students-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .students-count-pill {
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

        .students-card-body {
            padding: 20px 24px;
        }

        /* ---------- FLASH MESSAGE ---------- */
        .students-flash {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 14px 20px;
            border-radius: 14px;
            margin-bottom: 20px;
            font-size: 13.5px;
            font-weight: 700;
            animation: slideDownFlash 0.4s ease;
        }
        .students-flash i {
            width: 36px;
            height: 36px;
            border-radius: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
            color: #fff;
        }
        .students-flash.success {
            background: linear-gradient(180deg, #ffffff 0%, #f8fdfa 100%);
            border: 1.5px solid rgba(0,177,79,0.35);
            color: #1f3d33;
        }
        .students-flash.success i { background: linear-gradient(135deg, #00b14f, #4ed17e); }

        .students-flash.danger {
            background: linear-gradient(180deg, #ffffff 0%, #fef8f8 100%);
            border: 1.5px solid rgba(220,53,69,0.35);
            color: #7a1a26;
        }
        .students-flash.danger i { background: linear-gradient(135deg, #dc3545, #ff6b7a); }

        @keyframes slideDownFlash {
            from { opacity: 0; transform: translateY(-10px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        /* ---------- FILTERS ROW ---------- */
        .students-filters {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            align-items: end;
            margin-bottom: 20px;
        }
        .students-filters .students-field {
            flex: 1 1 180px;
            min-width: 0;
        }
        .students-filters .students-field:last-child {
            flex: 0 0 130px;
            max-width: 130px;
        }
        .students-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .students-field label {
            font-size: 13px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .students-field label i {
            color: #00b14f;
            font-size: 12px;
        }
        .students-field .form-control,
        .students-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 12px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px;
        }
        .students-field .form-control:focus,
        .students-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- BUTTONS ---------- */
        .students-btn-primary {
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
            width: 100%;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .students-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }

        /* ---------- TABLE ---------- */
        .students-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
            padding-top: 20px;
            padding-right: 15px;
        }
        .students-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .students-table thead th {
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
        .students-table tbody td {
            font-size: 13px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .students-table tbody tr {
            transition: all 0.15s ease;
        }
        .students-table tbody tr:hover td {
            background: #f8fdfa;
        }
        .students-table tbody tr:last-child td {
            border-bottom: none;
        }

        /* Student name */
        .student-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 700;
            color: #212529;
        }
        .reg-no-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-size: 13px;
            font-weight: 700;
            color: #0d6efd;
        }
        .national-id {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-weight: 700;
            font-size: 13px;
            color: #495057;
        }
        .class-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            color: #495057;
            font-size: 13px;
            font-weight: 500;
        }
        .section-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-size: 12.5px;
            font-weight: 700;
        }
        .gender-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            color: #495057;
            font-size: 13px;
            font-weight: 500;
        }
        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
        }
        .status-badge.active {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .status-badge.inactive {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }

        /* ---------- ACTION BUTTONS ---------- */
        .action-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 5px;
            padding: 7px 11px;
            border-radius: 9px;
            font-size: 12px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.15s ease;
            text-decoration: none;
            border: 1.5px solid transparent;
            white-space: nowrap;
            background: #fff;
            margin: 0 2px;
        }
        .action-btn i { font-size: 12px; }
        .action-btn.edit {
            color: #00b14f;
            border-color: rgba(0,177,79,0.3);
        }
        .action-btn.edit:hover {
            background: #00b14f;
            color: #fff;
            box-shadow: 0 4px 10px rgba(0,177,79,0.25);
            text-decoration: none;
        }
        .action-btn.view {
            color: #0d6efd;
            border-color: rgba(13,110,253,0.3);
        }
        .action-btn.view:hover {
            background: #0d6efd;
            color: #fff;
            box-shadow: 0 4px 10px rgba(13,110,253,0.25);
            text-decoration: none;
        }
        .action-btn.toggle {
            color: #fd7e14;
            border-color: rgba(253,126,20,0.3);
        }
        .action-btn.toggle:hover {
            background: #fd7e14;
            color: #fff;
            text-decoration: none;
        }
        .action-btn.danger {
            color: #dc3545;
            border-color: rgba(220,53,69,0.3);
        }
        .action-btn.danger:hover {
            background: #dc3545;
            color: #fff;
            text-decoration: none;
        }

        /* Pagination */
        .students-table .pagination {
            display: flex;
            gap: 6px;
            justify-content: center;
            padding: 16px 0 4px;
            margin: 0;
            list-style: none;
        }

        /* ---------- EMPTY STATE ---------- */
        .students-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .students-empty-icon {
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
        .students-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .students-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0;
        }

        /* Stage badge */
        .stage-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 800;
            background: rgba(255,255,255,0.2);
            color: #fff;
            border: 1px solid rgba(255,255,255,0.35);
            margin-top: 6px;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .students-hero { padding: 26px 20px 72px; }
            .students-hero-titles h1 { font-size: 19px; }
            .students-body { padding: 0 16px 40px; }

            .students-filters .students-field {
                flex: 1 1 45%;
            }
            .students-filters .students-field:last-child {
                flex: 1 1 100%;
                max-width: none;
            }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .students-table { font-size: 11px; min-width: 900px; }
            .students-table thead th,
            .students-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .students-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .students-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .students-hero-titles h1 { font-size: 16px; }
            .students-hero-titles p { font-size: 12px; }
            .students-btn-glass { padding: 8px 14px; font-size: 12px; }

            .students-filters .students-field {
                flex: 1 1 100%;
                max-width: none;
            }
        }

        /* عرض كل طالب كبطاقة على الهاتف بدلاً من تمرير الجدول أفقياً */
        @media (max-width: 767.98px) {
            .students-table-wrap {
                overflow: visible !important;
                border: 0;
                border-radius: 0;
                padding: 0;
            }
            .students-table,
            .students-table tbody { display: block; width: 100%; min-width: 0 !important; }
            .students-table thead { display: none; }
            .students-table tbody tr.students-table-header-row { display: none !important; }
            .students-table tbody tr {
                display: block;
                width: 100%;
                margin: 0 0 12px;
                padding: 10px 14px;
                border: 1px solid #e5eee9;
                border-radius: 14px;
                background: #fff;
                box-shadow: 0 4px 14px rgba(31, 61, 51, 0.06);
            }
            .students-table tbody tr.students-table-pager-row {
                display: block;
                margin: 0;
                padding: 12px 0 0;
                border: 0;
                border-radius: 0;
                background: transparent;
                box-shadow: none;
            }
            .students-table tbody tr.students-table-pager-row td {
                display: block;
                width: 100% !important;
                padding: 0 !important;
                border: 0;
                background: transparent;
                text-align: center !important;
            }
            .students-table tbody tr.students-table-pager-row td::before { content: none; display: none; }
            .students-table tbody tr.students-table-pager-row .pagination { flex-wrap: wrap; }
            .students-table tbody tr:hover td { background: #fff; }
            .students-table tbody td {
                display: grid;
                grid-template-columns: 82px minmax(0, 1fr);
                align-items: center;
                gap: 10px;
                width: 100% !important;
                padding: 8px 0 !important;
                border-bottom: 1px solid #f1f3f5;
                text-align: right !important;
                white-space: normal;
            }
            .students-table tbody td::before {
                content: attr(data-label);
                color: #6c757d;
                font-size: 11px;
                font-weight: 800;
                line-height: 1.5;
                text-align: right;
            }
            .students-table tbody td:first-child {
                display: block;
                padding-top: 4px !important;
                padding-bottom: 12px !important;
            }
            .students-table tbody td:first-child::before {
                display: block;
                margin-bottom: 8px;
            }
            .students-table tbody td:last-child {
                display: block;
                padding-bottom: 2px !important;
                border-bottom: 0;
            }
            .students-table tbody td:last-child::before { display: block; margin-bottom: 8px; }
            .students-table tbody td > *,
            .student-name-cell { min-width: 0; max-width: 100%; overflow-wrap: anywhere; }
            .students-table tbody td:last-child .d-flex {
                display: grid !important;
                grid-template-columns: repeat(2, minmax(0, 1fr));
                width: 100%;
                min-width: 0;
                gap: 6px !important;
            }
            .students-table tbody td:last-child .action-btn {
                width: 100%;
                min-width: 0;
                margin: 0;
                padding: 8px;
                white-space: normal;
            }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .students-filters,
            .no-print {
                display: none !important;
            }
            .students-hero-actions {
                display: none !important;
            }
            .pagination,
            .students-table .pagination {
                display: none !important;
            }
            .action-btn {
                display: none !important;
            }
            .students-page { background: #fff !important; }
            .students-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .students-body {
                margin-top: 0 !important;
                padding: 0 !important;
            }
            .students-card {
                box-shadow: none !important;
                border: 1px solid #ddd !important;
            }
            .students-table-wrap {
                overflow: visible !important;
                border: 1px solid #ccc !important;
            }
            .students-table {
                min-width: 0 !important;
                font-size: 11px !important;
            }
            .students-table thead th {
                background: #f0f8f3 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .students-table tr { page-break-inside: avoid; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="students-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="students-hero">
            <div class="students-hero-content">
                <div class="students-hero-left">
                    <div class="students-hero-icon">
                        <i class="fas fa-user-graduate"></i>
                    </div>
                    <div class="students-hero-titles">
                        <h1>إدارة الطلاب</h1>
                        <p>بيانات الطلاب — مدرسة نور الملائكة</p>
                        <asp:Label ID="lblStage" runat="server" CssClass="stage-badge" Visible="false" />
                    </div>
                </div>
                <div class="students-hero-actions">
                    <asp:Button ID="btnExportExcel" runat="server" Text="تصدير اكسل"
                        CssClass="students-btn-glass"
                        OnClick="btnExportExcel_Click" />
                    <asp:Button ID="btnExportWord" runat="server" Text="تصدير ورد"
                        CssClass="students-btn-glass"
                        OnClick="btnExportWord_Click" />
                    <asp:Button ID="btnAddStudent" runat="server" Text="إضافة طالب جديد"
                        CssClass="students-btn-glass students-btn-glass-primary"
                        OnClick="btnAddStudent_Click" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="students-body">

            <!-- ⭐ Flash Message (تظهر بعد إعادة التوجيه) -->
            <asp:Label ID="lblFlashMessage" runat="server" CssClass="d-none" />

            <div class="students-card">

                <div class="students-card-header">
                    <div class="students-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="students-card-header-text">
                        <div>
                            <h3>قائمة الطلاب</h3>
                        </div>
                    </div>
                    <span class="students-count-pill">
                        <i class="fas fa-users"></i>
                        <asp:Label ID="lblCount" runat="server" Text="0" />
                    </span>
                </div>

                <div class="students-card-body">

                    <!-- Filters -->
                    <div class="students-filters no-print">

                        <div class="students-field">
                            <label><i class="fas fa-search"></i> بحث بالاسم</label>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                                placeholder="اكتب اسم الطالب..." />
                        </div>

                        <div class="students-field">
                            <label><i class="fas fa-id-card"></i> رقم الطالب</label>
                            <asp:TextBox ID="txtStudentNo" runat="server" CssClass="form-control"
                                placeholder="رقم التسجيل" />
                        </div>

                        <div class="students-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlClassRoomFilter" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlClassRoomFilter_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="students-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSectionFilter" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSectionFilter_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الأقسام" Value="0" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="students-field">
                            <label><i class="fas fa-chair"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlClassRoom" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlClassRoom_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="جميع الشعب" />
                            </asp:DropDownList>
                        </div>

                        <div class="students-field">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnSearch" runat="server" Text="بحث"
                                CssClass="students-btn-primary" OnClick="btnSearch_Click" />
                        </div>

                    </div>

                    <!-- No Results -->
                    <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="students-empty">
                        <div class="students-empty-icon">
                            <i class="fas fa-user-graduate"></i>
                        </div>
                        <h4>لا توجد نتائج مطابقة</h4>
                        <p>جرّب تغيير كلمة البحث أو إزالة الفلاتر</p>
                    </asp:Panel>

                    <!-- Table -->
                    <div class="table-responsive students-table-wrap">
                        <asp:GridView ID="gvStudents" runat="server"
                            AutoGenerateColumns="False"
                            CssClass="students-table"
                            EmptyDataText=""
                            OnRowCommand="gvStudents_RowCommand"
                            OnRowDataBound="gvStudents_RowDataBound"
                            AllowPaging="True"
                            PageSize="200"
                            OnPageIndexChanging="gvStudents_PageIndexChanging"
                            ShowHeaderWhenEmpty="true"
                            GridLines="None">

                            <Columns>
                                <asp:TemplateField HeaderText="اسم الطالب">
                                    <ItemTemplate>
                                        <div class="student-name-cell">
                                            <span><%#: Eval("Name") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="رقم التسجيل" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="reg-no-badge"><%#: Eval("RegistrationNo") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الرقم الوطني" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("NationalId")))
                                            ? "<span class='text-muted'>—</span>"
                                            : "<span class='national-id' dir='ltr'>" + HttpUtility.HtmlEncode(Convert.ToString(Eval("NationalId"))) + "</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الصف" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="class-badge"><%#: FormatClassRoom(Eval("ClassRoom")) %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="القسم" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# FormatSectionBadge(Eval("Section")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الشعبة" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# FormatSectionRoom(Eval("ClassRoom")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الجنس" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="gender-badge"><%#: FormatGender(Eval("Gender")) %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# SafeToBool(Eval("IsActive"))
                                            ? "<span class='status-badge active'><i class='fas fa-check-circle' style='font-size:10px;'></i> نشط</span>"
                                            : "<span class='status-badge inactive'><i class='fas fa-pause-circle' style='font-size:10px;'></i> ملغي</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                               <asp:TemplateField HeaderText="إجراءات" ItemStyle-CssClass="text-center">
    <ItemTemplate>
        <div class="d-flex gap-1 justify-content-center align-items-center">
            <asp:LinkButton ID="lnkEdit" runat="server"
                CommandName="EditStudent"
                CommandArgument='<%# Eval("Id") %>'
                CssClass="action-btn edit"
                ToolTip="تعديل">
                <i class="fas fa-edit"></i>
            </asp:LinkButton>

            <asp:LinkButton ID="lnkDetails" runat="server"
                CommandName="ViewDetails"
                CommandArgument='<%# Eval("Id") %>'
                CssClass="action-btn view"
                ToolTip="تفاصيل">
                <i class="fas fa-eye"></i>
            </asp:LinkButton>

            <asp:LinkButton ID="lnkToggle" runat="server"
                CommandName="ToggleStatus"
                CommandArgument='<%# Eval("Id") %>'
                CssClass="action-btn toggle"
                ToolTip="تبديل الحالة">
                <i class="fas fa-sync-alt"></i>
            </asp:LinkButton>

            <%-- ⭐ بدون OnClientClick — الانتقال المباشر لصفحة الحذف --%>
            <asp:LinkButton ID="lnkDeleteHard" runat="server"
                CommandName="DeleteHard"
                CommandArgument='<%# Eval("Id") %>'
                CssClass="action-btn danger"
                ToolTip="حذف نهائي">
                <i class="fas fa-trash"></i>
            </asp:LinkButton>
        </div>
    </ItemTemplate>
</asp:TemplateField>
                            </Columns>

                            <PagerStyle CssClass="pagination justify-content-center" />

                        </asp:GridView>
                    </div>

                </div>
            </div>

        </div>
    </div>
</asp:Content>
