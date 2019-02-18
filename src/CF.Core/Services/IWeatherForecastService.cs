using CF.Domain.Weather;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Application.Services
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> GetWeatherForecasts();
    }
}
