namespace EventHub.Core.Models.Organizer
{
    using EventHub.Core.Enums.Organizer;
    using System.ComponentModel.DataAnnotations;
    using EventHub.Core.Common.Validation.Organizer;
    using EventHub.Core.Models.Common;

    public class OrganizerRequest : BaseModel
    {

        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Status Status { get; set; }

        [StringLength(OrganizerValidation.MaximumNoteLength)]
        public string? Note { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;
        public DateTime? LastRejectedAt { get; set; }
    }
}
