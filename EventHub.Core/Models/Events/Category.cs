

namespace EventHub.Core.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    public class Category
    {
        public Category()
        {
            this.Id = Guid.NewGuid();
            this.Events = new HashSet<Event>();
        }

        [Key]
        [Comment("The id of the Category")]
        public Guid Id { get; set; }


        [Comment("The name of the Category")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public virtual ICollection<Event> Events { get; set; }

    }
}

