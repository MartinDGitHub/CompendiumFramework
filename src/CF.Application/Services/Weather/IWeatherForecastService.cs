using CF.Domain.Weather;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CF.Application.Services
{
    public interface IWeatherForecastService : IService
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync();
    }
}
