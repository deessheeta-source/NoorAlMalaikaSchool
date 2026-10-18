
<%@ Page Title="لوحة التحكم"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="NoorAlMalaikaSchool.Default" %>

<asp:Content ID="HeadContent1" ContentPlaceHolderID="HeadContent" runat="server">

    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />


</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="upDashboard"
        runat="server"
        UpdateMode="Conditional">

        <ContentTemplate>

            <div class="container-fluid" dir="rtl">

                <!-- PAGE HEADER -->
                <div class="mb-4 d-flex align-items-center justify-content-between flex-wrap gap-3">

                    <div class="d-flex align-items-center">

                        <h6 class="mb-0 fw-bold d-flex align-items-center">
                            <i class="fas fa-th-large me-2 text-academia"></i>
                            لوحة التحكم
                        </h6>

                        <span class="badge bg-academia ms-3 rounded-pill px-3 py-2">
                            <i class="fas fa-graduation-cap me-1"></i>
                            نظرة عامة
                        </span>

                    </div>

                </div>


                <!-- STAT CARDS -->
                <div class="row g-3 mb-4">

                    <!-- إجمالي الطلاب -->
                    <div class="col-xl-2 col-md-4 col-sm-6">

                        <div class="dash-stat">

                            <div class="d-flex align-items-center justify-content-between">

                                <div>
                                    <div class="ds-value"
                                         id="lblTotalStudentsAll"
                                         runat="server">0</div>

                                    <div class="ds-label">
                                        إجمالي الطلاب
                                    </div>
                                </div>

                                <div class="ds-icon">
                                    <i class="fas fa-users"></i>
                                </div>

                            </div>

                        </div>

                    </div>


                    <!-- المدفوعات -->
                    <div class="col-xl-2 col-md-4 col-sm-6"
                         id="pnlPaymentsStat"
                         runat="server"
                         visible="false">

                        <div class="dash-stat">

                            <div class="d-flex align-items-center justify-content-between">

                                <div>

                                    <div class="ds-value"
                                         id="lblTotalPayments"
                                         runat="server">0</div>

                                    <div class="ds-label">
                                        المدفوعات
                                    </div>

                                </div>

                                <div class="ds-icon">
                                    <i class="fas fa-coins"></i>
                                </div>

                            </div>

                        </div>

                    </div>


                    <!-- غائبين اليوم -->
                    <div class="col-xl-2 col-md-4 col-sm-6">

                        <div class="dash-stat">

                            <div class="d-flex align-items-center justify-content-between">

                                <div>

                                    <div class="ds-value"
                                         id="lblAbsentToday"
                                         runat="server">0</div>

                                    <div class="ds-label">
                                        غائبين اليوم
                                    </div>

                                </div>

                                <div class="ds-icon">
                                    <i class="fas fa-user-times"></i>
                                </div>

                            </div>

                        </div>

                    </div>


                    <!-- الفصول -->
                    <div class="col-xl-2 col-md-4 col-sm-6">

                        <div class="dash-stat">

                            <div class="d-flex align-items-center justify-content-between">

                                <div>

                                    <div class="ds-value"
                                         id="lblTotalClasses"
                                         runat="server">0</div>

                                    <div class="ds-label">
                                        الفصول
                                    </div>

                                </div>

                                <div class="ds-icon">
                                    <i class="fas fa-chalkboard"></i>
                                </div>

                            </div>

                        </div>

                    </div>


                    <!-- المعلمون -->
                    <div class="col-xl-2 col-md-4 col-sm-6">

                        <div class="dash-stat">

                            <div class="d-flex align-items-center justify-content-between">

                                <div>

                                    <div class="ds-value"
                                         id="lblTotalTeachers"
                                         runat="server">0</div>

                                    <div class="ds-label">
                                        المعلمون
                                    </div>

                                </div>

                                <div class="ds-icon">
                                    <i class="fas fa-chalkboard-teacher"></i>
                                </div>

                            </div>

                        </div>

                    </div>


                    <!-- حسب المرحلة -->
                    <div class="col-xl-2 col-md-4 col-sm-6">

                        <div class="dash-stat">

                            <div class="d-flex align-items-center justify-content-between">

                                <div>

                                    <div class="ds-value"
                                         id="lblTotalStudents"
                                         runat="server">0</div>

                                    <div class="ds-label"
                                         id="lblStageLabel"
                                         runat="server">
                                        طلاب
                                    </div>

                                </div>

                                <div class="ds-icon">
                                    <i class="fas fa-user-graduate"></i>
                                </div>

                            </div>

                        </div>

                    </div>

                </div>


                <!-- MAIN ROW -->
                <div class="row g-3">


                    <!-- العمود الأيمن -->
                    <div class="col-xl-8">


                        <!-- أحدث المدفوعات -->
                        <div class="card academia-card"
                             id="pnlRecentPayments"
                             runat="server"
                             visible="false">

                            <div class="card-body pt-3">

                                <div class="d-flex align-items-center justify-content-between mb-2">

                                    <h6 class="fw-bold mb-0 d-flex align-items-center">
                                        <i class="fas fa-receipt me-2 text-academia"></i>
                                        أحدث المدفوعات
                                    </h6>

                                    <a href='<%= ResolveClientUrl("~/Pages/Payments/Default.aspx") %>'
                                       class="btn btn-academia btn-sm px-3">

                                        <i class="fas fa-eye me-1"></i>
                                        عرض الكل

                                    </a>

                                </div>


                                <div class="table-responsive">

                                    <asp:GridView
                                        ID="gvRecentPayments"
                                        runat="server"
                                        CssClass="table table-custom table-borderless table-hover mb-0 align-middle"
                                        AutoGenerateColumns="false"
                                        EmptyDataText="لا توجد مدفوعات حتى الآن"
                                        ShowHeader="true"
                                        GridLines="None">

                                        <Columns>

                                            <asp:TemplateField HeaderText="الطالب">

                                                <ItemTemplate>

                                                    <div class="d-flex align-items-center gap-2">

                                                        <div class="pmt-avatar">
                                                            <i class="fas fa-user-graduate"
                                                               style="font-size:13px;"></i>
                                                        </div>

                                                        <span class="pmt-name">
                                                            <%# Eval("StudentName") %>
                                                        </span>

                                                    </div>

                                                </ItemTemplate>

                                            </asp:TemplateField>


                                            <asp:TemplateField HeaderText="المبلغ">

                                                <ItemTemplate>

                                                    <span class="pmt-amount">
                                                        <%# Eval("Amount", "{0:N2}") %>
                                                        د.ل
                                                    </span>

                                                </ItemTemplate>

                                            </asp:TemplateField>


                                            <asp:TemplateField HeaderText="التاريخ">

                                                <ItemTemplate>

                                                    <span class="pmt-date">
                                                        <%# Eval("PaymentDate", "{0:yyyy/MM/dd}") %>
                                                    </span>

                                                </ItemTemplate>

                                            </asp:TemplateField>


                                            <asp:TemplateField HeaderText="الطريقة">

                                                <ItemTemplate>

                                                    <span class="inv-badge">
                                                        <%# Eval("PaymentMethodName") %>
                                                    </span>

                                                </ItemTemplate>

                                            </asp:TemplateField>

                                        </Columns>

                                    </asp:GridView>

                                </div>

                            </div>

                        </div>


                        <!-- الأساسي -->
                        <div class="chart-section">

                            <asp:Panel ID="pnlBasicChart" runat="server">

                                <div class="card academia-card">

                                    <div class="card-body pt-3">

                                        <h6 class="fw-bold mb-1 d-flex align-items-center">
                                            <i class="fas fa-book-open me-2 text-academia"></i>
                                            عدد الطلاب — الأساسي
                                        </h6>


                                        <asp:Repeater
                                            ID="rptBasicChart"
                                            runat="server"
                                            OnItemDataBound="rptBasicChart_ItemDataBound">

                                            <HeaderTemplate>
                                                <div class="chart-cols">
                                            </HeaderTemplate>


                                            <ItemTemplate>

                                                <div class="chart-col">

                                                    <span class="chart-col-value">
                                                        <%# Eval("Count") %>
                                                    </span>

                                                    <div class="chart-col-track">

                                                        <div class="chart-col-fill"
                                                             id="chartFill"
                                                             runat="server">
                                                        </div>

                                                    </div>

                                                    <span class="chart-col-name">
                                                        <%# Eval("ClassName") %>
                                                    </span>

                                                </div>

                                            </ItemTemplate>


                                            <FooterTemplate>
                                                </div>
                                            </FooterTemplate>

                                        </asp:Repeater>


                                        <asp:Label
                                            ID="lblBasicChartEmpty"
                                            runat="server"
                                            CssClass="text-muted"
                                            Style="font-size:13px;display:none;">
                                            لا توجد بيانات
                                        </asp:Label>

                                    </div>

                                </div>

                            </asp:Panel>

                        </div>


                        <!-- الثانوي -->
                        <div class="chart-section">

                            <asp:Panel ID="pnlSecondaryChart" runat="server">

                                <div class="card academia-card">

                                    <div class="card-body pt-3">

                                        <h6 class="fw-bold mb-1 d-flex align-items-center">
                                            <i class="fas fa-flask me-2 text-academia"></i>
                                            عدد الطلاب — الثانوي
                                        </h6>


                                        <asp:Repeater
                                            ID="rptSecondaryChart"
                                            runat="server"
                                            OnItemDataBound="rptSecondaryChart_ItemDataBound">

                                            <HeaderTemplate>
                                                <div class="chart-cols">
                                            </HeaderTemplate>


                                            <ItemTemplate>

                                                <div class="chart-col">

                                                    <span class="chart-col-value">
                                                        <%# Eval("Count") %>
                                                    </span>

                                                    <div class="chart-col-track">

                                                        <div class="chart-col-fill secondary"
                                                             id="chartFillSecondary"
                                                             runat="server">
                                                        </div>

                                                    </div>

                                                    <span class="chart-col-name">
                                                        <%# Eval("ClassName") %>
                                                    </span>

                                                </div>

                                            </ItemTemplate>


                                            <FooterTemplate>
                                                </div>
                                            </FooterTemplate>

                                        </asp:Repeater>


                                        <asp:Label
                                            ID="lblSecondaryChartEmpty"
                                            runat="server"
                                            CssClass="text-muted"
                                            Style="font-size:13px;display:none;">
                                            لا توجد بيانات
                                        </asp:Label>

                                    </div>

                                </div>

                            </asp:Panel>

                        </div>

                    </div>


                    <!-- العمود الأيسر -->
                    <div class="col-xl-4">


                        <!-- حضور الطلاب -->
                        <div class="dash-panel mb-3">

                            <div class="dp-head">

                                <h6>
                                    <i class="fas fa-clipboard-check"></i>
                                    حضور الطلاب اليوم
                                </h6>

                            </div>


                            <div class="dp-section-title px-3 pt-2 pb-1"
                                 style="font-size:12px;font-weight:700;color:#00b14f;background:rgba(0,177,79,.04);border-bottom:1px solid #f0f0f0;">

                                <i class="fas fa-book-open me-1"></i>
                                المرحلة الأساسية

                            </div>


                            <div class="dp-row">

                                <span class="dr-left">
                                    الحاضرون
                                </span>

                                <span class="dr-right"
                                      style="color:#00b14f;"
                                      id="lblPresentBasic"
                                      runat="server">
                                    0
                                </span>

                            </div>


                            <div class="dp-row">

                                <span class="dr-left">
                                    الغائبون
                                </span>

                                <span class="dr-right"
                                      style="color:#dc2626;"
                                      id="lblAbsentBasic"
                                      runat="server">
                                    0
                                </span>

                            </div>


                            <div class="dp-section-title px-3 pt-2 pb-1"
                                 style="font-size:12px;font-weight:700;color:#00b14f;background:rgba(0,177,79,.04);border-bottom:1px solid #f0f0f0;border-top:1px solid #f0f0f0;">

                                <i class="fas fa-flask me-1"></i>
                                المرحلة الثانوية

                            </div>


                            <div class="dp-row">

                                <span class="dr-left">
                                    الحاضرون
                                </span>

                                <span class="dr-right"
                                      style="color:#00b14f;"
                                      id="lblPresentSecondary"
                                      runat="server">
                                    0
                                </span>

                            </div>


                            <div class="dp-row">

                                <span class="dr-left">
                                    الغائبون
                                </span>

                                <span class="dr-right"
                                      style="color:#dc2626;"
                                      id="lblAbsentSecondary"
                                      runat="server">
                                    0
                                </span>

                            </div>


                            <div class="dp-progress">

                                <div class="bar"
                                     id="attendanceBar"
                                     runat="server"
                                     style="width:0%;">
                                </div>

                            </div>


                            <div class="dp-row">

                                <span class="dr-left">
                                    نسبة الحضور الكلية
                                </span>

                                <span class="dr-right"
                                      id="lblAttendanceRate"
                                      runat="server">
                                    -
                                </span>

                            </div>

                        </div>


                        <!-- حضور المعلمين بالساعات -->
                        <div class="dash-panel">

                            <div class="dp-head">

                                <h6>
                                    <i class="fas fa-chalkboard-teacher"></i>
                                    حضور المعلمين والمعلمات بالساعات اليوم
                                </h6>

                            </div>


                            <div class="dp-row">

                                <span class="dr-left">
                                    المسجل حضورهم بالساعات
                                </span>

                                <span class="dr-right"
                                      style="color:#00b14f;"
                                      id="lblPresentTeachers"
                                      runat="server">
                                    0
                                </span>

                            </div>


                            <div class="dp-row">

                                <span class="dr-left">
                                    المسجل غيابهم بالساعات
                                </span>

                                <span class="dr-right"
                                      style="color:#dc2626;"
                                      id="lblAbsentTeachers"
                                      runat="server">
                                    0
                                </span>

                            </div>

                        </div>

                    </div>

                </div>


                <asp:Label
                    ID="lblError"
                    runat="server"
                    CssClass="text-danger d-none" />

            </div>

        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>