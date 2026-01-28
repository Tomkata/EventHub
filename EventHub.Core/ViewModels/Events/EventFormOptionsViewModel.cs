
namespace EventHub.Core.ViewModels.Events
using EventHub.Core.DTOs.Category;
using EventHub.Core.DTOs.Location;
using EventHub.Core.Models;

using EventHub.Core.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

public class EventFormOptionsViewModel
    {
        public IEnumerable<DropdownOptionModel> Categories { get;private set; }
        public IEnumerable<DropdownOptionModel> Locations { get; private    set; }

        public EventFormOptionsViewModel(IEnumerable<DropdownOptionModel> categories,
            IEnumerable<DropdownOptionModel> locations)
        {
            Categories = categories;
            Locations = locations;
        }
    }
}
