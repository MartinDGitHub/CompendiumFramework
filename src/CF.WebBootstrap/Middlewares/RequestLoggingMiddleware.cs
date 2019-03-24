using CF.Common.Logging;
using CF.Common.Logging.Scopes;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace CF.WebBootstrap.Middlewares
{
    internal class RequestLoggingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationIdScopeProperty = new CorrelationIdScopeProperty();
            using (this._logger.BeginScope(correlationIdScopeProperty))
            {
                await next(context);
            }
        }
    }
}
