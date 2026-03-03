namespace EventHub.Web.ViewModels.Social
{
    public class SocialUserPreviewViewModel
    {
        public string UserId { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string ProfileImagePath { get; set; } = null!;

        public string Location { get; set; } = null!;
    }
}
