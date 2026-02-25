using EventHub.Core.Common.Validation.UserProfile;
using EventHub.Web.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Web.ViewModels.UserProfile
{
    public class    EditUserProfileViewModel : UserProfileFormBaseViewModel
    {
        public Guid Id { get; set; }

        [StringLength(UserProfileValidation.MaxLenName,
    MinimumLength = UserProfileValidation.MinLenName)]
        [Required]
        public string FirstName { get; set; } = null!;
        [StringLength(UserProfileValidation.MaxLenName,
           MinimumLength = UserProfileValidation.MinLenName)]
        [Required]
        public string LastName { get; set; } = null!;

        [StringLength(UserProfileValidation.MaxLenDesciption,
            MinimumLength = UserProfileValidation.MinLenDesciption)]
        [Required]
        public string Description { get; set; } = null!;
        [Phone]
        [Required]
        public string PhoneNumber { get; set; } = null!;
        public string? ExistingImagePath { get; set; }  

        public IFormFile? NewImage { get; set; } 

        public HashSet<Guid> SelectedInterestIds { get; set; } = new();



        public Guid? LocationId { get; set; }
    }
}
