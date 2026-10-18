/*
   Noor Al Malaika School - WhatsApp module enhancements
   Apply after 20261015_AddWhatsAppModule.sql, once, on a backed-up database.
   Adds campaign audience/option columns and explicit consent revocation data.
*/
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH(N'dbo.WhatsAppCampaigns', N'AudienceType') IS NULL
    ALTER TABLE dbo.WhatsAppCampaigns ADD AudienceType NVARCHAR(20) NULL;

IF COL_LENGTH(N'dbo.WhatsAppCampaigns', N'IncludeBalance') IS NULL
    ALTER TABLE dbo.WhatsAppCampaigns ADD IncludeBalance BIT NOT NULL
        CONSTRAINT DF_WhatsAppCampaigns_IncludeBalance DEFAULT 0;

IF COL_LENGTH(N'dbo.WhatsAppCampaigns', N'IncludeAttendance') IS NULL
    ALTER TABLE dbo.WhatsAppCampaigns ADD IncludeAttendance BIT NOT NULL
        CONSTRAINT DF_WhatsAppCampaigns_IncludeAttendance DEFAULT 0;

IF COL_LENGTH(N'dbo.WhatsAppCampaigns', N'IncludeGrades') IS NULL
    ALTER TABLE dbo.WhatsAppCampaigns ADD IncludeGrades BIT NOT NULL
        CONSTRAINT DF_WhatsAppCampaigns_IncludeGrades DEFAULT 0;

IF COL_LENGTH(N'dbo.WhatsAppCampaigns', N'AppendDailyLessons') IS NULL
    ALTER TABLE dbo.WhatsAppCampaigns ADD AppendDailyLessons BIT NOT NULL
        CONSTRAINT DF_WhatsAppCampaigns_AppendDailyLessons DEFAULT 0;

IF COL_LENGTH(N'dbo.WhatsAppConsents', N'RevokedAt') IS NULL
    ALTER TABLE dbo.WhatsAppConsents ADD RevokedAt DATETIME2 NULL;

IF COL_LENGTH(N'dbo.WhatsAppConsents', N'RevokeReason') IS NULL
    ALTER TABLE dbo.WhatsAppConsents ADD RevokeReason NVARCHAR(300) NULL;

COMMIT TRANSACTION;
