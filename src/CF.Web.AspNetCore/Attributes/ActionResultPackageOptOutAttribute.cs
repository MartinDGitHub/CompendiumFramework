using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Web.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class ActionResultPackageOptOutAttribute : Attribute
    {
    }
}
