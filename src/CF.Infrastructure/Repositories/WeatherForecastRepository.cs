using CF.Domain.Weather;
using CF.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.Common.Messaging;
using System.Threading.Tasks;
using CF.Common.Logging;

namespace CF.Infrastructure.Repositories
{
    class WeatherForecastRepository : IWeatherForecastRepository
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IScopedMessageRecorder _messageRecorder;
        private readonly ILogger _logger;

        public WeatherForecastRepository(IScopedMessageRecorder messageRecorder, ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
        }

        public async Task<IEnumerable<WeatherForecast>> ReadWeatherForecastsAsync()
        {
            this._logger.Information($"In [{nameof(WeatherForecastRepository)}].");
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
