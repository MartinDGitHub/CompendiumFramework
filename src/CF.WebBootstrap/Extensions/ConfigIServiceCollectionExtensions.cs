using CF.WebBootstrap.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CF.WebBootstrap.Extensions
{
    public static class ConfigIServiceCollectionExtensions
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RootOptions>(configuration);
        }
    }
}
