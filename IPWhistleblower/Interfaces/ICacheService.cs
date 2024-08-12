namespace IPWhistleblower.Services
{
    public interface ICacheService
    {
        T Get<T>(string cacheKey);
        T Set<T>(string cacheKey, T item);
        void Remove(string cacheKey);
    }
}
