using Microsoft.AspNetCore.Http;
using System;
using System.Text.RegularExpressions;

namespace CF.Web.AspNetCore.Extensions
{
    internal static class HttpRequestExtensions
    {
        private const string XRequestedWithHeader = "X-Requested-With";
        private const string XRequestedWithHeaderAjaxValue = "XMLHttpRequest";
        private const string ApiPathRegexPattern = @"^/api/v\d+(\.\d+)?/.*";

        public static bool IsAjaxRequest(this HttpRequest httpRequest)
        {
            return string.Equals(httpRequest?.Headers?[XRequestedWithHeader], XRequestedWithHeaderAjaxValue, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsApiRequest(this HttpRequest httpRequest)
        {
            return Regex.IsMatch(httpRequest?.Path ?? string.Empty, ApiPathRegexPattern, RegexOptions.IgnoreCase);
        }
    }
}
