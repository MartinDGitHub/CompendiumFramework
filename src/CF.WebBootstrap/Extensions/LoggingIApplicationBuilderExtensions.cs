using CF.WebBootstrap.DI;
using CF.WebBootstrap.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using SimpleInjector;

namespace CF.WebBootstrap.Extensions
{
    public static class LoggingIApplicationBuilderExtensions
    {
        public static void UseCustomLogging(this IApplicationBuilder app)
        {
            var container = ContainerProvider.Container.Value;
            app.UseMiddleware<LoggingCorrelationMiddleware>(container);
        }
    }
}
