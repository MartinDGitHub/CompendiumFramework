using CF.Application.Repositories;
using CF.Common.DI;
using CF.Common.Logging;
using CF.Infrastructure.Logging;
using CF.Infrastructure.Repositories;
using System;

namespace CF.Infrastructure.DI
{
    class InfrastructureRegistrations: IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<ILogger, Logger>(Lifetime.Singleton);

            registrar.Register<IWeatherForecastRepository, WeatherForecastRepository>();
        }
    }
}
