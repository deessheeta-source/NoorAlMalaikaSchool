<%@ Page Title="عرض الجداول المدرسية"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="SchoolTimetable.aspx.cs"
    Inherits="NoorAlMalaikaSchool.Pages.Timetable.SchoolTimetable" %>

<asp:Content ID="HeadContent"
    ContentPlaceHolderID="HeadContent"
    runat="server">

    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>"
        rel="stylesheet" />

    <style>
        /* =====================================================
           Premium Design — School Timetable View
           ===================================================== */

        .tt-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
            direction: rtl;
            color: #212529;
        }

        /* ================= HERO ================= */

        .tt-hero {
            position: relative;
            background: linear-gradient(
                135deg,
                #00b14f 0%,
                #00923f 50%,
                #007a33 100%
            );
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }

        .tt-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(
                    circle at 15% 20%,
                    rgba(255,255,255,.12) 0%,
                    transparent 35%
                ),
                radial-gradient(
                    circle at 85% 80%,
                    rgba(255,255,255,.08) 0%,
                    transparent 40%
                );
            pointer-events: none;
        }

        .tt-hero::after {
            content: '';
            position: absolute;
            top: -40%;
            left: -10%;
            width: 340px;
            height: 340px;
            border-radius: 50%;
            background: radial-gradient(
                circle,
                rgba(255,255,255,.15) 0%,
                transparent 70%
            );
            pointer-events: none;
        }

        .tt-hero-content {
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

        .tt-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }

        .tt-hero-icon {
            width: 68px;
            height: 68px;
            border-radius: 20px;
            background: rgba(255,255,255,.18);
            backdrop-filter: blur(10px);
            border: 1.5px solid rgba(255,255,255,.3);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 30px;
            flex-shrink: 0;
            box-shadow: 0 8px 24px rgba(0,0,0,.12);
        }

        .tt-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 8px;
            line-height: 1.5;
        }

        .tt-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,.85);
            margin: 0;
            line-height: 1.8;
        }

        .tt-stage-label {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            margin-inline-start: 8px;
            padding: 3px 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.15);
            border: 1px solid rgba(255,255,255,.3);
            font-size: 12px;
            font-weight: 700;
            color: #fff;
            vertical-align: middle;
        }

        .tt-stage-label i {
            font-size: 11px;
        }

        /* ================= BODY ================= */

        .tt-body {
            max-width: 1280px;
            margin: -56px auto 0;
            padding: 0 24px 40px;
            position: relative;
            z-index: 3;
        }

        /* ================= CARD ================= */

        .tt-card {
            background: #fff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .tt-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 18px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(
                180deg,
                #fff 0%,
                #fbfdfc 100%
            );
            flex-wrap: wrap;
        }

        .tt-card-header-icon {
            width: 44px;
            height: 44px;
            border-radius: 13px;
            background: linear-gradient(
                135deg,
                #00b14f 0%,
                #4ed17e 100%
            );
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            flex-shrink: 0;
            box-shadow: 0 6px 16px rgba(0,177,79,.3);
        }

        .tt-card-header-icon.blue {
            background: linear-gradient(
                135deg,
                #0d6efd 0%,
                #3d8bfd 100%
            );
            box-shadow: 0 6px 16px rgba(13,110,253,.3);
        }

        .tt-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
        }

        .tt-card-header-text p {
            margin: 4px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }

        .tt-card-body {
            padding: 20px 24px;
        }

        /* ================= FILTERS ================= */

        .tt-filters {
            display: grid;
            grid-template-columns: repeat(
                auto-fit,
                minmax(200px, 1fr)
            );
            gap: 16px;
            align-items: end;
        }

        .tt-field {
            display: flex;
            flex-direction: column;
            gap: 7px;
            min-width: 0;
        }

        .tt-field label {
            font-size: 12.5px;
            font-weight: 700;
            color: #495057;
            display: flex;
            align-items: center;
            gap: 7px;
        }

        .tt-field label i {
            color: #00b14f;
            font-size: 12px;
        }

        .tt-field .form-control,
        .tt-field .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 10px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #212529;
            transition: all .2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
            height: 44px;
            width: 100%;
        }

        .tt-field .form-control:focus,
        .tt-field .form-select:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,.12);
            outline: none;
        }

        .tt-field .form-select:disabled {
            background: #f1f3f5;
            color: #868e96;
            cursor: not-allowed;
        }

        /* ================= BUTTONS ================= */

        .tt-filter-actions {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        .tt-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 10px 20px;
            border-radius: 11px;
            background: linear-gradient(
                135deg,
                #00b14f 0%,
                #4ed17e 100%
            );
            border: none;
            color: #fff;
            font-size: 13px;
            font-weight: 800;
            cursor: pointer;
            transition: all .25s ease;
            box-shadow: 0 6px 16px rgba(0,177,79,.3);
            height: 44px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
            text-decoration: none;
        }

        .tt-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 24px rgba(0,177,79,.4);
            color: #fff;
        }

        .tt-btn-outline {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 10px 20px;
            border-radius: 11px;
            background: #fff;
            border: 1.5px solid #dee2e6;
            color: #495057;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            transition: all .2s ease;
            height: 44px;
            white-space: nowrap;
            font-family: 'Droid Arabic Kufi', sans-serif;
            text-decoration: none;
        }

        .tt-btn-outline:hover {
            border-color: #00b14f;
            background: #f0fdf4;
            color: #007a33;
            text-decoration: none;
        }

        /* ================= MESSAGE ================= */

        .tt-message {
            display: block;
            padding: 14px 18px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 16px;
        }

        .tt-message:empty {
            display: none;
            padding: 0;
            margin: 0;
        }

        /* ================= TABLE ================= */

        .tt-table-wrapper {
            width: 100%;
            overflow-x: auto;
            padding: 20px;
        }

        .table-timetable {
            table-layout: fixed;
            width: 100%;
            min-width: 900px;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
            margin: 0;
            background: #fff;
        }

        .table-timetable thead th {
            background: linear-gradient(
                180deg,
                #f8fdfa 0%,
                #f0f8f3 100%
            );
            color: #1f3d33;
            font-weight: 800;
            font-size: 12px;
            padding: 14px 6px;
            border: 1px solid #d8e6dd;
            text-align: center;
            white-space: normal;
        }

        .table-timetable thead th:first-child {
            border-radius: 0 12px 0 0;
            width: 90px;
        }

        .table-timetable thead th:last-child {
            border-radius: 12px 0 0 0;
        }

        .table-timetable tbody th {
            background: #f9fbfa;
            font-size: 13px;
            font-weight: 800;
            color: #007a33;
            padding: 12px 6px;
            border: 1px solid #e0e6ea;
            text-align: center;
            width: 90px;
        }

        .table-timetable tbody td {
            min-width: 100px;
            height: 90px;
            vertical-align: middle;
            padding: 8px 6px;
            border: 1px solid #e0e6ea;
            text-align: center;
            background: #fff;
            overflow-wrap: anywhere;
            transition: background .15s ease;
        }

        .table-timetable tbody tr:last-child th {
            border-radius: 0 0 12px 0;
        }

        .table-timetable tbody tr:last-child td:last-child {
            border-radius: 0 0 0 12px;
        }

        .table-timetable .timetable-cell.has-entry {
            background: linear-gradient(
                135deg,
                #f8fdfa 0%,
                #f0f8f3 100%
            );
        }

        .table-timetable .timetable-cell.has-entry:hover {
            background: #e8f4ec;
        }

        .tt-subject {
            font-weight: 800;
            color: #007a33;
            font-size: 12.5px;
            margin-bottom: 5px;
            line-height: 1.6;
        }

        .tt-teacher {
            font-size: 11px;
            color: #6c757d;
            line-height: 1.6;
        }

        /* ================= EMPTY STATE ================= */

        .tt-empty {
            text-align: center;
            padding: 60px 20px;
        }

        .tt-empty-icon {
            width: 80px;
            height: 80px;
            border-radius: 22px;
            background: linear-gradient(
                135deg,
                #e6f7ec 0%,
                #d0eedd 100%
            );
            color: #00b14f;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            margin-bottom: 18px;
        }

        .tt-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }

        .tt-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0;
            line-height: 1.8;
        }

        /* ================= PRINT ================= */

        .tt-print-heading {
            display: none;
        }

        @media print {
            @page {
                size: landscape;
                margin: 8mm;
            }

            body {
                background: #fff !important;
                -webkit-print-color-adjust: exact !important;
                print-color-adjust: exact !important;
            }

            body * {
                visibility: hidden !important;
            }

            #schoolTimetablePrint,
            #schoolTimetablePrint * {
                visibility: visible !important;
            }

            #schoolTimetablePrint {
                position: absolute !important;
                top: 0 !important;
                right: 0 !important;
                width: 100% !important;
                margin: 0 !important;
                padding: 0 !important;
                background: #fff !important;
            }

            .no-print,
            .tt-hero,
            .tt-filter-card,
            .tt-card-header {
                display: none !important;
            }

            .tt-page,
            .tt-body,
            .tt-card,
            .tt-card-body {
                background: #fff !important;
                box-shadow: none !important;
                border: none !important;
                margin: 0 !important;
                padding: 0 !important;
                max-width: none !important;
                overflow: visible !important;
            }

            .tt-print-heading {
                display: block !important;
                text-align: center;
                margin-bottom: 12px;
                color: #000 !important;
            }

            .tt-print-heading h2 {
                font-size: 20px;
                font-weight: 800;
                margin: 0 0 8px;
            }

            .tt-print-heading h3 {
                font-size: 16px;
                margin: 0 0 8px;
            }

            .tt-print-heading p {
                font-size: 12px;
                margin: 0;
            }

            .tt-table-wrapper {
                overflow: visible !important;
                padding: 0 !important;
                width: 100% !important;
            }

            .table-timetable {
                width: 100% !important;
                min-width: 0 !important;
                table-layout: fixed !important;
                border-collapse: collapse !important;
                font-size: 10px !important;
            }

            .table-timetable thead th,
            .table-timetable tbody th,
            .table-timetable tbody td {
                border: 1px solid #333 !important;
                color: #000 !important;
                padding: 6px 3px !important;
                overflow-wrap: anywhere !important;
            }

            .table-timetable thead th {
                background: #e9f5ed !important;
                font-size: 10px !important;
            }

            .table-timetable tbody th {
                background: #f2f6f3 !important;
                font-size: 10px !important;
            }

            .table-timetable tbody td {
                height: 60px !important;
                font-size: 9px !important;
            }

            .tt-subject {
                color: #000 !important;
                font-size: 10px !important;
                font-weight: 800 !important;
            }

            .tt-teacher {
                color: #333 !important;
                font-size: 9px !important;
            }

            .table-timetable .timetable-cell.has-entry {
                background: #f3faf5 !important;
            }
        }

        /* ================= RESPONSIVE ================= */

        @media (max-width: 992px) {
            .tt-hero {
                padding: 26px 20px 72px;
            }

            .tt-hero-titles h1 {
                font-size: 19px;
            }

            .tt-body {
                padding: 0 16px 40px;
            }
        }

        @media (max-width: 768px) {
            .tt-filters {
                grid-template-columns: 1fr;
            }

            .tt-card-body {
                padding: 16px;
            }

            .tt-hero {
                padding: 24px 16px 70px;
            }

            .tt-hero-icon {
                width: 56px;
                height: 56px;
                font-size: 24px;
            }

            .tt-hero-titles h1 {
                font-size: 17px;
            }

            .tt-table-wrapper {
                padding: 12px;
            }

            .tt-filter-actions {
                width: 100%;
            }

            .tt-btn-outline {
                flex: 1;
            }
        }
    </style>
</asp:Content>


<asp:Content ID="MainContentBlock"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="tt-page" dir="rtl">

        <!-- ================= HERO ================= -->

        <div class="tt-hero no-print">
            <div class="tt-hero-content">

                <div class="tt-hero-left">

                    <div class="tt-hero-icon">
                        <i class="fas fa-calendar-alt"></i>
                    </div>

                    <div class="tt-hero-titles">
                        <h1>
                            الجداول المدرسية

                            <asp:Panel
                                ID="pnlStageLabel"
                                runat="server"
                                CssClass="tt-stage-label">

                                <i class="fas fa-graduation-cap"></i>

                                <asp:Label
                                    ID="lblStage"
                                    runat="server" />

                            </asp:Panel>
                        </h1>

                        <p>
                            عرض الجدول الأسبوعي حسب الصف والقسم والشعبة
                        </p>
                    </div>

                </div>

            </div>
        </div>


        <!-- ================= BODY ================= -->

        <div class="tt-body">

            <!-- رسائل النظام -->

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="tt-message"
                EnableViewState="false" />


            <!-- ================= FILTER CARD ================= -->

            <div class="tt-card tt-filter-card no-print">

                <div class="tt-card-header">

                    <div class="tt-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>

                    <div class="tt-card-header-text">
                        <h3>تحديد الجدول الدراسي</h3>

                        <p>
                            اختر الصف الدراسي والقسم والشعبة لعرض الجدول الأسبوعي
                        </p>
                    </div>

                </div>


                <div class="tt-card-body">

                    <div class="tt-filters">

                        <!-- الصف الدراسي -->

                        <div class="tt-field">

                            <label for="<%= ddlGradeLevel.ClientID %>">
                                <i class="fas fa-graduation-cap"></i>
                                الصف الدراسي
                            </label>

                            <asp:DropDownList
                                ID="ddlGradeLevel"
                                runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged" />

                        </div>


                        <!-- القسم -->

                        <asp:Panel
                            ID="pnlSectionFilter"
                            runat="server"
                            CssClass="tt-field">

                            <label for="<%= ddlSection.ClientID %>">
                                <i class="fas fa-stream"></i>
                                القسم
                            </label>

                            <asp:DropDownList
                                ID="ddlSection"
                                runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">

                                <asp:ListItem
                                    Text="-- اختر القسم --"
                                    Value="" />

                            </asp:DropDownList>

                        </asp:Panel>


                        <!-- الشعبة -->

                        <div class="tt-field">

                            <label for="<%= ddlDivision.ClientID %>">
                                <i class="fas fa-users"></i>
                                الشعبة
                            </label>

                            <asp:DropDownList
                                ID="ddlDivision"
                                runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">

                                <asp:ListItem
                                    Text="-- اختر الشعبة --"
                                    Value="" />

                            </asp:DropDownList>

                        </div>


                        <!-- الطباعة -->

                        <div class="tt-field">

                            <label>&nbsp;</label>

                            <div class="tt-filter-actions">

                                <button
                                    type="button"
                                    class="tt-btn-outline"
                                    onclick="window.print();">

                                    <i class="fas fa-print"></i>
                                    طباعة الجدول

                                </button>

                            </div>

                        </div>

                    </div>

                </div>

            </div>


            <!-- ================= TIMETABLE CARD ================= -->

            <div class="tt-card">

                <div class="tt-card-header no-print">

                    <div class="tt-card-header-icon">
                        <i class="fas fa-calendar-check"></i>
                    </div>

                    <div class="tt-card-header-text">

                        <h3>الجدول الأسبوعي</h3>

                        <p>
                            الحصص الدراسية من الأحد إلى الخميس
                        </p>

                    </div>

                </div>


                <div class="tt-card-body" style="padding:0;">

                    <!-- منطقة الطباعة -->

                    <div id="schoolTimetablePrint">

                        <div class="tt-print-heading">

                            <h2>مدرسة نور الملائكة</h2>

                            <h3>الجدول الدراسي</h3>

                            <p>
                                <asp:Literal
                                    ID="litPrintDetails"
                                    runat="server" />
                            </p>

                        </div>


                        <div class="tt-table-wrapper">

                            <asp:Literal
                                ID="litTimetable"
                                runat="server" />

                        </div>

                    </div>

                </div>

            </div>


            <!-- ================= INFORMATION ================= -->

            <div class="tt-card no-print">

                <div class="tt-card-body"
                    style="display:flex;align-items:center;gap:12px;">

                    <div class="tt-card-header-icon">
                        <i class="fas fa-info-circle"></i>
                    </div>

                    <div class="tt-card-header-text">

                        <h3>معلومات الصفحة</h3>

                        <p>
                            هذه الصفحة مخصصة لعرض الجداول المدرسية وطباعتها فقط،
                            ولا تتيح إضافة الحصص أو تعديلها أو حذفها.
                        </p>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>