<%@ Page Title="تعديل دفعة" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Payments.Edit" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Edit Payment
           ========================================================= */
        .ep-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .ep-hero {
            position: relative;
            background: linear-gradient(135deg, #0d6efd 0%, #0a58ca 50%, #084298 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .ep-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .ep-hero::after {
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
        .ep-hero-content {
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
        .ep-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .ep-hero-icon {
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
        .ep-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .ep-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .ep-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .ep-btn-glass {
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
        .ep-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .ep-body {
            max-width: 900px;
            margin: -56px auto 0;
            padding: 0 24px 40px;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .ep-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99c5f6;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .ep-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #f7fbff 100%);
            flex-wrap: wrap;
        }
        .ep-card-header-icon {
            width: 44px;
            height: 44px;
            border-radius: 13px;
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            flex-shrink: 0;
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .ep-card-header-icon.green {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
        }
        .ep-card-header-icon.orange {
            background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%);
            box-shadow: 0 6px 16px rgba(253,126,20,0.3);
        }
        .ep-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .ep-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .ep-card-body { padding: 22px 24px; }

        /* ---------- INFO BOX ---------- */
        .ep-info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
        }
        .ep-info-cell {
            padding: 14px 16px;
            border-radius: 12px;
            background: #f9fafb;
            border: 1.5px solid #eef1f3;
        }
        .ep-info-cell .lbl {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 11.5px;
            font-weight: 700;
            color: #6c757d;
            margin-bottom: 5px;
        }
        .ep-info-cell .lbl i { color: #0d6efd; font-size: 11px; }
        .ep-info-cell .val {
            font-size: 14px;
            font-weight: 800;
            color: #212529;
        }
        .ep-info-cell .val.mono {
            font-family: 'Courier New', monospace;
            letter-spacing: 0.5px;
        }
        .ep-info-cell .val.green { color: #00b14f; }
        .ep-info-cell .val.blue { color: #0d6efd; }

        /* ---------- FORM ---------- */
        .ep-fields {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 16px;
        }
        .ep-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .ep-field.full { grid-column: 1 / -1; }
        .ep-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .ep-field label i { color: #0d6efd; font-size: 12px; }
        .ep-field label .req { color: #dc3545; }
        .ep-field .form-control,
        .ep-field .form-select {
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
        .ep-field textarea.form-control {
            height: auto;
            min-height: 90px;
            padding-top: 12px;
        }
        .ep-field .form-control:focus,
        .ep-field .form-select:focus {
            border-color: #0d6efd;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(13,110,253,0.12);
            outline: none;
        }
        .ep-field small {
            font-size: 11px;
            color: #6c757d;
            margin-top: 2px;
        }

        /* ---------- BUTTONS ---------- */
        .ep-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 12px 32px;
            border-radius: 11px;
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            border: none;
            color: #fff;
            font-size: 14px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
            height: 46px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .ep-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(13,110,253,0.4);
            color: #fff;
        }
        .ep-btn-outline {
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
        .ep-btn-outline:hover {
            border-color: #6c757d;
            background: #f8f9fa;
            color: #343a40;
            text-decoration: none;
        }
        .ep-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            flex-wrap: wrap;
            margin-bottom: 40px;
        }

        /* ---------- MESSAGE ---------- */
        .ep-message {
            display: block;
            padding: 14px 18px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 16px;
        }
        .ep-message.danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .ep-message.success {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .ep-message:empty { display: none; padding: 0; margin: 0; }

        @media (max-width: 992px) {
            .ep-hero { padding: 26px 20px 72px; }
            .ep-hero-titles h1 { font-size: 19px; }
            .ep-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .ep-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .ep-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .ep-hero-titles h1 { font-size: 16px; }
            .ep-card-body { padding: 16px; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ep-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="ep-hero">
            <div class="ep-hero-content">
                <div class="ep-hero-left">
                    <div class="ep-hero-icon">
                        <i class="fas fa-edit"></i>
                    </div>
                    <div class="ep-hero-titles">
                        <h1>تعديل الدفعة</h1>
                        <p>عدّل بيانات الدفعة ثم اضغط "حفظ التعديلات"</p>
                    </div>
                </div>
                <div class="ep-hero-actions">
                    <a href="<%= PageReturnUrl %>" class="ep-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="ep-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="ep-message" />

            <!-- بيانات الدفعة الأصلية (مرجعية) -->
            <asp:Panel ID="pnlSummary" runat="server" Visible="false">
                <div class="ep-card">
                    <div class="ep-card-header">
                        <div class="ep-card-header-icon green">
                            <i class="fas fa-info-circle"></i>
                        </div>
                        <div class="ep-card-header-text">
                            <h3>بيانات الدفعة الحالية</h3>
                            <p>معلومات مرجعية عن الدفعة قبل التعديل</p>
                        </div>
                    </div>
                    <div class="ep-card-body">
                        <div class="ep-info-grid">
                            <div class="ep-info-cell">
                                <div class="lbl"><i class="fas fa-receipt"></i> رقم الإيصال</div>
                                <div class="val mono"><asp:Literal ID="lblReceiptNumber" runat="server" /></div>
                            </div>
                            <div class="ep-info-cell">
                                <div class="lbl"><i class="fas fa-user-graduate"></i> الطالب</div>
                                <div class="val"><asp:Literal ID="lblStudentName" runat="server" /></div>
                            </div>
                            <div class="ep-info-cell">
                                <div class="lbl"><i class="fas fa-file-invoice-dollar"></i> نوع الرسم</div>
                                <div class="val"><asp:Literal ID="lblFeeName" runat="server" /></div>
                            </div>
                            <div class="ep-info-cell">
                                <div class="lbl"><i class="fas fa-coins"></i> المبلغ الحالي</div>
                                <div class="val green"><asp:Literal ID="lblCurrentAmount" runat="server" /> د.ل</div>
                            </div>
                            <div class="ep-info-cell">
                                <div class="lbl"><i class="fas fa-calendar-alt"></i> التاريخ الحالي</div>
                                <div class="val mono"><asp:Literal ID="lblCurrentDate" runat="server" /></div>
                            </div>
                            <div class="ep-info-cell">
                                <div class="lbl"><i class="fas fa-credit-card"></i> طريقة الدفع الحالية</div>
                                <div class="val"><asp:Literal ID="lblCurrentMethod" runat="server" /></div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- نموذج التعديل -->
            <asp:Panel ID="pnlEdit" runat="server" Visible="false">
                <div class="ep-card">
                    <div class="ep-card-header">
                        <div class="ep-card-header-icon">
                            <i class="fas fa-edit"></i>
                        </div>
                        <div class="ep-card-header-text">
                            <h3>تعديل البيانات</h3>
                            <p>قم بتحديث الحقول المطلوبة ثم اضغط "حفظ التعديلات"</p>
                        </div>
                    </div>
                    <div class="ep-card-body">
                        <div class="ep-fields">

                            <div class="ep-field">
                                <label><i class="fas fa-coins"></i> المبلغ <span class="req">*</span></label>
                                <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"
                                    TextMode="Number" step="0.01" min="0.01" placeholder="0.00" />
                                <small><i class="fas fa-info-circle"></i>
                                    الحد الأقصى: <asp:Literal ID="lblMaxAmount" runat="server" Text="0.00" /> د.ل
                                </small>
                            </div>

                            <div class="ep-field">
                                <label><i class="fas fa-credit-card"></i> طريقة الدفع <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="Cash" Text="نقدي" />
                                    <asp:ListItem Value="BankTransfer" Text="تحويل بنكي" />
                                    <asp:ListItem Value="Check" Text="شيك" />
                                    <asp:ListItem Value="Electronic" Text="بطاقة مصرفية" />
                                </asp:DropDownList>
                            </div>

                            <div class="ep-field">
                                <label><i class="fas fa-calendar-alt"></i> تاريخ الدفع <span class="req">*</span></label>
                                <asp:TextBox ID="txtPaymentDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>

                            <div class="ep-field">
                                <label><i class="fas fa-receipt"></i> رقم الإيصال</label>
                                <asp:TextBox ID="txtReceiptNumber" runat="server" CssClass="form-control" MaxLength="50" />
                                <small><i class="fas fa-info-circle"></i> اترك الحقل فارغاً للإبقاء على الرقم الحالي</small>
                            </div>

                            <div class="ep-field full">
                                <label><i class="fas fa-sticky-note"></i> ملاحظات</label>
                                <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control"
                                    TextMode="MultiLine" Rows="3"
                                    placeholder="ملاحظات إضافية (اختياري)..." />
                            </div>

                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- الأزرار -->
            <div class="ep-actions">
                <a href="<%= PageReturnUrl %>" class="ep-btn-outline">
                    <i class="fas fa-times"></i> إلغاء
                </a>
                <asp:Button ID="btnSave" runat="server" Text="حفظ التعديلات"
                    CssClass="ep-btn-primary" OnClick="btnSave_Click" Visible="false" />
            </div>

        </div>
    </div>
</asp:Content>
