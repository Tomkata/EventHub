

namespace EventHub.Services.Services
{
    using EventHub.Core.Common.Validation.Organizer;
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.Enums.Organizer;
    using EventHub.Core.Exceptions.Oranizer.ForApply;
    using EventHub.Core.Exceptions.Oranizer.ForApprove;
    using EventHub.Core.Exceptions.Oranizer.ForDemote;
    using EventHub.Core.Exceptions.Oranizer.ForReject;
    using EventHub.Core.Exceptions.User;
    using EventHub.Core.Models;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;

    public class OrganizerService : IOrganizerService
    {
        private readonly IOrganizerRequestRepository _requestRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public OrganizerService(IOrganizerRequestRepository requestRepository,
                                    UserManager<IdentityUser> userManager)
        {
            this._requestRepository = requestRepository;
            this._userManager = userManager;
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

                else if (existingRequest.LastRejectedAt.HasValue &&
    (DateTime.UtcNow - existingRequest.LastRejectedAt.Value).TotalDays < OrganizerValidation.OrganizerCooldownDays)
                    throw new OrganizerCooldownNotExpiredException();
            }

            var requester = new OrganizerRequest
            {
                UserId = userId,
                Email = formDto.Email,
                Note = formDto.Note,
                Status = Status.Pending
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


            await _userManager.AddToRoleAsync(user, "Organizer");

            existingRequest.Status = Status.Approved;

            await _requestRepository.SaveChangesAsync();

        }
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

            await _userManager.RemoveFromRoleAsync(user,"Organizer");

            existingRequest.Status = Status.Rejected;
            existingRequest.LastRejectedAt = DateTime.UtcNow;
            await _requestRepository.SaveChangesAsync();
        }

        public async Task RejectUserToOrganizerAsync(string userId)
        {
            var existingRequest = await _requestRepository.GetByUserIdAsync(userId);

            if (existingRequest == null)
                throw new InvalidApproveUser();

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
