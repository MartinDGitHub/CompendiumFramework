using CF.Common.Authorization;
using CF.Common.Authorization.Policies;
using CF.Common.DI;
using CF.Infrastructure.DI;
using CF.WebBootstrap.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleInjector.Integration.AspNetCore.Mvc;

namespace CF.WebBootstrap.DI
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

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(containerImpl));

            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(containerImpl));
        }

        public void RegisterServices()
        {
            this.Container.Register<IHttpContextAccessor, HttpContextAccessor>(Lifetime.Singleton);

            // Register a provider of the user's claim principal for HTTP requests.
            this.Container.Register<IClaimsPrincipalProvider, HttpClaimsPrincipalProvider>(Lifetime.Scoped);
        }
    }
}
