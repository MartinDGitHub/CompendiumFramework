using System;

namespace CF.Common.Exceptions
{
    public class ArgumentNullOrWhitespaceException : ArgumentException
    {
        public ArgumentNullOrWhitespaceException(string paramName)
            : base($"String value cannot be {(paramName == null ? "null" : "whitespace")}.", paramName)
        {
        }

    }
}
