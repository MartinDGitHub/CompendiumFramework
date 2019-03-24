using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CF.Common.Authorization
{
    public interface IClaimsPrincipalProvider
    {
        ClaimsPrincipal User { get; }
    }
}
