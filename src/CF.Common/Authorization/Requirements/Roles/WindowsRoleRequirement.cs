using CF.Common.Extensions;

namespace CF.Common.Authorization.Requirements.Roles
{
    public class WindowsRoleRequirement : IRequirement
    {
        /// <summary>
        /// Gets the role name fully qualified by the domain, when applicable.
        /// </summary>
        public string FullyQualifiedRoleName { get; }

        public WindowsRoleRequirement(string domainName, string roleName)
        {
            domainName.EnsureArgumentNotNullOrWhitespace(nameof(domainName));
            roleName.EnsureArgumentNotNullOrWhitespace(nameof(roleName));

            this.FullyQualifiedRoleName = $@"{domainName}\{roleName}";
        }
    }
}
