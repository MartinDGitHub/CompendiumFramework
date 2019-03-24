using System;
using System.Collections.Generic;

namespace CF.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => source.ToHashSet(comparer: null);

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new HashSet<TSource>(source, comparer);
        }
    }
}
