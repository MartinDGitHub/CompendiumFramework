using CF.Common.Exceptions;

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
            if (string.IsNullOrWhiteSpace(domainName))
            {
                throw new ArgumentNullOrWhitespaceException(nameof(domainName));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullOrWhitespaceException(nameof(roleName));
            }

            this.FullyQualifiedRoleName = $@"{domainName}\{roleName}";
        }
    }
}
