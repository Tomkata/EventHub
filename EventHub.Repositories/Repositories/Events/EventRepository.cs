namespace EventHub.Repositories.Repositories.Events
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Models.Events;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces.Events;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    public class EventRepository : IEventRepository
    {
        //TODO: Next course need to decide if I want to swtch to generic repo pattern ?(will analyze that)
        private readonly ApplicationDbContext _dbContext;

        public EventRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Event?> GetByIdAsync(Guid id,CancellationToken cancellation)
        {
            var eventEntity = await _dbContext.Events
                   .Include(x => x.Category)
                   .Include(x => x.Location)
                   .Include(x => x.EventParticipants)
                   .AsSplitQuery()
                   .FirstOrDefaultAsync(x => x.Id == id, cancellation);

            return eventEntity;
        }

        public async Task<DetailedEventDto?> GetByIdReadOnlyAsync(Guid id,CancellationToken cancellation)
        {
            var dto = await _dbContext.Events
                .Where(x => x.Id == id)
                .Select(e => new DetailedEventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    ImagePath = e.ImagePath,
                    Address = e.Address,
                    MaxParticipants = e.MaxParticipants,
                    ParticipantsCount = e.EventParticipants.Count(),

                    OrganizerName = (e.OrganizerProfile.FirstName + " " + e.OrganizerProfile.LastName).Trim(),

                    ParticipantList = e.EventParticipants
                    .Select(ep => new ParticipantDto
                    {
                        UserId = ep.UserId,
                        DisplayName = ((ep.UserProfile.FirstName ?? "") + " " + (ep.UserProfile.LastName ?? "")).Trim(),
                        ProfileImagePath = ep.UserProfile.ProfileImagePath
                    })
                    .ToList()
                })
                .FirstOrDefaultAsync(cancellation);

            return dto;
        }

        public async Task AddAsync(Event entity,CancellationToken cancellation)
        {
            await _dbContext.Events.AddAsync(entity, cancellation);
        }

        public void Remove(Event entity)
        {
            _dbContext.Events.Remove(entity);
        }



        public IQueryable<Event> GetAll()
            => _dbContext.Events.AsNoTracking();


        public IQueryable<Event> GetByOrganizerId(string id)
            => _dbContext.Events
                   .AsNoTracking()
                   .Where(x => x.OrganizerId == id);

        public async Task<EventJoinInfo?> GetEventJoinInfoAsync(Guid id,CancellationToken cancellation)
        {
            var eventDto = await _dbContext.Events
                .AsNoTracking()
                .Where(x => x.Id == id)
             .Select(x => new EventJoinInfo
             {
                 Id = x.Id,
                 EndDate = x.EndDate,
                 MaxParticipantsCount = x.MaxParticipants,
                 ParticipantsCount = x.EventParticipants.Count(),
                 OrganizerId = x.OrganizerId
             })
             .FirstOrDefaultAsync(cancellation);

            return eventDto;
        }
        public async Task SaveChangesAsync(CancellationToken  cancellationToken)
        {

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<Event?> Query()
        => _dbContext.Events
            .AsNoTracking()
            .AsQueryable();

        public async Task<bool> TryJoinAsync(Guid eventId, string userId,CancellationToken cancellation)
        {
            var affectedRows = await _dbContext.Database
     .ExecuteSqlInterpolatedAsync($@"
        INSERT INTO EventParticipants (EventId, UserId)
        SELECT {eventId}, {userId}
        WHERE (
            SELECT COUNT(*)
            FROM EventParticipants WITH (UPDLOCK, HOLDLOCK)
            WHERE EventId = {eventId}
        ) < (
            SELECT MaxParticipants
            FROM Events
            WHERE Id = {eventId}
        );", cancellation);

            return affectedRows == 1;
        }
    }
}
