using CF.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Application.DI
{
    class CoreRegistrations: IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<IWeatherForecastService, WeatherForecastService>();
        }
    }
}
