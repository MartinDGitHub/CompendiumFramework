using CF.Infrastructure.DI;

namespace CF.Web.AspNetCore.Filters
{
    public class ApiActionResultPackageActionFilter : ActionFilterBase<IApiActionResultPackageActionFilterHandler>
    {
        public ApiActionResultPackageActionFilter(IServiceLocatorContainer container) : base(container)
        {
        }
    }
}
