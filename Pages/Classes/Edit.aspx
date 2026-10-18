<%@ Page Title="تعديل فصل" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Classes.Edit" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Edit Class
           ========================================================= */
        .cls-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .cls-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .cls-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .cls-hero::after {
            content: '';
            position: absolute;
            top: -40%;
            left: -10%;
            width: 340px;
            height: 340px;
            border-radius: 50%;
            background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%);
            pointer-events: none;
        }
        .cls-hero-content {
            position: relative;
            z-index: 2;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 24px;
            flex-wrap: wrap;
            max-width: 900px;
            margin: 0 auto;
        }
        .cls-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .cls-hero-icon {
            width: 68px;
            height: 68px;
            border-radius: 20px;
            background: rgba(255,255,255,0.18);
            backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,0.3);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 30px;
            flex-shrink: 0;
            box-shadow: 0 8px 24px rgba(0,0,0,0.12);
        }
        .cls-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .cls-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .cls-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .cls-btn-glass {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 10px 20px;
            border-radius: 12px;
            background: rgba(255,255,255,0.15);
            backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,0.3);
            color: #fff;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.2s ease;
            cursor: pointer;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .cls-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .cls-body {
            max-width: 900px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .cls-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .cls-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .cls-card-header-icon {
            width: 44px;
            height: 44px;
            border-radius: 13px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            flex-shrink: 0;
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
        }
        .cls-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .cls-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .cls-card-body { padding: 20px 24px; }

        /* ---------- FIELDS ---------- */
        .cls-fields {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 16px;
        }
        .cls-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .cls-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .cls-field label i { color: #00b14f; font-size: 12px; }
        .cls-field label .req { color: #dc3545; }
        .cls-field .form-control,
        .cls-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 44px;
            width: 100%;
        }
        .cls-field .form-control:focus,
        .cls-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- BUTTONS ---------- */
        .cls-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 12px 32px;
            border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            color: #fff;
            font-size: 14px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
            height: 46px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .cls-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }
        .cls-btn-outline {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 12px 28px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #dee2e6;
            color: #495057;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            height: 46px;
            text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .cls-btn-outline:hover {
            border-color: #6c757d;
            background: #f8f9fa;
            color: #343a40;
            text-decoration: none;
        }
        .cls-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            flex-wrap: wrap;
            margin-bottom: 40px;
        }

        /* ---------- MESSAGE ---------- */
        .cls-message {
            display: block;
            padding: 14px 18px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 16px;
        }
        .cls-message.danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .cls-message.success {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .cls-message.d-none {
            display: none !important;
        }

        /* ---------- NOT FOUND ---------- */
        .cls-notfound {
            text-align: center;
            padding: 60px 20px;
        }
        .cls-notfound-icon {
            width: 80px;
            height: 80px;
            border-radius: 22px;
            background: linear-gradient(135deg, #fff4e5 0%, #ffe6c7 100%);
            color: #fd7e14;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            margin-bottom: 18px;
        }
        .cls-notfound h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .cls-notfound p {
            color: #6c757d;
            font-size: 13px;
            margin: 0 0 18px;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .cls-hero { padding: 26px 20px 72px; }
            .cls-hero-titles h1 { font-size: 19px; }
            .cls-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .cls-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .cls-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .cls-hero-titles h1 { font-size: 16px; }
            .cls-card-body { padding: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .cls-hero-actions, .cls-actions, .no-print { display: none !important; }
            .cls-page { background: #fff !important; }
            .cls-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .cls-body { margin-top: 0 !important; padding: 0 !important; }
            .cls-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cls-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="cls-hero">
            <div class="cls-hero-content">
                <div class="cls-hero-left">
                    <div class="cls-hero-icon">
                        <i class="fas fa-edit"></i>
                    </div>
                    <div class="cls-hero-titles">
                        <h1>تعديل الفصل</h1>
                        <p>عدّل بيانات الفصل ثم اضغط حفظ التعديلات</p>
                    </div>
                </div>
                <div class="cls-hero-actions no-print">
                    <a href="Default.aspx" class="cls-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="cls-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="cls-message d-none" />
            <asp:HiddenField ID="hfStage" runat="server" />
            <asp:HiddenField ID="hfClassId" runat="server" />

            <!-- Not Found -->
            <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
                <div class="cls-card">
                    <div class="cls-card-body">
                        <div class="cls-notfound">
                            <div class="cls-notfound-icon">
                                <i class="fas fa-exclamation-triangle"></i>
                            </div>
                            <h4>الفصل غير موجود</h4>
                            <p>ربما تم حذفه أو أن الرابط غير صالح</p>
                            <a href="Default.aspx" class="cls-btn-primary">
                                <i class="fas fa-arrow-right"></i> العودة
                            </a>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Form -->
            <asp:Panel ID="pnlForm" runat="server" Visible="true">

                <div class="cls-card">
                    <div class="cls-card-header">
                        <div class="cls-card-header-icon">
                            <i class="fas fa-chalkboard"></i>
                        </div>
                        <div class="cls-card-header-text">
                            <h3>بيانات الفصل</h3>
                            <p>السنة الدراسية، الصف، القسم، والشعبة</p>
                        </div>
                    </div>
                    <div class="cls-card-body">

                        <div class="cls-fields">

                            <div class="cls-field">
                                <label><i class="fas fa-calendar-alt"></i> السنة الدراسية <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select" />
                            </div>

                            <div class="cls-field">
                                <label><i class="fas fa-graduation-cap"></i> الصف الدراسي <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged" />
                            </div>

                            <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="cls-field" Visible="false">
                                <label><i class="fas fa-stream"></i> القسم</label>
                                <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="بدون قسم" Value="" Selected="True" />
                                    <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                    <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                                </asp:DropDownList>
                            </asp:Panel>

                            <div class="cls-field">
                                <label><i class="fas fa-chair"></i> الشعبة</label>
                                <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="بدون شعبة" Value="" Selected="True" />
                                    <asp:ListItem Text="شعبة أ" Value="أ" />
                                    <asp:ListItem Text="شعبة ب" Value="ب" />
                                    <asp:ListItem Text="شعبة ج" Value="ج" />
                                    <asp:ListItem Text="شعبة د" Value="د" />
                                </asp:DropDownList>
                            </div>

                        </div>

                    </div>
                </div>

                <!-- الأزرار -->
                <div class="cls-actions no-print">
                    <asp:Button ID="btnCancel" runat="server" Text="إلغاء"
                        CssClass="cls-btn-outline"
                        OnClick="btnCancel_Click"
                        CausesValidation="false" />
                    <asp:Button ID="btnSave" runat="server" Text="حفظ التعديلات"
                        CssClass="cls-btn-primary"
                        OnClick="btnSave_Click" />
                </div>

            </asp:Panel>

        </div>
    </div>
</asp:Content>