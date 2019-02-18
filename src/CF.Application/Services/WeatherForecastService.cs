using CF.Application.Config;
using CF.Domain.Weather;
using CF.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Application.Services
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
