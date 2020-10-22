[CmdletBinding()]
Param(
    [string]$From = "Build",
    [string]$Release = "Build"
)

Write-Host "从 $From 拷贝到 $Release"

Copy-Item -Path "$From/**/*.7z" -Destination $Release -Force
Copy-Item -Path "$From/**/*.deb" -Destination $Release -Force
Copy-Item -Path "$From/**/*.dmg" -Destination $Release -Force
Copy-Item -Path "$From/**/*.exe" -Destination $Release -Force

$pkgs = Get-ChildItem -Path $Release
$count = $pkgs.Count

Write-Host "总包数量: $count"
foreach ($item in $pkgs) {
    $name = $item.Name
    Write-Host "$name"
}