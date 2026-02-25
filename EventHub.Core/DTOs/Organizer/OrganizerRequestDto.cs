

namespace EventHub.Core.DTOs.Organizer
{
    using EventHub.Core.Enums.Organizer;
    using System;
    public class OrganizerRequestDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;

        public string UserId { get; set; } = null!; 

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public Status  Status { get; set; }
    }
}
