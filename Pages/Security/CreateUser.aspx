<%@ Page Title="إنشاء حساب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Security.CreateUser" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Create User
           ========================================================= */
        .cu-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .cu-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .cu-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .cu-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .cu-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 900px; margin: 0 auto; }
        .cu-hero-left { display: flex; align-items: center; gap: 20px; }
        .cu-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .cu-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .cu-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }
        .cu-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .cu-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .cu-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }

        /* ---------- BODY ---------- */
        .cu-body { max-width: 900px; margin: -56px auto 0; padding: 0 24px 48px; position: relative; z-index: 3; }

        /* ---------- CARD ---------- */
        .cu-card { background: #ffffff; border-radius: 20px; border: 1.5px solid #eef1f3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .cu-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .cu-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .cu-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .cu-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .cu-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .cu-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .cu-card-body { padding: 20px 24px; }

        /* ---------- FIELDS ---------- */
        .cu-fields { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 16px; }
        .cu-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .cu-field.full { grid-column: 1 / -1; }
        .cu-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .cu-field label i { color: #00b14f; font-size: 12px; }
        .cu-field label .req { color: #dc3545; }
        .cu-field .form-control, .cu-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .cu-field .form-control:focus, .cu-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }
        .cu-field small { font-size: 11px; color: #6c757d; margin-top: 4px; display: flex; align-items: center; gap: 4px; }

        /* Checkbox */
        .cu-check { display: flex; align-items: center; gap: 10px; padding: 14px 16px; border-radius: 12px; background: #f9fafb; border: 1.5px solid #e9ecef; transition: all 0.2s ease; cursor: pointer; height: 44px; }
        .cu-check:hover { border-color: #00b14f; background: rgba(0,177,79,0.02); }
        .cu-check input[type="checkbox"] { width: 18px; height: 18px; cursor: pointer; accent-color: #00b14f; flex-shrink: 0; }
        .cu-check label { font-size: 13px; font-weight: 700; color: #495057; cursor: pointer; margin: 0; display: flex; align-items: center; gap: 6px; }
        .cu-check label i { color: #00b14f; font-size: 12px; }

        /* ---------- BUTTONS ---------- */
        .cu-btn-primary { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 32px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); border: none; color: #fff; font-size: 14px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(0,177,79,0.3); height: 46px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .cu-btn-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }
        .cu-btn-outline { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 28px; border-radius: 11px; background: #fff; border: 1.5px solid #dee2e6; color: #495057; font-size: 14px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 46px; text-decoration: none; font-family: 'Droid Arabic Kufi', sans-serif; }
        .cu-btn-outline:hover { border-color: #6c757d; background: #f8f9fa; color: #343a40; text-decoration: none; }
        .cu-actions { display: flex; justify-content: flex-end; gap: 10px; flex-wrap: wrap; margin-bottom: 40px; }

        /* ---------- MESSAGE ---------- */
        .cu-message { display: block; padding: 14px 18px; border-radius: 12px; font-size: 13px; font-weight: 700; margin-bottom: 16px; }
        .cu-message.success { background: rgba(0,177,79,0.08); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .cu-message.danger { background: rgba(220,53,69,0.08); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }
        .cu-message:empty { display: none; padding: 0; margin: 0; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .cu-hero { padding: 26px 20px 72px; }
            .cu-hero-titles h1 { font-size: 19px; }
            .cu-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .cu-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .cu-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .cu-hero-titles h1 { font-size: 16px; }
            .cu-card-body { padding: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .cu-hero, .cu-hero-actions, .cu-btn-glass, .cu-actions, .no-print { display: none !important; }
            .cu-page { background: #fff !important; }
            .cu-body { margin: 0 !important; padding: 0 !important; }
            .cu-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cu-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="cu-hero">
            <div class="cu-hero-content">
                <div class="cu-hero-left">
                    <div class="cu-hero-icon">
                        <i class="fas fa-user-plus"></i>
                    </div>
                    <div class="cu-hero-titles">
                        <h1>إنشاء حساب جديد</h1>
                        <p>أدخل بيانات الحساب وحدد الدور والصلاحيات</p>
                    </div>
                </div>
                <div class="cu-hero-actions no-print">
                    <a href='<%= ResolveUrl("~/Pages/Security/Users.aspx") %>' class="cu-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="cu-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="cu-message" />

            <!-- ============ Account Info ============ -->
            <div class="cu-card">
                <div class="cu-card-header">
                    <div class="cu-card-header-icon blue">
                        <i class="fas fa-user-circle"></i>
                    </div>
                    <div class="cu-card-header-text">
                        <h3>بيانات الحساب</h3>
                        <p>اسم المستخدم، اسم العرض، وكلمة المرور</p>
                    </div>
                </div>
                <div class="cu-card-body">
                    <div class="cu-fields">

                        <div class="cu-field">
                            <label><i class="fas fa-at"></i> اسم المستخدم <span class="req">*</span></label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"
                                MaxLength="100" placeholder="مثال: ahmed.ali" />
                        </div>

                        <div class="cu-field">
                            <label><i class="fas fa-user"></i> اسم العرض <span class="req">*</span></label>
                            <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control"
                                MaxLength="100" placeholder="مثال: أحمد علي" />
                        </div>

                        <div class="cu-field full">
                            <label><i class="fas fa-lock"></i> كلمة المرور <span class="req">*</span></label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
                                TextMode="Password" MaxLength="100"
                                placeholder="كلمة مرور قوية" />
                            <small>
                                <i class="fas fa-shield-alt"></i>
                                8 أحرف على الأقل، تحتوي على حرف كبير وحرف صغير ورقم ورمز خاص
                            </small>
                        </div>

                    </div>
                </div>
            </div>

            <!-- ============ Role & Permissions ============ -->
            <div class="cu-card">
                <div class="cu-card-header">
                    <div class="cu-card-header-icon purple">
                        <i class="fas fa-user-tag"></i>
                    </div>
                    <div class="cu-card-header-text">
                        <h3>الدور والصلاحيات</h3>
                        <p>حدد دور المستخدم في النظام</p>
                    </div>
                </div>
                <div class="cu-card-body">
                    <div class="cu-fields">

                        <div class="cu-field">
                            <label><i class="fas fa-user-shield"></i> الدور</label>
                            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select">
                                <asp:ListItem Value="Accountant" Text="محاسب" />
                                <asp:ListItem Value="Supervisor" Text="مشرف" />
                                <asp:ListItem Value="DataEntry" Text="مدخل بيانات" />
                                <asp:ListItem Value="Teacher" Text="معلم" />
                                <asp:ListItem Value="View" Text="عرض البيانات (View)" />
                            </asp:DropDownList>
                        </div>

                        <div class="cu-field" id="teacherRow">
                            <label>
                                <i class="fas fa-chalkboard-teacher"></i> المعلم المرتبط بالحساب
                                <span class="req">*</span>
                            </label>
                            <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-select" />
                        </div>

                        <div class="cu-field">
                            <label><i class="fas fa-toggle-on"></i> حالة الحساب</label>
                            <div class="cu-check">
                                <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                                <label for="<%= chkActive.ClientID %>">
                                    <i class="fas fa-check-circle"></i> حساب نشط
                                </label>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <!-- الأزرار -->
            <div class="cu-actions no-print">
                <a href='<%= ResolveUrl("~/Pages/Security/Users.aspx") %>' class="cu-btn-outline">
                    <i class="fas fa-times"></i> إلغاء
                </a>
                <asp:Button ID="btnSave" runat="server" Text="إنشاء الحساب"
                    CssClass="cu-btn-primary"
                    OnClick="btnSave_Click" />
            </div>

        </div>
    </div>

    <script type="text/javascript">
        function toggleTeacherRow() {
            var role = document.getElementById('<%= ddlRole.ClientID %>');
            var row = document.getElementById('teacherRow');
            if (role && row) {
                row.style.display = role.value === 'Teacher' ? '' : 'none';
            }
        }
        var ddl = document.getElementById('<%= ddlRole.ClientID %>');
        if (ddl) { ddl.onchange = toggleTeacherRow; }
        window.addEventListener('load', toggleTeacherRow);
    </script>
</asp:Content>