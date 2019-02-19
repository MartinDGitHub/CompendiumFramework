using CF.Application.Repositories;
using CF.Common.DI;
using CF.Common.Logging;
using CF.Infrastructure.Logging;
using CF.Infrastructure.Repositories;
using System;

namespace CF.Infrastructure.DI
{
    internal class InfrastructureRegistrations: IRegistrations
    {
        public void RegisterServices(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Register<ILogger, Logger>(Lifetime.Singleton);

            container.Register<IWeatherForecastRepository, WeatherForecastRepository>();
        }
    }
}
