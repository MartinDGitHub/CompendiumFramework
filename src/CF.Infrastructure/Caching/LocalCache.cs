using CF.Common.Caching;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace CF.Infrastructure.Caching
{
    internal class LocalCache : ILocalCache
    {
        private readonly IAppCache _appCache;

        public string GetCacheKeyForType<TType>(string key) => $"CacheKey-Type[{typeof(TType).FullName}]-Key[{key}]";

        public LocalCache(IAppCache appCache)
        {
            this._appCache = appCache ?? throw new ArgumentNullException(nameof(appCache));
        }

        public T GetOrAdd<T>(string key, Func<T> addItemFunc, LocalCacheOptions options)
        {
            var entryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiry,
                SlidingExpiration = options.SlidingExpiry,
            };

            return this._appCache.GetOrAdd(key, addItemFunc, entryOptions);
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> addItemFunc, LocalCacheOptions options)
        {
            var entryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiry,
                SlidingExpiration = options.SlidingExpiry,
            };

            return await this._appCache.GetOrAddAsync(key, addItemFunc, entryOptions);
        }

        public void Remove(string key)
        {
            this._appCache.Remove(key);
        }
    }
}
