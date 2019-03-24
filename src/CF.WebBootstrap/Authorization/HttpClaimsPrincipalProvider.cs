using CF.Common.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace CF.WebBootstrap.Authorization
{
    internal class HttpClaimsPrincipalProvider : IClaimsPrincipalProvider
    {
        private IHttpContextAccessor _httpContextAccessor;

        public ClaimsPrincipal User => 
            this._httpContextAccessor?.HttpContext?.User as ClaimsPrincipal ?? throw new InvalidOperationException("Could not obtain a claims principal from the supplied HTTP context.");


        public HttpClaimsPrincipalProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
    }
}
