using System.Reflection.PortableExecutable;
using System.Threading.Channels;

namespace EventHub.Core.ViewModels.Events
{
    public class EventListViewModel
    {
        public string Title { get; set; } = null!;
        public string ImagePath { get; set; } = "/images/default-event.png";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CityName { get; set; } = null!;
        public Guid CityId { get; set; }
        public string Category { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public bool IsFull => ParticipantsCount >= MaxParticipants;
        public int MaxParticipants { get; set; }
        public int ParticipantsCount { get; set; }
    }
}
