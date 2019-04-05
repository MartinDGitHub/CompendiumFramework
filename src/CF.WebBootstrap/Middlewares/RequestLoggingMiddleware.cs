using CF.Common.Correlation;
using CF.Common.Logging;
using CF.Common.Logging.Scopes;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CF.WebBootstrap.Middlewares
{
    internal class RequestLoggingMiddleware : IMiddleware
    {
        private IScopedCorrelationIdProvider _scopedCorrelationIdProvider;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(IScopedCorrelationIdProvider scopedCorrelationIdProvider, ILogger<RequestLoggingMiddleware> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._scopedCorrelationIdProvider = scopedCorrelationIdProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationIdProvider));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationIdScopeProperty = new CorrelationGuidScopeProperty(this._scopedCorrelationIdProvider.CorrelationGuid);
            using (this._logger.BeginScope(correlationIdScopeProperty))
            {
                await next(context);
            }
        }
    }
}
