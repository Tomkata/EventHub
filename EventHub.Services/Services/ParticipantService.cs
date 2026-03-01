

namespace EventHub.Services.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using EventHub.Core.DTOs;
    using EventHub.Core.Exceptions.Event.ForJoin;
    using EventHub.Core.Exceptions.Event.ForLeft;
    using EventHub.Core.Exceptions.Oranizer.ForApply;
    using EventHub.Core.Exceptions.User;
    using EventHub.Infrastructure;
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Identity;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    public class ParticipantService : IParticipantService
    {

      

        private readonly IEventParticipantsRepository _eventParticipantsRepository;
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository _userProfile;
        private readonly ApplicationDbContext _applicationDb;

        public ParticipantService(IEventParticipantsRepository eventParticipantsRepository,
                                  IEventRepository eventRepository,
                                  UserManager<ApplicationUser> userManager,
                                  IMapper mapper,
                                  IUserProfileRepository userProfile,
                                  ApplicationDbContext applicationDb) 
        {
            this._eventParticipantsRepository = eventParticipantsRepository;
            this._eventRepository = eventRepository;
            this._userManager = userManager;
            this._mapper = mapper;
            this._userProfile = userProfile;
            this._applicationDb = applicationDb;
        }

        // TODO (Advanced):
        // This capacity check is NOT concurrency-safe.
        // In high-load scenarios multiple users can pass this validation and cause overbooking!!! 
        //In the next course - I need to fix this!
        public async Task JoinEventAsync(string userId, Guid eventId)
        {
            var @event = await _eventRepository.GetEventJoinInfoAsync(eventId);
                
            if (@event == null)
                throw new EventNotFoundException();

            var user = await _userManager.FindByIdAsync(userId) ?? throw new UserNotFoundException();

           

            // Too many queries to db. In the future need to be refacored!
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(Roles.Admin))
                throw new AdminCannnotJoinEventException();

            if (roles.Contains(Roles.Organizer))
            {
                if (IsOrganizerJoinOwnEvent(userId, @event.OrganizerId))
                    throw new OrganizerJoinOwnEventException();
            }

            var userProfile = await _userProfile.GetByUserIdAsync(userId);
            if (userProfile == null)
                throw new UserDontHavePrfileException();


            if (@event.EndDate < DateTime.UtcNow)
                throw new EventExpiredException();


            await using var transaction =
                           await _applicationDb.Database.BeginTransactionAsync();
            try
            {
                if (!await _eventRepository.TryJoinAsync(eventId, userId))
                    throw new EventFilledException();

                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                if (IsUniqueConstraintViolation(ex))
                    throw new UserAlreadyJoinedException();

                throw;
            }
        }

        private bool IsOrganizerJoinOwnEvent(string userId, string organizerId)
        => userId == organizerId;

        public async Task LeftEventAsync(string userId, Guid eventId)
        {
            await GetEventOrThrowAsync(eventId);

            var isParticipant = await _eventParticipantsRepository.ExistsAsync(userId, eventId);

            if (!isParticipant)
                throw new UserNotParticipantException();

            await _eventParticipantsRepository.RemoveParticipantFromEventAsync(userId,eventId);
            await _eventParticipantsRepository.SaveChangesAsync();
            
        }

        private async Task GetEventOrThrowAsync(Guid eventId)
        {
            var @event = await _eventRepository.GetEventJoinInfoAsync(eventId);

            if (@event == null)
                throw new EventNotFoundException();
        }

        public async Task<HashSet<Guid>> GetJoinedEventIdsAsync(string userId)
       => await _eventParticipantsRepository.GetJoinedEventIdsByUserAsync(userId);

        public async Task<PagedResult<EventDto>> GetJoinedEvents(string userId,int pageNumber,int pageSize)
        {
            return await _eventParticipantsRepository.GetJoinedEventsByUserId(userId)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .ToPagedResultAsync(pageNumber,pageSize);
        }

        public async Task<int> GetJoinedEventCountAsync(string userId)
        => await _eventParticipantsRepository.GetJoinedEventCountAsync(userId);

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Message.Contains("IX_EventParticipants_EventId_UserId");
            }

            return false;
        }
    }
}
