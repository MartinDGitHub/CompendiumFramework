using CF.Core.Domain.Weather;
using CF.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.Infrastructure.Repositories
{
    class WeatherForecastRepository : IWeatherForecastRepository
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        public IEnumerable<WeatherForecast> ReadWeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
    }
}
