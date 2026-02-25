namespace EventHub.Web.ViewModels.Organizers
{
    using System.ComponentModel.DataAnnotations;
    using EventHub.Core.Common.Validation.Organizer;
    using EventHub.Core.Enums.Organizer;

    public class ApplyOrganizerForm
    {
        [Required]
        [EmailAddress]  
        public  string Email { get; set; } = null!;

       
        public Status OrganizerState { get; set; }

        
        public string? UserId { get; set; } = null!;
        [StringLength(OrganizerValidation.MaximumNoteLength,
           MinimumLength = OrganizerValidation.MinimumNoteLength)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
