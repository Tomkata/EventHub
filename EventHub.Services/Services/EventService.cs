


namespace EventHub.Services.Services
{
    using EventHub.Core.Models;
    using EventHub.Core.DTOs;
    using EventHub.Infrastructure.Data;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _dbContext;

        public EventService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;    
        }


        public async Task<DetailedEventDto> GetByIdAsync(Guid id)
        {

            var eventEntity = await _dbContext.Events
                            .AsNoTracking()
                            .Select(x=>new
                            {
                                x.Id,
                                x.ImagePath,
                                x.Title,
                                Category = x.Category.Name,
                                x.MaxParticipants,
                                x.Description,
                                x.StartDate,
                                x.EndDate,
                                x.Address,
                                City = x.Location.City,
                                x.OrganizerId
                            })
                            .FirstOrDefaultAsync(x => x.Id == id);

            if (eventEntity == null) throw new InvalidOperationException("Invalid event id");


            var participants = await _dbContext.EventParticipants
               .AsNoTracking()
               .Where(ep => ep.EventId == id)
               .Join(
                   _dbContext.Users,
                   ep => ep.UserId,
                   u => u.Id,
                   (ep, u) => new ParticipantDto
                   {
                       UserId = u.Id,
                       UserName = u.UserName
                   }
               )
               .ToListAsync();


            var organizer = await _dbContext.Users
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.UserName
                })
                .FirstOrDefaultAsync(x => x.Id == eventEntity.OrganizerId);

            if(organizer == null) throw new InvalidOperationException("invalid organizer");

            var dto = new DetailedEventDto
            {
                 Title = eventEntity.Title,
                 Category = eventEntity.Category,
                 MaxParticipants = eventEntity.MaxParticipants,
                 Description = eventEntity.Description,
                 StartDate = eventEntity.StartDate,
                 EndDate = eventEntity.EndDate,     
                 OrganizerName = organizer.UserName,
                 City = eventEntity.City,
                 Address = eventEntity.Address,
                 ImagePath = eventEntity.ImagePath,
                 ParticipantList = participants
            };

            return dto;
        }


        public async Task CreateAsync(CreateEventDto dto)
        {
            bool categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            bool locationExists = await _dbContext.Locations.AnyAsync(l => l.Id == dto.LocationId);

            if (!categoryExists || !locationExists)
            {
                throw new ArgumentException("Invalid CategoryId or LocationId");
            }

            var eventEntity = new Event
            {
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ImagePath = dto.ImagePath,
                Address = dto.Address,
                MaxParticipants = dto.MaxParticipants,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                LocationId = dto.LocationId,
                OrganizerId = dto.OrganizerId
            };

            await _dbContext.Events.AddAsync(eventEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, CreateEventDto dto)
        {
            Event? eventEntity = await ValidateEvent(id);

            eventEntity.Title = dto.Title;
            eventEntity.LocationId = dto.LocationId;
            eventEntity.CategoryId = dto.CategoryId;
            eventEntity.StartDate = dto.StartDate;
            eventEntity.EndDate = dto.EndDate;
            eventEntity.MaxParticipants = dto.MaxParticipants;
            eventEntity.Description = dto.Description;
            eventEntity.Address = dto.Address;
            eventEntity.StartDate = dto.StartDate;
            eventEntity.ImagePath = dto.ImagePath;

            await _dbContext.SaveChangesAsync();

        }

       

        public async Task DeleteAsync(Guid id)
        {
            Event? eventEntity = await ValidateEvent(id);

             _dbContext.Events.Remove(eventEntity);
            await _dbContext.SaveChangesAsync();
        }



        private async Task<Event?> ValidateEvent(Guid id)
        {
            var eventEntity = await _dbContext.Events
                .FirstOrDefaultAsync(x => x.Id == id);

            if (eventEntity == null) throw new InvalidOperationException("Invalid event id");
            return eventEntity;
        }

        public async Task<IEnumerable<EventDto>> GetEventsAsync()
        {
            var events = await _dbContext.Events
                .AsNoTracking()
                .Select(x => new EventDto
                {
                    Title = x.Title,
                    Category = x.Category.Name,
                    CategoryId = x.CategoryId,
                    ImagePath = x.ImagePath,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    MaxParticipants = x.MaxParticipants,
                    CityId = x.Location.Id,
                    City = x.Location.City,
                    ParticipantsCount = x.EventParticipants.Count()
                })
                .OrderBy(x=>x.Title)
                .ThenByDescending(x=>x.ParticipantsCount)
                .ToListAsync();

            return events;
        }
    }
   }
