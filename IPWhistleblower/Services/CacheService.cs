using Microsoft.Extensions.Caching.Memory;

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
                return cachedItem;

            return default;
        }

        public T Set<T>(string cacheKey, T item)
        {
            _cache.Set(cacheKey, item);
            return item;
        }

        public void Remove(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }
    }
}
