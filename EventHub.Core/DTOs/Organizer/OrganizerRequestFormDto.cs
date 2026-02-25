namespace EventHub.Core.DTOs.Organizer
{
    public class OrganizerRequestFormDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }


    }
}
