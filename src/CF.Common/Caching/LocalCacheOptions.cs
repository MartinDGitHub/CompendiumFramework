using System;

namespace CF.Common.Caching
{
    public class LocalCacheOptions
    {
        public DateTimeOffset? AbsoluteExpiry { get; set; }

        public TimeSpan? SlidingExpiry { get; set; }
    }
}
