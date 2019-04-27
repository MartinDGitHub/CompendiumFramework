using CF.Web.AspNetCore.Config.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CF.Web.AspNetCore.Extensions.ServiceCollection
{
    public static class ConfigIServiceCollectionExtensions
    {
        public static void AddCustomConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Root>(configuration);
        }
    }
}
