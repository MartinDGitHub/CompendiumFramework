using System;

namespace CF.Common.Logging.Scopes
{
    public class CorrelationGuidScopeProperty : IScopeProperty<Guid>
    {
        public string Name => "CorrelationGuid";

        public Guid Value { get; } = Guid.NewGuid();

        public CorrelationGuidScopeProperty()
        {
        }

        public CorrelationGuidScopeProperty(Guid correlationGuid)
        {
            if (correlationGuid == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(CorrelationGuidScopeProperty));
            }

            this.Value = correlationGuid;
        }
    }
}
