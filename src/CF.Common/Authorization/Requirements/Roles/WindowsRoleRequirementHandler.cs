using CF.Common.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CF.Common.Authorization.Requirements.Roles
{
    internal class WindowsRoleRequirementHandler : IRequirementHandler<WindowsRoleRequirement>
    {
        private readonly static ConcurrentDictionary<string, string> _groupSidByRoleName = new ConcurrentDictionary<string, string>();
        private readonly static ConcurrentDictionary<string, string> _roleNameByGroupSid = new ConcurrentDictionary<string, string>();

        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;

        public WindowsRoleRequirementHandler(IClaimsPrincipalProvider claimsPrincipalProvider)
        {
            this._claimsPrincipalProvider = claimsPrincipalProvider ?? throw new ArgumentNullException(nameof(claimsPrincipalProvider));
        }

        public Task<RequirementResult> HandleRequirementAsync(WindowsRoleRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            var hasClaim = this._claimsPrincipalProvider.User?.Identity is WindowsIdentity &&
                (
                // Oddly, this check can fail at times if certain authentication schemes are set...
                this._claimsPrincipalProvider.User.IsInRole(requirement.FullyQualifiedRoleName) ||
                // Fallback if required to using translations of Windows group SIDs to role names.
                this._claimsPrincipalProvider.User.HasClaim(ClaimTypes.GroupSid, resolveGroupSidForRoleName(this._claimsPrincipalProvider.User.Claims, requirement.FullyQualifiedRoleName) ?? string.Empty)
                );

            return Task.FromResult(new RequirementResult(hasClaim, hasClaim ? null : $"No claim of type [{ClaimTypes.GroupSid}] was could be resolved for role name [{requirement.FullyQualifiedRoleName}]."));

            string resolveGroupSidForRoleName(IEnumerable<Claim> claims, string fullyQualifiedRoleName)
            {
                // If a group SID for the role name hasn't been resolved yet, translate any new group SIDs
                // into role names and return the group SID at the first match, if there is one.
                if (!_groupSidByRoleName.TryGetValue(fullyQualifiedRoleName, out string groupSid))
                {
                    foreach (var claim in claims)
                    {
                        if (string.Equals(claim.Type, ClaimTypes.GroupSid, StringComparison.OrdinalIgnoreCase) && !_roleNameByGroupSid.ContainsKey(claim.Value))
                        {
                            var translatedRoleName = new SecurityIdentifier(claim.Value).Translate(typeof(NTAccount)).ToString();
                            _roleNameByGroupSid.TryAdd(claim.Value, translatedRoleName);
                            _groupSidByRoleName.TryAdd(translatedRoleName, claim.Value);
                            if (string.Equals(fullyQualifiedRoleName, translatedRoleName, StringComparison.OrdinalIgnoreCase))
                            {
                                return claim.Value;
                            }
                        }
                    }
                }

                return groupSid;
            }
        }
    }
}
