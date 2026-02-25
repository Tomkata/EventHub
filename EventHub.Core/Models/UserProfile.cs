
namespace EventHub.Core.Models
{
    using EventHub.Core.Common.Validation.UserProfile;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserProfile
    {
        public UserProfile()
        {
            this.CreatedAt = DateTime.UtcNow;
        }

        [Key]
        public string UserId { get;  set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }   

        [MaxLength(UserProfileValidation.MaxLenDesciption)]
        public string? Description { get; set; } 

        [ForeignKey(nameof(Location))]
        public Guid? LocationId { get; set; }
        public Location? Location { get; set; }

        [RegularExpression("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$")]
        public string? Phone { get; set; } 

        public string? ProfileImagePath { get; set; }
            
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<UserProfileInterest> UserProfileInterests { get; set; }
    = new List<UserProfileInterest>();
    }
}
