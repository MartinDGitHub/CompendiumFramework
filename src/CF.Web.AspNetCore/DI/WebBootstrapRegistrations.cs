using CF.Common.Authorization;
using CF.Common.Authorization.Policies;
using CF.Common.Config;
using CF.Common.DI;
using CF.Common.Utility;
using CF.Infrastructure.DI;
using CF.Web.AspNetCore.Authorization;
using CF.Web.AspNetCore.Authorization.Requirements.Handlers;
using CF.Web.AspNetCore.Config;
using CF.Web.AspNetCore.Filters;
using CF.Web.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CF.Web.AspNetCore.DI
{
    internal class WebBootstrapRegistrations : RegistrationsBase, IRegistrations
    {
        public WebBootstrapRegistrations(IContainer container) : base(container)
        {
        }

        public void RegisterPolicyComponents(IServiceCollection services)
        {
            /*
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
            */
        }


        public void RegisterConfigurationSections(IConfiguration configuration)
        {
            /*
            // Register configuration sections so they can be injected into configuration interface implementations.
            var configSectionTypes = ReflectionHelper.GetImplementationTypes(Assembly.GetExecutingAssembly(), new HashSet<Type> { typeof(IConfigSection) }, new Regex(typeof(IConfigSection).Namespace.Replace(".", @"\.")));
            foreach (var configSectionType in configSectionTypes)
            {
                var populatedConfigSection = configuration.GetSection(configSectionType.Name).Get(configSectionType);
                this.Container.RegisterInstance(populatedConfigSection);
            }
            */
        }

        public void RegisterServices()
        {
            // Register policy infrastructure.
            var policyTypeFactory = new PolicyTypeFactory();
            var policyInterfaceTypes = ReflectionHelper.GetLeafInterfaceTypes(typeof(IStandalonePolicy), RegistrationTypes.CFTypes);
            foreach (var interfaceType in policyInterfaceTypes)
            {
                policyTypeFactory.RegisterPolicyType(interfaceType.Name, interfaceType);
            }
            this.Container.Register<IPolicyTypeFactory, PolicyTypeFactory>(Lifetime.Singleton);
            this.Container.Register<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>(Lifetime.Singleton);
            this.Container.Register<IAuthorizationHandler, PolicyRequirementHandler>(Lifetime.Scoped);

            // Register middlewares.
            this.Container.Register<LoggerScopesMiddleware>(Lifetime.Transient);
            this.Container.Register<GlobalExceptionHandlerMiddleware>(Lifetime.Transient);

            // Register a claims principal provider which will rely on the HTTP context accessor. Registered as transient
            // as IHttpContextAccessor is transient.
            this.Container.Register<IClaimsPrincipalProvider, HttpClaimsPrincipalProvider>(Lifetime.Transient);

            // Register custom filter handlers.
            // These handlers must be registered so that they can be resolved by the IServiceLocatorContainer, avoiding
            // the failure of ASP.NET Core to resolve action filter dependencies, and throwing the follow exception:
            //      System.ArgumentException: Cannot instantiate implementation type '...' for service type '...'.
            //      at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.Populate(IEnumerable`1 descriptors)
            //this.Container.Register<IApiActionResultPackageActionFilterHandler, ApiActionResultPackageActionFilterHandler>();
            //this.Container.Register<IWebActionResultCookieMessageActionFilterHandler, WebActionResultCookieMessageActionFilterHandler>();

            // Register configuration per request to ensure that any configuration changes are discovered 
            // and to keep authorization consistent throughout a request.
            this.RegisterDerivedInterfaceImplementations<IConfig>(this.Container, Lifetime.Scoped);
        }
    }
}
