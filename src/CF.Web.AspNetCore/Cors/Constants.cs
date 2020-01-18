namespace CF.Web.AspNetCore.Cors
{
    public static class Constants
    {
        internal static class Policies
        {
            public static readonly string[] All = new string[] { Web, Api };

            public const string Web = "web";
            public const string Api = "api";
        }
    }
}
