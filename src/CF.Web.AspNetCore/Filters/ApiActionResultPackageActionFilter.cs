using CF.Infrastructure.DI;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CF.Web.AspNetCore.Filters
{
    public class ApiActionResultPackageActionFilter : IActionFilter
    {
        private readonly IApiActionResultPackageActionFilterHandler _handler;

        public ApiActionResultPackageActionFilter(IServiceLocatorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            // Unfortunately, we cannot directly inject the dependencies here when using a third-party
            // container. However, we can inject the container which we've registered directly with
            // the .NET DI container. Therefore, we resort to the service-locator (anti-)pattern in
            // this edge case to leverage DI and proxy the calls through the resolved handler.
            this._handler = container.GetInstance<IApiActionResultPackageActionFilterHandler>() ?? throw new InvalidOperationException($"Could not resolve a handler of type [{typeof(IApiActionResultPackageActionFilterHandler).FullName}].");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            this._handler.OnActionExecuted(context);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do nothing on executing.
        }
    }
}
