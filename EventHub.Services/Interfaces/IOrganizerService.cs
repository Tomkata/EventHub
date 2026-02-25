
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.Enums.Organizer;
    using EventHub.Core.Models;
    using EventHub.Services.Common;

    public interface IOrganizerService
    {
        Task ApplyForOrganizerAsync(OrganizerRequestFormDto formDto, string userId);

        Task ApproveUserToOrganizerAsync(string userId);

        Task DemoteOrganizerToUserAsync(string userId);

        Task RejectUserToOrganizerAsync(string userId);

        Task<Status> GetOrganizerStateAsync(string userId);

        Task<PagedResult<PendingRequestForOrganizerDto>> GetAllPendingRequestsAsync(int pageNumber, int pageSize);

        Task<PagedResult<OrganizerRequestDto>> GetAllRequestsAsync(int pageNumber,int pageSize, Status? status = null);

    }
}
