using EventHub.Services.Common;

namespace EventHub.Web.ViewModels.Events
{
    public class EventsIndexViewModel
    {
        public SearchEventByFilterViewModel Search { get; set; } = new();
        public PagedResult<EventListViewModel> Paged { get; set; } = null!;
    }
}
