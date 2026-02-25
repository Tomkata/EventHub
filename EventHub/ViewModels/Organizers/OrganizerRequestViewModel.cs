

namespace EventHub.Web.ViewModels.Organizers
{
    public class OrganizerRequestViewModel
    {
        public Guid Id { get; set; }
        public string Status => "Pending";
        public string UserId { get; set; } = null!;
        public string? Note { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
