namespace EventHub.Core.DTOs.UserProfile
{
    public class EditUserProfileDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ProfileImagePath { get; set; } = null!;

        public Guid  LocationId { get; set; }
        public HashSet<Guid> SelectedInterestIds { get; set; } = new();

    }
}
