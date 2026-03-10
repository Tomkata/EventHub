namespace EventHub.Web.ViewComponents
{
    using EventHub.Services.Interfaces.Messaging;
    using EventHub.Services.Interfaces.User;
    using EventHub.Web.ViewModels.UserProfile;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class UserNavViewComponent(IUserProfileService _userProfile,
                                       IConversationService _conversationService)
        : ViewComponent
    {
      

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

            var unreadConversationCount = await _conversationService
             .GetUnreadConversationsCountAsync(userId, cancellation);

            var vm = new UserNavVm
            {
                IsAuthenticated =true,
                HasProfile = true,
                DisplayName = profileInfo.DisplayName,
                ImageUrl = profileInfo.ImageUrl,
                 UnreadConversationsCount = unreadConversationCount
            };


            return View("Profile",vm);
        } 

    }
}
