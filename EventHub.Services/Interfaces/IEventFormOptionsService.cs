using EventHub.Core.ViewModels.Events;

namespace EventHub.Services.Interfaces

{
    public interface IEventFormOptionsService
    {
        public Task<EventFormOptionsViewModel> GetFormOptionsAsync();
    }
}
    