using AutoMapper;
using EventHub.Services.Interfaces;
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
        public async Task<IActionResult> MyJoinedEvents()
        {
            var userId = GetUserId();

            var dtos = await _participantService.GetJoinedEvents(userId);

            var models = _mapper.Map<List<EventListViewModel>>(dtos, opt =>

            opt.Items["IsParticipantView"] = true
            );      
          

            return View(models);
        }
    }
}
