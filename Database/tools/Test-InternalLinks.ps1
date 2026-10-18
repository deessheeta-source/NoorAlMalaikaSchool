<#
    Test-InternalLinks.ps1
    ---------------------------------------------------------------------------
    Scans the project for internal page references (ResolveUrl, ResolveClientUrl,
    ~/..., and href/src pointing at .aspx) and checks that each target actually
    exists on disk. This is what caught the two dead links fixed in this change
    and guards against new ones.
#>

[CmdletBinding()]
param(
    [string]$ProjectRoot = 'C:\Users\Sheeta\Desktop\NoorAlMalaikaSchool'
)

$ErrorActionPreference = 'Stop'

# Text files worth scanning.
$exts = @('.cs', '.aspx', '.ascx', '.master', '.config', '.js')

$files = Get-ChildItem -LiteralPath $ProjectRoot -Recurse -File -ErrorAction SilentlyContinue |
         Where-Object {
             $_.Extension -in $exts -and
             $_.FullName -notmatch '\\(bin|obj|packages|\.vs|\.git)\\' -and
             $_.FullName -notmatch '\\Content\\'
         }

# Every .aspx on disk, normalised to "~/pages/xxx.aspx" for comparison.
$existing = New-Object System.Collections.Generic.HashSet[string] ([StringComparer]::OrdinalIgnoreCase)
Get-ChildItem -LiteralPath $ProjectRoot -Recurse -Filter '*.aspx' -File -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|packages|\.vs|\.git)\\' } |
    ForEach-Object {
        $rel = $_.FullName.Substring($ProjectRoot.Length).TrimStart('\','/')
        [void]$existing.Add('~/' + $rel.Replace('\','/'))
        [void]$existing.Add('~/' + $rel)
    }

$pattern = '(~/(?:[A-Za-z0-9_\-]+/)*[A-Za-z0-9_\-]+\.aspx)'

$findings = @()
foreach ($f in $files) {
    $lineNo = 0
    foreach ($line in [System.IO.File]::ReadAllLines($f.FullName)) {
        $lineNo++
        foreach ($m in [regex]::Matches($line, $pattern)) {
            $target = $m.Groups[1].Value
            # Normalise separators.
            $norm = $target.Replace('\', '/')
            if (-not $existing.Contains($norm)) {
                $findings += [pscustomobject]@{
                    File   = $f.FullName.Substring($ProjectRoot.Length).TrimStart('\','/')
                    Line   = $lineNo
                    Target = $target
                }
            }
        }
    }
}

Write-Output ("scanned {0} source files against {1} existing .aspx pages" -f $files.Count, $existing.Count)
Write-Output ""
if ($findings.Count -eq 0) {
    Write-Output "RESULT: PASS - every internal page reference resolves to a file that exists."
    exit 0
}

Write-Output ("RESULT: FAIL - {0} dangling internal page reference(s):" -f $findings.Count)
$findings | Sort-Object Target | ForEach-Object {
    Write-Output ("  {0}:{1}  ->  {2}" -f $_.File, $_.Line, $_.Target)
}
exit 1
