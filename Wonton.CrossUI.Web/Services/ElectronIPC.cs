using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;

namespace Wonton.CrossUI.Web.Services
{
    public class ElectronIPC
    {
        public static void Initialize(BrowserWindow window)
        {
            Electron.IpcMain.On("working-status", (args) =>
            {
                if (args is bool b)
                {
                    if (b)
                    {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        mainWindow.SetProgressBar(2, new ProgressBarOptions{Mode = ProgressBarMode.indeterminate});
                    }
                    else
                    {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        mainWindow.SetProgressBar(-1, new ProgressBarOptions { Mode = ProgressBarMode.none });
                    }
                }
            });

            Electron.IpcMain.On("window-status", async (args) =>
            {
                var state = (string) args;
                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                switch (state)
                {
                    case "minimize":
                        mainWindow.Minimize();
                        break;
                    case "maximize":
                        if (await mainWindow.IsMaximizedAsync())
                        {
                            mainWindow.Restore();
                        }
                        else
                        {
                            mainWindow.Maximize();
                        }
                        break;
                    case "restore":
                        mainWindow.Restore();
                        break;
                }
            });

            Electron.IpcMain.On("dev-tools", o =>
            {
                window.WebContents.OpenDevTools(new OpenDevToolsOptions
                {
                    Mode = DevToolsMode.detach
                });
            });

            Electron.IpcMain.On("open-project-save-folder", async args =>
            {              
                var dirs = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions
                { 
                    Title = "选择项目目录",
                    Properties = new[] { OpenDialogProperty.openDirectory }
                });

                if (dirs.Length != 0)
                {
                    Electron.IpcMain.Send(window, "open-project-save-folder-callback", dirs[0]);
                }
            });

            Electron.IpcMain.On("open-bitfile", async args =>
            {
                var files = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions
                {
                    Title = "选择Bitfile",
                    Properties = new[] { OpenDialogProperty.openFile },
                    Filters = new [] { new FileFilter { Extensions = new[] { "bit" }, Name = "Bitfile" } }
                });

                if (files.Length != 0)
                {
                    Electron.IpcMain.Send(window, "open-bitfile-callback", files[0]);
                }
            });

            Electron.IpcMain.On("open-project-io-file", async args =>
            {
                var files = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions
                {
                    Title = "选择引脚约束文件",
                    Properties = new[] { OpenDialogProperty.openFile },
                    Filters = new[] { new FileFilter { Extensions = new[] { "xml" }, Name = "引脚约束文件" } }
                });

                if (files.Length != 0)
                {
                    Electron.IpcMain.Send(window, "open-project-io-file-callback", files[0]);
                }
            });

            Electron.IpcMain.On("open-project-file", async args =>
            {
                var files = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions
                {
                    Title = "选择项目文件",
                    Properties = new[] { OpenDialogProperty.openFile },
                    Filters = new[] { new FileFilter { Extensions = new[] { "hwproj" }, Name = "项目文件" } }
                });

                if (files.Length != 0)
                {
                    Electron.IpcMain.Send(window, "open-project-callback", files[0]);
                }
            });

            Electron.IpcMain.On("show-unsave-prompt", async args =>
            {
                var result = await Electron.Dialog.ShowMessageBoxAsync(window, new MessageBoxOptions("是否关闭未保存的项目?")
                {
                    Title = "关闭", Type = MessageBoxType.question,
                    Buttons = new[] { "ok", "cancel" }, CancelId = 1,
                    DefaultId = 2,
                });

                switch (result.Response)
                {
                    case 0:
                        Electron.IpcMain.Send(window, "exit-callback", 1); //通知渲染进行关闭
                        break;
                    default:
                        break;
                }
            });

            Electron.IpcMain.On("check-update", args =>
            {
                //await CheckUpdateAsync();
            });
        }

        public static async Task CheckUpdateAsync()
        {
            var sr = File.OpenText("electron.manifest.json");
            var manifest = await sr.ReadToEndAsync().ConfigureAwait(false);
            var mani_j = JObject.Parse(manifest);
            var cur_ver = mani_j["build"]["buildVersion"].Value<string>();
            var cur_ver_frag = cur_ver.Split('.');
            var cur_ver_dec = cur_ver_frag.Select(s => int.Parse(s)).Select((dec, ind) => dec * Math.Pow(100, cur_ver_frag.Length - ind - 1)).Sum();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.github.com/");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36 Edg/80.0.361.62");
                try
                {
                    var res = await client.GetStringAsync("https://api.github.com/repos/WangyuHello/Wonton/releases/latest");
                    var jobj = JObject.Parse(res);
                    var tag_name = jobj["tag_name"].Value<string>(); //v1.0.6
                    var release_ver = tag_name.Substring(1);
                    var release_ver_frag = release_ver.Split('.');
                    var release_ver_dec = release_ver_frag.Select(s => int.Parse(s)).Select((dec, ind) => dec * Math.Pow(100, release_ver_frag.Length - ind - 1)).Sum();

                    if (release_ver_dec > cur_ver_dec)
                    {
                        //有更新
                        Electron.Notification.Show(new NotificationOptions("馄饨FPGA", "更新: " + release_ver) 
                        { 
                            
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("检查更新失败");
                    Console.WriteLine(e);
                }
            }
        }

        public static void SetWindow(BrowserWindow window)
        {
            window.OnMaximize += () =>
            {
                Electron.IpcMain.Send(window, "window-state-maximize", 1);
            };
            window.OnUnmaximize += () =>
            {
                Electron.IpcMain.Send(window, "window-state-unmaximize", 0);
            };
            window.OnBlur += () =>
            {
                Electron.IpcMain.Send(window, "window-state-blur", 0);
            };
            window.OnFocus += () => Electron.IpcMain.Send(window, "window-state-focus", 0);
#if Windows
            DarkMode.OnDarkModeChanged += dark =>
            {
                Electron.IpcMain.Send(window, "window-state-darkmode", dark);
            };
#endif
        }

        public static void SetMenu()
        {
            var menu = new[]
            {
                new MenuItem
                {
                    Label = "帮助",
                    Role = MenuRole.help,
                    Submenu = new []
                    {
                        new MenuItem
                        {
                            Label = "关于",
                            Click = () => { Process.Start("https://github.com/WangyuHello/Wonton"); }
                        }
                    }
                }
            };

            Electron.Menu.SetApplicationMenu(menu);
        }
    }
}
