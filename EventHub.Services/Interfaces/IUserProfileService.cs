

using EventHub.Core.DTOs.UserProfile;

namespace EventHub.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task EnsureProfileExistsAsync(string userId);
        Task CreateAsync(string userid,CreateUserProfileDto dto);
        Task<bool> ExistsAsync(string userId);

        Task<UserNavInfoDto> GetUserNavInfoAsync(string userId);

        Task<DetailUserProfileDto?> GetDetailAsync(string userId);

        Task<PublicUserProfileDto?> GetPublicDetailAsync(string userId);

        Task UpdateAsync(string userId, EditUserProfileDto dto);

        Task<bool> HasProfileAsync(string userId);
        Task<HashSet<Guid>> GetSelectedInterestIdsAsync(IEnumerable<string> interestNames);

    }
}
