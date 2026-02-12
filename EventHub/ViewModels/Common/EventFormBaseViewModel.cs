
namespace EventHub.Web.ViewModels.Common
{
using EventHub.Core.Common;
    public abstract class EventFormBaseViewModel
    {
        public IEnumerable<DropdownOptionModel> Categories { get; set; }
        =  new List<DropdownOptionModel>();

        public IEnumerable<DropdownOptionModel> Locations { get; set; }
                = new List<DropdownOptionModel>();


    }
}