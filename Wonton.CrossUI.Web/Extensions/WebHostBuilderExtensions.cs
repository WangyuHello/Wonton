using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wonton.CrossUI.Web.Services;

namespace Wonton.CrossUI.Web.Extensions
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseElectron(this IWebHostBuilder builder, string[] args)
        {
            foreach (string argument in args)
            {
                if (argument.ToUpper().Contains("ELECTRONPORT"))
                {
                    BridgeSettings.SocketPort = argument.ToUpper().Replace("/ELECTRONPORT=", "");
                    Console.WriteLine("Use Electron Port: " + BridgeSettings.SocketPort);
                }
                else if (argument.ToUpper().Contains("ELECTRONWEBPORT"))
                {
                    BridgeSettings.WebPort = argument.ToUpper().Replace("/ELECTRONWEBPORT=", "");
                }
            }

            builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                .UseUrls("http://127.0.0.1:" + BridgeSettings.WebPort);
            

            return builder;
        }
    }
}
