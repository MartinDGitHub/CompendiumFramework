using CF.Domain.Weather;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CF.Application.Repositories
{
    public interface IWeatherForecastRepository : IRepository
    {
        Task<IEnumerable<WeatherForecast>> ReadWeatherForecastsAsync();
    }
}
