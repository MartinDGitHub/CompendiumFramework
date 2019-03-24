using CF.Infrastructure.Config.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CF.Infrastructure.Config
{
    public static class OptionsConfigurer
    {
        public static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RootOptions>(configuration);
        }
    }
}
