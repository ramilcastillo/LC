using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LifeCouple.Server.Instrumentation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LifeCouple.Server
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var serilogConfigUsed = LCLog.Initialize();

            AppInfo.Current.AppInitializingStarted();

            try
            {
                LCLog.Information($"Starting web host with Serilog cfg:{serilogConfigUsed}");

                CreateWebHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception ex)
            {
                LCLog.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                LCLog.Information("Exiting web host");
                LCLog.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging(log =>
            {
                log.ClearProviders();
            });
    }
}
