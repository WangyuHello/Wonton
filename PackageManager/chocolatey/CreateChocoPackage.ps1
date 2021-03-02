[CmdletBinding()]
Param(
    [string]$ReleasePath = ""
)

$ProjectRoot = (Get-Item $PSScriptRoot).Parent.Parent.FullName

if ($ReleasePath -eq "") {
    $ReleasePath = Join-Path $ProjectRoot "Build"
}

Import-Module EPS

$pkgJsonFile = Get-Content "$ProjectRoot/Wonton.CrossUI.Web.HostApp/package.json" -Raw
$pkgJson = ConvertFrom-Json $pkgJsonFile
$Version = $pkgJson.version
Write-Host "Wonton Version: $Version"
$InstallerFileName = "Wonton-$Version-win-setup.exe"

if (Test-Path "$PSScriptRoot/wonton") {
    Remove-Item "$PSScriptRoot/wonton" -Recurse -Force
}

New-Item -Path "$PSScriptRoot/wonton" -ItemType Directory -Force | Out-Null
New-Item -Path "$PSScriptRoot/wonton/tools" -ItemType Directory -Force | Out-Null

$release_file = Join-Path $ReleasePath $InstallerFileName # Build/Wonton-1.0.11-win-setup.exe
$pack_install_file = Join-Path "$PSScriptRoot/wonton/tools" $InstallerFileName

# 拷贝安装程序
Copy-Item $release_file $pack_install_file -Force

$hash = Get-FileHash -Path $pack_install_file -Algorithm SHA256
$sha256 =  $hash.Hash

Write-Host "SHA256: $sha256"

Invoke-EpsTemplate -Path "$PSScriptRoot/wonton.t.nuspec" -Safe -Binding @{
    version = $Version
} | Out-File "$PSScriptRoot/wonton/wonton.nuspec"

Invoke-EpsTemplate -Path "$PSScriptRoot/LICENSE.t.txt" -Safe -Binding @{
    
} | Out-File "$PSScriptRoot/wonton/tools/LICENSE.txt"

Invoke-EpsTemplate -Path "$PSScriptRoot/VERIFICATION.t.txt" -Safe -Binding @{
    github_download_url = "https://github.com/WangyuHello/Wonton/releases/download/v$Version/Wonton-$Version-win-setup.exe"
    checksum = $sha256
} | Out-File "$PSScriptRoot/wonton/tools/VERIFICATION.txt"

Invoke-EpsTemplate -Path "$PSScriptRoot/chocolateyinstall.t.ps1" -Safe -Binding @{
    installer_file_name = $InstallerFileName
    checksum = $sha256
} | Out-File "$PSScriptRoot/wonton/tools/chocolateyinstall.ps1"

$cur = Get-Location

Set-Location "$PSScriptRoot/wonton"

Invoke-Expression "choco pack"

Set-Location $cur