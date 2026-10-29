<#
.SYNOPSIS
    يفعّل HTTPS للموقع: ربط شهادة، requireSSL، إعادة توجيه، و HSTS.

.DESCRIPTION
    سكربت إداري (يحتاج PowerShell مرفوعاً). يقوم بالخطوات التالية بالترتيب:
      1. التحقق من وجود شهادة مطابقة لاسم النطاق في LocalMachine\My.
      2. إضافة ربط HTTPS للموقع إن لم يكن موجوداً.
      3. أخذ نسخة احتياطية من Web.config.
      4. ضبط requireSSL=true و EnableHttpsRedirect=true و EnableHsts=true.
      5. اختبار الاستجابة على HTTPS، والتراجع عن Web.config عند الفشل.

    لا يُشغَّل تلقائياً من Build-Verify لأنه يغيّر إعدادات IIS على الخادم.

.PARAMETER SiteName
    اسم موقع IIS. الافتراضي: Default Web Site

.PARAMETER HostHeader
    اسم النطاق كما يكتبه المستخدمون (مثال: gecol.ly). يجب أن تطابقه الشهادة.

.PARAMETER Thumbprint
    بصمة الشهادة. إن لم تُحدَّد يبحث السكربت عن شهادة مطابقة لـ HostHeader.

.PARAMETER Port
    منفذ HTTPS. الافتراضي 443.

.PARAMETER WebConfigPath
    مسار Web.config. الافتراضي: Web.config بجوار مجلد Database.

.PARAMETER DryRun
    يعرض ما سيحدث دون تغيير أي شيء.

.EXAMPLE
    .\Enable-Https.ps1 -HostHeader gecol.ly -Thumbprint DA3DC89BC57CC5A440B677005A69610F9EFCC530

.EXAMPLE
    .\Enable-Https.ps1 -HostHeader sheetagecol -DryRun
#>

[CmdletBinding()]
param(
    [string]$SiteName = 'Default Web Site',
    [Parameter(Mandatory = $true)]
    [string]$HostHeader,
    [string]$Thumbprint,
    [int]$Port = 443,
    [string]$WebConfigPath,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

function Assert-Administrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw 'هذا السكربت يحتاج PowerShell مرفوعاً (Run as Administrator).'
    }
}

function Get-MatchingCertificate {
    param([string]$Name)

    $candidates = Get-ChildItem Cert:\LocalMachine\My | Where-Object { $_.HasPrivateKey }
    $matches = foreach ($cert in $candidates) {
        $names = @()
        if ($cert.Subject) {
            $names += ($cert.Subject -split ',') | ForEach-Object { ($_ -replace '^CN=', '').Trim() }
        }
        $san = $cert.Extensions | Where-Object { $_.Oid.Value -eq '2.5.29.17' }
        if ($san) {
            $names += [regex]::Matches($san.Format($false), 'DNS Name=([^,]+)') | ForEach-Object { $_.Groups[1].Value.Trim() }
        }
        if ($names -contains $Name) { $cert }
    }

    return $matches | Sort-Object NotAfter -Descending | Select-Object -First 1
}

Assert-Administrator
Import-Module WebAdministration -ErrorAction Stop

if (-not $WebConfigPath) {
    $WebConfigPath = Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) 'Web.config'
}
if (-not (Test-Path $WebConfigPath)) {
    throw "لم يُعثر على Web.config: $WebConfigPath"
}

Write-Host '=== تفعيل HTTPS ===' -ForegroundColor Cyan
Write-Host "الموقع     : $SiteName"
Write-Host "النطاق     : $HostHeader"
Write-Host "المنفذ     : $Port"
Write-Host "Web.config : $WebConfigPath"

$site = Get-Website -Name $SiteName -ErrorAction SilentlyContinue
if (-not $site) {
    throw "موقع IIS غير موجود: $SiteName  (اعرض القائمة بـ: Get-Website)"
}

if (-not $Thumbprint) {
    $cert = Get-MatchingCertificate -Name $HostHeader
    if (-not $cert) {
        throw "لا توجد شهادة في LocalMachine\My تطابق '$HostHeader'. أصدر شهادة للنطاق ثم مرّر -Thumbprint."
    }
    $Thumbprint = $cert.Thumbprint
    Write-Host "الشهادة    : $($cert.Subject)  ($Thumbprint)" -ForegroundColor Green
}
else {
    $cert = Get-Item "Cert:\LocalMachine\My\$Thumbprint" -ErrorAction SilentlyContinue
    if (-not $cert) {
        throw "الشهادة غير موجودة في LocalMachine\My: $Thumbprint"
    }
    $subjectNames = @(($cert.Subject -split ',') | ForEach-Object { ($_ -replace '^CN=', '').Trim() })
    if ($subjectNames -notcontains $HostHeader) {
        Write-Warning "اسم الشهادة ($($cert.Subject)) لا يطابق النطاق '$HostHeader'. قد يظهر تحذير شهادة للمستخدمين."
    }
}

$binding = Get-WebBinding -Name $SiteName -Protocol https -Port $Port -HostHeader $HostHeader -ErrorAction SilentlyContinue
if ($binding) {
    Write-Host 'ربط HTTPS موجود مسبقاً — سيُحدَّث إلى الشهادة المطلوبة.' -ForegroundColor Yellow
}
else {
    Write-Host "إضافة ربط HTTPS على المنفذ $Port ..." -ForegroundColor Yellow
    if (-not $DryRun) {
        New-WebBinding -Name $SiteName -Protocol https -Port $Port -HostHeader $HostHeader -IPAddress '*' -SslFlags 0 | Out-Null
    }
}

if (-not $DryRun) {
    $binding = Get-WebBinding -Name $SiteName -Protocol https -Port $Port -HostHeader $HostHeader
    $binding.AddSslCertificate($Thumbprint, 'My')
    Write-Host 'تم ربط الشهادة.' -ForegroundColor Green
}

$original = Get-Content -Path $WebConfigPath -Raw -Encoding UTF8
$updated = $original

# requireSSL في httpCookies و forms
$updated = $updated -replace '(<httpCookies[^>]*?)requireSSL="false"', '$1requireSSL="true"'
$updated = $updated -replace '(<forms\b[^>]*?)requireSSL="false"', '$1requireSSL="true"'

# مفاتيح التشغيل
$updated = $updated -replace '(<add key="EnableHttpsRedirect" value=")false(")', '${1}true${2}'
$updated = $updated -replace '(<add key="EnableHsts" value=")false(")', '${1}true${2}'

if ($updated -eq $original) {
    Write-Host 'Web.config لا يحتاج تعديلاً (HTTPS مفعّل مسبقاً).' -ForegroundColor Yellow
}

if ($DryRun) {
    Write-Host ''
    Write-Host 'DRY RUN — لن يُكتب أي تغيير.' -ForegroundColor Cyan
    Write-Host "requireSSL occurrences: $(([regex]::Matches($updated, 'requireSSL="true"')).Count)"
    return
}

$backup = "$WebConfigPath.bak-$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Copy-Item -Path $WebConfigPath -Destination $backup -Force
Write-Host "نسخة احتياطية: $backup" -ForegroundColor Green

Set-Content -Path $WebConfigPath -Value $updated -Encoding UTF8

# اختبار سريع: قد تفشل المصادقة الذاتية للشهادة على localhost
$previousCallback = [System.Net.ServicePointManager]::ServerCertificateValidationCallback
try {
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
    $response = Invoke-WebRequest -Uri "https://$HostHeader/Login.aspx" -UseBasicParsing -TimeoutSec 30 -MaximumRedirection 0
    Write-Host "اختبار HTTPS: HTTP $($response.StatusCode)" -ForegroundColor Green
}
catch {
    $status = $_.Exception.Response.StatusCode.value__
    if ($status -eq 302 -or $status -eq 200) {
        Write-Host "اختبار HTTPS: HTTP $status" -ForegroundColor Green
    }
    else {
        Write-Host "فشل اختبار HTTPS: $($_.Exception.Message)" -ForegroundColor Red
        Copy-Item -Path $backup -Destination $WebConfigPath -Force
        Write-Warning 'تم استرجاع Web.config. راجع الربط/الشهادة ثم أعد المحاولة.'
        throw
    }
}
finally {
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = $previousCallback
}

Write-Host ''
Write-Host 'تم تفعيل HTTPS. أعد تشغيل Application Pool لتطبيق requireSSL.' -ForegroundColor Green
Write-Host 'تحقق يدوياً من تسجيل الدخول عبر الرابط: https://' -NoNewline
Write-Host $HostHeader -ForegroundColor Cyan
