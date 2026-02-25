namespace EventHub.Core.DTOs
{
    public class ParticipantDto
    {
        public string UserId { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? ProfileImagePath { get; set; }
    }
}
