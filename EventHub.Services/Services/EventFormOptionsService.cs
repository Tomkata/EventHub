

namespace EventHub.Services.Services
{
    using EventHub.Core.ViewModels.Common;
    using EventHub.Core.ViewModels.Events;
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

        public async Task<EventFormOptionsViewModel> GetFormOptionsAsync()
        {
            var categories = await _categoryService.GetCategoriesForDropdownAsync();
            var categoriesModel = categories
                .Select(x => new DropdownOptionModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();



            var locations = await _locationService.GetLocationsForDropdownAsync();
            var locationsModel = locations
                .Select(x => new DropdownOptionModel
                {
                    Id = x.Id,
                    Name = x.City
                })
                .ToList();

            return new EventFormOptionsViewModel( categoriesModel, locationsModel);

        }
    }
}
    