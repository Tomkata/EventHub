

namespace EventHub.Web.Controllers
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Exceptions.Category;
    using EventHub.Core.Exceptions.Event.ForJoin;
    using EventHub.Core.Exceptions.Event.ForLeft;
    using EventHub.Core.Exceptions.Image;
    using EventHub.Core.Exceptions.Location;
    using EventHub.Core.Exceptions.User;
    using EventHub.Infrastructure;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Common;
    using EventHub.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Client;
    using System.Security.Claims;

    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IImageService _imageService;
        private readonly IEventFormOptionsService _eventFormOptionsService;
        private readonly IOrganizerService _organizerService;
        private readonly IParticipantService _participantService;


        public EventsController(IEventService eventService,
                                IImageService imageService,
                                IEventFormOptionsService eventFormOptionsService, 
                                IParticipantService participantService)
        {
            this._eventService = eventService;
            this._imageService = imageService;
            this._eventFormOptionsService = eventFormOptionsService;
            this._participantService = participantService;
        }

        public async Task<IActionResult> Index()
        {
            var allEvents = await _eventService.GetEventsAsync();

            HashSet<Guid> joinedIds = new();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                joinedIds = await _participantService.GetJoinedEventIdsAsync(userId);
            }



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
                     ParticipantsCount = x.ParticipantsCount,
                     CanDelete = false,
                     CanEdit = false,
                     IsParticipant = joinedIds.Contains(x.Id)
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


        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
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

  await FillDropDowns(model);
          


            return View(model);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
        [HttpPost]
        public async Task<IActionResult> Update(EditEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await FillDropDowns(model);

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


                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole(Roles.Admin);

                await _eventService.UpdateAsync(model.Id, eventToUpdate, userId, isAdmin);

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
            catch (InvalidUserPermissionsException)
            {
                return Unauthorized();
            }
        }

        private async Task FillDropDowns(EventFormBaseViewModel model)
        {
            var dropDown = await _eventFormOptionsService.GetFormOptionsAsync();

            model.Categories = dropDown.Categories;
            model.Locations = dropDown.Locations;
        }

        public async Task<IActionResult> Join(Guid eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _participantService.JoinEventAsync(userId, eventId);
                TempData["Success"] = "You have successfully joined the event.";
                return RedirectToAction(nameof(Details), new { eventId = eventId });

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex switch
                {
                    EventNotFoundException => "The event could not be found.",
                    EventExpiredException => "This event has already ended.",
                    UserNotFoundException => "You need to log in.",
                    UserAlreadyJoinedException => "You have already joined this event.",
                    OrganizerJoinOwnEventException => "Organizers cannot join their own events.",
                    EventFilledException => "This event has reached its maximum capacity.",
                    AdminCannnotJoinEventException => "Admin cannot join events.",
                    _ => "An unexpected error occurred. Please try again."
                };
            }
            return RedirectToAction(nameof(Details), new { eventId = eventId });
        }

        public async Task<IActionResult> Left(Guid eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _participantService.LeftEventAsync(userId,eventId);
                TempData["SuccessMessage"] = "You have successfully left the event.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex switch
                {
                    UserNotParticipantException => "You are not a participant in this event.",
                    EventNotFoundException => "The event could not be found.",
                    _ => "An unexpected error occurred."
                };

                return RedirectToAction(nameof(Index));
            }
        }


        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole(Roles.Admin);


                await _eventService.DeleteAsync(eventId, userId, isAdmin);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidUserPermissionsException)
            {
                return Unauthorized();
            }

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


        private async Task<IActionResult> HandleException(EventFormBaseViewModel model, Exception ex)
        {
            await FillDropDowns(model);

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