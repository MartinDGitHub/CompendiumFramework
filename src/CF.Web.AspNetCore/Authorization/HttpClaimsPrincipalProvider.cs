using CF.Common.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace CF.Web.AspNetCore.Authorization
{
    internal class HttpClaimsPrincipalProvider : IClaimsPrincipalProvider
    {
        private IHttpContextAccessor _httpContextAccessor;

        public ClaimsPrincipal User =>
            // Always go after the current claims principal on the HTTP context instead of 
            // holding a reference to the claims principal itself, which could become stale.
            this._httpContextAccessor?.HttpContext?.User as ClaimsPrincipal ?? throw new InvalidOperationException("Could not obtain a claims principal from the supplied HTTP context.");

        public HttpClaimsPrincipalProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
    }
}
