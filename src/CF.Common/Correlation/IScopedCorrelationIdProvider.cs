using System;

namespace CF.Common.Correlation
{
    public interface IScopedCorrelationIdProvider
    {
        string CorrelationId { get; }
    }
}
