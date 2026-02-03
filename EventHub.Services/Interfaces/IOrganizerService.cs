using EventHub.Core.DTOs.Organizer;

namespace EventHub.Services.Interfaces
{
    public interface IOrganizerService
    {
        Task ApplyForOrganizer(OrganizerRequestFormDto formDto, string userId);

        Task ApproveUserToOrganizer(string userId);

        Task DemoteOrganizerToUser(string userId);
    }
}
