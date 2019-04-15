using System;
using System.Threading.Tasks;

namespace CF.Common.Caching
{
    public interface ILocalCache
    {
        string GetCacheKeyForType<TType>(string key);

        T GetOrAdd<T>(string key, Func<T> addItemFunc, LocalCacheOptions options);

        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> addItemFunc, LocalCacheOptions options);

        void Remove(string key);
    }
}
