using CF.Infrastructure.DI;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CF.Web.AspNetCore.Filters
{
    public abstract class ActionFilterBase<THandler> : IActionFilter
        where THandler : class, IActionFilter
    {
        private readonly THandler _handler;

        public ActionFilterBase(IServiceLocatorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            // Unfortunately, we cannot directly inject the dependencies here when using a third-party
            // container. However, we can inject the container which we've registered directly with
            // the .NET DI container. Therefore, we resort to the service-locator (anti-)pattern in
            // this edge case to leverage DI and proxy the calls through the resolved handler.
            this._handler = container.GetInstance<THandler>() ?? throw new InvalidOperationException($"Could not resolve a handler of type [{typeof(IApiActionResultPackageActionFilterHandler).FullName}].");
        }

        public virtual void OnActionExecuted(ActionExecutedContext context)
        {
            this._handler.OnActionExecuted(context);
        }

        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
            this._handler.OnActionExecuting(context);
        }
    }
}
