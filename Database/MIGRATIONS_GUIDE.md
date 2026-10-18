# دليل ترحيلات قاعدة البيانات — نور الملائكة

## الوضع الحالي

المشروع يستخدم **SQL يدوي** لترحيل قاعدة البيانات (ليس EF Code-First Migrations).
كل تغيير في مخطط قاعدة البيانات يُكتب كملف `.sql` في مجلد:

```
Database/Migrations/
```

## قواعد إنشاء ملف ترحيل جديد

### 1. تسمية الملف
```
YYYYMMDD_وصف_موجز.sql
```
مثال: `20261016_AddTeacherPhoneColumn.sql`

### 2. هيكل كل ملف ترحيل

```sql
-- ================================================================
-- Migration: AddTeacherPhoneColumn
-- Date:      2026-10-16
-- Author:    [اسم المطوّر]
-- Depends:   [اسم الملف السابق إن وجد]
-- ================================================================

-- ✅ فحص التطبيق المسبق (منع التطبيق المزدوج)
IF COL_LENGTH('Teachers', 'Phone') IS NOT NULL
BEGIN
    RAISERROR('Migration already applied.', 16, 1);
    RETURN;
END

-- ➕ التغيير
ALTER TABLE Teachers ADD Phone NVARCHAR(30) NULL;
GO

-- ✅ تأكيد
PRINT 'Migration AddTeacherPhoneColumn applied successfully.';
```

### 3. قواعد أساسية

| القاعدة | السبب |
|---|---|
| كل ملف يحتوي تغيير واحد أو مجموعة مترابطة | سهولة التراجع |
| دائماً أضف فحص `IF NOT EXISTS` / `IF COL_LENGTH IS NULL` | منع الفشل عند إعادة التطبيق |
| لا تعدّل ملفاً موجوداً | سجل الترحيلات يجب أن يكون غير قابل للتعديل |
| اختبر على بيئة تطوير أولاً | لا تُطبَّق على الإنتاج مباشرة |
| أضف `GO` بعد كل عبارة DDL مستقلة | متطلب SQL Server |

### 4. ترتيب الترحيلات الحالية

```
20261015_AddWhatsAppModule.sql        ← تم التطبيق ✅ (جداول وحدة واتساب)
20261016_EnhanceWhatsAppModule.sql    ← ترقية وحدة واتساب (حقول الجمهور/الخيارات وإلغاء الموافقة)
```

### 5. تطبيق الترحيل على السيرفر

```powershell
# تشغيل ترحيل محدد
sqlcmd -S SHEETAGECOL\SQLSERVER2026 `
       -d NoorAlMalaikaSchool `
       -i "Database\Migrations\20261016_AddTeacherPhoneColumn.sql"
```

أو من **SQL Server Management Studio**: افتح الملف وشغّله على قاعدة `NoorAlMalaikaSchool`.

---

> [!IMPORTANT]
> قبل أي تغيير على قاعدة البيانات في الإنتاج، خذ **نسخة احتياطية كاملة** من لوحة `Pages/Database`.
