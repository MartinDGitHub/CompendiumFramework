using CF.Infrastructure.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CF.WebBootstrap.Extensions
{
    public static class ConfigIServiceCollectionExtensions
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Ideally we wouldn't have to couple the configuration loader to the interfaces.
            // However, there is no non-generic overload of IServiceCollection.Configure that
            // accepts the type as a parameter such that we could instead call:
            // new ConfigurationLoader().Load(type => { services.Configure(type, configuration); });
            // Reflection could be used, but is messy and creates a runtime dependency on
            // a CLR type that could change.
            new ConfigurationLoader().Load(services, configuration);
        }
    }
}
