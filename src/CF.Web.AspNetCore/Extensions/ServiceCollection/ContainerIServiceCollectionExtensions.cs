using CF.Infrastructure.DI;
using CF.Web.AspNetCore.DI;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace CF.Web.AspNetCore.Extensions.ServiceCollection
{
    public static class ContainerIServiceCollectionExtensions
    {
        public static void AddCustomContainer(this IServiceCollection services)
        {
            var container = new Container();
            var containerRegistry = new ContainerRegistry<Container>();

            containerRegistry.RegisterContainer(container);

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Register services with the native .NET Core container that cannot be
            // successfully registered with the custom container implementation.
            var webBootstrapRegistrations = new WebBootstrapRegistrations(containerRegistry.Container);
            webBootstrapRegistrations.RegisterNativeServices(services, containerRegistry.ContainerImpl);

            services.EnableSimpleInjectorCrossWiring(container);

            services.UseSimpleInjectorAspNetRequestScoping(container);
        }
    }
}
