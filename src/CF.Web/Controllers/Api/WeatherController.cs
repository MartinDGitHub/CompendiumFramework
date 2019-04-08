using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CF.Web.Controllers.Api
{
    public class WeatherController : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetWeatherItems()
        {
            return new string[] { "1 C", "20 C", "42 C" };
        }
    }
}