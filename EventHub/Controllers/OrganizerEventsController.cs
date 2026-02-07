using EventHub.Infrastructure.Identity;
using EventHub.Services.Interfaces;
using EventHub.Web.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class OrganizerEventsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventService _eventService;

        public OrganizerEventsController(UserManager<ApplicationUser> userManager,
                                          IEventService eventService)
        {
            this._userManager = userManager;
            this._eventService = eventService;
        }


        [HttpGet]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> MyEvents()
        {
            var currUser =  await GetCurrentUserAsync();
            var dtos = await _eventService.GetEventsByOrganizerIdAsync(currUser.Id);

            var events = dtos
                .Select(x => new EventListViewModel
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
                    CanDelete = true,
                    CanEdit = true
                })
                .ToList();

            return View(events);
        }



        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
