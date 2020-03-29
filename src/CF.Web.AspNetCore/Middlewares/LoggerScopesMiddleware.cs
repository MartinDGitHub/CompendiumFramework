using CF.Common.Correlation;
using CF.Common.Logging;
using CF.Common.Logging.Scopes;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Middlewares
{
    internal class LoggerScopesMiddleware : IMiddleware
    {
        private readonly IScopedCorrelationIdProvider _scopedCorrelationIdProvider;
        private readonly ILogger _logger;

        public LoggerScopesMiddleware(IScopedCorrelationIdProvider scopedCorrelationIdProvider, ILogger<LoggerScopesMiddleware> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._scopedCorrelationIdProvider = scopedCorrelationIdProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationIdProvider));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationGuidScopeProperty = new CorrelationIdScopeProperty(this._scopedCorrelationIdProvider.CorrelationId);
            using (this._logger.BeginScope(correlationGuidScopeProperty))
            {
                await next(context).ConfigureAwait(false);
            }
        }
    }
}
