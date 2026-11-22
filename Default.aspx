<%@ Page Title="لوحة التحكم"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="NoorAlMalaikaSchool.Default" %>

<asp:Content ID="HeadContent1" ContentPlaceHolderID="HeadContent" runat="server">

    <link href="<%= ResolveClientUrl("~/Content/css/dashboard.css") %>" rel="stylesheet" />

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="upDashboard"
        runat="server"
        UpdateMode="Conditional">

        <ContentTemplate>

            <div class="dashboard">

                <!-- =========================================================
                     HERO — المدرسة واليوم
                     ========================================================= -->
                <div class="dashboard-hero">

                    <div class="hero-brand">
                        <div class="hero-mark">
                            <i class="fas fa-graduation-cap"></i>
                        </div>
                        <div>
                            <div class="hero-school">مدرسة نور الملائكة</div>
                            <div class="hero-sub">Noor Al Malaika School — لوحة التحكم</div>
                        </div>
                    </div>

                    <div class="hero-today">
                        <i class="far fa-calendar-alt"></i>
                        <span class="hero-date">اليوم</span>
                    </div>

                    <span class="dash-stage-badge" id="lblStageLabel" runat="server">طلاب</span>

                </div>

                <!-- =========================================================
                     STAT STRIP — أرقام اليوم기의 ملخصة
                     ========================================================= -->
                <div class="dashboard-strip">

                    <div class="dash-pulse-card">
                        <h6 class="pulse-title">
                            <i class="fas fa-chart-area"></i>
                            نظرة سريعة على اليوم
                        </h6>

                        <div class="pulse-grid">

                            <!-- إجمالي الطلاب -->
                            <div class="pulse-stat green">
                                <span class="ps-label">إجمالي الطلاب</span>
                                <span class="ps-value" id="lblTotalStudentsAll" runat="server">0</span>
                                <i class="fas fa-users ps-icon"></i>
                            </div>

                            <!-- الفصول -->
                            <div class="pulse-stat slate">
                                <span class="ps-label">الفصول</span>
                                <span class="ps-value" id="lblTotalClasses" runat="server">0</span>
                                <i class="fas fa-chalkboard ps-icon"></i>
                            </div>

                            <!-- طلاب المرحلة المختارة -->
                            <div class="pulse-stat green">
                                <span class="ps-label">طلاب المرحلة المختارة</span>
                                <span class="ps-value" id="lblTotalStudents" runat="server">0</span>
                                <i class="fas fa-user-graduate ps-icon"></i>
                            </div>

                            <!-- المعلمون -->
                            <div class="pulse-stat slate">
                                <span class="ps-label">المعلمون</span>
                                <span class="ps-value" id="lblTotalTeachers" runat="server">0</span>
                                <i class="fas fa-chalkboard-teacher ps-icon"></i>
                            </div>

                            <!-- المدفوعات (تُعرض للموظفين المخولين فقط) -->
                            <div id="pnlPaymentsStat" runat="server" visible="false"
                                 class="pulse-stat amber">
                                <span class="ps-label">المدفوعات</span>
                                <span class="ps-value" id="lblTotalPayments" runat="server">0</span>
                                <i class="fas fa-coins ps-icon"></i>
                            </div>

                            <!-- الغياب اليوم -->
                            <div class="pulse-stat terracotta">
                                <span class="ps-label">الغياب اليوم</span>
                                <span class="ps-value" id="lblAbsentToday" runat="server">0</span>
                                <i class="fas fa-user-times ps-icon"></i>
                            </div>

                            <!-- نسبة الحضور -->
                            <div class="pulse-stat slate">
                                <span class="ps-label">نسبة الحضور</span>
                                <span class="ps-value" id="lblAttendanceRateBig" runat="server">-</span>
                                <i class="fas fa-percentage ps-icon"></i>
                            </div>

                            <!-- المعلمون الحاضرون اليوم -->
                            <div class="pulse-stat green">
                                <span class="ps-label">المعلمون الحاضرون</span>
                                <span class="ps-value" id="lblPresentTeachers" runat="server">0</span>
                                <i class="fas fa-user-check ps-icon"></i>
                            </div>

                            <!-- المعلمون الغائبون اليوم -->
                            <div class="pulse-stat terracotta">
                                <span class="ps-label">المعلمون الغائبون</span>
                                <span class="ps-value" id="lblAbsentTeachers" runat="server">0</span>
                                <i class="fas fa-user-times ps-icon"></i>
                            </div>

                        </div>
                    </div>

                </div>

                <!-- =========================================================
                     MAIN GRID — عمودان (2/1)
                     ========================================================= -->
                <div class="dashboard-grid">

                    <!-- العمود الأيمن — الحضور والرسم البياني -->
                    <div class="dash-column">

                        <!-- حضور الطلاب اليوم -->
                        <div class="dash-section">

                            <div class="dash-section-head">
                                <h6 class="dash-section-title">
                                    <i class="fas fa-clipboard-check"></i>
                                    حضور الطلاب والغياب اليوم
                                </h6>
                            </div>

                            <div class="dash-section-body">
                                <div class="attendance-wrap">

                                    <!-- المرحلة الأساسية -->
                                    <div class="attendance-stage">
                                        <div class="att-stage-head">
                                            <span class="stage-name">المرحلة الأساسية</span>
                                        </div>
                                        <div class="att-bar-track">
                                            <div class="att-bar-fill" id="attendanceBar" runat="server" style="width:0%"></div>
                                        </div>
                                        <div class="att-stage-row basic">
                                            <span class="att-label">حاضرون</span>
                                            <span class="att-val att-present" id="lblPresentBasic" runat="server">0</span>
                                        </div>
                                        <div class="att-stage-row basic">
                                            <span class="att-label">غائبون</span>
                                            <span class="att-val att-absent" id="lblAbsentBasic" runat="server">0</span>
                                        </div>
                                    </div>

                                    <!-- المرحلة الثانوية -->
                                    <div class="attendance-stage">
                                        <div class="att-stage-head">
                                            <span class="stage-name">المرحلة الثانوية</span>
                                        </div>
                                        <div class="att-stage-row basic">
                                            <span class="att-label">حاضرون</span>
                                            <span class="att-val att-present" id="lblPresentSecondary" runat="server">0</span>
                                        </div>
                                        <div class="att-stage-row basic">
                                            <span class="att-label">غائبون</span>
                                            <span class="att-val att-absent" id="lblAbsentSecondary" runat="server">0</span>
                                        </div>
                                    </div>

                                    <!-- نسبة الحضور الكلية -->
                                    <div class="att-rate">
                                        <span>نسبة الحضور الكلية</span>
                                        <span class="rate-val">-</span>
                                    </div>

                                </div>
                            </div>

                        </div>

                        <!-- توزيع الطلاب حسب الفصول (رسم أعمدة) -->
                        <div class="dash-section chart-section">

                            <div class="dash-section-head">
                                <h6 class="dash-section-title">
                                    <i class="fas fa-chart-bar"></i>
                                    توزيع الطلاب حسب الفصول
                                </h6>
                            </div>

                            <div class="dash-section-body">

                                <asp:Panel ID="pnlBasicChart" runat="server">
                                    <div class="chart-cols">
                                        <asp:Repeater ID="rptBasicChart" runat="server"
                                                      OnItemDataBound="rptBasicChart_ItemDataBound">
                                            <ItemTemplate>
                                                <div class="chart-col">
                                                    <span class="chart-col-value"><%# Eval("Count") %></span>
                                                    <div class="chart-col-track">
                                                        <div class="chart-col-fill" id="chartFill" runat="server"></div>
                                                    </div>
                                                    <span class="chart-col-name"><%# Eval("ClassName") %></span>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </asp:Panel>

                                <asp:Label ID="lblBasicChartEmpty" runat="server" CssClass="chart-empty">
                                    <i class="fas fa-book-open"></i>
                                    لا توجد بيانات للفصول الأساسية
                                </asp:Label>

                                <asp:Panel ID="pnlSecondaryChart" runat="server">
                                    <div class="chart-cols">
                                        <asp:Repeater ID="rptSecondaryChart" runat="server"
                                                      OnItemDataBound="rptSecondaryChart_ItemDataBound">
                                            <ItemTemplate>
                                                <div class="chart-col">
                                                    <span class="chart-col-value"><%# Eval("Count") %></span>
                                                    <div class="chart-col-track">
                                                        <div class="chart-col-fill secondary" id="chartFillSecondary" runat="server"></div>
                                                    </div>
                                                    <span class="chart-col-name"><%# Eval("ClassName") %></span>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </asp:Panel>

                                <asp:Label ID="lblSecondaryChartEmpty" runat="server" CssClass="chart-empty">
                                    <i class="fas fa-flask"></i>
                                    لا توجد بيانات للفصول الثانوية
                                </asp:Label>

                            </div>

                        </div>

                    </div>

                    <!-- العمود الأيسر — المدفوعات والإجراءات السريعة -->
                    <div class="dash-column">

                        <!-- أحدث المدفوعات -->
                        <div id="pnlRecentPayments" runat="server" visible="false"
                             class="dash-section">

                            <div class="dash-section-head">
                                <h6 class="dash-section-title">
                                    <i class="fas fa-receipt"></i>
                                    أحدث المدفوعات
                                </h6>
                                <a href='<%= ResolveClientUrl("~/Pages/Payments/Default.aspx") %>'
                                   class="dash-view-all">
                                    عرض الكل <i class="fas fa-chevron-right"></i>
                                </a>
                            </div>

                            <div class="dash-section-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="gvRecentPayments" runat="server"
                                                  CssClass="recent-table"
                                                  AutoGenerateColumns="false"
                                                  EmptyDataText="لا توجد مدفوعات حتى الآن"
                                                  ShowHeader="true"
                                                  GridLines="None">
                                        <Columns>
                                            <asp:TemplateField HeaderText="الطالب">
                                                <ItemTemplate>
                                                    <div class="d-flex align-items-center gap-2">
                                                        <div class="recent-avatar">
                                                            <i class="fas fa-user-graduate"></i>
                                                        </div>
                                                        <span class="recent-name"><%# Eval("StudentName") %></span>
                                                    </div>
                                                    <span class="recent-meta"><%# Eval("PaymentMethodName") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="المبلغ">
                                                <ItemTemplate>
                                                    <span class="recent-amount"><%# Eval("Amount", "{0:N2}") %></span> د.ل
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="التاريخ">
                                                <ItemTemplate>
                                                    <span class="recent-date"><%# Eval("PaymentDate", "{0:yyyy/MM/dd}") %></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>

                                <div class="recent-empty">
                                    <i class="fas fa-inbox"></i>
                                    لا توجد مدفوعات حتى الآن
                                </div>
                            </div>

                        </div>

                        <!-- إجراءات سريعة -->
                        <div class="dash-section">

                            <div class="dash-section-head">
                                <h6 class="dash-section-title">
                                    <i class="fas fa-bolt"></i>
                                    إجراءات سريعة
                                </h6>
                            </div>

                            <div class="dash-section-body">
                                <div class="quick-actions">
                                    <a href='<%= ResolveClientUrl("~/Pages/Students/Default.aspx") %>'
                                       class="action-btn">
                                        <span>إدارة الطلاب</span>
                                        <i class="fas fa-user-graduate"></i>
                                    </a>
                                    <a href='<%= ResolveClientUrl("~/Pages/Teachers/Default.aspx") %>'
                                       class="action-btn">
                                        <span>إدارة المعلمين</span>
                                        <i class="fas fa-chalkboard-teacher"></i>
                                    </a>
                                    <a href='<%= ResolveClientUrl("~/Pages/Classes/Default.aspx") %>'
                                       class="action-btn secondary">
                                        <span>إدارة الصفوف</span>
                                        <i class="fas fa-chalkboard"></i>
                                    </a>
                                    <a href='<%= ResolveClientUrl("~/Pages/Subjects/Default.aspx") %>'
                                       class="action-btn amber">
                                        <span>إدارة المواد</span>
                                        <i class="fas fa-book-open"></i>
                                    </a>
                                </div>
                            </div>

                        </div>

                    </div>

                </div>

                <asp:Label ID="lblError" runat="server" CssClass="dash-error" />

            </div>

        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>
