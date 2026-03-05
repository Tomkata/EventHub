using AutoMapper;
using EventHub.Services.Common;
using EventHub.Services.Interfaces.Event;
using EventHub.Web.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class ParticipantsController : BaseController
    {
        private readonly IParticipantService _participantService;
        private readonly IMapper _mapper;

        public ParticipantsController(IParticipantService participantService,
                                       IMapper mapper)
        {
            this._participantService = participantService;
            this._mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyJoinedEvents(
            CancellationToken cancellation,
            int page = 1,
            int pageSize = 10)
        {
            var userId = GetUserId();

            var eventList = await _participantService.GetJoinedEvents(userId, page,pageSize, cancellation);

            var mapped = _mapper.Map<List<EventListViewModel>>(eventList.Data,
                opt => opt.Items["IsParticipantView"] = true
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
