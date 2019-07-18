using System;
using CF.Domain.Weather;

namespace CF.Application.Services.Weather
{
    internal class WeatherStatisticsService : IWeatherStatisticsService
    {
        public TemperatureRange GetNormalTemperatureRange(DateTime date)
        {
            // Fake it until a source of truth is identified.
            if (date.Month >= 10 && date.Month < 4)
            {
                return new TemperatureRange(TemperatureScale.Celsius, -20, 11);
            }

            return new TemperatureRange(TemperatureScale.Celsius, 0, 26);
        }
    }
}
