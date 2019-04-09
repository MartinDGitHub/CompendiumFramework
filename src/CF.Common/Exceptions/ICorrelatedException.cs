using System;

namespace CF.Common.Exceptions
{
    public interface ICorrelatedException
    {
        Guid? CorrelationGuid { get; }
    }
}