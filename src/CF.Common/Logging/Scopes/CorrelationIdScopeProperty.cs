using CF.Common.Extensions;

namespace CF.Common.Logging.Scopes
{
    public class CorrelationIdScopeProperty : IScopeProperty<string>
    {
        public string Name => "CorrelationId";

        public string Value { get; }

        public CorrelationIdScopeProperty(string correlationId)
        {
            this.Value = correlationId.EnsureArgumentNotNullOrWhitespace(nameof(correlationId));
        }
    }
}
