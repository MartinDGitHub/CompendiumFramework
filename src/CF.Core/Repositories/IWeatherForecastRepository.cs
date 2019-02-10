using CF.Core.Domain.Weather;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Core.Repositories
{
    public interface IWeatherForecastRepository
    {
        IEnumerable<WeatherForecast> ReadWeatherForecasts();
    }
}
