using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FudanFPGA.CrossUI.Web.Controllers
{
#if RELEASE
    [Route("api/[controller]")]
    [ApiController]
    public class WindowController : ControllerBase
    {
        [HttpGet("maximize")]
        public async Task MaximizeAsync()
        {
            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            if (await mainWindow.IsMaximizedAsync())
            {
                mainWindow.Restore();
            }
            else
            {
                mainWindow.Maximize();
            }
        }

        [HttpGet("minimize")]
        public void Minimize()
        {
            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            mainWindow.Minimize();
        }

        [HttpGet("working-state")]
        public void WorkingState(int state)
        {
            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            switch (state)
            {
                case 0:
                    mainWindow.SetProgressBar(-1, new ProgressBarOptions { Mode = ProgressBarMode.none });
                    break;
                case 1:
                    mainWindow.SetProgressBar(0.5, new ProgressBarOptions { Mode = ProgressBarMode.indeterminate });
                    break;
            }
        }
    }
#endif
}