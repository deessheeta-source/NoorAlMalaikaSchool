<%@ Page Title="الفواتير" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Invoices.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Invoices List
           ========================================================= */
        .inv-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .inv-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .inv-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .inv-hero::after {
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
        .inv-hero-content {
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
        .inv-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .inv-hero-icon {
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
        .inv-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .inv-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .inv-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .inv-btn-glass {
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
        .inv-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .inv-body {
            max-width: 1280px;
            margin: -56px auto 0;
          /* padding: 0 24px 0;*/
            position: relative;
            z-index: 3;
        }

        /* ---------- STATS CARDS ---------- */
        .inv-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 14px;
            margin-bottom: 20px;
        }
        .inv-stat-card {
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
        .inv-stat-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.06);
        }
        .inv-stat-icon {
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
        .inv-stat-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .inv-stat-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .inv-stat-icon.red    { background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); box-shadow: 0 6px 16px rgba(220,53,69,0.3); }
        .inv-stat-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .inv-stat-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .inv-stat-info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
        .inv-stat-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            letter-spacing: 0.2px;
        }
        .inv-stat-value {
            font-size: 19px;
            font-weight: 800;
            color: #212529;
            line-height: 1.1;
            font-family: 'Courier New', monospace;
        }

        /* ---------- CARD ---------- */
        .inv-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .inv-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .inv-card-header-icon {
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
        .inv-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .inv-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .inv-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .inv-card-body { padding: 20px 10px; }

        /* ---------- FILTERS ---------- */
        .inv-filters {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
            gap: 12px;
            align-items: end;
        }
        .inv-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .inv-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .inv-field label i { color: #00b14f; font-size: 12px; }
        .inv-field .form-control,
        .inv-field .form-select {
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
        .inv-field .form-control:focus,
        .inv-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        .inv-btn-primary {
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
            font-family: 'Droid Arabic Kufi', sans-serif;
            width: 100%;
        }
        .inv-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }

        /* ---------- TABLE ---------- */
        .inv-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .inv-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .inv-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33;
            font-size: 11.5px;
            font-weight: 800;
            padding: 14px 10px !important;
            text-align: right;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
        }
        .inv-table tbody td {
            font-size: 13px;
            padding: 14px 5px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .inv-table tbody tr:hover td { background: #f8fdfa; }
        .inv-table tbody tr:last-child td { border-bottom: none; }

        /* Money values */
        .money-value { font-weight: 800; font-family: 'Courier New', monospace; font-size: 13px; }
        .money-value.green  { color: #00b14f; }
        .money-value.red    { color: #dc3545; }
        .money-value.blue   { color: #0d6efd; }
        .money-value.gray   { color: #495057; }

        /* Invoice number badge */
        .inv-no-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 5px;
                    
            font-size: 12px;
           
          
        }

        /* Status badge */
        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 5px;
           
            font-size: 11.5px;
           
            white-space: nowrap;
        }
        .status-badge.paid {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
      /*  .status-badge.partial {
            background: rgba(255,193,7,0.15);
            color: #a07800;
            border: 1px solid rgba(255,193,7,0.35);
        }*/
        .status-badge.issued {
            background: rgba(13,110,253,0.1);
            color: #0d6efd;
            border: 1px solid rgba(13,110,253,0.25);
        }
        .status-badge.draft {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }
        .status-badge.cancelled {
            background: rgba(220,53,69,0.1);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }

        /* Action buttons */
        .inv-btn-action {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 6px 7px;
            border-radius: 8px;
            font-size: 12px;
            font-weight: 700;
            text-decoration: none;
            border: 1.5px solid;
            transition: all 0.15s ease;
            cursor: pointer;
            background: #fff;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .inv-btn-action.view {
            border-color: #00b14f;
            color: #00b14f;
        }
        .inv-btn-action.view:hover {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            text-decoration: none;
        }
        .inv-btn-action.del {
            border-color: #dc3545;
            color: #dc3545;
        }
        .inv-btn-action.del:hover {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            text-decoration: none;
        }

        /* Pagination */
        .inv-table .pagination {
            display: flex;
            gap: 6px;
            justify-content: center;
            padding: 16px 0 4px;
            margin: 0;
            list-style: none;
        }

        /* Message */
        .inv-message {
            display: block;
            padding: 12px 16px;
            border-radius: 11px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 16px;
        }
        .inv-message.success {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .inv-message.danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .inv-message:empty { display: none; padding: 0; margin: 0; }

        /* Empty state */
        .inv-empty {
            text-align: center;
            padding: 50px 20px;
        }
        .inv-empty-icon {
            width: 76px; height: 76px; border-radius: 22px;
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            color: #00b14f;
            display: inline-flex; align-items: center; justify-content: center;
            font-size: 32px; margin-bottom: 16px;
        }
        .inv-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 6px; }
        .inv-empty p { color: #6c757d; font-size: 13px; margin: 0; }

        /* Responsive */
        @media (max-width: 992px) {
            .inv-hero { padding: 26px 20px 72px; }
            .inv-hero-titles h1 { font-size: 19px; }
            .inv-body { padding: 0 16px 40px; }
        }
        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .inv-table { font-size: 11px; min-width: 1100px; }
            .inv-table thead th,
            .inv-table tbody td { padding: 10px 6px !important; }
        }
        @media (max-width: 576px) {
            .inv-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .inv-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .inv-hero-titles h1 { font-size: 16px; }
            .inv-card-body { padding: 16px; }
        }

        /* Print */
        @media print {
            .inv-hero-actions, .inv-filters, .inv-stats, .no-print,
            .pagination, .inv-table .pagination { display: none !important; }
            .inv-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .inv-body { margin-top: 0 !important; padding: 0 !important; }
            .inv-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
        th {
  text-align: revert;
 
  padding-bottom: 15px;
  padding-top: 15px;
}
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="inv-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="inv-hero">
            <div class="inv-hero-content">
                <div class="inv-hero-left">
                    <div class="inv-hero-icon">
                        <i class="fas fa-file-invoice"></i>
                    </div>
                    <div class="inv-hero-titles">
                        <h1>إدارة الفواتير</h1>
                        <p>عرض، بحث، وإدارة فواتير الطلاب</p>
                    </div>
                </div>
                <div class="inv-hero-actions no-print">
                    <a href="<%= _returnUrl %>" class="inv-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="inv-body">

            <!-- شريط التنقل المالي -->
            <div class="no-print" style="margin-bottom:16px;">
               
            </div>

            <!-- بطاقات الإحصائيات -->
            <div class="inv-stats no-print">
                <div class="inv-stat-card">
                    <div class="inv-stat-icon blue"><i class="fas fa-file-invoice"></i></div>
                    <div class="inv-stat-info">
                        <span class="inv-stat-label">عدد الفواتير</span>
                        <span class="inv-stat-value"><asp:Label ID="lblTotalInvoices" runat="server" Text="0" /></span>
                    </div>
                </div>
                <div class="inv-stat-card">
                    <div class="inv-stat-icon purple"><i class="fas fa-coins"></i></div>
                    <div class="inv-stat-info">
                        <span class="inv-stat-label">إجمالي المبالغ</span>
                        <span class="inv-stat-value"><asp:Label ID="lblTotalAmount" runat="server" Text="0.00" /></span>
                    </div>
                </div>
                <div class="inv-stat-card">
                    <div class="inv-stat-icon green"><i class="fas fa-check-circle"></i></div>
                    <div class="inv-stat-info">
                        <span class="inv-stat-label">إجمالي المدفوع</span>
                        <span class="inv-stat-value"><asp:Label ID="lblTotalPaid" runat="server" Text="0.00" /></span>
                    </div>
                </div>
                <div class="inv-stat-card">
                    <div class="inv-stat-icon red"><i class="fas fa-hourglass-half"></i></div>
                    <div class="inv-stat-info">
                        <span class="inv-stat-label">إجمالي المتبقي</span>
                        <span class="inv-stat-value"><asp:Label ID="lblTotalRemaining" runat="server" Text="0.00" /></span>
                    </div>
                </div>
            </div>

            <!-- كارد الفلاتر -->
            <div class="inv-card no-print">
                <div class="inv-card-header">
                    <div class="inv-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="inv-card-header-text">
                        <h3>معايير البحث</h3>
                        <p>استخدم الفلاتر أدناه للبحث عن فواتير محددة</p>
                    </div>
                </div>
                <div class="inv-card-body">
                    <div class="inv-filters">

                        <div class="inv-field" style="grid-column: span 2;">
                            <label><i class="fas fa-search"></i> البحث</label>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                                placeholder="رقم الفاتورة أو اسم الطالب" />
                        </div>

                        <div class="inv-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="inv-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true">
                                <asp:ListItem Value="" Text="الكل" />
                                <asp:ListItem Value="علمي" Text="القسم العلمي" />
                                <asp:ListItem Value="أدبي" Text="القسم الأدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="inv-field">
                            <label><i class="fas fa-info-circle"></i> الحالة</label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <div class="inv-field">
                            <label><i class="fas fa-calendar-alt"></i> السنة الدراسية</label>
                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <div class="inv-field">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnSearch" runat="server" Text="بحث"
                                CssClass="inv-btn-primary" OnClick="btnSearch_Click" />
                        </div>

                    </div>
                </div>
            </div>

            <!-- كارد الفواتير -->
            <div class="inv-card">
                <div class="inv-card-header">
                    <div class="inv-card-header-icon">
                        <i class="fas fa-list-alt"></i>
                    </div>
                    <div class="inv-card-header-text">
                        <h3>قائمة الفواتير</h3>
                        <p>جميع الفواتير المطابقة لمعايير البحث</p>
                    </div>
                </div>
                <div class="inv-card-body">

                    <asp:Label ID="lblMessage" runat="server" CssClass="inv-message" />

                    <div class="table-responsive inv-table-wrap">
                        <asp:GridView ID="gvInvoices" runat="server" CssClass="inv-table"
                            AutoGenerateColumns="false" EmptyDataText="لا توجد فواتير" GridLines="None"
                            AllowPaging="true" PageSize="20"
                            OnPageIndexChanging="gvInvoices_PageIndexChanging"
                            OnRowCommand="gvInvoices_RowCommand"
                            DataKeyNames="Id">

                            <Columns>

                                <asp:TemplateField HeaderText="رقم الفاتورة">
                                    <ItemTemplate>
                                        <span class="inv-no-badge">
                                           
                                            <%#: Eval("InvoiceNumber") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="StudentName" HeaderText="اسم الطالب" HeaderStyle-Width="390px" />

                                <asp:BoundField DataField="GradeLevelName" HeaderText="الصف" ItemStyle-CssClass="text-center" HeaderStyle-Width="130px" />

                                <asp:TemplateField HeaderText="القسم" ItemStyle-CssClass="text-center" HeaderStyle-Width="100px">
                                    <ItemTemplate>
                                        <%# FormatSection(Eval("Section")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="InvoiceDate" HeaderText="التاريخ"
                                    DataFormatString="{0:yyyy-MM-dd}" ItemStyle-CssClass="text-center" />

                                <asp:TemplateField HeaderText="الإجمالي" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="170px">
                                    <ItemTemplate>
                                        <span class="money-value blue">
                                            <%# ((decimal)Eval("TotalAmount")).ToString("N2") %> د.ل
                                        </span>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="text-md-end"  />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الخصم" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="140px">
                                    <ItemTemplate>
                                        <span class="money-value red">
                                            <%# ((decimal)Eval("DiscountAmount")).ToString("N2") %> د.ل
                                        </span>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="text-md-end"  />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المتبقي" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="170px">
                                    <ItemTemplate>
                                        <span class="money-value red">
                                            <%# ((decimal)Eval("RemainingAmount")).ToString("N2") %> د.ل
                                        </span>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="text-md-end"  />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المدفوع" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="170px">
                                    <ItemTemplate>
                                        <span class="money-value green">
                                            <%# ((decimal)Eval("PaidAmount")).ToString("N2") %> د.ل
                                        </span>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="text-md-end"  />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-CssClass="text-center" >
                                    <ItemTemplate>
                                        <span class='<%# GetStatusCss(Convert.ToString(Eval("Status"))) %>'>
                                            <%# GetStatusName(Convert.ToString(Eval("Status"))) %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="إجراءات" ItemStyle-CssClass="text-center" >
                                    <ItemTemplate>
                                        <div class="d-flex gap-1 justify-content-center">
                                            <a href='<%# BuildDetailsUrl(Eval("Id")) %>'
                                               class="inv-btn-action view" title="عرض">
                                                <i class="fas fa-eye"></i> 
                                            </a>
                                            <asp:LinkButton ID="btnDelete" runat="server"
                                                CommandName="DeleteInvoice"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CssClass="inv-btn-action del"
                                                ToolTip="حذف"
                                                OnClientClick="return confirm('هل أنت متأكد من حذف هذه الفاتورة؟ لا يمكن التراجع.');">
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