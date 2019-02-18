using CF.WebBootstrap.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CF.WebBootstrap.Extensions
{
    public static class LoggingIConfigurationRootExtensions
    {
        public static Common.Logging.ILogger AddLogging(this IConfigurationRoot configurationRoot)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurationRoot)
                .CreateLogger();

            return new Logger();
        }
    }
}
