using CF.Application.Authorization.Policies.Access;
using CF.Web.AspNetCore.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CF.Web.Controllers.Api
{
    public class WeatherController : ApiControllerBase
    {
        private readonly IAdminAccessPolicy _adminAccessPolicy;

        public WeatherController(IAdminAccessPolicy adminAccessPolicy)
        {
            this._adminAccessPolicy = adminAccessPolicy ?? throw new ArgumentNullException(nameof(adminAccessPolicy));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetWeatherItems()
        {
            (await this._adminAccessPolicy.AuthorizeAsync()).EnsureAuthorized();

            return await Task.FromResult(new string[] { "1 C", "20 C", "42 C" });
        }
    }
}