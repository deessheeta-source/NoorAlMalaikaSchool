<%@ Page Title="قائمة الطلاب" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentList.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Reports.StudentList" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Students List (Reports)
           ========================================================= */
        .students-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .students-hero {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .students-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .students-hero::after {
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

        .students-hero-content {
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

        .students-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .students-hero-icon {
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
        .students-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .students-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
        }

        .students-hero-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            flex-shrink: 0;
        }

        .students-btn-glass {
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
        .students-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }
        .students-btn-glass-primary {
            background: #fff;
            color: #00b14f;
            border-color: #fff;
        }
        .students-btn-glass-primary:hover {
            background: #f8f9fa;
            color: #007a33;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .students-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .students-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .students-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .students-card-header-icon {
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
        .students-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .students-card-header-icon.blue {
            background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%);
            box-shadow: 0 6px 16px rgba(13,110,253,0.3);
        }
        .students-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 200px;
        }
        .students-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .students-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .students-count-pill {
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

        .students-card-body {
            padding: 20px 24px;
        }

        /* ---------- FILTERS ROW ---------- */
        .students-filters {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
            align-items: end;
            margin-bottom: 20px;
        }
        .students-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }
        .students-field label {
            font-size: 13px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .students-field label i {
            color: #00b14f;
            font-size: 12px;
        }
        .students-field .form-control,
        .students-field .form-select {
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
        .students-field .form-control:focus,
        .students-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- TABLE ---------- */
        .students-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .students-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }
        .students-table thead th {
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
        .students-table tbody td {
            font-size: 13px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: right;
        }
        .students-table tbody tr {
            transition: all 0.15s ease;
        }
        .students-table tbody tr:hover td {
            background: #f8fdfa;
        }
        .students-table tbody tr:last-child td {
            border-bottom: none;
        }

        /* Student name cell */
        .student-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            color: #212529;
            padding: 5px 12px;
        }
        .student-avatar {
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

        /* Registration number badge */
        .reg-no-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-size: 13px;
            font-weight: 700;
        }

        /* Class name badge */
        .class-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            color: #495057;
            font-size: 13px;
            font-weight: 500;
        }

        /* Section badge */
        .section-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-size: 12px;
            font-weight: 500;
        }

        /* Division badge */
        .division-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 11px;
            border-radius: 999px;
            background: #f8f9fa;
            color: #495057;
            font-size: 12px;
            font-weight: 700;
            border: 1px solid #e9ecef;
        }
        .division-badge i {
            color: #00b14f;
            font-size: 10px;
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

        /* National ID cell */
        .national-id {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-weight: 700;
            font-size: 13px;
            color: #495057;
        }

        /* Pagination */
        .students-table .pagination {
            display: flex;
            gap: 6px;
            justify-content: center;
            padding: 16px 0 4px;
            margin: 0;
            list-style: none;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .students-hero { padding: 26px 20px 72px; }
            .students-hero-titles h1 { font-size: 19px; }
            .students-body { padding: 0 16px 40px; }
            .students-filters {
                grid-template-columns: 1fr 1fr;
            }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .students-table { font-size: 11px; min-width: 1100px; }
            .students-table thead th,
            .students-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .students-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .students-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .students-hero-titles h1 { font-size: 16px; }
            .students-hero-titles p { font-size: 12px; }
            .students-btn-glass { padding: 8px 14px; font-size: 12px; }

            .students-filters { grid-template-columns: 1fr; }
        }

        /* =========================================================
           PRINT — إخفاء عناصر التحكم عند الطباعة
           ========================================================= */
        @media print {
            .students-filters,
            .no-print {
                display: none !important;
            }

            .students-hero-actions {
                display: none !important;
            }

            .pagination,
            .students-table .pagination {
                display: none !important;
            }

            .students-page { background: #fff !important; }

            .students-hero {
                background: #00923f !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                border-radius: 0 !important;
                padding: 16px 20px !important;
            }
            .students-body {
                margin-top: 0 !important;
                padding: 0 !important;
            }
            .students-card {
                box-shadow: none !important;
                border: 1px solid #ddd !important;
            }
            .students-table-wrap {
                overflow: visible !important;
                border: 1px solid #ccc !important;
            }
            .students-table {
                min-width: 0 !important;
                font-size: 11px !important;
            }
            .students-table thead th {
                background: #f0f8f3 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            .students-table tbody td { font-size: 11px !important; }

            .students-card-header {
                background: #fff !important;
                border-bottom: 1px solid #ccc !important;
            }

            .students-table tr { page-break-inside: avoid; }
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
    <div class="students-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="students-hero">
            <div class="students-hero-content">
                <div class="students-hero-left">
                    <div class="students-hero-icon">
                        <i class="fas fa-users"></i>
                    </div>
                    <div class="students-hero-titles">
                        <h1>قائمة الطلاب</h1>
                        <p> تقرير بيانات الطلاب </p>
                    </div>
                </div>
                <div class="students-hero-actions">
                    <asp:Button ID="btnExportExcel" runat="server" Text="تصدير اكسل"
                        CssClass="students-btn-glass"
                        OnClick="btnExportExcel_Click" />
                    <asp:Button ID="btnExportWord" runat="server" Text="تصدير ورد"
                        CssClass="students-btn-glass students-btn-glass-primary"
                        OnClick="btnExportWord_Click" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="students-body">

            <div class="students-card">

                <div class="students-card-header">
                    <div class="students-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="students-card-header-text">
                        <div>
                            <h3>بيانات الطلاب</h3>
                        </div>
                    </div>
                    <span class="students-count-pill">
                        <i class="fas fa-users"></i>
                        <asp:Label ID="lblCount" runat="server" Text="0" />
                    </span>
                </div>

                <div class="students-card-body">

                    <!-- Filters (مخفية عند الطباعة) -->
                    <div class="students-filters no-print">

                        <div class="students-field">
                            <label><i class="fas fa-calendar-alt"></i> السنة الدراسية</label>
                            <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <div class="students-field">
                            <label><i class="fas fa-layer-group"></i> الصف</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="الكل" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="students-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الأقسام" Value="0" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="students-field">
                            <label><i class="fas fa-chair"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlClassRoom" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlClassRoom_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="جميع الشعب" />
                            </asp:DropDownList>
                        </div>

                        <div class="students-field">
                            <label><i class="fas fa-link"></i> ربط ولي الأمر</label>
                            <asp:DropDownList ID="ddlParentLink" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlParentLink_SelectedIndexChanged">
                                <asp:ListItem Text="الكل" Value="0" Selected="True" />
                                <asp:ListItem Text="غير مربوط بولي أمر" Value="none" />
                                <asp:ListItem Text="مربوط بولي أمر" Value="linked" />
                            </asp:DropDownList>
                        </div>

                    </div>

                    <!-- Table -->
                    <div class="table-responsive students-table-wrap">
                        <asp:GridView ID="gvStudents" runat="server" CssClass="students-table"
                            AutoGenerateColumns="false" EmptyDataText="لا يوجد طلاب" GridLines="None"
                            AllowPaging="true" PageSize="100"
                            OnPageIndexChanging="gvStudents_PageIndexChanging">

                            <Columns>

                                <asp:TemplateField HeaderText="اسم الطالب">
                                    <ItemTemplate>
                                        <div class="student-name-cell">
                                            <span><%#: Eval("Name") %></span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="رقم التسجيل" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="reg-no-badge">
                                            <%#: Eval("RegistrationNo") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الرقم الوطني" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("NationalId")))
                                            ? "<span class='text-muted'>—</span>"
                                            : "<span class='national-id' dir='ltr'>" + HttpUtility.HtmlEncode(Convert.ToString(Eval("NationalId"))) + "</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الصف" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("ClassName")))
                                            ? "<span class='text-muted'>—</span>"
                                            : "<span class='class-badge'> " + HttpUtility.HtmlEncode(Convert.ToString(Eval("ClassName"))) + "</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="القسم" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# FormatSectionBadge(Eval("Section")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الشعبة" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# FormatDivisionBadge(Eval("DivisionName")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الجنس" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="gender-badge">
                                            <%#: Eval("GenderName") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الجنسية" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="class-badge">
                                            <%#: Eval("Nationality") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="السنة الدراسية" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="class-badge">
                                            <%#: Eval("AcademicYearName") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# Convert.ToString(Eval("StatusName")) == "نشط"
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