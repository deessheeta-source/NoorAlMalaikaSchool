<%@ Page Title="تسجيل الدخول" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NoorAlMalaikaSchool.Login" %>

<!DOCTYPE html>
<html dir="rtl" lang="ar">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>تسجيل الدخول - مدرسة نور الملائكة</title>
    <link href="Content/css/bootstrap.rtl.min.css" rel="stylesheet" />
    <link href="Content/fonts/droid-arabic-kufi/DroidKufi.css" rel="stylesheet" />
    <link href="Content/fonts/fontawesome-free/css/all.min.css" rel="stylesheet" />
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
            font-family: 'Droid Arabic Kufi', sans-serif;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            background: linear-gradient(135deg, #1759d4 0%, #3390fb 100%);
            padding: 20px;
        }
        .login-card {
            background: #fff;
            border-radius: 16px;
            padding: 40px 36px;
            width: 100%;
            max-width: 400px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.2);
        }
        .brand-icon {
            width: 64px;
            height: 64px;
            background: #1759d4;
            border-radius: 16px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 28px;
            color: #fff;
            margin: 0 auto 16px;
        }
        .login-title {
            text-align: center;
            font-weight: 800;
            font-size: 20px;
            color: #111827;
            margin-bottom: 4px;
        }
        .login-sub {
            text-align: center;
            color: #6b7280;
            font-size: 13px;
            margin-bottom: 24px;
        }
        .form-label { font-weight: 600; font-size: 13px; color: #374151; }
        .form-control { border-radius: 8px; padding: 10px 12px; border: 1px solid #e5e7eb; }
        .form-control:focus { border-color: #1759d4; box-shadow: 0 0 0 3px rgba(23,89,212,0.1); }
        .btn-login {
            width: 100%;
            background: #1759d4;
            border: none;
            color: #fff;
            font-weight: 700;
            padding: 11px;
            border-radius: 8px;
            font-family: 'Droid Arabic Kufi', sans-serif;
            transition: all 0.15s;
        }
        .btn-login:hover { background: #1446b0; }
        .alert { border-radius: 8px; font-size: 13px; }
        .input-group-text { background: #f9fafb; border: 1px solid #e5e7eb; border-inline-end: 0; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            <div class="brand-icon"><i class="fas fa-graduation-cap"></i></div>
            <div class="login-title">مدرسة نور الملائكة</div>
            <div class="login-sub">تسجيل الدخول إلى نظام الإدارة</div>

            <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger d-block mb-3" Visible="false" />

            <div class="mb-3">
                <label class="form-label">اسم المستخدم</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fas fa-user"></i></span>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="اسم المستخدم" />
                </div>
            </div>

            <div class="mb-4">
                <label class="form-label">كلمة المرور</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fas fa-lock"></i></span>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="كلمة المرور" />
                </div>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="دخول" CssClass="btn-login" OnClick="btnLogin_Click" />
        </div>
    </form>
</body>
</html>
