<#
.SYNOPSIS
    يولّد مفاتيح machineKey جديدة صالحة لـ ASP.NET ويحفظها في ملف خارجي.

.DESCRIPTION
    validationKey : 64 بايت → 128 حرف hex  (HMACSHA256)
    decryptionKey : 32 بايت → 64 حرف hex   (AES-256)

    بدون -WriteTo يُطبع الناتج فقط. مع -WriteTo يُكتب ملف configSource
    الذي يقرأه Web.config عبر:
        <machineKey configSource="Config\machineKey.config" />

.PARAMETER WriteTo
    مسار ملف الإخراج. الافتراضي عند تحديده: Config\machineKey.config

.PARAMETER Force
    استبدال ملف موجود. بدونه يفشل السكربت إن كان الملف موجوداً.

.EXAMPLE
    .\Generate-MachineKey.ps1 -WriteTo Config\machineKey.config -Force

.NOTES
    تدوير المفتاح يُسجّل خروج جميع المستخدمين ويُبطل ViewState القائم،
    فافعله خارج أوقات العمل. لا ترفع الملف الناتج إلى Git.
#>

[CmdletBinding()]
param(
    [string]$WriteTo,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path))

$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()

$validationBytes = New-Object byte[] 64
$rng.GetBytes($validationBytes)
$validationKey = -join ($validationBytes | ForEach-Object { $_.ToString("x2") })

$decryptionBytes = New-Object byte[] 32
$rng.GetBytes($decryptionBytes)
$decryptionKey = -join ($decryptionBytes | ForEach-Object { $_.ToString("x2") })

$rng.Dispose()

if ($WriteTo) {
    if (-not [System.IO.Path]::IsPathRooted($WriteTo)) {
        $WriteTo = Join-Path $root $WriteTo
    }

    if ((Test-Path $WriteTo) -and -not $Force) {
        throw "الملف موجود مسبقاً: $WriteTo  (استخدم -Force للاستبدال)"
    }

    $directory = Split-Path -Parent $WriteTo
    if (-not (Test-Path $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $content = @"
<?xml version="1.0" encoding="utf-8"?>
<!--
  machineKey — تم توليده بواسطة Database\tools\Generate-MachineKey.ps1
  بتاريخ $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss').
  لا يُرفع هذا الملف إلى Git.
-->
<machineKey validationKey="$validationKey"
            decryptionKey="$decryptionKey"
            validation="HMACSHA256"
            decryption="AES" />
"@

    Set-Content -Path $WriteTo -Value $content -Encoding UTF8
    Write-Host ""
    Write-Host "تم حفظ المفتاح في: $WriteTo" -ForegroundColor Green
    Write-Host "أعد تشغيل Application Pool ليقرأه التطبيق." -ForegroundColor Yellow
    Write-Host "تنبيه: كل الجلسات الحالية ستُفصل إذا كان هذا المفتاح مختلفاً عن السابق." -ForegroundColor Red
    Write-Host ""
    return
}

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
Write-Host "  احفظ القيم في Config\machineKey.config" -ForegroundColor Green
Write-Host "  أو أعد التشغيل مع: -WriteTo Config\machineKey.config" -ForegroundColor Green
Write-Host "  ولا تحفظها في أي ملف داخل Git" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
