
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.Enums.Organizer;
    using Microsoft.Extensions.Configuration.UserSecrets;

    public interface IOrganizerService
    {
        Task ApplyForOrganizerAsync(OrganizerRequestFormDto formDto, string userId);

        Task ApproveUserToOrganizerAsync(string userId);

        Task DemoteOrganizerToUserAsync(string userId);

        Task RejectUserToOrganizerAsync(string userId);

        Task<Status> GetOrganizerStateAsync(string userId);

        Task<IEnumerable<PendingRequestForOrganizerDto>> GetAllPendingRequestsAsync();

        Task<bool> CanApplyAgainAsync(string userId);
    }
}
