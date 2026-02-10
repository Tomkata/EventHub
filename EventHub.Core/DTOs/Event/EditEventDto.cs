namespace EventHub.Core.DTOs.Event
{
    public class EditEventDto
    {
        public string Title { get; set; }
        public int MaxParticipants { get; set; }
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid CategoryId { get; set; }
        public Guid LocationId { get; set; }
        public string OrganizerId { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
    }
}
