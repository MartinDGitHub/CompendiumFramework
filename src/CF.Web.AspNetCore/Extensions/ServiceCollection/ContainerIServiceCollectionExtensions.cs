using CF.Infrastructure.DI;
using Microsoft.Extensions.DependencyInjection;

namespace CF.Web.AspNetCore.Extensions.ServiceCollection
{
    public static class ContainerIServiceCollectionExtensions
    {
        public static void AddCustomContainer(this IServiceCollection services)
        {
            var containerRegistry = new ContainerRegistry<IServiceCollection>();

            // Register the .NET Core container within the framework's wrapping container.
            containerRegistry.RegisterContainer(services);

            // Performs registrations and other configuration.
            containerRegistry.ConfigureContainer();
        }
    }
}
