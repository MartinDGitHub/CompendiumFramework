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

            // First, register middleware that creates a request scope for subsequent middlewares that depend on request-scoped instances.
            app.UseMiddleware<AsyncScopedLifestyleMiddleware>(container);

            // Second, register logging enrichment middleware for subsequent middlewares that log.
            app.UseMiddleware<LoggerScopesMiddleware>(container);

            // Third, globally handle any exceptions raised by subsequent middlewares.
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>(container);
        }
    }
}
