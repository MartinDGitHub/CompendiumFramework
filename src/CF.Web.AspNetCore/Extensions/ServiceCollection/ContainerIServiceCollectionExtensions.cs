using CF.Infrastructure.DI;
using CF.Web.AspNetCore.DI;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
/*
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
*/
namespace CF.Web.AspNetCore.Extensions.ServiceCollection
{
    public static class ContainerIServiceCollectionExtensions
    {
        public static void AddCustomContainer(this IServiceCollection services)
        {
            var containerRegistry = new ContainerRegistry<IServiceCollection>();
            containerRegistry.RegisterContainer(services);
            containerRegistry.ConfigureContainer();

            /*
            var container = new Container();
            var containerRegistry = new ContainerRegistry<Container>();

            containerRegistry.RegisterContainer(container);

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            */


            // Register services with the native .NET Core container that cannot be
            // successfully registered with the custom container implementation.
            //var webBootstrapRegistrations = new WebBootstrapRegistrations(containerRegistry.Container);
            //webBootstrapRegistrations.RegisterPolicyComponents(services);

            // Register the service locator for cases where Simple Injector fails to perform DI as expected.
            // In those cases (e.g. action filters), we have to resort to the service locator (anti-)pattern.
            // See: https://simpleinjector.readthedocs.io/en/latest/webapiintegration.html#injecting-dependencies-into-web-api-filter-attributes
            //services.AddSingleton(typeof(IServiceLocatorContainer), containerRegistry.Container);

            //services.AddHttpContextAccessor();

            /*
            services.EnableSimpleInjectorCrossWiring(container);

            services.UseSimpleInjectorAspNetRequestScoping(container);

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));

            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(container));
            */
        }
    }
}
