using CF.Common.Logging;
using CF.Web.AspNetCore.Extensions.ConfigurationRoot;
using CF.Web.AspNetCore.Extensions.WebHostBuilder;
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
            ILogger logger;
            try
            {
                // Configuration has to come before adding logging, as it is used to configure the logger.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                // Add logging as early as possible to record errors ramping up the application.
                logger = configuration.AddLogging();
            }
            catch (Exception ex)
            {
                // Ensure any errors incurred while getting configuration and logging set up
                // are written to stdout.
                Console.WriteLine($"Error occurred building configuration:\n{ex.Message}");
                throw;
            }

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
