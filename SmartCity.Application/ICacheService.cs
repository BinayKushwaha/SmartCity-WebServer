namespace SmartCity.Application
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void SetWithAbsoluteExpiry<T>(string key, T value, TimeSpan expiration);
        void SetWithSlidingExpiry<T>(string key, T value);
        void Remove(string key);
    }
}
