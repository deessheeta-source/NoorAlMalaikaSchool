<#
.SYNOPSIS
    يبني ويشغّل اختبارات الحسابات (بدون قاعدة بيانات).

.DESCRIPTION
    يترجم Tests\InvoiceCalculatorTests.cs مقابل bin\NoorAlMalaikaSchool.dll
    باستخدام Roslyn csc المرفق مع Visual Studio، ثم يشغّل الناتج.
    يخرج برمز 1 عند أي فشل (مناسب للجدولة/CI).

.PARAMETER SkipBuild
    لا يعيد بناء المشروع قبل الاختبار (يستخدم DLL الحالي).

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File Tests\Run-Tests.ps1
#>

[CmdletBinding()]
param(
    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root 'NoorAlMalaikaSchool.csproj'
$dll = Join-Path $root 'bin\NoorAlMalaikaSchool.dll'
$outDir = Join-Path $root 'bin'
$outExe = Join-Path $outDir 'InvoiceCalculatorTests.exe'

if (-not $SkipBuild) {
    & (Join-Path $root 'Build-Verify.ps1') -Configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host 'Build failed - tests were not run.' -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

if (-not (Test-Path $dll)) {
    throw "Assembly not found: $dll (run the build first)"
}

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$csc = $null
if (Test-Path $vswhere) {
    $install = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath 2>$null
    if ($install) {
        $candidate = Join-Path $install 'MSBuild\Current\Bin\Roslyn\csc.exe'
        if (Test-Path $candidate) { $csc = $candidate }
    }
}
if (-not $csc) {
    $candidate = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
    if (Test-Path $candidate) { $csc = $candidate }
}
if (-not $csc) {
    throw 'csc.exe not found (install Visual Studio or the .NET Framework compiler).'
}

New-Item -ItemType Directory -Path $outDir -Force | Out-Null

Write-Host "Compiling tests with $csc" -ForegroundColor Cyan
& $csc /nologo /langversion:latest /target:exe "/out:$outExe" "/r:$dll" (Join-Path $PSScriptRoot 'InvoiceCalculatorTests.cs')
if ($LASTEXITCODE -ne 0) {
    Write-Host 'Test compilation FAILED.' -ForegroundColor Red
    exit $LASTEXITCODE
}

& $outExe
exit $LASTEXITCODE
