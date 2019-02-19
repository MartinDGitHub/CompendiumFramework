using CF.Common.Logging;
using CF.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;

namespace CF.WebBootstrap.Extensions
{
    public static class LoggingIConfigurationRootExtensions
    {
        public static ILogger AddLogging(this IConfigurationRoot configurationRoot)
        {
            return new LoggerFactory().GetLogger(configurationRoot);
        }
    }
}
