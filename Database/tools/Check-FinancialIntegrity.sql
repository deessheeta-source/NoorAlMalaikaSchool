-- ============================================================
-- Check-FinancialIntegrity.sql
-- فحص تساوي وسلامة البيانات المالية لقاعدة NoorAlMalaikaSchool
--
-- قواعد مُتحقَّق منها على البيانات الحالية (2026-10-18، 63 فاتورة):
--   NetAmount = CASE WHEN TotalAmount-DiscountAmount<0 THEN 0 ELSE TotalAmount-DiscountAmount END
--   SUM(InvoiceLines.Amount) per invoice = TotalAmount
--   SUM(Payments.Amount) per invoice (InvoiceId<>NULL) = PaidAmount
--   Status coherence (انظر أدناه)
--
-- التشغيل:
--   Database\tools\Invoke-FinancialCheck.ps1
--   أو يدوياً: sqlcmd -S <server> -d NoorAlMalaikaSchool -E -W -b -i Check-FinancialIntegrity.sql
--
-- المخرجات: جدول ملخص (CheckName/IssueCount/Result) ثم تفاصيل أي فشل.
-- أي IssueCount > 0 يعني انحرافاً حقيقياً ظهر بعد تاريخ الفحص الأول.
-- Status codes (Models\Enums.cs InvoiceStatus):
--   0=Draft 1=Issued 2=PartiallyPaid 3=Paid 4=Cancelled
-- ============================================================
SET NOCOUNT ON;

-- ------------------------------------------------------------
-- الملخص: فحص واحد لكل خطر
-- ------------------------------------------------------------
;WITH Checks AS (
    SELECT '01_NetAmount_Formula' AS CheckName,
           CAST(COUNT(*) AS int) AS IssueCount
    FROM Invoices
    WHERE NetAmount <> CASE WHEN TotalAmount - DiscountAmount < 0
                            THEN 0 ELSE TotalAmount - DiscountAmount END

    UNION ALL
    SELECT '02_InvoiceLines_Match_Total',
           CAST(COUNT(*) AS int)
    FROM (SELECT i.Id
          FROM Invoices i
          LEFT JOIN InvoiceLines l ON l.InvoiceId = i.Id
          GROUP BY i.Id, i.TotalAmount
          HAVING ABS(ISNULL(SUM(l.Amount), 0) - i.TotalAmount) > 0.01) x

    UNION ALL
    SELECT '03_Payments_Match_PaidAmount',
           CAST(COUNT(*) AS int)
    FROM (SELECT i.Id
          FROM Invoices i
          LEFT JOIN Payments p ON p.InvoiceId = i.Id
          GROUP BY i.Id, i.PaidAmount
          HAVING ABS(ISNULL(SUM(p.Amount), 0) - i.PaidAmount) > 0.01) x

    UNION ALL
    SELECT '04_PaidStatus_NotFullyPaid',
           CAST(COUNT(*) AS int)
    FROM Invoices
    WHERE Status = 3 AND ABS(PaidAmount - NetAmount) > 0.01

    UNION ALL
    SELECT '05_PartialStatus_OutOfRange',
           CAST(COUNT(*) AS int)
    FROM Invoices
    WHERE Status = 2 AND (PaidAmount <= 0 OR PaidAmount >= NetAmount)

    UNION ALL
    SELECT '06_IssuedStatus_HasPayment',
           CAST(COUNT(*) AS int)
    FROM Invoices
    WHERE Status = 1 AND PaidAmount <> 0

    UNION ALL
    SELECT '07_CancelledStatus_HasPayment',
           CAST(COUNT(*) AS int)
    FROM Invoices
    WHERE Status = 4 AND PaidAmount <> 0

    UNION ALL
    SELECT '08_PaidAmount_OutOfRange',
           CAST(COUNT(*) AS int)
    FROM Invoices
    WHERE PaidAmount < 0 OR PaidAmount > NetAmount

    UNION ALL
    SELECT '09_Orphan_InvoiceLines',
           CAST(COUNT(*) AS int)
    FROM InvoiceLines l
    WHERE NOT EXISTS (SELECT 1 FROM Invoices i WHERE i.Id = l.InvoiceId)

    UNION ALL
    SELECT '10_Orphan_Payments',
           CAST(COUNT(*) AS int)
    FROM Payments p
    WHERE (p.InvoiceId IS NOT NULL
           AND NOT EXISTS (SELECT 1 FROM Invoices i WHERE i.Id = p.InvoiceId))
       OR NOT EXISTS (SELECT 1 FROM Students s WHERE s.Id = p.StudentId)
)
SELECT CheckName,
       IssueCount,
       CASE WHEN IssueCount = 0 THEN 'OK' ELSE 'FAIL' END AS Result
FROM Checks
ORDER BY CheckName;

-- ------------------------------------------------------------
-- تفاصيل: الفواتير المخالفة (حتى 100 صف لكل فئة)
-- ------------------------------------------------------------
SELECT 'DETAIL_02_InvoiceLines_Mismatch' AS Detail,
       i.Id, i.InvoiceNumber, i.TotalAmount, i.PaidAmount, i.Status,
       ISNULL(SUM(l.Amount), 0) AS LinesSum
FROM Invoices i
LEFT JOIN InvoiceLines l ON l.InvoiceId = i.Id
GROUP BY i.Id, i.InvoiceNumber, i.TotalAmount, i.PaidAmount, i.Status
HAVING ABS(ISNULL(SUM(l.Amount), 0) - i.TotalAmount) > 0.01
ORDER BY i.Id
OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY;

SELECT 'DETAIL_03_Payments_Mismatch' AS Detail,
       i.Id, i.InvoiceNumber, i.TotalAmount, i.PaidAmount, i.Status,
       ISNULL(SUM(p.Amount), 0) AS PaymentsSum
FROM Invoices i
LEFT JOIN Payments p ON p.InvoiceId = i.Id
GROUP BY i.Id, i.InvoiceNumber, i.TotalAmount, i.PaidAmount, i.Status
HAVING ABS(ISNULL(SUM(p.Amount), 0) - i.PaidAmount) > 0.01
ORDER BY i.Id
OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY;

SELECT 'DETAIL_StatusOrRange' AS Detail,
       Id, InvoiceNumber, TotalAmount, DiscountAmount, NetAmount, PaidAmount, Status
FROM Invoices
WHERE (Status = 3 AND ABS(PaidAmount - NetAmount) > 0.01)
   OR (Status = 2 AND (PaidAmount <= 0 OR PaidAmount >= NetAmount))
   OR (Status = 1 AND PaidAmount <> 0)
   OR (Status = 4 AND PaidAmount <> 0)
   OR PaidAmount < 0 OR PaidAmount > NetAmount
ORDER BY Id
OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY;

SELECT 'DETAIL_Orphans' AS Detail,
       'InvoiceLines' AS TableName, CAST(l.Id AS nvarchar(20)) AS RowId,
       CAST(l.InvoiceId AS nvarchar(20)) AS RefId, l.Description AS Extra
FROM InvoiceLines l
WHERE NOT EXISTS (SELECT 1 FROM Invoices i WHERE i.Id = l.InvoiceId)
UNION ALL
SELECT 'DETAIL_Orphans', 'Payments', CAST(p.Id AS nvarchar(20)),
       CAST(p.InvoiceId AS nvarchar(20)), p.Notes
FROM Payments p
WHERE (p.InvoiceId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM Invoices i WHERE i.Id = p.InvoiceId))
   OR NOT EXISTS (SELECT 1 FROM Students s WHERE s.Id = p.StudentId)
ORDER BY TableName, RowId
OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY;
