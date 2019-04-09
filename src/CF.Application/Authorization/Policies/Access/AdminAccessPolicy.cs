﻿using CF.Common.Authorization;
using CF.Common.Authorization.Policies;
using CF.Common.Authorization.Requirements;
using CF.Common.Authorization.Requirements.Claims;
using CF.Common.Authorization.Requirements.Roles;
using CF.Common.Config;
using System.Collections.Generic;

namespace CF.Application.Authorization.Policies.Access
{
    internal class AdminAccessPolicy : AccessPolicyBase, IAdminAccessPolicy
    {
        private readonly static IEnumerable<RoleClaimRequirement> _roleClaimRequirements = new [] 
        {
            new RoleClaimRequirement(AdminRoleName)
        };

        protected override IEnumerable<RoleClaimRequirement> RoleClaimRequirements => _roleClaimRequirements;

        protected override IEnumerable<WindowsRoleRequirement> WindowsRoleRequirements { get; }

        public AdminAccessPolicy(
            IDomainConfig domainConfig,
            IClaimsPrincipalProvider claimsPrincipalProvider,
            IRequirementHandler<RoleClaimRequirement> roleClaimRequirementHandler,
            IRequirementHandler<WindowsRoleRequirement> windowsRoleRequirementHandler) 
            : base(domainConfig, claimsPrincipalProvider, roleClaimRequirementHandler, windowsRoleRequirementHandler)
        {
            this.WindowsRoleRequirements = new[] { new WindowsRoleRequirement(domainConfig.Name, AdminRoleName) };
        }
    }
}
