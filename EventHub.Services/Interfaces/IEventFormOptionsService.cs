
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs;

    public interface IEventFormOptionsService   
    {
        public Task<EventFormOptionsDto> GetFormOptionsAsync(CancellationToken cancellation);
    }
}
    