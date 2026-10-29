/*
   Noor Al Malaika School - WhatsApp delivery outcome safety
   Apply after 20261022_WhatsAppP0Hardening.sql, once, on a backed-up database.
   Safe to re-run.

   SendStartedAt distinguishes a claim interrupted before network submission (safe to
   requeue) from a request interrupted after submission began (outcome must be reviewed).
*/
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH(N'dbo.WhatsAppMessages', N'SendStartedAt') IS NULL
    ALTER TABLE dbo.WhatsAppMessages ADD SendStartedAt DATETIME2 NULL;

-- Existing claims may already have reached Meta before this migration is applied.
EXEC sys.sp_executesql N'UPDATE dbo.WhatsAppMessages
   SET SendStartedAt = COALESCE(ClaimedAt, CreatedAt)
 WHERE Status = N''Sending'' AND SendStartedAt IS NULL;';

COMMIT TRANSACTION;
