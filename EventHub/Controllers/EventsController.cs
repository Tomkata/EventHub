

namespace EventHub.Web.Controllers
{
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using EventHub.Core.ViewModels.Events;
    using System.Security.Cryptography.Pkcs;
    using EventHub.Core.DTOs;

    public class EventsController : Controller
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            this._eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            //Not necessary for authentication(Every user can see)

            var allEvents = await _eventService.GetEventsAsync();

            var eventList =
                 allEvents.Select(x => new EventListViewModel
                 {
                     Title = x.Title,
                     ImagePath = x.ImagePath,
                     Category = x.Category,
                     CategoryId = x.CategoryId,
                     CityId = x.CityId,
                     CityName = x.City,
                     StartDate = x.StartDate,
                     EndDate = x.EndDate,
                     MaxParticipants = x.MaxParticipants,
                     ParticipantsCount = x.ParticipantsCount
                 }
            )
                .ToList();


            return View(eventList);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
           
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {

        }



        
    }
}
