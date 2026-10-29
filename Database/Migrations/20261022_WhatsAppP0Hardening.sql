/*
   Noor Al Malaika School - WhatsApp P0 hardening
   Apply after 20261015_AddWhatsAppModule.sql and 20261016_EnhanceWhatsAppModule.sql,
   once, on a backed-up database. Safe to re-run.

   Adds:
   - retry scheduling (NextAttemptAt) and atomic claim timestamp (ClaimedAt)
   - normalized parent phone (computed, persisted) + index for fast webhook matching
*/
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH(N'dbo.WhatsAppMessages', N'NextAttemptAt') IS NULL
    ALTER TABLE dbo.WhatsAppMessages ADD NextAttemptAt DATETIME2 NULL;

IF COL_LENGTH(N'dbo.WhatsAppMessages', N'ClaimedAt') IS NULL
    ALTER TABLE dbo.WhatsAppMessages ADD ClaimedAt DATETIME2 NULL;

IF OBJECT_ID(N'dbo.ufn_NormalizePhone', N'FN') IS NULL
    EXEC(N'
CREATE FUNCTION dbo.ufn_NormalizePhone(@phone NVARCHAR(50))
RETURNS NVARCHAR(30)
WITH SCHEMABINDING
AS
BEGIN
    DECLARE @hadPlus BIT = CASE WHEN LEFT(LTRIM(ISNULL(@phone, N'''')), 1) = N''+'' THEN 1 ELSE 0 END;
    DECLARE @digits NVARCHAR(50) = ISNULL(@phone, N'''');
    SET @digits = REPLACE(@digits, N'' '', N'''');
    SET @digits = REPLACE(@digits, N''+'', N'''');
    SET @digits = REPLACE(@digits, N''-'', N'''');
    SET @digits = REPLACE(@digits, N''('', N'''');
    SET @digits = REPLACE(@digits, N'')'', N'''');
    SET @digits = REPLACE(@digits, N''.'', N'''');
    SET @digits = REPLACE(@digits, N''/'', N'''');
    SET @digits = REPLACE(@digits, NCHAR(9), N'''');
    IF @digits = N'''' RETURN NULL;
    IF @hadPlus = 1 RETURN @digits;
    IF @digits LIKE N''00%'' RETURN SUBSTRING(@digits, 3, 30);
    IF @digits LIKE N''218%'' RETURN @digits;
    IF @digits LIKE N''0%'' RETURN N''218'' + SUBSTRING(@digits, 2, 30);
    RETURN N''218'' + @digits;
END;
');

IF COL_LENGTH(N'dbo.Parents', N'PhoneNormalized') IS NULL
    ALTER TABLE dbo.Parents ADD PhoneNormalized AS (dbo.ufn_NormalizePhone(Phone)) PERSISTED;

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE object_id = OBJECT_ID(N'dbo.Parents')
                 AND name = N'IX_Parents_PhoneNormalized')
    CREATE INDEX IX_Parents_PhoneNormalized ON dbo.Parents(PhoneNormalized);

COMMIT TRANSACTION;
