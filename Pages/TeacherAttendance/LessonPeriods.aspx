<%@ Page Title="أوقات الحصص" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LessonPeriods.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.LessonPeriods" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Lesson Periods
           ========================================================= */
        .ta-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .ta-hero-section {
            position: relative;
            background: linear-gradient(135deg, #969899 0%, #878e94 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .ta-hero-section::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .ta-hero-section::after {
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

        .ta-hero-content {
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

        .ta-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .ta-hero-icon {
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
        .ta-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .ta-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
            max-width: 620px;
        }
        .ta-hero-titles p strong {
            color: #fff;
            font-weight: 800;
        }

        .ta-btn-glass {
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
            flex-shrink: 0;
        }
        .ta-btn-glass:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .ta-body {
            max-width: 1280px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- ALERT ---------- */
        .ta-alert {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 14px 20px;
            border-radius: 14px;
            margin-bottom: 20px;
            font-size: 13.5px;
            font-weight: 700;
            box-shadow: 0 8px 24px rgba(0,0,0,0.06);
            animation: slideDown 0.4s ease;
        }
        .ta-alert i {
            width: 36px;
            height: 36px;
            border-radius: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
            color: #fff;
        }
        .ta-alert.success {
            background: linear-gradient(180deg, #ffffff 0%, #f8fdfa 100%);
            border: 1.5px solid rgba(0,177,79,0.35);
            color: #1f3d33;
        }
        .ta-alert.success i { background: linear-gradient(135deg, #00b14f, #4ed17e); }

        .ta-alert.danger {
            background: linear-gradient(180deg, #ffffff 0%, #fef8f8 100%);
            border: 1.5px solid rgba(220,53,69,0.35);
            color: #7a1a26;
        }
        .ta-alert.danger i { background: linear-gradient(135deg, #dc3545, #ff6b7a); }

        @keyframes slideDown {
            from { opacity: 0; transform: translateY(-10px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        /* ---------- INFO NOTE ---------- */
        .ta-note {
            display: flex;
            align-items: flex-start;
            gap: 12px;
            padding: 14px 18px;
            background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 14px;
            margin-bottom: 20px;
            font-size: 13px;
            color: #1f3d33;
            line-height: 1.7;
            font-weight: 500;
        }
        .ta-note i {
            color: #00b14f;
            font-size: 18px;
            flex-shrink: 0;
            margin-top: 2px;
        }
        .ta-note strong {
            color: #007a33;
            font-weight: 800;
        }

        /* ---------- CARD ---------- */
        .ta-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .ta-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
            flex-wrap: wrap;
        }
        .ta-card-header-icon {
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
        .ta-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .ta-card-header-icon.teal {
            background: linear-gradient(135deg, #0dcaf0 0%, #63e1f5 100%);
            box-shadow: 0 6px 16px rgba(13,202,240,0.3);
        }
        .ta-card-header-text {
            display: flex;
            align-items: center;
            gap: 12px;
            flex: 1;
            min-width: 200px;
        }
        .ta-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .ta-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }
        .ta-count-pill {
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

        .ta-card-body {
            padding: 16px;
        }

        /* ---------- TABLE ---------- */
        .ta-table-wrap {
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid #eef1f3;
        }
        .ta-table {
            width: 100%;
            margin: 0;
            border-collapse: separate;
            border-spacing: 0;
        }
        .ta-table thead th {
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
        .ta-table tbody td {
            font-size: 12.5px;
            padding: 14px 10px !important;
            vertical-align: middle;
            border-bottom: 1px solid #f1f3f5;
            color: #212529;
            background: #fff;
            text-align: center;
        }
        .ta-table tbody tr {
            transition: all 0.15s ease;
        }
        .ta-table tbody tr:hover td {
            background: #f8fdfa;
        }
        .ta-table tbody tr:last-child td {
            border-bottom: none;
        }
        th {
          text-align: center;
          
        }

        /* Period number badge */
        .ta-period-badge {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 40px;
            height: 40px;
            border-radius: 11px;
            background: linear-gradient(135deg, rgba(0,177,79,0.12) 0%, rgba(0,177,79,0.06) 100%);
            color: #00b14f;
            font-weight: 800;
            font-size: 15px;
            border: 1px solid rgba(0,177,79,0.2);
            font-family: 'Courier New', monospace;
        }

        /* Time badge */
        .ta-time-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 12px;
            border-radius: 9px;
            background: #f8f9fa;
            border: 1px solid #eef1f3;
            font-size: 12px;
            font-weight: 700;
            color: #495057;
            font-family: 'Courier New', monospace;
            direction: ltr;
        }
        .ta-time-badge i {
            font-size: 10px;
            color: #00b14f;
        }

        /* Duration badge */
        .ta-duration-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 9px;
            background: linear-gradient(135deg, rgba(13,110,253,0.1), rgba(13,110,253,0.05));
            color: #0d6efd;
            font-size: 12px;
            font-weight: 800;
            border: 1px solid rgba(13,110,253,0.15);
            font-family: 'Courier New', monospace;
        }
        .ta-duration-badge small {
            font-size: 10px;
            color: #4a6fa5;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }

        /* Status badges */
        .badge-green {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            background: rgba(0,177,79,0.12);
            color: #007a33;
            border: 1px solid rgba(0,177,79,0.25);
        }
        .badge-red {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 5px 12px;
            border-radius: 999px;
            font-size: 11.5px;
            font-weight: 700;
            background: rgba(220,53,69,0.12);
            color: #a51c1c;
            border: 1px solid rgba(220,53,69,0.25);
        }

        /* Action buttons */
        .ta-action-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            padding: 8px 16px;
            border-radius: 9px;
            font-size: 12px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.15s ease;
            text-decoration: none;
            border: 1.5px solid transparent;
            white-space: nowrap;
            background: #fff;
        }
        .ta-action-btn i { font-size: 11px; }

        .ta-action-btn.edit {
            color: #00b14f;
            border-color: rgba(0,177,79,0.3);
        }
        .ta-action-btn.edit:hover {
            background: #00b14f;
            color: #fff;
            text-decoration: none;
            box-shadow: 0 4px 10px rgba(0,177,79,0.25);
            transform: translateY(-1px);
        }
        .ta-action-btn.save {
            color: #fff;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            box-shadow: 0 4px 10px rgba(0,177,79,0.25);
        }
        .ta-action-btn.save:hover {
            box-shadow: 0 6px 16px rgba(0,177,79,0.35);
            transform: translateY(-1px);
            color: #fff;
            text-decoration: none;
        }
        .ta-action-btn.cancel {
            color: #dc3545;
            border-color: rgba(220,53,69,0.3);
        }
        .ta-action-btn.cancel:hover {
            background: rgba(220,53,69,0.08);
            text-decoration: none;
            color: #dc3545;
        }

        /* Form controls in edit mode */
        .ta-table .form-control,
        .ta-table .form-select {
            border: 1.5px solid #e9ecef;
            border-radius: 9px;
            font-size: 12.5px;
            font-weight: 600;
            padding: 8px 12px;
            background: #fff;
            transition: all 0.15s ease;
        }
        .ta-table .form-control:focus,
        .ta-table .form-select:focus {
            border-color: #00b14f;
            box-shadow: 0 0 0 3px rgba(0,177,79,0.12);
        }

        /* Edit row highlight */
        .ta-table tr.grid-edit-row td {
            background: #fffbeb !important;
            border-bottom: 1px solid #fef3c7 !important;
        }
        .ta-table tr.grid-edit-row td:first-child {
            border-right: 3px solid #fd7e14;
        }

        /* ---------- EMPTY STATE ---------- */
        .ta-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .ta-empty-icon {
            width: 80px;
            height: 80px;
            border-radius: 22px;
            background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%);
            color: #00b14f;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            margin-bottom: 18px;
        }
        .ta-empty h4 {
            color: #1f3d33;
            font-weight: 800;
            font-size: 16px;
            margin: 0 0 8px;
        }
        .ta-empty p {
            color: #6c757d;
            font-size: 13px;
            margin: 0;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .ta-hero-section { padding: 26px 20px 72px; }
            .ta-hero-titles h1 { font-size: 19px; }
            .ta-body { padding: 0 16px 40px; }
        }

        @media (max-width: 768px) {
            .table-responsive { overflow-x: auto; -webkit-overflow-scrolling: touch; }
            .ta-table { font-size: 11px; min-width: 700px; }
            .ta-table thead th,
            .ta-table tbody td { padding: 10px 6px !important; }
        }

        @media (max-width: 576px) {
            .ta-hero-section { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .ta-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .ta-hero-titles h1 { font-size: 16px; }
            .ta-hero-titles p { font-size: 12px; }
            .ta-btn-glass { padding: 8px 14px; font-size: 12px; }
            .ta-btn-glass span { display: none; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .ta-hero-section,
            .ta-btn-glass,
            .ta-action-btn,
            .no-print { display: none !important; }
            .ta-body { margin: 0 !important; padding: 0 !important; }
            .ta-card { box-shadow: none !important; border: 1px solid #ddd !important; }
            .ta-table { font-size: 10.5px !important; }
            .table-responsive { overflow: visible !important; }
            body { background: #fff !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ta-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="ta-hero-section">
            <div class="ta-hero-content">
                <div class="ta-hero-left">
                    <div class="ta-hero-icon">
                        <i class="fas fa-clock"></i>
                    </div>
                    <div class="ta-hero-titles">
                        <h1>أوقات الحصص الدراسية</h1>
                        <p>
                            مدة الحصة الافتراضية <strong>45 دقيقة</strong> — يمكن تعديل أوقات البداية والنهاية من هنا
                        </p>
                    </div>
                </div>
                <a href="Home.aspx" class="ta-btn-glass">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة</span>
                </a>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="ta-body">

            <!-- Alert -->
            <asp:Label ID="lblMessage" runat="server" CssClass="d-none" />

            <!-- Info Note -->
            <div class="ta-note">
                <i class="fas fa-info-circle"></i>
                <span>
                    اضبط أوقات بداية ونهاية كل حصة بالصيغة <strong>HH:mm</strong> (مثل <strong>07:30</strong>).
                    سيتم حساب المدة تلقائياً. يمكنك تعطيل حصة مؤقتاً دون حذفها عبر خيار <strong>«موقوفة»</strong>.
                </span>
            </div>

            <!-- Periods Table Card -->
            <div class="ta-card">
                <div class="ta-card-header">
                    <div class="ta-card-header-icon teal">
                        <i class="fas fa-clock"></i>
                    </div>
                    <div class="ta-card-header-text">
                        <div>
                            <h3>جدول أوقات الحصص</h3>
                            <p>قائمة الحصص الدراسية اليومية وأوقاتها</p>
                        </div>
                    </div>
                    <span class="ta-count-pill">
                        <i class="fas fa-list-ol"></i>
                        <asp:Label ID="lblPeriodsCount" runat="server" Text="0" /> حصة
                    </span>
                </div>
                <div class="ta-card-body">

                    <div class="table-responsive ta-table-wrap">
                        <asp:GridView ID="gvPeriods" runat="server"
                            CssClass="ta-table"
                            AutoGenerateColumns="false"
                            EmptyDataText=""
                            GridLines="None"
                            ShowHeaderWhenEmpty="true"
                            OnRowEditing="gvPeriods_RowEditing"
                            OnRowUpdating="gvPeriods_RowUpdating"
                            OnRowCancelingEdit="gvPeriods_RowCancelingEdit"
                            DataKeyNames="Id">
                            <Columns>

                                <asp:TemplateField HeaderText="رقم الحصة" ItemStyle-Width="110px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="ta-period-badge"><%# Eval("PeriodNumber") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="وقت البداية" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <span class="ta-time-badge">
                                            <i class="fas fa-play"></i>
                                            <%# Eval("StartTimeText") %>
                                        </span>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtStart" runat="server"
                                            Text='<%# Eval("StartTimeText") %>'
                                            CssClass="form-control form-control-sm"
                                            dir="ltr"
                                            placeholder="HH:mm" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="وقت النهاية" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <span class="ta-time-badge">
                                            <i class="fas fa-stop"></i>
                                            <%# Eval("EndTimeText") %>
                                        </span>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtEnd" runat="server"
                                            Text='<%# Eval("EndTimeText") %>'
                                            CssClass="form-control form-control-sm"
                                            dir="ltr"
                                            placeholder="HH:mm" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="المدة" ItemStyle-Width="130px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="ta-duration-badge">
                                            <i class="fas fa-hourglass-half"></i>
                                            <%# Eval("DurationMinutes") %>
                                            <small>دقيقة</small>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="الحالة" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <%# (bool)Eval("IsActive")
                                            ? "<span class='badge-green'><i class='fas fa-check-circle' style='font-size:10px;'></i> نشطة</span>"
                                            : "<span class='badge-red'><i class='fas fa-pause-circle' style='font-size:10px;'></i> موقوفة</span>" %>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlActiveEdit" runat="server" CssClass="form-select form-select-sm">
                                            <asp:ListItem Text="نشطة" Value="True" />
                                            <asp:ListItem Text="موقوفة" Value="False" />
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="إجراءات" ItemStyle-Width="180px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEdit" runat="server"
                                            CommandName="Edit"
                                            CssClass="ta-action-btn edit">
                                            <i class="fas fa-edit"></i> تعديل
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <div class="d-flex gap-1 justify-content-center">
                                            <asp:LinkButton ID="lnkSave" runat="server"
                                                CommandName="Update"
                                                CssClass="ta-action-btn save">
                                                <i class="fas fa-save"></i> حفظ
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkCancel" runat="server"
                                                CommandName="Cancel"
                                                CssClass="ta-action-btn cancel">
                                                <i class="fas fa-times"></i> إلغاء
                                            </asp:LinkButton>
                                        </div>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="ta-empty">
                        <div class="ta-empty-icon">
                            <i class="fas fa-clock"></i>
                        </div>
                        <h4>لا توجد حصص مضافة</h4>
                        <p>لم يتم إعداد أوقات الحصص بعد</p>
                    </asp:Panel>

                </div>
            </div>

        </div>
    </div>
</asp:Content>