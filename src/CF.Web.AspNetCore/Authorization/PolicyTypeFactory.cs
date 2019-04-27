using CF.Common.Extensions;
using System;
using System.Collections.Concurrent;

namespace CF.Web.AspNetCore.Authorization
{
    internal class PolicyTypeFactory : IPolicyTypeFactory
    {
        private static readonly ConcurrentDictionary<string, Type> _policyTypeByPolicyName = new ConcurrentDictionary<string, Type>();

        public void RegisterPolicyType(string policyName, Type policyType)
        {
            policyName.EnsureArgumentNotNullOrWhitespace(nameof(policyName));

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
