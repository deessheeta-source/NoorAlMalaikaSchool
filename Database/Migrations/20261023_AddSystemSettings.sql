/*
    20261023_AddSystemSettings.sql
    ------------------------------------------------------------------
    Adds the key/value SystemSettings table used by the admin settings
    screen for non-secret WhatsApp options (manager phone, default
    country code, reply consent, manual links).

    Secrets must NOT be stored here. They stay in IIS environment
    variables or a server-side secret store.

    Idempotent: safe to run more than once.
*/

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.SystemSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SystemSettings
    (
        [Key]       NVARCHAR(100) NOT NULL
            CONSTRAINT PK_SystemSettings PRIMARY KEY,
        [Value]     NVARCHAR(MAX) NULL,
        UpdatedAt   DATETIME2(0) NOT NULL
            CONSTRAINT DF_SystemSettings_UpdatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy   NVARCHAR(100) NOT NULL
            CONSTRAINT DF_SystemSettings_UpdatedBy DEFAULT N'system'
    );

    PRINT N'Created dbo.SystemSettings.';
END
ELSE
BEGIN
    PRINT N'dbo.SystemSettings already exists.';
END;
