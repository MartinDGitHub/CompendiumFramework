using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CF.Core.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        private IFooConfig _fooConfig;

        public HomeController(ILogger<HomeController> logger, IFooConfig fooConfig) =>  (
            this._logger, 
            this._fooConfig) = (
            logger ?? throw new ArgumentNullException(nameof(logger)),
            fooConfig ?? throw new ArgumentNullException(nameof(logger)));

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
