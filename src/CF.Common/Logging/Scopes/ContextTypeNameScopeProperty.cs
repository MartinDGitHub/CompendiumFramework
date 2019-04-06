using CF.Common.Exceptions;
using CF.Common.Extensions;

namespace CF.Common.Logging.Scopes
{
    public class ContextTypeNameScopeProperty : IScopeProperty<string>
    {
        public string ContextTypeName { get; }

        public ContextTypeNameScopeProperty(string contextTypeName)
        {
            this.ContextTypeName = contextTypeName.EnsureArgumentNotNullOrWhitespace(nameof(contextTypeName));
        }

        public string Name => "ContextTypeName";

        public string Value => this.ContextTypeName;
    }
}
