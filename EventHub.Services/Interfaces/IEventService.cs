
namespace EventHub.Services.Interfaces
{
    using EventHub.Services.DTOs.Event;

    public interface IEventService
    {
        Task<DetailedEventDto> GetByIdAsync(Guid id);

        Task CreateAsync(CreateEventDto dto);

        Task UpdateAsync(Guid id,CreateEventDto dto);

        Task DeleteAsync(Guid id);

        Task<IEnumerable<EventDto>> GetEventsAsync();
    }
}
