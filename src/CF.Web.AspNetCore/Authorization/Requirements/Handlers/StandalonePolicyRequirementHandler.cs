using CF.Common.Authorization.Policies;
using CF.Infrastructure.DI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Authorization.Requirements.Handlers
{
    internal class StandalonePolicyRequirementHandler : AuthorizationHandler<StandalonePolicyRequirement>
    {
        private IEnumerable<IStandalonePolicy> _standalonePolicies;

        public StandalonePolicyRequirementHandler(IEnumerable<IStandalonePolicy> standalonePolicies)
        {
            this._standalonePolicies = standalonePolicies ?? throw new ArgumentNullException(nameof(standalonePolicies));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StandalonePolicyRequirement requirement)
        {
            var policies = this._standalonePolicies.Where(x => requirement.StandalonePolicyType.IsAssignableFrom(x.GetType()));

            if (policies.Count() > 1)
            {
                throw new InvalidOperationException($"[{policies.Count()}] policies [{typeof(IStandalonePolicy).Name}] implementing [{requirement.StandalonePolicyType.FullName}] were resolved - only one is permitted.");
            }
            else if (!policies.Any())
            {
                throw new InvalidOperationException($"No standalone policy [{typeof(IStandalonePolicy).Name}] implementing [{requirement.StandalonePolicyType.FullName}] could be resolved - only one is permitted.");
            }
            
            var policyResult = await policies.Single().AuthorizeAsync().ConfigureAwait(false);
            if (policyResult)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}
