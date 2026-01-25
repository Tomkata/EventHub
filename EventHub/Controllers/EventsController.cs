

namespace EventHub.Web.Controllers
{
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using EventHub.Core.ViewModels.Events;
    using System.Security.Cryptography.Pkcs;
    using EventHub.Core.DTOs;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using EventHub.Core.ViewModels.Common;
    using Microsoft.AspNetCore.Authorization;

    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;

        public EventsController(IEventService eventService,
                                ICategoryService categoryService,
                                ILocationService locationService)
        {
            this._eventService = eventService;
            this._categoryService = categoryService;
            this._locationService = locationService;
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

            var locations = await _locationService.GetLocationsForDropdownAsync();
            var locationsModel = locations
                .Select(x => new DropdownOptionModel
                {
                    Id = x.Id,
                    Name = x.City
                })
                .ToList();

            var model = new CreateEventViewModel
            {
                Categories = categoriesModel,
                Locations = locationsModel
            };


            return View(model);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //return Redirect();
            }

            var eventDate = new CreateEventDto
            {
                 Title = model.Title,
                 Description = model.Description,
                 MaxParticipants = model.MaxParticipants,
                 Address = model.Address,
                 StartDate = model.StartDate,
                 EndDate = model.EndDate,
                 ImagePath = model.Image.FileName,
                 CategoryId = model.CategoryId,
                 LocationId = model.LocationId
            };

            await _eventService.CreateAsync(eventDate);

            return View("Index");
        }



        
    }
}
