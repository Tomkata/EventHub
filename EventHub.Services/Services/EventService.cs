

namespace EventHub.Services.Services
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Data.Identity;
    using EventHub.Services.DTOs.ApplicationUser;
    using EventHub.Services.DTOs.Event;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using System.Security.Cryptography.X509Certificates;

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
                                x.Title,
                                Category = x.Category.Name,
                                x.MaxParticipants,
                                x.Description,
                                x.EventDate,
                                Address = x.Location.Address,
                                City = x.Location.City,
                                Country = x.Location.Country,
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
                 EventDate = eventEntity.EventDate,
                 OrganizerName = organizer.UserName,
                 City = eventEntity.City,
                 Country = eventEntity.Country,
                 Address = eventEntity.Address,
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
                EventDate = dto.EventDate,
                MaxParticipants = dto.MaxParticipants,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                LocationId = dto.LocationId
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
            eventEntity.EventDate = dto.EventDate;
            eventEntity.MaxParticipants = dto.MaxParticipants;
            eventEntity.Description = dto.Description;

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
    }
   }
