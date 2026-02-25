
namespace EventHub.Web.Controllers
{
    using EventHub.Infrastructure.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            this._signInManager = signInManager;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return Redirect($"/Identity/Account/Login?returnUrl={returnUrl}");
        }
    }
}
