<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="NoorAlMalaikaSchool.ErrorPage" %>
<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>حدث خطأ</title>
    <style>
        body{font-family:Segoe UI,Tahoma,Arial,sans-serif;background:#f5f7fa;margin:0;color:#1f2937}
        .wrap{min-height:100vh;display:flex;align-items:center;justify-content:center;padding:24px;box-sizing:border-box}
        .card{width:100%;max-width:620px;background:#fff;border-radius:16px;padding:38px;text-align:center;box-shadow:0 10px 30px rgba(0,0,0,.08)}
        .icon{font-size:52px;margin-bottom:12px}
        h1{margin:0 0 12px;font-size:28px}
        p{line-height:1.9;color:#6b7280;margin:8px 0}
        .actions{margin-top:24px}
        a{display:inline-block;text-decoration:none;background:#0d6efd;color:#fff;padding:10px 20px;border-radius:8px}
    </style>
</head>
<body>
<form id="form1" runat="server">
<div class="wrap"><div class="card">
    <div class="icon">⚠️</div>
    <h1>حدث خطأ غير متوقع</h1>
    <p>تعذر إكمال العملية حالياً.</p>
    <p>تم تسجيل الخطأ ويمكن متابعة العمل بعد إعادة المحاولة.</p>
    <div class="actions"><a href="<%= ResolveUrl("~/Default.aspx") %>">العودة إلى الصفحة الرئيسية</a></div>
</div></div>
</form>
</body>
</html>
