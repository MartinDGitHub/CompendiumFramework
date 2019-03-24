using CF.Common.Authorization.Requirements;
using CF.Common.Authorization.Requirements.Handlers;
using System.Collections.Generic;

namespace CF.Common.Authorization.Policies
{
    internal class UserAccessPolicy : AccessPolicyBase, IUserAccessPolicy
    {
        protected override IEnumerable<RoleClaimRequirement> ClaimRequirements => new[] 
        {
            new RoleClaimRequirement("UserGroup"),
            // Include higher access groups that include lower access groups.
            // Important! An authorize mode of any must be used when evaluating.
            new RoleClaimRequirement("AdminGroup"),
        };

        public UserAccessPolicy(IRequirementHandler<RoleClaimRequirement> handler) : base(handler, AuthorizeMode.Any)
        {
        }
    }
}
