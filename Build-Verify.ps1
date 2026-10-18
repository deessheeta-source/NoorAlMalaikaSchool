[CmdletBinding()]
param(
    [ValidateSet('Debug','Release')]
    [string]$Configuration = 'Release'
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'NoorAlMalaikaSchool.csproj'

function Find-MSBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $install = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath 2>$null
        if ($install) {
            $candidates = @(
                (Join-Path $install 'MSBuild\Current\Bin\MSBuild.exe'),
                (Join-Path $install 'MSBuild\Current\Bin\amd64\MSBuild.exe')
            )
            foreach ($c in $candidates) { if (Test-Path $c) { return $c } }
        }
    }
    $candidates = @(
        "$env:ProgramFiles\Microsoft Visual Studio\2026\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles\Microsoft Visual Studio\2026\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles\Microsoft Visual Studio\2026\Community\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    )
    foreach ($c in $candidates) { if (Test-Path $c) { return $c } }
    $cmd = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    return $null
}

Write-Host '=== NoorAlMalaikaSchool Web Forms Build Verification ===' -ForegroundColor Cyan
Write-Host "Configuration: $Configuration"
Write-Host "Project: $project"

if (-not (Test-Path $project)) { throw "Project file not found: $project" }
$msbuild = Find-MSBuild
if (-not $msbuild) {
    Write-Host 'MSBuild was not found.' -ForegroundColor Red
    Write-Host 'Install Visual Studio 2022/2026 with the ASP.NET and web development workload, then run this script from Windows.' -ForegroundColor Yellow
    exit 2
}
Write-Host "MSBuild: $msbuild" -ForegroundColor Green

$args = @(
    $project,
    '/restore',
    '/t:Build',
    "/p:Configuration=$Configuration",
    '/p:Platform=AnyCPU',
    '/m',
    '/verbosity:minimal'
)
& $msbuild @args
if ($LASTEXITCODE -ne 0) {
    Write-Host "BUILD FAILED (exit code $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}

$dll = Join-Path $root "bin\NoorAlMalaikaSchool.dll"
if (-not (Test-Path $dll)) {
    Write-Host "Build command succeeded but output DLL was not found: $dll" -ForegroundColor Red
    exit 3
}

Write-Host ''
Write-Host 'BUILD SUCCEEDED.' -ForegroundColor Green
Write-Host "Output: $dll"
Write-Host ''
Write-Host 'Database migration is intentionally not executed by this script.' -ForegroundColor Yellow
Write-Host 'After taking a backup (Database\tools\Backup-NoorDatabase.ps1), apply new scripts in Database\Migrations\ per Database\MIGRATIONS_GUIDE.md.'
