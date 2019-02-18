using CF.WebBootstrap.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace CF.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Bootstrap the logger as early as possible in the startup.
            var logger = configuration.AddLogging();

            try
            {
                logger.Information("Starting web host");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                logger.Critical(ex, "Host terminated unexpectedly");
            }
            finally
            {
                logger.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                // Bootstrap custom logging.
                .UseCustomLogging()
                .Build();
    }
}
