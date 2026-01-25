

namespace EventHub.Core.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;
    using EventHub.Core.EventValidation;
    using Microsoft.EntityFrameworkCore;

    public class Event
    {
        public Event()
        {
            this.Id = Guid.NewGuid();
            this.CreatedAt = DateTime.UtcNow;
            this.EventParticipants = new HashSet<EventParticipant>();
        }



        [Key]
        [Comment("The Id of the Event")]
        public Guid Id { get; set; }



        [Comment("The Title of the Event")]
        [StringLength(300, MinimumLength = 3)]
        [Required]
        public string Title { get; set; } = null!;


        [Comment("The Image of the Event")]
        [FileExtensions(Extensions = "jpg,png,webp,jpeg");
        [Required]
        public string? ImagePath { get; set; }



        [Comment("The Description of the Event")]
        public string? Description { get; set; }



        [Comment("The date when Event was created")]
        [Required]      
        public DateTime CreatedAt { get; set; }



        [Comment("The start date of the Event")]
        [FutureDateAttribute(ErrorMessage ="Start date must be in the future")]
        [Required]
        public DateTime StartDate { get; set; }

        [Comment("The end date of the event")]
        [Required]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }


        [Comment("The maximum participants capacity of the Event")]
        [Required]
        [Range(1, 1000)]
        public int MaxParticipants { get; set; }



        [Required]
        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        [Comment("The Category of the Event")]
        [Required]
        public Category Category { get; set; }



        [Comment("The Address of the Event")]
        [Required]
        [StringLength(200)]
        public string Address { get; set; }



        [Required]
        [ForeignKey(nameof(Location))]
        public Guid LocationId { get; set; }

        [Required]
        [Comment("The Location of the Event")]
        public Location Location { get; set; }


        [Comment("The Organizer of the event")]
        [Required]
        public string OrganizerId { get; set; }


        public virtual ICollection<EventParticipant> EventParticipants { get; set; }
    }
}
