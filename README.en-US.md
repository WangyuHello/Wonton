# Wonton

![chip32](./Imgs/chip32.png) **Hunger breeds discontentment**

![wonton](./Imgs/wonton2.gif)

Multilingual README：[English](./README.en-US.md), [简体中文](./README.md)

![GitHub](https://img.shields.io/github/license/WangyuHello/Wonton?style=flat-square)
![csharp](https://img.shields.io/badge/language-C%23-orange?style=flat-square)
![js](https://img.shields.io/badge/language-JavaScript-yellow?style=flat-square)
![platform](https://img.shields.io/badge/platform-Windows%20|%20macOS%20|%20Linux-blue?style=flat-square)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/WangyuHello/Wonton?style=flat-square)
![GitHub All Releases](https://img.shields.io/github/downloads/WangyuHello/Wonton/total?style=flat-square)

| Windows | macOS | Linux |
|:------------------------:|:------------------------:|:---------------------------:|
| ![win ](./Imgs/win.png) | ![mac ](./Imgs/mac.png) | ![lnx ](./Imgs/ubuntu.png) |

## Build Status

| Build Platform        | Windows                                                                                                                                                                                                                              | macOS                                                                                                                                                                                                                              | Linux                                                                                                                                                                                                                              |
|-----------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Travis CI       | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                      | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                    | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                    |
| Azure Pipelines | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=Windows)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=macOS)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=Linux)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) |

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

    Windows： https://npm.taobao.org/mirrors/electron/8.2.0/electron-v8.2.0-win32-x64.zip <br>
    macOS:    https://npm.taobao.org/mirrors/electron/8.2.0/electron-v8.2.0-darwin-x64.zip <br>
    Linux:    https://npm.taobao.org/mirrors/electron/8.2.0/electron-v8.2.0-linux-x64.zip

4.  Put the downloaded zip package into **Electron Cache Directory**

# Contributors ✨

<table>
  <tr>
    <td align="center"><a href="https://github.com/WangyuHello"><img src="https://avatars2.githubusercontent.com/u/16507233?v=4" width="100px;" alt=""/><br /><sub><b>WangyuHello</b></sub></a></td>
    <td align="center"><a href="https://github.com/GeraltShi"><img src="https://avatars0.githubusercontent.com/u/25215492?v=4" width="100px;" alt=""/><br /><sub><b>GeraltShi</b></sub></a></td>
  </tr>
</table>