using CF.Infrastructure.DI;

namespace CF.Web.AspNetCore.Filters
{
    public class WebActionResultCookieMessageActionFilter : ActionFilterBase<IWebActionResultCookieMessageActionFilterHandler>
   {
        public WebActionResultCookieMessageActionFilter(IServiceLocatorContainer container) : base(container)
        {
        }
    }
}
