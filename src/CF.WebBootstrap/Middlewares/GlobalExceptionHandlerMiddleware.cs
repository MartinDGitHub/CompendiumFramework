using CF.Common.Correlation;
using CF.Common.Dto.ActionResults;
using CF.Common.Dto.Messages;
using CF.Common.Exceptions;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.WebBootstrap.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CF.WebBootstrap.Middlewares
{
    internal class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        private readonly IScopedCorrelationGuidProvider _scopedCorrelationGuidProvider;
        private readonly ILogger _logger;

        public GlobalExceptionHandlerMiddleware(IScopedCorrelationGuidProvider scopedCorrelationGuidProvider, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._scopedCorrelationGuidProvider = scopedCorrelationGuidProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationGuidProvider));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                try
                {
                    context.Response.Clear();

                    if (context.Request.IsAjaxRequest() || context.Request.IsApiRequest())
                    {
                        context.Response.ContentType = "application/json";

                        var resultPackage = new ActionResultPackage<object>();

                        resultPackage.Success = false;

                        // Use the explicit correlation GUID if provided.
                        var correlatedException = ex as ICorrelatedException;
                        if (correlatedException != null && correlatedException.CorrelationGuid.HasValue)
                        {
                            resultPackage.CorrelationGuid = correlatedException.CorrelationGuid.Value;
                        }
                        else
                        {
                            // Use the ambient correlation GUID for the request if an explicit GUID wasn't found.
                            resultPackage.CorrelationGuid = this._scopedCorrelationGuidProvider.CorrelationGuid;
                        }

                        if (ex is ValidationFailedException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                            var consumerFriendlyMessagesException = ex as IConsumerFriendlyMessagesException;
                            resultPackage.ConsumerFriendlyMessages = consumerFriendlyMessagesException?.ConsumerFriendlyMessages
                                .Select(x => new Message { Timestamp = x.Timestamp, Severity = x.Severity, Text = x.Text }) ?? new Message[] { };
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            resultPackage.ConsumerFriendlyMessages = new Message[] 
                            {
                                new Message { Severity = MessageSeverity.Error, Text = $"An unexpected error has occurred. Please contact your system administrator with the following code [{resultPackage.CorrelationGuid}]." }
                            };

                            this._logger.Error(ex, $"An unexpected error occurred. Correlation GUID [{resultPackage.CorrelationGuid}].");
                        }

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(resultPackage));
                    }
                    else
                    {
                        // Redirect ordinary browser requests to the error page.
                        context.Request.Path = "/Error";
                    }
                }
                catch (Exception ex2)
                {
                    // An exception was thrown handling the exception. Log it and return an error status with no information.
                    this._logger.Error(ex, $"An unexpected error occurred during global exception handling!");

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
        }
    }
}
