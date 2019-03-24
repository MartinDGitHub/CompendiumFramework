using CF.Common.Exceptions;
using System;
using System.Security.Claims;

namespace CF.Common.Authorization.Requirements
{
    /// <summary>
    /// Requires that the user have a role claim with a value that matches the requirement role name.
    /// </summary>
    internal class RoleClaimRequirement : ClaimRequirementBase<string>
    {
        public RoleClaimRequirement(string roleName) : base(ClaimTypes.Role)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullOrWhitespaceException(nameof(roleName));
            }

            this.Value = roleName;
        }
    }
}
