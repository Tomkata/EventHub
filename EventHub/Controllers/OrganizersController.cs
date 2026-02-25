

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Core.Common.Validation.Organizer;
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.Exceptions.Oranizer.ForApply;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Organizers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class OrganizersController : BaseController
    {

        private readonly IOrganizerService _organizerService;
        private readonly IMapper _mapper;

        public OrganizersController(IOrganizerService organizerService,
                                     IMapper mapper)
        {
            this._organizerService = organizerService;
            this._mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Apply()
        {
            var userId = GetUserId();

            var organizerState = await _organizerService.GetOrganizerStateAsync(userId!);
            var model = new ApplyOrganizerForm();
            model.OrganizerState = organizerState;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Apply(ApplyOrganizerForm model)
        {

            var userId = GetUserId();

            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to apply.");
                return View(model);
            }

            model.UserId = userId;

            if (!ModelState.IsValid)
            {
                model.OrganizerState = await _organizerService.GetOrganizerStateAsync(userId);
                return View(model);
            }

            try
            {
                var dto = _mapper.Map<OrganizerRequestFormDto>(model);

                await _organizerService.ApplyForOrganizerAsync(dto, userId);

                TempData["SuccessMessage"] = "Your organizer application has been submitted successfully!";
                return RedirectToAction(nameof(Apply));
            }
            catch(CreateProfileApplyException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(actionName: "CreateProfile", controllerName: "UserProfile");
            }
            catch (AdminCannotApplyException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (UserAlreadyOrganizerException ex)
            {
                ModelState.AddModelError(string.Empty, "User is already an organizer!");
            }
            catch (OrganizerRequestPendingException ex)
            {
                ModelState.AddModelError(string.Empty, "Organizer request is pending!");
            }
            catch (OrganizerCooldownNotExpiredException ex)
            {
                var cooldownDays = OrganizerValidation.OrganizerCooldownDays;
                ModelState.AddModelError(string.Empty, $"You must wait {cooldownDays} days after rejection before applying again.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
            }

            model.OrganizerState = await _organizerService.GetOrganizerStateAsync(userId);
            return View(model);
        }

    }
}
