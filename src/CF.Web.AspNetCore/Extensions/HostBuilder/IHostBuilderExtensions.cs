using Microsoft.Extensions.Hosting;
using Serilog;

namespace CF.Web.AspNetCore.Extensions.HostBuilder
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder UseCustomLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog();
        }
    }
}
