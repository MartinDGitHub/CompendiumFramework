using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Core.Domain.Weather;
using CF.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SampleDataController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;

        public SampleDataController(IWeatherForecastService weatherForecastService)
        {
            this._weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
        }

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            return this._weatherForecastService.GetWeatherForecasts();
        }
    }
}
