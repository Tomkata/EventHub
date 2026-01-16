namespace EventHub.Core.Models
{
    public class EventParticipant
    {
        public Guid EventId { get; set; }
        public string UserId { get; set; } = null!;
    }
}
