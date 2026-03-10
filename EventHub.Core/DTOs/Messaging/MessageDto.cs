

namespace EventHub.Core.DTOs.Messaging
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; } = null!;
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
    