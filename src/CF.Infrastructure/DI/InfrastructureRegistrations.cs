using CF.Application.DI;
using CF.Application.Repositories;
using CF.Application.Services;
using CF.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Infrastructure.DI
{
    class InfrastructureRegistrations: IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<IWeatherForecastRepository, WeatherForecastRepository>();
        }
    }
}
