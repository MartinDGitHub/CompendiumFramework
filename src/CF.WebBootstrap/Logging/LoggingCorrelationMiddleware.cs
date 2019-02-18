using CF.Common.Logging.Scopes;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CF.WebBootstrap.Logging
{
    internal class LoggingCorrelationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationIdScopeProperty = new CorrelationIdScopeProperty();
            using (LogContext.PushProperty(correlationIdScopeProperty.Name, correlationIdScopeProperty.Value))
            {
                await next(context);
            }
        }
    }
}
