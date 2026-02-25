using EventHub.Core.Common.Validation.UserProfile;
using EventHub.Web.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Web.ViewModels.UserProfile
{
    public class CreateUserProfileViewModel : UserProfileFormBaseViewModel
    {
        [StringLength(UserProfileValidation.MaxLenName,
            MinimumLength = UserProfileValidation.MinLenName)]
        [Required]
        public string  FirstName { get; set; } = null!;
        [StringLength(UserProfileValidation.MaxLenName,
           MinimumLength = UserProfileValidation.MinLenName)]
        [Required]
        public string LastName { get; set; } = null!;

        [StringLength(UserProfileValidation.MaxLenDesciption,
            MinimumLength = UserProfileValidation.MinLenDesciption)]
        [Required]
        public string Description { get; set; } = null!;
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        public IFormFile Image { get; set; } = null!;

        [Required]
        public HashSet<Guid> SelectedInterestIds { get; set; } = new();

        [Required]
        public Guid LocationId { get; set; }
    }
}
    