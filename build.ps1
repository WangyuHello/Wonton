[CmdletBinding()]
Param(
    [string]$Script = "build.cake",
    [switch]$useMagic,
    [string]$node_version = "12.19.0",
    [string]$dotnet_version = "3.1.403",
    [string]$Target,
    [string]$Configuration,
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity,
    [switch]$ShowDescription,
    [Alias("WhatIf", "Noop")]
    [switch]$DryRun,
    [switch]$SkipToolPackageRestore,
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$ScriptArgs
)

# Attempt to set highest encryption available for SecurityProtocol.
# PowerShell will not set this by default (until maybe .NET 4.6.x). This
# will typically produce a message for PowerShell v2 (just an info
# message though)
try {
    # Set TLS 1.2 (3072), then TLS 1.1 (768), then TLS 1.0 (192), finally SSL 3.0 (48)
    # Use integers because the enumeration values for TLS 1.2 and TLS 1.1 won't
    # exist in .NET 4.0, even though they are addressable if .NET 4.5+ is
    # installed (.NET 4.5 is an in-place upgrade).
    # PowerShell Core already has support for TLS 1.2 so we can skip this if running in that.
    if (-not $IsCoreCLR) {
        [System.Net.ServicePointManager]::SecurityProtocol = 3072 -bor 768 -bor 192 -bor 48
    }
} catch {
    Write-Output 'Unable to set PowerShell to use TLS 1.2 and TLS 1.1 due to old .NET Framework installed. If you see underlying connection closed or trust errors, you may need to upgrade to .NET Framework 4.5+ and PowerShell v3'
}

[Reflection.Assembly]::LoadWithPartialName("System.Security") | Out-Null

function MD5HashFile([string] $filePath)
{
    if ([string]::IsNullOrEmpty($filePath) -or !(Test-Path $filePath -PathType Leaf))
    {
        return $null
    }

    [System.IO.Stream] $file = $null;
    [System.Security.Cryptography.MD5] $md5 = $null;
    try
    {
        $md5 = [System.Security.Cryptography.MD5]::Create()
        $file = [System.IO.File]::OpenRead($filePath)
        return [System.BitConverter]::ToString($md5.ComputeHash($file))
    }
    finally
    {
        if ($null -ne $file)
        {
            $file.Dispose()
        }
    }
}

function GetProxyEnabledWebClient
{
    $wc = New-Object System.Net.WebClient
    $proxy = [System.Net.WebRequest]::GetSystemWebProxy()
    $proxy.Credentials = [System.Net.CredentialCache]::DefaultCredentials
    $wc.Proxy = $proxy
    return $wc
}

function IsOnWindows {
    if (-not $IsCoreCLR) { # powershell 5
        return $true
    } else {               # powershell core
        if ($IsWindows) {
            return $true
        }
        return $false
    }
}

if(!$PSScriptRoot) { # powershell 5
    $PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
}

$dotnet_exe = "dotnet"
$npm_exe = "npm"
$dotnet_install_script = "dotnet-install"

if (IsOnWindows) {
    $dotnet_exe = "$dotnet_exe.exe"
    $npm_exe = "$npm_exe.cmd"
    $dotnet_install_script = "$dotnet_install_script.ps1"
} else {
    $dotnet_install_script = "$dotnet_install_script.sh"
}

$node_version = "v$node_version"

$tool_path = Join-Path $PSScriptRoot "tools"
$dotnet_install_path = Join-Path (Join-Path $tool_path "dotnet") "dotnet-sdk-$dotnet_version"
$node_install_path = Join-Path $tool_path "node"
$node_distro = "win-x64"
if ($IsMacOS) {
    $node_distro = "darwin-x64"
}
elseif ($IsLinux) {
    $node_distro = "linux-x64"
}
$node_dist = "node-$node_version-$node_distro"
$node_dist_path = if(IsOnWindows) { Join-Path $node_install_path $node_dist } else { Join-Path (Join-Path $node_install_path $node_dist) "bin" }

# $dotnet_exist = $false
$local_dotnet_exist = $false
# $npm_exist = $false
$local_npm_exist = $false

# 如果本地安装则使用本地
if ((Test-Path $dotnet_install_path) -and (Test-Path (Join-Path $dotnet_install_path $dotnet_exe))) {
    Write-Host "发现本地安装的 .NET Core: $dotnet_install_path" -ForegroundColor "Green"
    $local_dotnet_exist = $true
    $env:Path="$dotnet_install_path;"+$env:Path
    $env:DOTNET_ROOT=$dotnet_install_path
}

if((Test-Path $node_dist_path) -and (Test-Path (Join-Path $node_dist_path $npm_exe))) 
{
    Write-Host "发现本地安装的 Nodejs: $node_dist_path" -ForegroundColor "Green"
    $local_npm_exist = $true
    $env:Path="$node_dist_path;"+$env:Path
}


# try {
#     Invoke-Expression "$dotnet_exe --version" | Out-Null
#     $dotnet_exist = $true
#     if (-not $local_dotnet_exist) {
#         Write-Host "使用 Path 的 .NET Core" -ForegroundColor "Green"
#     }
# }
# catch {
    
# }

# try {
#     Invoke-Expression "$npm_exe -v" | Out-Null
#     $npm_exist = $true
#     if (-not $local_npm_exist) {
#         Write-Host "使用 Path 的 NodeJs" -ForegroundColor "Green"
#     }
# }
# catch {
    
# }

if (-not $local_dotnet_exist) {
    Write-Host "未发现本地 .NET Core, 将进行安装" -ForegroundColor "Yellow"
    # 安装 dotnet
    $dotnet_install_url = "https://dot.net/v1/$dotnet_install_script" # https://dot.net/v1/dotnet-install.sh
    $dotnet_install_file = Join-Path $tool_path $dotnet_install_script
    if (-not (Test-Path -Path $tool_path)) {
        New-Item -Path $tool_path -ItemType Directory | Out-Null
    }
    if (-not (Test-Path -Path $dotnet_install_path)) {
        New-Item -Path $dotnet_install_path -ItemType Directory | Out-Null
    }
    Write-Host "正在下载 .NET Core 安装脚本"
    $wc = GetProxyEnabledWebClient
    $wc.DownloadFile($dotnet_install_url, $dotnet_install_file)
    # Invoke-WebRequest -Uri $dotnet_install_url -OutFile $dotnet_install_file

    Write-Host "正在安装 .NET Core $dotnet_version"
    if (IsOnWindows) {
        Invoke-Expression "$dotnet_install_file -Channel Current -Version $dotnet_version -InstallDir $dotnet_install_path -NoPath"
    } else {
        Invoke-Expression "bash $dotnet_install_file --channel Current --version $dotnet_version --install-dir $dotnet_install_path --no-path"
    }

    if ($LASTEXITCODE -ne 0) {
        Throw "安装 .NET Core 时发生错误"
    }

    $env:Path="$dotnet_install_path;"+$env:Path
    $env:DOTNET_ROOT=$dotnet_install_path
}

if (-not $local_npm_exist) {
    Write-Host "未发现本地 NodeJs, 将进行安装" -ForegroundColor "Yellow"
    # 安装 nodejs
    $node_ext = if(IsOnWindows) { "zip" } else { "tar.xz" }

    $node_arc = "$node_dist.$node_ext"
    $node_downloaded_file = Join-Path $tool_path $node_arc

    $official_node_dist = "https://nodejs.org/dist/"
    $taobao_node_dist = "https://npm.taobao.org/mirrors/node/"
    $node_url = if ($useMagic) {
        "$taobao_node_dist$node_version/$node_arc";  
    } 
    else {
        "$official_node_dist$node_version/$node_arc";  
    }
    
    Write-Host "正在下载 $node_url"
    $wc = GetProxyEnabledWebClient
    $wc.DownloadFile($node_url, $node_downloaded_file)
    # Invoke-WebRequest -Uri $node_url -OutFile $node_downloaded_file

    if (-not (Test-Path -Path $node_install_path)) {
        New-Item -Path $node_install_path -ItemType Directory | Out-Null
    }

    Write-Host "正在解压 $node_arc"
    if (IsOnWindows) {
        Expand-Archive -LiteralPath $node_downloaded_file -DestinationPath $node_install_path
    }
    else {
        Invoke-Expression "tar -xJf $node_downloaded_file -C $node_install_path"
        if ($LASTEXITCODE -ne 0) {
            Throw "解压 NodeJs 时发生错误"
        }
    }

    $env:Path="$node_dist_path;"+$env:Path
    Remove-Item $node_downloaded_file -Force
}

$cake_bin = "dotnet-cake"
if (IsOnWindows) {
    $cake_bin = "$cake_bin.exe"
}
$cake_exe = Join-Path $tool_path $cake_bin

Write-Host "正在安装 Cake" -ForegroundColor "Green"
Invoke-Expression "$dotnet_exe tool install --tool-path $tool_path Cake.Tool" | Out-Null

$cakeArguments = @()
if ($Script) { $cakeArguments += "`"$Script`"" }
if ($Target) { $cakeArguments += "-target=`"$Target`"" }
if ($Configuration) { $cakeArguments += "-configuration=$Configuration" }
if ($Verbosity) { $cakeArguments += "-verbosity=$Verbosity" }
if ($ShowDescription) { $cakeArguments += "-showdescription" }
if ($DryRun) { $cakeArguments += "-dryrun" }
$cakeArguments += "-useMagic=$useMagic"
$cakeArguments += $ScriptArgs

# 运行Build
Write-Host "开始构建"
Invoke-Expression "$cake_exe $($cakeArguments -join " ")"
exit $LASTEXITCODE