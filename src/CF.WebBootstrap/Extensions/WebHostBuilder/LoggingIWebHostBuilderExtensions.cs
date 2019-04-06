using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace CF.WebBootstrap.Extensions.WebHostBuilder
{
    public static class LoggingIWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseCustomLogging(this IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder.UseSerilog();
        }
    }
}
