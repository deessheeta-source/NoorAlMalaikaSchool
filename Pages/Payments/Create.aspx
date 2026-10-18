<%@ Page Title="إضافة دفعة اشتراك" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Payments.Create" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Payment Create
           ========================================================= */
        .pay-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .pay-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .pay-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .pay-hero::after {
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
        .pay-hero-content {
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
        .pay-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .pay-hero-icon {
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
        .pay-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .pay-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .pay-stage-label {
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
        .pay-stage-label i { font-size: 11px; }
        .pay-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .pay-btn-glass {
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
        }
        .pay-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .pay-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .pay-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .pay-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .pay-card-header-icon {
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
        .pay-card-header-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .pay-card-header-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .pay-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .pay-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }
        .pay-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .pay-card-body {
            padding: 20px 24px;
        }

        /* ---------- FIELDS ---------- */
        .pay-filters {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(190px, 1fr));
            gap: 12px;
            align-items: end;
        }
        .pay-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .pay-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .pay-field label i { color: #00b14f; font-size: 12px; }
        .pay-field label .req { color: #dc3545; }
        .pay-field .form-control,
        .pay-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 12.5px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px;
            width: 100%;
        }
        .pay-field textarea.form-control {
            height: auto;
            min-height: 80px;
        }
        .pay-field .form-control:focus,
        .pay-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }
        .pay-field small {
            font-size: 11px;
            color: #6c757d;
        }

        /* Amount input with unit */
        .pay-amount-wrap {
            display: flex;
            gap: 0;
        }
        .pay-amount-wrap .form-control {
            border-top-right-radius: 0;
            border-bottom-right-radius: 0;
            flex: 1;
        }
        .pay-amount-wrap .unit {
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
        .pay-btn-primary {
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
        .pay-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }
        .pay-btn-outline {
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
        .pay-btn-outline:hover {
            border-color: #6c757d;
            background: #f8f9fa;
            color: #343a40;
            text-decoration: none;
        }
        .pay-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            flex-wrap: wrap;
        }

        /* Message */
        .pay-message {
            display: block;
            padding: 12px 16px;
            border-radius: 11px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 16px;
        }
        .pay-message.success {
            background: rgba(0,177,79,0.08);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .pay-message.danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .pay-message:empty { display: none; padding: 0; margin: 0; }

        /* Responsive */
        @media (max-width: 992px) {
            .pay-hero { padding: 26px 20px 72px; }
            .pay-hero-titles h1 { font-size: 19px; }
            .pay-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .pay-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .pay-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .pay-hero-titles h1 { font-size: 16px; }
            .pay-card-body { padding: 16px; }
        }

        /* Print */
        @media print {
            .pay-hero-actions, .pay-actions, .no-print { display: none !important; }
            .pay-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .pay-body { margin-top: 0 !important; padding: 0 !important; }
            .pay-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pay-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="pay-hero">
            <div class="pay-hero-content">
                <div class="pay-hero-left">
                    <div class="pay-hero-icon">
                        <i class="fas fa-hand-holding-usd"></i>
                    </div>
                    <div class="pay-hero-titles">
                        <h1>
                            إضافة دفعة اشتراك
                            <span class="pay-stage-label">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:Literal ID="lblStage" runat="server" />
                            </span>
                        </h1>
                        <p>اختر الطالب، الرسم، والمبلغ ثم اضغط حفظ لإتمام العملية</p>
                    </div>
                </div>
                <div class="pay-hero-actions no-print">
                    <a href="<%= ReturnUrl %>" class="pay-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="pay-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="pay-message" />

            <!-- بيانات الطالب -->
            <div class="pay-card">
                <div class="pay-card-header">
                    <div class="pay-card-header-icon blue">
                        <i class="fas fa-user-graduate"></i>
                    </div>
                    <div class="pay-card-header-text">
                        <h3>بيانات الطالب</h3>
                        <p>اختر السنة، الصف، القسم، الشعبة ثم الطالب</p>
                    </div>
                </div>
                <div class="pay-card-body">
                    <div class="pay-filters">

                        <div class="pay-field">
                            <label><i class="fas fa-calendar-alt"></i> السنة الدراسية</label>
                            <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="— اختر السنة —" />
                            </asp:DropDownList>
                        </div>

                        <div class="pay-field">
                            <label><i class="fas fa-school"></i> الصف الدراسي</label>
                            <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlClass_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="— اختر الصف —" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSection" runat="server" CssClass="pay-field" Visible="false">
                            <label><i class="fas fa-layer-group"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الأقسام" Value="" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="pay-field">
                            <label><i class="fas fa-chair"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الشعب" Value="" Selected="True" />
                            </asp:DropDownList>
                        </div>

                        <div class="pay-field">
                            <label><i class="fas fa-user"></i> اسم الطالب</label>
                            <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="— اختر الطالب —" />
                            </asp:DropDownList>
                        </div>

                    </div>
                </div>
            </div>

            <!-- بيانات الرسم والمبلغ -->
            <div class="pay-card">
                <div class="pay-card-header">
                    <div class="pay-card-header-icon">
                        <i class="fas fa-file-invoice-dollar"></i>
                    </div>
                    <div class="pay-card-header-text">
                        <h3>بيانات الرسم والمبلغ</h3>
                        <p>اختر نوع الرسم ثم سيُحسب المتبقي تلقائياً</p>
                    </div>
                </div>
                <div class="pay-card-body">
                    <div class="pay-filters">

                        <div class="pay-field" style="grid-column: span 2;">
                            <label><i class="fas fa-receipt"></i> نوع رسم الاشتراك <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlFee" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlFee_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="— اختر الطالب أولاً —" />
                            </asp:DropDownList>
                        </div>

                        <div class="pay-field" style="grid-column: span 2;">
                            <label><i class="fas fa-money-bill-wave"></i> المبلغ <span class="req">*</span></label>
                            <div class="pay-amount-wrap">
                                <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"
                                    TextMode="Number" step="0.01" min="0" placeholder="0.00" />
                                <span class="unit">د.ل</span>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <!-- بيانات الدفع -->
            <div class="pay-card">
                <div class="pay-card-header">
                    <div class="pay-card-header-icon orange">
                        <i class="fas fa-credit-card"></i>
                    </div>
                    <div class="pay-card-header-text">
                        <h3>بيانات الدفع</h3>
                        <p>رقم الإيصال، تاريخ الدفع، وطريقة الدفع</p>
                    </div>
                </div>
                <div class="pay-card-body">
                    <div class="pay-filters">

                        <div class="pay-field">
                            <label><i class="fas fa-hashtag"></i> رقم الإيصال</label>
                            <asp:TextBox ID="txtReceiptNumber" runat="server" CssClass="form-control"
                                ReadOnly="true" BackColor="#f8f9fa" />
                            <small><i class="fas fa-magic"></i> يتم توليد الرقم تلقائياً</small>
                        </div>

                        <%-- ✅ تاريخ الدفع: مع خاصية onchange للتحقق الفوري --%>
                        <div class="pay-field">
                            <label><i class="fas fa-calendar-alt"></i> تاريخ الدفع</label>
                            <asp:TextBox ID="txtPaymentDate" runat="server" CssClass="form-control"
                                TextMode="Date" onchange="updateMinDateInfo();" />
                            <small id="minDateHint" style="color:#dc3545;font-weight:600;"></small>
                        </div>

                        <div class="pay-field">
                            <label><i class="fas fa-credit-card"></i> طريقة الدفع</label>
                            <asp:DropDownList ID="ddlMethod" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true">
                                <asp:ListItem Value="Cash" Text="نقدي" />
                                <asp:ListItem Value="BankTransfer" Text="تحويل بنكي" />
                                <asp:ListItem Value="Check" Text="شيك" />
                                <asp:ListItem Value="Electronic" Text="بطاقة مصرفية" />
                            </asp:DropDownList>
                        </div>

                    </div>
                </div>
            </div>

            <!-- ملاحظات -->
            <div class="pay-card">
                <div class="pay-card-header">
                    <div class="pay-card-header-icon purple">
                        <i class="fas fa-sticky-note"></i>
                    </div>
                    <div class="pay-card-header-text">
                        <h3>ملاحظات</h3>
                        <p>أي ملاحظات إضافية حول الدفعة (اختياري)</p>
                    </div>
                </div>
                <div class="pay-card-body">
                    <div class="pay-field">
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="3"
                            placeholder="أدخل أي ملاحظات إضافية..." />
                    </div>
                </div>
            </div>

            <%-- ✅ حقول مخفية لتخزين معلومات الفاتورة المختارة --%>
            <asp:HiddenField ID="hfInvoiceDate" runat="server" />
            <asp:HiddenField ID="hfInvoiceId" runat="server" />
            <asp:HiddenField ID="hfRemaining" runat="server" />

            <!-- الأزرار -->
            <div class="pay-actions no-print">
                <a href="<%= ReturnUrl %>" class="pay-btn-outline">
                    <i class="fas fa-times"></i> إلغاء
                </a>
                <asp:Button ID="btnSave" runat="server" Text="حفظ الدفعة"
                    CssClass="pay-btn-primary" OnClick="btnSave_Click" />
            </div>

        </div>
    </div>

    <%-- ============ ✅ JavaScript: التحقق من تاريخ الدفع ============ --%>
    <script type="text/javascript">
        function updateMinDateInfo() {
            var invDate = document.getElementById('<%= hfInvoiceDate.ClientID %>').value;
            var payDate = document.getElementById('<%= txtPaymentDate.ClientID %>').value;
            var hint = document.getElementById('minDateHint');
            if (!hint) return;

            if (!invDate) { hint.textContent = ''; return; }

            if (payDate && payDate < invDate) {
                hint.textContent = '⚠️ تاريخ الدفع لا يمكن أن يسبق تاريخ الفاتورة (' + invDate + ')';
                hint.style.color = '#dc3545';
            } else {
                hint.textContent = '';
            }
        }
        document.addEventListener('DOMContentLoaded', updateMinDateInfo);
    </script>
    <%-- ================================================================== --%>

</asp:Content>