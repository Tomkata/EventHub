
namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Infrastructure;
    using EventHub.Infrastructure.Identity;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class OrganizerEventsController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public OrganizerEventsController(UserManager<ApplicationUser> userManager,
                                          IEventService eventService,
                                          IMapper mapper)
        {
            this._userManager = userManager;
            this._eventService = eventService;
            this._mapper = mapper;
        }


        [HttpGet]
        [Authorize(Roles = Roles.Organizer)]
        public async Task<IActionResult> MyEvents(CancellationToken cancellation ,int page = 1, int pageSize=10)
        {
            var userId = GetUserId();
            var eventList = await _eventService.GetEventsByOrganizerIdAsync(userId,page,pageSize, cancellation);

            var mapped = _mapper.Map<List<EventListViewModel>>(
                eventList.Data,
                opt =>   opt.Items["IsOrganizerView"] = true
            );

            var model = new PagedResult<EventListViewModel>
            {
                Data = mapped,
                CurrentPageNumber = eventList.CurrentPageNumber,
                PageSize = eventList.PageSize,
                TotalRecords = eventList.TotalRecords
            };

            return View(model);
        }



    }
}
