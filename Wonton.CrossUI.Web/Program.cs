using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wonton.CrossUI.Web.Extensions;

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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseElectron(args)
                        .UseStartup<Startup>()
                        .ConfigureLogging(logging =>
                        {
                            logging.AddConsole();
                            logging.AddDebug();
                            logging.AddEventSourceLogger();
                            logging.AddFile("logs/wonton-{Date}.log");
                        });
                });

        static void SetLaunchingProject(string[] args)
        {
            var pjs = args.Where(s => s.EndsWith("hwproj"));
            foreach (var item in pjs)
            {
                LaunchingProject = item;
            }
        }
    }
}
