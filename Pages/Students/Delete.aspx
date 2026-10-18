<%@ Page Title="حذف طالب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Students.Delete" %>
<%@ Import Namespace="NoorAlMalaikaSchool.Data.Helpers" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .del-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; }

        /* ---------- HERO ---------- */
        .del-hero-section { position: relative; background: linear-gradient(135deg,#dc3545,#a51c1c); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .del-hero-section::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%); pointer-events: none; }
        .del-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .del-hero-left { display: flex; align-items: center; gap: 20px; }
        .del-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; }
        .del-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; }
        .del-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; max-width: 640px; }
        .del-hero-chip { display: inline-flex; align-items: center; gap: 6px; padding: 5px 14px; border-radius: 999px; background: rgba(255,255,255,0.22); border: 1px solid rgba(255,255,255,0.3); color: #fff; font-weight: 700; font-size: 12.5px; margin-top: 8px; }
        .del-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; flex-shrink: 0; }
        .del-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }

        /* ---------- BODY ---------- */
        .del-body { max-width: 1280px; margin: -56px auto 0; padding: 0 24px 40px; position: relative; z-index: 3; }

        /* ---------- ALERT ---------- */
        .del-alert { display: flex; align-items: center; gap: 12px; padding: 14px 20px; border-radius: 14px; margin-bottom: 18px; font-size: 13.5px; font-weight: 700; box-shadow: 0 8px 24px rgba(0,0,0,0.06); }
        .del-alert i { width: 36px; height: 36px; border-radius: 10px; display: inline-flex; align-items: center; justify-content: center; font-size: 16px; flex-shrink: 0; color: #fff; }
        .del-alert.success { background: linear-gradient(180deg, #ffffff 0%, #f8fdfa 100%); border: 1.5px solid rgba(0,177,79,0.35); color: #1f3d33; }
        .del-alert.success i { background: linear-gradient(135deg, #00b14f, #4ed17e); }
        .del-alert.danger { background: linear-gradient(180deg, #ffffff 0%, #fef8f8 100%); border: 1.5px solid rgba(220,53,69,0.35); color: #7a1a26; }
        .del-alert.danger i { background: linear-gradient(135deg, #dc3545, #ff6b7a); }

        /* ---------- WARNING BANNER ---------- */
        .del-warning { background: linear-gradient(135deg, #fef2f2 0%, #fff5f5 100%); border: 1.5px solid rgba(220,53,69,0.3); border-radius: 18px; padding: 22px 26px; margin-bottom: 20px; display: flex; align-items: center; gap: 16px; position: relative; overflow: hidden; }
        .del-warning::before { content: ''; position: absolute; top: 0; right: 0; width: 6px; height: 100%; background: linear-gradient(180deg, #dc3545 0%, #ff6b7a 100%); }
        .del-warning-icon { width: 54px; height: 54px; border-radius: 14px; background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 24px; flex-shrink: 0; box-shadow: 0 6px 14px rgba(220,53,69,0.3); }
        .del-warning-content { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 200px; }
        .del-warning-label { font-size: 12px; font-weight: 800; color: #dc3545; text-transform: uppercase; letter-spacing: 0.5px; }
        .del-warning-title { font-size: 16px; font-weight: 800; color: #7a1a26; line-height: 1.4; }
        .del-warning-text { font-size: 13px; color: #a51c1c; line-height: 1.6; margin: 0; }

        /* ---------- CARD ---------- */
        .del-card { background: #ffffff; border-radius: 20px; border: 1px solid #f0c4c4; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 18px; }
        .del-card-header { display: flex; align-items: center; gap: 14px; padding: 20px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fefafa 100%); flex-wrap: wrap; }
        .del-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; }
        .del-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); }
        .del-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%); }
        .del-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .del-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .del-card-body { padding: 20px 24px; }

        /* ---------- STUDENT INFO TABLE ---------- */
        .del-info-table { width: 100%; border-collapse: separate; border-spacing: 0; border-radius: 16px; overflow: hidden; border: 1px solid #e8eef0; }
        .del-info-table tr { transition: background 0.15s ease; }
        .del-info-table tr:nth-child(even) { background: #fbfdfc; }
        .del-info-table tr:hover { background: #f0f8f3 !important; }
        .del-info-table tr:not(:last-child) td { border-bottom: 1px solid #f1f5f3; }
        .del-info-table td { padding: 14px 18px; font-size: 13px; vertical-align: middle; text-align: right; }
        .del-info-table td:first-child { width: 180px; }
        .del-detail-label { font-size: 11.5px; font-weight: 800; color: #6c757d; display: flex; align-items: center; gap: 8px; }
        .del-detail-label i { width: 26px; height: 26px; border-radius: 8px; background: linear-gradient(135deg, #f8f9fa 0%, #eef1f3 100%); color: #6c757d; display: inline-flex; align-items: center; justify-content: center; font-size: 11px; }
        .del-detail-value { font-size: 14px; font-weight: 700; color: #1f3d33; }

        .del-badge { display: inline-flex; align-items: center; gap: 5px; padding: 5px 12px; border-radius: 999px; font-size: 11.5px; font-weight: 800; }
        .del-badge.green { background: rgba(0,177,79,0.1); color: #007a33; border: 1px solid rgba(0,177,79,0.22); }
        .del-badge.blue { background: rgba(13,110,253,0.1); color: #0a58ca; border: 1px solid rgba(13,110,253,0.22); }

        /* ---------- ACTIONS ---------- */
        .del-actions { display: flex; gap: 12px; flex-wrap: wrap; padding-top: 20px; border-top: 1.5px dashed #f0c4c4; margin-top: 20px; }

        .del-btn { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 24px; border-radius: 12px; font-size: 13px; font-weight: 800; font-family: inherit; cursor: pointer; transition: all 0.2s ease; text-decoration: none; border: 1.5px solid transparent; white-space: nowrap; }
        .del-btn.danger { background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%); color: #fff; box-shadow: 0 6px 16px rgba(220,53,69,0.3); }
        .del-btn.danger:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(220,53,69,0.4); color: #fff; }
        .del-btn.secondary { background: #fff; color: #495057; border-color: #dee2e6; }
        .del-btn.secondary:hover { background: #f8f9fa; border-color: #adb5bd; color: #212529; }
        .del-btn.primary { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .del-btn.primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }

        /* ---------- EMPTY / NOT FOUND ---------- */
        .del-empty { text-align: center; padding: 60px 20px; }
        .del-empty-icon { width: 80px; height: 80px; border-radius: 22px; background: linear-gradient(135deg, #fff3cd 0%, #ffe8a1 100%); color: #b45309; display: inline-flex; align-items: center; justify-content: center; font-size: 36px; margin-bottom: 18px; }
        .del-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 8px; }
        .del-empty p { color: #6c757d; font-size: 13px; margin: 0 0 18px; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .del-hero-section { padding: 26px 20px 72px; }
            .del-hero-titles h1 { font-size: 19px; }
            .del-body { padding: 0 16px 40px; }
            .del-card-body { padding: 18px; }
        }
        @media (max-width: 576px) {
            .del-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .del-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .del-hero-titles h1 { font-size: 16px; }
            .del-btn-glass span { display: none; }
            .del-info-table td:first-child { width: 130px; }
            .del-actions { flex-direction: column; }
            .del-btn { width: 100%; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .del-hero-section, .del-btn-glass, .del-actions, .no-print { display: none !important; }
            .del-body { margin: 0 !important; padding: 0 !important; }
            .del-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="del-page" dir="rtl">

        <%-- ================== HERO ================== --%>
        <div class="del-hero-section">
            <div class="del-hero-content">
                <div class="del-hero-left">
                    <div class="del-hero-icon">
                        <i class="fas fa-trash-alt"></i>
                    </div>
                    <div class="del-hero-titles">
                        <h1><asp:Label ID="lblTitle" runat="server" Text="تعطيل حساب الطالب" /></h1>
                        <p>تأكيد العملية — راجع بيانات الطالب قبل المتابعة</p>
                        <span class="del-hero-chip">
                            <i class="fas fa-exclamation-triangle"></i>
                            <asp:Label ID="lblStageChip" runat="server" />
                        </span>
                    </div>
                </div>
              <%--  <a href="Default.aspx" class="del-btn-glass">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة</span>
                </a>--%>
            </div>
        </div>

        <%-- ================== BODY ================== --%>
        <div class="del-body">

            <%-- Alert --%>
            <asp:Label ID="lblMessage" runat="server" CssClass="d-none" />

            <asp:HiddenField ID="hfStudentId" runat="server" />
            <asp:HiddenField ID="hfStage" runat="server" />

            <%-- ================== NOT FOUND ================== --%>
            <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="del-card">
                <div class="del-card-body">
                    <div class="del-empty">
                        <div class="del-empty-icon">
                            <i class="fas fa-exclamation-triangle"></i>
                        </div>
                        <h4>الطالب غير موجود</h4>
                        <p>لم يتم العثور على الطالب المطلوب أو أنه لا ينتمي لهذه المرحلة.</p>
                        <asp:Button ID="btnBackToList" runat="server" Text="العودة للقائمة"
                            CssClass="del-btn primary" OnClick="btnBackToList_Click" />
                    </div>
                </div>
            </asp:Panel>

            <%-- ================== CONFIRM ================== --%>
            <asp:Panel ID="pnlConfirm" runat="server" Visible="true">

                <%-- Warning Banner --%>
                <div class="del-warning">
                    <div class="del-warning-icon">
                        <i class="fas fa-exclamation-triangle"></i>
                    </div>
                    <div class="del-warning-content">
                        <span class="del-warning-label">تنبيه مهم</span>
                        <span class="del-warning-title">
                            <asp:Label ID="lblWarningTitle" runat="server" Text="هل أنت متأكد من تعطيل حساب هذا الطالب؟" />
                        </span>
                        <p class="del-warning-text">
                            <asp:Label ID="lblWarning" runat="server"
                                Text="لن يتم حذفه نهائياً — يمكن إعادة تفعيله لاحقاً من قائمة الطلاب." />
                        </p>
                    </div>
                </div>

                <%-- Student Info Card --%>
                <div class="del-card">
                    <div class="del-card-header">
                        <div class="del-card-header-icon blue">
                            <i class="fas fa-user-graduate"></i>
                        </div>
                        <div class="del-card-header-text">
                            <h3>بيانات الطالب</h3>
                            <p>راجع البيانات التالية قبل تأكيد العملية</p>
                        </div>
                    </div>
                    <div class="del-card-body">
                        <table class="del-info-table">
                            <tr>
                                <td>
                                    <span class="del-detail-label">
                                        <i class="fas fa-user"></i>
                                        اسم الطالب
                                    </span>
                                </td>
                                <td>
                                    <span class="del-detail-value">
                                        <asp:Label ID="lblName" runat="server" />
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="del-detail-label">
                                        <i class="fas fa-id-card"></i>
                                        رقم الهوية
                                    </span>
                                </td>
                                <td>
                                    <span class="del-detail-value">
                                        <asp:Label ID="lblNationalId" runat="server" />
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="del-detail-label">
                                        <i class="fas fa-chalkboard"></i>
                                        الفصل الدراسي
                                    </span>
                                </td>
                                <td>
                                    <span class="del-detail-value">
                                        <asp:Label ID="lblClassRoom" runat="server" />
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="del-detail-label">
                                        <i class="fas fa-layer-group"></i>
                                        المستوى الدراسي
                                    </span>
                                </td>
                                <td>
                                    <span class="del-badge green">
                                        <i class="fas fa-graduation-cap" style="font-size:9px;"></i>
                                        <asp:Label ID="lblStage" runat="server" />
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="del-detail-label">
                                        <i class="fas fa-phone"></i>
                                        رقم الهاتف
                                    </span>
                                </td>
                                <td>
                                    <span class="del-detail-value">
                                        <asp:Label ID="lblPhone" runat="server" />
                                    </span>
                                </td>
                            </tr>
                        </table>

                        <%-- Action Buttons --%>
                        <div class="del-actions">
                            <asp:Button ID="btnConfirmDelete" runat="server"
                                Text="تعطيل الحساب"
                                CssClass="del-btn danger"
                                OnClick="btnConfirmDelete_Click" />

                            <asp:Button ID="btnCancel" runat="server"
                                Text="إلغاء والعودة"
                                CssClass="del-btn secondary"
                                OnClick="btnCancel_Click"
                                CausesValidation="false" />
                        </div>
                    </div>
                </div>

            </asp:Panel>

        </div>
    </div>
</asp:Content>