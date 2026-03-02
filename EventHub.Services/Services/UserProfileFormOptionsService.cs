
namespace EventHub.Services.Services
{

    using EventHub.Core.Common;
    using EventHub.Core.DTOs.UserProfile;
    using EventHub.Services.Interfaces;
    using Microsoft.Extensions.Caching.Memory;

    public class UserProfileFormOptionsService : IUserProfileFormOptionsService
    {
        private const string CacheKey = "user-profile-options-form";
        private readonly IInterestsService _interestsService;
        private readonly ILocationService _locationService;
        private readonly IMemoryCache _cache;

        public UserProfileFormOptionsService(IInterestsService interestsService,
                                            ILocationService locationService,
                                            IMemoryCache cache)
        {
            this._interestsService = interestsService;
            this._locationService = locationService;
            this._cache = cache;
        }
        public async Task<UserProfileOptionDto> GetFormOptionsAsync(CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(CacheKey, out UserProfileOptionDto cached))
                return cached;

            var interests = await _interestsService.GetInterestsForDropDownAsync(cancellationToken);
            var locations = await _locationService.GetLocationsForDropdownAsync(cancellationToken);

            var result = new UserProfileOptionDto(
                 locations.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name }),
                 interests.Select(x => new DropdownOptionModel { Id = x.Id, Name = x.Name })
                );

            _cache.Set(CacheKey,result,new MemoryCacheEntryOptions
            {
                 AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            return result;
        }
    }
}
