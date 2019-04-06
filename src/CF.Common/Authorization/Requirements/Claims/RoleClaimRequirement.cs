using CF.Common.Extensions;
using System.Security.Claims;

namespace CF.Common.Authorization.Requirements.Claims
{
    /// <summary>
    /// Requires that the user have a role claim with a value that matches the requirement role name.
    /// </summary>
    public class RoleClaimRequirement : ClaimRequirementBase<string>
    {
        public RoleClaimRequirement(string roleName) : base(ClaimTypes.Role)
        {
            this.Value = roleName.EnsureArgumentNotNullOrWhitespace(nameof(roleName));
        }
    }
}
