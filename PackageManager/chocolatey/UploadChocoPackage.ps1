[CmdletBinding()]
Param(
    [string]$API_KEY = "",
    [string]$CommitSHA = ""
)

if ($API_KEY -eq "") {
    $API_KEY = $env:CHOCOLATEY_API_KEY
}

$ProjectRoot = (Get-Item $PSScriptRoot).Parent.Parent.FullName
$pkgJsonFile = Get-Content "$ProjectRoot/Wonton.CrossUI.Web.HostApp/package.json" -Raw
$pkgJson = ConvertFrom-Json $pkgJsonFile
$Version = $pkgJson.version
Write-Host "Wonton Version: $Version"

$HEAD = "HEAD"

if ($CommitSHA -ne "") {
    $HEAD = $CommitSHA
}

$cur_tag = Invoke-Expression "git tag --points-at $HEAD"

if ($cur_tag -and $cur_tag.StartsWith("v")) {
    Write-Host "Tag found: $cur_tag, Uploading"
    $cur = Get-Location
    Set-Location "$PSScriptRoot/wonton"
    Invoke-Expression "choco push wonton.$Version.nupkg -k $API_KEY -s https://push.chocolatey.org/"
    Set-Location $cur
} else {
    Write-Host "No tag matched. Skip uploading"
}

