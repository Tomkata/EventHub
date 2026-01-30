using EventHub.Core.Common;
using EventHub.Core.Common.Validation;
using EventHub.Core.Common.Validation.Messages;
using EventHub.Core.EventValidation;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Web.ViewModels.Events
{
    public class CreateEventViewModel : IEventFormViewModel
    {
        [Required(ErrorMessage = ValidationMessages.Required)]
        [StringLength(
       DataValidations.Event.TitleMaxLength,
       MinimumLength = DataValidations.Event.TitleMinLength)]
        public string Title { get; set; } = null!;

        [Required]
        [Range(
        DataValidations.Event.MaxParticipantsMin,
        DataValidations.Event.MaxParticipantsMax)]
        public int MaxParticipants { get; set; }

        [StringLength(
       DataValidations.Event.DescriptionMaxLength,
       MinimumLength = DataValidations.Event.DescriptionMinLength)]
        [Required(ErrorMessage = ValidationMessages.Required)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = ValidationMessages.Required)]
        [StringLength(DataValidations.Event.AddressMaxLength,
               MinimumLength = DataValidations.Event.AddressMinLength)]
        public string Address { get; set; } = null!;


        [Required(ErrorMessage = ValidationMessages.Required)]
        [FutureDate(ErrorMessage = EventMessages.InvalidStartDate)]
        public DateTime StartDate { get; set; }


        [Required(ErrorMessage = ValidationMessages.Required)]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = EventMessages.InvalidEndDate)]
        public DateTime EndDate { get; set; }


        [Required(ErrorMessage = ValidationMessages.Required)]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = ValidationMessages.Required)]
        public Guid LocationId { get; set; }
        public IFormFile Image { get; set; }

        public IEnumerable<DropdownOptionModel> Categories { get; set; }
        = Enumerable.Empty<DropdownOptionModel>();
        public IEnumerable<DropdownOptionModel> Locations { get; set; } 
        = Enumerable.Empty<DropdownOptionModel>();
    }
}
