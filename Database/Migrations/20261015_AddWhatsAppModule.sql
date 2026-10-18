/*
   Noor Al Malaika School - WhatsApp module schema
   Apply once to the application database after taking a .bak backup.
   This script creates only the new WhatsApp tables and starter templates.
*/
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.WhatsAppCampaigns', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WhatsAppCampaigns
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WhatsAppCampaigns PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        MessageBody NVARCHAR(MAX) NOT NULL,
        MessageKind NVARCHAR(30) NOT NULL,
        Status NVARCHAR(30) NOT NULL,
        ClassRoomId INT NULL,
        TargetStudentId INT NULL,
        WhatsAppTemplateId INT NULL,
        ScheduledAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_WhatsAppCampaigns_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedBy NVARCHAR(100) NOT NULL
    );
    CREATE INDEX IX_WhatsAppCampaigns_Status_ScheduledAt
        ON dbo.WhatsAppCampaigns(Status, ScheduledAt);
END;

IF OBJECT_ID(N'dbo.WhatsAppMessages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WhatsAppMessages
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WhatsAppMessages PRIMARY KEY,
        CampaignId INT NULL,
        ParentId INT NULL,
        StudentId INT NULL,
        RecipientName NVARCHAR(200) NULL,
        PhoneNumber NVARCHAR(30) NOT NULL,
        MessageBody NVARCHAR(MAX) NOT NULL,
        Direction NVARCHAR(20) NOT NULL,
        Status NVARCHAR(30) NOT NULL,
        ProviderMessageId NVARCHAR(200) NULL,
        ProviderTemplateName NVARCHAR(100) NULL,
        TemplateLanguage NVARCHAR(20) NULL,
        TemplateParametersJson NVARCHAR(4000) NULL,
        ErrorDetails NVARCHAR(1500) NULL,
        Attempts INT NOT NULL CONSTRAINT DF_WhatsAppMessages_Attempts DEFAULT 0,
        IsReplyToIncoming BIT NOT NULL CONSTRAINT DF_WhatsAppMessages_IsReply DEFAULT 0,
        IsInternalRecipient BIT NOT NULL CONSTRAINT DF_WhatsAppMessages_Internal DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_WhatsAppMessages_CreatedAt DEFAULT SYSUTCDATETIME(),
        ScheduledAt DATETIME2 NULL,
        SentAt DATETIME2 NULL,
        CreatedBy NVARCHAR(100) NULL,
        CONSTRAINT FK_WhatsAppMessages_Campaigns FOREIGN KEY (CampaignId)
            REFERENCES dbo.WhatsAppCampaigns(Id),
        CONSTRAINT FK_WhatsAppMessages_Parents FOREIGN KEY (ParentId)
            REFERENCES dbo.Parents(Id),
        CONSTRAINT FK_WhatsAppMessages_Students FOREIGN KEY (StudentId)
            REFERENCES dbo.Students(Id)
    );
    CREATE INDEX IX_WhatsAppMessages_Status_ScheduledAt
        ON dbo.WhatsAppMessages(Direction, Status, ScheduledAt);
    CREATE INDEX IX_WhatsAppMessages_CampaignId ON dbo.WhatsAppMessages(CampaignId);
    CREATE INDEX IX_WhatsAppMessages_ParentId_CreatedAt
        ON dbo.WhatsAppMessages(ParentId, CreatedAt DESC);
    CREATE UNIQUE INDEX UX_WhatsAppMessages_ProviderMessageId
        ON dbo.WhatsAppMessages(ProviderMessageId) WHERE ProviderMessageId IS NOT NULL;
END;

IF OBJECT_ID(N'dbo.WhatsAppMessages', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.WhatsAppMessages') AND name = N'UX_WhatsAppMessages_ProviderMessageId')
    CREATE UNIQUE INDEX UX_WhatsAppMessages_ProviderMessageId
        ON dbo.WhatsAppMessages(ProviderMessageId) WHERE ProviderMessageId IS NOT NULL;

IF OBJECT_ID(N'dbo.WhatsAppTemplates', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WhatsAppTemplates
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WhatsAppTemplates PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Category NVARCHAR(30) NOT NULL,
        Body NVARCHAR(MAX) NOT NULL,
        ProviderTemplateName NVARCHAR(100) NULL,
        LanguageCode NVARCHAR(20) NOT NULL CONSTRAINT DF_WhatsAppTemplates_Language DEFAULT N'ar',
        IsMetaApproved BIT NOT NULL CONSTRAINT DF_WhatsAppTemplates_Approved DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_WhatsAppTemplates_Active DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_WhatsAppTemplates_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedBy NVARCHAR(100) NOT NULL
    );
    CREATE INDEX IX_WhatsAppTemplates_Active_Category
        ON dbo.WhatsAppTemplates(IsActive, Category);
END;

IF OBJECT_ID(N'dbo.WhatsAppConsents', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WhatsAppConsents
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WhatsAppConsents PRIMARY KEY,
        ParentId INT NOT NULL,
        PhoneNumber NVARCHAR(30) NOT NULL,
        IsOptedIn BIT NOT NULL,
        ConsentedAt DATETIME2 NULL,
        ConsentSource NVARCHAR(300) NOT NULL,
        UpdatedBy NVARCHAR(100) NOT NULL,
        UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_WhatsAppConsents_UpdatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_WhatsAppConsents_Parent UNIQUE (ParentId),
        CONSTRAINT FK_WhatsAppConsents_Parents FOREIGN KEY (ParentId)
            REFERENCES dbo.Parents(Id)
    );
END;

IF OBJECT_ID(N'dbo.WhatsAppClassMappings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WhatsAppClassMappings
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WhatsAppClassMappings PRIMARY KEY,
        ClassRoomId INT NOT NULL,
        DisplayName NVARCHAR(150) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_WhatsAppClassMappings_Active DEFAULT 1,
        UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_WhatsAppClassMappings_UpdatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy NVARCHAR(100) NOT NULL,
        CONSTRAINT UQ_WhatsAppClassMappings_Class UNIQUE (ClassRoomId),
        CONSTRAINT FK_WhatsAppClassMappings_ClassRooms FOREIGN KEY (ClassRoomId)
            REFERENCES dbo.ClassRooms(Id)
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.WhatsAppTemplates)
BEGIN
    INSERT dbo.WhatsAppTemplates(Name, Category, Body, ProviderTemplateName, LanguageCode,
        IsMetaApproved, IsActive, CreatedBy)
    VALUES
      (N'تذكير بقسط', N'مالي', N'السلام عليكم {guardianName}، نذكّركم بأن المبلغ المستحق للطالب {studentName} في {class} هو {balance}. للتفاصيل يرجى التواصل مع الإدارة.', NULL, N'ar', 0, 1, N'system'),
      (N'إشعار غياب', N'حضور', N'السلام عليكم {guardianName}، نحيطكم علماً بغياب الطالب {studentName} عن المدرسة بتاريخ {date}.', NULL, N'ar', 0, 1, N'system'),
      (N'استدعاء ولي أمر', N'إداري', N'السيد/ة {guardianName}، نرجو حضوركم إلى المدرسة لمتابعة أمر يخص الطالب {studentName} من {class}. يرجى التواصل لتحديد الموعد.', NULL, N'ar', 0, 1, N'system'),
      (N'إعلان عطلة', N'إعلان', N'تعلن إدارة مدرسة نور الملائكة عن عطلة بتاريخ {date}. {extra}', NULL, N'ar', 0, 1, N'system'),
      (N'اجتماع أولياء الأمور', N'إعلان', N'تدعوكم إدارة المدرسة لاجتماع أولياء الأمور للطالب {studentName} من {class} يوم {date} الساعة {time}.', NULL, N'ar', 0, 1, N'system');
END;

COMMIT TRANSACTION;
