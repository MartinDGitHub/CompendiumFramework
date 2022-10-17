using CF.Application.Authorization.Policies.Access;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.Web.AspNetCore.Controllers;
using CF.Web.AspNetCore.Helpers;
using CF.Web.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class HomeController : WebControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IScopedMessageRecorder scopedMessageRecorder, IScopedRedirectMessageRecorder scopedRedirectMessageRecorder,
            IPartialViewHelper partialViewHelper,
            ILogger<HomeController> logger) 
            : base(scopedMessageRecorder, scopedRedirectMessageRecorder, partialViewHelper)
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
            this._logger.Information("Foo!");
            return await this.PostRedirectGetAsync(
                async () =>
                {
                    this.ScopedMessageRecorder.Record(MessageSeverity.Success, "Success!");
                    await Task.CompletedTask;
                },                
                () => this.Redirect($"{Url.Action(nameof(Test))}"),
                async () => await Task.FromResult(this.View(model)).ConfigureAwait(false)
                );
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
