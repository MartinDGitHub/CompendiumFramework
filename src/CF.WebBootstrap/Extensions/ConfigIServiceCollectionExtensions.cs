using CF.WebBootstrap.Config.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CF.WebBootstrap.Extensions
{
    public static class ConfigIServiceCollectionExtensions
    {
        public static void AddCustomConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Root>(configuration);
        }
    }
}
