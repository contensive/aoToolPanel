<#
.SYNOPSIS
    Builds the aoToolPanel addon collection and deploys a versioned zip.

.DESCRIPTION
    Uses the shared Contensive5 build module
    (C:\Git\Contensive5\scripts\contensive-build.psm1) for version/folder/zip
    helpers. The project is an SDK-style csproj so dotnet build is used
    directly in place of the legacy MSBuild solution helper.

.EXAMPLE
    pwsh -File scripts\build.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
$collectionName = 'aoToolPanel'
$projectRoot    = (Resolve-Path "$PSScriptRoot\..").Path
$sourceProject  = Join-Path $projectRoot 'source\aoToolPanel\aoToolPanel.csproj'
$binPath        = Join-Path $projectRoot 'source\aoToolPanel\bin\Debug'
$collectionPath = Join-Path $projectRoot 'collections\aoToolPanel'
$uiPath         = Join-Path $projectRoot 'ui'
$helpPath       = Join-Path $projectRoot 'help'
$deployRoot     = 'C:\Deployments\aoToolPanel\Dev'
$configuration  = 'Debug'

# ---------------------------------------------------------------------------
# Import shared Contensive build module
# ---------------------------------------------------------------------------
$sharedModule = 'C:\Git\Contensive5\scripts\contensive-build.psm1'
if (-not (Test-Path $sharedModule)) {
    throw "Shared Contensive build module not found at: $sharedModule - clone the Contensive5 repo to C:\Git\Contensive5."
}
Import-Module $sharedModule -Force

# ---------------------------------------------------------------------------
# Step 1 - Resolve version and deployment folder
# ---------------------------------------------------------------------------
if (-not (Test-Path $deployRoot)) {
    New-Item -ItemType Directory -Path $deployRoot | Out-Null
}
$version          = Get-ContensiveVersion -DeploymentRoot $deployRoot
$deploymentFolder = New-DeploymentFolder  -DeploymentRoot $deployRoot -Version $version

Write-Host ""
Write-Host "========================================"
Write-Host "Building $collectionName v$version"
Write-Host "========================================"
Write-Host ""

# ---------------------------------------------------------------------------
# Step 2 - Clean build output
# ---------------------------------------------------------------------------
Write-Host "Cleaning build folders..."
$cleanFolders = @(
    (Join-Path $projectRoot 'source\aoToolPanel\bin'),
    (Join-Path $projectRoot 'source\aoToolPanel\obj')
)
foreach ($folder in $cleanFolders) {
    if (Test-Path $folder) { Remove-Item $folder -Recurse -Force }
}

# Remove everything from the collection folder except .xml files
Get-ChildItem -Path $collectionPath -File -ErrorAction SilentlyContinue |
    Where-Object { $_.Extension -ne '.xml' } |
    Remove-Item -Force

# ---------------------------------------------------------------------------
# Step 3 - Package help files
# ---------------------------------------------------------------------------
Write-Host "Packaging help files..."
Invoke-ZipFolder -SourceFolder $helpPath `
                 -DestZip      (Join-Path $collectionPath 'HelpFiles.zip')

# ---------------------------------------------------------------------------
# Step 4 - Build the SDK-style project with dotnet
# ---------------------------------------------------------------------------
Write-Host "Building $sourceProject..."
& dotnet build $sourceProject --configuration $configuration --no-incremental /property:FileVersion=$version
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed for: $sourceProject" }

# ---------------------------------------------------------------------------
# Step 5 - Stage UI files and assemblies, then zip the collection
# ---------------------------------------------------------------------------
Write-Host "Staging UI files..."
foreach ($pattern in '*.html','*.jpg','*.png','*.css','*.js','*.gif') {
    Copy-Item (Join-Path $uiPath $pattern) -Destination $collectionPath -Force -ErrorAction SilentlyContinue
}

Write-Host "Staging assemblies..."
Copy-Item (Join-Path $binPath '*.dll')        -Destination $collectionPath -Force
Copy-Item (Join-Path $binPath '*.dll.config') -Destination $collectionPath -Force -ErrorAction SilentlyContinue

Write-Host "Building collection zip..."
$collectionZip = Join-Path $collectionPath "$collectionName.zip"
if (Test-Path $collectionZip) { Remove-Item $collectionZip -Force }
Invoke-ZipFolder -SourceFolder $collectionPath -DestZip $collectionZip

Copy-Item $collectionZip -Destination $deploymentFolder -Force

# ---------------------------------------------------------------------------
# Step 6 - Clean staging files from collection folder (keep xml + zip)
# ---------------------------------------------------------------------------
Write-Host "Cleaning collection staging folder..."
$collectionZipName = "$collectionName.zip"
Get-ChildItem -Path $collectionPath -File -ErrorAction SilentlyContinue |
    Where-Object { $_.Extension -ne '.xml' -and $_.Name -ne $collectionZipName } |
    Remove-Item -Force -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "========================================"
Write-Host "Build complete: $collectionName v$version"
Write-Host "Deployed to: $deploymentFolder"
Write-Host "========================================"
