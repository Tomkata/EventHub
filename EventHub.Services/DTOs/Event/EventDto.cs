using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Services.DTOs.Event
{
    public class EventDto
    {
        public string Title { get; set; }
        public string? ImagePath { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public int MaxParticipants { get; set; }
        public int ParticipantsCount { get; set; }
    }
}
