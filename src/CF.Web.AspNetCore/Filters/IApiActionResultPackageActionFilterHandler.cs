using Microsoft.AspNetCore.Mvc.Filters;

namespace CF.Web.AspNetCore.Filters
{
    internal interface IApiActionResultPackageActionFilterHandler
    {
        void OnActionExecuted(ActionExecutedContext context);
    }
}