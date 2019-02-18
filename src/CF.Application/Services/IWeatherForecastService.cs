using CF.Domain.Weather;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CF.Application.Services
{
    public interface IWeatherForecastService
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync();
    }
}
