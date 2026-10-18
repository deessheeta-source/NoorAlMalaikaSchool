$ErrorActionPreference = 'Stop'

$endpoint = [Environment]::GetEnvironmentVariable('NOOR_WHATSAPP_DISPATCH_URL', 'Machine')
$dispatchKey = [Environment]::GetEnvironmentVariable('NOOR_WHATSAPP_DISPATCH_KEY', 'Machine')

if ([string]::IsNullOrWhiteSpace($endpoint) -or [string]::IsNullOrWhiteSpace($dispatchKey)) {
    throw 'Set machine environment variables NOOR_WHATSAPP_DISPATCH_URL and NOOR_WHATSAPP_DISPATCH_KEY first.'
}
if (-not $endpoint.StartsWith('https://', [StringComparison]::OrdinalIgnoreCase)) {
    throw 'The WhatsApp dispatch endpoint must use HTTPS.'
}

$response = Invoke-WebRequest -Uri $endpoint -Method Post -Headers @{ Authorization = "Bearer $dispatchKey" } -UseBasicParsing -TimeoutSec 90
Write-Output $response.Content
