using EventHub.Core.Common;

namespace EventHub.Core.DTOs.Interest
{
    public class InterestDto : DropdownOptionModel
    {
        public Guid Id  { get; set; }
        public string InterestName { get; set; }
    }
}
