using CF.Infrastructure.DI;
using CF.WebBootstrap.Middlewares;
using Microsoft.AspNetCore.Builder;
using SimpleInjector;

namespace CF.WebBootstrap.Extensions.ApplicationBuilder
{
    public static class MiddlewareIApplicationBuilderExtensions
    {
        public static void UseCustomMiddleware(this IApplicationBuilder app)
        {
            // Wire up middleware components.
            // Register a middleware that scopes objects registered with a lifetime of "scoped" at the web request level.
            var container = new ContainerRegistry<Container>().ContainerImpl;
            app.UseMiddleware<RequestScopedMiddleware>(container);
            app.UseMiddleware<RequestLoggingMiddleware>(container);
        }
    }
}
