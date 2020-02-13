#addin nuget:?package=Cake.DoInDirectory&version=3.3.0
#addin nuget:?package=Cake.Npm&version=0.17.0

var target = Argument("target", "Build");

var isMac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
var isWin = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

var elec_ver = "7.1.2";
var elec_args = "";
var elec_cache_dir = "";
var elec_bin = "";
var elec_name = "";

if(isMac) 
{
    elec_args = "build /target osx /package-json ./ClientApp/electron.package.json";
    elec_bin = "https://npm.taobao.org/mirrors/electron/"+elec_ver+"/electron-v"+elec_ver+"-darwin-x64.zip";
    elec_name = "electron-v"+elec_ver+"-darwin-x64.zip";
    var HOME = EnvironmentVariable("HOME");
    elec_cache_dir = System.IO.Path.Combine(HOME, "Library", "Caches", "electron"); 
}
else if(isWin)
{
    elec_args = "build /target win /package-json .\\ClientApp\\electron.package.json";
    elec_bin = "https://npm.taobao.org/mirrors/electron/"+elec_ver+"/electron-v"+elec_ver+"-win32-x64.zip";
    elec_name = "electron-v"+elec_ver+"-win32-x64.zip";
    var LOCALAPPDATA = EnvironmentVariable("LOCALAPPDATA");
    elec_cache_dir = System.IO.Path.Combine(LOCALAPPDATA, "electron", "Cache");
} 

Information("Electron Cache Dir: "+ elec_cache_dir);


Task("Build")
  .Does(() =>
{
    Information("开始构建");

    Information("安装 ElectronNET.CLI");
    DoInDirectory("FudanFPGA.CrossUI.Web", () => {
        StartProcess("dotnet", new ProcessSettings { Arguments = "tool install --tool-path tools ElectronNET.CLI" });

        // DotNetCoreTool(".", "install", "ElectronNET.CLI", new DotNetCoreToolSettings{ ToolPath = "tools" });
        //DotNetCoreTool(".", "electronize", elec_args);
    });

    Information("npm install");
    DoInDirectory(System.IO.Path.Combine("FudanFPGA.CrossUI.Web", "ClientApp"), () => {
        NpmInstall();
    });

    Information("开始构建C# Host程序");
    DotNetCoreBuild("FudanFPGA.CrossUI.Web", new DotNetCoreBuildSettings { Configuration = "Release" });

    Information("Hack webpack config");
    var render_config = "target: 'electron-renderer'";
    var config_file = System.IO.Path.Combine("FudanFPGA.CrossUI.Web", "ClientApp", "node_modules", "react-scripts", "config", "webpack.config.js");
    var config_contents = System.IO.File.ReadAllLines(config_file);
    List<string> modified_contents = new List<string>();
    var modified = false;
    foreach (var line in config_contents)
    {
        if (line.Contains(render_config))
        {
            modified = true;
            break;
        }
    }

    if (!modified)
    {
        foreach (var line in config_contents)
        {
            modified_contents.Add(line);
            if (line.Contains("mode: isEnvProduction"))
            {
                modified_contents.Add("    " + render_config + ",");
            }
        }

        System.IO.File.WriteAllText(config_file, string.Join(Environment.NewLine, modified_contents));
    }

    Information("下载Electron");
    if(!DirectoryExists(elec_cache_dir))
    {
        CreateDirectory(elec_cache_dir);
    }

    if(!FileExists(System.IO.Path.Combine(elec_cache_dir, elec_name)))
    {
        var elec_file = System.IO.Path.Combine("FudanFPGA.CrossUI.Web", "tools", elec_name);
        
        DoInDirectory(System.IO.Path.Combine("FudanFPGA.CrossUI.Web", "tools"), () => {
            DownloadFile(elec_bin, elec_name);
        });
        
        CopyFileToDirectory(elec_file, elec_cache_dir);
    }
    else 
    {
        Information("文件已下载，跳过");
    }


    Information("构建App");
    DoInDirectory("FudanFPGA.CrossUI.Web", () => {
        var elec_net_tool_bin = System.IO.Path.Combine(".", "tools", "electronize");
        // DotNetCoreTool(".", "electronize", elec_args, new DotNetCoreToolSettings{ ToolPath = "tools" } );

        StartProcess(elec_net_tool_bin, new ProcessSettings { Arguments = elec_args });
    });
});

RunTarget(target);