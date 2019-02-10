using CF.Core.Config;
using CF.Core.Domain.Weather;
using CF.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Core.Services
{
    internal class WeatherForecastService : IWeatherForecastService
    {
        private readonly IFooConfig _fooConfig;
        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository, IFooConfig fooConfig)
        {
            this._fooConfig = fooConfig ?? throw new ArgumentNullException(nameof(fooConfig));
            this._weatherForecastRepository = weatherForecastRepository ?? throw new ArgumentNullException(nameof(weatherForecastRepository));
        }

        public IEnumerable<WeatherForecast> GetWeatherForecasts()
        {
            return this._weatherForecastRepository.ReadWeatherForecasts();
        }
    }
}
