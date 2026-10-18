/* ===========================================================================
   Generate-SchemaScript.sql
   ---------------------------------------------------------------------------
   Produces a full, dependency-ordered DDL snapshot of the database on
   standard output, one line per row in the "line" column.

   Capture it with Database/tools/Export-Schema.ps1, which runs this file
   through ADO.NET and writes Database/000_FullSchema.sql. (ADO.NET is used
   rather than sqlcmd because sqlcmd cannot both keep full nvarchar(max) line
   widths -y 0 and suppress its headers -h -1; the two flags are exclusive.)

   The output is a genuine snapshot of what is really in the database, not a
   re-derivation of the EF model, so it also captures what EF6 cannot express:
   filtered indexes, hand-tuned composite indexes, sequences, and the tables
   that have no model behind them.

   Every statement carries a DROP guard, so the script can be re-run against a
   database that already has the schema. It never writes data.
   =========================================================================== */

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @lines TABLE (ord int IDENTITY(1,1) PRIMARY KEY, line nvarchar(max));
DECLARE @db sysname = DB_NAME();

/* ---- header ------------------------------------------------------------- */
INSERT @lines(line) VALUES
 (N'-- ==========================================================================='),
 (N'-- ' + QUOTENAME(@db) + N' - full schema snapshot (schema only, no data)'),
 (N'-- Server    : ' + CONVERT(nvarchar(128), SERVERPROPERTY('ServerName'))),
 (N'-- Database  : ' + @db),
 (N'-- Generated : ' + CONVERT(nvarchar(30), GETDATE(), 126)),
 (N'-- Generator : Database/tools/Generate-SchemaScript.sql'),
 (N'--'),
 (N'-- This snapshot is the authoritative description of the schema. EF6 also'),
 (N'-- builds the tables from Data/SchoolDbContext.cs on first run, but EF'),
 (N'-- cannot express filtered indexes or the hand-tuned indexes, so use this'),
 (N'-- file when provisioning an environment by hand.'),
 (N'--'),
 (N'-- Sequence positions are not captured by a full backup. After restoring a'),
 (N'-- backup, see Database/README.md to restart InvoiceNumberSequence and'),
 (N'-- ReceiptNumberSequence above the numbers the restored data already uses.'),
 (N'-- ==========================================================================='),
 (N''),
 (N'SET QUOTED_IDENTIFIER ON;'),
 (N'GO'),
 (N'');

/* ---- drop guards: children before parents -------------------------------- */
INSERT @lines(line)
SELECT N'IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = ' + QUOTENAME(fk.name, '''') + N') ALTER TABLE ' + QUOTENAME(ps.name) + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
FROM sys.foreign_keys fk
JOIN sys.objects po ON po.object_id = fk.parent_object_id
JOIN sys.schemas ps ON ps.schema_id = po.schema_id;

INSERT @lines(line)
SELECT N'IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = ' + QUOTENAME(cc.name, '''') + N') ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(OBJECT_NAME(cc.parent_object_id)) + N' DROP CONSTRAINT ' + QUOTENAME(cc.name) + N';'
FROM sys.check_constraints cc
JOIN sys.objects po ON po.object_id = cc.parent_object_id
JOIN sys.schemas s ON s.schema_id = po.schema_id;

INSERT @lines(line)
SELECT N'IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = ' + QUOTENAME(kc.name, '''') + N') ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(OBJECT_NAME(kc.parent_object_id)) + N' DROP CONSTRAINT ' + QUOTENAME(kc.name) + N';'
FROM sys.key_constraints kc
JOIN sys.objects po ON po.object_id = kc.parent_object_id
JOIN sys.schemas s ON s.schema_id = po.schema_id;

INSERT @lines(line)
SELECT N'IF EXISTS (SELECT 1 FROM sys.indexes i WHERE i.name = ' + QUOTENAME(i.name, '''') + N' AND i.object_id = ' + CONVERT(nvarchar(12), i.object_id) + N' AND i.index_id > 0) DROP INDEX ' + QUOTENAME(i.name) + N' ON ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N';'
FROM sys.indexes i
JOIN sys.tables t ON t.object_id = i.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE i.index_id > 0 AND i.is_hypothetical = 0 AND i.type <> 0;

INSERT @lines(line)
SELECT N'IF EXISTS (SELECT 1 FROM sys.sequences WHERE name = ' + QUOTENAME(sq.name, '''') + N') DROP SEQUENCE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(sq.name) + N';'
FROM sys.sequences sq
JOIN sys.schemas s ON s.schema_id = sq.schema_id;

INSERT @lines(line)
SELECT N'IF EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON s.schema_id = t.schema_id WHERE s.name = ' + QUOTENAME(s.name, '''') + N' AND t.name = ' + QUOTENAME(t.name, '''') + N' AND t.is_ms_shipped = 0) DROP TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N';'
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE t.is_ms_shipped = 0;

INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- sequences ---------------------------------------------------------- */
INSERT @lines(line)
SELECT N'CREATE SEQUENCE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(sq.name) + CHAR(13) + CHAR(10)
     + N'    AS ' + QUOTENAME(ty.name) + CHAR(13) + CHAR(10)
     + N'    START WITH ' + CONVERT(nvarchar(30), sq.start_value) + CHAR(13) + CHAR(10)
     + N'    INCREMENT BY ' + CONVERT(nvarchar(30), sq.increment) + CHAR(13) + CHAR(10)
     + N'    ' + CASE WHEN sq.is_cycling = 1 THEN 'CYCLE' ELSE 'NO CYCLE' END + CHAR(13) + CHAR(10)
     + N'    MINVALUE ' + CASE WHEN sq.minimum_value IS NOT NULL THEN CONVERT(nvarchar(30), sq.minimum_value) ELSE 'NO MINVALUE' END + CHAR(13) + CHAR(10)
     + N'    MAXVALUE ' + CASE WHEN sq.maximum_value IS NOT NULL THEN CONVERT(nvarchar(30), sq.maximum_value) ELSE 'NO MAXVALUE' END + CHAR(13) + CHAR(10)
     + N';' + CHAR(13) + CHAR(10)
FROM sys.sequences sq
JOIN sys.schemas s ON s.schema_id = sq.schema_id
LEFT JOIN sys.types ty ON ty.user_type_id = sq.user_type_id
WHERE ty.name IS NOT NULL;

IF EXISTS (SELECT 1 FROM sys.sequences) INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- tables ------------------------------------------------------------- */
/* Each column is emitted as "," + CRLF + indent; STUFF then drops the leading
   comma, which yields a clean "CREATE TABLE t (\n    [c] type,\n    [c] type\n);" */
INSERT @lines(line)
SELECT N'CREATE TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N' (' + CHAR(13) + CHAR(10)
     + CAST(STUFF((
         SELECT ',' + CHAR(13) + CHAR(10)
                + '    ' + QUOTENAME(c.name) + N' ' + ty.name
                + CASE WHEN c.is_identity = 1
                       THEN N' IDENTITY(' + CONVERT(nvarchar(10), ic.seed_value) + N',' + CONVERT(nvarchar(10), ic.increment_value) + N')'
                       ELSE N'' END
                + CASE WHEN c.is_computed = 1 THEN N' AS ' + cc.definition ELSE N'' END
                + CASE WHEN c.is_nullable = 0 AND c.is_computed = 0 THEN N' NOT NULL' ELSE N'' END
                + CASE WHEN dc.definition IS NOT NULL AND c.is_computed = 0
                       THEN N' CONSTRAINT ' + QUOTENAME(dc.name) + N' DEFAULT ' + dc.definition ELSE N'' END
         FROM sys.columns c
         JOIN sys.types ty ON ty.user_type_id = c.user_type_id
         LEFT JOIN sys.computed_columns cc ON cc.object_id = c.object_id AND cc.column_id = c.column_id
         LEFT JOIN sys.identity_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
         LEFT JOIN sys.default_constraints dc ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
         WHERE c.object_id = t.object_id
         ORDER BY c.column_id
         FOR XML PATH(''), TYPE
       ).value('.', 'nvarchar(max)'), 1, 1, '') AS nvarchar(max))
     + N');' + CHAR(13) + CHAR(10)
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE t.is_ms_shipped = 0
ORDER BY s.name, t.name;

INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- primary keys ------------------------------------------------------- */
INSERT @lines(line)
SELECT N'ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(OBJECT_NAME(kc.parent_object_id)) + N' ADD CONSTRAINT ' + QUOTENAME(kc.name) + N' PRIMARY KEY ' + CHAR(13) + CHAR(10)
     + N'    (' + CAST(STUFF((
         SELECT ', ' + QUOTENAME(col.name)
         FROM sys.index_columns ic
         JOIN sys.columns col ON col.object_id = ic.object_id AND col.column_id = ic.column_id
         WHERE ic.object_id = kc.parent_object_id
           AND ic.index_id = kc.unique_index_id
           AND ic.is_included_column = 0
         ORDER BY ic.key_ordinal
         FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS nvarchar(max))
     + N');' + CHAR(13) + CHAR(10)
FROM sys.key_constraints kc
JOIN sys.objects po ON po.object_id = kc.parent_object_id
JOIN sys.schemas s ON s.schema_id = po.schema_id
WHERE kc.type = 'PK';

IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE type = 'PK') INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- unique constraints ------------------------------------------------- */
INSERT @lines(line)
SELECT N'ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(OBJECT_NAME(kc.parent_object_id)) + N' ADD CONSTRAINT ' + QUOTENAME(kc.name) + N' UNIQUE ' + CHAR(13) + CHAR(10)
     + N'    (' + CAST(STUFF((
         SELECT ', ' + QUOTENAME(col.name)
         FROM sys.index_columns ic
         JOIN sys.columns col ON col.object_id = ic.object_id AND col.column_id = ic.column_id
         WHERE ic.object_id = kc.parent_object_id
           AND ic.index_id = kc.unique_index_id
           AND ic.is_included_column = 0
         ORDER BY ic.key_ordinal
         FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS nvarchar(max))
     + N');' + CHAR(13) + CHAR(10)
FROM sys.key_constraints kc
JOIN sys.objects po ON po.object_id = kc.parent_object_id
JOIN sys.schemas s ON s.schema_id = po.schema_id
WHERE kc.type = 'UQ';

IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE type = 'UQ') INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- check constraints -------------------------------------------------- */
INSERT @lines(line)
SELECT N'ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(OBJECT_NAME(cc.parent_object_id)) + N' ADD CONSTRAINT ' + QUOTENAME(cc.name) + N' CHECK ' + cc.definition + N';' + CHAR(13) + CHAR(10)
FROM sys.check_constraints cc
JOIN sys.objects po ON po.object_id = cc.parent_object_id
JOIN sys.schemas s ON s.schema_id = po.schema_id
WHERE cc.is_disabled = 0;

IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE is_disabled = 0) INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- foreign keys (safe now: every table already exists) ---------------- */
INSERT @lines(line)
SELECT N'ALTER TABLE ' + QUOTENAME(ps.name) + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) + N' ADD CONSTRAINT ' + QUOTENAME(fk.name) + N' FOREIGN KEY ' + CHAR(13) + CHAR(10)
     + N'    (' + CAST(STUFF((
         SELECT ', ' + QUOTENAME(c.name)
         FROM sys.foreign_key_columns fkc
         JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
         WHERE fkc.constraint_object_id = fk.object_id
         ORDER BY fkc.constraint_column_id
         FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS nvarchar(max))
     + N') REFERENCES ' + QUOTENAME(rs.name) + N'.' + QUOTENAME(OBJECT_NAME(fk.referenced_object_id)) + N' (' + CHAR(13) + CHAR(10)
     + CAST(STUFF((
         SELECT ', ' + QUOTENAME(c.name)
         FROM sys.foreign_key_columns fkc
         JOIN sys.columns c ON c.object_id = fkc.referenced_object_id AND c.column_id = fkc.referenced_column_id
         WHERE fkc.constraint_object_id = fk.object_id
         ORDER BY fkc.constraint_column_id
         FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS nvarchar(max))
     + N') ON DELETE ' + CASE fk.delete_referential_action_desc WHEN 'CASCADE' THEN 'CASCADE' WHEN 'SET_NULL' THEN 'SET NULL' WHEN 'SET_DEFAULT' THEN 'SET DEFAULT' ELSE 'NO ACTION' END
     + N' ON UPDATE ' + CASE fk.update_referential_action_desc WHEN 'CASCADE' THEN 'CASCADE' WHEN 'SET_NULL' THEN 'SET NULL' WHEN 'SET_DEFAULT' THEN 'SET DEFAULT' ELSE 'NO ACTION' END
     + N';' + CHAR(13) + CHAR(10)
FROM sys.foreign_keys fk
JOIN sys.objects po ON po.object_id = fk.parent_object_id
JOIN sys.schemas ps ON ps.schema_id = po.schema_id
JOIN sys.objects ro ON ro.object_id = fk.referenced_object_id
JOIN sys.schemas rs ON rs.schema_id = ro.schema_id;

IF EXISTS (SELECT 1 FROM sys.foreign_keys) INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- indexes (including filtered and hand-tuned) ------------------------ */
INSERT @lines(line)
SELECT N'CREATE ' + CASE WHEN i.is_unique = 1 THEN N'UNIQUE ' ELSE N'' END
     + N'INDEX ' + QUOTENAME(i.name) + N' ON ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + CHAR(13) + CHAR(10)
     + N'    (' + CAST(STUFF((
         SELECT ', ' + QUOTENAME(col.name) + CASE WHEN ic.is_descending_key = 1 THEN ' DESC' ELSE '' END
         FROM sys.index_columns ic
         JOIN sys.columns col ON col.object_id = ic.object_id AND col.column_id = ic.column_id
         WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 0
         ORDER BY ic.key_ordinal
         FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS nvarchar(max)) + N')'
     + CASE WHEN EXISTS (SELECT 1 FROM sys.index_columns ic2
                         WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id AND ic2.is_included_column = 1)
            THEN CHAR(13) + CHAR(10) + N'    INCLUDE (' + CAST(STUFF((
                 SELECT ', ' + QUOTENAME(col.name)
                 FROM sys.index_columns ic2
                 JOIN sys.columns col ON col.object_id = ic2.object_id AND col.column_id = ic2.column_id
                 WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id AND ic2.is_included_column = 1
                 ORDER BY ic2.index_column_id
                 FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS nvarchar(max)) + N')'
            ELSE N'' END
     + CASE WHEN i.has_filter = 1 THEN CHAR(13) + CHAR(10) + N'    WHERE ' + i.filter_definition ELSE N'' END
     + CHAR(13) + CHAR(10)
     + N'    WITH (PAD_INDEX = '          + CASE WHEN i.is_padded          = 1 THEN 'ON'  ELSE 'OFF' END
     + N', FILLFACTOR = '             + CONVERT(nvarchar(5), CASE WHEN i.fill_factor BETWEEN 1 AND 100 THEN i.fill_factor ELSE 100 END)
     + N', IGNORE_DUP_KEY = '         + CASE WHEN i.ignore_dup_key     = 1 THEN 'ON'  ELSE 'OFF' END
     + N', ALLOW_ROW_LOCKS = '        + CASE WHEN i.allow_row_locks    = 1 THEN 'ON'  ELSE 'OFF' END
     + N', ALLOW_PAGE_LOCKS = '       + CASE WHEN i.allow_page_locks   = 1 THEN 'ON'  ELSE 'OFF' END
     + N');' + CHAR(13) + CHAR(10)
FROM sys.indexes i
JOIN sys.tables t ON t.object_id = i.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE i.index_id > 0
  AND i.is_hypothetical = 0
  AND i.is_disabled = 0
  AND i.type IN (1, 2)                                        -- clustered / nonclustered B-tree only
  AND i.is_primary_key = 0
  AND i.is_unique_constraint = 0                             -- PK/UQ are emitted in their own sections
ORDER BY s.name, t.name, i.name;

INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- views -------------------------------------------------------------- */
-- sys.views has no `definition` column in SQL Server 2026, so read the module
-- text through OBJECT_DEFINITION, which is version-independent.
INSERT @lines(line)
SELECT N'CREATE VIEW ' + QUOTENAME(s.name) + N'.' + QUOTENAME(v.name) + CHAR(13) + CHAR(10)
     + N'AS' + CHAR(13) + CHAR(10) + OBJECT_DEFINITION(v.object_id) + N';' + CHAR(13) + CHAR(10)
FROM sys.views v
JOIN sys.schemas s ON s.schema_id = v.schema_id
WHERE v.is_ms_shipped = 0;

IF EXISTS (SELECT 1 FROM sys.views WHERE is_ms_shipped = 0) INSERT @lines(line) VALUES (N'GO'), (N'');

/* ---- footer ------------------------------------------------------------- */
INSERT @lines(line) VALUES
 (N'PRINT ''Schema snapshot applied successfully.'''),
 (N'GO');

/* ---- emit, one row per line -------------------------------------------- */
SELECT line FROM @lines ORDER BY ord;
