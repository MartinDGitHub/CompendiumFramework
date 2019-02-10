using CF.Core.Config;
using System;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Core.Services
{
    internal class WeatherForecastService : IWeatherForecastService
    {
        private readonly IFooConfig _fooConfig;

        public WeatherForecastService(IFooConfig fooConfig)
        {
            this._fooConfig = fooConfig ?? throw new ArgumentNullException(nameof(fooConfig));
        }

        public bool Test => true;
    }
}
