


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
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.DataProtection.XmlEncryption;
    using Microsoft.EntityFrameworkCore;

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventParticipantsRepository _participantsRepository;
        public EventService(IEventRepository eventRepository,
                            IEventParticipantsRepository participantsRepository)
        {
            this._eventRepository = eventRepository;
            this._participantsRepository = participantsRepository;
        }


        public async Task<DetailedEventDto> GetByIdAsync(Guid id)
        {

            var entity = await _eventRepository.GetByIdAsync(id);


            if (entity == null)
                throw new InvalidEventException();

            var participants = await _participantsRepository.GetParticipantsAsync(entity.Id);

            var participantsDto = participants
                .Select(x => new ParticipantDto
                {
                    UserId = x.Id,
                    UserName = x.UserName
                })
                .ToList();


            var organizer = await _participantsRepository.GetOrganizerAsync(entity.OrganizerId);

            if (organizer == null) throw new InvalidOrganizerException();

            var dto = new DetailedEventDto
            {

                Id = entity.Id,
                Title = entity.Title,
                CategoryName = entity.Category.Name,
                MaxParticipants = entity.MaxParticipants,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                OrganizerName = organizer.UserName,
                City = entity.Location.City,
                Address = entity.Address,
                ImagePath = entity.ImagePath,
                ParticipantList = participantsDto,
                CategoryId = entity.CategoryId,
                LocationId = entity.Location.Id
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
                    OrganizerId = dto.OrganizerId,
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
                eventEntity.ImagePath = dto.ImagePath;

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
                .Include(x => x.Location)
                .Include(x => x.Category)
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
                .OrderBy(x => x.Title)
                .ThenByDescending(x=>x.StartDate)
                .ThenByDescending(x => x.ParticipantsCount)
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
