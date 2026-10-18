<%@ Page Title="إضافة طالب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Students.Create" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        .create-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .create-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .create-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .create-hero::after {
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
        .create-hero-content {
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
        .create-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .create-hero-icon {
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
        .create-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .create-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }
        .create-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }
        .create-btn-glass {
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
        .create-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .create-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .create-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .create-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .create-card-header-icon {
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
            position: relative;
        }
        .create-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .create-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .create-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }

        .create-card-body {
            padding: 24px;
        }

        /* ---------- SECTION TITLE ---------- */
        .form-section-title {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 12px 18px;
            margin-bottom: 18px;
            border-radius: 12px;
            background: linear-gradient(135deg, rgba(0,177,79,0.08) 0%, rgba(0,177,79,0.03) 100%);
            border-right: 3px solid #00b14f;
            color: #007a33;
            font-weight: 800;
            font-size: 14px;
        }
        .form-section-title i {
            color: #00b14f;
            font-size: 15px;
        }

        /* ---------- FORM FIELDS ---------- */
        .create-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            margin-bottom: 16px;
            min-width: 0;
        }
        .create-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
        }
        .create-field label .req {
            color: #dc3545;
            margin-right: 3px;
        }
        .create-field .form-control,
        .create-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px;
            width: 100%;
        }
        .create-field textarea.form-control {
            height: auto;
            min-height: 70px;
        }
        .create-field .form-control:focus,
        .create-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }
        .create-field .text-danger {
            font-size: 11.5px;
            font-weight: 700;
            margin-top: 2px;
        }
        .create-field small {
            font-size: 11px;
            color: #6c757d;
        }

        /* Photo block */
        .photo-block {
            display: flex;
            align-items: center;
            gap: 18px;
            padding: 16px;
            border-radius: 14px;
            background: #f9fafb;
            border: 1.5px dashed #d8e6dd;
        }
        .photo-info {
            flex: 1;
            min-width: 0;
        }

        /* ---------- BUTTONS ---------- */
        .create-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 12px 28px;
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
        .create-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,0.4);
            color: #fff;
        }
        .create-btn-outline {
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
            white-space: nowrap;
            text-decoration: none;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .create-btn-outline:hover {
            border-color: #6c757d;
            background: #f8f9fa;
            text-decoration: none;
            color: #343a40;
        }

        /* Alerts */
        .create-alert {
            display: block;
            padding: 14px 18px;
            border-radius: 12px;
            font-weight: 700;
            font-size: 13px;
            margin-bottom: 18px;
        }
        .create-alert.alert-success {
            background: rgba(0,177,79,0.1);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.3);
        }
        .create-alert.alert-danger {
            background: rgba(220,53,69,0.08);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.3);
        }
        .create-alert.d-none {
            display: none !important;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .create-hero { padding: 26px 20px 72px; }
            .create-hero-titles h1 { font-size: 19px; }
            .create-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .create-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .create-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .create-hero-titles h1 { font-size: 16px; }
            .create-hero-titles p { font-size: 12px; }
            .create-btn-glass { padding: 8px 14px; font-size: 12px; }
            .create-card-body { padding: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .create-hero-actions,
            .no-print { display: none !important; }
            .create-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .create-body { margin-top: 0 !important; padding: 0 !important; }
            .create-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="create-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="create-hero">
            <div class="create-hero-content">
                <div class="create-hero-left">
                    <div class="create-hero-icon">
                        <i class="fas fa-user-plus"></i>
                    </div>
                    <div class="create-hero-titles">
                        <h1>إضافة طالب جديد</h1>
                        <p>قم بتعبئة بيانات الطالب ثم اضغط حفظ الطالب</p>
                    </div>
                </div>
                <div class="create-hero-actions">
                    <asp:Button ID="btnBackToListTop" runat="server" Text="العودة للقائمة"
                        CssClass="create-btn-glass"
                        OnClick="btnCancel_Click"
                        CausesValidation="false" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="create-body">

            <div class="create-card">

                <div class="create-card-header">
                    <div class="create-card-header-icon">
                        <i class="fas fa-plus-circle"></i>
                    </div>
                    <div class="create-card-header-text">
                        <h3>نموذج إضافة طالب</h3>
                        <p>الحقول المُعلَّمة بـ <span style="color:#dc3545;">*</span> إلزامية</p>
                    </div>
                </div>

                <div class="create-card-body">

                    <asp:Label ID="lblMessage" runat="server" CssClass="create-alert d-none" />
                    <asp:HiddenField ID="hfStage" runat="server" />

                    <!-- ============ البيانات الأساسية ============ -->
                    <div class="form-section-title">
                        <i class="fas fa-user"></i>
                        <span>بيانات الطالب</span>
                    </div>

                    <div class="row">
                        <div class="col-md-3">
                            <div class="create-field">
                                <label>الاسم <span class="req">*</span></label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="الاسم" />
                                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                                    ControlToValidate="txtFirstName" ErrorMessage="الاسم مطلوب"
                                    CssClass="text-danger" Display="Dynamic" ValidationGroup="vgSave" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="create-field">
                                <label>الأب <span class="req">*</span></label>
                                <asp:TextBox ID="txtFatherName" runat="server" CssClass="form-control" placeholder="اسم الأب" />
                                <asp:RequiredFieldValidator ID="rfvFatherName" runat="server"
                                    ControlToValidate="txtFatherName" ErrorMessage="اسم الأب مطلوب"
                                    CssClass="text-danger" Display="Dynamic" ValidationGroup="vgSave" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="create-field">
                                <label>الجد <span class="req">*</span></label>
                                <asp:TextBox ID="txtGrandFatherName" runat="server" CssClass="form-control" placeholder="اسم الجد" />
                                <asp:RequiredFieldValidator ID="rfvGrandFatherName" runat="server"
                                    ControlToValidate="txtGrandFatherName" ErrorMessage="اسم الجد مطلوب"
                                    CssClass="text-danger" Display="Dynamic" ValidationGroup="vgSave" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="create-field">
                                <label>اللقب <span class="req">*</span></label>
                                <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control" placeholder="اللقب" />
                                <asp:RequiredFieldValidator ID="rfvSurname" runat="server"
                                    ControlToValidate="txtSurname" ErrorMessage="اللقب مطلوب"
                                    CssClass="text-danger" Display="Dynamic" ValidationGroup="vgSave" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>الرقم الوطني <span class="req">*</span></label>
                                <asp:TextBox ID="txtNationalId" runat="server" CssClass="form-control" placeholder="أدخل الرقم الوطني" />
                                <asp:RequiredFieldValidator ID="rfvNationalId" runat="server"
                                    ControlToValidate="txtNationalId" ErrorMessage="الرقم الوطني مطلوب"
                                    CssClass="text-danger" Display="Dynamic" ValidationGroup="vgSave" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>رقم التسجيل</label>
                                <asp:TextBox ID="txtRegistrationNo" runat="server" CssClass="form-control" placeholder="أدخل رقم التسجيل" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>تاريخ الميلاد</label>
                                <asp:TextBox ID="txtBirthDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>الجنس</label>
                                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- اختر --" Value="" />
                                    <asp:ListItem Text="ذكر" Value="Male" />
                                    <asp:ListItem Text="أنثى" Value="Female" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>الجنسية</label>
                                <asp:TextBox ID="txtNationality" runat="server" CssClass="form-control" placeholder="مثال: ليبي" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>الهاتف</label>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="أدخل رقم الهاتف" />
                            </div>
                        </div>
                    </div>

                    <!-- ============ الصف / الشعبة / القسم ============ -->
                    <div class="form-section-title">
                        <i class="fas fa-layer-group"></i>
                        <span>الصف والشعبة</span>
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>الصف <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlClassRoom" runat="server" CssClass="form-select"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlClassRoom_SelectedIndexChanged" />
                                <asp:RequiredFieldValidator ID="rfvClassRoom" runat="server"
                                    ControlToValidate="ddlClassRoom" ErrorMessage="اختر الصف"
                                    CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="vgSave" InitialValue="0" />
                            </div>
                        </div>

                          <asp:Panel ID="pnlSection" runat="server" CssClass="col-md-4" Visible="false">
                              <div class="create-field">
                                  <label>القسم <span class="req">*</span></label>
                                  <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                      AutoPostBack="true"
                                      OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                      <asp:ListItem Text="-- اختر القسم --" Value="0" />
                                      <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                      <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                                  </asp:DropDownList>
                                  <asp:RequiredFieldValidator ID="rfvSection" runat="server"
                                      ControlToValidate="ddlSection" ErrorMessage="اختر القسم"
                                      CssClass="text-danger" Display="Dynamic"
                                      ValidationGroup="vgSave" InitialValue="0" Enabled="false" />
                              </div>
                          </asp:Panel>

                        <div class="col-md-4">
                            <div class="create-field">
                                <label>الشعبة <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlSectionRoom" runat="server" CssClass="form-select"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlSectionRoom_SelectedIndexChanged" />
                                <asp:RequiredFieldValidator ID="rfvSectionRoom" runat="server"
                                    ControlToValidate="ddlSectionRoom" ErrorMessage="اختر الشعبة"
                                    CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="vgSave" InitialValue="0" />
                            </div>
                        </div>

                      
                    </div>

                    <div class="row">
                        <div class="col-md-8">
                            <div class="create-field">
                                <label>العنوان</label>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="أدخل العنوان" />
                            </div>
                        </div>
                    </div>

                    <!-- ============ معلومات إضافية ============ -->
                    <div class="form-section-title">
                        <i class="fas fa-info-circle"></i>
                        <span>معلومات إضافية</span>
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>اسم الأم</label>
                                <asp:TextBox ID="txtMotherName" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>هاتف الأم</label>
                                <asp:TextBox ID="txtMotherPhone" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="create-field">
                                <label>ملاحظات</label>
                                <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control"
                                    TextMode="MultiLine" Rows="2" placeholder="أدخل أي ملاحظات إضافية" />
                            </div>
                        </div>
                    </div>

                    <!-- ============ الصورة ============ -->
                    <div class="form-section-title">
                        <i class="fas fa-image"></i>
                        <span>صورة الطالب</span>
                    </div>

                    <div class="photo-block">
                        <div class="photo-info">
                            <label style="font-size:12.5px;font-weight:700;color:#495057;display:block;margin-bottom:6px;">
                                اختر صورة الطالب (اختياري)
                            </label>
                            <asp:FileUpload ID="fuPhoto" runat="server" CssClass="form-control" />
                            <small class="text-muted" style="display:block;margin-top:6px;">
                                JPG أو PNG بحد أقصى 2 ميجابايت
                            </small>
                        </div>
                    </div>

                    <!-- ============ ولي الأمر ============ -->
                    <div class="form-section-title" style="margin-top:24px;">
                        <i class="fas fa-users"></i>
                        <span>بيانات ولي الأمر</span>
                    </div>

                    <div class="row">
                        
                        <div class="col-md-3">
                            <div class="create-field">
                                <label>اسم ولي الأمر</label>
                                <asp:TextBox ID="txtParentName" runat="server" CssClass="form-control" placeholder="أدخل الاسم" />
                            </div>
                        </div>
                        <div class="col-md-3">
    <div class="create-field">
        <label>الرقم الوطني</label>
        <asp:TextBox ID="txtParentNationalId" runat="server" CssClass="form-control" placeholder="أدخل الرقم الوطني" />
    </div>
</div>
                        <div class="col-md-3">
                            <div class="create-field">
                                <label>هاتف ولي الأمر</label>
                                <asp:TextBox ID="txtParentPhone" runat="server" CssClass="form-control" placeholder="أدخل رقم الهاتف" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="create-field">
                                <label>صلة القرابة</label>
                                <asp:DropDownList ID="ddlParentRelation" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- اختر --" Value="" />
                                    <asp:ListItem Text="أب" Value="Father" />
                                    <asp:ListItem Text="أم" Value="Mother" />
                                    <asp:ListItem Text="وصي" Value="Guardian" />
                                    <asp:ListItem Text="أخرى" Value="Other" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6">
                            <div class="create-field">
                                <label>عنوان ولي الأمر</label>
                                <asp:TextBox ID="txtParentAddress" runat="server" CssClass="form-control" placeholder="أدخل العنوان" />
                            </div>
                        </div>
                    </div>

                    <!-- ============ الأزرار ============ -->
                    <hr style="border-color:#eef1f3;margin:24px 0;" />

                    <div class="d-flex gap-2 flex-wrap">
                        <asp:Button ID="btnSave" runat="server" Text="حفظ الطالب"
                            CssClass="create-btn-primary"
                            ValidationGroup="vgSave"
                            OnClick="btnSave_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="إلغاء"
                            CssClass="create-btn-outline"
                            OnClick="btnCancel_Click"
                            CausesValidation="false" />
                    </div>

                </div>
            </div>

        </div>
    </div>
</asp:Content>