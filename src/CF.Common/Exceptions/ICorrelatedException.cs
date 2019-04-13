using System;

namespace CF.Common.Exceptions
{
    public interface ICorrelatedException
    {
        string CorrelationId { get; }
    }
}