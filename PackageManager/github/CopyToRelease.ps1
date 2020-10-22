[CmdletBinding()]
Param(
    [string]$From = "Build",
    [string]$Release = "Build"
)

Write-Host "Copy packages from $From to $Release"

Copy-Item -Path "$From/**/*.7z" -Destination $Release -Force
Copy-Item -Path "$From/**/*.deb" -Destination $Release -Force
Copy-Item -Path "$From/**/*.dmg" -Destination $Release -Force
Copy-Item -Path "$From/**/*.exe" -Destination $Release -Force

Get-ChildItem -Path $Release