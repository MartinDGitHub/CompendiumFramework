using CF.Application.Authorization.Policies;
using CF.Application.Authorization.Requirements.Contexts;
using CF.Application.Repositories;
using CF.Common.Config;
using CF.Common.Exceptions;
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
        private readonly IWarmTemperaturePolicy _warmTemperaturePolicy;
        private readonly IScopedMessageRecorder _messageRecorder;
        private readonly IFooConfig _fooConfig;
        private readonly ILogger _logger;

        public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository, IWarmTemperaturePolicy warmTemperaturePolicy, 
            IScopedMessageRecorder messageRecorder, IFooConfig fooConfig, ILogger<WeatherForecastService> logger)
        {
            this._weatherForecastRepository = weatherForecastRepository ?? throw new ArgumentNullException(nameof(weatherForecastRepository));
            this._warmTemperaturePolicy = warmTemperaturePolicy ?? throw new ArgumentNullException(nameof(warmTemperaturePolicy));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
            this._fooConfig = fooConfig ?? throw new ArgumentNullException(nameof(fooConfig));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
        {
            var policyResult = await this._warmTemperaturePolicy.AuthorizeAsync(new TemperatureRequirementContext(15));
            if (!policyResult)
            {
                throw new AuthorizationPolicyException<IWarmTemperaturePolicy>(this._warmTemperaturePolicy, policyResult);
            }

            this._logger.Information($"In [{nameof(WeatherForecastService)}].");
            this._messageRecorder.Record(MessageSeverity.Info, "Services checking in!");

            return await this._weatherForecastRepository.ReadWeatherForecastsAsync();
        }
    }
}
