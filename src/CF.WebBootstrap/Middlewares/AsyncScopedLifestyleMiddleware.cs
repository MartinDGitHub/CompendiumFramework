using Microsoft.AspNetCore.Http;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Threading.Tasks;

namespace CF.WebBootstrap.Middlewares
{
    internal class AsyncScopedLifestyleMiddleware : IMiddleware
    {
        private Container _container;

        public AsyncScopedLifestyleMiddleware(Container container)
        {
            this._container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (AsyncScopedLifestyle.BeginScope(this._container))
            {
                await next(context);
            }
        }
    }
}
