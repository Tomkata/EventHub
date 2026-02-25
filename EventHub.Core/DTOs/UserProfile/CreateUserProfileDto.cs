using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.DTOs.UserProfile
{
    public class CreateUserProfileDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid LocationId { get; set; }
        public string PhoneNumber { get; set; }
        public HashSet<Guid> InterestIds { get; set; } = new();
        public string ImagePath { get; set; } = null!;
    }
}
