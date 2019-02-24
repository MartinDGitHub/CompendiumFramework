using CF.Application.Repositories;
using CF.Common.Config;
using CF.Common.DI;
using CF.Common.Logging;
using CF.Infrastructure.Config;
using CF.Infrastructure.Logging;
using CF.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.Infrastructure.DI
{
    internal class InfrastructureRegistrations: IRegistrations
    {
        public void RegisterServices(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            // Only one logger should be registered per application.
            container.Register<ILogger, Logger>(Lifetime.Singleton);

            // Register all configuration interfaces to the concrete types that implement them.
            // Find all of the configuration interfaces that live in the designated namespace.
            var configInterfaceTypes = new HashSet<Type>(
                from type in typeof(IConfig).Assembly.GetExportedTypes()
                where
                type.IsInterface &&
                type.Namespace.StartsWith(typeof(IConfig).Namespace) &&
                type != typeof(IConfig) &&
                type.GetInterfaces().Any(x => x == typeof(IConfig))
                select type);
            // Find implementations for the configuration interfaces that live in the designated namespace.
            var configImplementationTypes =
                from type in typeof(RootConfig).Assembly.GetTypes()
                where
                type.IsClass &&
                type.Namespace.StartsWith(typeof(RootConfig).Namespace) &&
                configInterfaceTypes.Overlaps(type.GetInterfaces())
                select type;
            // Perform the registrations.
            foreach (var configImplementationType in configImplementationTypes)
            {
                // Ensure that there is a positive identification of a single interface.
                var configInterfaceType = configInterfaceTypes.Intersect(configImplementationType.GetInterfaces()).Single();
                // Resolve per request to ensure that any configuration changes are discovered.
                container.Register(configInterfaceType, configImplementationType, Lifetime.Scoped);
            }

            container.Register<IWeatherForecastRepository, WeatherForecastRepository>();
        }
    }
}
