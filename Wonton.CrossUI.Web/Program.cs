using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Hosting;
using BeetleX.FastHttpApi.Hosting;
using BeetleX.FastHttpApi.SpanJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wonton.Common;

namespace Wonton.CrossUI.Web
{
    public class Program
    {
        public static string LaunchingProject = ""; 

        public static void Main(string[] args)
        {
            SetLaunchingProject(args);
            CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder
        //                .UseElectron(args)
        //                .UseStartup<Startup>();
        //        });

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => 
                {
                    webBuilder
                        .UseElectron(args)
                        .UseStartup<Startup2>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                    .AddSingleton<FPGAManager>()
                    .UseBeetlexHttp(o => {
                        o.Port = GetElectronPort(args);
                        o.LogToConsole = true;
                        o.LogLevel = BeetleX.EventArgs.LogType.Debug;
                        o.SetDebug();
                        //o.AddFilter<SpanJsonResultFilter>();
                    }, typeof(Program).Assembly);
                });

        static void SetLaunchingProject(string[] args)
        {
            var pjs = args.Where(s => s.EndsWith("hwproj"));
            foreach (var item in pjs)
            {
                LaunchingProject = item;
            }
        }

        static int GetElectronPort(string[] args)
        {
            foreach (string argument in args)
            {
                if (argument.ToUpper().Contains("ELECTRONWEBPORT"))
                {
                    return int.Parse(argument.ToUpper().Replace("/ELECTRONWEBPORT=", ""));
                }
            }

            return 8080;
        }
    }
}
