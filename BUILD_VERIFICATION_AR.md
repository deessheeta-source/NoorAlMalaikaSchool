# التحقق الفعلي من بناء المشروع

تمت إضافة `Build-Verify.ps1` و`Build-Verify.cmd` لأن المشروع ASP.NET Web Forms على .NET Framework 4.8، وبالتالي يحتاج MSBuild/Visual Studio على Windows لإجراء Build حقيقي.

## الاستخدام

1. افتح المشروع على Windows بواسطة Visual Studio 2022 أو أحدث.
2. تأكد من تثبيت workload: **ASP.NET and web development**.
3. شغّل `Build-Verify.cmd` من داخل مجلد المشروع، أو:

```powershell
powershell -ExecutionPolicy Bypass -File .\Build-Verify.ps1 -Configuration Release
```

السكريبت يبحث تلقائيًا عن MSBuild عبر `vswhere` ثم يبني المشروع في وضع Release.

## قاعدة البيانات

لا ينفذ السكريبت تعديل قاعدة البيانات تلقائيًا. بعد نجاح Build خذ نسخة احتياطية (عبر `Database\tools\Backup-NoorDatabase.ps1`) ثم نفّذ أي ترحيلات جديدة في `Database\Migrations\` وفق `Database\MIGRATIONS_GUIDE.md`.

ملاحظة: كان هذا القسم يشير إلى `Database\002_TeacherPayroll_Refactor.sql`، لكن هذا الملف غير موجود في المستودع؛ مخطط ترحيل الرواتب (`TeacherPayrolls` / `PayrollLessonLines` / `TeacherSubjectAssignments`) مطبَّق أصلاً على قاعدة البيانات الحالية.

## ما تم تحسينه

تم توسيع `VSToolsPath` في ملف المشروع لدعم مسارات Visual Studio 2022 (MSBuild 17) وVisual Studio 2026 (MSBuild 18)، إضافة إلى المسار السابق.

هذا يعالج مشكلة شائعة عندما يفشل مشروع Web Forms في العثور على `Microsoft.WebApplication.targets` رغم وجود Visual Studio.
