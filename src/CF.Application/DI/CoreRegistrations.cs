using CF.Application.Services;
using CF.Common.DI;
using System;

namespace CF.Application.DI
{
    class CoreRegistrations : IRegistrations
    {
        public void RegisterServices(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Register<IWeatherForecastService, WeatherForecastService>();
        }
    }
}
