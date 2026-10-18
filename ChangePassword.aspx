<%@ Page Title="تغيير كلمة المرور" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="NoorAlMalaikaSchool.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid" dir="rtl">
        <div class="row justify-content-center">
            <div class="col-lg-6">
                <div class="card-custom">
                    <div class="card-header-custom"><h6><i class="fas fa-key me-2"></i>تغيير كلمة المرور</h6></div>
                    <div class="card-body-custom">
                        <asp:Label ID="lblMessage" runat="server" CssClass="mb-3 d-block" />
                        <div class="mb-3">
                            <label class="form-label fw-bold">كلمة المرور الحالية</label>
                            <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="100" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label fw-bold">كلمة المرور الجديدة</label>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="100" />
                        </div>
                        <div class="mb-4">
                            <label class="form-label fw-bold">تأكيد كلمة المرور الجديدة</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="100" />
                        </div>
                        <div class="alert alert-info">يجب أن تتكون كلمة المرور من 8 أحرف على الأقل، وتحتوي على حرف كبير وحرف صغير ورقم ورمز خاص.</div>
                        <asp:Button ID="btnSave" runat="server" Text="حفظ كلمة المرور" CssClass="btn btn-primary-green" OnClick="btnSave_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
