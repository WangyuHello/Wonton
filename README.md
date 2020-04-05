# 馄饨

![chip32](./Imgs/chip32.png) **民以食为天**

![wonton](./Imgs/wonton2.gif)

多语言文档：[English](./README.en-US.md), [简体中文](./README.md)

![GitHub](https://img.shields.io/github/license/WangyuHello/Wonton?label=%E8%AE%B8%E5%8F%AF%E8%AF%81&style=flat-square)
![csharp](https://img.shields.io/badge/%E8%AF%AD%E8%A8%80-C%23-orange?style=flat-square)
![js](https://img.shields.io/badge/%E8%AF%AD%E8%A8%80-JavaScript-yellow?style=flat-square)
![platform](https://img.shields.io/badge/%E5%B9%B3%E5%8F%B0-Windows%20|%20macOS%20|%20Linux-blue?style=flat-square)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/WangyuHello/Wonton?label=%E5%8F%91%E5%B8%83%E7%89%88%E6%9C%AC&style=flat-square)
![GitHub All Releases](https://img.shields.io/github/downloads/WangyuHello/Wonton/total?label=%E4%B8%8B%E8%BD%BD%E9%87%8F&style=flat-square)

| Windows | macOS | Linux |
|:------------------------:|:------------------------:|:---------------------------:|
| ![win ](./Imgs/win.png) | ![mac ](./Imgs/mac.png) | ![lnx ](./Imgs/ubuntu.png) |

## 构建状态

| 构建平台        | Windows                                                                                                                                                                                                                              | macOS                                                                                                                                                                                                                              | Linux                                                                                                                                                                                                                              |
|-----------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Travis CI       | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                      | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                    | [![Build Status](https://www.travis-ci.org/WangyuHello/Wonton.svg?branch=master)](https://www.travis-ci.org/WangyuHello/Wonton)                                                                                                    |
| Azure Pipelines | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=Windows)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=macOS)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) | [![Build Status](https://dev.azure.com/FudanUniversity/Wonton/_apis/build/status/WangyuHello.Wonton?branchName=master&jobName=Linux)](https://dev.azure.com/FudanUniversity/Wonton/_build/latest?definitionId=1&branchName=master) |


# 使用

## [发现 Bug 请提交 Issue](https://github.com/WangyuHello/Wonton/issues)
## [使用文档](https://github.com/WangyuHello/Wonton/wiki/%E8%BD%AF%E4%BB%B6%E4%BD%BF%E7%94%A8%E7%AE%80%E4%BB%8B)
## [添加新的器件](https://github.com/WangyuHello/Wonton/wiki/%E5%A6%82%E4%BD%95%E6%B7%BB%E5%8A%A0%E6%96%B0%E7%9A%84%E5%99%A8%E4%BB%B6)

# 编译指南

## 依赖软件

- NodeJS : https://nodejs.org/en/
- .NET Core SDK: https://dotnet.microsoft.com/download

## 编译步骤

1. 安装 NodeJS https://nodejs.org/en/
    
    LTS 和 Current 版本均可

2. 安装 .NET Core SDK https://dotnet.microsoft.com/download

    需 >= 3.1 版本，请勿安装 Runtime 版本

3. 克隆源代码仓库

    ```powershell
    git clone --recursive https://github.com/WangyuHello/Wonton.git
    ```

4. 在 Wonton 目录下运行

    ```powershell
    dotnet tool install --tool-path tools Cake.Tool
    ```

    > Cake.Tool 只需要安装一次

5. 开始编译，在 Wonton 目录下运行

    Windows
    ```powershell
    .\tools\dotnet-cake
    ```

    macOS、Linux
    ```bash
    ./tools/dotnet-cake
    ```

    **如果是海外用户**

    Windows
    ```powershell
    .\tools\dotnet-cake -useMagic=false
    ```

    macOS、Linux
    ```bash
    ./tools/dotnet-cake -useMagic=false
    ```

6. 编译后的文件在 Build 目录下

# 驱动编译指南

## 依赖软件

### Windows

- Visual Studio 2019 (MSVC 142)

### macOS

- Xcode
- cmake
- autoconf
- automake
- libtool
- m4
- libudev-dev

### Linux

- gcc
- cmake
- autoconf
- automake
- libtool
- m4
- libudev-dev

## 编译步骤

1. 将 **NativeDeps.zip** 压缩包复制到 Wonton目录下，其余步骤同[自动编译步骤](####自动编译步骤)

    *驱动代码未开源*

2. 如果想要撤销驱动编译，请删除 **NativeDeps.zip** 、VLFDDriver、SharpVLFD目录。

# 问题解决

#### Electron无法下载

1. 删除 Wonton.CrossUI.Web\ClientApp\node_modules\electron 目录

2. 找到**Electron缓存目录**，如果没有可自行创建。

    Windows： ```%LOCALAPPDATA%\electron\Cache``` <br>
    macOS:  ```~/Library/Caches/electron/``` <br>
    Linux: ```~/.cache/electron/```

3. 从淘宝镜像下载Electron包

    Windows： https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-win32-x64.zip <br>
    macOS:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-darwin-x64.zip <br>
    Linux:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-linux-x64.zip


4. 将上述下载的zip包存放在Electron缓存目录中即可

# 贡献 ✨

<table>
  <tr>
    <td align="center"><a href="https://github.com/WangyuHello"><img src="https://avatars2.githubusercontent.com/u/16507233?v=4" width="100px;" alt="" style="border-radius:50%;"/><br /><sub><b>WangyuHello</b></sub></a></td>
    <td align="center"><a href="https://github.com/GeraltShi"><img src="https://avatars0.githubusercontent.com/u/25215492?v=4" width="100px;" alt="" style="border-radius:50%;"/><br /><sub><b>GeraltShi</b></sub></a></td>
  </tr>
</table>

<!-- https://api.github.com/repos/WangyuHello/Wonton/contributors?page=1&per_page=100 -->