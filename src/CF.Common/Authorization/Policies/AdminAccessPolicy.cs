using CF.Common.Authorization.Requirements;
using CF.Common.Authorization.Requirements.Handlers;
using System.Collections.Generic;

namespace CF.Common.Authorization.Policies
{
    internal class AdminAccessPolicy : AccessPolicyBase, IAdminAccessPolicy
    {
        protected override IEnumerable<RoleClaimRequirement> ClaimRequirements => new[] { new RoleClaimRequirement("AdminGroup") };

        public AdminAccessPolicy(IRequirementHandler<RoleClaimRequirement> handler) : base(handler)
        {
        }
    }
}
