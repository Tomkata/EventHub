

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Organizers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Apply(CancellationToken cancellation,string? returnUrl)
        {
            var userId = GetUserId();

            var organizerState = await _organizerService.GetOrganizerStateAsync(userId!, cancellation);
            var model = new ApplyOrganizerForm();
            model.OrganizerState = organizerState;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Apply(ApplyOrganizerForm model,CancellationToken cancellation)
        {
            var userId = GetUserId();
            model.UserId = userId;

            if (!ModelState.IsValid)
            {
                model.OrganizerState = await _organizerService.GetOrganizerStateAsync(userId, cancellation);
                return View(model);
            }
            var dto = _mapper.Map<OrganizerRequestFormDto>(model);

            await _organizerService.ApplyForOrganizerAsync(dto, userId, cancellation);

            TempData["SuccessMessage"] = "Your organizer application has been submitted successfully!";
            return RedirectToAction(nameof(Apply));
            
        }

    }
}
