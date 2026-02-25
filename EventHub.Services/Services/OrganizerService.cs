

namespace EventHub.Services.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using EventHub.Core.Common.Validation.Organizer;
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.Enums.Organizer;
    using EventHub.Core.Exceptions.Event.ForJoin;
    using EventHub.Core.Exceptions.Oranizer.ForApply;
    using EventHub.Core.Exceptions.Oranizer.ForApprove;
    using EventHub.Core.Exceptions.Oranizer.ForDemote;
    using EventHub.Core.Exceptions.Oranizer.ForReject;
    using EventHub.Core.Exceptions.User;
    using EventHub.Core.Models;
    using EventHub.Infrastructure;
    using EventHub.Infrastructure.Identity;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Data;

    public class OrganizerService : IOrganizerService
    {
        private readonly IOrganizerRequestRepository _requestRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository _userProfileRepository;
        public OrganizerService(IOrganizerRequestRepository requestRepository,
                                    UserManager<ApplicationUser> userManager,
                                    IMapper mapper,
                                    IUserProfileRepository userProfileRepository)
        {
            this._requestRepository = requestRepository;    
            this._userManager = userManager;
            this._mapper = mapper;
            this._userProfileRepository = userProfileRepository;
        }

        public async Task ApplyForOrganizerAsync(OrganizerRequestFormDto formDto, string userId)
        {
            var existingRequest = await _requestRepository.GetByUserIdAsync(userId);

            if (existingRequest != null)
            {

                if (existingRequest.Status == Status.Approved)
                    throw new UserAlreadyOrganizerException();  

                else if (existingRequest.Status == Status.Pending)
                    throw new OrganizerRequestPendingException();


                if (existingRequest.Status == Status.Rejected)
                {
                    if (existingRequest.LastRejectedAt.HasValue &&
               (DateTime.UtcNow - existingRequest.LastRejectedAt.Value).TotalDays < OrganizerValidation.OrganizerCooldownDays)
                    {
                        throw new OrganizerCooldownNotExpiredException();
                    }

                    existingRequest.Email = formDto.Email;
                    existingRequest.Note = formDto.Note;
                    existingRequest.Status = Status.Pending;
                    await _requestRepository.SaveChangesAsync();
                    return;
                }

            }

            var user = await _userManager.FindByIdAsync(userId) ?? throw new UserNotFoundException();
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(Roles.Admin))
                throw new AdminCannnotJoinEventException();

            var profile = await _userProfileRepository.GetByUserIdAsync(userId);

            if (profile == null)
                throw new CreateProfileApplyException();

            var requester = new OrganizerRequest
            {
                UserId = userId,
                Email = formDto.Email,
                Note = formDto.Note,
                Status = Status.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _requestRepository.AddAsync(requester);
            await _requestRepository.SaveChangesAsync();
        }

        public async Task ApproveUserToOrganizerAsync(string userId)
        {
            var existingRequest = await _requestRepository.GetByUserIdAsync(userId);

            if (existingRequest == null)
                throw new InvalidApproveUser();

            if (existingRequest.Status == Status.Approved)
                throw new UserAlreadyOrganizerException();

            else if (existingRequest.Status == Status.Rejected)
                throw new ApproveRejectedUserException();


            var user = await _userManager.FindByIdAsync(userId) ?? throw new UserNotFoundException();


            await _userManager.AddToRoleAsync(user, Roles.Organizer);

            existingRequest.Status = Status.Approved;

            await _requestRepository.SaveChangesAsync();

        }

      
        //TODO: Add in advanced module this business logic(It is ready to be used)
        public async Task DemoteOrganizerToUserAsync(string userId)
        {
            var existingRequest = await _requestRepository.GetByUserIdAsync(userId);

            if (existingRequest == null)
                throw new InvalidDemoteUser();

            if (existingRequest.Status == Status.Pending)
                throw new DemotePendingRequestException();

            if (existingRequest.Status == Status.Rejected)
                throw new DemoteRejectedException();

            var user = await _userManager.FindByIdAsync(userId) ?? throw new UserNotFoundException();

            await _userManager.RemoveFromRoleAsync(user,Roles.Organizer);

            existingRequest.Status = Status.Rejected;
            existingRequest.LastRejectedAt = DateTime.UtcNow;
            await _requestRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<PendingRequestForOrganizerDto>> GetAllPendingRequestsAsync(int pageNumber, int pageSize)
            => await _requestRepository.GetPendingRequests()
                .ProjectTo<PendingRequestForOrganizerDto>(_mapper.ConfigurationProvider)
                .ToPagedResultAsync(pageNumber,pageSize);

        public async Task<PagedResult<OrganizerRequestDto>> GetAllRequestsAsync(
            int pageNumber, int pageSize, Status? status = null)
            =>
                await _requestRepository.GetAll()
                .Where(x => !status.HasValue || x.Status == status)
                .ProjectTo<OrganizerRequestDto>(_mapper.ConfigurationProvider)
                .ToPagedResultAsync(pageNumber, pageSize);

        public async Task<Status> GetOrganizerStateAsync(string userId)
        {
            var user = await _requestRepository.GetByUserIdAsync(userId);

            if (user == null)
                return Status.None;

            return user.Status;
        }

        public async Task RejectUserToOrganizerAsync(string userId)
        {
            var existingRequest = await _requestRepository.GetByUserIdAsync(userId);

            if (existingRequest == null)
                throw new InvalidUserToReject();

            if (existingRequest.Status == Status.Approved)
                throw new RejectApprovedRequest();

            if(existingRequest.Status == Status.Rejected)
                throw new InvalidRejectException();

            else
                existingRequest.Status = Status.Rejected;

            existingRequest.LastRejectedAt = DateTime.UtcNow;


            await _requestRepository.SaveChangesAsync();
        }
    }
}
