

namespace EventHub.Services.Services
{
    using EventHub.Core.Common;
    using EventHub.Core.DTOs;
    using EventHub.Services.Caching;
    using EventHub.Services.Interfaces;
    using Microsoft.Extensions.Caching.Memory;

    public class EventFormOptionsService :
          CachedFormOptionsService<EventFormOptionsDto>, IEventFormOptionsService
    {

        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;
        private const string _cacheKey = "event-form-options";
        public EventFormOptionsService(
         ICategoryService categoryService,
         ILocationService locationService,
         IMemoryCache cache)
         : base(cache)
        {
            _categoryService = categoryService;
            _locationService = locationService;
        }

        protected override string CacheKey => _cacheKey;

        protected override async Task<EventFormOptionsDto> LoadOptionsAsync(CancellationToken cancellationToken)
        {
            var locations = await _locationService.GetLocationsForDropdownAsync(cancellationToken);
            var categories = await _categoryService.GetCategoriesForDropdownAsync(cancellationToken);

            return  new EventFormOptionsDto(
                 categories.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }),
                 locations.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name})
                );
        }
    }
}
    