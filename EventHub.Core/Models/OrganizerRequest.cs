

namespace EventHub.Core.Models
{
using EventHub.Core.Enums.Organizer;
using System.ComponentModel.DataAnnotations;
    using Common.Validation.Messages;
    using Microsoft.EntityFrameworkCore;
    using System.Runtime.CompilerServices;

    public class OrganizerRequest
    {
        public OrganizerRequest()
        {
            this.CreatedAt = DateTime.UtcNow;
        }
        
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Status Status { get; set; }
        [StringLength(OrganizerMessages.MaximumNoteLength)]
        public string? Note { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastRejectedAt { get; set; }

    }
}
