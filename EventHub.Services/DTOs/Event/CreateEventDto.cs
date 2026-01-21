using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Services.DTOs.Event
{
    public class CreateEventDto
    {
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public int MaxParticipants { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public Guid LocationId { get; set; }

    }
}
