namespace EventHub.Core.Models
{
    public class EventParticipant
    {
        public EventParticipant()
        {
            this.JoinedAt = DateTime.UtcNow;
        }
        public Event Event { get; set; } = null!;
        public Guid EventId { get; set; }
        public string UserId { get; set; } = null!;
        public UserProfile UserProfile { get; set; } = null!;
        public DateTime JoinedAt { get; set; }
    }
}
