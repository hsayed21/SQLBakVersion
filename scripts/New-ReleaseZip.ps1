[CmdletBinding()]
param(
    [ValidatePattern('^\d+\.\d+\.\d+([-.][0-9A-Za-z.-]+)?$')]
    [string]$Version = '1.0.0',

    [ValidateSet('win-x64', 'win-x86')]
    [string]$Runtime = 'win-x64'
)

$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $PSScriptRoot
$projectFile = Join-Path $projectRoot 'SQLBakVersion.csproj'
$artifactsDirectory = Join-Path $projectRoot 'artifacts'
$releaseName = 'SQLBakVersion'
$buildDirectory = Join-Path $artifactsDirectory "$releaseName-$Version-$Runtime"
$zipPath = Join-Path $artifactsDirectory "$releaseName.zip"
$assemblyInfoFile = Join-Path $projectRoot 'Properties\AssemblyInfo.cs'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'

if (-not (Test-Path -LiteralPath $projectFile)) {
    throw "Project file was not found: $projectFile"
}
if (-not (Test-Path -LiteralPath $vswhere)) {
    throw 'Visual Studio Build Tools were not found.'
}

$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\Current\Bin\MSBuild.exe' |
    Select-Object -First 1

if (-not $msbuild -or -not (Test-Path -LiteralPath $msbuild)) {
    throw 'MSBuild.exe was not found.'
}

New-Item -ItemType Directory -Path $artifactsDirectory -Force | Out-Null

if (Test-Path -LiteralPath $buildDirectory) {
    Remove-Item -LiteralPath $buildDirectory -Recurse -Force
}
if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

$platformTarget = if ($Runtime -eq 'win-x86') { 'x86' } else { 'x64' }
$assemblyVersion = "$(($Version -split '[-+]')[0]).0"
$originalAssemblyInfo = [System.IO.File]::ReadAllBytes($assemblyInfoFile)
$assemblyInfo = Get-Content -LiteralPath $assemblyInfoFile -Raw

$assemblyInfo = [regex]::Replace($assemblyInfo, '(?<=AssemblyVersion\(")[^"]+(?="\))', $assemblyVersion)
$assemblyInfo = [regex]::Replace($assemblyInfo, '(?<=AssemblyFileVersion\(")[^"]+(?="\))', $assemblyVersion)

try {
    [System.IO.File]::WriteAllText($assemblyInfoFile, $assemblyInfo, [System.Text.UTF8Encoding]::new($false))

    Write-Host "Building $releaseName for $Runtime..."
    & $msbuild $projectFile `
        /t:Rebuild `
        "/p:Configuration=Release;Platform=AnyCPU;PlatformTarget=$platformTarget;OutputPath=$buildDirectory\" `
        /nologo `
        /verbosity:minimal

    if ($LASTEXITCODE -ne 0) {
        throw "MSBuild failed with exit code $LASTEXITCODE."
    }
}
finally {
    [System.IO.File]::WriteAllBytes($assemblyInfoFile, $originalAssemblyInfo)
}

Copy-Item -LiteralPath (Join-Path $projectRoot 'README.md') -Destination $buildDirectory
Compress-Archive -Path (Join-Path $buildDirectory '*') -DestinationPath $zipPath -CompressionLevel Optimal

Write-Host "Release ZIP created: $zipPath" -ForegroundColor Green
