
namespace EventHub.Web.ViewModels.Events
{
    using EventHub.Core.Common;

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

}
