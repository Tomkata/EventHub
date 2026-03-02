

namespace EventHub.Services.Services
{
    using EventHub.Core.Common;
    using EventHub.Core.DTOs;
    using EventHub.Services.Interfaces;
    using Microsoft.Extensions.Caching.Memory;

    public class SearchFormOptionsService : ISearchFormOptionsService
    {

        private const string CacheKey = "event-form-options";
        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;
        private readonly IMemoryCache _cache;

        public SearchFormOptionsService(ICategoryService categoryService, 
            ILocationService locationService,
            IMemoryCache cache)
        {
            this._categoryService = categoryService;
            this._locationService = locationService;
            this._cache = cache;
        }

        public async Task<EventFormOptionsDto> GetFormOptionsAsync(CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(CacheKey, out EventFormOptionsDto cached))
                return cached;

            var categories = await _categoryService.GetCategoriesForDropdownAsync(cancellationToken);
            var locations = await _locationService.GetLocationsForDropdownAsync(cancellationToken);

            var result = new EventFormOptionsDto(
                categories.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }),
                locations.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }));

            _cache.Set(CacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            return result;
        }
    }
}
