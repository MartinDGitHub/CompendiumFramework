using System;

namespace CF.Common.Authorization.Requirements.Claims
{
    public abstract class ClaimRequirementBase<T> : IClaimRequirement<T>
    {
        public string Type { get; }

        public T Value { get; protected set; }

        public ClaimRequirementBase(string claimType)
        {
            if (string.IsNullOrWhiteSpace(claimType))
            {
                throw new ArgumentException($"Parameter [{nameof(claimType)}] was unspecified.");
            }

            this.Type = claimType;
        }
    }
}
