using CF.Common.Exceptions;

namespace CF.Common.Logging.Scopes
{
    public class ContextTypeNameScopeProperty : IScopeProperty<string>
    {
        public string ContextTypeName { get; }

        public ContextTypeNameScopeProperty(string contextTypeName)
        {
            if (string.IsNullOrWhiteSpace(contextTypeName))
            {
                throw new ArgumentNullOrWhitespaceException(nameof(contextTypeName));
            }

            this.ContextTypeName = contextTypeName;
        }

        public string Name => "ContextTypeName";

        public string Value => this.ContextTypeName;
    }
}
