using EventHub.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Models
{
    public class EventFormOptionsViewModel
    {
        public IEnumerable<DropdownOptionModel> Categories { get; }
        public IEnumerable<DropdownOptionModel> Locations { get; }

        public EventFormOptionsViewModel(
            IEnumerable<DropdownOptionModel> categories,
            IEnumerable<DropdownOptionModel> locations)
        {
            Categories = categories;
            Locations = locations;
        }
    }

}
