using System;
using System.Threading.Tasks;

namespace IPWhistleblower.Services
{
    public interface ICacheService
    {
        T Get<T>(string cacheKey);
        T Set<T>(string cacheKey, T item, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null);
        void Remove(string cacheKey);
    }
}
