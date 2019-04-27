using CF.Common.Extensions;
using CF.Web.AspNetCore.Authentication;
using CF.Web.AspNetCore.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Authorization
{
    internal class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly IPolicyTypeFactory _policyTypeFactory;

        public AuthorizationPolicyProvider(IPolicyTypeFactory policyTypeFactory)
        {
            this._policyTypeFactory = policyTypeFactory ?? throw new ArgumentNullException(nameof(policyTypeFactory));
        }

        // The default policy requires an authenticated user.
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            policyName.EnsureArgumentNotNullOrWhitespace(nameof(policyName));

            var policyType = this._policyTypeFactory.GetPolicyType(policyName);
            if (policyType == null)
            {
                throw new ArgumentException($"No policy type could be resolved for policy with name [{policyName}].");
            }

            var policy = new AuthorizationPolicyBuilder()
                // NOTE: adding authentication schemes here can oddly break the lookup for IsInRole on a Windows claims principal...
                .AddAuthenticationSchemes(Constants.AuthenticationSchemes)
                .RequireAuthenticatedUser()
                .AddRequirements(new PolicyRequirement(policyType));

            return Task.FromResult(policy.Build());
        }
    }
}
