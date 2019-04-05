using CF.Domain.Weather;
using System;

namespace CF.Application.Services.Weather
{
    public interface IWeatherStatisticsService : IService
    {
        TemperatureRange GetNormalTemperatureRange(DateTime date);
    }
}
