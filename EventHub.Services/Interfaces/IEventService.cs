
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;

    public interface IEventService
    {
        Task<DetailedEventDto> GetByIdAsync(Guid id);


        Task CreateAsync(CreateEventDto dto);

        Task UpdateAsync(Guid id,EditEventDto dto);

        Task DeleteAsync(Guid id);

        Task<IEnumerable<EventDto>> GetEventsAsync();

        //SearchByFilter (later)

    }
}
