<%@ Page Title="حذف رسوم" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Fees.Delete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid" dir="rtl">
        <div class="card-custom">
            <div class="card-header-custom">
                <h6><i class="fas fa-trash me-2"></i>حذف الرسوم</h6>
            </div>
            <div class="card-body-custom">
                <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
                    <div class="alert alert-danger">الرسوم غير موجودة</div>
                    <a href="Default.aspx" class="btn btn-secondary">العودة</a>
                </asp:Panel>

                <asp:Panel ID="pnlForm" runat="server" Visible="false">
                    <div class="alert alert-warning">هل أنت متأكد من حذف هذه الرسوم؟</div>
                    <div class="card-custom mb-3">
                        <div class="card-body-custom">
                            <div class="row">
                                <div class="col-md-6"><strong>اسم الرسوم:</strong> <asp:Label ID="lblName" runat="server" /></div>
                                <div class="col-md-6"><strong>المبلغ:</strong> <asp:Label ID="lblAmount" runat="server" /></div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-6"><strong>الصف:</strong> <asp:Label ID="lblClassRoom" runat="server" /></div>
                            </div>
                        </div>
                    </div>
                    <asp:Label ID="lblMessage" runat="server" CssClass="mb-3 d-block" />
                    <div>
                        <asp:Button ID="btnDelete" runat="server" Text="تأكيد الحذف" CssClass="btn btn-danger" OnClick="btnDelete_Click" />
                        <a href="Default.aspx" class="btn btn-secondary ms-2">إلغاء</a>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
