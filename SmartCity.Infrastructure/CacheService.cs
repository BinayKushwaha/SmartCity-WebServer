using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SmartCity.Application;
using SmartCity.Application.Settings;

namespace SmartCity.Infrastructure
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(10);
        private readonly CacheSettings _cacheSettings;
        public CacheService(IMemoryCache memoryCache,
            IOptions<CacheSettings> cacheSettings)
        {
            _memoryCache = memoryCache;
            _cacheSettings = cacheSettings.Value;
        }

        public T? Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T? value);
            return value;
        }

        // ── Scenario 1 ───────────────────────────────────────────
        // Commission rates updated via SQL script
        // Cache MUST expire at exact time — no sliding
        // Fresh data fetched from DB after expiry automatically
        public void SetWithAbsoluteExpiry<T>(string key, T value, TimeSpan expiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                // ✅ expires at EXACTLY the given time
                AbsoluteExpirationRelativeToNow = expiration,

                SlidingExpiration = null,

                Priority = CacheItemPriority.High
            };

            options.RegisterPostEvictionCallback((k, v, reason, state) =>
            {
                Console.WriteLine(
                    $"[Cache Scenario 1] '{k}' expired after " +
                    $"{expiration.TotalMinutes} mins. " +
                    $"Reason: {reason}. " +
                    $"Fresh data will load from DB on next request.");
            });

            _memoryCache.Set(key, value, options);

            Console.WriteLine(
                $"[Cache Scenario 1] '{key}' cached for exactly " +
                $"{expiration.TotalMinutes} mins. No sliding.");
        }

        // ── Scenario 2 ───────────────────────────────────────────
        // Retail properties updated via API (Create/Update/Delete)
        // Cache manually removed on every write operation
        // Sliding + Absolute act as safety net only
        public void SetWithSlidingExpiry<T>(string key, T value)
        {
            var options = new MemoryCacheEntryOptions
            {
                // ✅ resets timer on every access
                // keeps cache alive while users are browsing
                SlidingExpiration = TimeSpan.FromMinutes(
                    _cacheSettings.SlidingExpiryMinutes),

                // ✅ hard limit — safety net
                // cache never lives longer than this
                // even if sliding keeps resetting
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                    _cacheSettings.DefaultAbsoluteExpiryMinutes),

                Priority = CacheItemPriority.Normal
            };

            options.RegisterPostEvictionCallback((k, v, reason, state) =>
            {
                Console.WriteLine(
                    $"[Cache Scenario 2] '{k}' evicted. " +
                    $"Reason: {reason}. " +
                    $"Sliding: {_cacheSettings.SlidingExpiryMinutes} mins | " +
                    $"Absolute: {_cacheSettings.DefaultAbsoluteExpiryMinutes} mins.");
            });

            _memoryCache.Set(key, value, options);

            Console.WriteLine(
                $"[Cache Scenario 2] '{key}' cached. " +
                $"Sliding: {_cacheSettings.SlidingExpiryMinutes} mins | " +
                $"Absolute: {_cacheSettings.DefaultAbsoluteExpiryMinutes} mins.");
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
