using CF.Common.Correlation;
using CF.Common.Dto.ActionResults;
using CF.Common.Dto.Messages;
using CF.Common.Exceptions;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.WebBootstrap.Extensions;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IHostingEnvironment _env;
        private readonly IScopedCorrelationIdProvider _scopedCorrelationGuidProvider;
        private readonly ILogger _logger;

        public GlobalExceptionHandlerMiddleware(IHostingEnvironment env, IScopedCorrelationIdProvider scopedCorrelationGuidProvider, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._scopedCorrelationGuidProvider = scopedCorrelationGuidProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationGuidProvider));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

                        // Use the explicit correlation ID if provided.
                        var correlatedException = ex as ICorrelatedException;
                        if (correlatedException != null && !string.IsNullOrWhiteSpace(correlatedException.CorrelationId))
                        {
                            resultPackage.CorrelationId = correlatedException.CorrelationId;
                        }
                        else
                        {
                            // Use the ambient correlation ID for the request if an explicit ID wasn't found.
                            resultPackage.CorrelationId = this._scopedCorrelationGuidProvider.CorrelationId;
                        }

                        if (ex is ValidationMessagesException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                            var consumerFriendlyMessagesException = ex as IValidationMessagesException;
                            resultPackage.ValidationMessages = consumerFriendlyMessagesException?.ValidationMessages
                                .Select(x => new Message { Timestamp = x.Timestamp, Severity = x.Severity, Text = x.Text }) ?? new Message[] { };
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            resultPackage.ValidationMessages = new Message[] 
                            {
                                new Message { Severity = MessageSeverity.Error, Text = $"An unexpected error has occurred. Please contact your system administrator with the following code {resultPackage.CorrelationId}." }
                            };

                            this._logger.Error(ex, $"An unexpected error occurred. Correlation GUID [{resultPackage.CorrelationId}].");
                        }

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(resultPackage));
                    }
                    else
                    {
                        // If not in a development environment redirect to the error page. Otherwise, rethrow to show the full error
                        // details.
                        if (this._env.IsDevelopment())
                        {
                            throw ex;
                        }

                        // Redirect ordinary browser requests to the error page.
                        context.Request.Path = "/Error";
                    }
                }
                catch (Exception ex2)
                {
                    // An exception was thrown handling the exception. Log it and return an error status with no information.
                    this._logger.Error(ex, $"The following unexpected error occurred during global exception handling:\n{ex2}\n\nThe original exception is included below.");

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
        }
    }
}
