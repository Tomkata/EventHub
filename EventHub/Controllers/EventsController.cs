

namespace EventHub.Web.Controllers
{
    using EventHub.Core.DTOs;
    using EventHub.Core.Exceptions.Image;
    using EventHub.Core.ViewModels.Common;
    using EventHub.Core.ViewModels.Events;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;
        private readonly IImageService _imageService;

        public EventsController(IEventService eventService,
                                ICategoryService categoryService,
                                ILocationService locationService,
                                IImageService imageService)
        {
            this._eventService = eventService;
            this._categoryService = categoryService;
            this._locationService = locationService;
            this._imageService = imageService;
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var imageUrl = await _imageService.StoreImageAsync(model.Image);


                var eventDate = new CreateEventDto
                {
                    Title = model.Title,
                    Description = model.Description,
                    MaxParticipants = model.MaxParticipants,
                    Address = model.Address,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ImagePath = imageUrl,
                    CategoryId = model.CategoryId,
                    LocationId = model.LocationId
                };

                await _eventService.CreateAsync(eventDate);

                return RedirectToAction("Index");
            }
            catch (ImageEmptyException imageException)
            {
                ModelState.AddModelError("Image",$"{imageException.Message}");
                return View(model);
            }
            catch (InvalidImageFormatException imageException)
            {
                ModelState.AddModelError("Image", $"{imageException.Message}");
                return View(model);
            }
           
        }
    }
}
