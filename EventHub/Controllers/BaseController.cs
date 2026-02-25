

namespace EventHub.Web.Controllers
{
    using EventHub.Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    public class BaseController : Controller
    {
        protected bool IsAdmin()
         => User.IsInRole(Roles.Admin);

        protected string GetUserId() 
            => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ??  throw new UnauthorizedAccessException();


    }
}
