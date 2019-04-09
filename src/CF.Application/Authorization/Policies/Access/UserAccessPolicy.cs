using CF.Common.Authorization;
using CF.Common.Authorization.Policies;
using CF.Common.Authorization.Requirements;
using CF.Common.Authorization.Requirements.Claims;
using CF.Common.Authorization.Requirements.Roles;
using CF.Common.Config;
using System.Collections.Generic;

namespace CF.Application.Authorization.Policies.Access
{
    internal class UserAccessPolicy : AccessPolicyBase, IUserAccessPolicy
    {
        private readonly static IEnumerable<RoleClaimRequirement> _roleClaimRequirements = new []
        {
            new RoleClaimRequirement(UserRoleName),
            // Include higher access groups that include lower access groups.
            // Important! An authorize mode of any must be used when evaluating.
            new RoleClaimRequirement(AdminRoleName),
        };

        protected override IEnumerable<RoleClaimRequirement> RoleClaimRequirements => _roleClaimRequirements;

        protected override IEnumerable<WindowsRoleRequirement> WindowsRoleRequirements { get; }

        public UserAccessPolicy(
            IDomainConfig domainConfig,
            IClaimsPrincipalProvider claimsPrincipalProvider,
            IRequirementHandler<RoleClaimRequirement> roleClaimRequirementHandler,
            IRequirementHandler<WindowsRoleRequirement> windowsRoleRequirementHandler)
            : base(domainConfig, claimsPrincipalProvider, roleClaimRequirementHandler, windowsRoleRequirementHandler, AuthorizeMode.Any)
        {
            this.WindowsRoleRequirements = new[]
            {
                new WindowsRoleRequirement(domainConfig.Name, UserRoleName),
                // Include higher access groups that include lower access groups.
                // Important! An authorize mode of any must be used when evaluating.
                new WindowsRoleRequirement(domainConfig.Name, AdminRoleName),
            };
        }
    }
}
