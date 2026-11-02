param(
    [string]$Configuration = "Release",
    [bool]$Build = $true,
    [bool]$Pack = $true
)

$projectPath = "DeploymentRings\DeploymentRings.csproj"
[xml]$projectFile = Get-Content $projectPath
$version = $projectFile.Project.PropertyGroup.Version |
    Where-Object { $_ -ne $null } |
    Select-Object -First 1

if ([string]::IsNullOrWhiteSpace($version)) {
    Write-Error "Could not read the version from $projectPath."
    exit 1
}

if ($Build) {
    Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
    if (Test-Path ".\publish") {
        Remove-Item ".\publish" -Recurse -Force
    }
    New-Item -ItemType Directory -Path ".\publish" | Out-Null

    Write-Host "Building version: $version" -ForegroundColor Green
    dotnet publish $projectPath `
        -c $Configuration `
        -o publish `
        -r win-x64 `
        --self-contained false

    if ($LASTEXITCODE -ne 0) {
        Write-Error "DeploymentRings build failed with exit code $LASTEXITCODE."
        exit 1
    }

    Get-ChildItem ".\publish\*.exe" | ForEach-Object {
        $size = [math]::Round($_.Length / 1MB, 2)
        Write-Host "$($_.Name) - $size MB" -ForegroundColor White
    }
    Write-Host "Output directory: .\publish\" -ForegroundColor Yellow
}

if ($Pack) {
    $signToolPath = "C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool"
    $signTool = Join-Path $signToolPath "signtool.exe"
    if (-not (Test-Path $signTool)) {
        Write-Error "SignTool.exe not found at $signToolPath. Please verify the path."
        exit 1
    }
    $env:Path += ";$signToolPath"

    $certificatePath = "C:\dev\EUCCodeSign.pfx"
    $password = Read-Host "Enter password for code signing certificate ($certificatePath)"
    if ([string]::IsNullOrWhiteSpace($password)) {
        Write-Error "No password entered; packaging was cancelled."
        exit 1
    }
    if (-not (Test-Path $certificatePath)) {
        Write-Error "Code-signing certificate not found at $certificatePath."
        exit 1
    }

    $releasePath = "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\DeploymentRings\Application"
    Write-Host "Downloading previous release..." -ForegroundColor Yellow
    vpk download local --path $releasePath
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Velopack download failed with exit code $LASTEXITCODE."
        exit 1
    }

    Write-Host "Packing version: $version" -ForegroundColor Green
    vpk pack -u DeploymentRings `
        -v $version `
        -p .\publish `
        -e DeploymentRings.exe `
        --noPortable `
        --signParams "/fd sha256 /f $certificatePath /p $password /t http://time.certum.pl/"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Velopack packaging failed with exit code $LASTEXITCODE."
        exit 1
    }

    Write-Host "Uploading release..." -ForegroundColor Yellow
    vpk upload local --keepMaxReleases 6 --path $releasePath
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Velopack upload failed with exit code $LASTEXITCODE."
        exit 1
    }
}
