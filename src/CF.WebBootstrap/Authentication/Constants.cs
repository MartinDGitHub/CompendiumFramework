using Microsoft.AspNetCore.Server.IISIntegration;

namespace CF.WebBootstrap.Authentication
{
    internal static class Constants
    {
        public static readonly string[] AuthenticationSchemes = new string[]
        {
            // Any changes to this set should be reflected in AuthenticationIServiceCollectionExtensions.
            IISDefaults.AuthenticationScheme,
        };

        public static readonly string DefaultAuthenticationScheme = IISDefaults.AuthenticationScheme;
    }
}
