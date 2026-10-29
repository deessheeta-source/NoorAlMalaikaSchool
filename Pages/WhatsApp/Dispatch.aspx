<%@ Page Title="إرسال الواتساب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dispatch.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.WhatsApp.WhatsAppDispatchPage" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- استخدام نفس أنماط الواتساب في الموقع -->
    <style>
        .wa-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 32px; }
        .wa-card { background: #fff; border-radius: 20px; border: 1px solid #eef1f3; box-shadow: 0 4px 12px rgba(0,0,0,0.05); overflow: hidden; max-width: 1000px; margin: 0 auto; }
        .wa-card-header { padding: 24px; border-bottom: 1px solid #f1f3f5; display: flex; align-items: center; justify-content: space-between; }
        .wa-card-body { padding: 24px; }
        .wa-grid { width: 100%; border-collapse: separate; border-spacing: 0; font-size: 13px; }
        .wa-grid th { background: #f8f9fa; padding: 14px; text-align: right; border-bottom: 2px solid #dee2e6; }
        .wa-grid td { padding: 14px; border-bottom: 1px solid #f1f3f5; }
        .btn-success { background: #25D366; border: none; color: #fff; padding: 8px 16px; border-radius: 8px; text-decoration: none; font-size: 12px; }
        .btn-success:hover { background: #128C7E; color: #fff; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="wa-page" dir="rtl">
        <div class="wa-card">
            <div class="wa-card-header">
                <h2>لوحة إرسال الواتساب</h2>
                <div class="badge bg-primary fs-6"><asp:Literal ID="lblPendingCount" runat="server" /> رسالة معلقة</div>
            </div>
            <div class="wa-card-body">
                <div class="alert alert-info">افتح رابط واتساب، أرسل الرسالة هناك، ثم ارجع واضغط «تأكيد الإرسال». لا يستطيع الموقع التحقق من الإرسال داخل تطبيق واتساب.</div>
                <asp:Literal ID="lblDispatchResult" runat="server" />
                <table class="wa-grid">
                    <thead>
                        <tr>
                            <th>المستلم</th>
                            <th>الرسالة</th>
                            <th class="text-center">الإجراء</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptMessages" runat="server" OnItemCommand="rptMessages_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="fw-bold"><%#: Eval("RecipientName") %></div>
                                        <small class="text-muted"><%#: Eval("PhoneNumber") %></small>
                                    </td>
                                    <td><%#: Eval("MessageBody") %></td>
                                    <td class="text-center">
                                        <a href='<%# GetWhatsAppLink(Eval("PhoneNumber"), Eval("MessageBody")) %>'
                                            target="_blank" rel="noopener noreferrer" class="btn btn-success">
                                            <i class="fab fa-whatsapp"></i> فتح واتساب
                                        </a>
                                        <asp:LinkButton ID="btnSend" runat="server"
                                            CommandName="ConfirmSent"
                                            CommandArgument='<%# Eval("Id") %>'
                                            CssClass="btn btn-primary"
                                            OnClientClick="return confirm('هل أرسلت هذه الرسالة فعلياً من واتساب؟');">
                                            تأكيد الإرسال
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
