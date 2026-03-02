

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.enums.Image;
    using EventHub.Core.Enums;
    using EventHub.Infrastructure;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Common;
    using EventHub.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

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


        public async Task<IActionResult> Index(
            CancellationToken cancellationToken,
            SearchEventByFilterViewModel search,
            int page = 1,
            int pageSize = 10)
        {


            var paged = await _eventService.SearchBy(
               search.Title,
               search.StartDate,
               search.EndDate,
               search.LocationId,
               search.CategoryId,
               page,
               pageSize,
               cancellationToken
                );

            var prepared = await PrepareSearchViewModel(cancellationToken);

            search.Locations = prepared.Locations;
            search.Categories = prepared.Categories;

            HashSet<Guid> joinedIds = new();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = GetUserId();
                joinedIds = await _participantService.GetJoinedEventIdsAsync(userId,cancellationToken);
            }

            var eventList = _mapper.Map<List<EventListViewModel>>(
              paged.Data,
              opt => opt.Items["JoinedIds"] = joinedIds
              );



            var model = new EventsIndexViewModel
            {
                Search = search,
                Paged = new PagedResult<EventListViewModel>
                {
                    Data = eventList,
                    CurrentPageNumber = paged.CurrentPageNumber,
                    PageSize = paged.PageSize,
                    TotalRecords = paged.TotalRecords
                }
            };

            return View(model);

        }


        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellation)
        {
            CreateEventViewModel model = await PrepareCreateViewModel(cancellation);

            return View(model);
        }


        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventViewModel model,CancellationToken cancellation)
        {

            if ((!ModelState.IsValid) && IsEmptyForm(model))
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Please fill in the form.");
                model = await PrepareCreateViewModel(cancellation);
                return View(model);
            }


            if (!ModelState.IsValid)
            {
                model = await PrepareCreateViewModel(cancellation);
                return View(model);
            }



            if (model.Image == null || model.Image.Length == 0)
                ModelState.AddModelError("Image", "Image is required.");

            using var stream = model.Image.OpenReadStream();

            var imageFormat = await _imageService.DetectFormat(stream,cancellation);

            if (imageFormat == ImageFormat.unknown)
                ModelState.AddModelError("Image", "Invalid image format.");

            if (!ModelState.IsValid)
            {
                await FillDropDowns(model, cancellation);
                return View(model);
            }

            var imageUrl = await _imageService.StoreImageAsync(stream, imageFormat, ImageFolder.Events,cancellation);

            var requestingUserId = GetUserId();

            var @event = _mapper.Map<CreateEventDto>(model);
            @event.ImagePath = imageUrl;


            await _eventService.CreateAsync(@event, requestingUserId, cancellation);

            TempData["SuccessMessage"] = "Event created successfully!";

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
        public async Task<IActionResult> Update(Guid eventId,CancellationToken cancellation)
        {

            var model = await PrepareEditViewModel(eventId, cancellation);

            if (model == null)
                return NotFound();

            await FillDropDowns(model, cancellation);



            return View(model);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
        [HttpPost]
        public async Task<IActionResult> Update(EditEventViewModel model,CancellationToken cancellation)
        {
            if (!ModelState.IsValid)
            {
                await FillDropDowns(model, cancellation);

                ModelState.AddModelError("", "Please fill in the form.");
                return View(model);
            }
            var eventToUpdate = _mapper.Map<EditEventDto>(model);

            if (model.NewImage != null)
            {
                if (model.NewImage.Length <= 0)
                {
                    ModelState.AddModelError("Image", "Invalid image.");
                }
                else
                {
                    using var stream = model.NewImage.OpenReadStream();

                    var imageFormat = await _imageService.DetectFormat(stream,cancellation);

                    if (imageFormat == ImageFormat.unknown)
                    {
                        ModelState.AddModelError("Image", "Invalid image format.");
                    }
                    else
                    {
                        var imageUrl = await _imageService
                            .StoreImageAsync(stream, imageFormat, ImageFolder.Events,cancellation);

                        eventToUpdate.ImagePath = imageUrl;
                    }
                }

                if (!ModelState.IsValid)
                {
                    await FillDropDowns(model, cancellation);
                    return View(model);
                }
            }

            var userId = GetUserId();
            var isAdmin = IsAdmin();

            await _eventService.UpdateAsync(model.Id, eventToUpdate, userId, isAdmin, cancellation);

            return RedirectToAction(nameof(Index));
        }



        private async Task FillDropDowns(EventFormBaseViewModel model, CancellationToken cancellation)
        {
            var dropDown = await _eventFormOptionsService.GetFormOptionsAsync(cancellation);

            model.Categories = dropDown.Categories;
            model.Locations = dropDown.Locations;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(Guid eventId,string? returnUrl, CancellationToken cancellation)
        {
            var userId = GetUserId();

                await _participantService.JoinEventAsync(userId, eventId, cancellation);
                TempData["SuccessMessage"] = "You have successfully joined the event.";
                return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Left(Guid eventId, CancellationToken cancellation,string? returnUrl = null)
        {
            var userId = GetUserId();
          
                await _participantService.LeftEventAsync(userId, eventId, cancellation);
                TempData["SuccessMessage"] = "You have successfully left the event.";
          

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = $"{Roles.Admin},{Roles.Organizer}")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid eventId,CancellationToken  cancellation)
        {
            var userId = GetUserId();
            var isAdmin = IsAdmin();


            await _eventService.DeleteAsync(eventId, userId, isAdmin, cancellation);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid eventId,CancellationToken cancellation)
        {

            var eventDto = await _eventService.GetByIdAsync(eventId, cancellation);
            var model = _mapper.Map<DetailedEventViewModel>(eventDto);
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

        private async Task<CreateEventViewModel> PrepareCreateViewModel(CancellationToken cancellation)
        {
            var dropDowns = await _eventFormOptionsService.GetFormOptionsAsync(cancellation);
            var model = new CreateEventViewModel
            {
                Categories = dropDowns.Categories,
                Locations = dropDowns.Locations
            };
            return model;
        }

        private async Task<EditEventViewModel> PrepareEditViewModel(Guid Id, CancellationToken cancellation)
        {
            var eventData = await _eventService.GetForEditAsync(Id,cancellation);

            var model = _mapper.Map<EditEventViewModel>(eventData);

            return model;
        }

        private async Task<SearchEventByFilterViewModel> PrepareSearchViewModel(CancellationToken cancellation)
        {
            var dropDowns = await _eventFormOptionsService.GetFormOptionsAsync(cancellation);
            var model = new SearchEventByFilterViewModel
            {
                Categories = dropDowns.Categories,
                Locations = dropDowns.Locations
            };
            return model;
        }

    }

}