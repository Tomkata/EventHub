namespace EventHub.Web.ViewModels.UserProfile
{
    public class UserNavVm
    {
        public bool IsAuthenticated { get; set; }
        public bool HasProfile { get; set; }
        public string? DisplayName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
