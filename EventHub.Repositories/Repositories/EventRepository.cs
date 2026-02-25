
namespace EventHub.Repositories.Repositories
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Exceptions.Event;
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class EventRepository : IEventRepository
    {
        //TODO: Next course need to decide if I want to swtch to generic repo pattern ?(will analyze that)
        private readonly ApplicationDbContext _dbContext;

        public EventRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            var eventEntity = await _dbContext.Events
                   .Include(x => x.Category)
                   .Include(x => x.Location)
                   .Include(x => x.EventParticipants)
                   .AsSplitQuery()
                   .FirstOrDefaultAsync(x => x.Id == id);

            return eventEntity;
        }

        public async Task<DetailedEventDto?> GetByIdReadOnlyAsync(Guid id)
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
                .FirstOrDefaultAsync();

            return dto;
        }

        public async Task AddAsync(Event entity)
        {
            await _dbContext.Events.AddAsync(entity);
        }

        public async Task RemoveAsync(Event entity)
        {
            _dbContext.Events.Remove(entity);
        }



        public IQueryable<Event> GetAll()
        {
            return _dbContext.Events.AsNoTracking();
        }


        public IQueryable<Event> GetByOrganizerId(string id)
        {
            return _dbContext.Events
                   .AsNoTracking()
                   .Where(x => x.OrganizerId == id);
        }

        public async Task<EventJoinInfo?> GetEventJoinInfoAsync(Guid id)
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
             .FirstOrDefaultAsync();

            return eventDto;
        }
        public async Task SaveChangesAsync()
        {

            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<Event?> Query()
        => _dbContext.Events
            .AsNoTracking()
            .AsQueryable();
    }
}
