using CF.Common.Utility;

namespace CF.Common.Authorization.Policies
{
    public class PolicyResult
    {
        /// <summary>
        /// Gets whether the policy evaluated to authorized.
        /// </summary>
        public bool IsAuthorized { get; }

        /// <summary>
        /// Gets a user-facing reason for a policy evaluating to unauthorized.
        /// IMPORTANT! Security-sensitive, and technical information should not be included here. Rather, if there
        /// are such details, they should be logged as required by the policy.
        /// </summary>
        public string UnauthorizedReason { get; }

        public PolicyResult(bool isAuthorized, string unauthorizedReason = null)
        {
            this.IsAuthorized = isAuthorized;
            this.UnauthorizedReason = unauthorizedReason ?? string.Empty;
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
            return HashCodeCalculator.Calculate(this.IsAuthorized, this.UnauthorizedReason);
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
            return PolicyResult.Equals(policyResultOperand1, policyResultOperand2);
        }

        public static bool operator !=(PolicyResult policyResultOperand1, PolicyResult policyResultOperand2)
        {
            return !PolicyResult.Equals(policyResultOperand1, policyResultOperand2);
        }

        public static bool operator==(bool boolOperand, PolicyResult policyResultOperand)
        {
            return boolOperand == policyResultOperand.IsAuthorized;
        }

        public static bool operator!=(bool boolOperand, PolicyResult policyResultOperand)
        {
            return boolOperand != policyResultOperand.IsAuthorized;
        }

        public static bool operator ==(PolicyResult policyResultOperand, bool boolOperand)
        {
            return boolOperand == policyResultOperand.IsAuthorized;
        }

        public static bool operator !=(PolicyResult policyResultOperand, bool boolOperand)
        {
            return boolOperand != policyResultOperand.IsAuthorized;
        }

    }
}
