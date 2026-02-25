
using EventHub.Core.Common;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.UserProfile;
using EventHub.Services.Interfaces;

namespace EventHub.Services.Services
{
    public class UserProfileFormOptionsService : IUserProfileFormOptionsService
    {
        private readonly IInterestsService _interestsService;
        private readonly ILocationService _locationService;

        public UserProfileFormOptionsService(IInterestsService interestsService,
                                            ILocationService locationService)
        {
            this._interestsService = interestsService;
            this._locationService = locationService;
        }
        public async Task<UserProfileOptionDto> GetFormOptionsAsync()
        {
            var interests = await _interestsService.GetInterestsForDropDownAsync();
            var locations = await _locationService.GetLocationsForDropdownAsync();

            return new UserProfileOptionDto(
                 locations.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }),
                 interests.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name })
                );
        }
    }
}
