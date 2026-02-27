

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Core.Enums.Organizer;
    using EventHub.Infrastructure;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Organizers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class OrganizerRequestsController : BaseController
    {
        private readonly IOrganizerService _organizerService;
        private readonly IMapper _mapper;

        public OrganizerRequestsController(IOrganizerService organizerService,
                                           IMapper mapper)
        {
            this._organizerService = organizerService;
            this._mapper = mapper;
        }



        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Requests(int page = 1, int pageSize = 10)
        {
            var requestsDtos = await _organizerService.GetAllPendingRequestsAsync(page, pageSize);

            var mappedData = _mapper.Map<List<OrganizerRequestViewModel>>(requestsDtos.Data);

            var model = new PagedResult<OrganizerRequestViewModel>
            {
                Data = mappedData,
                CurrentPageNumber = requestsDtos.CurrentPageNumber,
                PageSize = requestsDtos.PageSize,
                TotalRecords = requestsDtos.TotalRecords
            };

            return View(model);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> ApproveRequest(OrganizerRequestViewModel model)
        {
            await _organizerService.ApproveUserToOrganizerAsync(model.UserId);
            return RedirectToAction(nameof(Requests));
        }


        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index(Status? status = null, int pageSize = 10, int pageNumber = 1)
        {
            var allRequestsDto = await _organizerService.GetAllRequestsAsync(pageNumber, pageSize, status);

            var mappedData = _mapper.Map<List<AllRequestsViewModel>>(allRequestsDto.Data);

            var model = new PagedResult<AllRequestsViewModel>
            {
                Data = mappedData,
                CurrentPageNumber = allRequestsDto.CurrentPageNumber,
                PageSize = allRequestsDto.PageSize,
                TotalRecords = allRequestsDto.TotalRecords
            };

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DemoteOrganizer(string userId)
        {
            await _organizerService.DemoteOrganizerToUserAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> RejectRequest(OrganizerRequestViewModel model)
        {
            await _organizerService.RejectUserToOrganizerAsync(model.UserId);
            return RedirectToAction(nameof(Requests));
        }
    }
}
