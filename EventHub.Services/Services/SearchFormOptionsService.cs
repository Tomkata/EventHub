

namespace EventHub.Services.Services
{
    using EventHub.Core.Common;
    using EventHub.Core.DTOs;
    using EventHub.Services.Interfaces;

    public class SearchFormOptionsService : ISearchFormOptionsService
    {


        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;

        public SearchFormOptionsService(ICategoryService categoryService, ILocationService locationService)
        {
            this._categoryService = categoryService;
            this._locationService = locationService;
        }

        public async Task<EventFormOptionsDto> GetFormOptionsAsync(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetCategoriesForDropdownAsync(cancellationToken);
            var locations = await _locationService.GetLocationsForDropdownAsync(cancellationToken);

            return new EventFormOptionsDto(
                categories.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }),
                locations.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name })
            );
        }
    }
}
