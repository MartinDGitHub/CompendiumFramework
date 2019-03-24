using System;

namespace CF.WebBootstrap.Authorization
{
    internal interface IPolicyTypeFactory
    {
        Type GetPolicyType(string policyTypeName);
    }
}
