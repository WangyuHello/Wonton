# Wonton

**民以食为天**

![wonton](./imgs/wonton.jpg)

Multilingual README：[English](./README.en-US.md), [简体中文](./README.md)


# Compilation Guide

## Dependencies

- NodeJS : https://nodejs.org/en/
- .NET Core SDK: https://dotnet.microsoft.com/download

## Compilation Steps

### Build for the first time

1. Install NodeJS https://nodejs.org/en/
    
    LTS or Current versions are both supported

2. Install .NET Core SDK https://dotnet.microsoft.com/download

    Requires .NET Core SDK version >= 3.1 , do not install Runtime versions

3. Install ElectronNET 

    ```powershell
    dotnet tool install -g ElectronNET.CLI
    ```

4. Execute under FudanFPGA.CrossUI.Web

    ```powershell
    dotnet build -c Release
    ```

5. Execute under FudanFPGA.CrossUI.Web\ClientApp 
   
   *This step may take a pretty long time depending on network situations.*

    ```powershell
    npm i
    ```

6. Patch FudanFPGA.CrossUI.Web\ClientApp\node_modules\react-scripts\config\webpack.config.js

    Add ```target: 'electron-renderer'``` in the ```return``` function, and save

    ![webpack](./imgs/target.png)

7. Execute under FudanFPGA.CrossUI.Web

    *This step may take a pretty long time depending on network situations.*

    Compile for macOS
    ```powershell
    ~/.dotnet/tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
    ```

    Or, compile for Windows
    ```powershell
    ~/.dotnet/tools/electronize build /target win /package-json .\ClientApp\electron.package.json
    ```
8. Compiled binaries are located under FudanFPGA.CrossUI.Web\bin\Desktop

### Re-build

1. Execute under FudanFPGA.CrossUI.Web

    Compile for macOS
    ```powershell
    ~/.dotnet/tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
    ```

    Or，compile for Windows
    ```powershell
    ~/.dotnet/tools/electronize build /target win /package-json .\ClientApp\electron.package.json
    ```
2. Compiled binaries are located under FudanFPGA.CrossUI.Web\bin\Desktop