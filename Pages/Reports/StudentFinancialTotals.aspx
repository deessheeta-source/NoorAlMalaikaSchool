<%@ Page Title="التقرير المالي الإجمالي للطلاب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentFinancialTotals.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Reports.StudentFinancialTotals" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Student Financial Totals Report
           ========================================================= */
        .sft-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .sft-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .sft-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .sft-hero::after {
            content: '';
            position: absolute;
            top: -40%; left: -10%;
            width: 340px; height: 340px; border-radius: 50%;
            background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%);
            pointer-events: none;
        }
        .sft-hero-content {
            position: relative; z-index: 2;
            display: flex; align-items: center; justify-content: space-between;
            gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto;
        }
        .sft-hero-left { display: flex; align-items: center; gap: 20px; }
        .sft-hero-icon {
            width: 68px; height: 68px; border-radius: 20px;
            background: rgba(255,255,255,0.18); backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,0.3); color: #fff;
            display: inline-flex; align-items: center; justify-content: center;
            font-size: 30px; flex-shrink: 0;
            box-shadow: 0 8px 24px rgba(0,0,0,0.12);
        }
        .sft-hero-titles h1 {
            font-size: 22px; font-weight: 800; color: #fff;
            margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px;
        }
        .sft-hero-titles p {
            font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5;
        }
        .sft-hero-actions {
            display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0;
        }
        .sft-btn-glass {
            display: inline-flex; align-items: center; gap: 8px;
            padding: 10px 20px; border-radius: 12px;
            background: rgba(255,255,255,0.15); backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,0.3);
            color: #fff; font-size: 13px; font-weight: 700;
            text-decoration: none; transition: all 0.2s ease;
            cursor: pointer; white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .sft-btn-glass:hover {
            background: rgba(255,255,255,0.25); color: #fff;
            text-decoration: none; transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .sft-body {
            max-width: 1280px; margin: -56px auto 0;
            padding: 0 24px 40px; position: relative; z-index: 3;
        }

        /* ---------- CARD ---------- */
        .sft-card-wrap {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .sft-card-header {
            display: flex; align-items: center; gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .sft-card-header-icon {
            width: 44px; height: 44px; border-radius: 13px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex; align-items: center; justify-content: center;
            font-size: 18px; flex-shrink: 0;
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
        }
        .sft-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .sft-card-header-text h3 {
            margin: 0; font-size: 16px; font-weight: 800; color: #212529;
        }
        .sft-card-header-text p {
            margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500;
        }
        .sft-card-body { padding: 20px 24px; }

        /* ---------- FILTERS ---------- */
        .sft-filters {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 14px;
            align-items: end;
        }
        .sft-field {
            display: flex; flex-direction: column; gap: 6px; min-width: 0;
        }
        .sft-field label {
            font-size: 12.5px; font-weight: 700; color: #495057;
            letter-spacing: 0.2px;
            display: flex; align-items: center; gap: 6px;
        }
        .sft-field label i { color: #00b14f; font-size: 12px; }
        .sft-field .form-control,
        .sft-field .form-select {
            border: 1.5px solid #e9ecef; border-radius: 11px;
            padding: 10px 14px; font-size: 12.5px; font-weight: 600;
            color: #212529; transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px; width: 100%;
        }
        .sft-field .form-control:focus,
        .sft-field .form-select:focus {
            border-color: #00b14f; background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- BUTTONS ---------- */
        .sft-btn-primary {
            display: inline-flex; align-items: center; justify-content: center;
            gap: 8px; padding: 11px 28px; border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none; color: #fff; font-size: 13.5px; font-weight: 800;
            cursor: pointer; transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
            height: 44px; white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .sft-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff;
        }
        .sft-btn-outline {
            display: inline-flex; align-items: center; justify-content: center;
            gap: 8px; padding: 11px 24px; border-radius: 11px;
            background: #fff; border: 1.5px solid #dee2e6;
            color: #495057; font-size: 13.5px; font-weight: 700;
            cursor: pointer; transition: all 0.2s ease;
            height: 44px; text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .sft-btn-outline:hover {
            border-color: #6c757d; background: #f8f9fa;
            color: #343a40; text-decoration: none;
        }
        .sft-actions {
            display: flex; justify-content: flex-end; gap: 10px;
            flex-wrap: wrap; margin-top: 16px;
        }

        /* ---------- STAT CARDS ---------- */
        .sft-stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 16px;
            margin-bottom: 20px;
        }
        .sft-stat-card {
            display: flex;
            align-items: center;
            gap: 16px;
            padding: 20px 10px;
            border-radius: 18px;
            background: #ffffff;
            border: 1px solid #99f6c3;
            box-shadow: 0 4px 12px rgba(0,0,0,0.03);
            transition: all 0.2s ease;
            position: relative;
            overflow: hidden;
        }
        .sft-stat-card::before {
            content: '';
            position: absolute;
            top: 0;
            inset-inline-start: 0;
            width: 4px;
            height: 100%;
            border-radius: 0 4px 4px 0;
        }
        .sft-stat-card.blue::before   { background: linear-gradient(180deg, #0d6efd 0%, #3d8bfd 100%); }
        .sft-stat-card.green::before  { background: linear-gradient(180deg, #00b14f 0%, #4ed17e 100%); }
        .sft-stat-card.red::before    { background: linear-gradient(180deg, #dc3545 0%, #f0616d 100%); }
        .sft-stat-card.orange::before { background: linear-gradient(180deg, #fd7e14 0%, #ffa94d 100%); }
        .sft-stat-card.purple::before { background: linear-gradient(180deg, #6f42c1 0%, #9d6ee6 100%); }
        .sft-stat-card.teal::before   { background: linear-gradient(180deg, #10a6a0 0%, #4dd6d0 100%); }
        .sft-stat-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 24px rgba(0,0,0,0.08);
        }
        .sft-stat-icon {
            width: 35px;
            height: 35px;
            border-radius: 16px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
            color: #fff;
        }
        .sft-stat-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .sft-stat-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .sft-stat-icon.red    { background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); box-shadow: 0 6px 16px rgba(220,53,69,0.3); }
        .sft-stat-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .sft-stat-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .sft-stat-icon.teal   { background: linear-gradient(135deg, #10a6a0 0%, #4dd6d0 100%); box-shadow: 0 6px 16px rgba(16,166,160,0.3); }
        .sft-stat-info { display: flex; flex-direction: column; gap: 4px; min-width: 0; }
        .sft-stat-label {
            font-size: 12px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        .sft-stat-value {
            font-size: 18px;
            font-weight: 700;
            line-height: 1.1;
            font-family: 'Courier New', monospace;
        }
        .sft-stat-value.blue   { color: #0d6efd; }
        .sft-stat-value.green  { color: #00b14f; }
        .sft-stat-value.red    { color: #dc3545; }
        .sft-stat-value.orange { color: #fd7e14; }
        .sft-stat-value.purple { color: #6f42c1; }
        .sft-stat-value.teal   { color: #10a6a0; }
        .sft-stat-value small {
            font-size: 13px; opacity: 0.8;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }

        /* ---------- SCOPE NOTE ---------- */
        .sft-scope {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 14px 18px;
            border-radius: 12px;
            background: linear-gradient(135deg, rgba(0,177,79,0.06) 0%, rgba(0,177,79,0.02) 100%);
            border: 1px solid rgba(0,177,79,0.15);
            font-size: 12.5px;
            font-weight: 600;
            color: #007a33;
            margin-top: 4px;
        }
        .sft-scope i { font-size: 14px; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .sft-hero { padding: 26px 20px 72px; }
            .sft-hero-titles h1 { font-size: 19px; }
            .sft-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .sft-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .sft-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .sft-hero-titles h1 { font-size: 16px; }
            .sft-card-body { padding: 16px; }
            .sft-stat-value { font-size: 18px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .sft-hero-actions, .sft-actions, .no-print { display: none !important; }
            .sft-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .sft-body { margin-top: 0 !important; padding: 0 !important; }
            .sft-card-wrap, .sft-stat-card {
                box-shadow: none !important;
                border: 1px solid #ddd !important;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="sft-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="sft-hero">
            <div class="sft-hero-content">
                <div class="sft-hero-left">
                    <div class="sft-hero-icon">
                        <i class="fas fa-chart-pie"></i>
                    </div>
                    <div class="sft-hero-titles">
                        <h1>التقرير المالي الإجمالي للطلاب</h1>
                        <p>إجماليات الرسوم والخصومات والمبالغ المالية حسب السنة الدراسية والصف</p>
                    </div>
                </div>
                <div class="sft-hero-actions no-print">
                    <button type="button" onclick="window.print()" class="sft-btn-glass">
                        <i class="fas fa-print"></i> طباعة
                    </button>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="sft-body">

            <!-- فلاتر -->
            <div class="sft-card-wrap">
                <div class="sft-card-header">
                    <div class="sft-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="sft-card-header-text">
                        <h3>معايير التقرير</h3>
                        <p>اختر السنة الدراسية والصف لتحديد نطاق التقرير</p>
                    </div>
                </div>
                <div class="sft-card-body">
                    <div class="sft-filters">

                        <div class="sft-field">
                            <label><i class="fas fa-calendar-alt"></i> السنة الدراسية</label>
                            <asp:DropDownList ID="ddlYear" runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="FilterChanged" />
                        </div>

                        <div class="sft-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="FilterChanged" />
                        </div>

                    </div>

                    <div class="sft-actions no-print">
                        <asp:Button ID="btnRefresh" runat="server"
                            Text="تحديث التقرير"
                            CssClass="sft-btn-primary"
                            OnClick="btnRefresh_Click" />
                    </div>
                </div>
            </div>

            <!-- بطاقات الإحصائيات (5 بطاقات) -->
            <div class="sft-stats-grid">

                <!-- 1. إجمالي مبلغ الاشتراك -->
                <div class="sft-stat-card blue">
                    <div class="sft-stat-icon blue">
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="sft-stat-info">
                        <span class="sft-stat-label">إجمالي مبلغ الاشتراك</span>
                        <span class="sft-stat-value blue">
                            <asp:Literal ID="litSubscription" runat="server" />
                            <small>د.ل</small>
                        </span>
                    </div>
                </div>

                <!-- 2. إجمالي الخصم -->
                <div class="sft-stat-card purple">
                    <div class="sft-stat-icon purple">
                        <i class="fas fa-tags"></i>
                    </div>
                    <div class="sft-stat-info">
                        <span class="sft-stat-label">إجمالي الخصم</span>
                        <span class="sft-stat-value purple">
                            <asp:Literal ID="litDiscount" runat="server" />
                            <small>د.ل</small>
                        </span>
                    </div>
                </div>

                <!-- 3. إجمالي المبلغ المستحق -->
                <div class="sft-stat-card teal">
                    <div class="sft-stat-icon teal">
                        <i class="fas fa-coins"></i>
                    </div>
                    <div class="sft-stat-info">
                        <span class="sft-stat-label">إجمالي المبلغ المستحق</span>
                        <span class="sft-stat-value teal">
                            <asp:Literal ID="litDue" runat="server" />
                            <small>د.ل</small>
                        </span>
                    </div>
                </div>

                <!-- 4. إجمالي المبلغ المدفوع -->
                <div class="sft-stat-card green">
                    <div class="sft-stat-icon green">
                        <i class="fas fa-hand-holding-usd"></i>
                    </div>
                    <div class="sft-stat-info">
                        <span class="sft-stat-label">إجمالي المبلغ المدفوع</span>
                        <span class="sft-stat-value green">
                            <asp:Literal ID="litPaid" runat="server" />
                            <small>د.ل</small>
                        </span>
                    </div>
                </div>

                <!-- 5. إجمالي المبلغ المتبقي -->
                <div class="sft-stat-card orange">
                    <div class="sft-stat-icon orange">
                        <i class="fas fa-exclamation-circle"></i>
                    </div>
                    <div class="sft-stat-info">
                        <span class="sft-stat-label">إجمالي المبلغ المتبقي</span>
                        <span class="sft-stat-value orange">
                            <asp:Literal ID="litRemaining" runat="server" />
                            <small>د.ل</small>
                        </span>
                    </div>
                </div>

            </div>

            <!-- نطاق التقرير -->
            <div class="sft-scope">
                <i class="fas fa-info-circle"></i>
                <asp:Literal ID="litScope" runat="server" />
            </div>

        </div>
    </div>
</asp:Content>