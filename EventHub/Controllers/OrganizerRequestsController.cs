

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Core.Enums.Organizer;
    using EventHub.Core.Exceptions.Oranizer.ForApply;
    using EventHub.Core.Exceptions.Oranizer.ForApprove;
    using EventHub.Core.Exceptions.Oranizer.ForReject;
    using EventHub.Core.Exceptions.User;
    using EventHub.Infrastructure;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Organizers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

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

       

        [Authorize(Roles =Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Requests(int page = 1,int  pageSize = 10)
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
            try
            {
                await _organizerService.ApproveUserToOrganizerAsync(model.UserId);
                return RedirectToAction(nameof(Requests));
            }
            catch (InvalidApproveUser ex)
            {
              TempData["Error"] = "Invalid user to approve!";
    return RedirectToAction(nameof(Requests));
            }
            catch (UserAlreadyOrganizerException ex)
            {
                ModelState.AddModelError("", "User is already an organizer!!");
                return View(nameof(Requests));
            }
            catch (ApproveRejectedUserException ex)
            {
                ModelState.AddModelError("", "Cannot approve rejected user!");
                return View(nameof(Requests));
            }
            catch (UserNotFoundException ex)
            {
                ModelState.AddModelError("", "User doesn't exist!");
                return View(nameof(Requests));
            }
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
            try
            {
                await _organizerService.RejectUserToOrganizerAsync(model.UserId);
                return RedirectToAction(nameof(Requests));
            }
            catch (InvalidUserToReject ex)
            {
                ModelState.AddModelError("", "Invalid user to reject!");
                return View(nameof(Requests));
            }
            catch (RejectApprovedRequest ex)
            {
                ModelState.AddModelError("", "Cannot reject user with approved status!");
                return View(nameof(Requests));
            }
            catch (InvalidRejectException ex)
            {
                ModelState.AddModelError("", "The user is already in rejected status!");
                return View(nameof(Requests));
            }
        }
    }
}
