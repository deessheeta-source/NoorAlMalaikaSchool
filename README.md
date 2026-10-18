# مدرسة نور الملائكة — نظام الإدارة المدرسية

نظام إداري مدرسي متكامل مبني بـ **ASP.NET Web Forms على .NET Framework 4.8** (مشروع واحد) يغطي: الطلاب، المعلمون، الحصص والحضور، الجداول الدراسية، الفواتير والمدفوعات، رواتب المعلمين بالحصص، التقارير، ووحدة مواصلات واتساب لأولياء الأمور.

## المتطلبات

- Windows مع Visual Studio 2022 (أو أحدث) و workload **ASP.NET and web development**.
- SQL Server (بيئة التشغيل الحالية: `SHEETAGECOL\SQLSERVER2026` بمصادقة Windows).
- اتصال قاعدة البيانات في `Web.config` باستخدام `Integrated Security=True` (لا كلمة مرور مخزّنة).

## البناء والتحقق

```powershell
powershell -ExecutionPolicy Bypass -File .\Build-Verify.ps1 -Configuration Release
```

السكريبت يبحث عن MSBuild عبر `vswhere` ويبني في وضع Release دون تعديل قاعدة البيانات. راجع `BUILD_VERIFICATION_AR.md`.

## قاعدة البيانات

- الترحيلات: `Database\Migrations\` وفق `Database\MIGRATIONS_GUIDE.md` (idempotent، تُنفَّذ بعد نسخة احتياطية).
- تصدير المخطط الكامل: `Database\tools\Export-Schema.ps1`.
- وحدة واتساب: `Database\WHATSAPP_SETUP_AR.md`.

## النسخ الاحتياطي (مجدول — فعّال)

```powershell
# تشغيل يدوي الآن
Database\tools\Backup-NoorDatabase.ps1

# جدولة يومية 02:00 (مهمة Windows Task Scheduler — لا يوجد SQL Agent على هذا الخادم)
Database\tools\Backup-NoorDatabase.ps1 -RegisterTask
```

- ينسخ إلى `App_Data\DatabaseBackups\` باسم مؤرَّخ، يتحقّق بـ `RESTORE VERIFYONLY`، وينظّف الأقدم من 14 يوماً (قابل للتعديل بـ `-RetentionDays`).
- السجل: `App_Data\DatabaseBackups\backup.log`.
- نسخة خارجية على قرص/مجلد آخر: `-SecondaryDir "D:\..."` — **يُنصح بشدة** (النسخ داخل نفس القرص لا يحمي من عطله).
- **لازم مرّة واحدة:** منح خدمة SQL صلاحية الكتابة على مجلد النسخ مُنجز مسبقاً؛ إن نُقل المشروع لخادم آخر أعد المنح:
  `icacls "App_Data\DatabaseBackups" /grant "NT SERVICE\MSSQL$<اسم_الخادم>:(OI)(CI)M"`

## فحص التساوي المالي (أسبوعياً)

```powershell
Database\tools\Invoke-FinancialCheck.ps1
```

10 فحوصات: صيغة `NetAmount`، بنود الفواتير مقابل الإجمالي، المدفوعات مقابل `PaidAmount`، اتساق الحالة (Status)، المدفوعات اليتيمة. يخرج برمز 1 عند أي `FAIL` (صالح للجدولة). التحقق من قاعدة البيانات الحالية: **10/10 OK** بتاريخ 2026-10-18.

## الأمان والأدوار

- 6 أدوار: مدير النظام، محاسب، مشرف، مدخل بيانات، معلم، عرض — تحقق مركزي في `Services\SecurityHelper.cs` (`RequireRolesForPath`) مع قائمة سماح صارمة لدور «عرض».
- إجراءات مطبَّقة: `httpOnlyCookies`، `machineKey` مثبّت، `ViewStateUserKey` مرتبط بالمستخدم، قفل الحساب بعد محاولات دخول فاشلة، `debug=false`.
- ⚠️ **تنبيه أمني مفتوح:** الموقع يعمل عبر **HTTP فقط** (لا يوجد HTTPS). فعّل شهادة + رابط IIS ثم اضبط `requireSSL=true` في `Web.config` قبل أي نشر مكشوف للإنترنت.

## Git

مستودع محلي (لا remote). `.gitignore` يستبعد `bin/`، `packages/`، `*.bak`، `App_Data/*.log`، `Web.Release.config` والسرّيات (`*.pfx`, `.env`…). قبل رفع أي مستودع عام: افحص `Web.config` (يحتوي `machineKey` — استخدم نمط `Web.Release.config.example` لطرد المفاتيح).

## أبرز الوثائق

| الوثيقة | المحتوى |
|---|---|
| `BUILD_VERIFICATION_AR.md` | البناء والتحقق |
| `VIEW_ROLE_IMPLEMENTATION_AR.md` | صلاحيات دور «عرض» |
| `PAYROLL_REFACTOR_README_AR.md` | نموذج الرواتب بالحصص |
| `Database/MIGRATIONS_GUIDE.md` | دليل ترحيل قاعدة البيانات |
| `Database/WHATSAPP_SETUP_AR.md` | إعداد وحدة واتساب |
