using System;

namespace CF.Common.Codes
{
    public interface IIdCode<TId>
        where TId : Enum
    {
        TId Id { get; }
    }
}
