<#
.SYNOPSIS
    تدوير وتنظيف سجلات التطبيق تحت App_Data.

.DESCRIPTION
    نفس منطق Services\LogMaintenanceService.cs (الذي يعمل عند بدء التطبيق)،
    لكن كسكربت مستقل يمكن جدولته:
      - يدوّر app-errors.log و delete-error.log عندما يتجاوز أي منهما 5 MB.
      - يحتفظ بآخر 10 أرشيفات لكل سجل.
      - يحذف ملفات Logs الأقدم من مدة الاحتفاظ (افتراضياً 90 يوماً).

.PARAMETER RegisterTask
    يسجّل مهمة يومية 03:00 باسم NoorAlMalaikaSchool_LogMaintenance.

.PARAMETER UnregisterTask
    يحذف المهمة المجدولة.

.EXAMPLE
    .\Invoke-LogMaintenance.ps1
    .\Invoke-LogMaintenance.ps1 -RetentionDays 30
    .\Invoke-LogMaintenance.ps1 -RegisterTask
#>

[CmdletBinding(DefaultParameterSetName = 'Run')]
param(
    [string]$AppDataPath,
    [ValidateRange(1, 3650)]
    [int]$RetentionDays = 90,
    [long]$MaxBytes = 5MB,
    [ValidateRange(1, 1000)]
    [int]$MaxArchives = 10,

    [Parameter(ParameterSetName = 'Register')]
    [switch]$RegisterTask,

    [Parameter(ParameterSetName = 'Unregister')]
    [switch]$UnregisterTask
)

$ErrorActionPreference = 'Stop'
$TaskName = 'NoorAlMalaikaSchool_LogMaintenance'
$ProjectRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

if (-not $AppDataPath) {
    $AppDataPath = Join-Path $ProjectRoot 'App_Data'
}

if ($RegisterTask) {
    $scriptPath = Join-Path $PSScriptRoot 'Invoke-LogMaintenance.ps1'
    $action = New-ScheduledTaskAction -Execute 'powershell.exe' `
        -Argument "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" -RetentionDays $RetentionDays"
    $trigger = New-ScheduledTaskTrigger -Daily -At 03:00
    $principal = New-ScheduledTaskPrincipal -UserId "$env:USERDOMAIN\$env:USERNAME" -LogonType S4U -RunLevel Highest

    Register-ScheduledTask -TaskName $TaskName -Action $action -Trigger $trigger `
        -Principal $principal -Force `
        -Description 'Rotate and purge NoorAlMalaikaSchool application logs (App_Data).' | Out-Null

    Write-Host "Scheduled task '$TaskName' registered (daily 03:00)." -ForegroundColor Green
    exit 0
}

if ($UnregisterTask) {
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false -ErrorAction SilentlyContinue
    Write-Host "Scheduled task '$TaskName' removed." -ForegroundColor Yellow
    exit 0
}

if (-not (Test-Path -LiteralPath $AppDataPath)) {
    throw "App_Data not found: $AppDataPath"
}

$archive = Join-Path $AppDataPath 'Logs\archive'
New-Item -ItemType Directory -Path $archive -Force | Out-Null

function Rotate-Log {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) { return }
    $file = Get-Item -LiteralPath $Path
    if ($file.Length -lt $MaxBytes) { return }

    $name = [IO.Path]::GetFileNameWithoutExtension($file.Name)
    $target = Join-Path $archive ("{0}-{1}.log" -f $name, (Get-Date -Format 'yyyyMMdd_HHmmss'))

    try {
        Move-Item -LiteralPath $file.FullName -Destination $target -Force
    }
    catch {
        Copy-Item -LiteralPath $file.FullName -Destination $target -Force
        Set-Content -LiteralPath $file.FullName -Value $null -Encoding UTF8
    }

    Write-Host "Rotated: $($file.Name) -> $target" -ForegroundColor Yellow
}

Rotate-Log (Join-Path $AppDataPath 'app-errors.log')
Rotate-Log (Join-Path $AppDataPath 'delete-error.log')

Get-ChildItem -LiteralPath $archive -Filter '*.log' -ErrorAction SilentlyContinue |
    Group-Object { ($_.Name -split '-')[0] } |
    ForEach-Object {
        $_.Group | Sort-Object LastWriteTime -Descending | Select-Object -Skip $MaxArchives | ForEach-Object {
            Remove-Item -LiteralPath $_.FullName -Force -ErrorAction SilentlyContinue
            Write-Host "Removed old archive: $($_.Name)" -ForegroundColor DarkGray
        }
    }

$cutoff = (Get-Date).AddDays(-$RetentionDays)
Get-ChildItem -LiteralPath (Join-Path $AppDataPath 'Logs') -Recurse -File -ErrorAction SilentlyContinue |
    Where-Object { $_.LastWriteTime -lt $cutoff } |
    ForEach-Object {
        Remove-Item -LiteralPath $_.FullName -Force -ErrorAction SilentlyContinue
        Write-Host "Purged expired: $($_.Name)" -ForegroundColor DarkGray
    }

Write-Host 'Log maintenance completed.' -ForegroundColor Green
