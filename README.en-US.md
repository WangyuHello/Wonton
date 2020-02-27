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

## If you find a bug, please submit an issue

# Compilation Guide

## Dependencies

- NodeJS : https://nodejs.org/en/
- .NET Core SDK: https://dotnet.microsoft.com/download

## Automated Compilation Steps

1. Install NodeJS https://nodejs.org/en/

    LTS and Current versions are both supported

2. Install .NET Core SDK https://dotnet.microsoft.com/download

    Requires .NET Core SDK version >= 3.1 , do not install Runtime versions

3. Run the scripts under Wonton directory

    ```
    dotnet tool install --tool-path tools Cake.Tool
    ```

    > Cake.Tool only needs to be installed once

4. Start compiling and run the scripts under Wonton directory

    Windows
    ```powershell
    .\tools\dotnet-cake
    ```

    macOS、Linux
    ```bash
    ./tools/dotnet-cake
    ```

    **If you are not in mainland China**

    Windows
    ```powershell
    .\tools\dotnet-cake -useMagic="false"
    ```

    macOS、Linux
    ```bash
    ./tools/dotnet-cake -useMagic="false"
    ```
5. Compiled binaries are located under Wonton.CrossUI.Web\bin\Desktop

## Manual Compilation Steps

### Build for the first time

1. Install NodeJS https://nodejs.org/en/
    
    LTS or Current versions are both supported

2. Install .NET Core SDK https://dotnet.microsoft.com/download

    Requires .NET Core SDK version >= 3.1 , do not install Runtime versions

3. Install ElectronNET 

    ```powershell
    dotnet tool install -g ElectronNET.CLI
    ```

4. Execute under Wonton.CrossUI.Web

    ```powershell
    dotnet build -c Release
    ```

5. Execute under Wonton.CrossUI.Web\ClientApp 
   
   *This step may take a pretty long time depending on network situations.*

    ```powershell
    npm i
    ```

6. Patch Wonton.CrossUI.Web\ClientApp\node_modules\react-scripts\config\webpack.config.js

    Add ```target: 'electron-renderer'``` in the ```return``` function, and save

    ![webpack](./Imgs/target.png)

7. If you are not in mainland China, please skip this step

    - Configure Electron

    1. Find**Electron Cache Directory**. You can create it if not exists.

        Windows： ```%LOCALAPPDATA%\electron\Cache``` <br>
        macOS:  ```~/Library/Caches/electron/``` <br>
        Linux: ```~/.cache/electron/```
    
    2. Download Electron package from taobao mirror

        Windows： https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-win32-x64.zip <br>
        macOS:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-darwin-x64.zip <br>
        Linux:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-linux-x64.zip

    3.  Put the downloaded zip package into **Electron Cache Directory**

8. Execute under Wonton.CrossUI.Web

    *This step may take a pretty long time depending on network situations. If you failed to download Electron, please refer to Can not download Electron*

    Compile for Windows
    ```powershell
    .\tools\electronize build /target win /package-json .\ClientApp\electron.package.json
    ```

    Or, compile for macOS
    ```bash
    ./tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
    ```

    Or, compile for Linux
    ```bash
    ./tools/electronize build /target linux /package-json ./ClientApp/electron.package.json
    ```

9. Compiled binaries are located under Wonton.CrossUI.Web\bin\Desktop

### Re-build

1. Execute under Wonton.CrossUI.Web

    Compile for Windows
    ```powershell
    .\tools\electronize build /target win /package-json .\ClientApp\electron.package.json
    ```

    Or, compile for macOS
    ```bash
    ./tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
    ```

    Or, compile for Linux
    ```bash
    ./tools/electronize build /target linux /package-json ./ClientApp/electron.package.json
    ```

2. Compiled binaries are located under Wonton.CrossUI.Web\bin\Desktop

### Problems and Solution

#### Can not download Electron

1. Delete Wonton.CrossUI.Web\ClientApp\node_modules\electron

2. Find**Electron Cache Directory**. You can create it if not exists.

    Windows： ```%LOCALAPPDATA%\electron\Cache``` <br>
    macOS:  ```~/Library/Caches/electron/``` <br>
    Linux: ```~/.cache/electron/```

3. Download Electron package from taobao mirror

    Windows： https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-win32-x64.zip <br>
    macOS:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-darwin-x64.zip <br>
    Linux:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-linux-x64.zip

4.  Put the downloaded zip package into **Electron Cache Directory**