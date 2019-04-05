using System;
using System.Threading.Tasks;

namespace CF.Common.Authorization.Requirements.Claims
{
    internal class RoleClaimRequirementHandler : IRequirementHandler<RoleClaimRequirement>
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;

        public RoleClaimRequirementHandler(IClaimsPrincipalProvider claimsPrincipalProvider)
        {
            this._claimsPrincipalProvider = claimsPrincipalProvider ?? throw new ArgumentNullException(nameof(claimsPrincipalProvider));
        }

        public Task<RequirementResult> HandleRequirementAsync(RoleClaimRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            var hasClaim = this._claimsPrincipalProvider.User.HasClaim(requirement.Type, requirement.Value);

            return Task.FromResult(new RequirementResult(hasClaim, hasClaim ? null : $"No claim of type [{requirement.Type}] was found with value [{requirement.Value}]."));
        }
    }
}
