using System;
using System.Threading.Tasks;

namespace CF.Common.Authorization.Requirements.Handlers
{
    internal class EmailClaimRequirementHandler : IRequirementHandler<EmailClaimRequirement>
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;

        public EmailClaimRequirementHandler(IClaimsPrincipalProvider claimsPrincipalProvider)
        {
            this._claimsPrincipalProvider = claimsPrincipalProvider ?? throw new ArgumentNullException(nameof(claimsPrincipalProvider));
        }

        public Task<bool> HandleRequirementAsync(EmailClaimRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            return Task.FromResult(this._claimsPrincipalProvider.User.HasClaim(
                claim => (claim.Type.Equals(requirement.Type, StringComparison.Ordinal) && requirement.Value.IsMatch(claim.Value))
            ));
        }
    }
}
