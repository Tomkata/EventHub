using EventHub.Core.EventValidation;
using EventHub.Core.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.ViewModels.Events
{
    public class CreateEventViewModel 
    {
        [Required]
        [StringLength(100,MinimumLength =3)]
        public string Title { get; set; }
        [Required]
        [Range(1,1000)]
        public int MaxParticipants { get; set; }    
        [Required]
        [StringLength(600, MinimumLength = 5)]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        [FutureDate(ErrorMessage = "Start date must be in the future")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = "End date must be after start date")]
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public Guid LocationId { get; set; }
        public IFormFile Image { get; set; }

        public IEnumerable<DropdownOptionModel> Categories { get; set; } = new List<DropdownOptionModel>();
        public IEnumerable<DropdownOptionModel> Locations { get; set; } = new List<DropdownOptionModel>();
    }
}
