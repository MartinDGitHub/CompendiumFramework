using System;

namespace CF.Common.Correlation
{
    internal class ScopedCorrelationGuidProvider : IScopedCorrelationGuidProvider
    {
        public Guid CorrelationGuid { get; } = Guid.NewGuid();
    }
}
