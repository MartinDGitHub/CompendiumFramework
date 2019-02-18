using CF.Common.DI;
using CF.Web.Extensions;
using CF.WebBootstrap.DI;
using CF.WebBootstrap.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using SimpleInjector;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CF.WebBootstrap.Extensions
{
    public static class ContainerIApplicationBuilderExtensions
    {
        public static void UseCustomContainer(this IApplicationBuilder app, IHostingEnvironment env)
        {
            var container = ContainerProvider.Container.Value;

            // Wire up for MVC components.
            container.RegisterMvcControllers(app);
            container.RegisterMvcViewComponents(app);

            // Wire up with built-in DI container.
            container.AutoCrossWireAspNetComponents(app);

            // Wire up Compendium Framework assemblies.
            var registrar = new Registrar(container);
            var registrationsTypes = new DirectoryInfo(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .GetFiles("CF.*.dll") // Compendium Framework assemblies are prefixed with "CF."
                .SelectMany(file => Assembly.LoadFrom(file.FullName)
                    .GetTypes()
                    .Where(type => type.IsClass && typeof(IRegistrations).IsAssignableFrom(type)))
                .ToArray();
            registrationsTypes
                .Select(type => (IRegistrations)Activator.CreateInstance(type))
                .ToList()
                .ForEach(registrations => registrations.RegisterServices(registrar));

            // Wire up middleware components.
            // Register a middleware that scopes objects registered with a lifetime of "scoped" at the web request level.
            app.UseMiddleware<RequestScopedMiddleware>(container);

            if (env.IsDevelopment())
            {
                container.Verify();
            }
        }
    }
}
