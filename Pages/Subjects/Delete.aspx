<%@ Page Title="حذف مادة دراسية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Subjects.Delete" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Delete Subject
           ========================================================= */
        .sb-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        .sb-hero { position: relative; background: linear-gradient(135deg, #dc3545 0%, #a51c1c 50%, #7a1010 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .sb-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .sb-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .sb-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 900px; margin: 0 auto; }
        .sb-hero-left { display: flex; align-items: center; gap: 20px; }
        .sb-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .sb-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .sb-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }
        .sb-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .sb-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .sb-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }

        .sb-body { max-width: 900px; margin: -56px auto 0;padding: 0 24px 0; position: relative; z-index: 3; }

        .sb-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .sb-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .sb-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(220,53,69,0.3); }
        .sb-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .sb-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .sb-card-body { padding: 20px 24px; }

        .sb-warning { display: flex; align-items: center; gap: 12px; padding: 16px 20px; border-radius: 14px; background: linear-gradient(135deg, rgba(220,53,69,0.08) 0%, rgba(220,53,69,0.03) 100%); border: 1.5px solid rgba(220,53,69,0.25); color: #a51c1c; font-size: 14px; font-weight: 700; margin-bottom: 20px; }
        .sb-warning-icon { width: 44px; height: 44px; border-radius: 12px; background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 20px; flex-shrink: 0; box-shadow: 0 4px 12px rgba(220,53,69,0.35); }
        .sb-warning-text { display: flex; flex-direction: column; gap: 2px; }
        .sb-warning-text strong { font-size: 14px; color: #a51c1c; }
        .sb-warning-text span { font-size: 12.5px; color: #6c757d; font-weight: 600; }

        .sb-info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 14px; margin-bottom: 20px; }
        .sb-info-item { display: flex; align-items: center; gap: 12px; padding: 14px 16px; border-radius: 14px; background: #f9fafb; border: 1px solid #99f6c3; transition: all 0.2s ease; }
        .sb-info-item:hover { border-color: #d8e6dd; background: #f8fdfa; }
        .sb-info-item.full { grid-column: 1 / -1; }
        .sb-info-icon { width: 40px; height: 40px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 16px; flex-shrink: 0; box-shadow: 0 4px 12px rgba(0,177,79,0.3); }
        .sb-info-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 4px 12px rgba(13,110,253,0.3); }
        .sb-info-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 4px 12px rgba(111,66,193,0.3); }
        .sb-info-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 4px 12px rgba(253,126,20,0.3); }
        .sb-info-content { display: flex; flex-direction: column; gap: 2px; min-width: 0; flex: 1; }
        .sb-info-label { font-size: 11.5px; font-weight: 700; color: #6c757d; letter-spacing: 0.2px; display: flex; align-items: center; gap: 4px; }
        .sb-info-value { font-size: 13.5px; font-weight: 800; color: #212529; word-wrap: break-word; overflow-wrap: break-word; }

        .sb-btn-danger { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 32px; border-radius: 11px; background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); border: none; color: #fff; font-size: 14px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(220,53,69,0.3); height: 46px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .sb-btn-danger:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(220,53,69,0.4); color: #fff; }
        .sb-btn-outline { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 28px; border-radius: 11px; background: #fff; border: 1.5px solid #dee2e6; color: #495057; font-size: 14px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 46px; text-decoration: none; font-family: 'Droid Arabic Kufi', sans-serif; }
        .sb-btn-outline:hover { border-color: #6c757d; background: #f8f9fa; color: #343a40; text-decoration: none; }
        .sb-actions { display: flex; justify-content: flex-end; gap: 10px; flex-wrap: wrap; margin-bottom: 40px; }

        .sb-message { display: block; padding: 14px 18px; border-radius: 12px; font-size: 13px; font-weight: 700; margin-bottom: 16px; }
        .sb-message.danger { background: rgba(220,53,69,0.08); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }
        .sb-message.success { background: rgba(0,177,79,0.08); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .sb-message.d-none { display: none !important; }

        .sb-notfound { text-align: center; padding: 60px 20px; }
        .sb-notfound-icon { width: 80px; height: 80px; border-radius: 22px; background: linear-gradient(135deg, #fff4e5 0%, #ffe6c7 100%); color: #fd7e14; display: inline-flex; align-items: center; justify-content: center; font-size: 36px; margin-bottom: 18px; }
        .sb-notfound h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 8px; }
        .sb-notfound p { color: #6c757d; font-size: 13px; margin: 0 0 18px; }

        @media (max-width: 992px) {
            .sb-hero { padding: 26px 20px 72px; }
            .sb-hero-titles h1 { font-size: 19px; }
            .sb-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .sb-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .sb-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .sb-hero-titles h1 { font-size: 16px; }
            .sb-card-body { padding: 16px; }
        }

        @media print {
            .sb-hero-actions, .sb-actions, .no-print { display: none !important; }
            .sb-page { background: #fff !important; }
            .sb-hero { background: #dc3545 !important; -webkit-print-color-adjust: exact; print-color-adjust: exact; border-radius: 0 !important; padding: 16px 20px !important; }
            .sb-body { margin-top: 0 !important; padding: 0 !important; }
            .sb-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="sb-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="sb-hero">
            <div class="sb-hero-content">
                <div class="sb-hero-left">
                    <div class="sb-hero-icon">
                        <i class="fas fa-trash"></i>
                    </div>
                    <div class="sb-hero-titles">
                        <h1>حذف مادة دراسية</h1>
                        <p>يرجى تأكيد حذف المادة قبل المتابعة</p>
                    </div>
                </div>
                <div class="sb-hero-actions no-print">
                    <a href="Default.aspx" class="sb-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="sb-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="sb-message d-none" />
            <asp:HiddenField ID="hfSubjectId" runat="server" />

            <!-- Not Found -->
            <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
                <div class="sb-card">
                    <div class="sb-card-body">
                        <div class="sb-notfound">
                            <div class="sb-notfound-icon">
                                <i class="fas fa-exclamation-triangle"></i>
                            </div>
                            <h4>المادة غير موجودة</h4>
                            <p>ربما تم حذفها مسبقاً أو أن الرابط غير صالح</p>
                            <asp:Button ID="btnBackToList" runat="server" Text="العودة للقائمة"
                                CssClass="sb-btn-primary"
                                OnClick="btnCancel_Click"
                                CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Confirm Delete -->
            <asp:Panel ID="pnlConfirm" runat="server" Visible="true">

                <div class="sb-card">
                    <div class="sb-card-header">
                        <div class="sb-card-header-icon">
                            <i class="fas fa-exclamation-triangle"></i>
                        </div>
                        <div class="sb-card-header-text">
                            <h3>تأكيد الحذف</h3>
                            <p>يرجى مراجعة بيانات المادة قبل الحذف</p>
                        </div>
                    </div>
                    <div class="sb-card-body">

                        <!-- Warning Alert -->
                        <div class="sb-warning">
                            <div class="sb-warning-icon">
                                <i class="fas fa-exclamation-triangle"></i>
                            </div>
                            <div class="sb-warning-text">
                                <strong>هل أنت متأكد من حذف هذه المادة الدراسية؟</strong>
                                <span>لا يمكن التراجع عن هذه العملية بعد التأكيد</span>
                            </div>
                        </div>

                        <!-- Info Grid -->
                        <div class="sb-info-grid">

                            <div class="sb-info-item">
                                <div class="sb-info-icon blue">
                                    <i class="fas fa-book"></i>
                                </div>
                                <div class="sb-info-content">
                                    <span class="sb-info-label">اسم المادة</span>
                                    <span class="sb-info-value">
                                        <asp:Label ID="lblName" runat="server" />
                                    </span>
                                </div>
                            </div>

                            <div class="sb-info-item">
                                <div class="sb-info-icon purple">
                                    <i class="fas fa-hashtag"></i>
                                </div>
                                <div class="sb-info-content">
                                    <span class="sb-info-label">الرمز</span>
                                    <span class="sb-info-value">
                                        <asp:Label ID="lblCode" runat="server" />
                                    </span>
                                </div>
                            </div>

                            <div class="sb-info-item full">
                                <div class="sb-info-icon orange">
                                    <i class="fas fa-align-right"></i>
                                </div>
                                <div class="sb-info-content">
                                    <span class="sb-info-label">الوصف</span>
                                    <span class="sb-info-value">
                                        <asp:Label ID="lblDescription" runat="server" />
                                    </span>
                                </div>
                            </div>

                        </div>

                    </div>
                </div>

                <!-- الأزرار -->
                <div class="sb-actions no-print">
                    <asp:Button ID="btnCancel" runat="server" Text="إلغاء"
                        CssClass="sb-btn-outline"
                        OnClick="btnCancel_Click"
                        CausesValidation="false" />
                    <asp:Button ID="btnConfirmDelete" runat="server" Text="تأكيد الحذف"
                        CssClass="sb-btn-danger"
                        OnClick="btnConfirmDelete_Click" />
                </div>

            </asp:Panel>

        </div>
    </div>
</asp:Content>