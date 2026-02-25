

namespace EventHub.Core.DTOs
{
    public class DetailedEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public int MaxParticipants { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OrganizerName { get; set; } = null!;
        public string OrganizerId { get; set; } = null!;    
        public string City { get; set; } = null!;
        public Guid LocationId { get; set; }
        public string Address { get; set; } = null!;
        public int ParticipantsCount { get; set; }
        public List<ParticipantDto>  ParticipantList { get; set; } = new List<ParticipantDto>();
    }
}
