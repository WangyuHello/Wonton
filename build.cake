#addin nuget:?package=Cake.DoInDirectory&version=3.3.0
#addin nuget:?package=Cake.Npm&version=0.17.0
#addin nuget:?package=Cake.CMake&version=1.2.0
#addin nuget:?package=Cake.FileHelpers&version=3.2.1

var target = Argument("target", "Build");
var useMagic = Argument<bool>("useMagic", true);
var elec_target_os = Argument("targetOS", "SameAsHost");
var addi_name = Argument("AdditionalName", "");
var release_dir = Argument("releaseDir", "Build");
var clean_node = Argument<bool>("CleanNode", false);

var isHostMac = false;
var isHostWin = false;
var isHostLinux = false;

var elec_ver = "8.2.0";
var elec_args = "";
var elec_cache_dir = "";
var elec_mirror_zip = "";
var elec_mirror_sha = "";
var elec_zip_name = "";
var elec_zip_full_name = "";
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
elec_mirror_zip = "https://npm.taobao.org/mirrors/electron/"+elec_ver+"/electron-v"+elec_ver+"-"+ elec_target_os2 +"-x64.zip";
elec_mirror_sha = "https://npm.taobao.org/mirrors/electron/"+elec_ver+"/SHASUMS256.txt";
elec_zip_name = "electron-v"+elec_ver+"-"+ elec_target_os2 +"-x64.zip";
elec_zip_full_name = System.IO.Path.Combine(elec_cache_dir, elec_zip_name);

Information("Electron Cache Dir: "+ elec_cache_dir);
Information("Build for "+elec_target_os+" on "+host_os);
Information("Electron name "+elec_zip_name);
Information("Electron full name "+elec_zip_full_name);

build_path = MakeAbsolute(Directory(System.IO.Path.Combine(".", "Wonton.CrossUI.Web", "bin", "Desktop"))).FullPath;
Information("Build path "+build_path);


Task("BuildNative")
    .WithCriteria(FileExists("NativeDeps.zip"))
    .Does(() =>
{
    Information("构建Native依赖");
    DelDir("VLFDDriver");
    DelDir("SharpVLFD");
    DelFile("build.sh");
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

bool CheckSHA(string electron_file)
{
    if(!DirectoryExists(elec_cache_dir)) return false;
    if(!FileExists(elec_zip_full_name)) return false;

    bool correct = false;
    DoInDirectory(elec_cache_dir, () => {
        DelFile("SHASUMS256.txt");
        DownloadFile(elec_mirror_sha, "SHASUMS256.txt");
        var sha256 = CalculateFileHash(elec_zip_full_name, HashAlgorithm.SHA256).ToHex();
        Information("已下载的 Electron: " + sha256);
        var shas = FileReadLines("SHASUMS256.txt");
        foreach(var sha in shas)
        {
            var parts = sha.Split(new [] { ' ', '*' });
            var target_sha = parts[0];
            var zip_name = parts[2];
            if(zip_name == elec_zip_name)
            {
                if(target_sha == sha256)
                {
                    Information("SHA256 正确＜（＾－＾）＞");
                    correct = true;
                    break;
                }
                else
                {
                    Information("SHA256 错误，将重新下载");
                }
            }
        }
    });
    return correct;
}

Task("DownloadElectron")
    .WithCriteria(useMagic && (!CheckSHA(elec_zip_full_name)))
    .Does(()=> 
{
    if(!DirectoryExists(elec_cache_dir))
    {
        CreateDirectory(elec_cache_dir);
    }
    DelFile(elec_zip_full_name);
    Information("正在下载 Electron");
    DoInDirectory(elec_cache_dir, () => {
        DownloadFile(elec_mirror_zip, elec_zip_name);
    });
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
        env_dict.Add("ADDI_NAME", addi_name == "" ? "" : "-"+addi_name);
        env_dict.Add("FXDEPS", "");
        var cli_path = MakeAbsolute(Directory("../Electron.NET/ElectronNET.CLI/bin/") + File("ElectronNET.CLI.dll"));


        var elec_args_local = cli_path + " build /target "+ elec_target_os +" /package-json ./ClientApp/electron.package.json";
        StartProcess(elec_net_tool_bin_local, new ProcessSettings { Arguments = elec_args_local, EnvironmentVariables = env_dict });

        if(IsRunningOnWindows()) 
        {
            env_dict["FXDEPS"] = "-fxdependent";
            elec_args_local = cli_path + " build /target "+ elec_target_os +" /package-json ./ClientApp/electron.package.json /fxdeps";
            StartProcess(elec_net_tool_bin_local, new ProcessSettings { Arguments = elec_args_local, EnvironmentVariables = env_dict });
        }

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
    DotNetCoreBuild("Electron.NET/ElectronNET.CLI/ElectronNET.CLI.csproj", new DotNetCoreBuildSettings 
    { 
        Configuration = "Release",
        OutputDirectory = "Electron.NET/ElectronNET.CLI/bin/"
    });

});

Task("Build")
    .IsDependentOn("BuildElectronCLI")
    .IsDependentOn("BuildNative")
    .IsDependentOn("NpmInstall")
    .IsDependentOn("HackWebpack")
    .IsDependentOn("DownloadElectron")
    .IsDependentOn("BuildApp")
    .IsDependentOn("CopyPacakge")
    .Does(() =>
{

});

void DelDir(string dir)
{
    if(DirectoryExists(dir))
    {
        DeleteDirectory(dir, new DeleteDirectorySettings{Recursive=true, Force=true});
        Information("删除目录: "+ dir);
    }
}

void DelFile(string file)
{
    if(FileExists(file)) 
    {
        DeleteFile(file);
        Information("删除文件: "+ file);
    }
}

Task("Clean")
    .Does(() =>
{
    DelDir("VLFDDriver");
    DelDir("SharpVLFD");
    DelDir("Build");
    DelFile("build.sh");
    DelDir("Wonton.Common/bin");
    DelDir("Wonton.Common/obj");
    DelDir("Wonton.CrossUI/bin");
    DelDir("Wonton.CrossUI/obj");
    DelDir("Wonton.CrossUI.Web/bin");
    DelDir("Wonton.CrossUI.Web/obj");
    DelDir("Wonton.CrossUI.Web/logs");
    DelDir("Wonton.CrossUI.Web/ClientApp/build");
    DelDir("Wonton.Test/bin");
    DelDir("Wonton.Test/obj");
    DelDir("Wonton.WinUI.UWP/bin");
    DelDir("Wonton.WinUI.UWP/obj");
    DelDir("Wonton.WinUI.WPF/bin");
    DelDir("Wonton.WinUI.WPF/obj");
    DelDir("Electron.NET/ElectronNET.API/bin");
    DelDir("Electron.NET/ElectronNET.API/obj");
    DelDir("Electron.NET/ElectronNET.CLI/bin");
    DelDir("Electron.NET/ElectronNET.CLI/obj");

    if(clean_node)
    {
        DelDir("Wonton.CrossUI.Web/ClientApp/node_modules");
        DelDir("Wonton.CrossUI.Web/ElectronHost/node_modules");
    }
});

Task("InstallElectronHost")
    .Does(() =>
{
    DoInDirectory("Wonton.CrossUI.Web/ElectronHost", () => {
        var env_dict = new Dictionary<string, string>();
        var npms = new NpmInstallSettings();
        if(useMagic)
        {
            env_dict.Add("NPM_CONFIG_REGISTRY", npm_reg);
            env_dict.Add("ELECTRON_CUSTOM_DIR", elec_ver);
            env_dict.Add("ELECTRON_MIRROR", "https://npm.taobao.org/mirrors/electron/");
        }
        if(IsRunningOnWindows())
        {
            StartProcess("cmd.exe", new ProcessSettings { Arguments = "/C \"npm.cmd i\"", EnvironmentVariables = env_dict });
        }
        else
        {
            StartProcess("npm", new ProcessSettings { Arguments = "i", EnvironmentVariables = env_dict });
        }


    });
});

RunTarget(target);