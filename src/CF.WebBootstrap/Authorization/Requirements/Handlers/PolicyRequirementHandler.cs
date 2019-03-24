using CF.Common.Authorization.Policies;
using CF.Infrastructure.DI;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace CF.WebBootstrap.Authorization.Requirements.Handlers
{
    internal class PolicyRequirementHandler : AuthorizationHandler<PolicyRequirement>
    {
        private readonly IServiceLocatorContainer _serviceLocatorContainer;
        
        public PolicyRequirementHandler(IServiceLocatorContainer serviceLocatorContainer)
        {
            this._serviceLocatorContainer = serviceLocatorContainer ?? throw new ArgumentNullException(nameof(serviceLocatorContainer));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            var policy = this._serviceLocatorContainer.GetInstance(requirement.PolicyType) as IStandalonePolicy;
            if (policy == null)
            {
                throw new InvalidOperationException($"No policy of type [{typeof(IStandalonePolicy).FullName}] for policy type [{requirement.PolicyType.FullName}] could be resolved from the service container.");
            }

            var policyResult = await policy.AuthorizeAsync();
            if (policyResult)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}
