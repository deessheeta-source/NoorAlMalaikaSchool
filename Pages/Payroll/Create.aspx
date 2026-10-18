<%@ Page Title="فتح فترة مالية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Payroll.Create" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
         .payroll-page {
            font-family: 'Droid Arabic Kufi', sans-serif;
            background: #f7f9f8;
            min-height: 100vh;
            padding: 0;
        }

        /* ---------- HERO ---------- */
        .payroll-hero {
            position: relative;
            background: linear-gradient(135deg,#8a67d8 0%,#825ec2 100%);
            padding: 32px 32px 78px;
            overflow: hidden;
            border-radius: 0 0 32px 32px;
        }
        .payroll-hero::before {
            content: '';
            position: absolute;
            inset: 0;
            background-image:
                radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%),
                radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%),
                radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%);
            pointer-events: none;
        }
        .payroll-hero::after {
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

        .payroll-hero-content {
            position: relative;
            z-index: 2;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 24px;
            flex-wrap: wrap;
            max-width: 900px;
            margin: 0 auto;
        }

        .payroll-hero-left {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        .payroll-hero-icon {
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
        .payroll-hero-titles h1 {
            font-size: 22px;
            font-weight: 800;
            color: #fff;
            margin: 0 0 4px;
            line-height: 1.3;
            letter-spacing: -0.2px;
        }
        .payroll-hero-titles p {
            font-size: 13px;
            color: rgba(255,255,255,0.85);
            margin: 0;
            line-height: 1.5;
            max-width: 500px;
        }

        .payroll-btn-back {
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
            flex-shrink: 0;
            cursor: pointer;
        }
        .payroll-btn-back:hover {
            background: rgba(255,255,255,0.25);
            color: #fff;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* ---------- BODY ---------- */
        .payroll-body {
            max-width: 900px;
            margin: -56px auto 0;
           padding: 0 24px 0;
            position: relative;
            z-index: 3;
        }

        /* ---------- CARD ---------- */
        .payroll-card {
            background: #ffffff;
            border-radius: 20px;
            border: 1px solid #99f6c3;
            box-shadow: 0 8px 24px rgba(0,0,0,0.04);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .payroll-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 20px 24px;
            border-bottom: 1px solid #f1f3f5;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%);
        }
        .payroll-card-header-icon {
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
        .payroll-card-header-icon::after {
            content: '';
            position: absolute;
            inset: -3px;
            border-radius: 16px;
            border: 2px solid currentColor;
            opacity: 0.15;
        }
        .payroll-card-header-text h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 800;
            color: #212529;
            line-height: 1.2;
        }
        .payroll-card-header-text p {
            margin: 3px 0 0;
            font-size: 12.5px;
            color: #6c757d;
            font-weight: 500;
        }

        .payroll-card-body {
            padding: 24px;
        }

        /* ---------- FORM FIELDS ---------- */
        .payroll-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
            margin-bottom: 18px;
        }
        .payroll-field label {
            font-size: 12px;
            font-weight: 700;
            color: #495057;
            letter-spacing: 0.2px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .payroll-field label i {
            color: #00b14f;
            font-size: 13px;
        }
        .payroll-field .form-select,
        .payroll-field .form-control {
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            padding: 12px 16px;
            font-size: 13.5px;
            font-weight: 600;
            color: #212529;
            transition: all 0.2s ease;
            background: #f9fafb;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .payroll-field .form-select:focus,
        .payroll-field .form-control:focus {
            border-color: #00b14f;
            background: #fff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
            outline: none;
        }

        /* ---------- BUTTONS ---------- */
        .payroll-btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 13px 32px;
            border-radius: 12px;
            background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%);
            border: none;
            color: #fff;
            font-size: 14px;
            font-weight: 800;
            cursor: pointer;
            transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 8px 20px rgba(0,177,79,0.3);
            height: 48px;
            width: 100%;
            font-family: 'Droid Arabic Kufi', sans-serif;
        }
        .payroll-btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 12px 28px rgba(0,177,79,0.4);
            color: #fff;
        }
        .payroll-btn-primary:disabled {
            opacity: 0.6;
            cursor: not-allowed;
            transform: none;
        }

        /* ---------- ALERT ---------- */
        .payroll-alert {
            display: flex;
            align-items: flex-start;
            gap: 12px;
            padding: 16px 20px;
            border-radius: 14px;
            margin-top: 20px;
            font-size: 13px;
            font-weight: 700;
            line-height: 1.7;
            animation: slideDown 0.4s ease;
        }
        .payroll-alert i {
            width: 36px;
            height: 36px;
            border-radius: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
            color: #fff;
            margin-top: 2px;
        }
        .payroll-alert.success {
            background: linear-gradient(180deg, #ffffff 0%, #f8fdfa 100%);
            border: 1.5px solid rgba(0,177,79,0.35);
            color: #1f3d33;
        }
        .payroll-alert.success i { background: linear-gradient(135deg, #00b14f, #4ed17e); }

        .payroll-alert.danger {
            background: linear-gradient(180deg, #ffffff 0%, #fef8f8 100%);
            border: 1.5px solid rgba(220,53,69,0.35);
            color: #7a1a26;
        }
        .payroll-alert.danger i { background: linear-gradient(135deg, #dc3545, #ff6b7a); }

        @keyframes slideDown {
            from { opacity: 0; transform: translateY(-10px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        /* ---------- INFO NOTE ---------- */
        .payroll-note {
            display: flex;
            align-items: flex-start;
            gap: 12px;
            padding: 14px 18px;
            background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%);
            border: 1.5px solid rgba(0,177,79,0.2);
            border-radius: 14px;
            margin-top: 20px;
            font-size: 12.5px;
            color: #1f3d33;
            line-height: 1.7;
            font-weight: 500;
        }
        .payroll-note i {
            color: #00b14f;
            font-size: 18px;
            flex-shrink: 0;
            margin-top: 2px;
        }
        .payroll-note strong {
            color: #007a33;
            font-weight: 800;
        }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 768px) {
            .payroll-hero { padding: 26px 20px 72px; }
            .payroll-hero-titles h1 { font-size: 19px; }
            .payroll-body { padding: 0 16px 40px; }
            .payroll-card-body { padding: 20px; }
        }
        @media (max-width: 576px) {
            .payroll-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .payroll-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .payroll-hero-titles h1 { font-size: 16px; }
            .payroll-hero-titles p { font-size: 12px; }
            .payroll-btn-back { padding: 8px 14px; font-size: 12px; }
            .payroll-btn-back span { display: none; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="payroll-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="payroll-hero">
            <div class="payroll-hero-content">
                <div class="payroll-hero-left">
                    <div class="payroll-hero-icon">
                        <i class="fas fa-calendar-plus"></i>
                    </div>
                    <div class="payroll-hero-titles">
                        <h1>فتح فترة مالية جديدة</h1>
                        <p>اختر السنة والشهر لبدء دورة الاستحقاق الشهري للمعلمين والمعلمات</p>
                    </div>
                </div>
                <a href="../TeacherAttendance/Periods.aspx" class="payroll-btn-back">
                    <i class="fas fa-arrow-right"></i>
                    <span>عودة</span>
                </a>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="payroll-body">

            <div class="payroll-card">
                <div class="payroll-card-header">
                    <div class="payroll-card-header-icon">
                        <i class="fas fa-calendar-check"></i>
                    </div>
                    <div class="payroll-card-header-text">
                        <h3>تحديد الفترة المالية</h3>
                        <p>اختر السنة والشهر ثم اضغط «فتح الفترة»</p>
                    </div>
                </div>
                <div class="payroll-card-body">

                    <div class="row g-3">

                        <div class="col-md-6">
                            <div class="payroll-field">
                                <label>
                                    <i class="fas fa-calendar-alt"></i> السنة
                                </label>
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select" />
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="payroll-field">
                                <label>
                                    <i class="fas fa-calendar-day"></i> الشهر
                                </label>
                                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1" Text="يناير" />
                                    <asp:ListItem Value="2" Text="فبراير" />
                                    <asp:ListItem Value="3" Text="مارس" />
                                    <asp:ListItem Value="4" Text="أبريل" />
                                    <asp:ListItem Value="5" Text="مايو" />
                                    <asp:ListItem Value="6" Text="يونيو" />
                                    <asp:ListItem Value="7" Text="يوليو" />
                                    <asp:ListItem Value="8" Text="أغسطس" />
                                    <asp:ListItem Value="9" Text="سبتمبر" />
                                    <asp:ListItem Value="10" Text="أكتوبر" />
                                    <asp:ListItem Value="11" Text="نوفمبر" />
                                    <asp:ListItem Value="12" Text="ديسمبر" />
                                </asp:DropDownList>
                            </div>
                        </div>

                    </div>

                    <asp:Button ID="btnCreate" runat="server" Text="فتح الفترة"
                        CssClass="payroll-btn-primary" OnClick="btnCreate_Click" />

                    <!-- Info Note -->
                    <div class="payroll-note">
                        <i class="fas fa-info-circle"></i>
                        <span>
                            عند فتح الفترة، يقوم النظام <strong>تلقائياً</strong> بتجهيز
                            الحصص اليومية للشهر كامل من الجدول المدرسي المعتمد.
                            لن تُلمس الحصص التي عليها حضور مسجل مسبقاً.
                        </span>
                    </div>

                    <!-- Alert -->
                    <asp:Label ID="lblInfo" runat="server" CssClass="d-none" />

                </div>
            </div>

        </div>
    </div>
</asp:Content>