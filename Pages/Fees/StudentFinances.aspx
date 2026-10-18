<%@ Page Title="رسوم الطالب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentFinances.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Fees.StudentFinances" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Student Finances
           ========================================================= */
        .sf-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .sf-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .sf-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .sf-hero::after {
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
        .sf-hero-content {
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
        .sf-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .sf-hero-icon {
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
        .sf-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .sf-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .sf-stage-label {
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
        .sf-stage-label i { font-size: 11px; }

        /* Hero action buttons */
        .sf-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .sf-btn-glass {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 10px 18px;
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
        .sf-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .sf-btn-glass.primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .sf-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }
        .sf-btn-glass.warning {
            background: rgba(245,158,11,0.95);
            color: #fff;
            border-color: rgba(245,158,11,1);
        }
        .sf-btn-glass.warning:hover { background: #f59e0b; color: #fff; }
        .sf-btn-glass.blue {
            background: rgba(13,110,253,0.95);
            color: #fff;
            border-color: rgba(13,110,253,1);
        }
        .sf-btn-glass.blue:hover { background: #0d6efd; color: #fff; }
        .sf-btn-glass.move {
            background: rgba(147,0,177,0.95);
            color: #fff;
            border-color: rgba(147,0,177,1);
        }
        .sf-btn-glass.move:hover { background: #9300b1; color: #fff; }

        /* ---------- BODY ---------- */
        .sf-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .sf-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .sf-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .sf-card-header-icon {
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
        .sf-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .sf-card-header-icon.orange {
            background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%);
            box-shadow: 0 6px 16px rgba(253,126,20,0.3);
        }
        .sf-card-header-icon.purple {
            background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%);
            box-shadow: 0 6px 16px rgba(111,66,193,0.3);
        }
        .sf-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .sf-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .sf-card-body {
            padding: 20px 24px;
        }

        /* ---------- FILTERS ---------- */
        .sf-filters {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
            align-items: end;
        }
        .sf-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .sf-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .sf-field label i { color: #00b14f; font-size: 12px; }
        .sf-field .form-control,
        .sf-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 12.5px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px;
            width: 100%;
        }
        .sf-field .form-control:focus,
        .sf-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- TABLE ---------- */
        .sf-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .sf-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .sf-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33;
            font-size: 11.5px;
            font-weight: 800;
            padding: 14px 10px !important;
            text-align: right;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
            letter-spacing: 0.2px;
            text-align: revert;
        }
        .sf-table tbody td {
            font-size: 13px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: center;
        }
        .sf-table tbody tr:hover td { background: #f8fdfa; }
        .sf-table tbody tr:last-child td { border-bottom: none; }

        /* Money badges */
        .money-value { font-weight: 700;  }
        .money-value.green  { color: #00b14f; }
        .money-value.red    { color: #dc3545; }
        .money-value.blue   { color: #0d6efd; }
        .money-value.gray   { color: #495057; }

        /* Status badges */
        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            white-space: nowrap;
        }
        .status-badge.paid {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .status-badge.partial {
            background: rgba(255,193,7,0.15);
            color: #a07800;
            border: 1px solid rgba(255,193,7,0.35);
        }
        .status-badge.unpaid {
            background: rgba(220,53,69,0.1);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .status-badge.no-invoice {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }

        /* ✅ Notes badge (لعمود ملاحظة) */
        .sf-note-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 10px;
            border-radius: 8px;
            background: rgba(108,117,125,0.08);
            color: #495057;
            font-size: 11.5px;
            font-weight: 700;
            border: 1px solid rgba(108,117,125,0.2);
           
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }
        .sf-note-badge.empty {
            background: #f8f9fa;
            color: #adb5bd;
            border-color: #e9ecef;
            font-weight: 600;
        }

        /* ✅ Action buttons for payment rows */
        .sf-btn-payment-action {
            display: inline-flex;
            align-items: center;
            justify-content: center;
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
        .sf-btn-payment-action.edit {
            border-color: #0d6efd;
            color: #0d6efd;
        }
        .sf-btn-payment-action.edit:hover {
            background: rgba(13,110,253,0.08);
            color: #0a58ca;
            text-decoration: none;
        }
        .sf-btn-payment-action.delete {
            border-color: #dc3545;
            color: #dc3545;
        }
        .sf-btn-payment-action.delete:hover {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            text-decoration: none;
        }

        /* ✅ زر إجراء الفاتورة (view / edit / delete) */
        .sf-btn-invoice-action {
            display: inline-flex;
            align-items: center;
            justify-content: center;
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
        .sf-btn-invoice-action.view {
            border-color: #00b14f;
            color: #00b14f;
        }
        .sf-btn-invoice-action.view:hover {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            text-decoration: none;
        }
        .sf-btn-invoice-action.edit {
            border-color: #0d6efd;
            color: #0d6efd;
        }
        .sf-btn-invoice-action.edit:hover {
            background: rgba(13,110,253,0.08);
            color: #0a58ca;
            text-decoration: none;
        }
        .sf-btn-invoice-action.delete {
            border-color: #dc3545;
            color: #dc3545;
        }
        .sf-btn-invoice-action.delete:hover {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            text-decoration: none;
        }

        /* Quick action buttons (below hero) */
        .sf-actions-row {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-bottom: 20px;
        }

        /* Payment group card */
        .sf-payment-group {
            border: 1px solid #99f6c3;
            border-radius: 14px;
            overflow: hidden;
            margin-bottom: 14px;
            background: #fff;
            transition: all 0.15s ease;
        }
        .sf-payment-group:hover {
            border-color: #d8e6dd;
            box-shadow: 0 4px 12px rgba(0,177,79,0.06);
        }
        .sf-payment-group-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 12px 18px;
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            border-bottom: 1px solid #eef1f3;
            flex-wrap: wrap;
            gap: 10px;
        }
        .sf-payment-group-title {
            display: flex;
            align-items: center;
            gap: 10px;
            font-size: 14px;
            font-weight: 800;
            color: #007a33;
        }
        .sf-payment-group-title-icon {
            width: 28px;
            height: 28px;
            border-radius: 8px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
        }
        .sf-payment-group-meta {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }
        .sf-pill {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
        }
        .sf-pill.gray { background: #f8f9fa; color: #495057; border: 1px solid #e9ecef; }
        .sf-pill.blue { background: rgba(0,123,177,0.1); color: #007b9f; border: 1px solid rgba(0,123,177,0.25); }

        /* Empty state */
        .sf-empty {
            text-align: center;
            padding: 50px 20px;
        }
        .sf-empty-icon {
            width: 76px; height: 76px; border-radius: 22px;
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            color: #00b14f;
            display: inline-flex; align-items: center; justify-content: center;
            font-size: 32px; margin-bottom: 16px;
        }
        .sf-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 6px; }
        .sf-empty p { color: #6c757d; font-size: 13px; margin: 0; }

        /* Responsive */
        @media (max-width: 992px) {
            .sf-hero { padding: 26px 20px 72px; }
            .sf-hero-titles h1 { font-size: 19px; }
            .sf-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .sf-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .sf-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .sf-hero-titles h1 { font-size: 16px; }
            .sf-card-body { padding: 16px; }

            /* عرض كل أزرار الطالب في شبكة مرنة بدل قصّها خارج عرض الهاتف. */
            .sf-hero-content { align-items: flex-start; gap: 14px; }
            .sf-hero-left { width: 100%; min-width: 0; gap: 12px; }
            .sf-hero-titles { min-width: 0; }
            .sf-hero-actions {
                display: grid;
                grid-template-columns: repeat(2, minmax(0, 1fr));
                flex: 1 1 100%;
                width: 100%;
                min-width: 0;
                max-width: 100%;
                gap: 8px;
            }
            .sf-btn-glass {
                box-sizing: border-box;
                justify-content: center;
                width: 100%;
                min-width: 0;
                padding: 9px 6px;
                gap: 6px;
                text-align: center;
                white-space: normal;
                line-height: 1.4;
            }
        }

        /* Print */
        @media print {
            .sf-hero-actions, .sf-actions-row, .sf-filters, .no-print,
            .sf-btn-payment-action, .sf-btn-invoice-action { display: none !important; }
            .sf-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .sf-body { margin-top: 0 !important; padding: 0 !important; }
            .sf-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }

        th {
              text-align: revert;
              padding-bottom: 15px;
              padding-top: 15px;
              background-color: #f0fff5;
            }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="sf-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="sf-hero">
            <div class="sf-hero-content">
                <div class="sf-hero-left">
                    <div class="sf-hero-icon">
                        <i class="fas fa-money-bill-wave"></i>
                    </div>
                    <div class="sf-hero-titles">
                        <h1>
                            رسوم إشتراك الطالب
                            <span class="sf-stage-label">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:Literal ID="lblStage" runat="server" />
                            </span>
                        </h1>
                        <p> الرسوم، المدفوعات، والفواتير الخاصة بالطالب</p>
                    </div>
                </div>

                <!-- أزرار سياقية داخل الهيرو (تظهر فقط عند اختيار طالب) -->
                <asp:Panel ID="pnlQuickActions" runat="server" Visible="false" CssClass="sf-hero-actions no-print">
                    <a id="lnkGoToInvoice" runat="server" class="sf-btn-glass primary">
                        <i class="fas fa-file-invoice"></i> إنشاء فاتورة
                    </a>
                    <a id="lnkGoToAddPayment" runat="server" class="sf-btn-glass warning">
                        <i class="fas fa-money-bill-wave"></i> إضافة دفعة
                    </a>
                    <a id="lnkGoToInvoices" runat="server" class="sf-btn-glass blue">
                        <i class="fas fa-file-invoice"></i> إدارة الفواتير
                    </a>
                    <a id="lnkGoToStudentReport" runat="server" class="sf-btn-glass move">
                        <i class="fas fa-file-alt"></i> التقرير المالي
                    </a>
                </asp:Panel>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="sf-body">

            <!-- فلاتر -->
            <div class="sf-card">
                <div class="sf-card-header">
                    <div class="sf-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="sf-card-header-text">
                        <h3>الطالب</h3>
                        
                    </div>
                </div>
                <div class="sf-card-body">
                    <div class="sf-filters no-print">

                        <div class="sf-field">
                            <label><i class="fas fa-calendar-alt"></i> السنة الدراسية</label>
                            <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="-- اختر السنة --" />
                            </asp:DropDownList>
                        </div>

                        <div class="sf-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="-- اختر الصف --" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="sf-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الأقسام" Value="" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="sf-field">
                            <label><i class="fas fa-chair"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الشعب" Value="" Selected="True" />
                            </asp:DropDownList>
                        </div>

                        <div class="sf-field">
                            <label><i class="fas fa-user-graduate"></i> الطالب</label>
                            <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true" Enabled="false"
                                OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="-- اختر الطالب --" />
                            </asp:DropDownList>
                        </div>

                    </div>
                </div>
            </div>

            <!-- جدول الرسوم -->
            <asp:Panel ID="pnlFees" runat="server" Visible="false">
                <div class="sf-card">
                    <div class="sf-card-header">
                        <div class="sf-card-header-icon">
                            <i class="fas fa-file-invoice-dollar"></i>
                        </div>
                        <div class="sf-card-header-text">
                            <h3>الرسوم المقررة</h3>
                            <p>قائمة الرسوم الخاصة بالطالب مع حالة السداد</p>
                        </div>
                    </div>
                    <div class="sf-card-body">
                        <div class="sf-table-wrap">
                            <asp:GridView ID="gvStudentFees" runat="server"
                                CssClass="sf-table"
                                AutoGenerateColumns="false"
                                EmptyDataText="لا توجد رسوم"
                                GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="FeeName" HeaderText="اسم الرسوم" />
                                    <asp:BoundField DataField="Amount" HeaderText="القيمة الأصلية" DataFormatString="{0:N2}" />
                                    <asp:BoundField DataField="DiscountAmount" HeaderText="الخصم/الإعفاء" DataFormatString="{0:N2}" />
                                    <asp:BoundField DataField="NetAmount" HeaderText="المستحق" DataFormatString="{0:N2}" />
                                   
                                    <asp:BoundField DataField="Remaining" HeaderText="المتبقي" DataFormatString="{0:N2}" />
                                     <asp:BoundField DataField="PaidAmount" HeaderText="المدفوع" DataFormatString="{0:N2}" />
                                    <asp:BoundField DataField="Status" HeaderText="الحالة" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- سجل المدفوعات -->
            <asp:Panel ID="pnlPayments" runat="server" Visible="false">
                <div class="sf-card">
                    <div class="sf-card-header">
                        <div class="sf-card-header-icon orange">
                            <i class="fas fa-money-bill-wave"></i>
                        </div>
                        <div class="sf-card-header-text">
                            <h3>سجل المدفوعات</h3>
                            <p>الدفعات مجمّعة حسب نوع الرسم</p>
                        </div>
                    </div>
                    <div class="sf-card-body">
                        <%-- ✅ الـ Repeater الخارجي: بدون OnItemCommand --%>
                        <asp:Repeater ID="rptPaymentGroups" runat="server">
                            <ItemTemplate>
                                <div class="sf-payment-group">
                                    <div class="sf-payment-group-header">
                                        <div class="sf-payment-group-title">
                                            <span class="sf-payment-group-title-icon">
                                                <i class="fas fa-file-invoice-dollar"></i>
                                            </span>
                                            <%# Eval("FeeName") %>
                                        </div>
                                        <div class="sf-payment-group-meta">
                                            <span class="sf-pill gray">
                                                <i class="fas fa-list-ol"></i>
                                                عدد الدفعات: <strong><%# Eval("Count") %></strong>
                                            </span>
                                            <span class="sf-pill blue">
                                                <i class="fas fa-coins"></i>
                                                الإجمالي: <%# Eval("TotalAmountText") %> د.ل
                                            </span>
                                        </div>
                                    </div>
                                    <div style="padding:0;">
                                        <table class="sf-table mb-0">
                                            <thead>
                                                <tr>
                                                    <th style="width:50px;text-align:center;">#</th>
                                                    <th>رقم الإيصال</th>
                                                    <th style="text-align:center;">المبلغ</th>
                                                    <th>التاريخ</th>
                                                    <th>طريقة الدفع</th>
                                                    <%-- ✅ عمود ملاحظة --%>
                                                    <th>ملاحظة</th>
                                                    <%-- ✅ عمود إجراءات --%>
                                                    <th style="text-align:center;">إجراءات</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <%-- ✅ الـ Repeater الداخلي: هنا OnItemCommand --%>
                                                <asp:Repeater ID="rptGroupPayments" runat="server"
                                                    DataSource='<%# Eval("Payments") %>'
                                                    OnItemCommand="rptGroupPayments_ItemCommand">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td style="text-align:center;">
                                                                <span style="display:inline-flex;align-items:center;justify-content:center;width:26px;height:26px;border-radius:8px;background:#f8f9fa;color:#6c757d;font-weight:800;font-size:11px;border:1px solid #e9ecef;">
                                                                    <%# Container.ItemIndex + 1 %>
                                                                </span>
                                                            </td>
                                                            <td>
                                                                <span style="color:#00b14f;font-weight:700;font-size:13px;">
                                                                    <i class="fas fa-receipt" style="font-size:11px;margin-inline-end:4px;"></i>
                                                                    <%# Eval("ReceiptNumber") %>
                                                                </span>
                                                            </td>
                                                            <td style="text-align:center;">
                                                                <span class="money-value green"><%# Eval("AmountText") %> د.ل</span>
                                                            </td>
                                                            <td>
                                                                <span style="color:#6c757d;font-size:12.5px;">
                                                                    <i class="fas fa-calendar-alt" style="color:#00b14f;font-size:11px;margin-inline-end:4px;"></i>
                                                                    <%# Eval("PaymentDate", "{0:yyyy-MM-dd}") %>
                                                                </span>
                                                            </td>
                                                            <td>
                                                                <span class="status-badge paid"><%# Eval("MethodName") %></span>
                                                            </td>
                                                            <%-- ✅ خلية الملاحظة --%>
                                                            <td>
                                                                <%# RenderNote(Eval("Notes")) %>
                                                            </td>
                                                            <%-- ✅ خلية الإجراءات (تعديل + حذف) --%>
                                                            <td style="text-align:center;">
                                                                <div class="d-flex gap-1 justify-content-center flex-wrap">
                                                                    <asp:LinkButton ID="btnEditPayment" runat="server"
                                                                        CommandName="EditPayment"
                                                                        CommandArgument='<%# Eval("Id") %>'
                                                                        CssClass="sf-btn-payment-action edit"
                                                                        ToolTip="تعديل الدفعة">
                                                                        <i class="fas fa-edit"></i> تعديل
                                                                    </asp:LinkButton>
                                                                    <asp:LinkButton ID="btnDeletePayment" runat="server"
                                                                        CommandName="DeletePayment"
                                                                        CommandArgument='<%# Eval("Id") %>'
                                                                        CssClass="sf-btn-payment-action delete"
                                                                        ToolTip="حذف الدفعة"
                                                                        OnClientClick="return confirm('هل أنت متأكد من حذف هذه الدفعة؟ لا يمكن التراجع عن هذه العملية.');">
                                                                        <i class="fas fa-trash"></i> حذف
                                                                    </asp:LinkButton>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </asp:Panel>

            <!-- فواتير الطالب -->
            <asp:Panel ID="pnlInvoices" runat="server" Visible="false">
                <div class="sf-card">
                    <div class="sf-card-header">
                        <div class="sf-card-header-icon purple">
                            <i class="fas fa-file-invoice"></i>
                        </div>
                        <div class="sf-card-header-text">
                            <h3>فواتير الطالب</h3>
                            <p>جميع الفواتير الصادرة للطالب خلال السنة الدراسية</p>
                        </div>
                    </div>
                    <div class="sf-card-body">

                        <asp:Label ID="lblInvoiceMessage" runat="server" CssClass="d-block mb-2" />

                        <div class="sf-table-wrap">
                            <asp:GridView ID="gvStudentInvoices" runat="server"
                                CssClass="sf-table"
                                AutoGenerateColumns="false"
                                EmptyDataText="لا توجد فواتير لهذا الطالب"
                                GridLines="None"
                                DataKeyNames="Id"
                                OnRowCommand="gvStudentInvoices_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="StudentName" HeaderText="اسم الطالب" />
                                    <asp:BoundField DataField="GradeLevelName" HeaderText="الصف" />
                                    <asp:TemplateField HeaderText="القسم">
                                        <ItemTemplate><%# FormatSection(Eval("Section")) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="InvoiceDate" HeaderText="التاريخ"
                                        DataFormatString="{0:yyyy-MM-dd}" HeaderStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText="الإجمالي" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <span class="money-value blue">
                                                <%# ((decimal)Eval("TotalAmount")).ToString("N2") %> د.ل
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-md-end" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="الخصم" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <span class="money-value red">
                                                <%# ((decimal)Eval("DiscountAmount")).ToString("N2") %> د.ل
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-md-end" />
                                    </asp:TemplateField>
                                   
                                    <asp:TemplateField HeaderText="المدفوع" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <span class="money-value green">
                                                <%# ((decimal)Eval("PaidAmount")).ToString("N2") %> د.ل
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-md-end" />
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="المتبقي" HeaderStyle-HorizontalAlign="Center">
                                         <ItemTemplate>
                                             <span class="money-value red">
                                                 <%# ((decimal)Eval("RemainingAmount")).ToString("N2") %> د.ل
                                             </span>
                                         </ItemTemplate>
                                         <ItemStyle CssClass="text-md-end" />
                                     </asp:TemplateField>
                                    <asp:TemplateField HeaderText="إجراءات" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <div class="d-flex gap-1 justify-content-center flex-wrap">
                                                <%-- ✅ زر عرض --%>
                                                <a href='<%# ResolveClientUrl("~/Pages/Invoices/Details.aspx") + "?id=" + Eval("Id") %>'
                                                   class="sf-btn-invoice-action view"
                                                   title="عرض الفاتورة">
                                                    <i class="fas fa-eye"></i> عرض
                                                </a>
                                                <%-- ✅ زر تعديل (جديد) --%>
                                                <a href='<%# ResolveClientUrl("~/Pages/Invoices/Edit.aspx") + "?id=" + Eval("Id") + "&studentId=" + Eval("StudentId") + "&stage=" + Server.UrlEncode(Session["Stage"] as string ?? "Basic") %>'
                                                   class="sf-btn-invoice-action edit"
                                                   title="تعديل الفاتورة">
                                                    <i class="fas fa-edit"></i> تعديل
                                                </a>
                                                <%-- ✅ زر حذف --%>
                                                <asp:LinkButton ID="btnDeleteInvoice" runat="server"
                                                    CommandName="DeleteInvoice"
                                                    CommandArgument='<%# Eval("Id") %>'
                                                    CssClass="sf-btn-invoice-action delete"
                                                    ToolTip="حذف الفاتورة"
                                                    OnClientClick="return confirm('هل أنت متأكد من حذف هذه الفاتورة؟ لا يمكن التراجع.');">
                                                    <i class="fas fa-trash"></i> حذف
                                                </asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-center" Width="300px" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>
</asp:Content>
