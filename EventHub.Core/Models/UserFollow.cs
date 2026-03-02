
namespace EventHub.Core.Models
{
    public class UserFollow
    {
        public UserFollow()
        {
            this.CreatedAt = DateTime.UtcNow;
        }

        public string FollowerId { get; set; } = null!;
        public UserProfile Follower { get; set; } = null!;

        public string FollowingId { get; set; } = null!;
        public UserProfile Following { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
