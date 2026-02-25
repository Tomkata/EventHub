using EventHub.Web.ViewModels.Common;

namespace EventHub.Web.ViewModels.Events
{
    public class SearchEventByFilterViewModel : EventFormBaseViewModel
    {
        public string? Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? LocationId { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
