
using EventHub.Core.DTOs.Social;
using EventHub.Core.Models.Social;
using EventHub.Services.Common;

namespace EventHub.Services.Interfaces.Social
{
    public interface IUserFollowService
    {
        public Task Follow(string followerId, string followingId,CancellationToken cancellation);
        public Task Unfollow(string followerId, string followingId, CancellationToken cancellation);

        public Task<PagedResult<SocialUserPreviewDto>> GetFollowersAsync(
            string userId, 
            int pageNumber,
            int pageSize,
            CancellationToken cancellation
            );

        public Task<PagedResult<SocialUserPreviewDto>> GetFollingsAsync(
            string userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellation);

        public Task<bool> IsFollowingAsync(string followerId, string followingId, CancellationToken cancellation);

    }
}
