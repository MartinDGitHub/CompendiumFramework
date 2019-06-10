using CF.Common.Authorization.Policies;
using CF.Infrastructure.DI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Authorization.Requirements.Handlers
{
    internal class PolicyRequirementHandler : AuthorizationHandler<PolicyRequirement>
    {
        private readonly IServiceProvider _serviceProvider;
        
        public PolicyRequirementHandler(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            using (var serviceScope = this._serviceProvider.CreateScope())
            {
                var policy = serviceScope.ServiceProvider.GetRequiredService(requirement.PolicyType) as IStandalonePolicy;

                if (policy == null)
                {
                    throw new InvalidOperationException($"No policy of type [{typeof(IStandalonePolicy).FullName}] for policy type [{requirement.PolicyType.FullName}] could be resolved from the service container.");
                }

                var policyResult = await policy.AuthorizeAsync();
                if (policyResult)
                {
                    context.Succeed(requirement);
                }
            }

            return;
        }
    }
}
