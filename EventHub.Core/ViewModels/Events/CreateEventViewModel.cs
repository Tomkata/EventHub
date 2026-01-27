using EventHub.Core.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.ViewModels.Events
{
    public class CreateEventViewModel 
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public int MaxParticipants { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Guid LocationId { get; set; }
        public IFormFile Image { get; set; }

        public List<DropdownOptionModel> Categories { get; set; } = new();
        public List<DropdownOptionModel> Locations { get; set; } = new();
    }
}
