using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class EventsContrroller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
