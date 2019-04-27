using CF.Common.Logging;
using CF.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;

namespace CF.Web.AspNetCore.Extensions.ConfigurationRoot
{
    public static class LoggingIConfigurationRootExtensions
    {
        public static ILogger AddLogging(this IConfigurationRoot configurationRoot)
        {
            return LoggerFactory.GetLogger(configurationRoot);
        }
    }
}
