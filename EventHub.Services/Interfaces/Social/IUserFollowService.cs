
using EventHub.Core.DTOs.Social;

namespace EventHub.Services.Interfaces.Social
{
    public interface IUserFollowService
    {
        public Task Follow(string followerId, string followingId);
        public Task Unfollow(string followerId, string followingId);

        public Task<SocialUserPreviewDto> GetFollowersAsync(string userId);
        public Task<SocialUserPreviewDto> GetFollingsAsync(string userId);

    }
}
