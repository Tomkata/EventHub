
namespace EventHub.Core.DTOs.Social
{
    public class SocialUserPreviewDto
    {
        public string UserId { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string ProfileImagePath { get; set; } = null!;

        public string Location { get; set; } = null!;
        public bool IsFollowedByCurrentUser { get; set; }
    }
}
