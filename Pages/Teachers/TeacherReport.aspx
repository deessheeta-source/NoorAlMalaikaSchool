<%@ Page Title="تقرير المعلمين والمعلمات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TeacherReport.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Teachers.TeacherReport" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Teachers Report
           ========================================================= */
        .teachers-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .teachers-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .teachers-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .teachers-hero::after {
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

        .teachers-hero-content {
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

        .teachers-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .teachers-hero-icon {
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
        .teachers-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .teachers-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }

        .teachers-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }

        .teachers-btn-glass {
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
        .teachers-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .teachers-btn-glass-primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .teachers-btn-glass-primary:hover {
            background: #f8f9fa;
            color: #007a33;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .teachers-body {
            max-width: 1280px;
            margin: -56px auto 0;
            padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .teachers-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .teachers-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .teachers-card-header-icon {
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
        .teachers-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .teachers-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .teachers-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 200px;
        }
        .teachers-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .teachers-count-pill {
            margin-right: auto;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
            background: rgba(0,177,79,0.1);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
            flex-shrink: 0;
        }

        .teachers-card-body {
            padding: 20px 24px;
        }

        /* ---------- FILTERS ROW ---------- */
        .teachers-filters {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
            align-items: end;
            margin-bottom: 20px;
        }
        .teachers-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .teachers-field label {
            font-size: 13px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .teachers-field label i {
            color: #00b14f;
            font-size: 12px;
        }
        .teachers-field .form-control,
        .teachers-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 12px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 42px;
        }
        .teachers-field .form-control:focus,
        .teachers-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- TABLE ---------- */
        .teachers-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .teachers-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .teachers-table thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33;
            font-size: 11.5px;
            font-weight: 800;
            padding: 14px 10px !important;
            text-align: right;
            border-bottom: 2px solid #d8e6dd;
            white-space: nowrap;
            letter-spacing: 0.2px;
        }
        .teachers-table tbody td {
            font-size: 13px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .teachers-table tbody tr {
            transition: all 0.15s ease;
        }
        .teachers-table tbody tr:hover td {
            background: #f8fdfa;
        }
        .teachers-table tbody tr:last-child td {
            border-bottom: none;
        }

        /* Teacher name cell */
        .teacher-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            color: #212529;
            padding: 5px 12px;
        }
        .teacher-avatar {
            width: 34px;
            height: 34px;
            border-radius: 10px;
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            color: #00b14f;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            flex-shrink: 0;
        }

        /* Specialty badge */
        .spec-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-size: 13px;
            font-weight: 700;
            color: #0d6efd;
        }

        /* Gender badge */
        .gender-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            color: #495057;
            font-size: 13px;
            font-weight: 500;
        }

        /* Phone cell */
        .phone-cell {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-weight: 600;
            font-size: 13px;
            color: #495057;
        }

        /* Status badge */
        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
        }
        .status-badge.active {
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .status-badge.inactive {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }

        /* Pagination */
        .teachers-table .pagination {
            display: flex;
            gap: 6px;
            justify-content: center;
            padding: 16px 0 4px;
            margin: 0;
            list-style: none;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .teachers-hero { padding: 26px 20px 72px; }
            .teachers-hero-titles h1 { font-size: 19px; }
            .teachers-body { padding: 0 16px 40px; }
            .teachers-filters {
                grid-template-columns: 1fr 1fr;
            }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .teachers-table { font-size: 11px; min-width: 800px; }
            .teachers-table thead th,
            .teachers-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .teachers-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .teachers-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .teachers-hero-titles h1 { font-size: 16px; }
            .teachers-hero-titles p { font-size: 12px; }
            .teachers-btn-glass { padding: 8px 14px; font-size: 12px; }

            .teachers-filters { grid-template-columns: 1fr; }
        }

        /* =========================================================
           PRINT — إخفاء عناصر التحكم عند الطباعة
           ========================================================= */
        @media print {
            .teachers-filters,
            .no-print {
                display: none !important;
            }

            .teachers-hero-actions {
                display: none !important;
            }

            .pagination,
            .teachers-table .pagination {
                display: none !important;
            }

            .teachers-page { background: #fff !important; }

            .teachers-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .teachers-body {
                margin-top: 0 !important;
                padding: 0 !important;
            }
            .teachers-card {
                box-shadow: none !important;
                border: 1px solid #ddd !important;
            }
            .teachers-table-wrap {
                overflow: visible !important;
                border: 1px solid #ccc !important;
            }
            .teachers-table {
                min-width: 0 !important;
                font-size: 11px !important;
            }
            .teachers-table thead th {
                background: #f0f8f3 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .teachers-table tbody td { font-size: 11px !important; }

            .teachers-card-header {
                background: #fff !important;
                border-bottom: 1px solid #ccc !important;
            }

            .teachers-table tr { page-break-inside: avoid; }
        }

        tbody, td, tfoot, th, thead, tr {
            border-color: inherit;
            border-style: solid;
            border-width: 0;
            padding: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="teachers-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="teachers-hero">
            <div class="teachers-hero-content">
                <div class="teachers-hero-left">
                    <div class="teachers-hero-icon">
                        <i class="fas fa-chalkboard-teacher"></i>
                    </div>
                    <div class="teachers-hero-titles">
                        <h1>تقرير المعلمين والمعلمات</h1>
                        <p> تقرير بيانات الهيئة التعليمية </p>
                    </div>
                </div>
                <div class="teachers-hero-actions">
                    <asp:Button ID="btnExportExcel" runat="server" Text="تصدير اكسل"
                        CssClass="teachers-btn-glass"
                        OnClick="btnExportExcel_Click" />
                    <asp:Button ID="btnExportWord" runat="server" Text="تصدير ورد"
                        CssClass="teachers-btn-glass teachers-btn-glass-primary"
                        OnClick="btnExportWord_Click" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="teachers-body">

            <div class="teachers-card">

                <div class="teachers-card-header">
                    <div class="teachers-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="teachers-card-header-text">
                        <div>
                            <h3>بيانات المعلمين والمعلمات</h3>
                        </div>
                    </div>
                    <span class="teachers-count-pill">
                        <i class="fas fa-users"></i>
                        <asp:Label ID="lblCount" runat="server" Text="0" />
                    </span>
                </div>

                <div class="teachers-card-body">

                    <!-- Filters (مخفية عند الطباعة) -->
                    <div class="teachers-filters no-print">

                        <div class="teachers-field">
                            <label><i class="fas fa-search"></i> بحث بالاسم</label>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                                AutoPostBack="true"
                                OnTextChanged="txtSearch_TextChanged"
                                placeholder="اكتب اسم المعلم..." />
                        </div>

                        <div class="teachers-field">
                            <label><i class="fas fa-book"></i> التخصص</label>
                            <asp:DropDownList ID="ddlSpecialty" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSpecialty_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="جميع التخصصات" />
                            </asp:DropDownList>
                        </div>

                        <div class="teachers-field">
                            <label><i class="fas fa-venus-mars"></i> الجنس</label>
                            <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGender_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" Selected="True" />
                                <asp:ListItem Value="Male" Text="معلم" />
                                <asp:ListItem Value="Female" Text="معلمة" />
                            </asp:DropDownList>
                        </div>

                        <div class="teachers-field">
                            <label><i class="fas fa-toggle-on"></i> الحالة</label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" Selected="True" />
                                <asp:ListItem Value="1" Text="نشط" />
                                <asp:ListItem Value="0" Text="غير نشط" />
                            </asp:DropDownList>
                        </div>

                    </div>

                    <!-- Table -->
                    <div class="table-responsive teachers-table-wrap">
                        <asp:GridView ID="gvTeachers" runat="server" CssClass="teachers-table"
                            AutoGenerateColumns="false" EmptyDataText="لا يوجد معلمون" GridLines="None"
                            AllowPaging="true" PageSize="100"
                            OnPageIndexChanging="gvTeachers_PageIndexChanging">

                            <Columns>

                                <asp:TemplateField HeaderText="الاسم">
                                    <ItemTemplate>
                                        <div class="teacher-name-cell">
                                            <span><%#: Eval("Name") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الجنس" >
                                    <ItemTemplate>
                                        <span class="gender-badge">
                                            <%#: Eval("GenderName") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="التخصص" >
                                    <ItemTemplate>
                                        <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("Specialty")))
                                            ? "<span class='text-muted'>—</span>"
                                            : "<span class='spec-badge'>" + HttpUtility.HtmlEncode(Convert.ToString(Eval("Specialty"))) + "</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الهاتف" >
                                    <ItemTemplate>
                                        <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("Phone")))
                                            ? "<span class='text-muted'>—</span>"
                                            : "<span class='phone-cell' dir='ltr'>" + HttpUtility.HtmlEncode(Convert.ToString(Eval("Phone"))) + "</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة" >
                                    <ItemTemplate>
                                        <%# Convert.ToBoolean(Eval("IsActive"))
                                            ? "<span class='status-badge active'><i class='fas fa-check-circle' style='font-size:10px;'></i> نشط</span>"
                                            : "<span class='status-badge inactive'><i class='fas fa-pause-circle' style='font-size:10px;'></i> غير نشط</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                            <PagerStyle CssClass="pagination justify-content-center" />
                        </asp:GridView>
                    </div>

                </div>
            </div>

        </div>
    </div>
</asp:Content>