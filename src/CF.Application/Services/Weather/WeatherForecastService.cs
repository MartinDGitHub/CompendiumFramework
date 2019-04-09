using CF.Application.Authorization.Policies.Weather;
using CF.Application.Authorization.Requirements.Contexts;
using CF.Application.Repositories;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.Domain.Weather;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CF.Application.Services
{
    internal class WeatherForecastService : IWeatherForecastService
    {
        private readonly IWeatherForecastRepository _weatherForecastRepository;
        private readonly INormalTemperaturePolicy _warmTemperaturePolicy;
        private readonly IScopedMessageRecorder _messageRecorder;
        private readonly ILogger _logger;

        public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository, INormalTemperaturePolicy warmTemperaturePolicy, 
            IScopedMessageRecorder messageRecorder, ILogger<WeatherForecastService> logger)
        {
            this._weatherForecastRepository = weatherForecastRepository ?? throw new ArgumentNullException(nameof(weatherForecastRepository));
            this._warmTemperaturePolicy = warmTemperaturePolicy ?? throw new ArgumentNullException(nameof(warmTemperaturePolicy));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
        {
            (await this._warmTemperaturePolicy.AuthorizeAsync(new TemperatureRequirementContext(new Temperature(10)))).EnsureAuthorized();

            this._logger.Information($"In [{nameof(WeatherForecastService)}].");
            this._messageRecorder.Record(MessageSeverity.Info, "Services checking in!");

            return await this._weatherForecastRepository.ReadWeatherForecastsAsync();
        }
    }
}
