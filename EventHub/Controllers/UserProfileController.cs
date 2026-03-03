

namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Core.DTOs.Social;
    using EventHub.Core.DTOs.UserProfile;
    using EventHub.Core.enums.Image;
    using EventHub.Core.Enums;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using EventHub.Services.Interfaces.Social;
    using EventHub.Web.ViewModels.Common;
    using EventHub.Web.ViewModels.Social;
    using EventHub.Web.ViewModels.UserProfile;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    public class UserProfileController : BaseController
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;
        private readonly IInterestsService _interestsService;
        private readonly IParticipantService _participantService;
        private readonly IUserProfileFormOptionsService _userProfileFormOptions;
        private IUserFollowService _userFollowService;
        public UserProfileController(IUserProfileService userProfileService,
                                     IImageService imageService,
                                     IMapper mapper,
                                     IInterestsService interestsService,
                                     ILocationService locationService,
                                     IParticipantService participantService,
                                     IUserProfileFormOptionsService userProfileFormOptions,
                                     IUserFollowService userFollowService)
        {
            this._userProfileService = userProfileService;
            this._imageService = imageService;
            this._mapper = mapper;
            this._interestsService = interestsService;
            this._locationService = locationService;
            this._participantService = participantService;
            this._userProfileFormOptions = userProfileFormOptions;
            this._userFollowService = userFollowService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateProfile(CancellationToken cancellation)
        {
            var model = await PrepareCreateViewModel(cancellation);

            return View(model);
        }


        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateProfile(CreateUserProfileViewModel model,CancellationToken cancellation)
        {
            string? imageUrl = null;

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model, cancellation);
                return View(model);
            }

            var userId = GetUserId();
            var dto = _mapper.Map<CreateUserProfileDto>(model);

            if (model.SelectedInterestIds.Count == 0 || (!await _userProfileService.IsValidInterests(model.SelectedInterestIds,cancellation)))
            {
                ModelState.AddModelError("Interests", "Invalid interests.");
                await PopulateDropdowns(model,cancellation);
                return View(model);
            }

            if ( model.Image == null || model.Image.Length <= 0)
            {
                ModelState.AddModelError("Image", "Invalid image.");
            }
            else
            {
                using var stream = model.Image.OpenReadStream();

                var imageFormat = await _imageService.DetectFormat(stream,cancellation);

                if (imageFormat == ImageFormat.unknown)
                {
                    ModelState.AddModelError("Image", "Invalid image format.");
                }
                else
                {
                    imageUrl = await _imageService
                          .StoreImageAsync(stream, imageFormat, ImageFolder.Profiles,cancellation);

                    dto.ImagePath = imageUrl;
                }
            }
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model, cancellation);
                return View(model);
            }   

            await _userProfileService.CreateAsync(userId, dto, cancellation);

            TempData["SuccessMessage"] = "Profile created successfully!";

            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(string userId,CancellationToken cancellation)
        {

            var model = await PrepareEditViewModel(cancellation);

            if (model == null)
                return NotFound();


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Update(EditUserProfileViewModel model,CancellationToken cancellation)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model, cancellation);
                return View(model);
            }

            if (model.SelectedInterestIds.Count == 0 || (!await _userProfileService.IsValidInterests(model.SelectedInterestIds, cancellation)))
            {
                ModelState.AddModelError(string.Empty, "Invalid interests.");
                await PopulateDropdowns(model, cancellation);
                return View(model);
            }

            string? newImagePath = null;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dto = _mapper.Map<EditUserProfileDto>(model);

            if (model.NewImage != null)
            {
                if (model.NewImage.Length <= 0)
                {
                    ModelState.AddModelError("Image", "Invalid image.");
                }
                else
                {
                    using var stream = model.NewImage.OpenReadStream();

                    var imageFormat = await _imageService.DetectFormat(stream, cancellation);

                    if (imageFormat == ImageFormat.unknown)
                    {
                        ModelState.AddModelError("Image", "Invalid image format.");
                    }
                    else
                    {
                        newImagePath = await _imageService
                            .StoreImageAsync(stream, imageFormat, ImageFolder.Profiles, cancellation);

                        dto.ProfileImagePath = newImagePath;
                    }
                }

                if (!ModelState.IsValid)
                {
                    await PopulateDropdowns(model, cancellation);
                    return View(model);
                }
            }

                await _userProfileService.UpdateAsync(userId, dto, cancellation);

                if (newImagePath != null && model.ExistingImagePath != null)
                    await _imageService.DeleteImageAsync(model.ExistingImagePath);

                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Details));
            }

        private async Task<CreateUserProfileViewModel> PrepareCreateViewModel(CancellationToken cancellation)
        {
            var dropDown = await _userProfileFormOptions.GetFormOptionsAsync(cancellation);
            var model = new CreateUserProfileViewModel
            {
                Interests = dropDown.Interests,
                Locations = dropDown.Locations
            };

            return model;
        }
        private async Task<EditUserProfileViewModel?> PrepareEditViewModel(CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileDto = await _userProfileService.GetDetailAsync(userId, cancellation);

            if (profileDto == null)
                return null;

            var dropDown = await _userProfileFormOptions.GetFormOptionsAsync(cancellation);

            var selectedIds = await _userProfileService.GetSelectedInterestIdsAsync(profileDto.Interests,cancellation);

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
        public async Task<IActionResult> Details(CancellationToken cancellation)
        {
            var userId = GetUserId();

            var profileDto = await _userProfileService.GetDetailAsync(userId, cancellation);

            if (profileDto == null)
                return RedirectToAction(nameof(CreateProfile));

            var vm = _mapper.Map<DetailedUserProfileViewModel>(profileDto);
            vm.JoinedEventsCount = await _participantService.GetJoinedEventCountAsync(userId, cancellation);

            return View(vm);
        }


        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Follow(
            string userId,
            CancellationToken cancellationToken,
            string? returnUrl)
        {
            var currUserId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            if (!await _userProfileService.ExistsAsync(currUserId, cancellationToken))
                return RedirectToAction(nameof(CreateProfile));

            if (!await _userProfileService.ExistsAsync(userId, cancellationToken))
                return NotFound();

            await _userFollowService.Follow(currUserId,userId,cancellationToken);
            TempData["Success"] = "You are now following this user.";
            return RedirectToAction(nameof(Public), new { userId = userId });

        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Unfollow(
            string userId,
            CancellationToken cancellationToken,
             string? returnUrl)
        {
            var currUserId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            await _userFollowService.Unfollow(currUserId, userId, cancellationToken);
            return RedirectToAction(nameof(Public), new { userId = userId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyFollowers(
            CancellationToken cancellation,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            if (!await _userProfileService.ExistsAsync(userId,cancellation))
                return RedirectToAction(nameof(CreateProfile));

            var dtos = await _userFollowService.GetFollowersAsync(userId,pageNumber,pageSize,cancellation);

            var followersModel = _mapper.Map<List<SocialUserPreviewViewModel>>(dtos.Data);

            var paged = new PagedResult<SocialUserPreviewViewModel>
            {
                Data = followersModel,
                PageSize = pageSize,
                CurrentPageNumber = pageNumber,
                TotalRecords = followersModel.Count
            };

            return View(paged);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyFollowing(
            CancellationToken cancellation,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            if (!await _userProfileService.ExistsAsync(userId, cancellation))
                return RedirectToAction(nameof(CreateProfile));

            var dtos = await _userFollowService.GetFollingsAsync(userId, pageNumber, pageSize, cancellation);

            var followersModel = _mapper.Map<List<SocialUserPreviewViewModel>>(dtos.Data);

            var paged = new PagedResult<SocialUserPreviewViewModel>
            {
                Data = followersModel,
                PageSize = pageSize,
                CurrentPageNumber = pageNumber,
                TotalRecords = followersModel.Count
            };

            return View(paged);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Public(string userId, string? returnUrl, CancellationToken cancellation)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var currUserId = GetUserId();

            if (userId == currUserId) return RedirectToAction(nameof(Details));

            if (!await _userProfileService.HasProfileAsync(currUserId,cancellation))
                return RedirectToAction(nameof(CreateProfile));


            var publicUserProfile = await _userProfileService.GetPublicDetailAsync(userId, cancellation);

            if (publicUserProfile == null)
            {
                TempData["Error"] = "User doesn't exist.";
                return RedirectToAction(controllerName: "Events", actionName: "Index");
            }

            var model = _mapper.Map<PublicUserProfileViewModel>(publicUserProfile);
            model.UserId = userId;
            model.IsFollowing = await _userFollowService.IsFollowingAsync(currUserId,userId,cancellation);
            model.IsOwnProfile = currUserId == userId;
            model.ReturnUrl = returnUrl;

            return View(model);
        }

        private async Task PopulateDropdowns(UserProfileFormBaseViewModel model,CancellationToken cancellation)
        {
            var dropDown = await _userProfileFormOptions.GetFormOptionsAsync(cancellation);
            model.Interests = dropDown.Interests;
            model.Locations = dropDown.Locations;
        }

    }
}
