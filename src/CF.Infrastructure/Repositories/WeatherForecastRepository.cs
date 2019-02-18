using CF.Domain.Weather;
using CF.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.Common.Messaging;
using System.Threading.Tasks;

namespace CF.Infrastructure.Repositories
{
    class WeatherForecastRepository : IWeatherForecastRepository
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IScopedMessageRecorder _messageRecorder;

        public WeatherForecastRepository(IScopedMessageRecorder messageRecorder)
        {
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
        }

        public async Task<IEnumerable<WeatherForecast>> ReadWeatherForecastsAsync()
        {
            this._messageRecorder.Record(MessageSeverity.Info, "Repository checking in!");

            var rng = new Random();
            return await Task.Run(() => Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }));
        }
    }
}
