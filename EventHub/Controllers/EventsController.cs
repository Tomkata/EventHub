

namespace EventHub.Web.Controllers
{
    using EventHub.Core.DTOs;
    using EventHub.Core.Exceptions.Image;
    using EventHub.Core.ViewModels.Common;
    using EventHub.Core.ViewModels.Events;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Validation;

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
                     ParticipantsCount = x.ParticipantsCount
                 }
            )
                .ToList();


            return View(eventList);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateEventViewModel model = await PrepareCreateViewModel();

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
                    model = await PrepareCreateViewModel();
                    return View(model);
                }

                var imageUrl = await _imageService.StoreImageAsync(model.Image);

                //Check how u can optimize this (mayble in service) XDDD
                var categories = await _categoryService.GetCategoriesForDropdownAsync();
                var isCategoryExist = categories.Any(x => x.Id == model.CategoryId);

                if (!isCategoryExist)
                {
                    ModelState.AddModelError($"{nameof(model.CategoryId)}","Invalid category is selected!");

                    model = await PrepareCreateViewModel();
                    return View(model);
                }

                var locations = await _locationService.GetLocationsForDropdownAsync();
                var isLocationExist = locations.Any(x => x.Id == model.LocationId);

                if (!isLocationExist)
                {
                    ModelState.AddModelError($"{nameof(model.LocationId)}", "Invalid location is selected!");

                    model = await PrepareCreateViewModel();
                    return View(model);
                }


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
                 model = await PrepareCreateViewModel();

                return View(model);
            }
            catch (InvalidImageFormatException imageException)
            {
                ModelState.AddModelError("Image", $"{imageException.Message}");
                model = await PrepareCreateViewModel();

                return View(model);
            }
        }


        private async Task<CreateEventViewModel> PrepareCreateViewModel()
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
            return model;
        }

    }
}
