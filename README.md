# FudanFPGAInterface

为复旦微电子FPGA课程量身打造的教学辅助软件。支持Windows和macOS操作系统。

![win](./Imgs/win.png)

![mac](./Imgs/mac.png)


# 编译指南

## 依赖软件

- NodeJS : https://nodejs.org/en/
- .NET Core SDK: https://dotnet.microsoft.com/download

## 编译步骤

### 首次编译

1. 安装 NodeJS https://nodejs.org/en/
    
    LTS和Current版本均可

2. 安装 .NET Core SDK https://dotnet.microsoft.com/download

    需 >= 3.1 版本，请勿安装Runtime版本

3. 安装ElectronNET 

    ```powershell
    dotnet tool install -g ElectronNET.CLI
    ```

4. 在 FudanFPGA.CrossUI.Web 目录运行

    ```powershell
    dotnet build -c Release
    ```

5. 在 FudanFPGA.CrossUI.Web\ClientApp 目录运行，此步骤可能需要较长时间，视网络状况。

    ```powershell
    npm i
    ```

6. 修改 FudanFPGA.CrossUI.Web\ClientApp\node_modules\react-scripts\config\webpack.config.js 文件, 在 ```return``` 函数中添加一个字段 ```target: 'electron-renderer'``` （注意逗号）然后保存。

    ![webpack](./Imgs/target.png)

7. 如果是海外用户请跳过此步骤

    - 配置Electron

    1. 找到**Electron缓存目录**

        Windows： ```%LOCALAPPDATA%\electron\Cache``` <br>
        macOS:  ```~/Library/Caches/electron/```

    2. 从淘宝镜像下载Electron包

        Windows： https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-win32-x64.zip <br>
        macOS:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-darwin-x64.zip

    3. 将上述下载的zip包存放在**Electron缓存目录**中

8. 在 FudanFPGA.CrossUI.Web 目录运行如下命令，此步骤可能需要较长时间，视网络状况。如果出现Electron无法下载问题，请查看[Electron无法下载](####Electron无法下载)

    编译macOS版本
    ```powershell
    ~/.dotnet/tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
    ```

    或者，编译Windows版本
    ```powershell
    ~/.dotnet/tools/electronize build /target win /package-json .\ClientApp\electron.package.json
    ```
9. 编译后的文件在 FudanFPGA.CrossUI.Web\bin\Desktop 目录下

### 非首次编译

1. 在 FudanFPGA.CrossUI.Web 目录运行

    编译macOS版本
    ```powershell
    ~/.dotnet/tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
    ```

    或者，编译Windows版本
    ```powershell
    ~/.dotnet/tools/electronize build /target win /package-json .\ClientApp\electron.package.json
    ```
2. 编译后的文件在 FudanFPGA.CrossUI.Web\bin\Desktop 目录下

### 问题解决

#### Electron无法下载

1. 删除 FudanFPGA.CrossUI.Web\ClientApp\node_modules\electron 目录

2. 找到Electron缓存目录

    Windows： ```%LOCALAPPDATA%\electron\Cache``` <br>
    macOS: ```~/Library/Caches/electron/```

3. 从淘宝镜像下载Electron包

    Windows： https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-win32-x64.zip <br>
    macOS:    https://npm.taobao.org/mirrors/electron/7.1.2/electron-v7.1.2-darwin-x64.zip

4. 将上述下载的zip包存放在Electron缓存目录中即可