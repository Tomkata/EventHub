using EventHub.Core.Models.Users;

namespace EventHub.Core.Models.Common
{
    public class Interest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InterestName { get; set; }

        public virtual ICollection<UserProfileInterest> UserProfileInterests { get; set; }
            = new List<UserProfileInterest>();
    }
}
