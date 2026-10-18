<#
.SYNOPSIS
    يولّد مفاتيح machineKey جديدة صالحة لـ ASP.NET Web.config.
    validationKey : 64 بايت → 128 حرف hex  (HMACSHA256)
    decryptionKey : 32 بايت → 64 حرف hex   (AES-256)

.USAGE
    .\Generate-MachineKey.ps1

    انسخ الناتج إلى Web.Release.config في بيئة الإنتاج.
    لا تحفظ المفاتيح في أي ملف داخل الـ repository.
#>

$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()

$validationBytes = New-Object byte[] 64
$rng.GetBytes($validationBytes)
$validationKey = -join ($validationBytes | ForEach-Object { $_.ToString("x2") })

$decryptionBytes = New-Object byte[] 32
$rng.GetBytes($decryptionBytes)
$decryptionKey = -join ($decryptionBytes | ForEach-Object { $_.ToString("x2") })

$rng.Dispose()

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  مفاتيح machineKey الجديدة" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "validationKey ($($validationBytes.Length) bytes / $($validationKey.Length) chars):" -ForegroundColor Yellow
Write-Host "  $validationKey" -ForegroundColor White
Write-Host ""
Write-Host "decryptionKey ($($decryptionBytes.Length) bytes / $($decryptionKey.Length) chars):" -ForegroundColor Yellow
Write-Host "  $decryptionKey" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  انسخ القيم أعلاه إلى Web.Release.config" -ForegroundColor Green
Write-Host "  ولا تحفظها في أي ملف داخل Git"           -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
