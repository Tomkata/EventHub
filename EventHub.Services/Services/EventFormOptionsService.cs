

namespace EventHub.Services.Services
{
    using EventHub.Core.Common;
    using EventHub.Core.DTOs;
    using EventHub.Services.Interfaces;

    public class EventFormOptionsService : IEventFormOptionsService
    {
        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;

        public EventFormOptionsService(ICategoryService categoryService, ILocationService locationService)
        {
            this._categoryService = categoryService;
            this._locationService = locationService;
        }

        public async Task<EventFormOptionsDto> GetFormOptionsAsync(CancellationToken cancellation)
        {
                var categories = await _categoryService.GetCategoriesForDropdownAsync(cancellation);
                var locations = await _locationService.GetLocationsForDropdownAsync(cancellation);

                return new EventFormOptionsDto(
                    categories.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }),
                    locations.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name })
                );
        }
    }
}
    