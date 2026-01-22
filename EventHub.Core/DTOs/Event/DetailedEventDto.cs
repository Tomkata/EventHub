

namespace EventHub.Core.DTOs
{
    public class DetailedEventDto
    {
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int MaxParticipants { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public string OrganizerName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Address { get; set; } = null!;        
        public List<ParticipantDto>  ParticipantList { get; set; } = new List<ParticipantDto>();
    }
}
