using CF.Common.Correlation;
using CF.Common.Dto.ActionResults;
using CF.Common.Dto.Messaging;
using CF.Common.Messaging;
using CF.Web.AspNetCore.Attributes;
using CF.Web.AspNetCore.Controllers.Api;
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
        private readonly IScopedCorrelationIdProvider _scopedCorrelationGuidProvider;
        private readonly IScopedMessageRecorder _messageRecorder;

        public ApiActionResultPackageActionFilter(
            IScopedCorrelationIdProvider scopedCorrelationGuidProvider,
            IScopedMessageRecorder messageRecorder)
        {
            this._scopedCorrelationGuidProvider = scopedCorrelationGuidProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationGuidProvider));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (// Only API controller actions automatically return a uniform result package.
                context.Controller is ApiControllerBase &&
                // If another filter canceled the action, do not produce a package.
                !context.Canceled &&
                // If an exception occurred executing the action, let the global exception handler take care of it.
                context.Exception == null &&
                // Check for an opt out on the controller or action.
                Attribute.GetCustomAttribute(context.Controller.GetType(), typeof(ActionResultPackageOptOutAttribute)) == null &&
                context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor &&
                Attribute.GetCustomAttribute(controllerActionDescriptor.MethodInfo, typeof(ActionResultPackageOptOutAttribute)) == null)
            {
                var resultPackage = new ActionResultPackage<object>
                {
                    Success = !this._messageRecorder.HasErrors,
                    CorrelationId = this._scopedCorrelationGuidProvider.CorrelationId,
                    Messages = this._messageRecorder.Messages.Select(x => new Message(x)) ?? Array.Empty<Message>(),
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

                    resultPackage.Result = objectResult.Value;

                    objectResult.Value = resultPackage;
                    objectResult.StatusCode = overrideStatusCode ?? objectResult.StatusCode;
                }
                else if (typeof(JsonResult).IsAssignableFrom(actionResultType))
                {
                    var jsonResult = (JsonResult)context.Result;

                    resultPackage.Result = jsonResult.Value;

                    jsonResult.Value = resultPackage;
                    jsonResult.StatusCode = overrideStatusCode ?? jsonResult.StatusCode;
                }
                else if (typeof(EmptyResult).IsAssignableFrom(actionResultType))
                {
                    var emptyResult = (EmptyResult)context.Result;

                    var objectResult = new ObjectResult(null)
                    {
                        StatusCode = overrideStatusCode ?? (int)HttpStatusCode.OK,
                    };
                    context.Result = objectResult;
                }
                else if (typeof(ContentResult).IsAssignableFrom(actionResultType))
                {
                    var contentResult = (ContentResult)context.Result;

                    var objectResult = new ObjectResult(contentResult.Content);
                    objectResult.ContentTypes.Add(new MediaTypeHeaderValue(contentResult.ContentType));
                    contentResult.StatusCode = overrideStatusCode ?? (int)HttpStatusCode.OK;
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
