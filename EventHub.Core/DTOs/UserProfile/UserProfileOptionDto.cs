using EventHub.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.DTOs.UserProfile
{
    public class UserProfileOptionDto
    {

        public IEnumerable<DropdownOptionModel> Locations { get; }
        public IEnumerable<DropdownOptionModel> Interests { get; }


        public UserProfileOptionDto(IEnumerable<DropdownOptionModel> Locations,
                                     IEnumerable<DropdownOptionModel> Interests)
        {
            this.Locations = Locations;
            this.Interests = Interests;
        }


    }
}
