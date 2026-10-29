#requires -RunAsAdministrator
$ErrorActionPreference = 'Stop'

$taskName = 'NoorAlMalaikaSchool-WhatsAppQueue'
$endpoint = [Environment]::GetEnvironmentVariable('NOOR_WHATSAPP_DISPATCH_URL', 'Machine')
$dispatchKey = [Environment]::GetEnvironmentVariable('NOOR_WHATSAPP_DISPATCH_KEY', 'Machine')
if ([string]::IsNullOrWhiteSpace($endpoint) -or [string]::IsNullOrWhiteSpace($dispatchKey)) {
    throw 'Set the machine-level NOOR_WHATSAPP_DISPATCH_URL and NOOR_WHATSAPP_DISPATCH_KEY first.'
}
if ($dispatchKey.Length -lt 32) {
    throw 'NOOR_WHATSAPP_DISPATCH_KEY must contain at least 32 characters.'
}
try { $dispatchUri = [Uri]$endpoint } catch { throw 'Set a valid absolute WhatsApp dispatch URL.' }
if (-not $dispatchUri.IsAbsoluteUri -or
    -not ($dispatchUri.Scheme -eq 'https' -or
        ($dispatchUri.Scheme -eq 'http' -and $dispatchUri.IsLoopback)) -or
    -not [string]::IsNullOrEmpty($dispatchUri.UserInfo) -or
    -not [string]::IsNullOrEmpty($dispatchUri.Query) -or
    -not [string]::IsNullOrEmpty($dispatchUri.Fragment) -or
    -not $dispatchUri.AbsolutePath.EndsWith('/Pages/WhatsApp/Dispatch.ashx', [StringComparison]::OrdinalIgnoreCase)) {
    throw 'The endpoint must use HTTPS, or HTTP only on localhost, and must not contain credentials or query data.'
}

$dispatchScript = Join-Path $PSScriptRoot 'Invoke-WhatsAppQueue.ps1'
if (-not (Test-Path -LiteralPath $dispatchScript -PathType Leaf)) {
    throw 'Invoke-WhatsAppQueue.ps1 was not found beside this setup script.'
}
if (Get-ScheduledTask -TaskName $taskName -ErrorAction SilentlyContinue) {
    throw "Task '$taskName' already exists. Inspect it before replacing it."
}

$powerShell = Join-Path $env:SystemRoot 'System32\WindowsPowerShell\v1.0\powershell.exe'
$action = New-ScheduledTaskAction -Execute $powerShell -Argument `
    ('-NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -File "' + $dispatchScript + '"')
$trigger = New-ScheduledTaskTrigger -Once -At (Get-Date).AddMinutes(1) `
    -RepetitionInterval (New-TimeSpan -Minutes 1) -RepetitionDuration ([TimeSpan]::MaxValue)
$settings = New-ScheduledTaskSettingsSet -StartWhenAvailable -RunOnlyIfNetworkAvailable `
    -MultipleInstances IgnoreNew -ExecutionTimeLimit (New-TimeSpan -Minutes 5)
$principal = New-ScheduledTaskPrincipal -UserId 'SYSTEM' -LogonType ServiceAccount -RunLevel Highest
Register-ScheduledTask -TaskName $taskName -Action $action -Trigger $trigger `
    -Settings $settings -Principal $principal `
    -Description 'Dispatch due school WhatsApp messages every minute over the authenticated HTTPS endpoint.' | Out-Null
Write-Output "Registered '$taskName'. It runs every minute as SYSTEM."