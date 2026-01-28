using EventHub.Core.EventValidation;
using EventHub.Core.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventHub.Core.ViewModels.Events
{
    public class EditEventViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        public int CategoryId { get; set; }
        public int LocationId { get; set; }

        public int MaxParticipants { get; set; }
        public string Address { get; set; }

        public string ExistingImagePath { get; set; }
        public IFormFile? NewImage { get; set; }

        public IEnumerable<DropdownOptionModel> Categories { get; set; }
        public IEnumerable<DropdownOptionModel> Locations { get; set; }
    }

}
