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
        public static void Initialize()
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
        }
    }
}
