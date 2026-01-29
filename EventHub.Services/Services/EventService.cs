


namespace EventHub.Services.Services
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Exceptions.Category;
    using EventHub.Core.Exceptions.Event;
    using EventHub.Core.Exceptions.Location;
    using EventHub.Core.Exceptions.User;
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Client;

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
                            .Select(x => new
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


            if (eventEntity == null)
                throw new InvalidEventException();

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

            if(organizer == null) throw new InvalidOrganizerException();

            var dto = new DetailedEventDto
            {
                  
                 Id = eventEntity.Id,
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
            if (!await IsCategoryIdExistAsync(dto.CategoryId))
                throw new InvalidCategoryException();

            if (!await IsLocationIdExistAsync(dto.LocationId))
                throw new InvalidLocationException();


            if (!await IsOrganizerExistAsync(dto.OrganizerId))
                throw new InvalidOrganizerException();

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

        private async Task<bool> IsOrganizerExistAsync(string Id)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == Id);
        }

        public async Task UpdateAsync(Guid id, EditEventDto dto)
        {   
            var eventEntity = await GetEventEntityOrThrowAsync(id);


            if (!await IsCategoryIdExistAsync(dto.CategoryId))
                throw new InvalidCategoryException();

            if (!await IsLocationIdExistAsync(dto.LocationId))
                throw new InvalidLocationException();



            eventEntity.Title = dto.Title;
            eventEntity.LocationId = dto.LocationId;
            eventEntity.CategoryId = dto.CategoryId;
            eventEntity.StartDate = dto.StartDate;
            eventEntity.EndDate = dto.EndDate;
            eventEntity.MaxParticipants = dto.MaxParticipants;
            eventEntity.Description = dto.Description;
            eventEntity.Address = dto.Address;


            if (dto.ImagePath != null)
            {
                eventEntity.ImagePath = dto.ImagePath;
            }

            await _dbContext.SaveChangesAsync();

        }

       
        public async Task DeleteAsync(Guid id)
        {
            var eventEntity = await GetEventEntityOrThrowAsync(id);

             _dbContext.Events.Remove(eventEntity);
            await _dbContext.SaveChangesAsync();
        }



        private async Task<Event> GetEventEntityOrThrowAsync(Guid id)
        {
            var eventEntity = await _dbContext.Events
                .Include(x=>x.Location)
                .Include(x=>x.Category)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (eventEntity == null)
                throw new InvalidEventException();

            return eventEntity;
        }

        public async Task<IEnumerable<EventDto>> GetEventsAsync()
        {
            var events = await _dbContext.Events
                .AsNoTracking()
                .Select(x => new EventDto
                {
                     Id = x.Id,
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

        private async Task<bool> IsLocationIdExistAsync(Guid Id)
        {
            return await _dbContext.Locations.AnyAsync(x => x.Id == Id);
        }

        private async Task<bool> IsCategoryIdExistAsync(Guid Id)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .AnyAsync(x => x.Id == Id);
        }
    }
   }
