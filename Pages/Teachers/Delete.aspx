<%@ Page Title="حذف معلم/معلمة" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Teachers.Delete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />

    <div class="container-fluid" dir="rtl">
        <div class="academia-card" style="max-width:780px;margin:20px auto;">

            <div class="card-header border-bottom d-flex align-items-center py-2">
                <h6 class="mb-0 text-danger">
                    <i class="fas fa-trash me-2"></i> حذف معلم/معلمة
                </h6>
            </div>

            <div class="card-body">

                <asp:Label ID="lblMessage" runat="server" CssClass="d-none" />
                <asp:HiddenField ID="hfTeacherId" runat="server" />

                <!-- ========================= -->
                <!-- غير موجود -->
                <!-- ========================= -->
                <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="text-center py-5">
                    <i class="fas fa-exclamation-triangle fa-3x text-warning mb-3"></i>
                    <p class="text-muted">المعلم/المعلمة غير موجود</p>
                    <asp:Button ID="btnBackToList" runat="server" Text="العودة للقائمة"
                        CssClass="btn btn-academia" OnClick="btnCancel_Click" />
                </asp:Panel>


                <!-- ========================= -->
                <!-- تأكيد الحذف (لا توجد ارتباطات) -->
                <!-- ========================= -->
                <asp:Panel ID="pnlConfirm" runat="server" Visible="false">

                    <div class="alert alert-warning d-flex gap-2 align-items-start">
                        <i class="fas fa-exclamation-triangle mt-1"></i>
                        <div>
                            <strong>تحذير:</strong> سيتم حذف السجل نهائياً. لا يمكن التراجع عن هذه العملية.
                        </div>
                    </div>

                    <div style="border:1px solid #e6eef0;border-radius:12px;padding:14px;background:#fbfdfc;margin-bottom:14px;">
                        <div style="display:grid;grid-template-columns:140px 1fr;gap:10px;font-size:13px;">
                            <strong>الاسم:</strong><span><asp:Label ID="lblName" runat="server" /></span>
                            <strong>الجنس:</strong><span><asp:Label ID="lblGender" runat="server" /></span>
                            <strong>الرقم التعريفي:</strong><span><asp:Label ID="lblEmployeeNo" runat="server" /></span>
                            <strong>التخصص:</strong><span><asp:Label ID="lblSpecialty" runat="server" /></span>
                            <strong>الهاتف:</strong><span><asp:Label ID="lblPhone" runat="server" /></span>
                            <strong>الحالة:</strong><span><asp:Label ID="lblStatus" runat="server" /></span>
                        </div>
                    </div>

                    <div class="d-flex gap-2 justify-content-end">
                        <asp:Button ID="btnCancel" runat="server" Text="إلغاء"
                            CssClass="btn btn-outline-secondary"
                            OnClick="btnCancel_Click" CausesValidation="false" />

                        <asp:Button ID="btnConfirmDelete" runat="server" Text="تأكيد الحذف النهائي"
                            CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click"
                            OnClientClick="return confirm('هل أنت متأكد من الحذف النهائي؟ لا يمكن التراجع.');" />
                    </div>
                </asp:Panel>


                <!-- ========================= -->
                <!-- لا يمكن الحذف (توجد ارتباطات) -->
                <!-- ========================= -->
                <asp:Panel ID="pnlBlocked" runat="server" Visible="false">

                    <div class="alert alert-danger d-flex gap-2 align-items-start">
                        <i class="fas fa-lock mt-1"></i>
                        <div>
                            <strong>لا يمكن الحذف:</strong>
                            <div style="margin-top:6px;">
                                <asp:Label ID="lblBlockReason" runat="server" />
                            </div>
                        </div>
                    </div>

                    <div class="alert alert-info d-flex gap-2 align-items-start">
                        <i class="fas fa-lightbulb mt-1"></i>
                        <div>
                            <strong>البديل:</strong> يمكنك <strong>إلغاء تنشيط</strong> المعلم بدلاً من حذفه.
                            سيختفي من القوائم النشطة ولكن تبقى سجلاته التاريخية سليمة.
                        </div>
                    </div>

                    <div style="border:1px solid #e6eef0;border-radius:12px;padding:14px;background:#fbfdfc;margin-bottom:14px;">
                        <div style="display:grid;grid-template-columns:140px 1fr;gap:10px;font-size:13px;">
                            <strong>الاسم:</strong><span><asp:Label ID="lblName2" runat="server" /></span>
                            <strong>التخصص:</strong><span><asp:Label ID="lblSpecialty2" runat="server" /></span>
                            <strong>الحالة:</strong><span><asp:Label ID="lblStatus2" runat="server" /></span>
                        </div>
                    </div>

                    <div class="d-flex gap-2 justify-content-end">
                        <asp:Button ID="btnCancel2" runat="server" Text="رجوع للقائمة"
                            CssClass="btn btn-outline-secondary"
                            OnClick="btnCancel_Click" CausesValidation="false" />

                        <asp:Button ID="btnDeactivate" runat="server" Text="إلغاء التنشيط"
                            CssClass="btn btn-warning" OnClick="btnDeactivate_Click"
                            OnClientClick="return confirm('سيتم إلغاء تنشيط المعلم. يمكن إعادة تنشيطه لاحقاً. متابعة؟');" />
                    </div>
                </asp:Panel>

            </div>
        </div>
    </div>

</asp:Content>