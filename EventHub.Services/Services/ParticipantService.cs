

namespace EventHub.Services.Services
{
    using EventHub.Core.Exceptions.Event.ForJoin;
    using EventHub.Core.Exceptions.User;
    using EventHub.Infrastructure;
    using EventHub.Infrastructure.Identity;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel;

    public class ParticipantService : IParticipantService
    {
        private readonly IEventParticipantsRepository _eventParticipantsRepository;
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParticipantService(IEventParticipantsRepository eventParticipantsRepository,
                                  IEventRepository eventRepository,
                                  UserManager<ApplicationUser> userManager)
        {
            this._eventParticipantsRepository = eventParticipantsRepository;
            this._eventRepository = eventRepository;
            this._userManager = userManager;
        }


        public async Task JoinEventAsync(string userId, Guid eventId)
        {
            var @event = await _eventRepository.GetEventJoinInfoAsync(eventId);

            if (@event == null)
                throw new EventNotFoundException();


            if (@event.EndDate < DateTime.UtcNow)
                throw new EventExpiredException();

            var user = await _userManager.FindByIdAsync(userId) ?? throw new UserNotFoundException();

            if (await _eventParticipantsRepository.ExistsAsync(userId, eventId))
                throw new UserAlreadyJoinedException();

            // To many queries to db. In the future need to be refacored!
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(Roles.Admin))
                throw new AdminCannnotJoinEventException();

            if (roles.Contains(Roles.Organizer))
            {

                if (IsOrganizerJoinOwnEvent(userId, @event.OrganizerId))
                    throw new OrganizerJoinOwnEventException();
            }

            if (@event.MaxParticipantsCount <= @event.ParticipantsCount)
                throw new EventFilledException();


            await _eventParticipantsRepository.AddParticipantToEventAsync(userId,eventId);
            await _eventParticipantsRepository.SaveChangesAsync();
        }

        private bool IsOrganizerJoinOwnEvent(string userId, string organizerId)
        => userId == organizerId;

        public Task LeftEventAsync(string userId, Guid eventId)
        {
            throw new NotImplementedException();
        }
    }
}
