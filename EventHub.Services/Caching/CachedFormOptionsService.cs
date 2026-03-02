

using Microsoft.Extensions.Caching.Memory;

namespace EventHub.Services.Caching
{
    public abstract class CachedFormOptionsService<TDto>
    {
        private readonly IMemoryCache _cache;

        protected CachedFormOptionsService(IMemoryCache cache)
        {
            _cache = cache;
        }
        protected abstract string CacheKey { get; }

        public async Task<TDto> GetFormOptionsAsync(CancellationToken cancellation)
        {
            if (_cache.TryGetValue(CacheKey, out TDto cached))
                return cached;

            var result = await LoadOptionsAsync(cancellation);

            _cache.Set(CacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            return result;
        }

        protected abstract Task<TDto> LoadOptionsAsync(CancellationToken cancellationToken);

    }
}
