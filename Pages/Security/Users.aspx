<%@ Page Title="المستخدمون والصلاحيات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Security.Users" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Users & Permissions
           ========================================================= */
        .us-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .us-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .us-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .us-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .us-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .us-hero-left { display: flex; align-items: center; gap: 20px; }
        .us-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .us-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .us-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }
        .us-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .us-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .us-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }
        .us-btn-glass.primary { background: #fff; color: #00b14f; border-color: #fff; }
        .us-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .us-body { max-width: 1280px; margin: -56px auto 0; padding: 0 24px 48px; position: relative; z-index: 3; }

        /* ---------- CARD ---------- */
        .us-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .us-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .us-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .us-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .us-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .us-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .us-count-pill { margin-right: auto; display: inline-flex; align-items: center; gap: 6px; padding: 6px 14px; border-radius: 999px; font-size: 12px; font-weight: 800; background: rgba(0,177,79,0.1); color: #007a33; border: 1px solid rgba(0,177,79,0.25); flex-shrink: 0; }
        .us-card-body { padding: 20px 24px; }
        .us-card-body.no-padding { padding: 0; }

        /* ---------- MESSAGE ---------- */
        .us-message { display: block; padding: 14px 18px; border-radius: 12px; font-size: 13px; font-weight: 700; margin-bottom: 16px; }
        .us-message.success { background: rgba(0,177,79,0.08); color: #dffdeb; border: 1px solid rgba(0,177,79,0.25); }
        .us-message.danger { background: rgba(220,53,69,0.08); color: #ffceb6; border: 1px solid rgba(220,53,69,0.25); }
        .us-message:empty { display: none; padding: 0; margin: 0; }

        /* ---------- TABLE ---------- */
        .us-table-wrap { overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .us-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 13px; min-width: 1150px; }
        .us-table thead th { background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%); color: #1f3d33; font-size: 11.5px; font-weight: 800; padding: 14px 12px !important; text-align: center; border-bottom: 2px solid #d8e6dd; white-space: nowrap; }
        .us-table tbody td { font-size: 13px; padding: 12px 10px !important; vertical-align: middle; border-bottom: 1px solid #f1f3f5; color: #212529; background: #fff; text-align: center; }
        .us-table tbody tr:hover td { background: #f8fdfa; }
        .us-table tbody tr:last-child td { border-bottom: none; }

        /* User name cell */
        .us-user-cell { display: flex; align-items: center; gap: 10px; font-weight: 700; color: #212529; text-align: start; }
        .us-user-avatar { width: 36px; height: 36px; border-radius: 10px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 15px; flex-shrink: 0; }
        .us-user-info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
        .us-user-name { font-weight: 800; font-size: 13px; color: #212529; }
        .us-user-username { font-size: 13px; color: #6c757d;  }

        /* Role badge */
        .us-role-badge { display: inline-flex; align-items: center; gap: 5px; padding: 5px 12px; border-radius: 999px; font-size: 11.5px; font-weight: 700; background: rgba(13,110,253,0.08); color: #0d6efd; border: 1px solid rgba(13,110,253,0.2); white-space: nowrap; }
        .us-role-badge.admin { background: rgba(220,53,69,0.08); color: #e12b3d; border-color: rgba(220,53,69,0.2); }
        .us-role-badge.accountant { background: rgba(0,177,79,0.08); color: #00b14f; border-color: rgba(0,177,79,0.2); }
        .us-role-badge.supervisor { background: rgba(111,66,193,0.08); color: #6f42c1; border-color: rgba(111,66,193,0.2); }
        .us-role-badge.dataentry { background: rgba(253,126,20,0.08); color: #fd7e14; border-color: rgba(253,126,20,0.2); }
        .us-role-badge.teacher { background: rgba(97, 49, 189, 0.16); color: #470bb1; border-color: rgba(95, 43, 187, 0.31); }

        /* Status badge */
        .us-status-badge { display: inline-flex; align-items: center; gap: 5px; padding: 5px 12px; border-radius: 999px; font-size: 11.5px; font-weight: 700; white-space: nowrap; }
        .us-status-badge.active { background: rgba(0,177,79,0.12); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .us-status-badge.inactive { background: #f8f9fa; color: #6c757d; border: 1px solid #e9ecef; }

        /* Teacher link */
        .us-teacher-link { font-size: 12px; color: #495057;  background: #f8f9fa; padding: 3px 8px; border-radius: 6px; border: 1px solid #e9ecef; display: inline-block; }

        /* Inline role select */
        .us-role-select { display: inline-flex; align-items: center; gap: 6px; }
        .us-role-select .form-select { border: 1.5px solid #e9ecef; border-radius: 9px; padding: 6px 10px; font-size: 12px; font-weight: 600; background: #f9fafb; height: 36px; min-width: 130px; font-family: 'Droid Arabic Kufi', sans-serif; }
        .us-role-select .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 3px rgba(0,177,79,0.12); outline: none; }

        /* ---------- ACTION BUTTONS ---------- */
        .us-btn-action { display: inline-flex; align-items: center; justify-content: center; gap: 4px; padding: 7px 12px; border-radius: 8px; font-size: 11.5px; font-weight: 700; text-decoration: none; border: 1.5px solid; transition: all 0.15s ease; cursor: pointer; background: #fff; font-family: 'Droid Arabic Kufi', sans-serif; white-space: nowrap; }
        .us-btn-action.edit { border-color: #00b14f; color: #00b14f; }
        .us-btn-action.edit:hover { background: rgba(0,177,79,0.08); color: #007a33; text-decoration: none; }
        .us-btn-action.disable { border-color: #dc3545; color: #dc3545; }
        .us-btn-action.disable:hover { background: rgba(220,53,69,0.08); color: #a51c1c; text-decoration: none; }
        .us-btn-action.enable { border-color: #00b14f; color: #00b14f; background: rgba(0,177,79,0.04); }
        .us-btn-action.enable:hover { background: rgba(0,177,79,0.12); color: #007a33; text-decoration: none; }
        .us-btn-action.reset { border-color: #0d6efd; color: #0d6efd; }
        .us-btn-action.reset:hover { background: rgba(13,110,253,0.08); color: #0d6efd; text-decoration: none; }
        .us-btn-action.save { border-color: #00b14f; color: #fff; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); }
        .us-btn-action.save:hover { transform: translateY(-1px); box-shadow: 0 4px 10px rgba(0,177,79,0.3); color: #fff; text-decoration: none; }
        .us-btn-action.delete { border-color: #dc3545; color: #fff; background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); }
        .us-btn-action.delete:hover { transform: translateY(-1px); box-shadow: 0 4px 10px rgba(220,53,69,0.35); color: #fff; text-decoration: none; }

        /* ---------- EDIT PANEL ---------- */
        .us-edit-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(135deg, #f8fdfa 0%, #ffffff 100%); }
        .us-edit-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(111,66,193,0.3); }

        .us-fields { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 16px; margin-bottom: 20px; }
        .us-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .us-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .us-field label i { color: #00b14f; font-size: 12px; }
        .us-field .form-control, .us-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .us-field .form-control:focus, .us-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }

        .us-check { display: flex; align-items: center; gap: 10px; padding: 14px 16px; border-radius: 12px; background: #f9fafb; border: 1.5px solid #e9ecef; transition: all 0.2s ease; cursor: pointer; height: 44px; }
        .us-check:hover { border-color: #00b14f; background: rgba(0,177,79,0.02); }
        .us-check input[type="checkbox"] { width: 18px; height: 18px; cursor: pointer; accent-color: #00b14f; flex-shrink: 0; }
        .us-check label { font-size: 13px; font-weight: 700; color: #495057; cursor: pointer; margin: 0; display: flex; align-items: center; gap: 6px; }
        .us-check label i { color: #00b14f; font-size: 12px; }

        .us-btn-primary { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 32px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); border: none; color: #fff; font-size: 14px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(0,177,79,0.3); height: 46px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .us-btn-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }
        .us-btn-outline { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 28px; border-radius: 11px; background: #fff; border: 1.5px solid #dee2e6; color: #495057; font-size: 14px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 46px; text-decoration: none; font-family: 'Droid Arabic Kufi', sans-serif; }
        .us-btn-outline:hover { border-color: #6c757d; background: #f8f9fa; color: #343a40; text-decoration: none; }

        /* ---------- EMPTY ---------- */
        .us-empty { text-align: center; padding: 60px 20px; }
        .us-empty-icon { width: 80px; height: 80px; border-radius: 22px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 36px; margin-bottom: 18px; }
        .us-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 8px; }
        .us-empty p { color: #6c757d; font-size: 13px; margin: 0; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .us-hero { padding: 26px 20px 72px; }
            .us-hero-titles h1 { font-size: 19px; }
            .us-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .us-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .us-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .us-hero-titles h1 { font-size: 16px; }
            .us-card-body { padding: 16px; }
        }

        /* عرض كل مستخدم كبطاقة على الهاتف بدلاً من تمرير الجدول أفقياً */
        @media (max-width: 767.98px) {
            .us-card-body.no-padding { padding: 12px !important; }
            .us-table-wrap { overflow: visible; }
            .us-table,
            .us-table tbody { display: block; width: 100%; min-width: 0; }
            .us-table thead { display: none; }
            .us-table tbody tr.us-table-header-row { display: none !important; }
            .us-table tbody tr {
                display: block;
                width: 100%;
                margin: 0 0 12px;
                padding: 10px 14px;
                border: 1px solid #e5eee9;
                border-radius: 14px;
                background: #fff;
                box-shadow: 0 4px 14px rgba(31, 61, 51, 0.06);
            }
            .us-table tbody tr:last-child { margin-bottom: 0; }
            .us-table tbody td {
                display: grid;
                grid-template-columns: 82px minmax(0, 1fr);
                align-items: center;
                gap: 10px;
                width: 100% !important;
                padding: 9px 0 !important;
                border-bottom: 1px solid #f1f3f5;
                text-align: right !important;
                white-space: normal;
            }
            .us-table tbody td::before {
                content: attr(data-label);
                color: #6c757d;
                font-size: 11px;
                font-weight: 800;
                line-height: 1.5;
                text-align: right;
            }
            .us-table tbody td:first-child {
                display: block;
                padding-top: 4px !important;
                padding-bottom: 12px !important;
            }
            .us-table tbody td:first-child::before {
                display: block;
                margin-bottom: 8px;
            }
            .us-table tbody td:last-child {
                display: block;
                border-bottom: 0;
                padding-bottom: 2px !important;
            }
            .us-table tbody td:last-child::before { display: block; margin-bottom: 8px; }
            .us-user-cell { align-items: flex-start; }
            .us-user-info,
            .us-user-name,
            .us-user-username,
            .us-teacher-link { min-width: 0; overflow-wrap: anywhere; }
            .us-role-select { display: flex; flex-wrap: wrap; justify-content: flex-start; }
            .us-role-select .form-select { flex: 1 1 130px; min-width: 0; width: 100%; }
            .us-table tbody td:last-child .d-flex {
                display: grid !important;
                grid-template-columns: repeat(2, minmax(0, 1fr));
                width: 100%;
                min-width: 0;
                gap: 6px !important;
            }
            .us-table tbody td:last-child .us-btn-action {
                width: 100%;
                min-width: 0;
                padding: 6px 8px;
                font-size: 10.5px;
                white-space: normal;
                overflow-wrap: anywhere;
            }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .us-hero, .us-hero-actions, .us-btn-glass, .no-print { display: none !important; }
            .us-page { background: #fff !important; }
            .us-body { margin: 0 !important; padding: 0 !important; }
            .us-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .us-table { font-size: 10px !important; min-width: 0 !important; }
        }
        th {
            text-align: center;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="us-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="us-hero">
            <div class="us-hero-content">
                <div class="us-hero-left">
                    <div class="us-hero-icon">
                        <i class="fas fa-users-cog"></i>
                    </div>
                    <div class="us-hero-titles">
                        <h1>المستخدمون والصلاحيات</h1>
                        <p>إدارة المستخدمين وأدوارهم والصلاحيات المرتبطة بهم</p>
                    </div>
                </div>
                <div class="us-hero-actions no-print">
                    <a href='<%= ResolveUrl("~/Pages/Security/CreateUser.aspx") %>' class="us-btn-glass primary">
                        <i class="fas fa-user-plus"></i> إنشاء حساب جديد
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="us-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="us-message" />

            <!-- ============ Users Table ============ -->
            <div class="us-card">
                <div class="us-card-header">
                    <div class="us-card-header-icon">
                        <i class="fas fa-users"></i>
                    </div>
                    <div class="us-card-header-text">
                        <h3>قائمة المستخدمين</h3>
                        <p>جميع الحسابات المسجلة في النظام</p>
                    </div>
                    <span class="us-count-pill">
                        <i class="fas fa-user-check"></i>
                        <asp:Label ID="lblCount" runat="server" Text="0" />
                    </span>
                </div>

                <div class="us-card-body no-padding">
                    <div class="us-table-wrap">
                        <asp:GridView ID="gvUsers" runat="server" CssClass="us-table"
                            AutoGenerateColumns="false"
                            EmptyDataText="" GridLines="None"
                            DataKeyNames="Id"
                            ShowHeaderWhenEmpty="true"
                            OnRowCommand="gvUsers_RowCommand"
                            OnRowDataBound="gvUsers_RowDataBound">

                            <Columns>

                                <asp:TemplateField HeaderText="المستخدم" ItemStyle-CssClass="text-start">
                                    <ItemTemplate>
                                        <div class="us-user-cell">
                                            <div class="us-user-avatar">
                                                <i class="fas fa-user"></i>
                                            </div>
                                            <div class="us-user-info">
                                                <span class="us-user-name"><%#: Eval("DisplayName") %></span>
                                                <span class="us-user-username"><%#: Eval("Username") %></span>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الدور">
                                    <ItemTemplate>
                                        <%# FormatRoleBadge(Eval("RoleName"), Eval("Role")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المعلم المرتبط">
                                    <ItemTemplate>
                                        <%# FormatTeacherLink(Eval("TeacherLink")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة">
                                    <ItemTemplate>
                                        <%# FormatStatusBadge(Eval("IsActive")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="تغيير الدور" ItemStyle-CssClass="no-print">
                                    <ItemTemplate>
                                        <div class="us-role-select">
                                            <asp:DropDownList ID="ddlRoleGrid" runat="server" CssClass="form-select">
                                                <asp:ListItem Value="Admin" Text="مدير النظام" />
                                                <asp:ListItem Value="Accountant" Text="محاسب" />
                                                <asp:ListItem Value="Supervisor" Text="مشرف" />
                                                <asp:ListItem Value="DataEntry" Text="مدخل بيانات" />
                                                <asp:ListItem Value="Teacher" Text="معلم" />
                                <asp:ListItem Value="View" Text="عرض البيانات (View)" />
                                            </asp:DropDownList>
                                            <asp:Button ID="btnSetRole" runat="server" Text="حفظ"
                                                CssClass="us-btn-action save"
                                                CommandName="SetRole"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CausesValidation="false" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="إجراءات" ItemStyle-CssClass="text-center no-print" ItemStyle-Width="340px">
                                    <ItemTemplate>
                                        <div class="d-flex gap-1 justify-content-center flex-wrap">

                                            <%-- 1) تعديل --%>
                                            <asp:Button ID="btnEditUser" runat="server" Text="تعديل"
                                                CssClass="us-btn-action edit"
                                                CommandName="EditUser"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CausesValidation="false"
                                                ToolTip="تعديل بيانات المستخدم" />

                                            <%-- 2) تعطيل / تفعيل --%>
                                            <asp:Button ID="btnToggle" runat="server"
                                                Text='<%# (bool)Eval("IsActive") ? "تعطيل" : "تفعيل" %>'
                                                CssClass='<%# (bool)Eval("IsActive") ? "us-btn-action disable" : "us-btn-action enable" %>'
                                                CommandName="ToggleActive"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CausesValidation="false"
                                                ToolTip="تعطيل أو تفعيل الحساب" />

                                            <%-- 3) إعادة كلمة المرور --%>
                                            <asp:Button ID="btnReset" runat="server" Text="إعادة كلمة المرور"
                                                CssClass="us-btn-action reset"
                                                CommandName="ResetPass"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CausesValidation="false"
                                                ToolTip="توليد كلمة مرور مؤقتة"
                                                OnClientClick="return confirm('هل أنت متأكد من إعادة كلمة المرور لهذا المستخدم؟');" />

                                            <%-- 4) حذف --%>
                                            <asp:Button ID="btnDeleteUser" runat="server" Text="حذف"
                                                CssClass="us-btn-action delete"
                                                CommandName="DeleteUser"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CausesValidation="false"
                                                ToolTip="حذف المستخدم نهائياً"
                                                OnClientClick="return confirm('هل أنت متأكد من حذف هذا المستخدم؟ لا يمكن التراجع عن هذا الإجراء.');" />

                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>
                    </div>
                </div>
            </div>

            <!-- ============ Edit Panel ============ -->
            <asp:Panel ID="pnlEdit" runat="server" Visible="false" CssClass="us-card">
                <div class="us-edit-header">
                    <div class="us-edit-header-icon">
                        <i class="fas fa-user-edit"></i>
                    </div>
                    <div class="us-card-header-text">
                        <h3>تعديل بيانات المستخدم</h3>
                        <p>قم بتحديث بيانات المستخدم والدور المرتبط به</p>
                    </div>
                </div>

                <div class="us-card-body">
                    <asp:HiddenField ID="hfEditId" runat="server" />

                    <div class="us-fields">

                        <div class="us-field">
                            <label><i class="fas fa-user"></i> اسم العرض</label>
                            <asp:TextBox ID="txtEditName" runat="server" CssClass="form-control" MaxLength="100" />
                        </div>

                        <div class="us-field">
                            <label><i class="fas fa-user-tag"></i> الدور</label>
                            <asp:DropDownList ID="ddlEditRole" runat="server" CssClass="form-select">
                                <asp:ListItem Value="Admin" Text="مدير النظام" />
                                <asp:ListItem Value="Accountant" Text="محاسب" />
                                <asp:ListItem Value="Supervisor" Text="مشرف" />
                                <asp:ListItem Value="DataEntry" Text="مدخل بيانات" />
                                <asp:ListItem Value="Teacher" Text="معلم" />
                                <asp:ListItem Value="View" Text="عرض البيانات (View)" />
                            </asp:DropDownList>
                        </div>

                        <div class="us-field" id="editTeacherRow">
                            <label><i class="fas fa-chalkboard-teacher"></i> المعلم المرتبط</label>
                            <asp:DropDownList ID="ddlEditTeacher" runat="server" CssClass="form-select" />
                        </div>

                        <div class="us-field">
                            <label><i class="fas fa-toggle-on"></i> حالة الحساب</label>
                            <div class="us-check">
                                <asp:CheckBox ID="chkEditActive" runat="server" />
                                <label for="<%= chkEditActive.ClientID %>">
                                    <i class="fas fa-check-circle"></i> حساب نشط
                                </label>
                            </div>
                        </div>

                    </div>

                    <div class="d-flex gap-2 no-print">
                        <asp:Button ID="btnEditSave" runat="server" Text="حفظ التعديلات"
                            CssClass="us-btn-primary"
                            OnClick="btnEditSave_Click" />
                        <asp:Button ID="btnEditCancel" runat="server" Text="إلغاء"
                            CssClass="us-btn-outline"
                            OnClick="btnEditCancel_Click"
                            CausesValidation="false" />
                    </div>

                    <asp:Label ID="lblEditMessage" runat="server" CssClass="us-message" style="margin-top:16px;" />
                </div>
            </asp:Panel>

        </div>
    </div>

    <script type="text/javascript">
        function toggleEditTeacherRow() {
            var role = document.getElementById('<%= ddlEditRole.ClientID %>');
            var row = document.getElementById('editTeacherRow');
            if (role && row) row.style.display = role.value === 'Teacher' ? '' : 'none';
        }
        var edd = document.getElementById('<%= ddlEditRole.ClientID %>');
        if (edd) { edd.onchange = toggleEditTeacherRow; window.toggleEditTeacherRow = toggleEditTeacherRow; }
        window.addEventListener('load', toggleEditTeacherRow);
    </script>
</asp:Content>
