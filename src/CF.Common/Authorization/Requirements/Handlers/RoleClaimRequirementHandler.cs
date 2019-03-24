using System;
using System.Threading.Tasks;

namespace CF.Common.Authorization.Requirements.Handlers
{
    internal class RoleClaimRequirementHandler : IRequirementHandler<RoleClaimRequirement>
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;

        public RoleClaimRequirementHandler(IClaimsPrincipalProvider claimsPrincipalProvider)
        {
            this._claimsPrincipalProvider = claimsPrincipalProvider ?? throw new ArgumentNullException(nameof(claimsPrincipalProvider));
        }

        public Task<bool> HandleRequirementAsync(RoleClaimRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            return Task.FromResult(this._claimsPrincipalProvider.User.HasClaim(requirement.Type, requirement.Value));
        }
    }
}
