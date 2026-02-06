using EventHub.Core.Exceptions.Oranizer.ForApply;
using EventHub.Core.Exceptions.Oranizer.ForApprove;
using EventHub.Core.Exceptions.Oranizer.ForReject;
using EventHub.Core.Exceptions.User;
using EventHub.Services.Interfaces;
using EventHub.Web.ViewModels.Organizers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class OrganizerRequestsController : Controller
    {
        private readonly IOrganizerService _organizerService;

        public OrganizerRequestsController(IOrganizerService organizerService)
        {
            this._organizerService = organizerService;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            var requestsDtos = await _organizerService.GetAllPendingRequestsAsync();


            var model =  requestsDtos
                .Select(x => new OrganizerRequestViewModel
                {
                    Id = x.Id,
                    Note = x.Note,
                    Email = x.Email,
                    CreatedRequestDate = x.CreatedAt,
                    UserId = x.UserId
                })
                .ToList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
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
                ModelState.AddModelError("", "Invalid user to approve!");
                return View(nameof(Requests));
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

        [Authorize(Roles = "Admin")]
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
