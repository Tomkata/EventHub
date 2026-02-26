

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Core.DTOs.UserProfile;
    using EventHub.Core.Enums;
    using EventHub.Core.Exceptions.UserProfile;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Interfaces;
    using EventHub.Web.ViewModels.Common;
    using EventHub.Web.ViewModels.UserProfile;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Validation;
    using System.Security.Claims;
    public class UserProfileController : BaseController
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;
        private readonly IInterestsService _interestsService;
        private readonly IParticipantService   _participantService;
        private readonly IUserProfileFormOptionsService _userProfileFormOptions;
        public UserProfileController(IUserProfileService userProfileService,
                                     IImageService imageService,
                                     IMapper mapper,
                                     IInterestsService interestsService,
                                     ILocationService locationService,
                                     IParticipantService participantService,
                                     IUserProfileFormOptionsService userProfileFormOptions)
        {
            this._userProfileService = userProfileService;
            this._imageService = imageService;
            this._mapper = mapper;
            this._interestsService = interestsService;
            this._locationService = locationService;
            this._participantService = participantService;
            this._userProfileFormOptions = userProfileFormOptions;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateProfile()
        {
            var model = await PrepareCreateViewModel();

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProfile(CreateUserProfileViewModel model)
        {
            string? imageUrl = null;

            try
            {
                var userId = GetUserId();

                using var stream = model.Image.OpenReadStream();

                imageUrl = await _imageService.StoreImageAsync(
                    stream,
                    model.Image.FileName,
                    ImageFolder.Profiles);

                var dto = _mapper.Map<CreateUserProfileDto>(model);
                dto.ImagePath = imageUrl;

                await _userProfileService.CreateAsync(userId, dto);

                TempData["SuccessMessage"] = "Profile created successfully!";


                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            catch (Exception e)
                 when (e is ProfileAlreadyExistsException ||
                       e is UserNotAppliedAnyInterestsException ||
                       e is InvalidInterestException ||
                       e is ProfileRequiredException)
            {
                if (imageUrl != null)
                    await _imageService.DeleteImageAsync(imageUrl);

                ModelState.AddModelError(string.Empty, e.Message);
                await PopulateDropdowns(model);
                return View(model);
            }
            catch (Exception)
            {
                if (imageUrl != null)
                    await _imageService.DeleteImageAsync(imageUrl);

                ModelState.AddModelError("", "Something went wrong. Please try again.");
                await PopulateDropdowns(model);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(string userId)
        {

            var model = await PrepareEditViewModel();

            if (model == null)
                return NotFound();


            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(EditUserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                ModelState.AddModelError("", "Please fill in the form.");
                return View(model);
            }
            string? newImagePath = null;
            try
            {

                if (model.NewImage != null)
                {
                    using var stream = model.NewImage.OpenReadStream();

                    newImagePath = await _imageService.StoreImageAsync(
                        stream,
                        model.NewImage.FileName,
                        ImageFolder.Profiles);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dto = _mapper.Map<EditUserProfileDto>(model);

                if (newImagePath != null)
                    dto.ProfileImagePath = newImagePath;

                await _userProfileService.UpdateAsync(userId, dto);

                if (newImagePath != null && model.ExistingImagePath != null)
                    await _imageService.DeleteImageAsync(model.ExistingImagePath);

                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Details));
            }
            catch (Exception ex)
            {

                if (newImagePath != null)
                    await _imageService.DeleteImageAsync(newImagePath);

                ModelState.AddModelError("", ex.Message);
                await PopulateDropdowns(model);
                return View(model);
            }
        }

        private async Task<CreateUserProfileViewModel> PrepareCreateViewModel()
        {
            var dropDown = await _userProfileFormOptions.GetFormOptionsAsync();
            var model = new CreateUserProfileViewModel
            {
                Interests = dropDown.Interests,
                Locations = dropDown.Locations
            };

            return model;
        }
        private async Task<EditUserProfileViewModel?> PrepareEditViewModel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileDto = await _userProfileService.GetDetailAsync(userId);

            if (profileDto == null)
                return null;

            var dropDown = await _userProfileFormOptions.GetFormOptionsAsync();

            var selectedIds = await _userProfileService.GetSelectedInterestIdsAsync(profileDto.Interests);

            var model = new EditUserProfileViewModel
            {
                FirstName = profileDto.FirstName,
                LastName = profileDto.LastName,
                Description = profileDto.Description,
                PhoneNumber = profileDto.PhoneNumber,
                ExistingImagePath = profileDto.ProfileImagePath,
                SelectedInterestIds = selectedIds,
                LocationId = dropDown.Locations
                    .FirstOrDefault(l => l.Name == profileDto.Location)?.Id ?? Guid.Empty
            };
            model.Interests = dropDown.Interests;
            model.Locations = dropDown.Locations;

            return model;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details()
        {
            var userId = GetUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var profileDto = await _userProfileService.GetDetailAsync(userId);

            if (profileDto == null)
                return RedirectToAction(nameof(CreateProfile));

            var vm = _mapper.Map<DetailedUserProfileViewModel>(profileDto);
            vm.JoinedEventsCount = await _participantService.GetJoinedEventCountAsync(userId);

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Public(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var currUserId = GetUserId();

            if (userId == currUserId) return RedirectToAction(nameof(Details));

            if (!await _userProfileService.HasProfileAsync(currUserId))
                return RedirectToAction(nameof(CreateProfile));


            var publicUserProfile = await _userProfileService.GetPublicDetailAsync(userId);

            if (publicUserProfile == null)
            {
                TempData["Error"] = "User doesn't exist.";
                return RedirectToAction(controllerName:"Events",actionName:"Index");
            }

            var model = _mapper.Map<PublicUserProfileViewModel>(publicUserProfile);

            return View(model);
        }

        private async Task PopulateDropdowns(UserProfileFormBaseViewModel model)
        {
            var dropDown = await _userProfileFormOptions.GetFormOptionsAsync();
            model.Interests = dropDown.Interests;
            model.Locations = dropDown.Locations;
        }

    }
}
