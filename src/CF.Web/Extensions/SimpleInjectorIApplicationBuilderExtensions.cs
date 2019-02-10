using CF.Core.Config;
using CF.Core.DI;
using Microsoft.AspNetCore.Builder;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Web.DI;

namespace Web.Extensions
{
    public static class SimpleInjectorIApplicationBuilderExtensions
    {
        public static void InitializeContainer(this IApplicationBuilder app, Container container)
        {
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
        }
    }
}
