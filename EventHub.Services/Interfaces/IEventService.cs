
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Models;
    using EventHub.Services.Common;

    public interface  IEventService
    {
        Task<DetailedEventDto> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);
            

        Task CreateAsync(
            CreateEventDto dto,
            string requestingUserId, 
            CancellationToken cancellation);   

        Task UpdateAsync(Guid eventId,
            EditEventDto dto,
            string requestingUserId,
            bool isAdmin,
            CancellationToken cancellation);

        Task DeleteAsync(
            Guid eventId, 
            string requestingUserId, 
            bool isAdmin,
            CancellationToken cancellationToken);

        Task<PagedResult<EventDto>> GetEventsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<PagedResult<EventDto>> GetEventsByOrganizerIdAsync(
            string organizerId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellation);

        Task<EditEventDto> GetForEditAsync(
            Guid Id,
            CancellationToken cancellationToken);

        Task<PagedResult<EventDto>> SearchBy(string? Tite,
                                DateTime? StartDate,
                                DateTime? EndDate,
                                Guid? LocationId,
                                Guid? CategoryId,
                                int pageNumber,
                                int pageSize,
                                CancellationToken cancellationToken);
    }
}
