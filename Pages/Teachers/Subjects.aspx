<%@ Page Title="مواد المعلم" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Subjects.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Teachers.Subjects" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="container-fluid" dir="rtl">
    <div class="academia-card">
        <div class="card-header border-bottom d-flex align-items-center justify-content-between py-3">
            <div>
                <h6 class="mb-1 text-academia"><i class="fas fa-book me-2"></i>مواد وتكليفات المعلم</h6>
                <div class="text-muted" style="font-size:12px;">المعلم: <strong><asp:Label ID="lblTeacherName" runat="server" /></strong></div>
            </div>
            <a class="btn btn-outline-secondary btn-sm" href="Default.aspx">عودة</a>
        </div>
        <div class="card-body">
            <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" />
            <div class="alert alert-info" style="font-size:12px;">
                يمكن للمعلم الواحد تدريس أكثر من مادة دون إنشاء سجل معلم جديد. سعر الحصة هنا خاص بالمادة،
                بينما السعر الذي دخل في راتب قديم يبقى محفوظاً داخل سجل الحضور والكشف المالي.
            </div>
            <div class="row g-2 mb-4">
                <div class="col-md-5">
                    <label class="form-label fw-bold">المادة</label>
                    <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-2">
                    <label class="form-label fw-bold">قيمة الحصة</label>
                    <asp:TextBox ID="txtLessonRate" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" />
                </div>
                <div class="col-md-2">
                    <label class="form-label fw-bold">قيمة الساعة</label>
                    <asp:TextBox ID="txtHourlyRate" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" />
                </div>
                <div class="col-md-3 d-flex align-items-end">
                    <asp:Button ID="btnSave" runat="server" Text="إضافة / تحديث المادة" CssClass="btn btn-academia w-100" OnClick="btnSave_Click" />
                </div>
            </div>
            <asp:GridView ID="gvAssignments" runat="server" CssClass="table table-custom table-borderless table-hover" AutoGenerateColumns="false" GridLines="None" OnRowCommand="gvAssignments_RowCommand">
                <Columns>
                    <asp:BoundField DataField="SubjectName" HeaderText="المادة" />
                    <asp:BoundField DataField="LessonRate" HeaderText="قيمة الحصة" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="HourlyRate" HeaderText="قيمة الساعة" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="StartDate" HeaderText="من" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="EndDate" HeaderText="إلى" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:TemplateField HeaderText="الحالة">
                        <ItemTemplate><span class='<%# (bool)Eval("IsActive") ? "badge-soft green" : "badge-soft gray" %>'><%# (bool)Eval("IsActive") ? "نشط" : "غير نشط" %></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="إجراء">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnToggle" runat="server" CommandName="Toggle" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-outline-secondary">
                                <%# (bool)Eval("IsActive") ? "تعطيل" : "تفعيل" %>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>لا توجد مواد مسجلة لهذا المعلم.</EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</div>
</asp:Content>
