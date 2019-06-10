using CF.Common.Dto.Messaging;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.Web.AspNetCore.Controllers;
using CF.Web.AspNetCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.Web.AspNetCore.Filters
{
    public class WebActionResultCookieMessageActionFilter : IActionFilter
    {
        private static HashSet<Type> RedirectTypes = new HashSet<Type>
        {
            typeof(LocalRedirectResult),
            typeof(RedirectResult),
            typeof(RedirectToActionResult),
            typeof(RedirectToPageResult),
            typeof(RedirectToRouteResult),
        };

        private readonly IHostingEnvironment _env;
        private readonly IScopedCookieMessageRecorder _messageRecorder;
        private readonly ILogger _logger;

        public const string CookieName = "cf-messages";

        public WebActionResultCookieMessageActionFilter(
            IHostingEnvironment env,
            IScopedCookieMessageRecorder messageRecorder, ILogger<ApiActionResultPackageActionFilter> logger)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (// Only Web controller actions automatically return a uniform result package.
                context.Controller is WebControllerBase &&
                // If another filter canceled the action, do not produce a package.
                !context.Canceled &&
                // If an exception occurred executing the action, let the global exception handler take care of it.
                context.Exception == null &&
                // Only create the message cookie if there are messages.
                this._messageRecorder.Messages.Any() &&
                // Only relay messages via cookies if a redirect is occurring.
                RedirectTypes.Contains(context.Result.GetType()))
            {
                if (string.IsNullOrWhiteSpace(this._messageRecorder.TargetUrl))
                {
                    throw new InvalidOperationException($"No target URL was set on the cookie message recorder.");
                }

                var cookieExpires = DateTime.UtcNow.Add(this._messageRecorder.ExpiresAfter);

                var cookieMessagesModel = new CookieMessagesModel
                {
                    Expires = cookieExpires,
                    TargetUrl = this._messageRecorder.TargetUrl,
                    Messages = this._messageRecorder.Messages.Select(x => new Message(x)) ?? new Message[] { },
                };

                context.HttpContext.Response.Cookies.Append(CookieName, JsonConvert.SerializeObject(cookieMessagesModel), new CookieOptions
                {
                    Expires = cookieExpires,
                    HttpOnly = false,
                });
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do nothing before the action execution.
        }
    }
}
