

namespace EventHub.Web.Controllers
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Exceptions.Image;
    using EventHub.Core.ViewModels.Common;
    using EventHub.Core.ViewModels.Events;
    using EventHub.Services.Interfaces;
    using EventHub.Services.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Validation;
    using System.Net.WebSockets;
    using System.Security.Claims;

    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;
        private readonly IImageService _imageService;
        private readonly IEventFormOptionsService _eventFormOptionsService;

        public EventsController(IEventService eventService,
                                ICategoryService categoryService,
                                ILocationService locationService,
                                IImageService imageService,
                                IEventFormOptionsService eventFormOptionsService)
        {
            this._eventService = eventService;
            this._categoryService = categoryService;
            this._locationService = locationService;
            this._imageService = imageService;
            this._eventFormOptionsService = eventFormOptionsService;
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


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            try
            {

                if ((!ModelState.IsValid) && IsEmptyForm(model))
                {
                    ModelState.Clear();
                    ModelState.AddModelError("", "Please fill in the form.");
                    model = await PrepareCreateViewModel();
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    model = await PrepareCreateViewModel();
                    return View(model);
                }

                var imageUrl = await _imageService.StoreImageAsync(model.Image);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    ModelState.AddModelError(nameof(userId), "The user is not logged in.");
                    return View(model);
                }

                if (!model.StartDate.HasValue || !model.EndDate.HasValue)
                {
                    ModelState.AddModelError("", "The date is requierd");
                    model = await PrepareCreateViewModel();
                    return View(model);
                }


                var eventDate = new CreateEventDto
                {
                    Title = model.Title,
                    Description = model.Description,
                    MaxParticipants = model.MaxParticipants,
                    Address = model.Address,
                    StartDate = (DateTime)model.StartDate,
                    EndDate = (DateTime)model.EndDate,
                    ImagePath = imageUrl,
                    CategoryId = model.CategoryId,
                    LocationId = model.LocationId,
                    OrganizerId = userId!
                };

                await _eventService.CreateAsync(eventDate);

                TempData["SuccessMessage"] = "Event created successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (ImageEmptyException imageException)
            {
                model = await PrepareCreateViewModel();
              return  HandleImageException(model, imageException);
            }
            catch (InvalidImageFormatException imageException)
            {
                
                model = await PrepareCreateViewModel();
                return HandleImageException(model,imageException);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Update(Guid Id)
        {

            var model = await PrepareEditViewModel(Id);

            if (model == null)
                return NotFound();

            var dropDowns = await _eventFormOptionsService.GetFormOptionsAsync();


            model.Categories = dropDowns.Categories;
            model.Locations = dropDowns.Locations;

            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(EditEventViewModel model)
        {
            if (!ModelState.IsValid)
            {

                 var dropDown = await _eventFormOptionsService.GetFormOptionsAsync();

                model.Categories = dropDown.Categories;
                model.Locations = dropDown.Locations;

                ModelState.AddModelError("", "Please fill in the form.");

                return View(model);
            }
            try
            {
                var eventToUpdate = new EditEventDto
                {
                    Title = model.Title,
                    Description = model.Description,
                    MaxParticipants = model.MaxParticipants,
                    Address = model.Address,
                    StartDate = (DateTime)model.StartDate,
                    EndDate = (DateTime)model.EndDate,
                    CategoryId = model.CategoryId,
                    LocationId = model.LocationId,
                };

                if (model.NewImage != null)
                {

                    var newImagePath = await _imageService.StoreImageAsync(model.NewImage);
                    eventToUpdate.ImagePath = newImagePath;
                }


                await _eventService.UpdateAsync(model.Id, eventToUpdate);

                return RedirectToAction(nameof(Index));

            }
            catch (ImageEmptyException imageException)
            {
                return HandleImageException(model, imageException);
            }
            catch (InvalidImageFormatException imageException)
            {
                return HandleImageException(model, imageException);
            }

        }

        private IActionResult HandleImageException(object model, Exception ex)
        {
            ModelState.AddModelError("Image", $"{ex.Message}");
            return View(model);
        }

        private bool IsEmptyForm(CreateEventViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) &&
                model.EndDate == null &&
                model.StartDate == null &&
                string.IsNullOrWhiteSpace(model.Description) &&
                model.CategoryId == default && 
                model.LocationId == default &&
                model.MaxParticipants == default &&
                string.IsNullOrWhiteSpace(model.Address) &&
                model.Image == null)
            {
                return true;
            }

            return false;
        }

        private async Task<CreateEventViewModel> PrepareCreateViewModel()
        {
            var dropDowns = await _eventFormOptionsService.GetFormOptionsAsync();
            var model = new CreateEventViewModel
            {
                 Categories = dropDowns.Categories,
                 Locations = dropDowns.Locations
            };
            return model;
        }

        private async Task<EditEventViewModel> PrepareEditViewModel(Guid Id)
        {
            var eventData = await _eventService.GetByIdAsync(Id);

            if (eventData == null)
                return null;


            var model = new EditEventViewModel
            {
                 Id = eventData.Id,
                Title = eventData.Title,
                Address = eventData.Address,
                Description = eventData.Description,
                StartDate = eventData.StartDate,
                EndDate = eventData.EndDate,
                MaxParticipants = eventData.MaxParticipants,
                ExistingImagePath = eventData.ImagePath
            };
            return model;
        }
    }

}
