

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Enums;
    using EventHub.Core.Exceptions.Event.ForJoin;
    using EventHub.Core.Exceptions.Event.ForLeft;
    using EventHub.Core.Exceptions.Image;
    using EventHub.Core.Exceptions.User;
    using EventHub.Infrastructure;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Common;
    using EventHub.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class EventsController : BaseController
    {
        private readonly IEventService _eventService;
        private readonly IImageService _imageService;
        private readonly IEventFormOptionsService _eventFormOptionsService;
        private readonly IOrganizerService _organizerService;
        private readonly IParticipantService _participantService;
        private readonly IMapper _mapper;


        public EventsController(IEventService eventService,
                                IImageService imageService,
                                IEventFormOptionsService eventFormOptionsService, 
                                IParticipantService participantService,
                                IMapper mapper)
        {
            this._eventService = eventService;
            this._imageService = imageService;
            this._eventFormOptionsService = eventFormOptionsService;
            this._participantService = participantService;
            this._mapper = mapper;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var allEvents = await _eventService.GetEventsAsync(page, pageSize);

            HashSet<Guid> joinedIds = new();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = GetUserId();
                joinedIds = await _participantService.GetJoinedEventIdsAsync(userId);
            }

            var eventList = _mapper.Map<List<EventListViewModel>>(
              allEvents.Data,
              opt => opt.Items["JoinedIds"] = joinedIds
              );

            var model = new PagedResult<EventListViewModel>
            {
                Data = eventList,
                CurrentPageNumber = allEvents.CurrentPageNumber,
                PageSize = allEvents.PageSize,
                TotalRecords = allEvents.TotalRecords
            };
            return View(model);

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

                using var stream = model.Image.OpenReadStream();

                var imageUrl = await _imageService.StoreImageAsync(stream,model.Image.FileName, ImageFolder.Events);

                var requestingUserId = GetUserId();

                var @event = _mapper.Map<CreateEventDto>(model);


                await _eventService.CreateAsync(@event, requestingUserId);

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
        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
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
                var eventToUpdate = _mapper.Map<EditEventDto>(model);

                if (model.NewImage != null)
                {

                    using var stream = model.NewImage.OpenReadStream();

                    var imageUrl = await _imageService.StoreImageAsync(stream, model.NewImage.FileName, ImageFolder.Events);
                    eventToUpdate.ImagePath = imageUrl;
                }


                var userId = GetUserId();
                var isAdmin = IsAdmin();

                await _eventService.UpdateAsync(model.Id, eventToUpdate, userId, isAdmin);

                return RedirectToAction(nameof(Index));

            }
            catch (InvalidUserPermissionsException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return await HandleException(model, ex);
            }
        }
        
        private async Task FillDropDowns(EventFormBaseViewModel model)
        {
            var dropDown = await _eventFormOptionsService.GetFormOptionsAsync();

            model.Categories = dropDown.Categories;
            model.Locations = dropDown.Locations;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(Guid eventId)
        {
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException();

            try
            {
                await _participantService.JoinEventAsync(userId, eventId);
                TempData["SuccessMessage"] = "You have successfully joined the event.";
                return RedirectToAction(nameof(Index));
            }
            catch (UserDontHavePrfileException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(actionName:"CreateProfile",controllerName: "UserProfile");

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { eventId = eventId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Left(Guid eventId, string? returnUrl = null)
        {
            var userId = GetUserId();
            try
            {
                await _participantService.LeftEventAsync(userId, eventId);
                TempData["SuccessMessage"] = "You have successfully left the event.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex switch
                {
                    UserNotParticipantException => "You are not a participant in this event.",
                    EventNotFoundException => "The event could not be found.",
                    _ => "An unexpected error occurred."
                };
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();


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
            var model = _mapper.Map<DetailedEventViewModel>(eventDto);
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
            var eventData = await _eventService.GetForEditAsync(Id);

            var model = _mapper.Map<EditEventViewModel>(eventData);

            return model;
        }



    }

}