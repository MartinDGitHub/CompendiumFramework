using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Application.Config;
using CF.Domain.Weather;
using CF.Application.Services;
using Microsoft.AspNetCore.Mvc;
using CF.Common.Messaging;

namespace Web.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SampleDataController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly IScopedMessageRecorder _messageRecorder;
        private IFooConfig _fooConfig;

        public SampleDataController(IWeatherForecastService weatherForecastService, IScopedMessageRecorder messageRecorder, IFooConfig fooConfig)
        {
            this._weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
            this._fooConfig = fooConfig ?? throw new ArgumentNullException(nameof(fooConfig));
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<WeatherForecast>> WeatherForecasts()
        {
            var forecasts = await this._weatherForecastService.GetWeatherForecastsAsync();

            return forecasts;
        }
    }
}
