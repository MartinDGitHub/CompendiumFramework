using CF.Infrastructure.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using SimpleInjector;

namespace CF.WebBootstrap.Extensions.ApplicationBuilder
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

            if (env.IsDevelopment())
            {
                containerRegistry.ContainerImpl.Verify();
            }
        }
    }
}
