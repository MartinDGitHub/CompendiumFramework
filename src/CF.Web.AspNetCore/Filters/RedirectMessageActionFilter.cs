﻿using CF.Common.Dto.Messaging;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.Web.AspNetCore.Controllers;
using CF.Web.AspNetCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.Web.AspNetCore.Filters
{
    public class RedirectMessageActionFilter : IActionFilter
    {
        private const string RedirectMessagesTempDataKey = "cf-redirect-messages";

        // Set an expiry for the messages to help prevent stale messages from being shown.
        // A short duration should be sufficient here, as it simply needs to last for the 
        // time it takes the response to complete, reach the browser which will redirect,
        // and the consequent request to reach this middleware again. Ostensibly, no lengthy
        // processing in the application need be accounted for.
        private static readonly TimeSpan DefaultExpiresTimeSpan = TimeSpan.FromMinutes(1);

        private readonly static IDictionary<Type, Func<IActionResult, string>> RedirectTypes = new Dictionary<Type, Func<IActionResult, string>>
        {
            { typeof(LocalRedirectResult), x => ((LocalRedirectResult)x).Url },
            { typeof(RedirectResult), x =>  ((RedirectResult)x).Url },
            { typeof(RedirectToActionResult), x =>
                {
                    var result = x as RedirectToActionResult;
                    return result.UrlHelper.Action(result.ActionName, result.ControllerName, result.RouteValues, protocol: null, host: null, result.Fragment);
                }
            },
            { typeof(RedirectToPageResult), x =>
                {
                    var result = x as RedirectToPageResult;
                    return result.UrlHelper.Page(result.PageName, result.PageHandler, result.RouteValues, protocol: null, host: null, result.Fragment);
                }
            },
            { typeof(RedirectToRouteResult), x =>
                {
                    var result = x as RedirectToRouteResult;
                    return result.UrlHelper.RouteUrl(result.RouteName, result.RouteValues, protocol: null, host: null, result.Fragment);
                }
            },
        };

        private readonly IHostingEnvironment _env;
        private readonly IScopedRedirectMessageRecorder _redirectMessageRecorder;
        private readonly ILogger _logger;

        public RedirectMessageActionFilter(
            IHostingEnvironment env, IScopedRedirectMessageRecorder redirectMessageRecorder, ILogger<ApiActionResultPackageActionFilter> logger)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._redirectMessageRecorder = redirectMessageRecorder ?? throw new ArgumentNullException(nameof(redirectMessageRecorder));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var webController = context.Controller as WebControllerBase;
            if (// Only Web controller actions automatically return a uniform result package.
                webController != null &&
                // If another filter canceled the action, do not produce a package.
                !context.Canceled &&
                // If an exception occurred executing the action, let the global exception handler take care of it.
                context.Exception == null &&
                // Check that there are messages to relay through the redirect.
                this._redirectMessageRecorder.OutboundMessages.Any())
            {
                // Only relay messages via cookies if a redirect is occurring.
                if (RedirectTypes.ContainsKey(context.Result.GetType()))
                {
                    // Ensure that only the encoded path and query are specified so that a different scheme
                    // or host will not break the detection.
                    string targetUrlPathAndQuery;
                    var redirectUrl = RedirectTypes[context.Result.GetType()](context.Result);
                    if (Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute))
                    {
                        targetUrlPathAndQuery = new Uri(redirectUrl, UriKind.Absolute).PathAndQuery;
                    }
                    else
                    {
                        targetUrlPathAndQuery = redirectUrl;
                    }

                    var refererValue = (context.HttpContext.Request.Headers?[HeaderNames.Referer] ?? StringValues.Empty);
                    var redirectMessagesModel = new RedirectMessagesModel
                    {
                        // Expire to help prevent stale messages from being shown.
                        Expires = DateTime.UtcNow.Add(DefaultExpiresTimeSpan),
                        // Bind messages to their intended target to prevent messages from being shown outside their intended context.
                        TargetPathAndQuery = targetUrlPathAndQuery,
                        // Bind messages to their intended source to help ensure messages are pulled in from their intended context.
                        ReferrerPathAndQuery = (refererValue == StringValues.Empty) ? null : new Uri(refererValue, UriKind.Absolute).PathAndQuery,
                        Messages = this._redirectMessageRecorder.OutboundMessages.Select(x => new Message(x)) ?? new Message[] { },
                    };

                    webController.TempData[RedirectMessagesTempDataKey] = JsonConvert.SerializeObject(redirectMessagesModel);
                }
                // The redirect failed and messages will be lost, so throw an exception.
                else
                {
                    throw new InvalidOperationException($"Redirect messages could not be relayed due to a redirect type of [{context.Result.GetType().FullName}] being unrecognized.");
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do nothing before the action execution.
            var webController = context.Controller as WebControllerBase;
            if (webController != null && (webController.TempData[RedirectMessagesTempDataKey] as string) != null)
            {
                var redirectMessagesModelJson = webController.TempData[RedirectMessagesTempDataKey] as string;
                try
                {
                    var redirectMessagesModel = JsonConvert.DeserializeObject<RedirectMessagesModel>(redirectMessagesModelJson);
                    if (redirectMessagesModel != null)
                    {
                        var messageRetrieveTimestamp = DateTimeOffset.UtcNow;
                        var requestPathAndQuery = context.HttpContext.Request.GetEncodedPathAndQuery();
                        var refererValue = (context.HttpContext.Request.Headers?[HeaderNames.Referer] ?? StringValues.Empty);

                        // Only record the messages if we can verify they are *most likely* the messages intended to be shown
                        // in the context of this request. There is no 100% guarantee -- cookie relay for PRG is a bit of a kludge.

                        // There may not be a referer header if the redirect failed such that the temp data cookie was not removed.
                        if (refererValue == StringValues.Empty)
                        {
                            this._logger.Error($"No referer header value was found. Message data that was discarded [{redirectMessagesModelJson}].");
                        }
                        else if (!string.Equals(new Uri(refererValue, UriKind.Absolute).PathAndQuery, redirectMessagesModel.ReferrerPathAndQuery, StringComparison.OrdinalIgnoreCase))
                        {
                            // "referer" is a typo in the HTTP specification.
                            this._logger.Error($"The actual referer header value path and query [{refererValue}] did not match the expected referrer path and query [{redirectMessagesModel.ReferrerPathAndQuery}]. Message data that was discarded [{redirectMessagesModelJson}].");
                        }
                        else if (!string.Equals(requestPathAndQuery, redirectMessagesModel.TargetPathAndQuery, StringComparison.OrdinalIgnoreCase))
                        {
                            this._logger.Error($"The actual request path and query [{requestPathAndQuery}] did not match the expected target path and query [{redirectMessagesModel.TargetPathAndQuery}]. Message data that was discarded [{redirectMessagesModelJson}].");
                        }
                        else if (redirectMessagesModel.Expires <= messageRetrieveTimestamp)
                        {
                            this._logger.Error($"The messages with a time stamp of [{redirectMessagesModel.Expires.ToString("o")}] are expired relative to when they were retrieved [{messageRetrieveTimestamp.ToString("o")}]. Message data that was discarded [{redirectMessagesModelJson}].");
                        }
                        else
                        {
                            foreach (var message in redirectMessagesModel.Messages)
                            {
                                this._redirectMessageRecorder.RecordInbound(message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"An error occurred recording inbound redirect messages for JSON [{redirectMessagesModelJson}].", ex);
                }
            }
        }
    }
}
