using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.DependencyInjection;

namespace CF.WebBootstrap.Extensions
{
    public static class AuthorizationIServiceCollectionExtensions
    {
        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            // Required for Windows authentication with IIS.
            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            // Ensure the HTTP Context providing the claims principal is available for injecting into
            // authorization policies.
            services.AddHttpContextAccessor();
        }
    }
}
