<%@ Page Title="المواد الدراسية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Subjects.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Subjects List
           ========================================================= */
        .sb-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .sb-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .sb-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .sb-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .sb-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .sb-hero-left { display: flex; align-items: center; gap: 20px; }
        .sb-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .sb-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .sb-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }
        .sb-stage-label { display: inline-flex; align-items: center; gap: 6px; margin-inline-start: 8px; padding: 3px 12px; border-radius: 999px; background: rgba(255,255,255,0.15); border: 1px solid rgba(255,255,255,0.3); font-size: 12px; font-weight: 700; color: #fff; }
        .sb-stage-label i { font-size: 11px; }
        .sb-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .sb-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .sb-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }
        .sb-btn-glass.primary { background: #fff; color: #00b14f; border-color: #fff; }
        .sb-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .sb-body { max-width: 1280px; margin: -56px auto 0;padding: 0 24px 0; position: relative; z-index: 3; }

        /* ---------- FILTER CARD ---------- */
        .sb-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .sb-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .sb-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .sb-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .sb-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .sb-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .sb-count-pill { margin-right: auto; display: inline-flex; align-items: center; gap: 6px; padding: 6px 14px; border-radius: 999px; font-size: 12px; font-weight: 800; background: rgba(0,177,79,0.1); color: #007a33; border: 1px solid rgba(0,177,79,0.25); flex-shrink: 0; }
        .sb-card-body { padding: 20px 24px; }
        .sb-card-body.no-padding { padding: 0; }

        /* ---------- FILTERS ---------- */
        .sb-filters { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 12px; align-items: end; }
        .sb-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .sb-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .sb-field label i { color: #00b14f; font-size: 12px; }
        .sb-field .form-control,
        .sb-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .sb-field .form-control:focus,
        .sb-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }

        .sb-btn-primary { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 11px 22px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); border: none; color: #fff; font-size: 13px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(0,177,79,0.3); height: 44px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .sb-btn-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }
        .sb-btn-outline { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 11px 22px; border-radius: 11px; background: #fff; border: 1.5px solid #dee2e6; color: #495057; font-size: 13px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 44px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; text-decoration: none; }
        .sb-btn-outline:hover { border-color: #6c757d; background: #f8f9fa; color: #343a40; text-decoration: none; }

        /* ---------- TABLE ---------- */
        .sb-table-wrap { overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .sb-table { width: 100%; margin: 0; border-collapse: separate; border-spacing: 0; font-size: 13px; min-width: 900px; }
        .sb-table thead th { background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%); color: #1f3d33; font-size: 11.5px; font-weight: 800; padding: 14px 12px !important; text-align: center; border-bottom: 2px solid #d8e6dd; white-space: nowrap; }
        .sb-table tbody td { font-size: 13px; padding: 12px 12px !important; vertical-align: middle; border-bottom: 1px solid #f1f3f5; color: #212529; background: #fff; text-align: center; }
        .sb-table tbody tr:hover td { background: #f8fdfa; }
        .sb-table tbody tr:last-child td { border-bottom: none; }

        /* Subject name cell */
        .sb-name-cell { display: flex; align-items: center; gap: 10px; font-weight: 700; color: #212529; text-align: start; }
        .sb-name-icon { width: 34px; height: 34px; border-radius: 10px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 14px; flex-shrink: 0; }

        /* Code badge */
        .sb-code-badge { display: inline-flex; align-items: center; gap: 5px; padding: 4px 12px; border-radius: 8px; font-size: 11.5px; font-weight: 800; background: rgba(13,110,253,0.08); color: #0d6efd; border: 1px solid rgba(13,110,253,0.2); font-family: 'Courier New', monospace; white-space: nowrap; }

        /* Category badge */
        .sb-category-badge { display: inline-flex; align-items: center; gap: 5px; padding: 4px 12px; border-radius: 999px; font-size: 11.5px; font-weight: 700; background: rgba(111,66,193,0.08); color: #6f42c1; border: 1px solid rgba(111,66,193,0.2); white-space: nowrap; }
        .sb-category-badge.basic { background: rgba(0,177,79,0.08); color: #007a33; border-color: rgba(0,177,79,0.2); }

        /* Row number */
        .sb-row-num { display: inline-flex; align-items: center; justify-content: center; width: 30px; height: 30px; border-radius: 9px; background: #f8f9fa; color: #6c757d; font-weight: 800; font-size: 12px; border: 1px solid #e9ecef; }

        /* Description */
        .sb-desc { color: #6c757d; font-size: 12.5px; text-align: start; max-width: 300px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

        /* ---------- ACTION BUTTONS ---------- */
        .sb-btn-action { display: inline-flex; align-items: center; justify-content: center; gap: 4px; padding: 7px 12px; border-radius: 8px; font-size: 12px; font-weight: 700; text-decoration: none; border: 1.5px solid; transition: all 0.15s ease; cursor: pointer; background: #fff; font-family: 'Droid Arabic Kufi', sans-serif; white-space: nowrap; }
        .sb-btn-action.edit { border-color: #00b14f; color: #00b14f; }
        .sb-btn-action.edit:hover { background: rgba(0,177,79,0.08); color: #007a33; text-decoration: none; }
        .sb-btn-action.delete { border-color: #dc3545; color: #dc3545; }
        .sb-btn-action.delete:hover { background: rgba(220,53,69,0.08); color: #a51c1c; text-decoration: none; }

        /* ---------- EMPTY STATE ---------- */
        .sb-empty { text-align: center; padding: 60px 20px; }
        .sb-empty-icon { width: 80px; height: 80px; border-radius: 22px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 36px; margin-bottom: 18px; }
        .sb-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 8px; }
        .sb-empty p { color: #6c757d; font-size: 13px; margin: 0 0 18px; }

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
            .sb-hero, .sb-hero-actions, .sb-btn-glass, .sb-filters, .sb-btn-action, .no-print { display: none !important; }
            .sb-page { background: #fff !important; }
            .sb-body { margin: 0 !important; padding: 0 !important; }
            .sb-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .sb-table { font-size: 10px !important; min-width: 0 !important; }
        }
        .d{
            border: 1px solid #6ddf9f;
             border-radius: 12px;
             padding:20px;
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
                        <i class="fas fa-book-open"></i>
                    </div>
                    <div class="sb-hero-titles">
                        <h1>
                            المواد الدراسية
                            <asp:Panel ID="pnlStageLabel" runat="server" CssClass="sb-stage-label">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:Label ID="CurrentStageLabel" runat="server" />
                            </asp:Panel>
                        </h1>
                        <p>عرض وإدارة المواد الدراسية للمرحلة الحالية</p>
                    </div>
                </div>
                <div class="sb-hero-actions no-print">
                    <asp:Button ID="btnAddSubject" runat="server" Text="إضافة مادة دراسية"
                        CssClass="sb-btn-glass primary"
                        OnClick="btnAddSubject_Click" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="sb-body">

            <!-- Filters -->
            <div class="sb-card no-print">
                <div class="sb-card-header">
                    <div class="sb-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="sb-card-header-text">
                        <h3>معايير البحث</h3>
                        <p>استخدم الفلاتر للبحث عن مادة محددة</p>
                    </div>
                    <span class="sb-count-pill">
                        <i class="fas fa-book-open"></i>
                        <asp:Label ID="lblCount" runat="server" Text="0" />
                    </span>
                </div>
                <div class="sb-card-body">
                    <div class="sb-filters">

                        <div class="sb-field" style="grid-column: span 2;">
                            <label><i class="fas fa-search"></i> البحث بالاسم</label>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                                placeholder="اكتب اسم المادة..." />
                        </div>

                        <asp:Panel ID="pnlCategoryFilter" runat="server" CssClass="sb-field">
                            <label><i class="fas fa-layer-group"></i> الصف / الفرع</label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                                <asp:ListItem Text="كل الفروع" Value="" />
                                <asp:ListItem Text="أول ثانوي" Value="FirstSecondary" />
                                <asp:ListItem Text="ثاني وثالث ثانوي علمي" Value="Scientific" />
                                <asp:ListItem Text="ثاني وثالث ثانوي أدبي" Value="Literary" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="sb-field">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnSearch" runat="server" Text="بحث"
                                CssClass="sb-btn-primary" OnClick="btnSearch_Click" />
                        </div>

                        <div class="sb-field">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnClearSearch" runat="server" Text="مسح البحث"
                                CssClass="sb-btn-outline" OnClick="btnClearSearch_Click" />
                        </div>

                    </div>
                </div>
            </div>

            <!-- Table -->
            <div class="sb-card">
                <div class="sb-card-header">
                    <div class="sb-card-header-icon">
                        <i class="fas fa-list-ul"></i>
                    </div>
                    <div class="sb-card-header-text">
                        <h3>قائمة المواد</h3>
                        <p>جميع المواد الدراسية المسجلة</p>
                    </div>
                </div>

                <div class="sb-card-body no-padding">
                    <div class="sb-table-wrap " style="text-align:center; padding:20px; ">
                        <asp:GridView ID="gvSubjects" runat="server" CssClass="sb-table "
                            AutoGenerateColumns="False"
                            EmptyDataText="" GridLines="None"
                            OnRowCommand="gvSubjects_RowCommand"
                            AllowPaging="True" PageSize="15"
                            OnPageIndexChanging="gvSubjects_PageIndexChanging"
                            ShowHeaderWhenEmpty="true">

                            <Columns>

                                <asp:TemplateField HeaderText="#"  ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="sb-row-num" ><%# Container.DataItemIndex + 1 %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="اسم المادة" >
                                    <ItemTemplate>
                                        <div class="sb-name-cell"  >
                                            <div class="sb-name-icon">
                                                <i class="fas fa-book"></i>
                                            </div>
                                            <span><%# Eval("Name") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الرمز"  ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("Code")))
                                            ? "<span class='text-muted'>—</span>"
                                            : "<span class='sb-code-badge'>" + HttpUtility.HtmlEncode(Convert.ToString(Eval("Code"))) + "</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الصف / الفرع"  ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class='<%# GetCategoryCss(Eval("CategoryName")) %>'>
                                            <%# Eval("CategoryName") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الوصف">
                                    <ItemTemplate>
                                        <div class="sb-desc">
                                            <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("Description")))
                                                ? "—"
                                                : HttpUtility.HtmlEncode(Convert.ToString(Eval("Description"))) %>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="إجراءات"  ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <div class="d-flex gap-1 justify-content-center flex-wrap">
                                            <asp:LinkButton ID="lnkEdit" runat="server"
                                                CommandName="EditSubject"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CssClass="sb-btn-action edit"
                                                ToolTip="تعديل">
                                                <i class="fas fa-edit"></i> تعديل
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="lnkDelete" runat="server"
                                                CommandName="DeleteSubject"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CssClass="sb-btn-action delete"
                                                ToolTip="حذف">
                                                <i class="fas fa-trash"></i> حذف
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                            <PagerStyle CssClass="pagination justify-content-center" />
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="sb-empty">
                        <div class="sb-empty-icon">
                            <i class="fas fa-book-open"></i>
                        </div>
                        <h4>لا توجد نتائج مطابقة</h4>
                        <p>لم يتم العثور على مواد تطابق معايير البحث</p>
                    </asp:Panel>
                </div>
            </div>

        </div>
    </div>
</asp:Content>