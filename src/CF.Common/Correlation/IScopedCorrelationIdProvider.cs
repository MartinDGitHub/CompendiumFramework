using System;

namespace CF.Common.Correlation
{
    public interface IScopedCorrelationIdProvider
    {
        Guid CorrelationGuid { get; }
    }
}
