using System;

namespace CF.Common.Codes
{
    public interface IIdCode<TId>
        // Constrain the ID to as close to an enum as possible.
        // An ID enum must be positively enforced on specification.
        where TId : struct, IComparable 
    {
        TId Id { get; }
    }
}
