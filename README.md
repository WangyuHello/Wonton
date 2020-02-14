# 馄饨

**民以食为天**

![wonton](./imgs/wonton.jpg)

支持Windows、macOS和Linux

多语言文档：[English](./README.en-US.md) (不完善), [简体中文](./README.md)

# 编译指南

## 依赖软件

- NodeJS : https://nodejs.org/en/
- .NET Core SDK: https://dotnet.microsoft.com/download

## 自动编译步骤

1. 安装 NodeJS https://nodejs.org/en/
    
    LTS 和 Current 版本均可

2. 安装 .NET Core SDK https://dotnet.microsoft.com/download

    需 >= 3.1 版本，请勿安装 Runtime 版本

3. 在 Wonton 目录下运行

    ```powershell
    dotnet tool install --tool-path tools Cake.Tool
    ```

    > Cake.Tool 只需要安装一次

4. 开始编译

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
    .\tools\dotnet-cake -useMagic="false"
    ```

    macOS、Linux
    ```bash
    ./tools/dotnet-cake -useMagic="false"
    ```

5. 编译后的文件在 Wonton.CrossUI.Web\bin\Desktop 目录下

## 手动编译步骤

### 首次编译

1. 安装 NodeJS https://nodejs.org/en/
    
    LTS和Current版本均可

2. 安装 .NET Core SDK https://dotnet.microsoft.com/download

    需 >= 3.1 版本，请勿安装 Runtime 版本

3. 在 Wonton.CrossUI.Web 目录运行

    ```powershell
    dotnet tool install --tool-path tools ElectronNET.CLI
    ```

4. 在 Wonton.CrossUI.Web 目录运行

    ```powershell
    dotnet build -c Release
    ```

5. 在 Wonton.CrossUI.Web\ClientApp 目录运行，此步骤可能需要较长时间，视网络状况。

    ```powershell
    npm i
    ```

6. 修改 Wonton.CrossUI.Web\ClientApp\node_modules\react-scripts\config\webpack.config.js 文件, 在 ```return``` 函数中添加一个字段 ```target: 'electron-renderer'``` （注意逗号）然后保存。

    ![webpack](./imgs/target.png)

7. 如果是海外用户请跳过此步骤

    - 配置Electron

    1. 找到**Electron缓存目录**，如果没有可自行创建。

        Windows： ```%LOCALAPPDATA%\electron\Cache``` <br>
        macOS:  ```~/Library/Caches/electron/``` <br>
        Linux: ```~/.cache/electron/```

    2. 从淘宝镜像下载Electron包

        Windows： https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-win32-x64.zip <br>
        macOS:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-darwin-x64.zip <br>
        Linux:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-linux-x64.zip

    3. 将上述下载的zip包存放在**Electron缓存目录**中

8. 在 Wonton.CrossUI.Web 目录运行如下命令，此步骤可能需要较长时间，视网络状况。如果出现Electron无法下载问题，请查看[Electron无法下载](####Electron无法下载)

    编译 Windows 版本
    ```powershell
    .\tools\electronize build /target win /package-json .\ClientApp\electron.package.json
    ```

    或者，编译 macOS 版本
    ```bash
    ./tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
    ```
9. 编译后的文件在 Wonton.CrossUI.Web\bin\Desktop 目录下

### 非首次编译

1. 在 Wonton.CrossUI.Web 目录运行

    编译 Windows 版本
    ```powershell
    .\tools\electronize build /target win /package-json .\ClientApp\electron.package.json
    ```

    或者，编译 macOS 版本
    ```bash
    ./tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
    ```
2. 编译后的文件在 Wonton.CrossUI.Web\bin\Desktop 目录下

### 问题解决

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