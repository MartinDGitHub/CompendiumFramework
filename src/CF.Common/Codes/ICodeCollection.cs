using System;
using System.Collections.Generic;

namespace CF.Common.Codes
{
    public interface ICodeCollection<TCode, TId> : IEnumerable<TCode>
        where TCode : CodeBase<TCode, TId>
        where TId : struct, Enum
    {
        TCode this[TId id] { get; }

        TCode this[string code] { get; }
    }
}
