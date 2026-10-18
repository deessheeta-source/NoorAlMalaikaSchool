<%@ Page Title="تفاصيل الفاتورة" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Invoices.Details" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
       
        .id-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .id-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .id-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .id-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .id-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .id-hero-left { display: flex; align-items: center; gap: 20px; }
        .id-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .id-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .id-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; display: flex; align-items: center; gap: 8px; flex-wrap: wrap; }
        .id-invoice-no { display: inline-flex; align-items: center; gap: 6px; padding: 3px 12px; border-radius: 999px; background: rgba(255,255,255,0.15); border: 1px solid rgba(255,255,255,0.3); font-size: 12px; font-weight: 700; color: #fff; font-family: 'Courier New', monospace; letter-spacing: 0.5px; }
        .id-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .id-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .id-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }
        .id-btn-glass.primary { background: #fff; color: #00b14f; border-color: #fff; }
        .id-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .id-body { max-width: 1280px; margin: -56px auto 0;padding: 0 24px 0; position: relative; z-index: 3; }

        /* ---------- CARD ---------- */
        .id-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; height: 100%; display: flex; flex-direction: column; }
        .id-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .id-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .id-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .id-card-header-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .id-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .id-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .id-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .id-card-body { padding: 20px 24px; flex: 1; }

        /* ---------- INFO ROWS ---------- */
        .id-info-row { display: flex; align-items: center; gap: 8px; font-size: 13px; margin-bottom: 12px; color: #495057; padding: 8px 12px; border-radius: 8px; background: rgba(0,177,79,0.02); }
        .id-info-row:last-child { margin-bottom: 0; }
        .id-info-row i { color: #00b14f; width: 16px; text-align: center; font-size: 12px; flex-shrink: 0; }
        .id-info-row .lbl { color: #6c757d; font-weight: 600; }
        .id-info-row .val { color: #212529; font-weight: 800; margin-inline-start: auto; text-align: end; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
        .id-info-row .val.mono { font-family: 'Courier New', monospace; letter-spacing: 0.4px; }

        /* ---------- SUMMARY TABLE ---------- */
        .id-summary-table { width: 100%; border-collapse: separate; border-spacing: 0; border-radius: 12px; overflow: hidden; border: 1px solid #eef1f3; }
        .id-summary-table tr th { background: #f9fafb; font-weight: 700; color: #495057; padding: 13px 16px; font-size: 12.5px; border-bottom: 1px solid #eef1f3; width: 55%; text-align: start; }
        .id-summary-table tr th i { color: #00b14f; margin-inline-end: 6px; font-size: 12px; }
        .id-summary-table tr td { padding: 13px 16px; font-size: 13.5px; border-bottom: 1px solid #eef1f3; text-align: end; color: #212529; font-weight: 800; font-family: 'Courier New', monospace; white-space: nowrap; }
        .id-summary-table tr:last-child th, .id-summary-table tr:last-child td { border-bottom: none; }
        .id-summary-table tr.net th, .id-summary-table tr.net td { background: linear-gradient(90deg, rgba(0,177,79,0.08) 0%, rgba(0,177,79,0.04) 100%); color: #007a33; font-weight: 800; font-size: 14.5px; }
        .id-summary-table tr.remaining th, .id-summary-table tr.remaining td { background: linear-gradient(90deg, rgba(220,53,69,0.06) 0%, rgba(220,53,69,0.02) 100%); color: #a51c1c; font-weight: 800; }

        /* ---------- TABLE ---------- */
        .id-table-wrap { width: 100%; max-width: 100%; border-radius: 14px; overflow-x: auto; -webkit-overflow-scrolling: touch; border: 1px solid #eef1f3; }
        .id-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 13px; table-layout: auto; }
        .id-table thead th { background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%); color: #1f3d33; font-size: 11.5px; font-weight: 800; padding: 13px 10px !important; text-align: center; border-bottom: 2px solid #d8e6dd; white-space: normal; }
        .id-table tbody td { font-size: 13px; padding: 12px 10px !important; vertical-align: middle; border-bottom: 1px solid #f1f3f5; color: #212529; background: #fff; text-align: center; }
        .id-table tbody tr:hover td { background: #f8fdfa; }
        .id-table tbody tr:last-child td { border-bottom: none; }

        .id-type-badge { display: inline-flex; align-items: center; gap: 4px; padding: 4px 10px; border-radius: 8px; font-size: 11.5px; font-weight: 700; background: #f8f9fa; color: #495057; border: 1px solid #e9ecef; white-space: nowrap; }
        .id-type-badge.fee { background: rgba(0,177,79,0.08); color: #007a33; border-color: rgba(0,177,79,0.2); }
        .id-type-badge.manual { background: rgba(13,110,253,0.08); color: #0d6efd; border-color: rgba(13,110,253,0.2); }

        .id-money { font-weight: 800; font-family: 'Courier New', monospace; font-size: 13px; }
        .id-money.green { color: #00b14f; }
        .id-money.red { color: #dc3545; }

        .id-empty { text-align: center; padding: 32px 20px; }
        .id-empty-icon { width: 56px; height: 56px; border-radius: 16px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 22px; margin-bottom: 12px; }
        .id-empty h4 { color: #1f3d33; font-weight: 800; font-size: 14px; margin: 0 0 4px; }
        .id-empty p { color: #6c757d; font-size: 12px; margin: 0; }

        /* ============ تخطيط الصفحة ============ */
        .id-layout-row {
            display: grid;
            grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
            gap: 20px;
            margin-bottom: 20px;
        }
        .id-layout-row.full {
            grid-template-columns: minmax(0, 1fr);
        }
        .id-layout-col {
            display: flex;
            flex-direction: column;
            min-width: 0;
        }
        .id-layout-col > .id-card {
            flex: 1;
            margin-bottom: 0;
        }
        .id-card,
        .id-card-body,
        .id-card-header-text { min-width: 0; }

        @media (max-width: 992px) {
            .id-hero { padding: 26px 20px 72px; }
            .id-hero-titles h1 { font-size: 19px; }
            .id-body { padding: 0 16px 40px; }
            .id-layout-row { grid-template-columns: minmax(0, 1fr); }
        }
        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .id-table { font-size: 11px; min-width: 400px; }
        }
        @media (max-width: 576px) {
            .id-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .id-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .id-hero-titles h1 { font-size: 16px; }
            .id-card-body { padding: 16px; }
            .id-info-row {
                display: grid;
                grid-template-columns: 16px max-content minmax(0, 1fr);
                align-items: start;
                gap: 4px 8px;
            }
            .id-info-row .val {
                min-width: 0;
                margin-inline-start: 0;
                white-space: normal;
                overflow: visible;
                overflow-wrap: anywhere;
                text-overflow: clip;
            }
        }

        /* =========================================================
           PRINT — A4 Portrait — كل شيء في صفحة واحدة
           ========================================================= */
        @page { size: A4 portrait; margin: 8mm; }

        @media print {
            .id-hero, .id-hero-actions, .no-print, .fin-nav, .id-btn-glass,
            .sidebar, .top-header, .footer, .sidebar-overlay, #sidebarOverlay,
            .header-actions, .sidebar-toggle { display: none !important; }

            html, body { background: #fff !important; margin: 0 !important; padding: 0 !important; width: 100% !important; direction: rtl; font-family: 'Droid Arabic Kufi', sans-serif; }
            .main-content, .content-wrapper, .container-fluid { margin: 0 !important; padding: 0 !important; width: 100% !important; max-width: 100% !important; }
            .id-page { background: #fff !important; min-height: 0 !important; }
            .id-body { max-width: 100% !important; margin: 0 !important; padding: 0 !important; }

            * { -webkit-print-color-adjust: exact !important; print-color-adjust: exact !important; }

            /* ============ البطاقات ============ */
            .id-card { border-radius: 10px !important; margin-bottom: 0 !important; page-break-inside: avoid; box-shadow: 0 1px 4px rgba(0,0,0,0.05) !important; height: 100% !important; }
            .id-card-header { padding: 6px 10px !important; gap: 6px !important; }
            .id-card-header-icon { width: 24px !important; height: 24px !important; border-radius: 7px !important; font-size: 11px !important; box-shadow: none !important; }
            .id-card-header-text h3 { font-size: 9.5pt !important; }
            .id-card-header-text p { font-size: 7pt !important; margin-top: 0 !important; }
            .id-card-body { padding: 6px 8px !important; }

            /* ============ صفوف المعلومات ============ */
            .id-info-row { font-size: 7.5pt !important; margin-bottom: 3px !important; padding: 3px 6px !important; gap: 4px !important; }
            .id-info-row i { width: 9px !important; font-size: 7pt !important; }
            .id-info-row .lbl { font-size: 7pt !important; }
            .id-info-row .val { font-size: 7.5pt !important; }

            /* ============ جدول الملخص ============ */
            .id-summary-table { border-radius: 6px !important; }
            .id-summary-table tr th { padding: 4px 8px !important; font-size: 7.5pt !important; background: #f9fafb !important; color: #495057 !important; }
            .id-summary-table tr th i { font-size: 7pt !important; margin-inline-end: 3px !important; }
            .id-summary-table tr td { padding: 4px 8px !important; font-size: 8pt !important; color: #212529 !important; font-family: 'Courier New', monospace !important; }
            .id-summary-table tr.net th, .id-summary-table tr.net td { background: linear-gradient(90deg, rgba(0,177,79,0.1) 0%, rgba(0,177,79,0.04) 100%) !important; color: #007a33 !important; font-size: 8.5pt !important; }
            .id-summary-table tr.remaining th, .id-summary-table tr.remaining td { background: linear-gradient(90deg, rgba(220,53,69,0.08) 0%, rgba(220,53,69,0.02) 100%) !important; color: #a51c1c !important; font-size: 8.5pt !important; }

            /* ============ الجداول ============ */
            .id-table-wrap { border-radius: 6px !important; overflow: visible !important; border: 1px solid #d5ead9 !important; }
            .id-table { width: 100% !important; table-layout: fixed !important; border-collapse: separate !important; border-spacing: 0 !important; font-size: 7pt !important; }
            .id-table thead th { font-size: 6.5pt !important; padding: 3px 2px !important; white-space: normal !important; word-break: break-word !important; background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%) !important; color: #1f3d33 !important; border-bottom: 1px solid #d8e6dd !important; text-align: center !important; line-height: 1.1 !important; }
            .id-table tbody td { font-size: 7pt !important; padding: 3px 2px !important; white-space: normal !important; word-break: break-word !important; text-align: center !important; color: #212529 !important; border-bottom: 1px solid #f1f3f5 !important; line-height: 1.2 !important; overflow: visible !important; }
            .id-table tbody tr:last-child td { border-bottom: none !important; }
            .id-table tbody tr:hover td { background: inherit !important; }

            /* أعمدة جدول البنود */
            .id-table thead th:nth-child(1),
            .id-table tbody td:nth-child(1) { width: 40% !important; }
            .id-table thead th:nth-child(2),
            .id-table tbody td:nth-child(2) { width: 25% !important; }
            .id-table thead th:nth-child(3),
            .id-table tbody td:nth-child(3) { width: 35% !important; }

            /* badge النوع */
            .id-type-badge { padding: 1px 5px !important; font-size: 6.5pt !important; border-radius: 4px !important; background: rgba(0,177,79,0.08) !important; color: #007a33 !important; border: 1px solid rgba(0,177,79,0.2) !important; display: inline-block !important; }
            .id-type-badge.manual { background: rgba(13,110,253,0.08) !important; color: #0d6efd !important; border: 1px solid rgba(13,110,253,0.2) !important; }

            .id-money { font-size: 7pt !important; font-weight: 800 !important; font-family: 'Courier New', monospace !important; display: inline-block !important; white-space: nowrap !important; }
            .id-money.green { color: #00b14f !important; }
            .id-money.red   { color: #dc3545 !important; }

            /* ============ الحالة الفارغة ============ */
            .id-empty { padding: 8px 6px !important; }
            .id-empty-icon { width: 24px !important; height: 24px !important; border-radius: 6px !important; font-size: 10px !important; margin-bottom: 3px !important; }
            .id-empty h4 { font-size: 7.5pt !important; margin: 0 0 1px !important; }
            .id-empty p { font-size: 6.5pt !important; }

            /* ============ التخطيط — 2×2 + صف كامل ============ */
            .id-layout-row {
                display: grid !important;
                grid-template-columns: 1fr 1fr !important;
                gap: 5px !important;
                margin-bottom: 5px !important;
            }
            .id-layout-row.full {
                grid-template-columns: 1fr !important;
            }
            .id-layout-col {
                display: flex !important;
                flex-direction: column !important;
            }

            /* ============ منع فواصل الصفحات ============ */
            .id-card, .id-info-row, .id-table-wrap, .id-summary-table, .id-table tr, .id-summary-table tr {
                page-break-inside: avoid !important;
                break-inside: avoid !important;
            }
            thead { display: table-header-group; }
        }
                    /* أعمدة جدول الخصومات (بعرض كامل) */
            .id-layout-row.full .id-table thead th:nth-child(1),
            .id-layout-row.full .id-table tbody td:nth-child(1) { width: 50% !important; }
            .id-layout-row.full .id-table thead th:nth-child(2),
            .id-layout-row.full .id-table tbody td:nth-child(2) { width: 25% !important; }
            .id-layout-row.full .id-table thead th:nth-child(3),
            .id-layout-row.full .id-table tbody td:nth-child(3) { width: 25% !important; }

        /* أعمدة الخصومات تتحول إلى صفوف كاملة العرض عند عرضها كبطاقات على الهاتف. */
        @media screen and (max-width: 767.98px) {
            .id-layout-row.full .id-discounts-table[data-mobile-card-table="true"] > tbody > tr > td,
            .id-layout-row.full .id-discounts-table[data-mobile-card-table="true"] > tfoot > tr > td {
                width: 100% !important;
                min-width: 0 !important;
                max-width: 100% !important;
                box-sizing: border-box !important;
            }

            .id-layout-row.full .id-discounts-table[data-mobile-card-table="true"] .id-money {
                display: inline-block !important;
                white-space: nowrap !important;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="id-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="id-hero">
            <div class="id-hero-content">
                <div class="id-hero-left">
                    <div class="id-hero-icon">
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="id-hero-titles">
                        <h1>تفاصيل الفاتورة</h1>
                        <p>
                            <span> تفاصيل الفاتورة، بنودها، وملخصها المالي</span>
                            <asp:Panel ID="pnlHeroInvoiceNo" runat="server" CssClass="id-invoice-no">
                                <i class="fas fa-hashtag" style="font-size:10px;"></i>
                                <asp:Literal ID="lblHeroInvoiceNo" runat="server" />
                            </asp:Panel>
                        </p>
                    </div>
                </div>
                <div class="id-hero-actions no-print">
                    <button type="button" onclick="window.print()" class="id-btn-glass">
                        <i class="fas fa-print"></i> طباعة
                    </button>
                    <a href="<%= _returnUrl %>" class="id-btn-glass primary">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="id-body">

            <!-- ═══════════════ الصف الأول: بيانات المدرسة (يمين) | ملخص الفاتورة (يسار) ═══════════════ -->
            <div class="id-layout-row">

                <!-- بيانات المدرسة -->
                <div class="id-layout-col">
                    <div class="id-card">
                        <div class="id-card-header">
                            <div class="id-card-header-icon blue">
                                <i class="fas fa-school"></i>
                            </div>
                            <div class="id-card-header-text">
                                <h3>بيانات المدرسة</h3>
                                <p>معلومات الفاتورة الأساسية</p>
                            </div>
                        </div>
                        <div class="id-card-body">

                            <div class="id-info-row">
                                <i class="fas fa-signature"></i>
                                <span class="lbl">اسم المدرسة:</span>
                                <span class="val">مدرسة نور الملائكة</span>
                            </div>

                            <div class="id-info-row">
                                <i class="fas fa-hashtag"></i>
                                <span class="lbl">رقم الفاتورة:</span>
                                <span class="val mono"><asp:Label ID="lblInvoiceNumber" runat="server" /></span>
                            </div>

                            <div class="id-info-row">
                                <i class="fas fa-calendar-alt"></i>
                                <span class="lbl">التاريخ:</span>
                                <span class="val mono"><asp:Label ID="lblDate" runat="server" /></span>
                            </div>

                            <div class="id-info-row">
                                <i class="fas fa-book"></i>
                                <span class="lbl">السنة الدراسية:</span>
                                <span class="val"><asp:Label ID="lblYear" runat="server" /></span>
                            </div>

                        </div>
                    </div>
                </div>

                <!-- ملخص الفاتورة -->
                <div class="id-layout-col">
                    <div class="id-card">
                        <div class="id-card-header">
                            <div class="id-card-header-icon">
                                <i class="fas fa-calculator"></i>
                            </div>
                            <div class="id-card-header-text">
                                <h3>ملخص الفاتورة</h3>
                                <p>الإجماليات والمبالغ المستحقة</p>
                            </div>
                        </div>
                        <div class="id-card-body">
                            <table class="id-summary-table">
                                <tr>
                                    <th><i class="fas fa-shopping-cart"></i>إجمالي البنود</th>
                                    <td><asp:Label ID="lblTotal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th><i class="fas fa-tags"></i>إجمالي الخصومات</th>
                                    <td><asp:Label ID="lblDiscount" runat="server" /></td>
                                </tr>
                                <tr class="net">
                                    <th><i class="fas fa-check-circle"></i>الصافي المستحق</th>
                                    <td><asp:Label ID="lblNet" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th><i class="fas fa-money-bill-wave"></i>المبلغ المدفوع</th>
                                    <td><asp:Label ID="lblPaid" runat="server" /></td>
                                </tr>
                                <tr class="remaining">
                                    <th><i class="fas fa-exclamation-circle"></i>المتبقي</th>
                                    <td><asp:Label ID="lblRemaining" runat="server" /></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>

            </div>

            <!-- ═══════════════ الصف الثاني: بيانات الطالب (يمين) | بنود الفاتورة (يسار) ═══════════════ -->
            <div class="id-layout-row">

                <!-- بيانات الطالب -->
                              <!-- بيانات الطالب -->
                <div class="id-layout-col">
                    <div class="id-card">
                        <div class="id-card-header">
                            <div class="id-card-header-icon blue">
                                <i class="fas fa-user-graduate"></i>
                            </div>
                            <div class="id-card-header-text">
                                <h3>بيانات الطالب</h3>
                                <p>المعلومات الأساسية للطالب</p>
                            </div>
                        </div>
                        <div class="id-card-body">

                            <div class="id-info-row">
                                <i class="fas fa-user"></i>
                                <span class="lbl">الاسم:</span>
                                <span class="val"><asp:Label ID="lblStudent" runat="server" /></span>
                            </div>

                            <div class="id-info-row">
                                <i class="fas fa-graduation-cap"></i>
                                <span class="lbl">الصف:</span>
                                <span class="val"><asp:Label ID="lblClass" runat="server" /></span>
                            </div>

                            <div class="id-info-row">
                                <i class="fas fa-layer-group"></i>
                                <span class="lbl">القسم:</span>
                                <span class="val"><asp:Label ID="lblSection" runat="server" /></span>
                            </div>

                            <div class="id-info-row">
                                <i class="fas fa-chair"></i>
                                <span class="lbl">الشعبة:</span>
                                <span class="val"><asp:Label ID="lblDivision" runat="server" /></span>
                            </div>

                            <div class="id-info-row">
                                <i class="fas fa-user-friends"></i>
                                <span class="lbl">ولي الأمر:</span>
                                <span class="val"><asp:Label ID="lblParent" runat="server" /></span>
                            </div>

                        </div>
                    </div>
                </div>

                <!-- بنود الفاتورة -->
                <div class="id-layout-col">
                    <div class="id-card">
                        <div class="id-card-header">
                            <div class="id-card-header-icon">
                                <i class="fas fa-list-ul"></i>
                            </div>
                            <div class="id-card-header-text">
                                <h3>بنود الفاتورة</h3>
                                <p>تفاصيل الرسوم المُدرجة</p>
                            </div>
                        </div>
                        <div class="id-card-body">
                            <div class="table-responsive id-table-wrap">
                                <asp:GridView ID="gvLines" runat="server" CssClass="id-table"
                                    AutoGenerateColumns="false" GridLines="None" ShowHeader="true">
                                    <Columns>
                                        <asp:BoundField DataField="Description" HeaderText="البيان" />
                                        <asp:TemplateField HeaderText="النوع">
                                            <ItemTemplate>
                                                <span class='<%# GetLineTypeCss(Eval("TypeText")) %>'>
                                                    <%# Eval("TypeText") %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="المبلغ">
                                            <ItemTemplate>
                                                <span class="id-money green">
                                                    <%# ((decimal)Eval("Amount")).ToString("N2") %> د.ل
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="id-empty" style="padding:20px 10px;">
                                            <div class="id-empty-icon"><i class="fas fa-inbox"></i></div>
                                            <h4>لا توجد بنود</h4>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <!-- ═══════════════ الصف الثالث: الخصومات والإعفاءات (كامل العرض) ═══════════════ -->
            <div class="id-layout-row full">

                <div class="id-layout-col">
                    <div class="id-card">
                        <div class="id-card-header">
                            <div class="id-card-header-icon orange">
                                <i class="fas fa-tags"></i>
                            </div>
                            <div class="id-card-header-text">
                                <h3>الخصومات والإعفاءات</h3>
                                <p>التخفيضات المطبقة على الفاتورة</p>
                            </div>
                        </div>
                        <div class="id-card-body">
                            <div class="table-responsive id-table-wrap">
                                <asp:GridView ID="gvDiscounts" runat="server" CssClass="id-table id-discounts-table"
                                    AutoGenerateColumns="false" GridLines="None" ShowHeader="true">
                                    <Columns>
                                        <asp:BoundField DataField="Name" HeaderText="اسم الخصم" />
                                        <asp:BoundField DataField="ValueText" HeaderText="القيمة" />
                                        <asp:TemplateField HeaderText="المبلغ">
                                            <ItemTemplate>
                                                <span class="id-money red">
                                                    <%# ((decimal)Eval("DiscountAmount")).ToString("N2") %> د.ل
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="id-empty">
                                            <div class="id-empty-icon"><i class="fas fa-info-circle"></i></div>
                                            <h4>لا توجد خصومات</h4>
                                            <p>لا توجد خصومات أو إعفاءات على هذه الفاتورة</p>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

        </div>
    </div>
</asp:Content>
