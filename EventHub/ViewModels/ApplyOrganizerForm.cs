

namespace EventHub.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using EventHub.Core.Common.Validation.Messages;
    using EventHub.Core.Enums.Organizer;

    public class ApplyOrganizerForm
    {
        [Required]
        [EmailAddress]  
        public  string Email { get; set; } = null!;

       
        public Status OrganizerState { get; set; }

        [StringLength(OrganizerMessages.MaximumNoteLength,
           MinimumLength = OrganizerMessages.MinimumNoteLength)]
        public string? Note { get; set; }
    }
}
