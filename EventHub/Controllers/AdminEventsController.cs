using EventHub.Services.Interfaces;
using EventHub.Web.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class AdminEventsController : Controller
    {
        private readonly IEventService _eventService;

        public AdminEventsController(IEventService eventService)
        {
            this._eventService = eventService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllEvents()
        {
            var events = await _eventService.GetEventsAsync();

            bool isAdmin = User.IsInRole("Admin");

            var eventList =
                 events.Select(x => new EventListViewModel
                 {
                     Id = x.Id,
                     Title = x.Title,
                     ImagePath = x.ImagePath,
                     Category = x.Category,
                     CategoryId = x.CategoryId,
                     CityId = x.CityId,
                     CityName = x.City,
                     StartDate = x.StartDate,
                     EndDate = x.EndDate,
                     MaxParticipants = x.MaxParticipants,
                     ParticipantsCount = x.ParticipantsCount,
                     CanDelete = isAdmin,
                     CanEdit = isAdmin
                 })
                .ToList();

            return View(eventList);
        }
    }
}
