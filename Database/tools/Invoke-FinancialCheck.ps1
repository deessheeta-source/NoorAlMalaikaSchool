<#
.SYNOPSIS
    Invoke-FinancialCheck.ps1 - يشغّل فحص التساوي المالي ويرسل النتيجة.

.DESCRIPTION
    ينفّذ Check-FinancialIntegrity.sql عبر sqlcmd ويعرض جدول الملخص ثم
    التفاصيل. يُخرج برمز 1 إذا ظهرت أي حالة FAIL (صالح للاستخدام في
    جدولة أسبوعية أو أي إنذار).

.EXAMPLE
    .\Invoke-FinancialCheck.ps1
    .\Invoke-FinancialCheck.ps1 -OutFile "D:\Reports\fincheck_$(Get-Date -f yyyyMMdd).txt"
#>
[CmdletBinding()]
param(
    [string]$Server = 'SHEETAGECOL\SQLSERVER2026',
    [string]$Database = 'NoorAlMalaikaSchool',
    [string]$OutFile = ''
)

$ErrorActionPreference = 'Stop'

$ScriptDir = $PSScriptRoot
$SqlFile   = Join-Path $ScriptDir 'Check-FinancialIntegrity.sql'

function Find-SqlCmd {
    $candidates = @(
        (Join-Path $env:ProgramFiles 'Microsoft SQL Server\Client SDK\ODBC\*\Tools\Binn\SQLCMD.EXE'),
        (Join-Path ${env:ProgramFiles(x86)} 'Microsoft SQL Server\Client SDK\ODBC\*\Tools\Binn\SQLCMD.EXE')
    )
    foreach ($pattern in $candidates) {
        if ($pattern) {
            $hit = Get-Item -Path $pattern -ErrorAction SilentlyContinue | Sort-Object FullName -Descending | Select-Object -First 1
            if ($hit) { return $hit.FullName }
        }
    }
    $onPath = Get-Command sqlcmd.exe -ErrorAction SilentlyContinue
    if ($onPath) { return $onPath.Source }
    return $null
}

try {
    if (-not (Test-Path -LiteralPath $SqlFile)) {
        Write-Host "SQL file not found: $SqlFile" -ForegroundColor Red
        exit 2
    }

    $sqlcmd = Find-SqlCmd
    if (-not $sqlcmd) {
        Write-Host 'sqlcmd.exe not found (SQL Server command line utilities missing).' -ForegroundColor Red
        exit 2
    }

    Write-Host "Financial integrity check - $Database @ $Server" -ForegroundColor Cyan
    Write-Host ('=' * 70)

    $output = & $sqlcmd -S $Server -E -W -b -i $SqlFile 2>&1
    $exitCode = $LASTEXITCODE

    foreach ($line in $output) {
        Write-Host $line
    }

    if ($exitCode -ne 0) {
        Write-Host "sqlcmd failed with exit code $exitCode." -ForegroundColor Red
        if ($OutFile) { $output | Out-File -LiteralPath $OutFile -Encoding UTF8 }
        exit $exitCode
    }

    $failCount = ($output | Where-Object { $_ -match '\bFAIL\b' }).Count
    $okCount   = ($output | Where-Object { $_ -match '\bOK\b' }).Count

    Write-Host ('=' * 70)
    if ($failCount -eq 0) {
        Write-Host "RESULT: ALL OK ($okCount checks passed)." -ForegroundColor Green
    }
    else {
        Write-Host "RESULT: $failCount check(s) FAILED - see details above." -ForegroundColor Red
    }

    if ($OutFile) {
        $output | Out-File -LiteralPath $OutFile -Encoding UTF8
        Write-Host "Report saved: $OutFile"
    }

    if ($failCount -gt 0) { exit 1 }
    exit 0
}
catch {
    Write-Host ("EXCEPTION: " + $_.Exception.Message) -ForegroundColor Red
    exit 1
}
