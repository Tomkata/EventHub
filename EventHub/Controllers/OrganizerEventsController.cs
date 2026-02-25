
namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Infrastructure;
    using EventHub.Infrastructure.Identity;
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
        public async Task<IActionResult> MyEvents()
        {
            var userId = GetUserId();
            var dtos = await _eventService.GetEventsByOrganizerIdAsync(userId);

            var models = _mapper.Map<List<EventListViewModel>>(dtos, opt =>
            {
                opt.Items["IsOrganizerView"] = true;
            });

            return View(models);
        }



    }
}
