using System;

namespace CF.Common.Correlation
{
    internal class ScopedCorrelationIdProvider : IScopedCorrelationIdProvider
    {
        public Guid CorrelationGuid { get; } = Guid.NewGuid();
    }
}
