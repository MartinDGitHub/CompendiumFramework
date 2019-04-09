using CF.Common.Exceptions;
using CF.Common.Utility;
using System;
using System.Collections.Generic;

namespace CF.Common.Authorization.Policies
{
    public class PolicyResult
    {
        /// <summary>
        /// The policy the result is for.
        /// </summary>
        public IPolicy Policy { get; }

        /// <summary>
        /// Gets whether the policy evaluated to authorized.
        /// </summary>
        public bool IsAuthorized { get; }

        /// <summary>
        /// Gets a consumer-friendly message that can be surfaced to consumers.
        /// </summary>
        public string ConsumerFriendlyMessage { get; }

        /// <summary>
        /// Gets the technical reasons for why the policy evaluated to unauthorized.
        /// </summary>
        public IEnumerable<string> UnauthorizedReasons { get; }

        public PolicyResult(IPolicy policy, bool isAuthorized, IEnumerable<string> unauthorizedReasons, string consumerFriendlyMessage = null)
        {
            this.Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            this.IsAuthorized = isAuthorized;
            this.UnauthorizedReasons = unauthorizedReasons ?? new string[] { };
            this.ConsumerFriendlyMessage = consumerFriendlyMessage ?? "Unauthorized.";
        }

        /// <summary>
        /// Throws an <c>AuthorizationPolicyException</c> if unauthorized.
        /// </summary>
        public void EnsureAuthorized()
        {
            if (!this.IsAuthorized)
            {
                throw new AuthorizationPolicyException(this);
            }
        }

        public override int GetHashCode()
        {
            return HashCodeCalculator.Calculate(this.Policy.GetType().FullName, this.IsAuthorized);
        }

        public override bool Equals(object obj)
        {
            var otherPolicyResult = obj as PolicyResult;

            return
                // Avoid using operators which cause operator overload issues.
                !object.ReferenceEquals(otherPolicyResult, null) && 
                string.Equals(otherPolicyResult.GetType().FullName, this.Policy.GetType().FullName, StringComparison.OrdinalIgnoreCase) && 
                this.IsAuthorized == otherPolicyResult.IsAuthorized;
        }

        public static bool operator true(PolicyResult policyResultOperand)
        {
            return policyResultOperand.IsAuthorized;
        }

        public static bool operator false(PolicyResult policyResultOperand)
        {
            return !policyResultOperand.IsAuthorized;
        }

        public static bool operator !(PolicyResult policyResultOperand)
        {
            return !policyResultOperand.IsAuthorized;
        }

        public static bool operator ==(PolicyResult policyResult1, PolicyResult policyResult2)
        {
            // Avoid using operators which cause operator overload issues.
            if (object.ReferenceEquals(policyResult1, null))
            {
                return object.ReferenceEquals(policyResult2, null);
            }

            return policyResult1.Equals(policyResult2);
        }

        public static bool operator !=(PolicyResult policyResult1, PolicyResult policyResult2) => !(policyResult1 == policyResult2);

        public static bool operator==(bool boolOperand, PolicyResult policyResult)
        {
            if (policyResult == null)
            {
                return false;
            }

            return boolOperand == policyResult.IsAuthorized;
        }

        public static bool operator!=(bool boolOperand, PolicyResult policyResultOperand)
        {
            if (policyResultOperand == null)
            {
                return true;
            }

            return boolOperand != policyResultOperand.IsAuthorized;
        }

        public static bool operator ==(PolicyResult policyResultOperand, bool boolOperand)
        {
            if (policyResultOperand == null)
            {
                return false;
            }

            return boolOperand == policyResultOperand.IsAuthorized;
        }

        public static bool operator !=(PolicyResult policyResultOperand, bool boolOperand)
        {
            if (policyResultOperand == null)
            {
                return true;
            }

            return boolOperand != policyResultOperand.IsAuthorized;
        }

    }
}
