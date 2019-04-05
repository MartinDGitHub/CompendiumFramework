using CF.Common.Authorization;
using CF.Common.Authorization.Policies;
using CF.Common.Config;
using CF.Common.DI;
using CF.Infrastructure.DI;
using CF.WebBootstrap.Authorization;
using CF.WebBootstrap.Authorization.Requirements.Handlers;
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
            services.AddSingleton<IAuthorizationHandler, PolicyRequirementHandler>();
            services.AddSingleton(typeof(IServiceLocatorContainer), this.Container);

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(containerImpl));

            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(containerImpl));

            // For a web application, we will need to access the HTTP context to get the claims principal.
            services.AddHttpContextAccessor();
        }

        public void RegisterServices()
        {
            // Register a claims principal provider which will rely on the HTTP context accessor.
            this.Container.Register<IClaimsPrincipalProvider, HttpClaimsPrincipalProvider>(Lifetime.Scoped);

            // Register configuration per request to ensure that any configuration changes are discovered 
            // and to keep authorization consistent throughout a request.
            this.RegisterDerivedInterfaceImplementations<IConfig>(this.Container, Lifetime.Scoped);
        }
    }
}
