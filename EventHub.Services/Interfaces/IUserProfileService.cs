

using EventHub.Core.DTOs.UserProfile;
using EventHub.Core.Models;

namespace EventHub.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task EnsureProfileExistsAsync(
            string userId,
            CancellationToken cancellationToken);
        Task CreateAsync(
            string userid,
            CreateUserProfileDto dto,
            CancellationToken cancellationToken);
        Task<bool> ExistsAsync(
            string userId,
            CancellationToken cancellationToken);

        Task<UserNavInfoDto> GetUserNavInfoAsync(
            string userId,
            CancellationToken cancellationToken);

        Task<DetailUserProfileDto?> GetDetailAsync(
            string userId,
            CancellationToken cancellationToken);

        Task<PublicUserProfileDto?> GetPublicDetailAsync(
            string userId,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            string userId, 
            EditUserProfileDto dto,
            CancellationToken cancellation);

        Task<bool> HasProfileAsync(
            string userId,
            CancellationToken cancellationToken);
        Task<HashSet<Guid>> GetSelectedInterestIdsAsync(
            IEnumerable<string> interestNames,
            CancellationToken cancellationToken);

        Task<bool> IsValidInterests(
            HashSet<Guid> interestDto,
            CancellationToken cancellationToken);


    }
}
