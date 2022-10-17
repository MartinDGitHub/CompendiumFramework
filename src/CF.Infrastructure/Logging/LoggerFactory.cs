using Microsoft.Extensions.Configuration;
using Serilog;
using System.Threading;

namespace CF.Infrastructure.Logging
{
    /// <summary>
    /// Provides a way to manually get a logger when one cannot be injected via DI.
    /// </summary>
    public static class LoggerFactory
    {
        private static Serilog.Core.Logger _logger;

        public static Common.Logging.ILogger GetLogger(IConfigurationRoot configurationRoot)
        {
            // Perform a thread-safe swap of the logger if one has already been established.
            // Loggers should be stateless and this permits reconfiguration.
            var oldLogger = Interlocked.Exchange(ref _logger,
                new LoggerConfiguration()
                .ReadFrom.Configuration(configurationRoot)
                .CreateLogger());

            // Apply the new logger.
            Log.Logger = _logger;

            // If the logger was replaced, ensure the old logger writes out
            // any remaining entries by explicitly disposing of it.
            if (oldLogger != null)
            {
                oldLogger.Dispose();
            }

            return new Logger();
        }
    }
}
