using CF.Common.Authorization;
using CF.Common.Authorization.Policies;
using CF.Common.Config;
using CF.Common.DI;
using CF.Common.Utility;
using CF.Infrastructure.DI;
using CF.Web.AspNetCore.Authorization;
using CF.Web.AspNetCore.Authorization.Requirements.Handlers;
using CF.Web.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace CF.Web.AspNetCore.DI
{
    internal class WebBootstrapRegistrations : RegistrationsBase, IRegistrations
    {
        public WebBootstrapRegistrations(IContainer container) : base(container)
        {
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
            this.Container.Register<IAuthorizationHandler, StandalonePolicyRequirementHandler>(Lifetime.Scoped);

            // Register middlewares.
            this.Container.Register<LoggerScopesMiddleware>(Lifetime.Transient);
            this.Container.Register<GlobalExceptionHandlerMiddleware>(Lifetime.Transient);

            // Register a claims principal provider which will rely on the HTTP context accessor. Registered as transient
            // as IHttpContextAccessor is transient.
            this.Container.Register<IClaimsPrincipalProvider, HttpClaimsPrincipalProvider>(Lifetime.Transient);

            // Register configuration per request to ensure that any configuration changes are discovered 
            // and to keep authorization consistent throughout a request.
            this.RegisterDerivedInterfaceImplementations<IConfig>(this.Container, Lifetime.Scoped);
        }
    }
}
