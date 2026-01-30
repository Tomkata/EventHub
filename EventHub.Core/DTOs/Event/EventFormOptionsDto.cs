using EventHub.Core.Common;

namespace EventHub.Core.DTOs
{
    public class EventFormOptionsDto
    {
        public IEnumerable<DropdownOptionModel> Categories { get; }
        public IEnumerable<DropdownOptionModel> Locations { get; }

        public EventFormOptionsDto(
            IEnumerable<DropdownOptionModel> categories,
            IEnumerable<DropdownOptionModel> locations)
        {
            Categories = categories;
            Locations = locations;
        }
    }
}
