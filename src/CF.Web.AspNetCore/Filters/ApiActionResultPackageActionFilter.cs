using CF.Common.Correlation;
using CF.Common.Dto.ActionResults;
using CF.Common.Dto.Messaging;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.Web.AspNetCore.Attributes;
using CF.Web.AspNetCore.Controllers.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;

namespace CF.Web.AspNetCore.Filters
{
    public class ApiActionResultPackageActionFilter : IActionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly IScopedCorrelationIdProvider _scopedCorrelationGuidProvider;
        private readonly IScopedMessageRecorder _messageRecorder;
        private readonly ILogger _logger;

        public ApiActionResultPackageActionFilter(
            IHostingEnvironment env,
            IScopedCorrelationIdProvider scopedCorrelationGuidProvider,
            IScopedMessageRecorder messageRecorder, ILogger<ApiActionResultPackageActionFilter> logger)
        {
            this._env = env ?? throw new ArgumentNullException  (nameof(env));
            this._scopedCorrelationGuidProvider = scopedCorrelationGuidProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationGuidProvider));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            ControllerActionDescriptor controllerActionDescriptor;
            if (// Only API controller actions automatically return a uniform result package.
                context.Controller is ApiControllerBase &&
                // If another filter canceled the action, do not produce a package.
                !context.Canceled &&
                // If an exception occurred executing the action, let the global exception handler take care of it.
                context.Exception == null &&
                // Check for an opt out on the controller or action.
                Attribute.GetCustomAttribute(context.Controller.GetType(), typeof(ActionResultPackageOptOutAttribute)) == null &&
                (controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor) != null &&
                Attribute.GetCustomAttribute(controllerActionDescriptor.MethodInfo, typeof(ActionResultPackageOptOutAttribute)) == null)
            {
                var resultPackage = new ActionResultPackage<object>
                {
                    Success = !this._messageRecorder.HasErrors,
                    CorrelationId = this._scopedCorrelationGuidProvider.CorrelationId,
                    ValidationMessages = this._messageRecorder.Messages.Select(x => new Message(x)) ?? new Message[] { },
                };

                // If error severity messages were recorded, consider this to have been bad request. Return a 404
                // to signal the caller that the operation failed due to validation errors. Unexpected errors should
                // be thrown and handled by the global exception handler.
                int? overrideStatusCode = this._messageRecorder.HasErrors ? (int)HttpStatusCode.BadRequest : (int?)null;

                // Wrap the raw result from the action in the result package, when the action is of the sort
                // that contains a value returned from the action.
                var actionResultType = context.Result.GetType();
                if (typeof(ObjectResult).IsAssignableFrom(actionResultType))
                {
                    var objectResult = (ObjectResult)context.Result;

                    resultPackage.Data = objectResult.Value;

                    objectResult.Value = resultPackage;
                    objectResult.StatusCode = overrideStatusCode.HasValue ? overrideStatusCode.Value : objectResult.StatusCode;
                }
                else if (typeof(JsonResult).IsAssignableFrom(actionResultType))
                {
                    var jsonResult = (JsonResult)context.Result;

                    resultPackage.Data = jsonResult.Value;

                    jsonResult.Value = resultPackage;
                    jsonResult.StatusCode = overrideStatusCode.HasValue ? overrideStatusCode.Value : jsonResult.StatusCode;
                }
                else if (typeof(EmptyResult).IsAssignableFrom(actionResultType))
                {
                    var emptyResult = (EmptyResult)context.Result;

                    var objectResult = new ObjectResult(null);
                    objectResult.StatusCode = overrideStatusCode.HasValue ? overrideStatusCode.Value : (int)HttpStatusCode.OK;
                    context.Result = objectResult;
                }
                else if (typeof(ContentResult).IsAssignableFrom(actionResultType))
                {
                    var contentResult = (ContentResult)context.Result;

                    var objectResult = new ObjectResult(contentResult.Content);
                    objectResult.ContentTypes.Add(new MediaTypeHeaderValue(contentResult.ContentType));
                    contentResult.StatusCode = overrideStatusCode.HasValue ? overrideStatusCode.Value : (int)HttpStatusCode.OK;
                    context.Result = objectResult;
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do nothing before the action execution.
        }
    }
}
