<%@ Page Title="لا تملك صلاحية" Language="C#" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="NoorAlMalaikaSchool.AccessDenied" %>
<html dir="rtl" lang="ar">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>لا تملك صلاحية - مدرسة نور الملائكة</title>
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
        .denied-card {
            background: #fff;
            border-radius: 16px;
            padding: 40px 36px;
            width: 100%;
            max-width: 460px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.2);
            text-align: center;
        }
        .denied-icon {
            width: 72px;
            height: 72px;
            background: #fee2e2;
            color: #dc2626;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 30px;
            margin: 0 auto 18px;
        }
        .denied-title { font-weight: 800; font-size: 20px; color: #111827; margin-bottom: 8px; }
        .denied-sub { color: #6b7280; font-size: 14px; margin-bottom: 24px; line-height: 1.8; }
        .btn-home {
            display: inline-block;
            background: #1759d4;
            color: #fff;
            font-weight: 700;
            padding: 10px 28px;
            border-radius: 8px;
            text-decoration: none;
            font-size: 14px;
            transition: all 0.15s;
        }
        .btn-home:hover { background: #1446b0; color: #fff; }
    </style>
</head>
<body>
    <div class="denied-card">
        <div class="denied-icon"><i class="fas fa-shield-alt"></i></div>
        <div class="denied-title">غير مصرّح بالوصول</div>
        <div class="denied-sub">
            دورك الحالي لا يملك صلاحية الوصول إلى هذه الصفحة.<br />
            <asp:Literal ID="litDeniedPath" runat="server" />
        </div>
        <a href="Default.aspx" class="btn-home"><i class="fas fa-th-large me-1"></i>العودة إلى لوحة التحكم</a>
    </div>
</body>
</html>