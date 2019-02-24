using CF.WebBootstrap.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CF.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configuration has to come before adding logging, as it is used to configure the logger.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Add logging as early as possible to record errors ramping up the application.
            var logger = configuration.AddLogging();

            try
            {
                logger.Information("Running the web host.");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                logger.Critical(ex, "Web host terminated unexpectedly.");
            }
            finally
            {
                // Ensure all entries are sent to the configured sinks.
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
