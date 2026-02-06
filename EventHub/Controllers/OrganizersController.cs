

namespace EventHub.Web.Controllers
{
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.Exceptions.Oranizer.ForApply;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Organizers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class OrganizersController : Controller
    {

        private readonly IOrganizerService _organizerService;

        public OrganizersController(IOrganizerService organizerService)
        {
            this._organizerService = organizerService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Apply()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var organizerState = await _organizerService.GetOrganizerStateAsync(userId);
            var model = new ApplyOrganizerForm();
            model.OrganizerState = organizerState;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Apply(ApplyOrganizerForm model)
        {   
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("","The form is invalid!");
                return View(model);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                ModelState.AddModelError(nameof(userId), "The user is not logged in.");
                return View(model);
            }

            try
            {
                var dto = new OrganizerRequestFormDto
                {
                     Email = model.Email,
                     UserId = userId,
                     Note = model.Note
                };

                await _organizerService.ApplyForOrganizerAsync(dto,userId);
                return View(model); // if everything okey, its gonna be refresh and chenge with status case(i will figure it out)
            }
            catch (UserAlreadyOrganizerException ex)
            {
                ModelState.AddModelError("", "User is already an organizer!");
                return View(model);

            }
            catch (OrganizerRequestPendingException ex)
            {
                ModelState.AddModelError("", "Organizer request is in pending!");
                return View(model);
            }
            catch (OrganizerCooldownNotExpiredException ex)
            {
                ModelState.AddModelError("", "Organizer cooldown is not expired!");
                return View(model);
            }
        }

    }
}
