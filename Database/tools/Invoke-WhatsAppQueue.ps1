$ErrorActionPreference = 'Stop'

$endpoint = [Environment]::GetEnvironmentVariable('NOOR_WHATSAPP_DISPATCH_URL', 'Machine')
$dispatchKey = [Environment]::GetEnvironmentVariable('NOOR_WHATSAPP_DISPATCH_KEY', 'Machine')

if ([string]::IsNullOrWhiteSpace($endpoint) -or [string]::IsNullOrWhiteSpace($dispatchKey)) {
    throw 'Set machine environment variables NOOR_WHATSAPP_DISPATCH_URL and NOOR_WHATSAPP_DISPATCH_KEY first.'
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
if ($dispatchKey.Length -lt 32) { throw 'NOOR_WHATSAPP_DISPATCH_KEY must contain at least 32 characters.' }

$response = Invoke-WebRequest -Uri $endpoint -Method Post -Headers @{ Authorization = "Bearer $dispatchKey" } -UseBasicParsing -TimeoutSec 210 -MaximumRedirection 0
Write-Output $response.Content
