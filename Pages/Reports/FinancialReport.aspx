<%@ Page Title="التقرير المالي" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FinancialReport.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Reports.FinancialReport" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Financial Report
           ========================================================= */
        .fr-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .fr-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .fr-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .fr-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .fr-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .fr-hero-left { display: flex; align-items: center; gap: 20px; }
        .fr-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .fr-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .fr-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }
        .fr-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .fr-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .fr-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }
        .fr-btn-glass.primary { background: #fff; color: #00b14f; border-color: #fff; }
        .fr-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .fr-body { max-width: 1280px; margin: -56px auto 0; padding: 0 24px 48px; position: relative; z-index: 3; }

        /* ---------- CARD ---------- */
        .fr-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .fr-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .fr-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .fr-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .fr-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .fr-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .fr-card-body { padding: 20px 15px 0 0; }
        /*.fr-card-body.no-padding { padding: 0; }*/

        /* ---------- FILTERS ---------- */
        .fr-filters { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 12px; align-items: end; }
        .fr-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .fr-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .fr-field label i { color: #00b14f; font-size: 12px; }
        .fr-field .form-control,
        .fr-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .fr-field .form-control:focus,
        .fr-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }

        .fr-btn-primary { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 11px 22px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); border: none; color: #fff; font-size: 13px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(0,177,79,0.3); height: 44px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; width: 100%; }
        .fr-btn-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }
        .fr-btn-whatsapp { display: inline-flex; align-items: center; gap: 8px; padding: 10px 16px; border: 0; border-radius: 11px; background: #25d366; color: #083b20; font: 700 13px 'Droid Arabic Kufi', sans-serif; cursor: pointer; }
        .fr-btn-whatsapp:hover { background: #1fbd5b; }
        .fr-whatsapp-actions { display: flex; align-items: center; gap: 10px; margin-inline-start: auto; flex-wrap: wrap; }
        .fr-whatsapp-actions small { color: #66736b; font-size: 11px; }

        /* ---------- SUMMARY CARDS ---------- */
        .fr-summary { display: grid; grid-template-columns: repeat(6, minmax(0, 1fr)); gap: 14px; margin-bottom: 20px; }
        .fr-summary-card { display: flex; align-items: center; gap: 12px; padding: 16px 10px; border-radius: 16px; background: #ffffff; border: 1px solid #99f6c3; box-shadow: 0 4px 12px rgba(0,0,0,0.03); transition: all 0.2s ease; position: relative; overflow: hidden; }
        .fr-summary-card::before { content: ''; position: absolute; top: 0; inset-inline-start: 0; width: 4px; height: 100%; }
        .fr-summary-card:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(0,0,0,0.06); }
        .fr-summary-card.blue::before   { background: linear-gradient(180deg, #0d6efd 0%, #3d8bfd 100%); }
        .fr-summary-card.purple::before { background: linear-gradient(180deg, #6f42c1 0%, #9d6ee6 100%); }
        .fr-summary-card.red::before    { background: linear-gradient(180deg, #dc3545 0%, #f0616d 100%); }
        .fr-summary-card.green::before  { background: linear-gradient(180deg, #00b14f 0%, #4ed17e 100%); }
        .fr-summary-card.teal::before   { background: linear-gradient(180deg, #0dcaf0 0%, #63e1f5 100%); }
        .fr-summary-card.orange::before { background: linear-gradient(180deg, #fd7e14 0%, #ffa94d 100%); }
        .fr-summary-icon { width: 35px; height: 35px; border-radius: 12px; display: inline-flex; align-items: center; justify-content: center; font-size: 16px; flex-shrink: 0; color: #fff; }
        .fr-summary-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 4px 12px rgba(13,110,253,0.3); }
        .fr-summary-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 4px 12px rgba(111,66,193,0.3); }
        .fr-summary-icon.red    { background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); box-shadow: 0 4px 12px rgba(220,53,69,0.3); }
        .fr-summary-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 4px 12px rgba(0,177,79,0.3); }
        .fr-summary-icon.teal   { background: linear-gradient(135deg, #0dcaf0 0%, #63e1f5 100%); box-shadow: 0 4px 12px rgba(13,202,240,0.3); }
        .fr-summary-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 4px 12px rgba(253,126,20,0.3); }
        .fr-summary-info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
        .fr-summary-label { font-size: 11px; font-weight: 700; color: #6c757d; letter-spacing: 0.2px; white-space: nowrap; }
        .fr-summary-value { font-size: 14px; font-weight: 700; color: #212529; line-height: 1.15; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
        .fr-summary-value small { font-size: 10px; opacity: 0.7; font-family: 'Droid Arabic Kufi', sans-serif; font-weight: 700; }

        /* ---------- TABLE ---------- */
        .fr-table-wrap { overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .fr-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 13px; min-width: 1000px; }
        .fr-table thead th { background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%); color: #1f3d33; font-size: 11.5px; font-weight: 800; padding: 14px 10px !important; text-align: center; border-bottom: 2px solid #d8e6dd; white-space: nowrap; }
        .fr-table tbody td { font-size: 13px; padding: 12px 10px !important; vertical-align: middle; border-bottom: 1px solid #f1f3f5; color: #212529; background: #fff; text-align: center; }
        .fr-table tbody tr:hover td { background: #f8fdfa; }
        .fr-table tbody tr:last-child td { border-bottom: none; }

        /* Money values */
        .fr-money { font-weight: 700; font-size: 13px; }
        .fr-money.green { color: #00b14f; }
        .fr-money.red   { color: #dc3545; }
        .fr-money.blue  { color: #0d6efd; }
        .fr-money.gray  { color: #495057; }

        /* Status badge */
        .fr-status { display: inline-flex; align-items: center; gap: 5px; padding: 5px 12px; border-radius: 999px; font-size: 13px; font-weight: 700; white-space: nowrap; }
        .fr-status.paid { background: rgba(0,177,79,0.12); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .fr-status.partial { color: #a07800;  }
        .fr-status.unpaid { background: rgba(220,53,69,0.1); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }

        /* Empty */
        .fr-empty { text-align: center; padding: 60px 20px; }
        .fr-empty-icon { width: 80px; height: 80px; border-radius: 22px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 36px; margin-bottom: 18px; }
        .fr-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 8px; }
        .fr-empty p { color: #6c757d; font-size: 13px; margin: 0; }

        /* Responsive */
        @media (max-width: 1200px) { .fr-summary { grid-template-columns: repeat(3, minmax(0, 1fr)); } }
        @media (max-width: 992px) {
            .fr-hero { padding: 26px 20px 72px; }
            .fr-hero-titles h1 { font-size: 19px; }
            .fr-body { padding: 0 16px 40px; }
        }
        @media (max-width: 768px) { .fr-summary { grid-template-columns: repeat(2, minmax(0, 1fr)); } }
        @media (max-width: 576px) {
            .fr-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .fr-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .fr-hero-titles h1 { font-size: 16px; }
            .fr-card-body { padding: 16px; }
            .fr-summary { grid-template-columns: 1fr; }
        }
        th {
  text-align: center;

}

        /* Print */
        @media print {
            .fr-hero, .fr-hero-actions, .fr-btn-glass, .fr-filters, .no-print { display: none !important; }
            .fr-page { background: #fff !important; }
            .fr-body { margin: 0 !important; padding: 0 !important; }
            .fr-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .fr-table { font-size: 11px !important; min-width: 0 !important; text-align: center; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fr-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="fr-hero">
            <div class="fr-hero-content">
                <div class="fr-hero-left">
                    <div class="fr-hero-icon">
                        <i class="fas fa-chart-line"></i>
                    </div>
                    <div class="fr-hero-titles">
                        <h1>التقرير المالي والتحصيل</h1>
                        <p>عرض شامل للفواتير والمدفوعات والمبالغ المتبقية</p>
                    </div>
                </div>
                <div class="fr-hero-actions no-print">
                    <asp:Button ID="btnExportWord" runat="server" Text="تصدير ورد"
                        CssClass="fr-btn-glass primary"
                        OnClick="btnExportWord_Click" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="fr-body">

            <!-- ============ Filter Card ============ -->
            <div class="fr-card no-print">
                <div class="fr-card-header">
                    <div class="fr-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="fr-card-header-text">
                        <h3>معايير التقرير</h3>
                        <p>حدد الفترة الزمنية، السنة، والصف ثم اضغط عرض التقرير</p>
                    </div>
                </div>
                <div class="fr-card-body">
                    <div class="fr-filters">

                        <div class="fr-field">
                            <label><i class="fas fa-calendar-alt"></i> من تاريخ</label>
                            <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                        <div class="fr-field">
                            <label><i class="fas fa-calendar-check"></i> إلى تاريخ</label>
                            <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                        <div class="fr-field">
                            <label><i class="fas fa-book"></i> السنة الدراسية</label>
                            <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <div class="fr-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="fr-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true">
                                <asp:ListItem Value="" Text="الكل" />
                                <asp:ListItem Value="علمي" Text="القسم العلمي" />
                                <asp:ListItem Value="أدبي" Text="القسم الأدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="fr-field">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnFilter" runat="server" Text="عرض التقرير"
                                CssClass="fr-btn-primary" OnClick="btnFilter_Click" />
                        </div>

                    </div>
                </div>
            </div>

            <!-- ============ Summary Cards ============ -->
            <asp:Panel ID="pnlSummary" runat="server" Visible="false">
                <div class="fr-summary">

                    <div class="fr-summary-card blue">
                        <div class="fr-summary-icon blue"><i class="fas fa-users"></i></div>
                        <div class="fr-summary-info">
                            <span class="fr-summary-label">الطلاب</span>
                            <span class="fr-summary-value"><asp:Label ID="lblTotalStudents" runat="server" /></span>
                        </div>
                    </div>

                    <div class="fr-summary-card purple">
                        <div class="fr-summary-icon purple"><i class="fas fa-coins"></i></div>
                        <div class="fr-summary-info">
                            <span class="fr-summary-label">الإجمالي</span>
                            <span class="fr-summary-value"><asp:Label ID="lblTotalFees" runat="server" /> <small>د.ل</small></span>
                        </div>
                    </div>

                    <div class="fr-summary-card red">
                        <div class="fr-summary-icon red"><i class="fas fa-tags"></i></div>
                        <div class="fr-summary-info">
                            <span class="fr-summary-label">الخصومات</span>
                            <span class="fr-summary-value"><asp:Label ID="lblTotalDiscount" runat="server" /> <small>د.ل</small></span>
                        </div>
                    </div>

                    <div class="fr-summary-card green">
                        <div class="fr-summary-icon green"><i class="fas fa-check-circle"></i></div>
                        <div class="fr-summary-info">
                            <span class="fr-summary-label">الصافي</span>
                            <span class="fr-summary-value"><asp:Label ID="lblTotalNet" runat="server" /> <small>د.ل</small></span>
                        </div>
                    </div>

                    <div class="fr-summary-card teal">
                        <div class="fr-summary-icon teal"><i class="fas fa-hand-holding-usd"></i></div>
                        <div class="fr-summary-info">
                            <span class="fr-summary-label">المحصل</span>
                            <span class="fr-summary-value"><asp:Label ID="lblTotalPayments" runat="server" /> <small>د.ل</small></span>
                        </div>
                    </div>

                    <div class="fr-summary-card orange">
                        <div class="fr-summary-icon orange"><i class="fas fa-exclamation-circle"></i></div>
                        <div class="fr-summary-info">
                            <span class="fr-summary-label">المتبقي</span>
                            <span class="fr-summary-value"><asp:Label ID="lblRemaining" runat="server" /> <small>د.ل</small></span>
                        </div>
                    </div>

                </div>
            </asp:Panel>

            <!-- ============ Report Table ============ -->
            <div class="fr-card">
                <div class="fr-card-header">
                    <div class="fr-card-header-icon">
                        <i class="fas fa-table"></i>
                    </div>
                    <div class="fr-card-header-text">
                        <h3>تفاصيل الفواتير</h3>
                    <div class="fr-whatsapp-actions no-print">
                        <asp:Button ID="btnExportWhatsAppPdf" runat="server" Text="تحميل PDF وفتح واتساب"
                            CssClass="fr-btn-whatsapp" OnClick="btnExportWhatsAppPdf_Click" />
                        <small>سيتم تنزيل التقرير وفتح محادثة المدير؛ أرفق ملف PDF في واتساب ثم أرسله.</small>
                    </div>
                        <p>جميع الفواتير ضمن الفترة المحددة</p>
                    </div>
                </div>

                <div class="fr-card-body no-padding">
                    <div class="fr-table-wrap">
                        <asp:GridView ID="gvReport" runat="server" CssClass="fr-table"
                            AutoGenerateColumns="false"
                            EmptyDataText="" GridLines="None"
                            ShowHeaderWhenEmpty="true">

                            <Columns>

                                <asp:BoundField DataField="AcademicYearName" HeaderText="السنة الدراسية" />

                                <asp:TemplateField HeaderText="الطالب">
                                    <ItemTemplate>
                                        <strong><%#: Eval("StudentName") %></strong>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الفاتورة">
                                    <ItemTemplate>
                                        <span class="fr-money blue"><%#: Eval("InvoiceNumber") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="InvoiceDate" HeaderText="التاريخ"
                                    DataFormatString="{0:yyyy-MM-dd}" />

                                <asp:TemplateField HeaderText="الإجمالي">
                                    <ItemTemplate>
                                        <span class="fr-money gray"><%# ((decimal)Eval("TotalAmount")).ToString("N2") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الخصم/الإعفاء">
                                    <ItemTemplate>
                                        <span class="fr-money red"><%# ((decimal)Eval("DiscountAmount")).ToString("N2") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الصافي">
                                    <ItemTemplate>
                                        <span class="fr-money blue"><%# ((decimal)Eval("NetAmount")).ToString("N2") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المدفوع">
                                    <ItemTemplate>
                                        <span class="fr-money green"><%# ((decimal)Eval("TotalPaid")).ToString("N2") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المتبقي">
                                    <ItemTemplate>
                                        <span class="fr-money red"><%# ((decimal)Eval("Remaining")).ToString("N2") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة">
                                    <ItemTemplate>
                                        <%# FormatStatus(Eval("Status")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>