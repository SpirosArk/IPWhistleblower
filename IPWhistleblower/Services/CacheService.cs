using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace IPWhistleblower.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string cacheKey)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedItem))
            {
                return cachedItem;
            }
            return default; // Return default value if item is not found
        }

        public T Set<T>(string cacheKey, T item, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null)
        {
            //var cacheEntryOptions = new MemoryCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            //    SlidingExpiration = slidingExpiration,
            //    Priority = CacheItemPriority.Normal
            //};

            _cache.Set(cacheKey, item);
            //_cache.Set(cacheKey, item, cacheEntryOptions);
            return item;
        }

        public void Remove(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }
    }
}
