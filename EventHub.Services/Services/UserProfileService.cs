
namespace EventHub.Services.Services
{
    using AutoMapper;
    using EventHub.Core.DTOs.Interest;
    using EventHub.Core.DTOs.UserProfile;
    using EventHub.Core.Exceptions.Location;
    using EventHub.Core.Exceptions.UserProfile;
    using EventHub.Core.Models;
    using EventHub.Repositories.Interfaces;
    using EventHub.Repositories.Repositories;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Diagnostics.Contracts;
    using System.Xml;
    using System.Xml.Linq;

    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfile;
        private readonly IMapper _mapper;
        private readonly ILocationRepository _locationRepository;
        private readonly IInterestRepository _interestRepository;

        public UserProfileService(IUserProfileRepository userProfile,
                                  IMapper mapper,
                                  ILocationRepository locationRepository,
                                  IInterestRepository interestRepository)
        {
            this._userProfile = userProfile;
            this._mapper = mapper;
            this._locationRepository = locationRepository;
            this._interestRepository = interestRepository;
        }
        public async Task CreateAsync(string userId, CreateUserProfileDto dto)
        {
            if (await ExistsAsync(userId))
                throw new ProfileAlreadyExistsException();



            var profile = _mapper.Map<UserProfile>(dto);
            profile.UserId = userId;

            var interests = await _userProfile.GetInterestsByIdsAsync(dto.InterestIds);

            //if someone bypasses the controller check (via API..),       
            if (interests.Count != dto.InterestIds.Count)
                throw new InvalidInterestException();

            profile.UserProfileInterests = interests
                .Select(x => new UserProfileInterest
                {
                    InterestId = x.Id,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            await _userProfile.AddAsync(profile);
            await _userProfile.SaveChangesAsync();
        }

       
        public async Task<bool> IsValidInterests(HashSet<Guid> interestDto)
        {
            var interestCount = await _userProfile.GetInterestsCountAsync(interestDto);
            if (interestCount != interestDto.Count)
                return false;

            return true;
        }

        public async Task EnsureProfileExistsAsync(string userId)
        {
            if (!await _userProfile.ExistsAsync(userId))
                throw new ProfileRequiredException();
        }

        public async Task<bool> ExistsAsync(string userId)
        => await _userProfile.ExistsAsync(userId);

        public async Task<DetailUserProfileDto?> GetDetailAsync(string userId)
            => await _userProfile.GetAll()
                .Where(x => x.UserId == userId)
                .Select(x => new DetailUserProfileDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Description = x.Description,
                    Location = x.Location.City,
                    PhoneNumber = x.Phone,
                    ProfileImagePath = x.ProfileImagePath,
                    Interests =
                         x.UserProfileInterests.Select(x => x.Interest.InterestName)
                         .ToList()
                })
                .FirstOrDefaultAsync();

        public async Task<UserNavInfoDto?> GetUserNavInfoAsync(string userId)
        {
            return await _userProfile.GetAll()
                .Where(p => p.UserId == userId)
                .Select(p => new UserNavInfoDto
                {
                    DisplayName = p.FirstName,
                    ImageUrl = p.ProfileImagePath
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string userId,EditUserProfileDto dto)
        {
            var profile = await _userProfile.GetByUserIdAsync(userId);

            if (profile == null)
                throw new ProfileNotFoundException();

            if(!await LocationExistsAsync(dto.LocationId))
        throw new InvalidLocationException();



            var interests = await _userProfile.GetInterestsByIdsAsync(dto.SelectedInterestIds);


            profile.FirstName = dto.FirstName;
            profile.LastName = dto.LastName;
            profile.Description = dto.Description;
            profile.LocationId = dto.LocationId;
            profile.Phone = dto.PhoneNumber;
            profile.UserProfileInterests = interests
                .Select(x => new UserProfileInterest
                {
                    InterestId = x.Id,
                    UserId = profile.UserId
                })
                .ToList();

            if (dto.ProfileImagePath != null)
                profile.ProfileImagePath = dto.ProfileImagePath;

            await _userProfile.SaveChangesAsync();
        }

        private async Task<bool> LocationExistsAsync(Guid Id) =>
            await _locationRepository.GetByIdAsync(Id) != null ? true : false;

        public async Task<HashSet<Guid>> GetSelectedInterestIdsAsync(IEnumerable<string> interestNames)
        {
            var allInterests = _interestRepository.GetAll();

            return allInterests
                .Where(i => interestNames.Contains(i.InterestName))
                .Select(i => i.Id)
                .ToHashSet();
        }

        public async Task<PublicUserProfileDto?> GetPublicDetailAsync(string userId)
       => await _userProfile.GetAll()
            .Where(x => x.UserId == userId)
            .Select(x => new PublicUserProfileDto
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Description = x.Description,
                ProfileImagePath = x.ProfileImagePath,
                Interests = x.UserProfileInterests
                .Select(x => x.Interest.InterestName)
                .ToList()
            })
            .FirstOrDefaultAsync();

        public async Task<bool> HasProfileAsync(string userId)
        => await _userProfile.ExistsAsync(userId);

        
    }
}
