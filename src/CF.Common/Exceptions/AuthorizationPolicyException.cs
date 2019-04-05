using CF.Common.Authorization.Policies;
using System;
using System.Collections.Generic;

namespace CF.Common.Exceptions
{
    public class AuthorizationPolicyException : Exception
    {
        public override string Message { get; }

        public IEnumerable<string> UnauthorizedReasons { get; }

        public AuthorizationPolicyException(PolicyResult policyResult, string message = null) : base()
        {
            if (policyResult == null)
            {
                throw new ArgumentNullException(nameof(policyResult));
            }

            this.UnauthorizedReasons = new List<string>(policyResult.UnauthorizedReasons ?? new string[] { });

            this.Message = $"Message: {message}\nPolicy [{policyResult.Policy.GetType().FullName}] did not authorize for the following reasons:\n[{string.Join("]\n[", this.UnauthorizedReasons)}].";
        }
    }
}
