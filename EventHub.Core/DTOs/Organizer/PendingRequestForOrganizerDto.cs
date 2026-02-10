
namespace EventHub.Core.DTOs.Organizer
{

    public class PendingRequestForOrganizerDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
