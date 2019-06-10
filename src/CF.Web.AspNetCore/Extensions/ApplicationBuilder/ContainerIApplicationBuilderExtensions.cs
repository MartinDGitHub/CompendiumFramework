using CF.Infrastructure.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace CF.Web.AspNetCore.Extensions.ApplicationBuilder
{
    public static class ContainerIApplicationBuilderExtensions
    {
        public static void UseCustomContainer(this IApplicationBuilder app, IHostingEnvironment env)
        {
            /*
            var containerRegistry = new ContainerRegistry<Container>();

            containerRegistry.ConfigureContainer(container =>
            {
                // Wire up MVC components.
                container.RegisterMvcControllers(app);
                container.RegisterMvcViewComponents(app);

                // Have Simple Injector fall back to resolving using the built-in ASP.NET Core container.
                // container.AutoCrossWireAspNetComponents(app);
            });

            if (env.IsDevelopment())
            {
                containerRegistry.ContainerImpl.Verify();
            }
            */

            if (env.IsDevelopment())
            {
                new Container().Verify();
            }
        }
    }
}
