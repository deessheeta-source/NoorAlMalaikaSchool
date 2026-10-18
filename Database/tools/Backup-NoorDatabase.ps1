<#
.SYNOPSIS
    Backup-NoorDatabase.ps1 - نسخ احتياطي مجدول لقاعدة بيانات مدرسة نور الملائكة.

.DESCRIPTION
    ينفّذ BACKUP DATABASE كامل مضغوطاً إلى App_Data\DatabaseBackups باسم مؤرَّخ،
    يتحقّق من سلامة النسخة (RESTORE VERIFYONLY)، ينظّف النسق الأقدم من مدة
    الاحتفاظ، ويسجّل النتيجة في backup.log بجانب النسخة.

    المهمة المجدولة:
        powershell -NoProfile -ExecutionPolicy Bypass -File Backup-NoorDatabase.ps1 -RegisterTask
    (يجدول تشغيل يومي 02:00 باسم NoorAlMalaikaSchool_DBBackup)

    ملاحظة: لا يوجد SQL Server Agent على هذا الخادم، لذا تُستخدم Windows
    Task Scheduler بدلاً منه. المهمة تعمل بهوية الحساب الحالي (S4U).
    نسخة ثانوية على قرص/مشاركة خارجية: استخدم -SecondaryDir (تنبيه: حساب
    S4U لا يصل إلى مشاركات الشبكة؛ شغّل يدوياً بمعامل آخر أو استخدم مهمة
    منفصلة تعمل بهوية لها صلاحية شبكة).

.EXAMPLE
    .\Backup-NoorDatabase.ps1
    .\Backup-NoorDatabase.ps1 -RetentionDays 30 -SecondaryDir "E:\SchoolBackups"
    .\Backup-NoorDatabase.ps1 -RegisterTask
    .\Backup-NoorDatabase.ps1 -UnregisterTask
#>
[CmdletBinding(DefaultParameterSetName = 'Run')]
param(
    [string]$Server = 'SHEETAGECOL\SQLSERVER2026',
    [string]$Database = 'NoorAlMalaikaSchool',

    [string]$BackupDir = '',

    [ValidateRange(1, 3650)]
    [int]$RetentionDays = 14,

    [string]$SecondaryDir = '',

    [switch]$SkipVerify,

    [Parameter(ParameterSetName = 'Register')]
    [switch]$RegisterTask,

    [Parameter(ParameterSetName = 'Unregister')]
    [switch]$UnregisterTask
)

$ErrorActionPreference = 'Stop'

$ScriptName    = 'Backup-NoorDatabase.ps1'
$TaskName      = 'NoorAlMalaikaSchool_DBBackup'
$ProjectRoot   = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ToolsDir      = $PSScriptRoot

if ([string]::IsNullOrWhiteSpace($BackupDir)) {
    $BackupDir = Join-Path $ProjectRoot 'App_Data\DatabaseBackups'
}

# ---------------------------------------------------------------
# Task registration
# ---------------------------------------------------------------
if ($RegisterTask) {
    $scriptPath = Join-Path $ToolsDir $ScriptName
    $action  = New-ScheduledTaskAction -Execute 'powershell.exe' `
        -Argument "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`""
    $trigger = New-ScheduledTaskTrigger -Daily -At 02:00
    $principal = New-ScheduledTaskPrincipal `
        -UserId "$env:USERDOMAIN\$env:USERNAME" -LogonType S4U -RunLevel Highest

    Register-ScheduledTask -TaskName $TaskName -Action $action -Trigger $trigger `
        -Principal $principal -Force `
        -Description 'Daily full backup of NoorAlMalaikaSchool DB (see Backup-NoorDatabase.ps1)' | Out-Null

    Write-Host "Scheduled task '$TaskName' registered (daily 02:00)." -ForegroundColor Green
    Write-Host "Run it once now to verify: Start-ScheduledTask -TaskName '$TaskName'"
    exit 0
}

if ($UnregisterTask) {
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false -ErrorAction SilentlyContinue
    Write-Host "Scheduled task '$TaskName' removed." -ForegroundColor Yellow
    exit 0
}

# ---------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------
function Write-Log {
    param([string]$Message, [string]$Level = 'INFO')

    $line = '{0} [{1}] {2}' -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'), $Level, $Message
    Add-Content -LiteralPath $LogPath -Value $line -Encoding UTF8
    if ($Level -eq 'FAIL') {
        Write-Host $line -ForegroundColor Red
    }
    elseif ($Level -eq 'WARN') {
        Write-Host $line -ForegroundColor Yellow
    }
    else {
        Write-Host $line
    }
}

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

# ---------------------------------------------------------------
# Run backup
# ---------------------------------------------------------------
$started      = Get-Date
$logFileName  = 'backup.log'
$LogPath      = Join-Path $BackupDir $logFileName

try {
    if (-not (Test-Path -LiteralPath $BackupDir)) {
        New-Item -ItemType Directory -Path $BackupDir -Force | Out-Null
    }

    $sqlcmd = Find-SqlCmd
    if (-not $sqlcmd) {
        Write-Log 'sqlcmd.exe not found (SQL Server command line utilities missing).' 'FAIL'
        exit 2
    }

    $stamp   = Get-Date -Format 'yyyyMMdd_HHmmss'
    $bakFile = Join-Path $BackupDir ("{0}_{1}.bak" -f $Database, $stamp)

    if (Test-Path -LiteralPath $bakFile) {
        $bakFile = Join-Path $BackupDir ("{0}_{1}_{2}.bak" -f $Database, $stamp, (Get-Random -Maximum 9999))
    }

    $bakPathSql = $bakFile.Replace("'", "''")

    Write-Log "Backup start -> $bakFile (server=$Server)"

    $backupSql = "BACKUP DATABASE [$Database] TO DISK='$bakPathSql' WITH INIT, COMPRESSION, STATS=10;"
    $output = & $sqlcmd -S $Server -E -b -Q $backupSql 2>&1
    $sqlExit = $LASTEXITCODE

    if ($sqlExit -ne 0) {
        foreach ($line in $output) { Write-Log ("sqlcmd: " + $line) 'WARN' }
        Write-Log "BACKUP FAILED (sqlcmd exit $sqlExit)." 'FAIL'
        exit $sqlExit
    }

    if (-not (Test-Path -LiteralPath $bakFile)) {
        Write-Log 'Backup file was not created.' 'FAIL'
        exit 3
    }

    $sizeMB = [math]::Round((Get-Item -LiteralPath $bakFile).Length / 1MB, 2)
    Write-Log ("Backup created ({0} MB)." -f $sizeMB)

    if (-not $SkipVerify) {
        $verifySql = "RESTORE VERIFYONLY FROM DISK='$bakPathSql' WITH STATS=10;"
        $vOutput = & $sqlcmd -S $Server -E -b -Q $verifySql 2>&1
        if ($LASTEXITCODE -ne 0) {
            foreach ($line in $vOutput) { Write-Log ("verify: " + $line) 'WARN' }
            Write-Log 'VERIFY FAILED - backup file may be corrupt.' 'FAIL'
            exit 4
        }
        Write-Log 'Verify OK (RESTORE VERIFYONLY).'
    }

    if (-not [string]::IsNullOrWhiteSpace($SecondaryDir)) {
        try {
            if (-not (Test-Path -LiteralPath $SecondaryDir)) {
                New-Item -ItemType Directory -Path $SecondaryDir -Force | Out-Null
            }
            Copy-Item -LiteralPath $bakFile -Destination $SecondaryDir -Force
            Write-Log ("Secondary copy -> {0}" -f $SecondaryDir)
        }
        catch {
            Write-Log ("Secondary copy FAILED: " + $_.Exception.Message) 'WARN'
        }
    }

    $cutoff = (Get-Date).AddDays(-$RetentionDays)
    $old = Get-ChildItem -LiteralPath $BackupDir -Filter "$Database*.bak" -ErrorAction SilentlyContinue |
        Where-Object { $_.LastWriteTime -lt $cutoff }
    foreach ($f in $old) {
        try {
            Remove-Item -LiteralPath $f.FullName -Force
            Write-Log ("Retention removed: {0}" -f $f.Name)
        }
        catch {
            Write-Log ("Retention remove failed for {0}: {1}" -f $f.Name, $_.Exception.Message) 'WARN'
        }
    }

    $elapsed = [math]::Round(((Get-Date) - $started).TotalSeconds, 1)
    Write-Log ("Backup completed in {0}s." -f $elapsed)
    exit 0
}
catch {
    Write-Log ("EXCEPTION: " + $_.Exception.Message) 'FAIL'
    exit 1
}
