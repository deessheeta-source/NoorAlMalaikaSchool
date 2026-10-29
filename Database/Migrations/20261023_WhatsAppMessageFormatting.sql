/*
    20261023_WhatsAppMessageFormatting.sql
    ------------------------------------------------------------------
    Rewrites the built-in WhatsApp templates with a consistent, modern
    multi-line layout. Every outbound message starts with the school name
    so parents can identify the sender before reading the content.

    Idempotent: safe to run more than once. Templates renamed or edited by
    an administrator are preserved; only templates still carrying one of
    the original names are rewritten. Any other template that does not yet
    start with the school name gets the header prepended without changing
    its content.

    Apply after 20261022_WhatsAppP0Hardening.sql.
*/

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.WhatsAppTemplates', N'U') IS NULL
BEGIN
    PRINT N'WhatsAppTemplates does not exist. Apply 20261015_AddWhatsAppModule.sql first.';
    RETURN;
END;

/* ---------- 1) تذكير بقسط (Financial reminder) ---------- */
IF EXISTS (SELECT 1 FROM dbo.WhatsAppTemplates WHERE Name = N'تذكير بقسط')
BEGIN
    UPDATE dbo.WhatsAppTemplates
    SET Category = N'مالي',
        Body = N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
               N'السلام عليكم' + CHAR(13)+CHAR(10) +
               N'ولي الأمر: {guardianName}' + CHAR(13)+CHAR(10) +
               N'نذكّركم بأن المبلغ المستحق للطالب: {studentName}' + CHAR(13)+CHAR(10) +
               N'{class}' + CHAR(13)+CHAR(10) +
               N'المبلغ المستحق: {balance}' + CHAR(13)+CHAR(10) +
               N'للتفاصيل يرجى التواصل مع الإدارة.'
    WHERE Name = N'تذكير بقسط';
END
ELSE
BEGIN
    INSERT dbo.WhatsAppTemplates
        (Name, Category, Body, ProviderTemplateName, LanguageCode, IsMetaApproved, IsActive, CreatedBy)
    VALUES
        (N'تذكير بقسط', N'مالي',
         N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
         N'السلام عليكم' + CHAR(13)+CHAR(10) +
         N'ولي الأمر: {guardianName}' + CHAR(13)+CHAR(10) +
         N'نذكّركم بأن المبلغ المستحق للطالب: {studentName}' + CHAR(13)+CHAR(10) +
         N'{class}' + CHAR(13)+CHAR(10) +
         N'المبلغ المستحق: {balance}' + CHAR(13)+CHAR(10) +
         N'للتفاصيل يرجى التواصل مع الإدارة.',
         NULL, N'ar', 0, 1, N'system');
END;

/* ---------- 2) إشعار غياب (Absence notice) ---------- */
IF EXISTS (SELECT 1 FROM dbo.WhatsAppTemplates WHERE Name = N'إشعار غياب')
BEGIN
    UPDATE dbo.WhatsAppTemplates
    SET Category = N'حضور',
        Body = N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
               N'السلام عليكم' + CHAR(13)+CHAR(10) +
               N'ولي الأمر: {guardianName}' + CHAR(13)+CHAR(10) +
               N'نحيطكم علماً بغياب الطالب: {studentName}' + CHAR(13)+CHAR(10) +
               N'{class}' + CHAR(13)+CHAR(10) +
               N'التاريخ: {date}' + CHAR(13)+CHAR(10) +
               N'للتفاصيل يرجى التواصل مع الإدارة.'
    WHERE Name = N'إشعار غياب';
END
ELSE
BEGIN
    INSERT dbo.WhatsAppTemplates
        (Name, Category, Body, ProviderTemplateName, LanguageCode, IsMetaApproved, IsActive, CreatedBy)
    VALUES
        (N'إشعار غياب', N'حضور',
         N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
         N'السلام عليكم' + CHAR(13)+CHAR(10) +
         N'ولي الأمر: {guardianName}' + CHAR(13)+CHAR(10) +
         N'نحيطكم علماً بغياب الطالب: {studentName}' + CHAR(13)+CHAR(10) +
         N'{class}' + CHAR(13)+CHAR(10) +
         N'التاريخ: {date}' + CHAR(13)+CHAR(10) +
         N'للتفاصيل يرجى التواصل مع الإدارة.',
         NULL, N'ar', 0, 1, N'system');
END;

/* ---------- 3) استدعاء ولي أمر (Parent summons) ---------- */
IF EXISTS (SELECT 1 FROM dbo.WhatsAppTemplates WHERE Name = N'استدعاء ولي أمر')
BEGIN
    UPDATE dbo.WhatsAppTemplates
    SET Category = N'إداري',
        Body = N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
               N'السلام عليكم' + CHAR(13)+CHAR(10) +
               N'ولي الأمر: {guardianName}' + CHAR(13)+CHAR(10) +
               N'نرجو حضوركم إلى المدرسة لمتابعة أمر يخص الطالب: {studentName}' + CHAR(13)+CHAR(10) +
               N'{class}' + CHAR(13)+CHAR(10) +
               N'يرجى التواصل مع الإدارة لتحديد الموعد.'
    WHERE Name = N'استدعاء ولي أمر';
END
ELSE
BEGIN
    INSERT dbo.WhatsAppTemplates
        (Name, Category, Body, ProviderTemplateName, LanguageCode, IsMetaApproved, IsActive, CreatedBy)
    VALUES
        (N'استدعاء ولي أمر', N'إداري',
         N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
         N'السلام عليكم' + CHAR(13)+CHAR(10) +
         N'ولي الأمر: {guardianName}' + CHAR(13)+CHAR(10) +
         N'نرجو حضوركم إلى المدرسة لمتابعة أمر يخص الطالب: {studentName}' + CHAR(13)+CHAR(10) +
         N'{class}' + CHAR(13)+CHAR(10) +
         N'يرجى التواصل مع الإدارة لتحديد الموعد.',
         NULL, N'ar', 0, 1, N'system');
END;

/* ---------- 4) إعلان عطلة (Holiday announcement) ---------- */
IF EXISTS (SELECT 1 FROM dbo.WhatsAppTemplates WHERE Name = N'إعلان عطلة')
BEGIN
    UPDATE dbo.WhatsAppTemplates
    SET Category = N'إعلان',
        Body = N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
               N'السلام عليكم' + CHAR(13)+CHAR(10) +
               N'تعلن إدارة المدرسة عن عطلة بتاريخ: {date}' + CHAR(13)+CHAR(10) +
               N'{extra}' + CHAR(13)+CHAR(10) +
               N'مع تحيات إدارة المدرسة.'
    WHERE Name = N'إعلان عطلة';
END
ELSE
BEGIN
    INSERT dbo.WhatsAppTemplates
        (Name, Category, Body, ProviderTemplateName, LanguageCode, IsMetaApproved, IsActive, CreatedBy)
    VALUES
        (N'إعلان عطلة', N'إعلان',
         N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
         N'السلام عليكم' + CHAR(13)+CHAR(10) +
         N'تعلن إدارة المدرسة عن عطلة بتاريخ: {date}' + CHAR(13)+CHAR(10) +
         N'{extra}' + CHAR(13)+CHAR(10) +
         N'مع تحيات إدارة المدرسة.',
         NULL, N'ar', 0, 1, N'system');
END;

/* ---------- 5) اجتماع أولياء الأمور (Parent meeting) ---------- */
IF EXISTS (SELECT 1 FROM dbo.WhatsAppTemplates WHERE Name = N'اجتماع أولياء الأمور')
BEGIN
    UPDATE dbo.WhatsAppTemplates
    SET Category = N'إعلان',
        Body = N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
               N'السلام عليكم' + CHAR(13)+CHAR(10) +
               N'ولي الأمر: {guardianName}' + CHAR(13)+CHAR(10) +
               N'تدعوكم إدارة المدرسة لاجتماع أولياء الأمور للطالب: {studentName}' + CHAR(13)+CHAR(10) +
               N'{class}' + CHAR(13)+CHAR(10) +
               N'التاريخ: {date}' + CHAR(13)+CHAR(10) +
               N'الوقت: {time}' + CHAR(13)+CHAR(10) +
               N'حضوركم محل اهتمامنا، مع تحيات إدارة المدرسة.'
    WHERE Name = N'اجتماع أولياء الأمور';
END
ELSE
BEGIN
    INSERT dbo.WhatsAppTemplates
        (Name, Category, Body, ProviderTemplateName, LanguageCode, IsMetaApproved, IsActive, CreatedBy)
    VALUES
        (N'اجتماع أولياء الأمور', N'إعلان',
         N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
         N'السلام عليكم' + CHAR(13)+CHAR(10) +
         N'ولي الأمر: {guardianName}' + CHAR(13)+CHAR(10) +
         N'تدعوكم إدارة المدرسة لاجتماع أولياء الأمور للطالب: {studentName}' + CHAR(13)+CHAR(10) +
         N'{class}' + CHAR(13)+CHAR(10) +
         N'التاريخ: {date}' + CHAR(13)+CHAR(10) +
         N'الوقت: {time}' + CHAR(13)+CHAR(10) +
         N'حضوركم محل اهتمامنا، مع تحيات إدارة المدرسة.',
         NULL, N'ar', 0, 1, N'system');
END;

/* ---------- 6) Safety net for custom templates ---------- */
/* Any template not touched above still gets the school header. */
UPDATE dbo.WhatsAppTemplates
SET Body = N'مدرسة نور الملائكة' + CHAR(13)+CHAR(10) +
           CASE
               WHEN Body LIKE N'السلام عليكم%' THEN Body
               ELSE N'السلام عليكم' + CHAR(13)+CHAR(10) + Body
           END
WHERE Body NOT LIKE N'مدرسة نور الملائكة%';
