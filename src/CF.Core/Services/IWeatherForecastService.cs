using CF.Core.Domain.Weather;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Core.Services
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> GetWeatherForecasts();
    }
}
