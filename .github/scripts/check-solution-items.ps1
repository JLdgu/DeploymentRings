$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$solutionPath = Join-Path $repoRoot "DeploymentRings.slnx"
[xml]$solution = Get-Content -Raw $solutionPath

$explicitItems = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($item in $solution.SelectNodes("//File[@Path]")) {
    [void]$explicitItems.Add($item.Path.Replace("\", "/"))
}
foreach ($project in $solution.SelectNodes("//Project[@Path]")) {
    [void]$explicitItems.Add($project.Path.Replace("\", "/"))
}

$implicitProjectRoots = @(
    foreach ($project in $solution.SelectNodes("//Project[@Path]")) {
        $projectDirectory = Split-Path -Parent $project.Path
        if ($projectDirectory) {
            "$($projectDirectory.Replace("\", "/"))/"
        }
    }
)

$repositoryFiles = @(& git -C $repoRoot ls-files --cached --others --exclude-standard)
if ($LASTEXITCODE -ne 0) {
    throw "Unable to enumerate non-ignored repository files with git."
}

$missingItems = @(
    foreach ($file in $repositoryFiles) {
        $normalizedPath = $file.Replace("\", "/")
        if ($normalizedPath -eq "DeploymentRings.slnx" -or $explicitItems.Contains($normalizedPath)) {
            continue
        }

        $isImplicitProjectItem = $false
        foreach ($projectRoot in $implicitProjectRoots) {
            if ($normalizedPath.StartsWith($projectRoot, [StringComparison]::Ordinal)) {
                $isImplicitProjectItem = $true
                break
            }
        }

        if (-not $isImplicitProjectItem) {
            $normalizedPath
        }
    }
)

if ($missingItems.Count -gt 0) {
    Write-Error "These non-ignored files are not included in DeploymentRings.slnx:`n  $($missingItems -join "`n  ")"
    exit 1
}

Write-Output "All non-ignored files are included in DeploymentRings.slnx."
