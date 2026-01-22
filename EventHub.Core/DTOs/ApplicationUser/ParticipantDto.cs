using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.DTOs
{
    public class ParticipantDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; } = null!;
    }
}
