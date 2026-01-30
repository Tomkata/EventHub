using EventHub.Core.Common;

namespace EventHub.Web.ViewModels.Events;
public class EventFormOptionsViewModel
    {
        public IEnumerable<DropdownOptionModel> Categories { get;private set; }
        public IEnumerable<DropdownOptionModel> Locations { get; private    set; }

        public EventFormOptionsViewModel(IEnumerable<DropdownOptionModel> categories,
            IEnumerable<DropdownOptionModel> locations)
        {
            this.Categories = categories;
            this.Locations = locations;
        }
    }

