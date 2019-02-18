using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Application.Config;
using CF.Domain.Weather;
using CF.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SampleDataController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;
        private IFooConfig _fooConfig;

        public SampleDataController(IWeatherForecastService weatherForecastService, IFooConfig fooConfig)
        {
            this._weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
            this._fooConfig = fooConfig ?? throw new ArgumentNullException(nameof(fooConfig));
        }

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            return this._weatherForecastService.GetWeatherForecasts();
        }
    }
}
