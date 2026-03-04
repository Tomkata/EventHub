namespace EventHub.Core.Models.Events
{
    using EventHub.Core.Common.Validation;
    using EventHub.Core.Common.Validation.Messages;
    using EventHub.Core.EventValidation;
    using EventHub.Core.Models.Common;
    using EventHub.Core.Models.Users;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Event  : BaseModel
    {
        public Event()
        {
            this.Id = Guid.NewGuid();
            this.EventParticipants = new HashSet<EventParticipant>();
        }



        [Key]
        [Comment("The Id of the Event")]
        public Guid Id { get; set; }



        [Comment("The Title of the Event")]
        [StringLength(DataValidations.Event.TitleMaxLength)]
        [Required]
        public string Title { get; set; } = null!;
            

        [Comment("The Image of the Event")]
        [Required]
        public string ImagePath { get; set; }



        [Comment("The Description of the Event")]
        public string? Description { get; set; }


        [Comment("The start date of the Event")]
        [FutureDate(ErrorMessage = EventMessages.InvalidStartDate)]
        [Required]
        public DateTime StartDate   { get; set; } 

        [Comment("The end date of the event")]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = EventMessages.InvalidEndDate)]
        [Required]
        public DateTime EndDate { get; set; }


        [Comment("The maximum participants capacity of the Event")]
        [Required]
        public int MaxParticipants { get; set; }



        [Required]
        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        [Comment("The Category of the Event")]
        [Required]
        public Category Category { get; set; }



        [Comment("The Address of the Event")]
        [Required]
        [StringLength(DataValidations.Event.AddressMaxLength)]
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
        public UserProfile OrganizerProfile { get; set; } = null!;
        public virtual ICollection<EventParticipant> EventParticipants { get; set; }
    }
}
