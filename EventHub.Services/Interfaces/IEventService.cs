
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Models;
    using EventHub.Services.Common;

    public interface  IEventService
    {
        Task<DetailedEventDto> GetByIdAsync(Guid id);
            

        Task CreateAsync(CreateEventDto dto, string requestingUserId);   

        Task UpdateAsync(Guid eventId,EditEventDto dto,string requestingUserId,bool isAdmin);

        Task DeleteAsync(Guid eventId, string requestingUserId, bool isAdmin);

        Task<PagedResult<EventDto>> GetEventsAsync(int pageNumber, int pageSize);

        Task<PagedResult<EventDto>> GetEventsByOrganizerIdAsync(
            string organizerId,
            int pageNumber,
            int pageSize);

        Task<EditEventDto> GetForEditAsync(Guid Id);

        Task<PagedResult<EventDto>> SearchBy(string? Tite,
                                DateTime? StartDate,
                                DateTime? EndDate,
                                Guid? LocationId,
                                Guid? CategoryId,
                                int pageNumber,
                                int pageSize);

      

    }
}
