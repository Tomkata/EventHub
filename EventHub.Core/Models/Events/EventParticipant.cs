

namespace EventHub.Core.Models.Events
{
    using EventHub.Core.Models.Common;
    using EventHub.Core.Models.Users;
    public class EventParticipant : BaseModel
    {
        
        public Event Event { get; set; } = null!;
        public Guid EventId { get; set; }
        public string UserId { get; set; } = null!;
        public UserProfile UserProfile { get; set; } = null!;
    }
}
