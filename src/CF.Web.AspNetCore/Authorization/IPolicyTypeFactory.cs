using System;

namespace CF.Web.AspNetCore.Authorization
{
    internal interface IPolicyTypeFactory
    {
        Type GetPolicyType(string policyTypeName);
    }
}
