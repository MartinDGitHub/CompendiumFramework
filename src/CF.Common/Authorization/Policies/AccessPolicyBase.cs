using CF.Common.Authorization.Requirements;
using CF.Common.Authorization.Requirements.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Common.Authorization.Policies
{
    /// <summary>
    /// A base policy for access authorization.
    /// </summary>
    internal abstract class AccessPolicyBase
    {
        private readonly IRequirementHandler<RoleClaimRequirement> _handler;

        private readonly AuthorizeMode _authorizeMode;

        protected enum AuthorizeMode
        {
            Any,
            All, // Most secure, default.
        }

        protected abstract IEnumerable<RoleClaimRequirement> ClaimRequirements { get; }

        protected AccessPolicyBase(IRequirementHandler<RoleClaimRequirement> handler, AuthorizeMode authorizeMode = AuthorizeMode.All)
        {
            this._handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this._authorizeMode = authorizeMode;
        }

        public virtual async Task<PolicyResult> AuthorizeAsync()
        {
            var tasks = this.ClaimRequirements.Select(async x => await this._handler.HandleRequirementAsync(x));
            var results = await Task.WhenAll(tasks);

            bool isAuthorized;
            switch (this._authorizeMode)
            {
                case AuthorizeMode.Any:
                    isAuthorized = results.Any(x => x);
                    break;
                case AuthorizeMode.All:
                    isAuthorized = results.All(x => x);
                    break;
                default:
                    throw new InvalidOperationException($"The authorize mode of [{this._authorizeMode}] is unrecognized.");
            }

            var unauthorizedReason = isAuthorized ? null : "User has insufficient access for the operation.";

            return new PolicyResult(isAuthorized, unauthorizedReason);
        }
    }
}
