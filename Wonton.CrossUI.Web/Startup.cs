using System.Reflection.Emit;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Wonton.Common;
using Wonton.CrossUI.Web.Rpc;
using Wonton.CrossUI.Web.RPC;
using Wonton.CrossUI.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Wonton.CrossUI.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddSingleton<FPGAManager>();
            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapGrpcService<FPGARpcService>();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

#if RELEASE
            Task.Run(async () =>
            {
                var window = await Electron.WindowManager.CreateWindowAsync(
                    new BrowserWindowOptions
                    {
                        TitleBarStyle = TitleBarStyle.hiddenInset,
                        Frame = false,
                        Width = 1000,
                        Height = 650
                    });
                SetWindow(window);
            });

            SetMenu();
            ElectronIPC.Initialize();
#endif
        }

        public void SetMenu()
        {
            var menu = new []
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

        public void SetWindow(BrowserWindow window)
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
    }
}
