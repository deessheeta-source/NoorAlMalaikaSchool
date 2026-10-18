<%@ Page Title="المدفوعات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Payments.Default" %>
<%@ Register Src="~/Controls/FinanceNav.ascx" TagPrefix="uc" TagName="FinanceNav" %>

<asp:Content ID="HeadContent1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid" dir="rtl">

        <uc:FinanceNav runat="server" ActivePage="Payments" />

        <div class="mb-4 d-flex align-items-center justify-content-between flex-wrap gap-3">
            <div class="d-flex align-items-center">
                <h6 class="mb-0 fw-bold d-flex align-items-center">
                    <i class="fas fa-credit-card me-2 text-academia"></i> المدفوعات
                </h6>
                <span class="badge bg-academia ms-3 rounded-pill px-3 py-2 fs-6"><asp:Literal ID="lblStage" runat="server" /></span>
            </div>

            <%-- ✅ زر العودة الذكي --%>
            <a href="<%= _returnUrl %>" class="btn btn-academia px-4">
                <i class="fas fa-arrow-right me-1"></i> العودة
            </a>
        </div>

        <div class="card academia-card mb-4 filter-card">
            <div class="card-body py-3">
                <div class="row g-3 align-items-end">
                    <div class="col-md-4">
                        <label class="form-label fw-bold mb-1">البحث بالطالب</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="اسم الطالب" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-academia w-100" OnClick="btnSearch_Click" />
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlTable" runat="server">
            <div class="card academia-card">
                <div class="card-body pt-3">
                    <div class="table-responsive">
                        <table class="table table-custom table-borderless table-hover mb-0 text-center align-middle">
                            <thead>
                                <tr>
                                    <th style="width: 50px;">#</th>
                                    <th>اسم الطالب</th>
                                    <th>الصف</th>
                                    <th>نوع الرسوم</th>
                                    <th> المبلغ المدفوع</th>
                                    <th>التاريخ</th>
                                    <th>طريقة الدفع</th>
                                    <th>رقم الإيصال</th>
                                    <th>الإجراءات</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptPayments" runat="server">
                                    <ItemTemplate>
                                        <tr class="border-bottom fee-row">
                                            <td class="text-muted fw-bold"><%# Container.ItemIndex + 1 %></td>
                                            <td class="fw-semibold text-dark py-3">
                                                <div class="d-flex align-items-center gap-2">
                                                    <div class="rounded-circle d-flex align-items-center justify-content-center" style="width:32px; height:32px; flex-shrink:0; background-color: rgba(0,177,79,0.1) !important;">
                                                        <i class="fas fa-user-graduate text-academia" style="font-size:13px; color:#00b14f;"></i>
                                                    </div>
                                                    <%# Eval("StudentName") %>
                                                </div>
                                            </td>
                                            <td class="text-muted"><%# Eval("ClassRoomName") %></td>
                                            <td><%# Eval("FeeName") %></td>
                                            <td>
                                                <span class="fw-bold text-academia"><%# FormatAmount((decimal)Eval("Amount")) %></span>
                                            </td>
                                            <td class="text-muted"><%# Eval("PaymentDate", "{0:yyyy-MM-dd}") %></td>
                                            <td><%# RenderMethodBadge(Eval("MethodName")) %></td>
                                            <td class="text-muted"><%# string.IsNullOrEmpty((string)Eval("ReceiptNumber")) ? "—" : Eval("ReceiptNumber") %></td>
                                            <td>
                                                <a href='<%# "StudentReport.aspx?studentId=" + Eval("StudentId") + "&stage=" + Server.UrlEncode(CurrentStage) %>'
                                                   class="btn btn-sm px-3" title="تقرير الطالب"
                                                   style="border:1px solid #00b14f; color:#00b14f; font-size:12px;">
                                                    <i class="fas fa-file-alt me-1"></i> التقرير
                                                </a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlEmpty" runat="server">
            <div class="card academia-card">
                <div class="card-body">
                    <div class="text-center">
                        <div class="mb-3">
                            <i class="fas fa-credit-card fa-4x" style="color: #d4edda;"></i>
                        </div>
                        <h5 class="fw-bold text-muted">لا توجد مدفوعات</h5>
                        <p class="text-muted mb-0">لم يتم تسجيل أي دفعة لهذه المرحلة بعد</p>

                        <%-- ✅ زر العودة الذكي (بدل "إضافة دفعة جديدة") --%>
                        <a href="<%= _returnUrl %>" class="btn btn-academia rounded-pill px-4 mt-3">
                            <i class="fas fa-arrow-right me-1"></i> العودة
                        </a>
                    </div>
                </div>
            </div>
        </asp:Panel>

    </div>
</asp:Content>