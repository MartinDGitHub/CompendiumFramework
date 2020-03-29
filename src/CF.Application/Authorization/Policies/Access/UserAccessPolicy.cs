using CF.Common.Authorization;
using CF.Common.Authorization.Policies;
using CF.Common.Authorization.Requirements;
using CF.Common.Authorization.Requirements.Claims;
using CF.Common.Authorization.Requirements.Roles;
using CF.Common.Config;
using System;
using System.Collections.Generic;

namespace CF.Application.Authorization.Policies.Access
{
    internal class UserAccessPolicy : AccessPolicyBase, IUserAccessPolicy
    {
        public override IEnumerable<RoleClaimRequirement> Requirements { get; } = new[]
        {
            new RoleClaimRequirement(UserRoleName),
            // Include higher access groups that include lower access groups.
            // Important! An authorize mode of any must be used when evaluating.
            new RoleClaimRequirement(AdminRoleName),
        };

        protected override IEnumerable<WindowsRoleRequirement> WindowsRoleRequirements { get; }

        public UserAccessPolicy(
            IDomainConfig domainConfig,
            IClaimsPrincipalProvider claimsPrincipalProvider,
            IRequirementHandler<RoleClaimRequirement> roleClaimRequirementHandler,
            IRequirementHandler<WindowsRoleRequirement> windowsRoleRequirementHandler)
            : base(claimsPrincipalProvider, roleClaimRequirementHandler, windowsRoleRequirementHandler, AuthorizeMode.Any)
        {
            if (domainConfig == null)
            {
                throw new ArgumentNullException(nameof(domainConfig));
            }

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
