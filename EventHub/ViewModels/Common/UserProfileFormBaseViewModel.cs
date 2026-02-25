using EventHub.Core.Common;
using EventHub.Core.DTOs.Interest;
using EventHub.Core.DTOs.Location;

namespace EventHub.Web.ViewModels.Common
{
    public abstract class UserProfileFormBaseViewModel
    {
        public IEnumerable<DropdownOptionModel> Interests { get; set; } = new List<DropdownOptionModel>();
        public IEnumerable<DropdownOptionModel> Locations { get; set; } = new List<DropdownOptionModel>();
    }
}
