using EventHub.Core.DTOs;

namespace EventHub.Core.ViewModels.Events
{
    public class DetailedEventViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string ImagePath { get; set; } = "images/default-event.png";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public string CityName { get; set; } = null!;
        public Guid CityId { get; set; }
        public string Category { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public int MaxParticipants { get; set; }
        public int ParticipantsCount { get; set; }  
        public IEnumerable<ParticipantDto> Participants { get; set; } =
            Enumerable.Empty<ParticipantDto>();


    }
}
