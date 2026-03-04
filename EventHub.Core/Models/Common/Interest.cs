

namespace EventHub.Core.Models
{
    public class Interest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InterestName { get; set; }

        public virtual ICollection<UserProfileInterest> UserProfileInterests { get; set; }
            = new List<UserProfileInterest>();
    }
}
