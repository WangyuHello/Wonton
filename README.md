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

5. 修改 FudanFPGA.CrossUI.Web\ClientApp\node_modules\react-scripts\config\webpack.config.js 文件, 在 ```return``` 函数中添加一个字段 ```target: 'electron-renderer'``` 然后保存

6. 在 *FudanFPGA.CrossUI.Web* 目录运行

```powershell
electronize build /target osx /package-json .\ClientApp\electron.package.json
```

或者

```powershell
electronize build /target win /package-json .\ClientApp\electron.package.json
```
7. 编译后的文件在 FudanFPGA.CrossUI.Web\bin\Desktop 目录下