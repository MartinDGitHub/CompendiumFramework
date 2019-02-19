using CF.WebBootstrap.DI;
using CF.WebBootstrap.Middlewares;
using Microsoft.AspNetCore.Builder;
using SimpleInjector;

namespace CF.WebBootstrap.Extensions
{
    public static class LoggingIApplicationBuilderExtensions
    {
        public static void UseCustomLogging(this IApplicationBuilder app)
        {
            var container = ContainerProvider.Container.Value;
            app.UseMiddleware<RequestLoggingMiddleware>(container);
        }
    }
}
