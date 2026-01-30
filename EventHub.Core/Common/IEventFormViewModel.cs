

namespace EventHub.Core.Common
{
    public interface IEventFormViewModel
    {
        public IEnumerable<DropdownOptionModel>? Categories { get; set; }
        public IEnumerable<DropdownOptionModel>? Locations { get; set; }
    }
}
