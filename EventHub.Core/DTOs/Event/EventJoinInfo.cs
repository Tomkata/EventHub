using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.DTOs.Event
{
    public class EventJoinInfo
    {
        public Guid Id { get; set; }
        public int MaxParticipantsCount { get; set; }
        public int ParticipantsCount { get; set; }
        public DateTime EndDate { get; set; }

        public string OrganizerId { get; set; } = null!;
    }
}
