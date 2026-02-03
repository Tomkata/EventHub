
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs.Organizer;

    public interface IOrganizerService
    {
        Task ApplyForOrganizer(OrganizerRequestFormDto formDto, string userId);

        Task ApproveUserToOrganizer(string userId);

        Task DemoteOrganizerToUser(string userId);

        Task RejectUserToOrganizer(string userId);
    }
}
