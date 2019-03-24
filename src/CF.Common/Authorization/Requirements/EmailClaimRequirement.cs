using System;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace CF.Common.Authorization.Requirements
{
    /// <summary>
    /// Requires that the user have an email claim with a value that matches the requirement regular expression.
    /// </summary>
    internal class EmailClaimRequirement : ClaimRequirementBase<Regex>
    {
        public EmailClaimRequirement(Regex emailRegex) : base(ClaimTypes.Email)
        {
            this.Value = emailRegex ?? throw new ArgumentNullException(nameof(emailRegex));
        }
    }
}
