using EventHub.Core.EventValidation;
using EventHub.Core.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventHub.Core.ViewModels.Events
{
    public class    EditEventViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        [FutureDate(ErrorMessage = "Start date must be in the future")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = "End date must be after start date")]
        public DateTime? EndDate { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Guid LocationId { get; set; }

        [Required]
        public int MaxParticipants { get; set; }
        [Required]
        public string Address { get; set; }

        public string ExistingImagePath { get; set; }
        public IFormFile? NewImage { get; set; }

        public IEnumerable<DropdownOptionModel> Categories { get; set; }
        public IEnumerable<DropdownOptionModel> Locations { get; set; }
    }

}
