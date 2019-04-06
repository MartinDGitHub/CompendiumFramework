using CF.Common.Exceptions;

namespace CF.Common.Extensions
{
    public static class StringExtensions
    {
        public static string EnsureArgumentNotNullOrWhitespace(this string source, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullOrWhitespaceException(argumentName);
            }

            return source;
        }
    }
}