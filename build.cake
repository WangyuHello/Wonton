#addin nuget:?package=Cake.DoInDirectory&version=3.3.0
#addin nuget:?package=Cake.Npm&version=0.17.0

var target = Argument("target", "Build");
var useMagic = Argument("useMagic", "true");
var elec_target_os = Argument("targetOS", "SameAsHost");
var addi_name = Argument("AdditionalName", "");
var release_dir = Argument("releaseDir", "");

var isHostMac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
var isHostWin = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
var isHostLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);

var elec_ver = "7.1.2";
var elec_args = "";
var elec_cache_dir = "";
var elec_bin = "";
var elec_name = "";
// var elec_target_os = "";
var elec_target_os2 = "";
var host_os = "";

var npm_reg = "https://registry.npm.taobao.org";

if(isHostMac)
{
    if(elec_target_os == "SameAsHost") { elec_target_os = "osx"; }
    host_os = "macOS";
    var HOME = EnvironmentVariable("HOME");
    elec_cache_dir = System.IO.Path.Combine(HOME, "Library", "Caches", "electron"); 
}
else if(isHostWin)
{
    if(elec_target_os == "SameAsHost") { elec_target_os = "win"; }
    host_os = "Windows";
    var LOCALAPPDATA = EnvironmentVariable("LOCALAPPDATA");
    elec_cache_dir = System.IO.Path.Combine(LOCALAPPDATA, "electron", "Cache");
}
else if(isHostLinux)
{
    host_os = "Linux";
    if(elec_target_os == "SameAsHost") { elec_target_os = "linux"; }
    var HOME = EnvironmentVariable("HOME");
    elec_cache_dir = System.IO.Path.Combine(HOME, ".cache", "electron"); 
}

if(elec_target_os == "osx")
{
    elec_target_os2 = "darwin";
}
if(elec_target_os == "win")
{
    elec_target_os2 = "win32";
}
if(elec_target_os == "linux")
{
    elec_target_os2 = "linux";
}

elec_args = "build /target "+ elec_target_os +" /package-json ./ClientApp/electron.package.json";
elec_bin = "https://npm.taobao.org/mirrors/electron/"+elec_ver+"/electron-v"+elec_ver+"-"+ elec_target_os2 +"-x64.zip";
elec_name = "electron-v"+elec_ver+"-"+ elec_target_os2 +"-x64.zip";

Information("Electron Cache Dir: "+ elec_cache_dir);
Information("Build for "+elec_target_os+" on "+host_os);

void Rename(string dir, string addi_name, string ext)
{
    var arcs = System.IO.Directory.EnumerateFiles(dir, "*."+ext);
    foreach (var f in arcs)
    {
        var n = System.IO.Path.GetFileNameWithoutExtension(f);
        n = n + "-" + addi_name;
        var f2 = System.IO.Path.Combine(dir, n + "." + ext);
        System.IO.File.Move(f, f2, true);
    }
}

Task("Build")
  .Does(() =>
{
    Information("开始构建");

    Information("安装 ElectronNET.CLI");
    DoInDirectory("Wonton.CrossUI.Web", () => {
        StartProcess("dotnet", new ProcessSettings { Arguments = "tool install --tool-path tools ElectronNET.CLI" });

        // DotNetCoreTool(".", "install", "ElectronNET.CLI", new DotNetCoreToolSettings{ ToolPath = "tools" });
        //DotNetCoreTool(".", "electronize", elec_args);
    });

    Information("npm install");
    DoInDirectory(System.IO.Path.Combine("Wonton.CrossUI.Web", "ClientApp"), () => {
        var npms = new NpmInstallSettings();
        var arg = "install";
        if(useMagic == "true") 
        {
            npms.Registry = new Uri(npm_reg);
            arg = arg + " --registry="+npm_reg;
        }
        if(isHostWin)
        {
            NpmInstall(npms);
        }
        else
        {
            StartProcess("npm", new ProcessSettings { Arguments = arg });
        }
    });

    Information("开始构建C# Host程序");
    DotNetCoreBuild("Wonton.CrossUI.Web", new DotNetCoreBuildSettings { Configuration = "Release" });

    Information("Hack webpack config");
    var render_config = "target: isEnvProduction ? 'electron-renderer' : isEnvDevelopment && 'web'";
    // var render_config = "target: 'electron-renderer'";
    var config_file = System.IO.Path.Combine("Wonton.CrossUI.Web", "ClientApp", "node_modules", "react-scripts", "config", "webpack.config.js");
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

    if(useMagic == "true") 
    {
        Information("下载Electron");
        if(!DirectoryExists(elec_cache_dir))
        {
            CreateDirectory(elec_cache_dir);
        }
    
        if(!FileExists(System.IO.Path.Combine(elec_cache_dir, elec_name)))
        {
            var elec_file = System.IO.Path.Combine("Wonton.CrossUI.Web", "tools", elec_name);
            
            DoInDirectory(System.IO.Path.Combine("Wonton.CrossUI.Web", "tools"), () => {
                DownloadFile(elec_bin, elec_name);
            });
            
            CopyFileToDirectory(elec_file, elec_cache_dir);
        }
        else 
        {
            Information("文件已下载，跳过");
        }

    }

    Information("构建App");
    DoInDirectory("Wonton.CrossUI.Web", () => {
        var elec_net_tool_bin = System.IO.Path.Combine(".", "tools", "electronize");
        // DotNetCoreTool(".", "electronize", elec_args, new DotNetCoreToolSettings{ ToolPath = "tools" } );
        var env_dict = new Dictionary<string, string>();
        if(useMagic == "true")
        {
            env_dict.Add("NPM_CONFIG_REGISTRY", npm_reg);
        }
        StartProcess(elec_net_tool_bin, new ProcessSettings { Arguments = elec_args, EnvironmentVariables = env_dict });
    });

    if(!string.IsNullOrEmpty(addi_name))
    {
        Information("重命名:"+addi_name);
        var archives = GetFiles(System.IO.Path.Combine("Wonton.CrossUI.Web", "bin", "Desktop", "*.7z"));
        var build_path = MakeAbsolute(Directory(System.IO.Path.Combine(".", "Wonton.CrossUI.Web", "bin", "Desktop"))).FullPath;
        Information(build_path);
        Rename(build_path, addi_name, "7z");
        Rename(build_path, addi_name, "zip");
    }
});

Task("CopyToRelease")
  .Does(() =>
{
    Information(release_dir);
    var files = GetFiles("./Build/**/*.7z");
    CopyFiles(files, release_dir);
});

RunTarget(target);