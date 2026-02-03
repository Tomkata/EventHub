using EventHub.Core.Models;

namespace EventHub.Core.DTOs.Organizer
{
    public class OrganizerRequestFormDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Note { get; set; } 
    }
}
