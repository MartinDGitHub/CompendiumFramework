using CF.Domain.Weather;
using CF.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using CF.Common.Messaging;
using System.Threading.Tasks;
using CF.Common.Config;
using CF.Common.Logging;

namespace CF.Application.Services
{
    internal class WeatherForecastService : IWeatherForecastService
    {
        private readonly IWeatherForecastRepository _weatherForecastRepository;
        private readonly IScopedMessageRecorder _messageRecorder;
        private readonly IFooConfig _fooConfig;
        private readonly ILogger _logger;

        public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository, IScopedMessageRecorder messageRecorder, IFooConfig fooConfig, ILogger logger)
        {
            this._weatherForecastRepository = weatherForecastRepository ?? throw new ArgumentNullException(nameof(weatherForecastRepository));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
            this._fooConfig = fooConfig ?? throw new ArgumentNullException(nameof(fooConfig));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
        {
            this._logger.Information($"In [{nameof(WeatherForecastService)}].");
            this._messageRecorder.Record(MessageSeverity.Info, "Services checking in!");

            return await this._weatherForecastRepository.ReadWeatherForecastsAsync();
        }
    }
}
