using EventHub.Core.Common.Validation;
using EventHub.Core.Common.Validation.Messages;
using EventHub.Core.EventValidation;
using EventHub.Core.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventHub.Core.ViewModels.Events
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
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DateGreaterThan(nameof(StartDate), ErrorMessage =  EventMessages.InvalidEndDate)]
        public DateTime? EndDate { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Guid LocationId { get; set; }

        [Required]
        [Range(
        DataValidations.Event.MaxParticipantsMin,
        DataValidations.Event.MaxParticipantsMax,
        ErrorMessage = EventMessages.InvalidParticipants)]
        public int MaxParticipants { get; set; }
        [Required]
        [StringLength(DataValidations.Event.AddressMaxLength,
            MinimumLength = DataValidations.Event.AddressMinLength)]
        public string Address { get; set; } = null!;

        public string? ExistingImagePath { get; set; }
        public IFormFile? NewImage { get; set; }

        public IEnumerable<DropdownOptionModel>? Categories { get; set; }
        public IEnumerable<DropdownOptionModel>? Locations { get; set; }
    }

}
