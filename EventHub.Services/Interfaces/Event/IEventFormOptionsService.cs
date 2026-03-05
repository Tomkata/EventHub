namespace EventHub.Services.Interfaces.Event
{
    using EventHub.Core.DTOs;

    public interface IEventFormOptionsService   
    {
        public Task<EventFormOptionsDto> GetFormOptionsAsync(CancellationToken cancellation);
    }
}
    