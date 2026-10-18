<%@ Page Title="حذف سياسة خصم" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.DiscountPolicies.Delete" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Delete Discount Policy
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
            background: linear-gradient(135deg, #dc3545 0%, #a51c1c 50%, #7a1010 100%);
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
            max-width: 900px;
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

        /* ---------- BODY ---------- */
        .dp-body {
            max-width: 900px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
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
            background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            flex-shrink: 0;
            box-shadow: 0 6px 16px rgba(220,53,69,0.3);
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
        .dp-card-body { padding: 20px 24px; }

        /* ---------- WARNING ALERT ---------- */
        .dp-warning {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 16px 20px;
            border-radius: 14px;
            background: linear-gradient(135deg, rgba(220,53,69,0.08) 0%, rgba(220,53,69,0.03) 100%);
            border: 1.5px solid rgba(220,53,69,0.25);
            color: #a51c1c;
            font-size: 14px;
            font-weight: 700;
            margin-bottom: 20px;
        }
        .dp-warning-icon {
            width: 44px;
            height: 44px;
            border-radius: 12px;
            background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            flex-shrink: 0;
            box-shadow: 0 4px 12px rgba(220,53,69,0.35);
        }
        .dp-warning-text {
            display: flex;
            flex-direction: column;
            gap: 2px;
            min-width: 0;
        }
        .dp-warning-text strong {
            font-size: 14px;
            color: #a51c1c;
        }
        .dp-warning-text span {
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 600;
        }
        .dp-warning-usage {
            display: block;
            margin-top: 6px;
            font-size: 12.5px;
            color: #a51c1c;
            font-weight: 700;
        }
        .dp-warning-usage:empty { display: none; }

        /* ---------- INFO GRID ---------- */
        .dp-info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 14px;
            margin-bottom: 20px;
        }
        .dp-info-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 14px 16px;
            border-radius: 14px;
            background: #f9fafb;
            border: 1px solid #99f6c3;
            transition: all 0.2s ease;
        }
        .dp-info-item:hover {
            border-color: #d8e6dd;
            background: #f8fdfa;
        }
        .dp-info-item.full {
            grid-column: 1 / -1;
        }
        .dp-info-item.highlight {
            background: linear-gradient(135deg, rgba(0,177,79,0.06) 0%, rgba(0,177,79,0.02) 100%);
            border-color: rgba(0,177,79,0.2);
        }
        .dp-info-item.highlight .dp-info-icon {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            box-shadow: 0 4px 12px rgba(0,177,79,0.3);
        }
        .dp-info-icon {
            width: 40px;
            height: 40px;
            border-radius: 11px;
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
            box-shadow: 0 4px 12px rgba(13,110,253,0.3);
        }
        .dp-info-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 4px 12px rgba(0,177,79,0.3); }
        .dp-info-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 4px 12px rgba(111,66,193,0.3); }
        .dp-info-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 4px 12px rgba(253,126,20,0.3); }
        .dp-info-content {
            display: flex;
            flex-direction: column;
            gap: 2px;
            min-width: 0;
            flex: 1;
        }
        .dp-info-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 4px;
        }
        .dp-info-value {
            font-size: 13.5px;
            font-weight: 800;
            color: #212529;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }
        .dp-info-value.mono {
            font-family: 'Courier New', monospace;
        }

        /* Value badge */
        .dp-value-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 14px;
            border-radius: 999px;
            font-size: 13px;
            font-weight: 800;
            background: linear-gradient(135deg, rgba(0,177,79,0.1) 0%, rgba(0,177,79,0.04) 100%);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
            font-family: 'Courier New', monospace;
            white-space: nowrap;
        }

        /* ---------- BUTTONS ---------- */
        .dp-btn-danger {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 12px 32px;
            border-radius: 11px;
            background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%);
            border: none;
            color: #fff;
            font-size: 14px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(220,53,69,0.3);
            height: 46px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .dp-btn-danger:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(220,53,69,0.4);
            color: #fff;
        }
        .dp-btn-outline {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 12px 28px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #dee2e6;
            color: #495057;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 46px;
            text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .dp-btn-outline:hover {
            border-color: #6c757d;
            background: #f8f9fa;
            color: #343a40;
            text-decoration: none;
        }
        .dp-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            flex-wrap: wrap;
            margin-bottom: 40px;
        }

        /* ---------- MESSAGE ---------- */
        .dp-message {
            display: block;
            padding: 14px 18px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 16px;
        }
        .dp-message.danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .dp-message.success {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .dp-message:empty { display: none; padding: 0; margin: 0; }

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
            .dp-card-body { padding: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .dp-hero-actions, .dp-actions, .no-print { display: none !important; }
            .dp-page { background: #fff !important; }
            .dp-hero {
                background: #dc3545 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .dp-body { margin-top: 0 !important; padding: 0 !important; }
            .dp-card { box-shadow: none !important; border: 1px solid #ddd !important; }
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
                        <i class="fas fa-trash-alt"></i>
                    </div>
                    <div class="dp-hero-titles">
                        <h1>حذف سياسة الخصم / الإعفاء</h1>
                        <p>يرجى مراجعة بيانات السياسة قبل التأكيد</p>
                    </div>
                </div>
                <div class="dp-hero-actions no-print">
                    <a href="Default.aspx" class="dp-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="dp-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="dp-message" />

            <asp:Panel ID="pnlDetails" runat="server">

                <div class="dp-card">
                    <div class="dp-card-header">
                        <div class="dp-card-header-icon">
                            <i class="fas fa-exclamation-triangle"></i>
                        </div>
                        <div class="dp-card-header-text">
                            <h3>تأكيد الحذف</h3>
                            <p>يرجى مراجعة بيانات السياسة قبل الحذف</p>
                        </div>
                    </div>
                    <div class="dp-card-body">

                        <!-- Warning Alert -->
                        <div class="dp-warning">
                            <div class="dp-warning-icon">
                                <i class="fas fa-exclamation-triangle"></i>
                            </div>
                            <div class="dp-warning-text">
                                <strong>هل أنت متأكد من حذف سياسة الخصم التالية؟</strong>
                                <span>لا يمكن التراجع عن هذه العملية بعد التأكيد</span>
                                <asp:Label ID="lblWarningUsages" runat="server" CssClass="dp-warning-usage" />
                            </div>
                        </div>

                        <!-- Info Grid -->
                        <div class="dp-info-grid">

                            <div class="dp-info-item highlight full">
                                <div class="dp-info-icon green">
                                    <i class="fas fa-percent"></i>
                                </div>
                                <div class="dp-info-content">
                                    <span class="dp-info-label">اسم السياسة</span>
                                    <span class="dp-info-value">
                                        <asp:Label ID="lblName" runat="server" />
                                    </span>
                                </div>
                            </div>

                            <div class="dp-info-item">
                                <div class="dp-info-icon">
                                    <i class="fas fa-tag"></i>
                                </div>
                                <div class="dp-info-content">
                                    <span class="dp-info-label">النوع</span>
                                    <span class="dp-info-value">
                                        <asp:Label ID="lblType" runat="server" />
                                    </span>
                                </div>
                            </div>

                            <div class="dp-info-item">
                                <div class="dp-info-icon purple">
                                    <i class="fas fa-calculator"></i>
                                </div>
                                <div class="dp-info-content">
                                    <span class="dp-info-label">القيمة</span>
                                    <span class="dp-info-value mono">
                                        <asp:Label ID="lblValue" runat="server" />
                                    </span>
                                </div>
                            </div>

                            <div class="dp-info-item full">
                                <div class="dp-info-icon orange">
                                    <i class="fas fa-sticky-note"></i>
                                </div>
                                <div class="dp-info-content">
                                    <span class="dp-info-label">الوصف</span>
                                    <span class="dp-info-value">
                                        <asp:Label ID="lblDescription" runat="server" />
                                    </span>
                                </div>
                            </div>

                        </div>

                    </div>
                </div>

                <!-- الأزرار -->
                <div class="dp-actions no-print">
                    <a href="Default.aspx" class="dp-btn-outline">
                        <i class="fas fa-times"></i> إلغاء
                    </a>
                    <asp:Button ID="btnDelete" runat="server" Text="حذف نهائي"
                        CssClass="dp-btn-danger"
                        OnClick="btnDelete_Click" />
                </div>

            </asp:Panel>

        </div>
    </div>
</asp:Content>