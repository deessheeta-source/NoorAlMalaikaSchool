<%@ Page Title="تعديل رسم إشتراك" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Fees.Edit" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Edit Fee
           ========================================================= */
        .fee-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .fee-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .fee-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .fee-hero::after {
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
        .fee-hero-content {
            position: relative;
            z-index: 2;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 24px;
            flex-wrap: wrap;
            max-width: 1280px;
            margin: 0 auto;
        }
        .fee-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .fee-hero-icon {
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
        .fee-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .fee-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .fee-stage-label {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            margin-inline-start: 8px;
            padding: 3px 12px;
            border-radius: 999px;
            background: rgba(255,255,255,0.15);
            border: 1px solid rgba(255,255,255,0.3);
            font-size: 12px;
            font-weight: 700;
            color: #fff;
        }
        .fee-stage-label i { font-size: 11px; }
        .fee-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .fee-btn-glass {
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
        .fee-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .fee-body {
            max-width: 1000px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .fee-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .fee-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .fee-card-header-icon {
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
        .fee-card-header-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .fee-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .fee-card-header-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .fee-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .fee-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .fee-card-body { padding: 20px 24px; }

        /* ---------- FIELDS ---------- */
        .fee-fields {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 16px;
        }
        .fee-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .fee-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .fee-field label i { color: #00b14f; font-size: 12px; }
        .fee-field label .req { color: #dc3545; }
        .fee-field .form-control,
        .fee-field .form-select {
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
        .fee-field textarea.form-control {
            height: auto;
            min-height: 90px;
            padding-top: 12px;
        }
        .fee-field .form-control:focus,
        .fee-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* Amount wrap */
        .fee-amount-wrap {
            display: flex;
        }
        .fee-amount-wrap .form-control {
            border-top-right-radius: 0;
            border-bottom-right-radius: 0;
            flex: 1;
        }
        .fee-amount-wrap .unit {
            display: inline-flex;
            align-items: center;
            padding: 0 16px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            font-weight: 800;
            font-size: 13px;
            border-radius: 11px 0 0 11px;
            border: 1.5px solid #00b14f;
            border-inline-start: 0;
        }

        /* ---------- BUTTONS ---------- */
        .fee-btn-primary {
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
        .fee-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }
        .fee-btn-outline {
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
        .fee-btn-outline:hover {
            border-color: #6c757d;
            background: #f8f9fa;
            color: #343a40;
            text-decoration: none;
        }
        .fee-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            flex-wrap: wrap;
            margin-bottom: 40px;
        }

        /* ---------- MESSAGE ---------- */
        .fee-message {
            display: block;
            padding: 14px 18px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 16px;
        }
        .fee-message.danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .fee-message.success {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .fee-message:empty { display: none; padding: 0; margin: 0; }

        /* ---------- NOT FOUND ---------- */
        .fee-notfound {
            text-align: center;
            padding: 60px 20px;
        }
        .fee-notfound-icon {
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
        .fee-notfound h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .fee-notfound p {
            color: #6c757d;
            font-size: 13px;
            margin: 0 0 18px;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .fee-hero { padding: 26px 20px 72px; }
            .fee-hero-titles h1 { font-size: 19px; }
            .fee-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .fee-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .fee-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .fee-hero-titles h1 { font-size: 16px; }
            .fee-card-body { padding: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .fee-hero-actions, .fee-actions, .no-print { display: none !important; }
            .fee-page { background: #fff !important; }
            .fee-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .fee-body { margin-top: 0 !important; padding: 0 !important; }
            .fee-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fee-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="fee-hero">
            <div class="fee-hero-content">
                <div class="fee-hero-left">
                    <div class="fee-hero-icon">
                        <i class="fas fa-edit"></i>
                    </div>
                    <div class="fee-hero-titles">
                        <h1>
                            تعديل رسم إشتراك
                            <span class="fee-stage-label">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:Literal ID="lblStage" runat="server" />
                            </span>
                        </h1>
                        <p>عدّل بيانات رسم الإشتراك ثم اضغط حفظ التعديلات</p>
                    </div>
                </div>
                <div class="fee-hero-actions no-print">
                    <a href="Default.aspx" class="fee-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="fee-body">

            <!-- Not Found -->
            <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
                <div class="fee-card">
                    <div class="fee-card-body">
                        <div class="fee-notfound">
                            <div class="fee-notfound-icon">
                                <i class="fas fa-exclamation-triangle"></i>
                            </div>
                            <h4>الرسوم غير موجودة</h4>
                            <p>ربما تم حذفها أو أن الرابط غير صالح</p>
                            <a href="Default.aspx" class="fee-btn-primary">
                                <i class="fas fa-arrow-right"></i> العودة
                            </a>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Form -->
            <asp:Panel ID="pnlForm" runat="server" Visible="false">

                <asp:Label ID="lblMessage" runat="server" CssClass="fee-message" />

                <!-- بيانات الرسم -->
                <div class="fee-card">
                    <div class="fee-card-header">
                        <div class="fee-card-header-icon blue">
                            <i class="fas fa-file-invoice-dollar"></i>
                        </div>
                        <div class="fee-card-header-text">
                            <h3>بيانات رسم الإشتراك</h3>
                            <p>الاسم، المبلغ، والنوع</p>
                        </div>
                    </div>
                    <div class="fee-card-body">
                        <div class="fee-fields">

                            <div class="fee-field">
                                <label><i class="fas fa-tag"></i> اسم رسم الإشتراك <span class="req">*</span></label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control"
                                    placeholder="مثال: رسوم الدراسة - رسوم رحلة مدرسية" />
                            </div>

                            <div class="fee-field">
                                <label><i class="fas fa-money-bill-wave"></i> المبلغ <span class="req">*</span></label>
                                <div class="fee-amount-wrap">
                                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"
                                        TextMode="Number" step="0.01" placeholder="0.00" />
                                    <span class="unit">د.ل</span>
                                </div>
                            </div>

                            <div class="fee-field">
                                <label><i class="fas fa-list"></i> نوع الرسم <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select" />
                            </div>

                        </div>
                    </div>
                </div>

                <!-- بيانات الدراسة -->
                <div class="fee-card">
                    <div class="fee-card-header">
                        <div class="fee-card-header-icon purple">
                            <i class="fas fa-graduation-cap"></i>
                        </div>
                        <div class="fee-card-header-text">
                            <h3>بيانات الدراسة</h3>
                            <p>السنة، الصف، والقسم</p>
                        </div>
                    </div>
                    <div class="fee-card-body">
                        <div class="fee-fields">

                            <div class="fee-field">
                                <label><i class="fas fa-calendar-alt"></i> السنة الدراسية</label>
                                <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select"
                                    AppendDataBoundItems="true" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged">
                                    <asp:ListItem Value="" Text="— اختر السنة الدراسية —" />
                                </asp:DropDownList>
                            </div>

                            <div class="fee-field">
                                <label><i class="fas fa-school"></i> الصف الدراسي</label>
                                <asp:DropDownList ID="ddlClassRoom" runat="server" CssClass="form-select"
                                    AppendDataBoundItems="true" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlClassRoom_SelectedIndexChanged">
                                    <asp:ListItem Value="" Text="— اختر الصف —" />
                                </asp:DropDownList>
                            </div>

                            <asp:Panel ID="pnlSection" runat="server" CssClass="fee-field" Visible="false">
                                <label><i class="fas fa-layer-group"></i> القسم</label>
                                <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="— اختر القسم —" Value="0" />
                                    <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                    <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                                </asp:DropDownList>
                            </asp:Panel>

                        </div>
                    </div>
                </div>

                <!-- ملاحظات -->
                <div class="fee-card">
                    <div class="fee-card-header">
                        <div class="fee-card-header-icon orange">
                            <i class="fas fa-sticky-note"></i>
                        </div>
                        <div class="fee-card-header-text">
                            <h3>ملاحظات</h3>
                            <p>أي ملاحظات إضافية (اختياري)</p>
                        </div>
                    </div>
                    <div class="fee-card-body">
                        <div class="fee-field">
                            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control"
                                TextMode="MultiLine" Rows="3"
                                placeholder="أدخل أي ملاحظات إضافية..." />
                        </div>
                    </div>
                </div>

                <!-- الأزرار -->
                <div class="fee-actions no-print">
                    <a href="Default.aspx" class="fee-btn-outline">
                        <i class="fas fa-times"></i> إلغاء
                    </a>
                    <asp:Button ID="btnUpdate" runat="server" Text="حفظ التعديلات"
                        CssClass="fee-btn-primary" OnClick="btnUpdate_Click" />
                </div>

            </asp:Panel>

        </div>
    </div>
</asp:Content>