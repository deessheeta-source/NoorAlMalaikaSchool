<%@ Page Title="الكشف المالي للطلاب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentFinancialStatement.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Reports.StudentFinancialStatement" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Student Financial Statement
           ========================================================= */
        .sf-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .sf-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .sf-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .sf-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .sf-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .sf-hero-left { display: flex; align-items: center; gap: 20px; }
        .sf-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .sf-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .sf-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }

        /* ---------- BODY ---------- */
        .sf-body { max-width: 1280px; margin: -56px auto 0; padding: 0 24px 48px; position: relative; z-index: 3; }

        /* ---------- CARD ---------- */
        .sf-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .sf-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .sf-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .sf-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .sf-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .sf-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .sf-card-body { padding: 20px 24px; }
        .sf-card-body.no-padding { padding: 0; }

        /* ---------- FILTERS ---------- */
        .sf-filters { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 12px; align-items: end; }
        .sf-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .sf-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .sf-field label i { color: #00b14f; font-size: 12px; }
        .sf-field .form-control,
        .sf-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .sf-field .form-control:focus,
        .sf-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }

        .sf-btn-primary { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 11px 22px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); border: none; color: #fff; font-size: 13px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(0,177,79,0.3); height: 44px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .sf-btn-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }

        /* ---------- SUMMARY CARDS ---------- */
        .sf-summary { display: grid; grid-template-columns: repeat(auto-fit, minmax(210px, 1fr)); gap: 14px; margin-bottom: 20px; }
        .sf-summary-card { display: flex; align-items: center; gap: 12px; padding: 16px 10px; border-radius: 16px; background: #ffffff; border: 1px solid #99f6c3; box-shadow: 0 4px 12px rgba(0,0,0,0.03); transition: all 0.2s ease; position: relative; overflow: hidden; }
        .sf-summary-card::before { content: ''; position: absolute; top: 0; inset-inline-start: 0; width: 4px; height: 100%; }
        .sf-summary-card:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(0,0,0,0.06); }
        .sf-summary-card.blue::before   { background: linear-gradient(180deg, #0d6efd 0%, #3d8bfd 100%); }
        .sf-summary-card.purple::before { background: linear-gradient(180deg, #6f42c1 0%, #9d6ee6 100%); }
        .sf-summary-card.green::before  { background: linear-gradient(180deg, #00b14f 0%, #4ed17e 100%); }
        .sf-summary-card.orange::before { background: linear-gradient(180deg, #fd7e14 0%, #ffa94d 100%); }
        .sf-summary-icon { width: 35px; height: 35px; border-radius: 13px; display: inline-flex; align-items: center; justify-content: center; font-size: 16px; flex-shrink: 0; color: #fff; }
        .sf-summary-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 4px 12px rgba(13,110,253,0.3); }
        .sf-summary-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 4px 12px rgba(111,66,193,0.3); }
        .sf-summary-icon.green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 4px 12px rgba(0,177,79,0.3); }
        .sf-summary-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 4px 12px rgba(253,126,20,0.3); }
        .sf-summary-info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
        .sf-summary-label { font-size: 11.5px; font-weight: 700; color: #6c757d; letter-spacing: 0.2px; white-space: nowrap; }
        .sf-summary-value { font-size: 18px; font-weight: 800; color: #212529; line-height: 1.15; font-family: 'Courier New', monospace; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
        .sf-summary-value small { font-size: 10.5px; opacity: 0.7; font-family: 'Droid Arabic Kufi', sans-serif; font-weight: 700; }

        /* ---------- TABLE ---------- */
        .sf-table-wrap { overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .sf-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 13px; min-width: 900px; }
        .sf-table thead th { background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%); color: #1f3d33; font-size: 11.5px; font-weight: 800; padding: 14px 10px !important; text-align: center; border-bottom: 2px solid #d8e6dd; white-space: nowrap; }
        .sf-table tbody td { font-size: 13px; padding: 12px 10px !important; vertical-align: middle; border-bottom: 1px solid #f1f3f5; color: #212529; background: #fff; text-align: center; }
        .sf-table tbody tr:hover td { background: #f8fdfa; }
        .sf-table tbody tr:last-child td { border-bottom: none; }

        /* Money values */
        .sf-money { font-weight: 700; font-size: 13px; }
        .sf-money.green { color: #00b14f; }
        .sf-money.red   { color: #dc3545; }
        .sf-money.blue  { color: #0d6efd; }
        .sf-money.gray  { color: #495057; }

        /* Registration badge */
        .sf-reg-badge { display: inline-flex; align-items: center; gap: 4px; padding: 4px 10px;  font-size: 13px; font-weight: 700;  }

        /* Section badge */
        .sf-section-badge { display: inline-flex; align-items: center; gap: 5px; padding: 4px 12px; border-radius: 999px; font-size: 11.5px; font-weight: 700; background: rgba(111,66,193,0.08); color: #6f42c1; border: 1px solid rgba(111,66,193,0.2); white-space: nowrap; }
        .sf-section-badge.empty { background: transparent; color: #6c757d; border: none; }

        /* Details button */
        .sf-btn-details { display: inline-flex; align-items: center; gap: 5px; padding: 7px 14px; border-radius: 8px; background: #fff; border: 1.5px solid #00b14f; color: #00b14f; font-size: 12px; font-weight: 700; text-decoration: none; transition: all 0.15s ease; font-family: 'Droid Arabic Kufi', sans-serif; white-space: nowrap; }
        .sf-btn-details:hover { background: rgba(0,177,79,0.08); color: #007a33; text-decoration: none; }

        /* Empty */
        .sf-empty { text-align: center; padding: 60px 20px; }
        .sf-empty-icon { width: 80px; height: 80px; border-radius: 22px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 36px; margin-bottom: 18px; }
        .sf-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 8px; }
        .sf-empty p { color: #6c757d; font-size: 13px; margin: 0; }

        /* Pager */
        .sf-table .pagination { display: flex; gap: 6px; justify-content: center; padding: 16px 0 4px; margin: 0; list-style: none; }

        /* Responsive */
        @media (max-width: 992px) {
            .sf-hero { padding: 26px 20px 72px; }
            .sf-hero-titles h1 { font-size: 19px; }
            .sf-body { padding: 0 16px 40px; }
            .sf-summary { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        }
        @media (max-width: 576px) {
            .sf-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .sf-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .sf-hero-titles h1 { font-size: 16px; }
            .sf-card-body { padding: 16px; }
            .sf-summary { grid-template-columns: 1fr; }
        }

        /* Print */
        @media print {
            .sf-hero, .sf-btn-details, .sf-filters, .no-print, .pagination { display: none !important; }
            .sf-page { background: #fff !important; }
            .sf-body { margin: 0 !important; padding: 0 !important; }
            .sf-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .sf-table { font-size: 11px !important; min-width: 0 !important; }
        }
        th {
  text-align: center;
  
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
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="sf-hero-titles">
                        <h1>الكشف المالي للطلاب</h1>
                        <p>عرض شامل للمستحقات والمدفوعات والمتبقي لكل طالب</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="sf-body">

            <!-- ============ Filter Card ============ -->
            <div class="sf-card no-print">
                <div class="sf-card-header">
                    <div class="sf-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="sf-card-header-text">
                        <h3>معايير البحث</h3>
                        <p>حدد السنة، الصف، القسم، أو ابحث باسم الطالب</p>
                    </div>
                </div>
                <div class="sf-card-body">
                    <div class="sf-filters">

                        <div class="sf-field">
                            <label><i class="fas fa-book"></i> السنة الدراسية</label>
                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <div class="sf-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="sf-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                                <asp:ListItem Value="علمي" Text="القسم العلمي" />
                                <asp:ListItem Value="أدبي" Text="القسم الأدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="sf-field">
                            <label><i class="fas fa-users"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <div class="sf-field">
                            <label><i class="fas fa-user-graduate"></i> الطالب</label>
                            <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="جميع الطلاب" />
                            </asp:DropDownList>
                        </div>

                        <div class="sf-field">
                            <label><i class="fas fa-chart-pie"></i> النوع</label>
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="جميع الإجماليات" />
                                <asp:ListItem Value="subscription" Text="إجمالي مبلغ الاشتراك" />
                                <asp:ListItem Value="discount" Text="إجمالي الخصم" />
                                <asp:ListItem Value="due" Text="إجمالي المبلغ المستحق" />
                                <asp:ListItem Value="paid" Text="إجمالي المبلغ المدفوع" />
                                <asp:ListItem Value="remaining" Text="إجمالي المبلغ المتبقي" />
                            </asp:DropDownList>
                        </div>

                        <div class="sf-field" style="grid-column: span 2;">
                            <label><i class="fas fa-search"></i> البحث</label>
                            <div style="display:flex;gap:8px;">
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                                    placeholder="اسم الطالب أو رقم التسجيل" style="flex:1;" />
                                <asp:Button ID="btnFilter" runat="server" Text="بحث"
                                    CssClass="sf-btn-primary" OnClick="btnFilter_Click" style="width:auto;padding-left:22px;padding-right:22px;" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <!-- ============ Summary Cards ============ -->
            <asp:Panel ID="pnlSummary" runat="server" Visible="false">
                <div class="sf-summary">
                    <asp:Panel ID="pnlSummaryStudents" runat="server" CssClass="sf-summary-card blue">
                        <div class="sf-summary-icon blue"><i class="fas fa-users"></i></div>
                        <div class="sf-summary-info"><span class="sf-summary-label">عدد الطلاب</span>
                            <span class="sf-summary-value"><asp:Label ID="lblStudents" runat="server" /></span></div>
                    </asp:Panel>
                    <asp:Panel ID="pnlSummarySubscription" runat="server" CssClass="sf-summary-card blue">
                        <div class="sf-summary-icon blue"><i class="fas fa-file-invoice-dollar"></i></div>
                        <div class="sf-summary-info"><span class="sf-summary-label">إجمالي مبلغ الاشتراك</span>
                            <span class="sf-summary-value"><asp:Label ID="lblTotalSubscription" runat="server" /> <small>د.ل</small></span></div>
                    </asp:Panel>
                    <asp:Panel ID="pnlSummaryDiscount" runat="server" CssClass="sf-summary-card purple">
                        <div class="sf-summary-icon purple"><i class="fas fa-tags"></i></div>
                        <div class="sf-summary-info"><span class="sf-summary-label">إجمالي الخصم</span>
                            <span class="sf-summary-value"><asp:Label ID="lblTotalDiscount" runat="server" /> <small>د.ل</small></span></div>
                    </asp:Panel>
                    <asp:Panel ID="pnlSummaryDue" runat="server" CssClass="sf-summary-card purple">
                        <div class="sf-summary-icon purple"><i class="fas fa-coins"></i></div>
                        <div class="sf-summary-info"><span class="sf-summary-label">إجمالي المبلغ المستحق</span>
                            <span class="sf-summary-value"><asp:Label ID="lblTotalDue" runat="server" /> <small>د.ل</small></span></div>
                    </asp:Panel>
                    <asp:Panel ID="pnlSummaryPaid" runat="server" CssClass="sf-summary-card green">
                        <div class="sf-summary-icon green"><i class="fas fa-hand-holding-usd"></i></div>
                        <div class="sf-summary-info"><span class="sf-summary-label">إجمالي المبلغ المدفوع</span>
                            <span class="sf-summary-value"><asp:Label ID="lblTotalPaid" runat="server" /> <small>د.ل</small></span></div>
                    </asp:Panel>
                    <asp:Panel ID="pnlSummaryRemaining" runat="server" CssClass="sf-summary-card orange">
                        <div class="sf-summary-icon orange"><i class="fas fa-exclamation-circle"></i></div>
                        <div class="sf-summary-info"><span class="sf-summary-label">إجمالي المبلغ المتبقي</span>
                            <span class="sf-summary-value"><asp:Label ID="lblTotalRemaining" runat="server" /> <small>د.ل</small></span></div>
                    </asp:Panel>
                </div>
            </asp:Panel>

            <!-- ============ Report Table ============ -->
            <div class="sf-card">
                <div class="sf-card-header">
                    <div class="sf-card-header-icon">
                        <i class="fas fa-table"></i>
                    </div>
                    <div class="sf-card-header-text">
                        <h3>الكشف المالي للطلاب</h3>
                        <p>قائمة الطلاب والمبالغ المستحقة والمدفوعة</p>
                    </div>
                </div>

                <div class="sf-card-body no-padding">
                    <div class="sf-table-wrap">
                        <asp:GridView ID="gvReport" runat="server" CssClass="sf-table"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="200"
                            OnPageIndexChanging="gvReport_PageIndexChanging"
                            EmptyDataText="" GridLines="None"
                            ShowHeaderWhenEmpty="true">

                            <Columns>

                                <asp:TemplateField HeaderText="رقم التسجيل">
                                    <ItemTemplate>
                                        <span class="sf-reg-badge">
                                            <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("RegistrationNo")))
                                                ? "—"
                                                : System.Web.HttpUtility.HtmlEncode(Convert.ToString(Eval("RegistrationNo"))) %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الطالب">
                                    <ItemTemplate>
                                        <strong><%#: Eval("StudentName") %></strong>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="ClassName" HeaderText="الصف" />

                                <asp:TemplateField HeaderText="القسم">
                                    <ItemTemplate>
                                        <%# FormatSection(Eval("Section")) %>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="text-center" />
                                </asp:TemplateField>

                                <asp:BoundField DataField="YearName" HeaderText="السنة الدراسية" />

                                <asp:TemplateField HeaderText="مبلغ الاشتراك">
                                    <ItemTemplate><span class="sf-money gray"><%# ((decimal)Eval("TotalSubscription")).ToString("N2") %></span></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الخصم">
                                    <ItemTemplate><span class="sf-money purple"><%# ((decimal)Eval("TotalDiscount")).ToString("N2") %></span></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المستحق">
                                    <ItemTemplate>
                                        <span class="sf-money blue"><%# ((decimal)Eval("TotalDue")).ToString("N2") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المدفوع">
                                    <ItemTemplate>
                                        <span class="sf-money green"><%# ((decimal)Eval("TotalPaid")).ToString("N2") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المتبقي">
                                    <ItemTemplate>
                                        <span class="sf-money red"><%# ((decimal)Eval("Remaining")).ToString("N2") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="التفاصيل" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server"
                                            NavigateUrl='<%# BuildDetailsUrl(Eval("StudentId")) %>'
                                            CssClass="sf-btn-details">
                                            <i class="fas fa-eye"></i> التفاصيل
                                        </asp:HyperLink>
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