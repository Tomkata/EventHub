

namespace EventHub.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using EventHub.Core.Common.Validation.Messages;
    public class OrganizerApplyFormViewModel
    {
        [Required]
        [EmailAddress]
        public  string Email { get; set; } = null!;

        [StringLength(OrganizerMessages.MaximumNoteLength, 
            MinimumLength = OrganizerMessages.MinimumNoteLength)]
        public string? Note { get; set; }
    }
}
