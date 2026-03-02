
namespace EventHub.Services.Services
{

    using EventHub.Core.Common;
    using EventHub.Core.DTOs.UserProfile;
    using EventHub.Services.Caching;
    using EventHub.Services.Interfaces;
    using Microsoft.Extensions.Caching.Memory;

    public class UserProfileFormOptionsService :
        CachedFormOptionsService<UserProfileOptionDto>, IUserProfileFormOptionsService
    {
        private const string _cacheKey = "user-profile-form-options";
        private readonly IInterestsService _interestsService;
        private readonly ILocationService _locationService;

        public UserProfileFormOptionsService(
             IInterestsService interestsService,
             ILocationService locationService,
             IMemoryCache cache)
            : base(cache)
        {
            this._interestsService = interestsService;
            this._locationService = locationService;
        }

        protected override string CacheKey => _cacheKey;

        protected override async Task<UserProfileOptionDto> LoadOptionsAsync(CancellationToken cancellationToken)
        {
            var interests = await _interestsService.GetInterestsForDropDownAsync(cancellationToken);
            var locations = await _locationService.GetLocationsForDropdownAsync(cancellationToken);

            return new UserProfileOptionDto(
                    locations.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }),
                    interests.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }
                    ));
        }
    }
}
