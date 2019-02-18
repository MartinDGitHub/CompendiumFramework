using CF.Domain.Weather;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Application.Repositories
{
    public interface IWeatherForecastRepository
    {
        IEnumerable<WeatherForecast> ReadWeatherForecasts();
    }
}
