using CF.Common.Authorization.Policies;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace CF.Web.AspNetCore.Authorization.Requirements
{
    public class StandalonePolicyRequirement : IAuthorizationRequirement
    {
        public Type StandalonePolicyType { get; }

        public StandalonePolicyRequirement(Type standalonePolicyType)
        {
            this.StandalonePolicyType = standalonePolicyType ?? throw new ArgumentNullException(nameof(standalonePolicyType));

            if (!this.StandalonePolicyType.GetInterfaces().Any(x => x == typeof(IStandalonePolicy)))
            {
                throw new ArgumentException($"The policy type [{standalonePolicyType.FullName}] does not inherit from [{typeof(IStandalonePolicy).FullName}].");
            }
        }
    }
}
