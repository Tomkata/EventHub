

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Infrastructure;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces.Event;
    using EventHub.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class AdminEventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public AdminEventsController(IEventService eventService,
                                     IMapper mapper)
        {
            this._eventService = eventService;
            this._mapper = mapper;
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AllEvents(CancellationToken cancellationToken, int page = 1, int pageSize = 10)
        {

            var events = await _eventService.GetEventsAsync(page, pageSize, cancellationToken);


            var eventList = _mapper.Map<List<EventListViewModel>>(
                  events.Data,
                  opt => opt.Items["IsAdminView"] = true
              );


            var model = new PagedResult<EventListViewModel>
            {
                Data = eventList,
                CurrentPageNumber = events.CurrentPageNumber,
                PageSize = events.PageSize,
                TotalRecords = events.TotalRecords
            };

            return View(model);
        }
    }
}
