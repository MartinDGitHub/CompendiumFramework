using CF.Web.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CF.Web.AspNetCore.Extensions.ServiceCollection
{
    public static class AuthenticationIServiceCollectionExtensions
    {
        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            // Only one default authentication scheme can be added.
            services.AddAuthentication(Constants.DefaultAuthenticationScheme);
        }
    }
}
