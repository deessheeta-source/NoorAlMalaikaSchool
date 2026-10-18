<%@ Page Title="إضافة سياسة خصم" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.DiscountPolicies.Create" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        .dp-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }
        .dp-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .dp-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .dp-hero::after {
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
        .dp-hero-content {
            position: relative;
            z-index: 2;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 24px;
            flex-wrap: wrap;
            max-width: 1000px;
            margin: 0 auto;
        }
        .dp-hero-left { display: flex; align-items: center; gap: 20px; }
        .dp-hero-icon {
            width: 68px; height: 68px; border-radius: 20px;
            background: rgba(255,255,255,0.18);
            backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,0.3);
            color: #fff;
            display: inline-flex; align-items: center; justify-content: center;
            font-size: 30px; flex-shrink: 0;
            box-shadow: 0 8px 24px rgba(0,0,0,0.12);
        }
        .dp-hero-titles h1 {
            font-size: 22px; font-weight: 800; color: #fff;
            margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px;
        }
        .dp-hero-titles p {
            font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5;
        }
        .dp-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .dp-btn-glass {
            display: inline-flex; align-items: center; gap: 8px;
            padding: 10px 20px; border-radius: 12px;
            background: rgba(255,255,255,0.15);
            backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,0.3);
            color: #fff; font-size: 13px; font-weight: 700;
            text-decoration: none; transition: all 0.2s ease;
            cursor: pointer; white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .dp-btn-glass:hover {
            background: rgba(255,255,255,0.25); color: #fff;
            text-decoration: none; transform: translateY(-2px);
        }

        .dp-body {
            max-width: 1000px; margin: -56px auto 0; padding: 0 24px 0;
            position: relative; z-index: 3;
        }

        .dp-card {
            background: #ffffff; border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden; margin-bottom: 20px;
        }
        .dp-card-header {
            display: flex; align-items: center; gap: 14px;
            padding: 18px 24px; border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .dp-card-header-icon {
            width: 44px; height: 44px; border-radius: 13px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex; align-items: center; justify-content: center;
            font-size: 18px; flex-shrink: 0;
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
        }
        .dp-card-header-icon.blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .dp-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .dp-card-header-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .dp-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .dp-card-header-text p  { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .dp-card-body { padding: 20px 24px; }

        .dp-fields {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 16px;
        }
        .dp-field {
            display: flex; flex-direction: column; gap: 6px; min-width: 0;
        }
        .dp-field.full { grid-column: 1 / -1; }
        .dp-field label {
            font-size: 12.5px; font-weight: 700; color: #495057;
            letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px;
        }
        .dp-field label i { color: #00b14f; font-size: 12px; }
        .dp-field label .req { color: #dc3545; }
        .dp-field .form-control,
        .dp-field .form-select {
            border: 1.5px solid #e9ecef; border-radius: 11px;
            padding: 10px 14px; font-size: 13px; font-weight: 600;
            color: #212529; transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 44px; width: 100%;
        }
        .dp-field textarea.form-control {
            height: auto; min-height: 90px; padding-top: 12px;
        }
        .dp-field .form-control:focus,
        .dp-field .form-select:focus {
            border-color: #00b14f; background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none;
        }
        .dp-field small { font-size: 11px; color: #6c757d; margin-top: 2px; }

        .dp-checks {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 12px;
        }
        .dp-check {
            display: flex; align-items: center; gap: 10px;
            padding: 14px 16px; border-radius: 12px;
            background: #f9fafb; border: 1.5px solid #e9ecef;
            transition: all 0.2s ease; cursor: pointer;
        }
        .dp-check:hover { border-color: #00b14f; background: rgba(0,177,79,0.02); }
        .dp-check input[type="checkbox"] {
            width: 18px; height: 18px; cursor: pointer;
            accent-color: #00b14f; flex-shrink: 0;
        }
        .dp-check label {
            font-size: 13px; font-weight: 700; color: #495057;
            cursor: pointer; margin: 0;
            display: flex; align-items: center; gap: 6px;
        }
        .dp-check label i { color: #00b14f; font-size: 12px; }

        /* ✅ NEW: Manual value box */
        .dp-manual-box {
            display: none;
            margin-top: 12px;
            padding: 14px 16px;
            border-radius: 12px;
            background: linear-gradient(135deg, rgba(0,177,79,0.06) 0%, rgba(0,177,79,0.02) 100%);
            border: 1.5px dashed rgba(0,177,79,0.4);
        }
        .dp-manual-box.show { display: block; }
        .dp-manual-box .dp-check {
            background: #fff;
            border-color: rgba(0,177,79,0.3);
        }
        .dp-manual-box .hint {
            font-size: 11.5px; color: #6c757d;
            margin-top: 8px; font-weight: 500;
        }

        .dp-btn-primary {
            display: inline-flex; align-items: center; justify-content: center;
            gap: 8px; padding: 12px 32px; border-radius: 11px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none; color: #fff; font-size: 14px; font-weight: 800;
            cursor: pointer; transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
            height: 46px; white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .dp-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff;
        }
        .dp-btn-outline {
            display: inline-flex; align-items: center; justify-content: center;
            gap: 8px; padding: 12px 28px; border-radius: 11px;
            background: #fff; border: 1.5px solid #dee2e6;
            color: #495057; font-size: 14px; font-weight: 700;
            cursor: pointer; transition: all 0.2s ease;
            height: 46px; text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .dp-btn-outline:hover {
            border-color: #6c757d; background: #f8f9fa;
            color: #343a40; text-decoration: none;
        }
        .dp-actions {
            display: flex; justify-content: flex-end; gap: 10px;
            flex-wrap: wrap; margin-bottom: 40px;
        }

        .dp-message {
            display: block; padding: 14px 18px; border-radius: 12px;
            font-size: 13px; font-weight: 700; margin-bottom: 16px;
        }
        .dp-message.danger {
            background: rgba(220,53,69,0.08); color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }
        .dp-message.success {
            background: rgba(0,177,79,0.08); color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .dp-message:empty { display: none; padding: 0; margin: 0; }

        @media (max-width: 992px) {
            .dp-hero { padding: 26px 20px 72px; }
            .dp-hero-titles h1 { font-size: 19px; }
            .dp-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .dp-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .dp-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .dp-hero-titles h1 { font-size: 16px; }
            .dp-card-body { padding: 16px; }
        }

        @media print {
            .dp-hero-actions, .dp-actions, .no-print { display: none !important; }
            .dp-page { background: #fff !important; }
            .dp-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact; print-color-adjust: exact;
                border-radius: 0 !important; padding: 16px 20px !important;
            }
            .dp-body { margin-top: 0 !important; padding: 0 !important; }
            .dp-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dp-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="dp-hero">
            <div class="dp-hero-content">
                <div class="dp-hero-left">
                    <div class="dp-hero-icon">
                        <i class="fas fa-plus-circle"></i>
                    </div>
                    <div class="dp-hero-titles">
                        <h1>إضافة سياسة خصم / إعفاء</h1>
                        <p>أدخل بيانات السياسة ثم اضغط حفظ</p>
                    </div>
                </div>
                <div class="dp-hero-actions no-print">
                    <a href="Default.aspx" class="dp-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="dp-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="dp-message" />

            <!-- بيانات السياسة -->
            <div class="dp-card">
                <div class="dp-card-header">
                    <div class="dp-card-header-icon blue">
                        <i class="fas fa-percent"></i>
                    </div>
                    <div class="dp-card-header-text">
                        <h3>بيانات السياسة</h3>
                        <p>الاسم، النوع، والقيمة</p>
                    </div>
                </div>
                <div class="dp-card-body">
                    <div class="dp-fields">

                        <div class="dp-field">
                            <label><i class="fas fa-tag"></i> اسم السياسة <span class="req">*</span></label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"
                                placeholder="مثال: خصم الإخوة، إعفاء أبناء الشهداء" />
                        </div>

                        <div class="dp-field">
                            <label><i class="fas fa-list"></i> النوع <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select"
                                AutoPostBack="false" />
                        </div>

                        <div class="dp-field">
                            <label><i class="fas fa-balance-scale"></i> نوع القيمة <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlKind" runat="server" CssClass="form-select">
                                <asp:ListItem Value="Percentage" Text="نسبة مئوية (%)" />
                                <asp:ListItem Value="Amount" Text="مبلغ ثابت (د.ل)" />
                            </asp:DropDownList>
                        </div>

                        <%-- ✅ الحقل الذي سُيخفي عند تفعيل "المبلغ اليدوي" --%>
                        <div class="dp-field" id="valueField">
                            <label><i class="fas fa-calculator"></i> القيمة <span class="req">*</span></label>
                            <asp:TextBox ID="txtValue" runat="server" CssClass="form-control"
                                TextMode="Number" step="1" min="0" placeholder="مثال: 5 أو 25" />
                            <small><i class="fas fa-info-circle"></i> أدخل رقمًا صحيحًا فقط</small>
                        </div>

                        <%-- ============ ✅ NEW: صندوق "المبلغ اليدوي" ============ --%>
                        <div id="manualValueBox" class="dp-manual-box dp-field full">
                            <div class="dp-check">
                                <asp:CheckBox ID="chkManualValue" runat="server" />
                                <label>
                                    <i class="fas fa-hand-holding-usd"></i>
                                    المبلغ يُدخل يدوياً عند إنشاء كل فاتورة
                                </label>
                            </div>
                            <div class="hint">
                                عند التفعيل، لن تُثبَّت قيمة ثابتة في هذه السياسة، بل سيُطلب من المستخدم
                                إدخال مقدار الخصم يدوياً في كل فاتورة (مثلاً: 150 د.ل).
                            </div>
                        </div>
                        <%-- ======================================================== --%>

                    </div>
                </div>
            </div>

            <!-- نطاق التطبيق -->
            <div class="dp-card">
                <div class="dp-card-header">
                    <div class="dp-card-header-icon purple">
                        <i class="fas fa-bullseye"></i>
                    </div>
                    <div class="dp-card-header-text">
                        <h3>نطاق التطبيق</h3>
                        <p>الرسوم التي يُطبق عليها الخصم</p>
                    </div>
                </div>
                <div class="dp-card-body">
                    <div class="dp-fields">
                        <div class="dp-field full">
                            <label><i class="fas fa-file-invoice-dollar"></i> رسم الإشتراك</label>
                            <asp:DropDownList ID="ddlAppliesToFeeType" runat="server" CssClass="form-select">
                                <asp:ListItem Value="" Text="كل أنواع الرسوم" />
                                <asp:ListItem Value="AnnualSubscription" Text="الاشتراك السنوي" />
                                <asp:ListItem Value="Tuition" Text="الرسوم الدراسية" />
                                <asp:ListItem Value="Transport" Text="النقل المدرسي" />
                                <asp:ListItem Value="Activity" Text="النشاط المدرسي" />
                                <asp:ListItem Value="Trip" Text="الرحلات" />
                                <asp:ListItem Value="Other" Text="رسوم أخرى" />
                            </asp:DropDownList>
                            <small><i class="fas fa-info-circle"></i> يحدد الرسم الذي يسمح للنظام بتطبيق الخصم عليه.</small>
                        </div>
                    </div>
                </div>
            </div>

            <!-- الإعدادات -->
            <div class="dp-card">
                <div class="dp-card-header">
                    <div class="dp-card-header-icon orange">
                        <i class="fas fa-cog"></i>
                    </div>
                    <div class="dp-card-header-text">
                        <h3>الإعدادات</h3>
                        <p>التطبيق التلقائي والحالة</p>
                    </div>
                </div>
                <div class="dp-card-body">
                    <div class="dp-checks">
                        <div class="dp-check">
                            <asp:CheckBox ID="chkAuto" runat="server" Checked="true" />
                            <label><i class="fas fa-bolt"></i> يُطبق تلقائياً عند إنشاء الفاتورة</label>
                        </div>
                        <div class="dp-check">
                            <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                            <label><i class="fas fa-check-circle"></i> نشطة</label>
                        </div>
                    </div>
                </div>
            </div>

            <!-- الوصف -->
            <div class="dp-card">
                <div class="dp-card-header">
                    <div class="dp-card-header-icon">
                        <i class="fas fa-sticky-note"></i>
                    </div>
                    <div class="dp-card-header-text">
                        <h3>الوصف</h3>
                        <p>وصف تفصيلي للسياسة (اختياري)</p>
                    </div>
                </div>
                <div class="dp-card-body">
                    <div class="dp-field full">
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="3"
                            placeholder="أدخل وصفاً تفصيلياً للسياسة..." />
                    </div>
                </div>
            </div>

            <!-- الأزرار -->
            <div class="dp-actions no-print">
                <a href="Default.aspx" class="dp-btn-outline">
                    <i class="fas fa-times"></i> إلغاء
                </a>
                <asp:Button ID="btnSave" runat="server" Text="حفظ"
                    CssClass="dp-btn-primary" OnClick="btnSave_Click" />
            </div>

        </div>
    </div>

    <%-- ============ ✅ JS: إظهار/إخفاء صندوق "المبلغ اليدوي" ============ --%>
  <script type="text/javascript">
      (function () {
          function initManualToggle() {
              var ddl = document.getElementById('<%= ddlType.ClientID %>');
            var box      = document.getElementById('manualValueBox');
            var valueFld = document.getElementById('valueField');
            var chk      = document.getElementById('<%= chkManualValue.ClientID %>');
              if (!ddl || !box) return;

              // ✅ الأنواع التي تُإدخَل يدوياً (تمت إضافة annualsubscription)
              function isManualType() {
                  var v = (ddl.value || '').toLowerCase();
                  return v === 'sibling'
                      || v === 'earlybird'
                      || v === 'annualsubscription';
              }

              function toggleValueField() {
                  if (!valueFld || !chk) return;
                  valueFld.style.display = (chk.checked) ? 'none' : 'flex';
              }

              function toggleManualBox() {
                  if (isManualType()) {
                      box.classList.add('show');
                  } else {
                      box.classList.remove('show');
                      if (chk) chk.checked = false;
                      toggleValueField();
                  }
              }

              ddl.addEventListener('change', toggleManualBox);
              if (chk) chk.addEventListener('change', toggleValueField);

              toggleManualBox();
              toggleValueField();
          }

          if (document.readyState === 'loading')
              document.addEventListener('DOMContentLoaded', initManualToggle);
          else
              initManualToggle();
      })();
</script>
    <%-- ===================================================================== --%>

</asp:Content>