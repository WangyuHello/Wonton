[CmdletBinding()]
Param(
    [string]$API_KEY = ""
)

if ($API_KEY -eq "") {
    $API_KEY = $env:CHOCOLATEY_API_KEY
}

$ProjectRoot = (Get-Item $PSScriptRoot).Parent.Parent.FullName
$pkgJsonFile = Get-Content "$ProjectRoot/Wonton.CrossUI.Web.HostApp/package.json" -Raw
$pkgJson = ConvertFrom-Json $pkgJsonFile
$Version = $pkgJson.version
Write-Host "Wonton Version: $Version"

$cur = Get-Location
Set-Location "$PSScriptRoot/wonton"

Invoke-Expression "choco apikey -k $API_KEY -source https://push.chocolatey.org/"
Invoke-Expression "choco push wonton.$Version.nupkg -s https://push.chocolatey.org/"

Set-Location $cur