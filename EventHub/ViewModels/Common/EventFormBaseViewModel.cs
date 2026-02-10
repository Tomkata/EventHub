using EventHub.Core.Common;

namespace EventHub.Web.ViewModels.Common
{
    public abstract class EventFormBaseViewModel
    {
        public IEnumerable<DropdownOptionModel> Categories { get; set; }
        public IEnumerable<DropdownOptionModel> Locations { get; set; }

    }
}
