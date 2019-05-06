using CF.Common.Correlation;
using CF.Common.Dto.ActionResults;
using CF.Common.Dto.Messaging;
using CF.Common.Exceptions;
using CF.Common.Logging;
using CF.Common.Messaging;
using CF.Web.AspNetCore.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Middlewares
{
    internal class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        private readonly IHostingEnvironment _env;
        private readonly IScopedCorrelationIdProvider _scopedCorrelationGuidProvider;
        private readonly IScopedMessageRecorder _messageRecorder;
        private readonly ILogger _logger;

        public GlobalExceptionHandlerMiddleware(IHostingEnvironment env, IScopedCorrelationIdProvider scopedCorrelationGuidProvider, IScopedMessageRecorder messageRecorder, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._scopedCorrelationGuidProvider = scopedCorrelationGuidProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationGuidProvider));
            this._messageRecorder = messageRecorder ?? throw new ArgumentNullException(nameof(messageRecorder));
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
                var rethrow = false;
                try
                {
                    // Always reset the response so that it is a clean replacement.
                    context.Response.Clear();

                    // Resolve the correlation ID, using the explicit correlation ID provided by the exception over
                    // the implicit, ambient one, when the explicit ID is available.
                    string correlationId;
                    var correlatedException = ex as ICorrelatedException;
                    if (correlatedException != null && !string.IsNullOrWhiteSpace(correlatedException.CorrelationId))
                    {
                        correlationId = correlatedException.CorrelationId;
                    }
                    else
                    {
                        // Use the ambient correlation ID for the request if an explicit ID wasn't found.
                        correlationId  = this._scopedCorrelationGuidProvider.CorrelationId;
                    }

                    if (context.Request.IsAjaxRequest() || context.Request.IsApiRequest())
                    {
                        context.Response.ContentType = "application/json";

                        var resultPackage = new ActionResultPackage<object>
                        {
                            Success = false,
                            CorrelationId = correlationId,
                        };

                        if (ex is ValidationMessagesException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                            var consumerFriendlyMessagesException = ex as IValidationMessagesException;
                            resultPackage.ValidationMessages = consumerFriendlyMessagesException?.ValidationMessages.Select(x => new Message(x)) ?? new Message[] { };
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            resultPackage.UnexpectedErrorMessage =
                                new Message { Severity = MessageSeverity.Error, Text = $"An unexpected error has occurred. Please contact your system administrator with the following code {resultPackage.CorrelationId}." };

                            this._logger.Error(ex, $"An unexpected error occurred. Correlation GUID [{resultPackage.CorrelationId}].");
                        }

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(resultPackage));
                    }
                    else
                    {
                        // If not in a development environment redirect to the error page. Otherwise, rethrow to show the full error
                        // details on the developer exception page.
                        if (this._env.IsDevelopment())
                        {
                            rethrow = true;
                        }
                        else
                        {
                            // Clear and record a generic, unexpected error message that the error page can display.
                            this._messageRecorder.Clear();
                            this._messageRecorder.Record(new Message { Severity = MessageSeverity.Error, Text = $"An unexpected error has occurred. Please contact your system administrator with the following code {correlationId}." });

                            this._logger.Error(ex, $"An unexpected error occurred. Correlation GUID [{correlationId}].");

                            if (context.Response.HasStarted)
                            {
                                // If the reponse already started, all we can do is redirect to the error page with the 
                                // correlation ID.
                                context.Response.Redirect($"/Home/Error?correlationId={correlationId}");
                            }
                            else
                            {
                                // If the response hasn't started yet, re-execute to the error page where the
                                // messages can be extracted and displayed. Disable any caching that would
                                // interfere with displaying the new error message.
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.OnStarting(state =>
                                {
                                    var response = (HttpResponse)state;
                                    response.Headers[HeaderNames.CacheControl] = "no-cache";
                                    response.Headers[HeaderNames.Pragma] = "no-cache";
                                    response.Headers[HeaderNames.Expires] = "-1";
                                    response.Headers.Remove(HeaderNames.ETag);
                                    return Task.CompletedTask;
                                }, context.Response);
                                context.Request.Path = "/Home/Error";

                                // Re-execute to the new path.
                                await next(context);
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {
                    // An exception was thrown handling the exception. Log it and return an error status with no information.
                    this._logger.Error(ex, $"The following unexpected error occurred during global exception handling:\n{ex2}\n\nThe original exception is included below.");

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

                if (rethrow)
                {
                    throw;
                }
            }
        }
    }
}
