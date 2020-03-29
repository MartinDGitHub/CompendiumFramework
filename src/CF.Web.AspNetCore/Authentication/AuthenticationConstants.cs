using Microsoft.AspNetCore.Server.IISIntegration;

namespace CF.Web.AspNetCore.Authentication
{
    internal static class AuthenticationConstants
    {
        public static readonly string[] AuthenticationSchemes = new string[]
        {
            IISDefaults.AuthenticationScheme,
        };

        public const string DefaultAuthenticationScheme = IISDefaults.AuthenticationScheme;
    }
}
