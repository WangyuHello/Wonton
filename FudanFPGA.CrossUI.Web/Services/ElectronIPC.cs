using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace FudanFPGA.CrossUI.Web.Services
{
    public class ElectronIPC
    {
        public static void Initialize()
        {
            Electron.IpcMain.On("working-status", (args) =>
            {
                if (args is bool)
                {
                    if ((bool)args)
                    {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        mainWindow.SetProgressBar(0, new ProgressBarOptions{Mode = ProgressBarMode.indeterminate});
                    }
                    else
                    {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        mainWindow.SetProgressBar(-1, new ProgressBarOptions { Mode = ProgressBarMode.none });
                    }
                }
            });

            Electron.IpcMain.On("window-status", (args) =>
            {
                var state = (string) args;
                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                switch (state)
                {
                    case "minimize":
                        mainWindow.Minimize();
                        break;
                    case "maximize":
                        mainWindow.Maximize();
                        break;
                    case "restore":
                        mainWindow.Restore();
                        break;
                }
            });
        }
    }
}
