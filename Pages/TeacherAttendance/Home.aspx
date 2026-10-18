<%@ Page Title="حضور المعلمين" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Home.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.TeacherAttendance.Home" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Dashboard — Teacher Attendance
           ========================================================= */
        .home-wrap {
            padding: 0;
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
        }

        /* ---------- HERO HEADER ---------- */
        .hero-section {
            position: relative;
            background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%);
            padding: 36px 32px 84px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .hero-section::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .hero-section::after {
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

        .hero-content {
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

        .hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .hero-icon {
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
        .hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .hero-titles p {
            font-size: 13.5px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            display: flex;
            align-items: center;
            gap: 8px;
            flex-wrap: wrap;
        }

        .hero-chip {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 5px 14px;
            border-radius: 999px;
            background: rgba(255,255,255,0.22);
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255,255,255,0.3);
            color: #fff;
            font-weight: 700;
            font-size: 12px;
            letter-spacing: 0.2px;
        }

        .hero-right {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }
        .hero-stat {
            background: rgba(255,255,255,0.15);
            backdrop-filter: blur(12px);
            border: 1px solid rgba(255,255,255,0.25);
            border-radius: 14px;
            padding: 12px 20px;
            min-width: 110px;
            text-align: center;
            color: #fff;
        }
        .hero-stat .stat-label {
            font-size: 11px;
            font-weight: 600;
            opacity: 0.85;
            margin-bottom: 4px;
            letter-spacing: 0.3px;
        }
        .hero-stat .stat-value {
            font-size: 20px;
            font-weight: 800;
            line-height: 1;
        }
        .hero-stat .stat-value small {
            font-size: 11px;
            font-weight: 600;
            opacity: 0.8;
            margin-right: 2px;
        }

        /* ---------- BODY CONTAINER ---------- */
        .body-container {
            max-width: 1280px;
            margin: -60px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- FLASH ---------- */
        .flash-card {
            background: linear-gradient(180deg, #ffffff 0%, #f8fdfa 100%);
            border: 1.5px solid rgba(0,177,79,0.35);
            border-radius: 14px;
            padding: 14px 20px;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 12px;
            box-shadow: 0 8px 24px rgba(0,177,79,0.1);
            animation: slideDown 0.4s ease;
        }
        .flash-card i {
            width: 36px;
            height: 36px;
            border-radius: 10px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
        }
        .flash-card span {
            color: #1f3d33;
            font-weight: 700;
            font-size: 13.5px;
        }
        @keyframes slideDown {
            from { opacity: 0; transform: translateY(-10px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        /* ---------- TODAY PANEL ---------- */
        .today-panel {
            background: linear-gradient(135deg, #ffffff 0%, #f8fdfa 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 20px;
            padding: 22px 26px;
            margin-bottom: 28px;
            display: flex;
            align-items: center;
            gap: 20px;
            flex-wrap: wrap;
            box-shadow: 0 8px 24px rgba(0,177,79,0.06);
            position: relative;
            overflow: hidden;
        }
        .today-panel::before {
            content: '';
            position: absolute;
            top: 0;
            right: 0;
            width: 6px;
            height: 100%;
            background: linear-gradient(180deg, #00b14f 0%, #4ed17e 100%);
        }
        .today-panel .today-icon {
            width: 54px;
            height: 54px;
            border-radius: 14px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
            flex-shrink: 0;
            box-shadow: 0 6px 14px rgba(0,177,79,0.3);
        }
        .today-panel .today-content {
            display: flex;
            flex-direction: column;
            gap: 4px;
            flex: 1;
            min-width: 200px;
        }
        .today-panel .today-date {
            font-size: 12px;
            font-weight: 700;
            color: #00b14f;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .today-panel .today-summary {
            font-size: 15px;
            font-weight: 700;
            color: #212529;
            line-height: 1.5;
        }
        .today-panel .today-date-value {
            font-size: 20px;
            font-weight: 800;
            color: #212529;
        }

        /* ---------- SECTION ---------- */
        .home-section {
            margin-bottom: 36px;
        }
        .section-header {
            display: flex;
            align-items: center;
            gap: 14px;
            margin-bottom: 20px;
        }
        .section-header-icon {
            width: 44px;
            height: 44px;
            border-radius: 13px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            color: #fff;
            font-size: 18px;
            flex-shrink: 0;
            position: relative;
        }
        .section-header-icon.daily {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            box-shadow: 0 6px 16px rgba(0,177,79,0.3);
        }
        .section-header-icon.monthly {
            background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%);
            box-shadow: 0 6px 16px rgba(111,66,193,0.3);
        }
        .section-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .section-header-text h3 {
            margin: 0;
            font-size: 17px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .section-header-text p {
            margin: 2px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }

        /* ---------- PROGRESS BAR (Daily) ---------- */
        .progress-track {
            display: flex;
            align-items: center;
            gap: 0;
            margin-bottom: 24px;
            padding: 0 8px;
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }
        .progress-step {
            flex: 1;
            display: flex;
            align-items: center;
            gap: 10px;
            min-width: 140px;
            position: relative;
        }
        .progress-step:not(:last-child)::after {
            content: '';
            flex: 1;
            height: 2px;
            background: #e9ecef;
            margin: 0 8px;
            border-radius: 2px;
            transition: background 0.4s ease;
        }
        .progress-step.done:not(:last-child)::after {
            background: linear-gradient(90deg, #00b14f 0%, #4ed17e 100%);
        }
        .progress-dot {
            width: 30px;
            height: 30px;
            border-radius: 50%;
            background: #fff;
            border: 2px solid #dee2e6;
            color: #adb5bd;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
            font-weight: 800;
            flex-shrink: 0;
            transition: all 0.3s ease;
        }
        .progress-step.done .progress-dot {
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border-color: #00b14f;
            color: #fff;
            box-shadow: 0 4px 10px rgba(0,177,79,0.3);
        }
        .progress-step.active .progress-dot {
            background: #fff;
            border-color: #fd7e14;
            color: #fd7e14;
            box-shadow: 0 0 0 5px rgba(253,126,20,0.15);
            animation: pulse-dot 2s ease-in-out infinite;
        }
        @keyframes pulse-dot {
            0%, 100% { box-shadow: 0 0 0 5px rgba(253,126,20,0.15); }
            50%      { box-shadow: 0 0 0 10px rgba(253,126,20,0.05); }
        }
        .progress-label {
            font-size: 12.5px;
            font-weight: 700;
            color: #6c757d;
            white-space: nowrap;
        }
        .progress-step.done .progress-label { color: #00b14f; }
        .progress-step.active .progress-label { color: #d97706; }

        /* ---------- CARDS GRID ---------- */
        .cards-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
            gap: 18px;
        }

        .home-card {
            display: block;
            background: #ffffff;
            border: 1px solid #99f6c3;
            border-radius: 18px;
            padding: 22px 20px;
            text-decoration: none;
            color: inherit;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            position: relative;
            overflow: hidden;
            min-height: 160px;
        }
        .home-card::before {
            content: '';
            position: absolute;
            top: 0;
            right: 0;
            left: 0;
            height: 3px;
            background: linear-gradient(90deg, transparent, transparent);
            transition: background 0.3s ease;
        }
        .home-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 16px 40px rgba(0,177,79,0.12), 0 4px 12px rgba(0,0,0,0.04);
            border-color: #00b14f;
            text-decoration: none;
            color: inherit;
        }
        .home-card:hover::before {
            background: linear-gradient(90deg, #00b14f, #4ed17e);
        }

        .home-card .card-head {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            margin-bottom: 14px;
            gap: 10px;
        }
        .home-card .card-icon {
            width: 48px;
            height: 48px;
            border-radius: 14px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            color: #fff;
            flex-shrink: 0;
            transition: transform 0.3s ease;
        }
        .home-card:hover .card-icon {
            transform: scale(1.08) rotate(-4deg);
        }

        .home-card .card-num {
            font-size: 11px;
            font-weight: 800;
            color: #adb5bd;
            background: #f8f9fa;
            border: 1px solid #eef1f3;
            border-radius: 8px;
            padding: 4px 10px;
            letter-spacing: 0.5px;
        }

        .home-card .card-title {
            font-size: 14.5px;
            font-weight: 800;
            color: #212529;
            margin-bottom: 6px;
            line-height: 1.4;
        }
        .home-card .card-desc {
            font-size: 12.5px;
            color: #6c757d;
            line-height: 1.65;
            margin-bottom: 14px;
        }
        .home-card .card-status {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: 11.5px;
            font-weight: 700;
            padding: 5px 12px;
            border-radius: 999px;
        }
        .home-card .card-arrow {
            display: none;
        }

        /* ---------- Card state: DONE ---------- */
        .home-card.is-done {
            border-color: rgba(0,177,79,0.3);
            background: linear-gradient(180deg, #ffffff 0%, #f6fdf8 100%);
        }
        .home-card.is-done:hover {
            border-color: #00b14f;
        }
        .home-card.is-done .card-icon {
            background: linear-gradient(135deg, #8dcfa4 0%, #b0e2c2 100%) !important;
            opacity: 0.95;
        }
        .home-card.is-done .card-title::after {
            content: "";
            display: inline-block;
            width: 16px;
            height: 16px;
            margin-right: 6px;
            background: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='%2300b14f'%3E%3Cpath d='M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z'/%3E%3C/svg%3E") center/contain no-repeat;
            vertical-align: middle;
        }

        /* ---------- Card state: ACTIVE ---------- */
        .home-card.is-active {
            border-color: #00b14f;
            box-shadow: 0 12px 30px rgba(0,177,79,0.16), 0 2px 8px rgba(0,177,79,0.08);
            background: linear-gradient(180deg, #ffffff 0%, #f8fdfa 100%);
        }
        .home-card.is-active::before {
            background: linear-gradient(90deg, #00b14f, #4ed17e);
        }

        /* ---------- Status Badges ---------- */
        .st-neutral {
            background: #f8f9fa;
            color: #6c757d;
            border: 1px solid #e9ecef;
        }
        .st-done {
            background: rgba(0,177,79,0.1) !important;
            color: #00b14f !important;
            border: 1px solid rgba(0,177,79,0.3);
        }
        .st-active {
            background: rgba(253,126,20,0.1) !important;
            color: #d97706 !important;
            border: 1px solid rgba(253,126,20,0.3);
        }

        /* ---------- Icons (Color gradients) ---------- */
        .ic-green  { background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); box-shadow: 0 6px 14px rgba(0,177,79,0.28); }
        .ic-blue   { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 14px rgba(13,110,253,0.28); }
        .ic-orange { background: linear-gradient(135deg, #fd7e14 0%, #ffb066 100%); box-shadow: 0 6px 14px rgba(253,126,20,0.28); }
        .ic-teal   { background: linear-gradient(135deg, #0dcaf0 0%, #63e1f5 100%); box-shadow: 0 6px 14px rgba(13,202,240,0.28); }
        .ic-purple { background: linear-gradient(135deg, #6f42c1 0%, #9d7bea 100%); box-shadow: 0 6px 14px rgba(111,66,193,0.28); }
        .ic-red    { background: linear-gradient(135deg, #dc3545 0%, #ff6b7a 100%); box-shadow: 0 6px 14px rgba(220,53,69,0.28); }
        .ic-pink   { background: linear-gradient(135deg, #d63384 0%, #ef6fa7 100%); box-shadow: 0 6px 14px rgba(214,51,132,0.28); }
        .ic-gray   { background: linear-gradient(135deg, #6c757d 0%, #adb5bd 100%); box-shadow: 0 6px 14px rgba(108,117,125,0.28); }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .hero-section { padding: 28px 20px 76px; }
            .hero-titles h1 { font-size: 19px; }
            .hero-stat { padding: 10px 16px; min-width: 90px; }
            .hero-stat .stat-value { font-size: 17px; }
            .body-container { padding: 0 16px 40px; }
        }

        @media (max-width: 576px) {
            .hero-section { padding: 22px 16px 70px; border-radius: 0 0 24px 24px; }
            .hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .hero-titles h1 { font-size: 16px; }
            .hero-titles p { font-size: 12px; }
            .hero-stats { width: 100%; }
            .hero-stat { flex: 1; }

            .cards-grid { grid-template-columns: 1fr; gap: 12px; }
            .home-card { padding: 18px 16px; min-height: 0; }
            .home-card .card-icon { width: 42px; height: 42px; font-size: 18px; }
            .home-card .card-desc { display: none; }
            .home-card .card-num { display: none; }

            .progress-track { padding-bottom: 8px; }
            .progress-step { min-width: 120px; }
            .progress-label { font-size: 11px; }

            .today-panel { padding: 18px 20px; }
            .today-panel .today-icon { width: 44px; height: 44px; font-size: 18px; }
            .today-panel .today-date-value { font-size: 17px; }
            .today-panel .today-summary { font-size: 13px; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="home-wrap" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="hero-section">
            <div class="hero-content">
                <div class="hero-left">
                    <div class="hero-icon">
                        <i class="fas fa-chalkboard-teacher"></i>
                    </div>
                    <div class="hero-titles">
                        <h1>الاستحقاق المالي للمعلمين والمعلمات</h1>
                        <p>
                            لوحة إدارة الحضور اليومي والشهري
                            <span class="hero-chip">
                                <i class="fas fa-layer-group"></i>
                                <asp:Label ID="lblCurrentStage" runat="server" />
                            </span>
                        </p>
                    </div>
                </div>

                <div class="hero-right">
                    <div class="hero-stat">
                        <div class="stat-label">اليوم</div>
                        <div class="stat-value"><asp:Label ID="lblTodayDayOnly" runat="server" Text="—" /></div>
                    </div>
                    <div class="hero-stat">
                        <div class="stat-label">التاريخ</div>
                        <div class="stat-value" style="font-size:14px;"><asp:Label ID="lblTodayDateShort" runat="server" Text="—" /></div>
                    </div>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="body-container">

            <!-- Flash Message -->
            <asp:Label ID="lblFlash" runat="server" CssClass="d-none" />

            <!-- Today Status -->
            <asp:Panel ID="pnlTodayStatus" runat="server" Visible="false" CssClass="today-panel">
                <div class="today-icon">
                    <i class="fas fa-bullhorn"></i>
                </div>
                <div class="today-content">
                    <div class="today-date">حالة اليوم</div>
                    <div class="today-date-value"><asp:Label ID="lblTodayDate" runat="server" /></div>
                    <div class="today-summary"><asp:Label ID="lblTodaySummary" runat="server" /></div>
                </div>
            </asp:Panel>

            <!-- ============ العمل اليومي ============ -->
            <div class="home-section">
                <div class="section-header">
                    <div class="section-header-icon daily">
                        <i class="fas fa-sun"></i>
                    </div>
                    <div class="section-header-text">
                        <h3>العمل اليومي</h3>
                        <p>أربع خطوات لإكمال حضور اليوم واعتماده مالياً</p>
                    </div>
                </div>

                <!-- Progress Timeline -->
                <div class="progress-track">
                    <div class="progress-step <%= Step1Done ? "done" : (Step1Active ? "active" : "") %>">
                        <div class="progress-dot">
                            <%= Step1Done ? "<i class='fas fa-check'></i>" : "1" %>
                        </div>
                        <span class="progress-label">توليد حصص اليوم</span>
                    </div>
                    <div class="progress-step <%= Step2Done ? "done" : (Step2Active ? "active" : "") %>">
                        <div class="progress-dot">
                            <%= Step2Done ? "<i class='fas fa-check'></i>" : "2" %>
                        </div>
                        <span class="progress-label">تسجيل الساعات</span>
                    </div>
                    <div class="progress-step <%= Step3Done ? "done" : (Step3Active ? "active" : "") %>">
                        <div class="progress-dot">
                            <%= Step3Done ? "<i class='fas fa-check'></i>" : "3" %>
                        </div>
                        <span class="progress-label">المراجعة والاعتماد</span>
                    </div>
                    <div class="progress-step <%= Step4Active ? "active" : "" %>">
                        <div class="progress-dot">4</div>
                        <span class="progress-label">كشف اليوم</span>
                    </div>
                </div>

                <div class="cards-grid">

                    <a href="Generate.aspx" class="<%= GetStep1Class() %>">
                        <div class="card-head">
                            <div class="card-icon ic-green"><i class="fas fa-cogs"></i></div>
                            <div class="card-num">STEP 01</div>
                        </div>
                        <div class="card-title">توليد حصص اليوم</div>
                        <div class="card-desc">إنشاء حصص اليوم تلقائياً من الجدول الدراسي المعتمد.</div>
                        <%= GetStep1Badge() %>
                    </a>

                    <a href="Record.aspx" class="<%= GetStep2Class() %>">
                        <div class="card-head">
                            <div class="card-icon ic-blue"><i class="fas fa-pen"></i></div>
                            <div class="card-num">STEP 02</div>
                        </div>
                        <div class="card-title">تسجيل الساعات</div>
                        <div class="card-desc">تسجيل حالة كل معلم ودقائق الحضور لكل حصة.</div>
                        <%= GetStep2Badge() %>
                    </a>

                    <a href="Review.aspx" class="<%= GetStep3Class() %>">
                        <div class="card-head">
                            <div class="card-icon ic-orange"><i class="fas fa-clipboard-check"></i></div>
                            <div class="card-num">STEP 03</div>
                        </div>
                        <div class="card-title">مراجعة واعتماد اليوم</div>
                        <div class="card-desc">مراجعة دقيقة للسجلات ثم الاعتماد المالي لليوم.</div>
                        <%= GetStep3Badge() %>
                    </a>

                    <a href="DayDetail.aspx" class="<%= GetStep4Class() %>">
                        <div class="card-head">
                            <div class="card-icon ic-teal"><i class="fas fa-print"></i></div>
                            <div class="card-num">STEP 04</div>
                        </div>
                        <div class="card-title">كشف اليوم (طباعة)</div>
                        <div class="card-desc">عرض وطباعة سجلات الحضور ليوم محدد.</div>
                        <%= GetStep4Badge() %>
                    </a>

                </div>
            </div>

            <!-- ============ العمل الشهري ============ -->
            <div class="home-section">
                <div class="section-header">
                    <div class="section-header-icon monthly">
                        <i class="fas fa-calendar-alt"></i>
                    </div>
                    <div class="section-header-text">
                        <h3>العمل الشهري والإعدادات</h3>
                        <p>الفترات المالية، فحص الجاهزية، والتعويضيات</p>
                    </div>
                </div>

                <div class="cards-grid">

                    <a href="Periods.aspx" class="home-card">
                        <div class="card-head">
                            <div class="card-icon ic-purple"><i class="fas fa-file-invoice-dollar"></i></div>
                            <div class="card-num">شهرياً</div>
                        </div>
                        <div class="card-title">الفترات المالية</div>
                        <div class="card-desc">مزامنة، فحص، حساب، اعتماد، وإغلاق الفترة المالية.</div>
                        <span class="card-status st-neutral"><i class="fas fa-calculator"></i> متابعة</span>
                    </a>

                    <a href="Readiness.aspx" class="home-card">
                        <div class="card-head">
                            <div class="card-icon ic-red"><i class="fas fa-shield-alt"></i></div>
                            <div class="card-num">فحص</div>
                        </div>
                        <div class="card-title">تقرير الجاهزية</div>
                        <div class="card-desc">فحص شامل لبيانات الحضور قبل الحساب المالي.</div>
                        <span class="card-status st-neutral"><i class="fas fa-check-double"></i> تحقق</span>
                    </a>

                    <a href="Substitutions.aspx" class="home-card">
                        <div class="card-head">
                            <div class="card-icon ic-pink"><i class="fas fa-exchange-alt"></i></div>
                            <div class="card-num">خاص</div>
                        </div>
                        <div class="card-title">التعويضيات</div>
                        <div class="card-desc">تسجيل واعتماد تغطية معلم لحصة معلم آخر.</div>
                        <span class="card-status st-neutral"><i class="fas fa-sync"></i> إدارة</span>
                    </a>

                    <a href="LessonPeriods.aspx" class="home-card">
                        <div class="card-head">
                            <div class="card-icon ic-gray"><i class="fas fa-clock"></i></div>
                            <div class="card-num">إعداد</div>
                        </div>
                        <div class="card-title">أوقات الحصص</div>
                        <div class="card-desc">ضبط أوقات بداية ونهاية كل حصة خلال اليوم.</div>
                        <span class="card-status st-neutral"><i class="fas fa-cog"></i> تهيئة</span>
                    </a>

                </div>
            </div>

        </div>
    </div>
</asp:Content>