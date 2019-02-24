using CF.Infrastructure.Config.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CF.Infrastructure.Config
{
    public class ConfigurationLoader
    {
        public void Load(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RootOptions>(configuration);
        }
    }
}
