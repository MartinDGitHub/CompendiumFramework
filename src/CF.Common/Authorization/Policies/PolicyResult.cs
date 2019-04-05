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
        /// Gets a user-friendly message that can be surfaced to consumers.
        /// </summary>
        public string UserFriendlyMessage { get; }

        /// <summary>
        /// Gets the technical reasons for why the policy evaluated to unauthorized.
        /// </summary>
        public IEnumerable<string> UnauthorizedReasons { get; }

        public PolicyResult(IPolicy policy, bool isAuthorized, IEnumerable<string> unauthorizedReasons, string userFriendlyMessage = null)
        {
            this.Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            this.IsAuthorized = isAuthorized;
            this.UnauthorizedReasons = unauthorizedReasons ?? new string[] { };
            this.UserFriendlyMessage = userFriendlyMessage ?? "Unauthorized.";
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

        public static bool Equals(PolicyResult policyResult1, PolicyResult policyResult2)
        {
            if (policyResult1 == null && policyResult2 == null)
            {
                return true;
            }
            else if (policyResult1 == null || policyResult2 == null)
            {
                return true;
            }
            else
            {
                return policyResult1.Equals(policyResult2);
            }
        }

        public override int GetHashCode()
        {
            return HashCodeCalculator.Calculate(this.IsAuthorized, this.UnauthorizedReasons);
        }

        public override bool Equals(object obj)
        {
            var otherPolicyResult = obj as PolicyResult;

            return (otherPolicyResult != null && otherPolicyResult == this);
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

        public static bool operator ==(PolicyResult policyResultOperand1, PolicyResult policyResultOperand2)
        {
            return Equals(policyResultOperand1, policyResultOperand2);
        }

        public static bool operator !=(PolicyResult policyResultOperand1, PolicyResult policyResultOperand2)
        {
            return !Equals(policyResultOperand1, policyResultOperand2);
        }

        public static bool operator==(bool boolOperand, PolicyResult policyResultOperand)
        {
            if (policyResultOperand == null)
            {
                return false;
            }

            return boolOperand == policyResultOperand.IsAuthorized;
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
