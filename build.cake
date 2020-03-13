#addin nuget:?package=Cake.DoInDirectory&version=3.3.0
#addin nuget:?package=Cake.Npm&version=0.17.0
#addin nuget:?package=Cake.CMake&version=1.2.0

var target = Argument("target", "Build");
var useMagic = Argument<bool>("useMagic", true);
var elec_target_os = Argument("targetOS", "SameAsHost");
var addi_name = Argument("AdditionalName", "");
var release_dir = Argument("releaseDir", "Build");

var isHostMac = false;
var isHostWin = false;
var isHostLinux = false;

var elec_ver = "7.1.2";
var elec_args = "";
var elec_cache_dir = "";
var elec_bin = "";
var elec_name = "";
var elec_full_name = "";
var elec_target_os2 = "";
var host_os = "";

var npm_reg = "https://registry.npm.taobao.org";
var build_path = "";

void Rename(string dir, string addi_name, string ext)
{
    var arcs = System.IO.Directory.EnumerateFiles(dir, "*."+ext);
    foreach (var f in arcs)
    {
        var n = System.IO.Path.GetFileNameWithoutExtension(f);
        n = n + "-" + addi_name;
        var f2 = System.IO.Path.Combine(dir, n + "." + ext);
        System.IO.File.Move(f, f2, true);
        Information("重命名: "+f2);
    }
}


isHostMac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
isHostWin = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
isHostLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);

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
elec_full_name = System.IO.Path.Combine(elec_cache_dir, elec_name);

Information("Electron Cache Dir: "+ elec_cache_dir);
Information("Build for "+elec_target_os+" on "+host_os);
Information("Electron name "+elec_name);
Information("Electron full name "+elec_full_name);

build_path = MakeAbsolute(Directory(System.IO.Path.Combine(".", "Wonton.CrossUI.Web", "bin", "Desktop"))).FullPath;
Information("Build path "+build_path);


Task("BuildNative")
    .WithCriteria(FileExists("NativeDeps.zip"))
    .Does(() =>
{
    Information("构建Native依赖");
    if(DirectoryExists("VLFDDriver")){DeleteDirectory("VLFDDriver", new DeleteDirectorySettings{Recursive=true, Force=true});}
    if(DirectoryExists("SharpVLFD")){DeleteDirectory("SharpVLFD", new DeleteDirectorySettings{Recursive=true, Force=true});}
    Unzip("NativeDeps.zip", ".");

    if(IsRunningOnWindows())
    {
        var is64 = System.Environment.Is64BitOperatingSystem;
        var p = PlatformTarget.x64;
        if(!is64){ p = PlatformTarget.x86;  }
        MSBuild("./VLFDDriver/VLFDDriver.sln", new MSBuildSettings {
            Verbosity = Verbosity.Minimal,
            Configuration = "Release",
            PlatformTarget = p
        });
    }
    else 
    {
        DoInDirectory("./VLFDDriver/VLFDLibUSBDriver", () => {
            var shs = GetFiles("../**/*.sh");
            foreach(var sh in shs)
            {
                Information("Add excutable permission for "+sh);
                StartProcess("chmod", new ProcessSettings { Arguments = "+x "+ sh });
            }
            CMake(new CMakeSettings
            {
                SourcePath = ".",
                OutputPath = "."
            });
            CMake(new CMakeSettings
            {
                SourcePath = ".",
                OutputPath = "."
            });
            CMakeBuild(new CMakeBuildSettings
            {
                BinaryPath = "."
            });
        });
        
    }
});

Task("Install Electron NET CLI")
    .Does(()=> 
{
    Information("安装 ElectronNET.CLI");
    DoInDirectory("Wonton.CrossUI.Web", () => {
        StartProcess("dotnet", "tool install --tool-path tools ElectronNET.CLI");
    });
});

Task("NpmInstall")
    .Does(()=> 
{
    Information("Npm install");
    DoInDirectory(System.IO.Path.Combine("Wonton.CrossUI.Web", "ClientApp"), () => {
        var npms = new NpmInstallSettings();
        var arg = "install";
        if(useMagic) 
        {
            npms.Registry = new Uri(npm_reg);
            arg = arg + " --registry="+npm_reg;
        }
        if(IsRunningOnWindows())
        {
            NpmInstall(npms);
        }
        else
        {
            StartProcess("npm", new ProcessSettings { Arguments = arg });
        }
    });
});

Task("HackWebpack")
    .Does(()=> 
{
    Information("Hack webpack config");
    // var render_config = "target: isEnvProduction ? 'electron-renderer' : isEnvDevelopment && 'web'";
    var render_config = "target: 'electron-renderer'";
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
});

Task("DownloadElectron")
    .WithCriteria(useMagic & (!FileExists(elec_full_name)))
    .Does(()=> 
{
    Information("下载Electron");
    if(!DirectoryExists(elec_cache_dir))
    {
        CreateDirectory(elec_cache_dir);
    }

    if(!FileExists(System.IO.Path.Combine(elec_cache_dir, elec_name)))
    {        
        DoInDirectory(elec_cache_dir, () => {
            DownloadFile(elec_bin, elec_name);
        });
    }
    else 
    {
        Information("文件已下载，跳过");
    }
});

Task("BuildApp")
    .Does(() =>
{
    Information("构建App");
    DoInDirectory("Wonton.CrossUI.Web", () => {
        var elec_net_tool_bin = System.IO.Path.Combine(".", "tools", "electronize");
        var elec_net_tool_bin_local = "dotnet";
        // DotNetCoreTool(".", "electronize", elec_args, new DotNetCoreToolSettings{ ToolPath = "tools" } );
        var env_dict = new Dictionary<string, string>();
        if(useMagic)
        {
            env_dict.Add("NPM_CONFIG_REGISTRY", npm_reg);
            env_dict.Add("ELECTRON_CUSTOM_DIR", elec_ver);
            env_dict.Add("ELECTRON_MIRROR", "https://npm.taobao.org/mirrors/electron/");
        }
        // StartProcess(elec_net_tool_bin, new ProcessSettings { Arguments = elec_args, EnvironmentVariables = env_dict });
        var cli_path = MakeAbsolute(Directory("../Electron.NET/ElectronNET.CLI/bin/Release/netcoreapp3.1/") + File("ElectronNET.CLI.dll"));
        var elec_args_local = cli_path + " build /target "+ elec_target_os +" /package-json ./ClientApp/electron.package.json";

        StartProcess(elec_net_tool_bin_local, new ProcessSettings { Arguments = elec_args_local, EnvironmentVariables = env_dict });
    });


    if(release_dir != "") //拷贝到Release目录
    {

    }
});

Task("RenamePacakge")
    .WithCriteria(!string.IsNullOrEmpty(addi_name))
    .Does(() =>
{
    Information("重命名: "+addi_name);    
    Rename(build_path, addi_name, "7z");
    Rename(build_path, addi_name, "dmg");
    Rename(build_path, addi_name, "deb");
    Rename(build_path, addi_name, "zip");
});

Task("CopyPacakge")
    .WithCriteria(!string.IsNullOrEmpty(release_dir))
    .Does(() =>
{
    Information("目标目录: "+ release_dir);
    if(!DirectoryExists(release_dir)){ CreateDirectory(release_dir); }
    var files = GetFiles(build_path+"/*.7z");
    CopyFiles(files, release_dir);
    files = GetFiles(build_path+"/*.deb");
    CopyFiles(files, release_dir);
    files = GetFiles(build_path+"/*.dmg");
    CopyFiles(files, release_dir);
    files = GetFiles(build_path+"/*.exe");
    CopyFiles(files, release_dir);
});

Task("CopyToRelease")
  .Does(() =>
{
    Information(release_dir);
    var files = GetFiles("./Build/**/*.7z");
    CopyFiles(files, release_dir);
    files = GetFiles("./Build/**/*.deb");
    CopyFiles(files, release_dir);
    files = GetFiles("./Build/**/*.dmg");
    CopyFiles(files, release_dir);
    files = GetFiles("./Build/**/*.exe");
    CopyFiles(files, release_dir);
});

Task("BuildElectronCLI")
  .Does(() =>
{
    Information("构建CLI");
    DotNetCoreBuild("Electron.NET/ElectronNET.CLI/ElectronNET.CLI.csproj", new DotNetCoreBuildSettings { Configuration = "Release" });

});

Task("Build")
    .IsDependentOn("BuildElectronCLI")
    .IsDependentOn("BuildNative")
    .IsDependentOn("NpmInstall")
    .IsDependentOn("HackWebpack")
    .IsDependentOn("DownloadElectron")
    .IsDependentOn("BuildApp")
    .IsDependentOn("RenamePacakge")
    .IsDependentOn("CopyPacakge")
    .Does(() =>
{

});

RunTarget(target);