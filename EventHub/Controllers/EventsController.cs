

namespace EventHub.Web.Controllers
{
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using EventHub.Core.ViewModels.Events;
    using System.Security.Cryptography.Pkcs;
    using EventHub.Core.DTOs;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using EventHub.Core.ViewModels.Common;

    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;

        public EventsController(IEventService eventService,
                                ICategoryService categoryService)
        {
            this._eventService = eventService;
            this._categoryService = categoryService;
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
            var categories = await _categoryService.GetCategoriesForDropdownAsync();
            var categoriesModel = categories
                .Select(x => new DropdownOptionModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();


            //return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {

        }



        
    }
}
