using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.DTOs
{
    public class EventDto
    {
        public string Title { get; set; } = null!;
        public string? ImagePath { get; set; }

        public Guid CityId { get; set; }
        public string City { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public string Category { get; set; } = null!;

        public int MaxParticipants { get; set; }
        public int ParticipantsCount { get; set; }
    }
}
