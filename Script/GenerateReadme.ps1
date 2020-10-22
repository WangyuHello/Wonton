Import-Module EPS

$ProjectRoot = (Get-Item $PSScriptRoot).Parent.FullName
$pkgJsonFile = Get-Content "$ProjectRoot/Wonton.CrossUI.Web.HostApp/package.json" -Raw
$pkgJson = ConvertFrom-Json $pkgJsonFile
$Version = $pkgJson.version
$ElctronVersion = $pkgJson.devDependencies.electron
Write-Host "Wonton Version  : $Version"
Write-Host "Electron Version: $ElctronVersion"

# 提取Contributor信息

Write-Host "获取贡献者信息"
$r = Invoke-WebRequest -Uri "https://api.github.com/repos/WangyuHello/Wonton/contributors?page=1&per_page=100"
$content = $r.Content
$contriJson = ConvertFrom-Json $content

foreach ($item in $contriJson) {
    $cur_html_url = $item.html_url
    $cur_login = $item.login
    Write-Host "${cur_login}: $cur_html_url"
}

Invoke-EpsTemplate -Path "$PSScriptRoot/README.t.md" -Safe -Binding @{
    version = $Version
    elec_version = $ElctronVersion
    contributors = $contriJson
} | Out-File "$ProjectRoot/README.md" -Force

Invoke-EpsTemplate -Path "$PSScriptRoot/README.en-US.t.md" -Safe -Binding @{
    version = $Version
    elec_version = $ElctronVersion
    contributors = $contriJson
} | Out-File "$ProjectRoot/README.en-US.md" -Force
