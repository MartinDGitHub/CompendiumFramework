using System;

namespace CF.Common.Logging.Scopes
{
    public class CorrelationIdScopeProperty : IScopeProperty<Guid>
    {
        public string Name => "CorrelationId";

        public Guid Value { get; } = Guid.NewGuid();
    }
}
