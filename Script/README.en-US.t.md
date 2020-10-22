<h1 align="center">
    <img src="./Imgs/chip.svg" width="128"/>
</h1>
<h2 align="center">
    <img src="./Imgs/title.svg" />
</h2>

<br>

<div align="center">

Multilingual README：[English](./README.en-US.md), [简体中文](./README.md)

![GitHub](https://img.shields.io/github/license/WangyuHello/Wonton?style=flat-square)
![csharp](https://img.shields.io/badge/language-C%23-orange?style=flat-square)
![js](https://img.shields.io/badge/language-JavaScript-yellow?style=flat-square)
![platform](https://img.shields.io/badge/platform-Windows%20|%20macOS%20|%20Linux-blue?style=flat-square)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/WangyuHello/Wonton?style=flat-square)
![GitHub All Releases](https://img.shields.io/github/downloads/WangyuHello/Wonton/total?style=flat-square)

</div>

| Windows | macOS | Linux |
|:------------------------:|:------------------------:|:---------------------------:|
| ![win ](./Imgs/win.png) | ![mac ](./Imgs/mac.png) | ![lnx ](./Imgs/ubuntu.png) |

## Build Status

| Build Platform        | Windows                                                                                                                                                                                                                              | macOS                                                                                                                                                                                                                              | Linux                                                                                                                                                                                                                              |
|-----------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Travis CI       | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                      | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                    | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                    |
| Azure Pipelines | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=Windows)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=macOS)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=Linux20)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) |

# Installation

## Download from Github Release

You can download Wonton installation package for any platform below

| Platform                        | Architecture | Installation Package      |  Package                      |
| --------------------------------|--------------| --------------------------|-------------------------------|
| Windows                         |  x64         | [.exe][rl-windows]        |  [.7z][rl-pkg-windows]        |
| Windows (Framework Dependent)   |  x64         | [.exe][rl-windows-fxdep]  |  [.7z][rl-pkg-windows-fxdep]  |
| macOS 10.15                     |  x64         | [.dmg][rl-macos15]        |  [.7z][rl-pkg-macos15]        |
| macOS 10.14                     |  x64         | [.dmg][rl-macos14]        |  [.7z][rl-pkg-macos14]        |
| Ubuntu 20.04                    |  x64         | [.deb][rl-ubuntu20]       |  [.7z][rl-pkg-ubuntu20]       |
| Ubuntu 18.04                    |  x64         | [.deb][rl-ubuntu18]       |  [.7z][rl-pkg-ubuntu18]       |
| Ubuntu 16.04                    |  x64         | [.deb][rl-ubuntu16]       |  [.7z][rl-pkg-ubuntu16]       |
| Ubuntu 18.04                    |  armv7       | [.deb][rl-ubuntu18-arm]   |  [.7z][rl-pkg-ubuntu18-arm]   |
| Ubuntu 18.04                    |  arm64       | [.deb][rl-ubuntu18-arm64] |  [.7z][rl-pkg-ubuntu18-arm64] |

[rl-windows]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-win-setup.exe
[rl-windows-fxdep]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-win-fxdependent-setup.exe
[rl-ubuntu20]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-amd64-ubuntu-20.04.deb
[rl-ubuntu18]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-amd64-ubuntu-18.04.deb
[rl-ubuntu16]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-amd64-ubuntu-16.04.deb
[rl-macos14]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-mac-10.14.dmg
[rl-macos15]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-mac-10.15.dmg
[rl-ubuntu18-arm]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-armv7l.deb
[rl-ubuntu18-arm64]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-arm64.deb

[rl-pkg-windows]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-win-x64.7z
[rl-pkg-windows-fxdep]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-win-x64-fxdependent.7z
[rl-pkg-ubuntu20]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-amd64-ubuntu-20.04.7z
[rl-pkg-ubuntu18]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-amd64-ubuntu-18.04.7z
[rl-pkg-ubuntu16]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-amd64-ubuntu-16.04.7z
[rl-pkg-macos14]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-mac-10.14.7z
[rl-pkg-macos15]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-mac-10.15.7z
[rl-pkg-ubuntu18-arm]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-armv7l.7z
[rl-pkg-ubuntu18-arm64]: https://github.com/WangyuHello/Wonton/releases/download/v<%= $version %>/Wonton-<%= $version %>-linux-arm64.7z

## Install from Package Manager

- Windows

    If you have Chocolatey installed

    ```bash
    choco install wonton -y --version=<%= $version %>
    ```
- macOS / Linux

    Not supported yet

# Architecture

<div align="center">
    <img src="./Imgs/Wonton.svg" />
</div>

# Usage

## [Please submit an Issue upon finding a bug](https://github.com/WangyuHello/Wonton/issues)
## [Documents](https://github.com/WangyuHello/Wonton/wiki/%E8%BD%AF%E4%BB%B6%E4%BD%BF%E7%94%A8%E7%AE%80%E4%BB%8B)
## [Adding new devices](https://github.com/WangyuHello/Wonton/wiki/%E5%A6%82%E4%BD%95%E6%B7%BB%E5%8A%A0%E6%96%B0%E7%9A%84%E5%99%A8%E4%BB%B6)

# Compilation Guide

## Dependencies

- NodeJS : https://nodejs.org/en/

    LTS and Current versions are both supported

- .NET Core SDK: https://dotnet.microsoft.com/download

    Requires .NET Core SDK version >= 3.1, please do not install Runtime versions

## Compilation Steps

1. Clone the repository

    ```bash
    git clone https://github.com/WangyuHello/Wonton.git
    ``` 

2. To start compiling, run the scripts under **Wonton** directory

    > If you did not install NodeJS or .NET Core SDK, the following script will install them automatically.

    - Windows
        - Use PowerShell
        ```powershell
        .\build.ps1 -useMagic
        ```
        - Or use cmd
        ```cmd
        .\build.cmd -useMagic
        ```
        - Or you can double click **build.cmd** file.

    - macOS、Linux
        ```bash
        ./build.bash -useMagic
        ```

    > If you are not in mainland China, please remove the -useMagic option

3. Compiled binaries are located under **Build**

# Driver Compilation Guide

## Dependencies

### Windows

- Visual Studio 2019 (MSVC 142)

### macOS

- Xcode
- cmake
- autoconf
- automake
- libtool
- m4

### Linux

- gcc
- cmake
- autoconf
- automake
- libtool
- m4

## Compilation Steps

1. Put the **NativeDeps.zip** archive under the **Wonton** directory, and follow the steps in [Compilation Steps](##Compilation Steps)

    *The source code of driver is not opensourced*

2. If you want revoke driver compilation, please delete **NativeDeps.zip** , VLFDDriver, and SharpVLFD directories。

# Troubleshoot

#### Can not download Electron

1. Delete Wonton.CrossUI.Web\ClientApp\node_modules\electron

2. Find**Electron Cache Directory**. You can create it if not exists.

    Windows： ```%LOCALAPPDATA%\electron\Cache``` <br>
    macOS:  ```~/Library/Caches/electron/``` <br>
    Linux: ```~/.cache/electron/```

3. Download Electron package from taobao mirror

    Windows： https://npm.taobao.org/mirrors/electron/<%= $elec_version %>/electron-v<%= $elec_version %>-win32-x64.zip <br>
    macOS:    https://npm.taobao.org/mirrors/electron/<%= $elec_version %>/electron-v<%= $elec_version %>-darwin-x64.zip <br>
    Linux:    https://npm.taobao.org/mirrors/electron/<%= $elec_version %>/electron-v<%= $elec_version %>-linux-x64.zip

4.  Put the downloaded zip package into **Electron Cache Directory**

# Contributors ✨

<table>
<%-
$row = $contributors.Count / 6
$pointer = 0
for ($i = 0; $i -lt $row; $i++) { -%>
    <tr>
<%- for(;$pointer -lt $contributors.Count; $pointer++) { 
    $cur = $contributors[$pointer]
    $cur_html_url = $cur.html_url
    $cur_avatar_url = $cur.avatar_url
    $cur_login = $cur.login
-%>
        <td align="center"><a href="<%= $cur_html_url %>"><img src="<%= $cur_avatar_url %>" width="100px;" alt="<%= $cur_login %>" style="border-radius:50%;"/><br /><sub><b><%= $cur_login %></b></sub></a></td>
<%- } -%>
    </tr>
<%- } -%>
</table>