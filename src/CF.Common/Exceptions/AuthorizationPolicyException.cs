using CF.Common.Authorization.Policies;
using System;

namespace CF.Common.Exceptions
{
    public class AuthorizationPolicyException<TPolicy> : Exception
        where TPolicy : IPolicy
    {
        public AuthorizationPolicyException(TPolicy policy, PolicyResult policyResult, string message = null) 
            : base($"The policy [{policy?.GetType().FullName ?? string.Empty}] did not authorize for the following reason:[{policyResult?.UnauthorizedReason ?? string.Empty}].\n{message ?? string.Empty}")
        {
        }
    }
}
