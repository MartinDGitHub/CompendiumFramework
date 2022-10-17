using CF.Common.Logging;
using CF.Infrastructure.Logging;
using CF.Web.AspNetCore.Extensions.HostBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CF.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Build a logger from configuration outside of the 
            ILogger logger;
            try
            {
                // Configuration has to come before adding logging, as it is used to configure the logger.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "[ASPNETCORE_ENVIRONMENT not set]"}.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                // Add logging as early as possible to record errors ramping up the application.
                logger = LoggerFactory.GetLogger(configuration);
            }
            catch (Exception ex)
            {
                // Ensure any errors incurred while getting configuration and logging set up are written to stdout.
                Console.WriteLine($"Error occurred building configuration:\n{ex.Message}");
                throw;
            }

            try
            {
                logger.Information("Configuring the web application.");

                var builder = WebApplication.CreateBuilder(new WebApplicationOptions
                {
                    Args = args,
                    ApplicationName = typeof(Program).Assembly.FullName,
                    ContentRootPath = Directory.GetCurrentDirectory(),
                });

                logger.Information($"Application Name: {builder.Environment.ApplicationName}");
                logger.Information($"Environment Name: {builder.Environment.EnvironmentName}");
                logger.Information($"ContentRoot Path: {builder.Environment.ContentRootPath}");
                logger.Information($"WebRootPath: {builder.Environment.WebRootPath}");

                builder.Host.UseCustomLogging();

                var startup = new Startup(builder.Configuration);

                startup.ConfigureServices(builder.Services);

                var app = builder.Build();

                startup.Configure(app, app.Services, app.Environment);

                logger.Information("running the web application.");

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Critical(ex, "Error occurred configuring and running the web application.");

                throw;
            }
            finally
            {
                // Ensure all entries are sent to the configured sinks.
                logger.CloseAndFlush();
            }
        }
    }
}
