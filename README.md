# FudanFPGAInterface 编译要求

- NodeJS : https://nodejs.org/en/
- dotnet : https://dotnet.microsoft.com/download

# 编译步骤

1. 安装NodeJS
2. 安装dotnet
3. 安装ElectronNET

```powershell
dotnet tool install -g ElectronNET.CLI
```

4. 在*FudanFPGA.CrossUI.Web*目录运行

```powershell
dotnet build -c Release
```

5. 在*FudanFPGA.CrossUI.Web\ClientApp*目录运行

```powershell
npm i
```

6. 修改 FudanFPGA.CrossUI.Web\ClientApp\node_modules\react-scripts\config\webpack.config.js 文件, 在 ```return``` 函数中添加一个字段 ```target: 'electron-renderer'``` 然后保存

![avatar](./Imgs/target.png)

7. 在 *FudanFPGA.CrossUI.Web* 目录运行

编译macOS版本
```powershell
~/.dotnet/tools/electronize build /target osx /package-json ./ClientApp/electron.package.json
```

或者，编译Windows版本
```powershell
~/.dotnet/tools/electronize build /target win /package-json .\ClientApp\electron.package.json
```
8. 编译后的文件在 FudanFPGA.CrossUI.Web\bin\Desktop 目录下