using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.DTOs.UserProfile
{
    public class PublicUserProfileDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ProfileImagePath { get; set; }
        public int JoinedEventsCount { get; set; }

        public string Location { get; set; } = null!;
        public IEnumerable<string> Interests { get; set; } =
            new List<string>();



    }
}
