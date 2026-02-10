using EventHub.Core.Common;
using EventHub.Core.Common.Validation;
using EventHub.Core.Common.Validation.Messages;
using EventHub.Core.EventValidation;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Web.ViewModels.Events
{
    public class EditEventViewModel : IEventFormViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = ValidationMessages.Required)]
        [StringLength(
       DataValidations.Event.TitleMaxLength,
       MinimumLength = DataValidations.Event.TitleMinLength,
            ErrorMessage = EventMessages.TitleLength)]
        public string Title { get; set; } = null!;

        [StringLength(
       DataValidations.Event.DescriptionMaxLength,
       MinimumLength = DataValidations.Event.DescriptionMinLength,
            ErrorMessage = EventMessages.DescriptionLength)]
        public string Description { get; set; } = null!;

        [FutureDate(ErrorMessage = EventMessages.InvalidStartDate)]
        [Required(ErrorMessage = ValidationMessages.Required)]

        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = ValidationMessages.Required)]
        [DateGreaterThan(nameof(StartDate), ErrorMessage =  EventMessages.InvalidEndDate)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = ValidationMessages.Required)]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = ValidationMessages.Required)]
        public Guid LocationId { get; set; }

        [Required(ErrorMessage = ValidationMessages.Required)]
        [Range(
        DataValidations.Event.MaxParticipantsMin,
        DataValidations.Event.MaxParticipantsMax,
        ErrorMessage = EventMessages.InvalidParticipants)]
        public int MaxParticipants { get; set; }
        [Required(ErrorMessage = ValidationMessages.Required)]
        [StringLength(DataValidations.Event.AddressMaxLength,
            MinimumLength = DataValidations.Event.AddressMinLength)]
        public string Address { get; set; } = null!;

        public string? ExistingImagePath { get; set; }
        public IFormFile? NewImage { get; set; }


        public IEnumerable<DropdownOptionModel>? Categories { get; set; }
        public IEnumerable<DropdownOptionModel>? Locations { get; set; }
    }

}
