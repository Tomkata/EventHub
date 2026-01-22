

namespace EventHub.Core.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.IO.Compression;

    public class Location
    {
        public Location()
        {
            this.Id = Guid.NewGuid();
            this.Events = new HashSet<Event>();
        }

        [Key]
        [Comment("The Id of the location")]
        public Guid Id { get; set; }


        [Comment("The City where the event is located")]
        [Required]
        [StringLength(100)]
        public string City { get; set; }


        [Comment("The Country where the event is located")]
        [Required]
        [StringLength(100)]
        public string Country { get; set; }


        [Comment("The Zip Code of the City")]
        [Required]
        [Range(1000,9999)]
        public int Zip { get; set; }


        public virtual ICollection<Event> Events { get; set; }
    }
}
