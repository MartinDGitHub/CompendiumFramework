using CF.Application.Config;
using CF.Domain.Weather;
using CF.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using CF.Common.Messaging;
using System.Threading.Tasks;

namespace CF.Application.Services
{
    internal class WeatherForecastService : IWeatherForecastService
    {
        private readonly IWeatherForecastRepository _weatherForecastRepository;
        private readonly IScopedMessageRecorder _messageRecorder;
        private readonly IFooConfig _fooConfig;

        public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository, IScopedMessageRecorder messageRecorder, IFooConfig fooConfig)
        {
            this._weatherForecastRepository = weatherForecastRepository ?? throw new ArgumentNullException(nameof(weatherForecastRepository));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
            this._fooConfig = fooConfig ?? throw new ArgumentNullException(nameof(fooConfig));
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
        {
            this._messageRecorder.Record(MessageSeverity.Info, "Services checking in!");

            return await this._weatherForecastRepository.ReadWeatherForecastsAsync();
        }
    }
}
