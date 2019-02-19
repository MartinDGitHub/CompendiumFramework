using CF.Infrastructure.DI;
using CF.WebBootstrap.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using SimpleInjector;

namespace CF.WebBootstrap.Extensions
{
    public static class ContainerIApplicationBuilderExtensions
    {
        public static void UseCustomContainer(this IApplicationBuilder app, IHostingEnvironment env)
        {
            var containerRegistry = new ContainerRegistry<Container>();

            containerRegistry.ConfigureContainer(container =>
            {
                // Wire up MVC components.
                container.RegisterMvcControllers(app);
                container.RegisterMvcViewComponents(app);

                // Wire up with built-in DI container.
                container.AutoCrossWireAspNetComponents(app);
            });

            // Wire up middleware components.
            // Register a middleware that scopes objects registered with a lifetime of "scoped" at the web request level.
            app.UseMiddleware<RequestScopedMiddleware>(containerRegistry.ContainerImpl);

            if (env.IsDevelopment())
            {
                containerRegistry.ContainerImpl.Verify();
            }
        }
    }
}
