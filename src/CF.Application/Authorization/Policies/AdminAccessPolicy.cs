﻿using CF.Common.Authorization.Policies;
using CF.Common.Authorization.Requirements;
using CF.Common.Authorization.Requirements.Claims;
using CF.Common.Authorization.Requirements.Roles;
using CF.Common.Config;
using System.Collections.Generic;

namespace CF.Application.Authorization.Policies
{
    internal class AdminAccessPolicy : AccessPolicyBase, IAdminAccessPolicy
    {
        protected override IEnumerable<RoleClaimRequirement> RoleClaimRequirements { get; } = new[] { new RoleClaimRequirement(AdminRoleName) };

        protected override IEnumerable<WindowsRoleRequirement> WindowsRoleRequirements { get; }

        public AdminAccessPolicy(
            IDomainConfig domainConfig,
            IRequirementHandler<RoleClaimRequirement> roleClaimRequirementHandler,
            IRequirementHandler<WindowsRoleRequirement> windowsRoleRequirementHandler) 
            : base(domainConfig, roleClaimRequirementHandler, windowsRoleRequirementHandler)
        {
            this.WindowsRoleRequirements = new[] { new WindowsRoleRequirement(domainConfig.Name, AdminRoleName) };
        }
    }
}
