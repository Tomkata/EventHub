
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs.Organizer;

    public interface IOrganizerService
    {
        Task ApplyForOrganizerAsync(OrganizerRequestFormDto formDto, string userId);

        Task ApproveUserToOrganizerAsync(string userId);

        Task DemoteOrganizerToUserAsync(string userId);

        Task RejectUserToOrganizerAsync(string userId);
    }
}
