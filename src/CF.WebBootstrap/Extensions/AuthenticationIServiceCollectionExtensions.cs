using CF.WebBootstrap.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CF.WebBootstrap.Extensions
{
    public static class AuthenticationIServiceCollectionExtensions
    {
        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            // Only one default authentication scheme can be added.
            services.AddAuthentication(Constants.DefaultAuthenticationScheme);

            // Ensure the HTTP Context providing the claims principal is available for injecting into
            // authorization policies.
            services.AddHttpContextAccessor();
        }
    }
}
