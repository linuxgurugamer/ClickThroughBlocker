#Requires -Version 5.0
<#
.SYNOPSIS
    Builds the mod in Release and packages GameData into a versioned zip.

.DESCRIPTION
    Builds the project, during which KSPBT copies the plugin and the .version
    file into GameData. Then read the version back out, clear out any
    leftover .pdb files and zip GameData as Builds\<modName>-<version>.zip.

    No external requirements besides .NET.
#>

$ErrorActionPreference = "Stop"

$root = $PSScriptRoot

# Read mod name from the .version file in the repo root.
$versionFiles = @(Get-ChildItem -Path $root -Filter *.version -File)
if ($versionFiles.Count -ne 1) {
    throw "Expected exactly one .version file in $root, found $($versionFiles.Count)."
}
$modName = $versionFiles[0].BaseName

# Build in Release.
Write-Host "Building $modName (Release)..." -ForegroundColor Cyan
dotnet build "$root\$modName\$modName.csproj" -c Release -p:SolutionDir="$root\"
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

# Read the version out of the .version file that KSPBT just wrote.
$gameDataDirs = @(Get-ChildItem -Path (Join-Path $root "GameData") -Directory)
if ($gameDataDirs.Count -ne 1) {
    throw "Expected exactly one mod folder in GameData, found $($gameDataDirs.Count)."
}
$versionFile = Join-Path $gameDataDirs[0].FullName "$modName.version"
$version = (Get-Content $versionFile -Raw | ConvertFrom-Json).VERSION
Write-Host "Version: $version" -ForegroundColor Cyan

# Remove any stray .pdb files.
Get-ChildItem (Join-Path $root "GameData") -Recurse -File -Filter *.pdb | Remove-Item -Force

# Zip GameData into Builds\<mod>-<version>.zip.
$buildsDir = Join-Path $root "Builds"
New-Item -ItemType Directory -Force -Path $buildsDir | Out-Null

$zip = Join-Path $buildsDir "$modName-$version.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }

Add-Type -AssemblyName System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory((Join-Path $root "GameData"), $zip, 'Optimal', $true)
Write-Host "Created $zip" -ForegroundColor Green
