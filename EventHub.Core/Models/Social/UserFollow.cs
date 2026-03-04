using EventHub.Core.Models.Common;
using EventHub.Core.Models.Users;

namespace EventHub.Core.Models.Social
{
    public class UserFollow : BaseModel
    {
   

        public string FollowerId { get; set; } = null!;
        public UserProfile Follower { get; set; } = null!;

        public string FollowingId { get; set; } = null!;
        public UserProfile Following { get; set; } = null!;

    }
}
