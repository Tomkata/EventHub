namespace EventHub.Web.ViewComponents
{   
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.UserProfile;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class UserNavViewComponent : ViewComponent
    {
        private readonly IUserProfileService _userProfile;

        public UserNavViewComponent(IUserProfileService  userProfile) => this._userProfile = userProfile;

        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellation)
        {
            if (!UserClaimsPrincipal.Identity?.IsAuthenticated ?? true)
                return View("Guest");


            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return View("Guest");

            var profileInfo = await _userProfile.GetUserNavInfoAsync(userId, cancellation);

            if (profileInfo == null)
                return View("NoProfile");

                
            var vm = new UserNavVm
            {
                IsAuthenticated =true,
                HasProfile = true,
                DisplayName = profileInfo.DisplayName,
                ImageUrl = profileInfo.ImageUrl
            };

        

            return View("Profile",vm);
        } 

    }
}
