using CF.Core.Services;
using SimpleInjector;
using SimpleInjector.Packaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Core.DI
{
    public class CorePackage : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.Register<IWeatherForecastService, WeatherForecastService>();
        }
    }
}
