<%@ Page Title="إنشاء فاتورة" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Invoices.Create" EnableViewState="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        .inv-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

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

        .inv-body {
            max-width: 1280px;
            margin: -56px auto 0;
            padding: 0 24px 40px;
            position: relative;
            z-index: 3;
        }

        .inv-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .inv-card:last-child { margin-bottom: 0; }
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
        .inv-card-header-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .inv-card-header-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .inv-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
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
        .inv-card-body {
            padding: 20px 24px;
        }

        .inv-filters {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
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
        .inv-field label .req { color: #dc3545; }
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

        .inv-student-info {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 12px 16px;
            border-radius: 12px;
            background: linear-gradient(135deg, rgba(0,177,79,0.06) 0%, rgba(0,177,79,0.02) 100%);
            border: 1px solid rgba(0,177,79,0.15);
            font-size: 13px;
            font-weight: 600;
            color: #007a33;
            margin-top: 14px;
        }
        .inv-student-info:empty { display: none; }
        .inv-student-info i { font-size: 14px; }

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
            padding: 12px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .inv-table tbody tr:hover td { background: #f8fdfa; }
        .inv-table tbody tr:last-child td { border-bottom: none; }

        .inv-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 24px;
            border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            color: #fff;
            font-size: 13.5px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
            height: 44px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
            width: 100%;
        }
        .inv-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }
        .inv-btn-outline {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 24px;
            border-radius: 11px;
            background: #fff;
            border: 1px solid #08b45463;
            color: #495057;
            font-size: 13.5px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 44px;
            text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
            width: 100%;
            margin-top: 8px;
        }
        .inv-btn-outline:hover {
            border-color: #6c757d;
            background: #f8f9fa;
            color: #343a40;
            text-decoration: none;
        }

        .inv-discount-list {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }
        .inv-discount-list input[type="checkbox"] {
            margin-inline-end: 8px;
            accent-color: #00b14f;
        }
        .inv-discount-list label {
            font-size: 12px;
            font-weight: 600;
            color: #212529;
            cursor: pointer;
            align-items: center;
            line-height: 2.2;
        }

        /* ✅ Manual discount box (sibling / early bird / annual subscription) */
        .inv-sibling-box {
            display: none;
            margin-top: 14px;
            padding: 12px 14px;
            border-radius: 12px;
            background: linear-gradient(135deg, rgba(0,177,79,0.06) 0%, rgba(0,177,79,0.02) 100%);
            border: 1.5px dashed rgba(0,177,79,0.4);
        }
        .inv-sibling-box.show { display: block; }
        .inv-sibling-box label {
            font-size: 12.5px;
            font-weight: 800;
            color: #007a33;
            display: flex;
            align-items: center;
            gap: 6px;
            margin-bottom: 8px;
        }
        .inv-sibling-box .form-control {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 13px;
            font-weight: 700;
            color: #007a33;
            background: #fff;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px;
            width: 100%;
        }
        .inv-sibling-box .form-control:focus {
            border-color: #00b14f;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }
        .inv-sibling-box .hint {
            font-size: 11.5px;
            color: #6c757d;
            margin-top: 6px;
            font-weight: 500;
        }

        /* ============================================
           ✅ ملخص الفاتورة — تصميم أفقي أنيق
           ============================================ */
        .inv-summary-grid {
            display: grid;
            grid-template-columns: 1fr 1fr 1.2fr;
            gap: 12px;
            margin-top: 4px;
        }
        .inv-summary-cell {
            padding: 16px 18px;
            border-radius: 14px;
            background: #f9fafb;
            border: 1.5px solid #eef1f3;
            display: flex;
            flex-direction: column;
            gap: 6px;
            transition: all 0.2s ease;
        }
        .inv-summary-cell .cell-label {
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            display: flex;
            align-items: center;
            gap: 6px;
            letter-spacing: 0.2px;
        }
        .inv-summary-cell .cell-label i { font-size: 12px; }
        .inv-summary-cell .cell-value {
            font-family: 'Courier New', monospace;
            font-size: 18px;
            font-weight: 800;
            color: #212529;
            line-height: 1.1;
        }
        .inv-summary-cell .cell-value small {
            font-size: 11px;
            font-weight: 600;
            color: #6c757d;
            font-family: 'Droid Arabic Kufi', sans-serif;
            margin-inline-start: 4px;
        }

        .inv-summary-cell.total {
            background: linear-gradient(135deg, rgba(0,177,79,0.12) 0%, rgba(0,177,79,0.04) 100%);
            border-color: rgba(0,177,79,0.35);
        }
        .inv-summary-cell.total .cell-label { color: #007a33; }
        .inv-summary-cell.total .cell-value { color: #007a33; font-size: 20px; }
        .inv-summary-cell.total .cell-value small { color: #007a33; }

        .inv-summary-cell.discount .cell-value { color: #dc3545; }

        .inv-summary-actions {
            display: flex;
            gap: 10px;
            margin-top: 16px;
            flex-wrap: wrap;
        }
        .inv-summary-actions .inv-btn-primary { flex: 1; min-width: 160px; width: auto; }
        .inv-summary-actions .inv-btn-outline { flex: 1; min-width: 140px; width: auto; margin-top: 0; }

        .inv-message {
            display: block;
            padding: 12px 16px;
            border-radius: 11px;
            font-size: 13px;
            font-weight: 700;
            margin: 12px 0;
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

        @media (max-width: 992px) {
            .inv-hero { padding: 26px 20px 72px; }
            .inv-hero-titles h1 { font-size: 19px; }
            .inv-body { padding: 0 16px 40px; }
            .inv-summary-grid { grid-template-columns: 1fr 1fr; }
            .inv-summary-grid .total { grid-column: 1 / -1; }
        }
        @media (max-width: 576px) {
            .inv-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .inv-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .inv-hero-titles h1 { font-size: 16px; }
            .inv-card-body { padding: 16px; }
            .inv-summary-grid { grid-template-columns: 1fr; }
            .inv-summary-grid .total { grid-column: auto; }
        }

        @media print {
            .inv-hero-actions, .no-print { display: none !important; }
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
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="inv-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="inv-hero">
            <div class="inv-hero-content">
                <div class="inv-hero-left">
                    <div class="inv-hero-icon">
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="inv-hero-titles">
                        <h1>إنشاء فاتورة جديدة</h1>
                        <p>اختر الطالب، البنود، وسياسات الخصم ثم احفظ الفاتورة</p>
                    </div>
                </div>
                <div class="inv-hero-actions">
                    <a href="<%= ReturnUrl %>" class="inv-btn-glass no-print">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="inv-body">

            <!-- كارد اختيار الطالب -->
            <div class="inv-card">
                <div class="inv-card-header">
                    <div class="inv-card-header-icon blue">
                        <i class="fas fa-user-graduate"></i>
                    </div>
                    <div class="inv-card-header-text">
                        <h3>بيانات الطالب</h3>
                        <p>اختر الصف، القسم، الشعبة ثم الطالب</p>
                    </div>
                </div>
                <div class="inv-card-body">
                    <div class="inv-filters no-print">

                        <div class="inv-field">
                            <label><i class="fas fa-layer-group"></i> الصف الدراسي <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlClass_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="-- اختر الصف --" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSection" runat="server" CssClass="inv-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الأقسام" Value="" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="inv-field">
                            <label><i class="fas fa-chair"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الشعب" Value="" Selected="True" />
                            </asp:DropDownList>
                        </div>

                        <div class="inv-field">
                            <label><i class="fas fa-user"></i> الطالب <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="-- اختر الطالب --" />
                            </asp:DropDownList>
                        </div>

                        <div class="inv-field">
                            <label><i class="fas fa-calendar-alt"></i> تاريخ الفاتورة</label>
                            <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>

                    </div>

                    <asp:Label ID="lblStudentInfo" runat="server" CssClass="inv-student-info" />
                </div>
            </div>

            <!-- قسم بناء الفاتورة -->
            <asp:Panel ID="pnlBuild" runat="server" Visible="false">
                <div class="row g-3">

                    <!-- ✅ العمود الأيسر (8): البنود + الملخص -->
                    <div class="col-lg-8">

                        <!-- بنود الرسوم -->
                        <div class="inv-card">
                            <div class="inv-card-header">
                                <div class="inv-card-header-icon">
                                    <i class="fas fa-list-ul"></i>
                                </div>
                                <div class="inv-card-header-text">
                                    <h3>بنود الفاتورة (رسوم الصف)</h3>
                                    <p>اختر البنود المُراد إدراجها في الفاتورة وحدّد المبالغ</p>
                                </div>
                            </div>
                            <div class="inv-card-body">
                                <div class="inv-table-wrap">
                                    <asp:GridView ID="gvLines" runat="server" CssClass="inv-table" DataKeyNames="Id"
                                        AutoGenerateColumns="false" EmptyDataText="لا توجد رسوم لهذا الصف" GridLines="None">
                                        <Columns>
                                            <asp:TemplateField HeaderText="تضمين" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkInclude" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Name" HeaderText="الرسوم" />
                                            <asp:TemplateField HeaderText="المبلغ" ItemStyle-Width="160px">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtAmount" runat="server"
                                                        CssClass="form-control form-control-sm"
                                                        TextMode="Number"
                                                        step="0.01"
                                                        min="0"
                                                        inputmode="decimal"
                                                        Text='<%# Convert.ToDecimal(Eval("Amount")).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>

                        <!-- ✅ ملخص الفاتورة — الآن أسفل البنود -->
                        <div class="inv-card">
                            <div class="inv-card-header">
                                <div class="inv-card-header-icon">
                                    <i class="fas fa-calculator"></i>
                                </div>
                                <div class="inv-card-header-text">
                                    <h3>ملخص الفاتورة</h3>
                                    <p>اضغط "حساب / معاينة" لحساب الإجمالي والصافي، ثم "حفظ الفاتورة"</p>
                                </div>
                            </div>
                            <div class="inv-card-body">

                                <asp:Panel ID="pnlSummary" runat="server" Visible="false" CssClass="inv-summary-grid">
                                    <div class="inv-summary-cell">
                                        <span class="cell-label">
                                            <i class="fas fa-list-ul" style="color:#00b14f;"></i>
                                            إجمالي البنود
                                        </span>
                                        <span class="cell-value">
                                            <asp:Label ID="lblTotal" runat="server" />
                                        </span>
                                    </div>

                                    <div class="inv-summary-cell discount">
                                        <span class="cell-label">
                                            <i class="fas fa-percent" style="color:#dc3545;"></i>
                                            إجمالي الخصومات
                                        </span>
                                        <span class="cell-value">
                                            <asp:Label ID="lblDiscount" runat="server" />
                                        </span>
                                    </div>

                                    <div class="inv-summary-cell total">
                                        <span class="cell-label">
                                            <i class="fas fa-check-circle"></i>
                                            الصافي المستحق
                                        </span>
                                        <span class="cell-value">
                                            <asp:Label ID="lblNet" runat="server" />
                                        </span>
                                    </div>

                                    <asp:HiddenField ID="hfTotal" runat="server" />
                                    <asp:HiddenField ID="hfDiscount" runat="server" />
                                    <asp:HiddenField ID="hfNet" runat="server" />
                                </asp:Panel>

                                <asp:Label ID="lblMessage" runat="server" CssClass="inv-message" />

                                <div class="inv-summary-actions no-print">
                                    <asp:Button ID="btnPreview" runat="server" Text="حساب / معاينة"
                                        CssClass="inv-btn-outline" OnClick="btnPreview_Click" />

                                    <asp:Button ID="btnSave" runat="server" Text="حفظ الفاتورة"
                                        CssClass="inv-btn-primary" OnClick="btnSave_Click" />
                                </div>

                            </div>
                        </div>

                    </div>

                    <!-- ✅ العمود الأيمن (4): سياسات الخصم -->
                    <div class="col-lg-4">

                        <div class="inv-card">
                            <div class="inv-card-header">
                                <div class="inv-card-header-icon purple">
                                    <i class="fas fa-percent"></i>
                                </div>
                                <div class="inv-card-header-text">
                                    <h3>سياسات الخصم / الإعفاء</h3>
                                    <p>اختر السياسات المُراد تطبيقها على الفاتورة</p>
                                </div>
                            </div>
                            <div class="inv-card-body">
                                <div class="inv-discount-list">
                                    <asp:CheckBoxList ID="cblDiscounts" runat="server" CssClass="list-unstyled mbc" RepeatLayout="Flow" />
                                </div>

                                <%-- ============ ✅ Manual discount inputs ============ --%>
                                <asp:HiddenField ID="hfSiblingPolicyId" runat="server" />
                                <asp:HiddenField ID="hfEarlyBirdPolicyId" runat="server" />
                                <asp:HiddenField ID="hfAnnualSubscriptionPolicyId" runat="server" />

                                <div id="siblingAmountBox" class="inv-sibling-box">
                                    <label>
                                        <i class="fas fa-hand-holding-usd"></i>
                                        مقدار خصم الإخوة (د.ل)
                                    </label>
                                    <asp:TextBox ID="txtSiblingDiscount" runat="server"
                                        CssClass="form-control"
                                        TextMode="Number"
                                        placeholder="مثال: 150"
                                        step="0.01"
                                        min="0" />
                                    <div class="hint">
                                        أدخل مقدار الخصم يدوياً بدلاً من النسبة التلقائية.
                                    </div>
                                </div>

                                <div id="earlyBirdAmountBox" class="inv-sibling-box">
                                    <label>
                                        <i class="fas fa-hand-holding-usd"></i>
                                        مقدار التسجيل المبكر (د.ل)
                                    </label>
                                    <asp:TextBox ID="txtEarlyBirdDiscount" runat="server"
                                        CssClass="form-control"
                                        TextMode="Number"
                                        placeholder="مثال: 200"
                                        step="0.01"
                                        min="0" />
                                    <div class="hint">
                                        أدخل مقدار الخصم يدوياً بدلاً من النسبة التلقائية.
                                    </div>
                                </div>

                                <div id="annualSubscriptionAmountBox" class="inv-sibling-box">
                                    <label>
                                        <i class="fas fa-hand-holding-usd"></i>
                                        مقدار خصم الاشتراك السنوي (د.ل)
                                    </label>
                                    <asp:TextBox ID="txtAnnualSubscriptionDiscount" runat="server"
                                        CssClass="form-control"
                                        TextMode="Number"
                                        placeholder="مثال: 250"
                                        step="0.01"
                                        min="0" />
                                    <div class="hint">
                                        أدخل مقدار الخصم يدوياً بدلاً من النسبة التلقائية.
                                    </div>
                                </div>

                                <script type="text/javascript">
                                    (function () {
                                        function wireManualBox(hiddenId, boxId, listId) {
                                            var hidden = document.getElementById(hiddenId);
                                            var box = document.getElementById(boxId);
                                            var cbl = document.getElementById(listId);
                                            if (!hidden || !box || !cbl) return;

                                            var policyId = (hidden.value || '').trim();
                                            if (!policyId) { box.classList.remove('show'); return; }

                                            var checkboxes = cbl.querySelectorAll('input[type="checkbox"]');

                                            function toggle() {
                                                var show = false;
                                                for (var i = 0; i < checkboxes.length; i++) {
                                                    var cb = checkboxes[i];
                                                    if (cb.value === policyId && cb.checked) { show = true; break; }
                                                }
                                                if (show) box.classList.add('show');
                                                else box.classList.remove('show');
                                            }

                                            for (var i = 0; i < checkboxes.length; i++) {
                                                checkboxes[i].addEventListener('change', toggle);
                                            }
                                            toggle();
                                        }

                                        function initAll() {
                                            wireManualBox(
                                                '<%= hfSiblingPolicyId.ClientID %>',
                                                'siblingAmountBox',
                                                '<%= cblDiscounts.ClientID %>');
                                            wireManualBox(
                                                '<%= hfEarlyBirdPolicyId.ClientID %>',
                                                'earlyBirdAmountBox',
                                                '<%= cblDiscounts.ClientID %>');
                                            wireManualBox(
                                                '<%= hfAnnualSubscriptionPolicyId.ClientID %>',
                                                'annualSubscriptionAmountBox',
                                                '<%= cblDiscounts.ClientID %>');
                                        }

                                        if (document.readyState === 'loading')
                                            document.addEventListener('DOMContentLoaded', initAll);
                                        else
                                            initAll();
                                    })();
                                </script>
                                <%-- ==================================================== --%>

                                <asp:Label ID="lblNoPolicies" runat="server" CssClass="text-muted d-block mt-2" Text="لا توجد سياسات نشطة" />
                            </div>
                        </div>

                        <!-- زر الإلغاء أسفل العمود الأيمن -->
                        <a href="<%= ReturnUrl %>" class="inv-btn-outline no-print">
                            <i class="fas fa-times"></i> إلغاء والعودة
                        </a>

                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>
</asp:Content>