using CF.Core.DI;
using CF.Core.Repositories;
using CF.Core.Services;
using CF.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Infrastructure.DI
{
    public class InfrastructureRegistrations: IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<IWeatherForecastRepository, WeatherForecastRepository>();
        }
    }
}
