using EventHub.Core.Common;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Event;
using EventHub.Core.Exceptions.Category;
using EventHub.Core.Exceptions.Image;
using EventHub.Core.Exceptions.Location;
using EventHub.Services.Interfaces;
using EventHub.Web.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EventHub.Web.Controllers
{
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
                 })
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
        [ValidateAntiForgeryToken]
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
                    LocationId = model.LocationId,
                    OrganizerId = userId!
                };

                await _eventService.CreateAsync(eventDate);

                TempData["SuccessMessage"] = "Event created successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (ImageEmptyException imageException)
            {
                return await HandleException(model, imageException);
            }
            catch (InvalidImageFormatException imageException)
            {

                return await HandleException(model, imageException);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Update(Guid eventId)
        {

            var model = await PrepareEditViewModel(eventId);

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
                    ImagePath = model.ExistingImagePath
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
                return await HandleException(model, imageException);
            }
            catch (InvalidImageFormatException imageException)
            {
                return await HandleException(model, imageException);
            }
            catch (InvalidCategoryException categoryException)
            {
                return await HandleException(model, categoryException);
            }
            catch (InvalidLocationException locationException)
            {
                return await HandleException(model, locationException);
            }

        }



        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            await _eventService.DeleteAsync(eventId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid eventId)
        {

            var eventDto = await _eventService.GetByIdAsync(eventId);

            var model = new DetailedEventViewModel
            {
                Id = eventDto.Id,
                Title = eventDto.Title,
                Description = eventDto.Description,
                StartDate = eventDto.StartDate,
                EndDate = eventDto.EndDate,
                Category = eventDto.CategoryName,
                CityName = eventDto.City,
                ImagePath = eventDto.ImagePath,
                ParticipantsCount = eventDto.ParticipantList.Count(),
                MaxParticipants = eventDto.MaxParticipants,
                Participants = eventDto.ParticipantList
            };  

            return View(model);
        }


        private async Task<IActionResult> HandleException(IEventFormViewModel model, Exception ex)
        {
            var dropDown = await _eventFormOptionsService.GetFormOptionsAsync();

            model.Categories = dropDown.Categories;
            model.Locations = dropDown.Locations;

            ModelState.AddModelError("", $"{ex.Message}");

            return View(model);
        }

        private bool IsEmptyForm(CreateEventViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) &&
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


            var model = new EditEventViewModel
            {
                Id = eventData.Id,
                Title = eventData.Title,
                Address = eventData.Address,
                Description = eventData.Description,
                StartDate = eventData.StartDate,
                EndDate = eventData.EndDate,
                MaxParticipants = eventData.MaxParticipants,
                ExistingImagePath = eventData.ImagePath,
                CategoryId = eventData.CategoryId,
                LocationId = eventData.LocationId
            };
            return model;
        }
    }

}
