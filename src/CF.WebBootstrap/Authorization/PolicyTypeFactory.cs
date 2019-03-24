using CF.Common.Exceptions;
using System;
using System.Collections.Concurrent;

namespace CF.WebBootstrap.Authorization
{
    internal class PolicyTypeFactory : IPolicyTypeFactory
    {
        private static readonly ConcurrentDictionary<string, Type> _policyTypeByPolicyName = new ConcurrentDictionary<string, Type>();

        public void RegisterPolicyType(string policyName, Type policyType)
        {
            if (string.IsNullOrWhiteSpace(policyName))
            {
                throw new ArgumentNullOrWhitespaceException(nameof(policyName));
            }

            if (policyType == null)
            {
                throw new ArgumentNullException(nameof(policyType));
            }

            _policyTypeByPolicyName.TryAdd(policyName, policyType);
        }

        public Type GetPolicyType(string policyName)
        {
            _policyTypeByPolicyName.TryGetValue(policyName, out Type policyType);
            if (policyType == null)
            {
                throw new ArgumentException($"No policy type is registered for policy with name [{policyName}].");
            }

            return policyType;
        }
    }
}
