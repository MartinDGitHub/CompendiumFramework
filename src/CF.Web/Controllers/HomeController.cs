using CF.Common.Authorization.Policies;
using CF.Common.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        private IFooConfig _fooConfig;

        public HomeController(ILogger<HomeController> logger, IFooConfig fooConfig)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._fooConfig = fooConfig ?? throw new ArgumentNullException(nameof(fooConfig));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = nameof(IUserAccessPolicy))]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
