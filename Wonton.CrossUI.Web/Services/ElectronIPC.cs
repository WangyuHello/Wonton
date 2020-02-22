using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;

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
                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                mainWindow.WebContents.OpenDevTools(new OpenDevToolsOptions
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
                            Label = "关于"
                        }
                    }
                }
            };

            Electron.Menu.SetApplicationMenu(menu);
        }
    }
}
