<%@ Page Title="إضافة مادة دراسية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Subjects.Create" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Add Subject
           ========================================================= */
        .sb-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .sb-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
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

        /* ---------- BODY ---------- */
        .sb-body { max-width: 900px; margin: -56px auto 0;padding: 0 24px 0; position: relative; z-index: 3; }

        /* ---------- CARD ---------- */
        .sb-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .sb-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .sb-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .sb-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .sb-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .sb-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .sb-card-body { padding: 20px 24px; }

        /* ---------- FIELDS ---------- */
        .sb-fields { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 16px; }
        .sb-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .sb-field.full { grid-column: 1 / -1; }
        .sb-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .sb-field label i { color: #00b14f; font-size: 12px; }
        .sb-field label .req { color: #dc3545; }
        .sb-field .form-control,
        .sb-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .sb-field textarea.form-control { height: auto; min-height: 90px; padding-top: 12px; }
        .sb-field .form-control:focus,
        .sb-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }
        .sb-field .text-danger { font-size: 11.5px; font-weight: 700; margin-top: 2px; }

        /* ---------- BUTTONS ---------- */
        .sb-btn-primary { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 32px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); border: none; color: #fff; font-size: 14px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(0,177,79,0.3); height: 46px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .sb-btn-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }
        .sb-btn-outline { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 28px; border-radius: 11px; background: #fff; border: 1.5px solid #dee2e6; color: #495057; font-size: 14px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 46px; text-decoration: none; font-family: 'Droid Arabic Kufi', sans-serif; }
        .sb-btn-outline:hover { border-color: #6c757d; background: #f8f9fa; color: #343a40; text-decoration: none; }
        .sb-actions { display: flex; justify-content: flex-end; gap: 10px; flex-wrap: wrap; margin-bottom: 40px; }

        /* ---------- MESSAGE ---------- */
        .sb-message { display: block; padding: 14px 18px; border-radius: 12px; font-size: 13px; font-weight: 700; margin-bottom: 16px; }
        .sb-message.danger { background: rgba(220,53,69,0.08); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }
        .sb-message.success { background: rgba(0,177,79,0.08); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .sb-message.d-none { display: none !important; }

        /* ---------- RESPONSIVE ---------- */
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

        /* ---------- PRINT ---------- */
        @media print {
            .sb-hero-actions, .sb-actions, .no-print { display: none !important; }
            .sb-page { background: #fff !important; }
            .sb-hero { background: #00923f !important; -webkit-print-color-adjust: exact; print-color-adjust: exact; border-radius: 0 !important; padding: 16px 20px !important; }
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
                        <i class="fas fa-plus-circle"></i>
                    </div>
                    <div class="sb-hero-titles">
                        <h1>إضافة مادة دراسية</h1>
                        <p>أدخل بيانات المادة ثم اضغط حفظ</p>
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

            <!-- بيانات المادة -->
            <div class="sb-card">
                <div class="sb-card-header">
                    <div class="sb-card-header-icon blue">
                        <i class="fas fa-book"></i>
                    </div>
                    <div class="sb-card-header-text">
                        <h3>بيانات المادة</h3>
                        <p>الاسم، الرمز، والمرحلة الدراسية</p>
                    </div>
                </div>
                <div class="sb-card-body">
                    <div class="sb-fields">

                        <div class="sb-field">
                            <label><i class="fas fa-tag"></i> اسم المادة <span class="req">*</span></label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"
                                placeholder="مثال: الرياضيات" />
                            <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                ControlToValidate="txtName"
                                ErrorMessage="اسم المادة مطلوب"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="vgSave" />
                        </div>

                        <div class="sb-field">
                            <label><i class="fas fa-hashtag"></i> الرمز</label>
                            <asp:TextBox ID="txtCode" runat="server" CssClass="form-control"
                                MaxLength="20" placeholder="مثال: MATH" />
                        </div>

                        <div class="sb-field">
                            <label><i class="fas fa-graduation-cap"></i> المرحلة الدراسية <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlStage" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlStage_SelectedIndexChanged">
                                <asp:ListItem Text="أساسي" Value="Basic" />
                                <asp:ListItem Text="ثانوي" Value="Secondary" />
                            </asp:DropDownList>
                        </div>

                        <div class="sb-field">
                            <label><i class="fas fa-layer-group"></i> الصف / الفرع <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                                <asp:ListItem Text="عام" Value="None" />
                                <asp:ListItem Text="أول ثانوي" Value="FirstSecondary" />
                                <asp:ListItem Text="ثاني وثالث ثانوي علمي" Value="Scientific" />
                                <asp:ListItem Text="ثاني وثالث ثانوي أدبي" Value="Literary" />
                            </asp:DropDownList>
                        </div>

                        <div class="sb-field full">
                            <label><i class="fas fa-align-right"></i> الوصف</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"
                                TextMode="MultiLine" Rows="3" MaxLength="500"
                                placeholder="أدخل وصفاً اختيارياً للمادة" />
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
                <asp:Button ID="btnSave" runat="server" Text="حفظ"
                    CssClass="sb-btn-primary"
                    ValidationGroup="vgSave"
                    OnClick="btnSave_Click" />
            </div>

        </div>
    </div>
</asp:Content>