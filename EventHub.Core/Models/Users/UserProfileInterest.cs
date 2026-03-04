using EventHub.Core.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.Models.Users
{
    public class UserProfileInterest
    {
        public UserProfileInterest()
        {
            this.CreatedAt = DateTime.UtcNow;
        }
        [Key]
        public string UserId { get; set; }
        public UserProfile UserProfile { get; set; }

        public Guid InterestId { get; set; }
        public Interest Interest { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
