
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.Enums.Organizer;
    using EventHub.Core.Models;
    using EventHub.Services.Common;

    public interface IOrganizerService
    {
        Task ApplyForOrganizerAsync(
            OrganizerRequestFormDto formDto,
            string userId,
            CancellationToken cancellationToken);

        Task ApproveUserToOrganizerAsync(
            string userId,
            CancellationToken cancellationToken);

        Task DemoteOrganizerToUserAsync(
            string userId,
            CancellationToken cancellationToken);

        Task RejectUserToOrganizerAsync(
            string userId,
            CancellationToken cancellationToken);

        Task<Status> GetOrganizerStateAsync(
            string userId,
            CancellationToken cancellationToken);

        Task<PagedResult<PendingRequestForOrganizerDto>> GetAllPendingRequestsAsync(
            int pageNumber, 
            int pageSize,
            CancellationToken cancellation);

        Task<PagedResult<OrganizerRequestDto>> GetAllRequestsAsync(int pageNumber,
            int pageSize,
            CancellationToken cancellation,
            Status? status = null);

    }
}
