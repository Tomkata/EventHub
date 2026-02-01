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
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.EntityFrameworkCore;

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventParticipantsRepository _participantsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILocationRepository  _locationRepository;
        public EventService(IEventRepository eventRepository,
                            IEventParticipantsRepository participantsRepository,
                            ICategoryRepository categoryRepository,
                            ILocationRepository locationRepository)
        {
            this._eventRepository = eventRepository;
            this._participantsRepository = participantsRepository;
            this._categoryRepository = categoryRepository;
            this._locationRepository = locationRepository;
        }


        public async Task<DetailedEventDto> GetByIdAsync(Guid id)
        {

            var entity = await _eventRepository.GetByIdReadOnlyAsync(id);

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

        public async Task<IEnumerable<EventDto>> GetEventsAsync()
        {
            var events =  await _eventRepository.GetAllAsync();

            var dtos =   events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    MaxParticipants = e.MaxParticipants,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    CityId = e.LocationId,
                    City = e.Location.City,
                    CategoryId = e.CategoryId,
                    Category = e.Category.Name,
                    ParticipantsCount = e.EventParticipants.Count(),
                    ImagePath = e.ImagePath
                })
                .ToList();

            return dtos;
        }

        public async Task CreateAsync(CreateEventDto dto)
        {
            if (!await CategoryExistsAsync(dto.CategoryId))
                throw new InvalidCategoryException();
            if (!await LocationExistsAsync(dto.LocationId))
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

            await _eventRepository.AddAsync(eventEntity);
        }

        private async Task<bool> IsOrganizerExistAsync(string Id)=>
            await _participantsRepository.GetOrganizerAsync(Id) == null ? false : true;

        public async Task UpdateAsync(Guid id, EditEventDto dto)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);

            if (eventEntity == null)
                throw new InvalidEventException();

            if (!await CategoryExistsAsync(dto.CategoryId))
                throw new InvalidCategoryException();

            if (!await LocationExistsAsync(dto.LocationId))
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

            await _eventRepository.UpdateAsync(eventEntity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);

            if (eventEntity == null)
                throw new InvalidEventException();

            await _eventRepository.RemoveAsync(eventEntity);
        }

        private async Task<bool> LocationExistsAsync(Guid Id) =>
            await _locationRepository.GetByIdAsync(Id) != null ? true : false;

        private async Task<bool> CategoryExistsAsync(Guid Id)=>
             await _categoryRepository.GetByIdAsync(Id) != null ? true : false;



    }
}
