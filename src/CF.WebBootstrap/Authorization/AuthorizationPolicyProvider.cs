using CF.Common.Exceptions;
using CF.WebBootstrap.Authorization.Requirements;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CF.WebBootstrap.Authorization
{
    internal class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private static readonly IEnumerable<string> _authenticationSchemes = new string[]
        {
            CookieAuthenticationDefaults.AuthenticationScheme,
            JwtBearerDefaults.AuthenticationScheme,
        };

        private readonly IPolicyTypeFactory _policyTypeFactory;

        public AuthorizationPolicyProvider(IPolicyTypeFactory policyTypeFactory)
        {
            this._policyTypeFactory = policyTypeFactory ?? throw new ArgumentNullException(nameof(policyTypeFactory));
        }

        // The default policy requires an authenticated user.
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (string.IsNullOrWhiteSpace(policyName))
            {
                throw new ArgumentNullOrWhitespaceException(nameof(policyName));
            }

            var policyType = this._policyTypeFactory.GetPolicyType(policyName);
            if (policyType == null)
            {
                throw new ArgumentException($"No policy type could be resolved for policy with name [{policyName}].");
            }

            var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(_authenticationSchemes.ToArray())
                .RequireAuthenticatedUser()
                .AddRequirements(new PolicyRequirement(policyType));

            return Task.FromResult(policy.Build());
        }
    }
}
