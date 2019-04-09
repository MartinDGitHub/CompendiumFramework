using System;

namespace CF.Common.Correlation
{
    public interface IScopedCorrelationGuidProvider
    {
        Guid CorrelationGuid { get; }
    }
}
