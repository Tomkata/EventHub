

namespace EventHub.Services.Services
{
    using EventHub.Core.Common;
    using EventHub.Core.DTOs;
    using EventHub.Core.Models;
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

        public async Task<EventFormOptionsDto> GetFormOptionsAsync()
        {
            var categories = await _categoryService.GetCategoriesForDropdownAsync();
            var locations = await _locationService.GetLocationsForDropdownAsync();

            return new EventFormOptionsDto(
                categories.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }),
                locations.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.City })
            );
        }
    }
}
    