<#
    Export-Schema.ps1
    ---------------------------------------------------------------------------
    Runs Database\tools\Generate-SchemaScript.sql and writes the resulting
    DDL to Database\000_FullSchema.sql.

    ADO.NET is used instead of sqlcmd because sqlcmd cannot both keep full
    nvarchar(max) line widths (-y 0) and suppress its column headers (-h -1);
    those two options are mutually exclusive, and truncating every DDL
    statement at 256 characters would silently corrupt the script.

    Usage:
        powershell -ExecutionPolicy Bypass -File Database\tools\Export-Schema.ps1
#>

[CmdletBinding()]
param(
    [string]$ServerInstance = 'SHEETAGECOL\SQLSERVER2026',
    [string]$DatabaseName   = 'NoorAlMalaikaSchool',
    [string]$GeneratorPath,
    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'

# $PSScriptRoot is not yet populated while param defaults are being evaluated,
# so resolve the paths here from the script's own location.
$dbFolder  = Split-Path -Parent $PSScriptRoot
if (-not $dbFolder) { $dbFolder = Split-Path -Parent $MyInvocation.MyCommand.Path }
if (-not $GeneratorPath) { $GeneratorPath = Join-Path $dbFolder 'tools\Generate-SchemaScript.sql' }
if (-not $OutputPath)    { $OutputPath    = Join-Path $dbFolder '000_FullSchema.sql' }

if (-not (Test-Path -LiteralPath $GeneratorPath)) {
    throw "Generator not found: $GeneratorPath"
}

$sql = Get-Content -LiteralPath $GeneratorPath -Raw -Encoding UTF8

$cs = 'Server={0};Database={1};Integrated Security=True;TrustServerCertificate=True;Connect Timeout=30' -f $ServerInstance, $DatabaseName
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()

try {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText   = $sql
    $cmd.CommandTimeout = 600

    $lines = New-Object System.Collections.Generic.List[string]
    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        if (-not $reader.IsDBNull(0)) { $lines.Add([string]$reader.GetValue(0)) }
    }
    $reader.Close()
}
finally {
    $conn.Close()
}

if ($lines.Count -eq 0) {
    throw 'The generator returned no lines - nothing was written.'
}

# UTF-8 without BOM, CRLF line endings: safe for sqlcmd and for editors.
$text = ($lines -join "`r`n") + "`r`n"
[System.IO.File]::WriteAllText($OutputPath, $text, (New-Object System.Text.UTF8Encoding $false))

Write-Output ("Wrote {0}" -f $OutputPath)
Write-Output ("  lines : {0}" -f $lines.Count)
Write-Output ("  size  : {0} KB" -f [math]::Round((Get-Item -LiteralPath $OutputPath).Length / 1KB, 1))
