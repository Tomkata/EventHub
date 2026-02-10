
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Models;

    public interface  IEventService
    {
        Task<DetailedEventDto> GetByIdAsync(Guid id);
            

        Task CreateAsync(CreateEventDto dto);   

        Task UpdateAsync(Guid eventId,EditEventDto dto,string requestingUserId,bool isAdmin);

        Task DeleteAsync(Guid eventId, string requestingUserId, bool isAdmin);

        Task<IEnumerable<EventDto>> GetEventsAsync();

        Task<IEnumerable<EventDto>> GetEventsByOrganizerIdAsync(string organizerId);

        Task<UserBasicInfo> GetOrganizerAsync(string organizerId);

        //SearchByFilter (later)

    }
}
