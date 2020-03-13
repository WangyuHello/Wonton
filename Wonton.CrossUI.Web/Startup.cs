using System.Reflection.Emit;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Wonton.Common;
using Wonton.CrossUI.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System;
using System.Diagnostics;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
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
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
            Task.Run(async () =>
            {
                var window = await Electron.WindowManager.CreateWindowAsync(
                    new BrowserWindowOptions
                    {
                        TitleBarStyle = TitleBarStyle.hiddenInset,
                        Frame = false,
                        Width = 1000,
                        Height = 650,
                        BackgroundColor = "#FFF"
                    });
                window.OnClosed += lifetime.StopApplication;
                ElectronIPC.SetWindow(window);
                ElectronIPC.Initialize(window);
#if DEBUG
                window.WebContents.OpenDevTools(new OpenDevToolsOptions
                {
                    Mode = DevToolsMode.detach
                });
#endif
            });

            ElectronIPC.SetMenu();
        }

        
    }
}
