using CF.Application.Services;
using CF.Common.Authorization.Policies;
using CF.Common.Config;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.Domain.Weather;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SampleDataController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly IScopedMessageRecorder _messageRecorder;
        private readonly ILogger _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAdminAccessPolicy _adminAccessPolicy;

        public SampleDataController(IAdminAccessPolicy adminAccessPolicy, IHttpContextAccessor httpContextAccessor, IWeatherForecastService weatherForecastService, IScopedMessageRecorder messageRecorder, ILogger<SampleDataController> logger)
        {
            this._weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._httpContextAccessor = httpContextAccessor;
            this._adminAccessPolicy = adminAccessPolicy;
        }

        [HttpGet("[action]")]
        [Authorize(Policy = nameof(IAdminAccessPolicy))]
        public async Task<IEnumerable<WeatherForecast>> WeatherForecasts()
        {
            this._logger.Information($"In [{nameof(SampleDataController)}].");

            return await this._weatherForecastService.GetWeatherForecastsAsync();

        }
    }
}
