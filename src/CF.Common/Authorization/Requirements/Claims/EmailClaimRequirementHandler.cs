using System;
using System.Threading.Tasks;

namespace CF.Common.Authorization.Requirements.Claims
{
    internal class EmailClaimRequirementHandler : IRequirementHandler<EmailClaimRequirement>
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;

        public EmailClaimRequirementHandler(IClaimsPrincipalProvider claimsPrincipalProvider)
        {
            this._claimsPrincipalProvider = claimsPrincipalProvider ?? throw new ArgumentNullException(nameof(claimsPrincipalProvider));
        }

        public Task<RequirementResult> HandleRequirementAsync(EmailClaimRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (requirement.Value == null)
            {
                throw new ArgumentException("No value was provided by the requirement.");
            }

            var hasClaim = this._claimsPrincipalProvider.User.HasClaim(
                claim => (claim.Type.Equals(requirement.Type, StringComparison.OrdinalIgnoreCase) && requirement.Value.IsMatch(claim.Value))
            );

            return Task.FromResult(new RequirementResult(hasClaim, hasClaim ? null : $"No claim of type [{requirement.Type}] was matched for regex [{requirement.Value}]."));
        }
    }
}
