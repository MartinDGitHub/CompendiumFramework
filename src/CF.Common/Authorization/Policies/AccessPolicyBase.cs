using CF.Common.Authorization.Requirements;
using CF.Common.Authorization.Requirements.Claims;
using CF.Common.Authorization.Requirements.Roles;
using CF.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Common.Authorization.Policies
{
    /// <summary>
    /// A base policy for access authorization that checks for access based on either role claims or Windows roles.
    /// </summary>
    public abstract class AccessPolicyBase : IPolicy
    {
        private readonly IDomainConfig _domainConfig;
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;
        private readonly IRequirementHandler<RoleClaimRequirement> _roleClaimRequirementHandler;
        private readonly IRequirementHandler<WindowsRoleRequirement> _windowsRoleRequirementHandler;
        private readonly AuthorizeMode _authorizeMode;

        protected const string AdminRoleName = "AdminGroup";
        protected const string UserRoleName = "UserGroup";

        /// <summary>
        /// This mode determines within a set of requirements of a common type (e.g. role claim) whether 
        /// all requirements must be met, or if at least one must be met, to be considered authorized.
        /// </summary>
        protected enum AuthorizeMode
        {
            Any,
            All, // Most secure, default.
        }

        protected abstract IEnumerable<RoleClaimRequirement> RoleClaimRequirements { get; }

        protected abstract IEnumerable<WindowsRoleRequirement> WindowsRoleRequirements { get; }

        protected AccessPolicyBase(
            IDomainConfig domainConfig,
            IClaimsPrincipalProvider claimsPrincipalProvider,
            IRequirementHandler<RoleClaimRequirement> roleClaimRequirementHandler,
            IRequirementHandler<WindowsRoleRequirement> windowsRoleRequirementHandler,
            AuthorizeMode authorizeMode = AuthorizeMode.All)
        {
            this._domainConfig = domainConfig ?? throw new ArgumentNullException(nameof(domainConfig));
            this._claimsPrincipalProvider = claimsPrincipalProvider ?? throw new ArgumentNullException(nameof(claimsPrincipalProvider));
            this._roleClaimRequirementHandler = roleClaimRequirementHandler ?? throw new ArgumentNullException(nameof(roleClaimRequirementHandler));
            this._windowsRoleRequirementHandler = windowsRoleRequirementHandler ?? throw new ArgumentNullException(nameof(windowsRoleRequirementHandler));
            this._authorizeMode = authorizeMode;
        }

        public virtual async Task<PolicyResult> AuthorizeAsync()
        {
            // First try the generic route of checking for standard role claims.
            var unmetReasons = new List<string>();
            var tasks = this.RoleClaimRequirements.Select(async x => await this._roleClaimRequirementHandler.HandleRequirementAsync(x));
            var taskResults = await Task.WhenAll(tasks);
            unmetReasons.AddRange(taskResults.Where(x => !x.IsMet).Select(x => x.UnmetMessage));
            var isAuthorized = IsAuthorized(taskResults);
            // If unable to authorize via standard role claims, fall back to checking for Windows
            // roles which are represented as group SIDs.
            if (!isAuthorized)
            {
                tasks = this.WindowsRoleRequirements.Select(async x => await this._windowsRoleRequirementHandler.HandleRequirementAsync(x));
                taskResults = await Task.WhenAll(tasks);
                unmetReasons.AddRange(taskResults.Where(x => !x.IsMet).Select(x => x.UnmetMessage));
                isAuthorized = IsAuthorized(taskResults);
            }

            var unauthorizedReason = isAuthorized ? null : $"User [{this._claimsPrincipalProvider?.User?.Identity?.Name}] has insufficient access for the operation.";

            return new PolicyResult(this, isAuthorized, (new string[] { unauthorizedReason }).Concat(unmetReasons));

            bool IsAuthorized(RequirementResult[] requirementResults)
            {
                switch (this._authorizeMode)
                {
                    case AuthorizeMode.Any:
                        return requirementResults.Any(x => x.IsMet);
                    case AuthorizeMode.All:
                        return requirementResults.All(x => x.IsMet);
                    default:
                        throw new InvalidOperationException($"The authorize mode of [{this._authorizeMode}] is unrecognized.");
                }
            }
        }
    }
}
