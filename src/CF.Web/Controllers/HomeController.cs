using CF.Application.Authorization.Policies.Access;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.Web.AspNetCore.Controllers;
using CF.Web.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class HomeController : WebControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IScopedMessageRecorder scopedMessageRecorder, IScopedRedirectMessageRecorder scopedRedirectMessageRecorder, 
            ILogger<HomeController> logger) 
            : base(scopedMessageRecorder, scopedRedirectMessageRecorder)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Authorize(Policy = nameof(IUserAccessPolicy))]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return await Task.FromResult(View(new TestViewModel())).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Test(TestViewModel model)
        {
            return await PostRedirectGetAsync(
                model, 
                () => {
                    this.ScopedMessageRecorder.Record(MessageSeverity.Success, "Success!");
                    return Task.CompletedTask;
                    }, 
                new Uri(Url.Action(nameof(Test), new { testId = 42, testValue = @"\&/&?=" })))
                .ConfigureAwait(false);
        }

        [HttpGet]
        public IActionResult Error(string correlationId)
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            ViewData["CorrelationId"] = correlationId;
            return View();
        }
    }
}
