using CF.Common.Authorization;
using CF.Common.Authorization.Policies;
using CF.Common.Config;
using CF.Common.DI;
using CF.Infrastructure.DI;
using CF.Web.AspNetCore.Authorization;
using CF.Web.AspNetCore.Authorization.Requirements.Handlers;
using CF.Web.AspNetCore.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleInjector.Integration.AspNetCore.Mvc;

namespace CF.Web.AspNetCore.DI
{
    internal class WebBootstrapRegistrations : RegistrationsBase, IRegistrations
    {
        public WebBootstrapRegistrations(IContainer container) : base(container)
        {
        }

        public void RegisterNativeServices(IServiceCollection services, SimpleInjector.Container containerImpl)
        {
            // The authorization policy provider must be overridden here, or the following exception will be thrown on app.UseMvc:
            // System.InvalidOperationException: 'The AuthorizationPolicy named: '...' was not found.'
            // Only standalone policies are compatible with the authorization policy provider which resolves policies for
            // context free attributes, etc.
            var policyTypeFactory = new PolicyTypeFactory();
            var interfaceTypes = this.GetLeafInterfaceTypes(typeof(IStandalonePolicy), RegistrationTypes.CFTypes);
            foreach (var interfaceType in interfaceTypes)
            {
                policyTypeFactory.RegisterPolicyType(interfaceType.Name, interfaceType);
            }
            services.AddSingleton<IPolicyTypeFactory, PolicyTypeFactory>();
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PolicyRequirementHandler>();

            // Register the service locator for cases where ASP.NET Core fails to perform DI as expected.
            // In those cases (e.g. action filters), we have to resort to the service locator (anti-)pattern.
            // See: https://simpleinjector.readthedocs.io/en/latest/webapiintegration.html#injecting-dependencies-into-web-api-filter-attributes
            services.AddSingleton(typeof(IServiceLocatorContainer), this.Container);

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(containerImpl));

            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(containerImpl));
        }

        public void RegisterServices()
        {
            // Register a claims principal provider which will rely on the HTTP context accessor.
            this.Container.Register<IClaimsPrincipalProvider, HttpClaimsPrincipalProvider>(Lifetime.Scoped);

            // Register custom filter handlers.
            // These handlers must be registered so that they can be resolved by the IServiceLocatorContainer, avoiding
            // the failure of ASP.NET Core to resolve action filter dependencies, and throwing the follow exception:
            //      System.ArgumentException: Cannot instantiate implementation type '...' for service type '...'.
            //      at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.Populate(IEnumerable`1 descriptors)
            this.Container.Register<IApiActionResultPackageActionFilterHandler, ApiActionResultPackageActionFilterHandler>();
            this.Container.Register<IWebActionResultCookieMessageActionFilterHandler, WebActionResultCookieMessageActionFilterHandler>();

            // Register configuration per request to ensure that any configuration changes are discovered 
            // and to keep authorization consistent throughout a request.
            this.RegisterDerivedInterfaceImplementations<IConfig>(this.Container, Lifetime.Scoped);

        }
    }
}
