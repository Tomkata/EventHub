namespace EventHub.Services.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Exceptions.Category;
    using EventHub.Core.Exceptions.Event;
    using EventHub.Core.Exceptions.Location;
    using EventHub.Core.Exceptions.Oranizer;
    using EventHub.Core.Exceptions.User;
    using EventHub.Core.Exceptions.UserProfile;
    using EventHub.Core.Models;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventParticipantsRepository _participantsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILocationRepository  _locationRepository;
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository _userProfileRepository;

        public EventService(IEventRepository eventRepository,
                            IEventParticipantsRepository participantsRepository,
                            ICategoryRepository categoryRepository,
                            ILocationRepository locationRepository,
                            IMapper mapper,
                            IUserProfileRepository userProfileRepository)
        {
            this._eventRepository = eventRepository;
            this._participantsRepository = participantsRepository;
            this._categoryRepository = categoryRepository;
            this._locationRepository = locationRepository;
            this._mapper = mapper;
            this._userProfileRepository = userProfileRepository;
        }

        public async Task<DetailedEventDto> GetByIdAsync(Guid id)
        {
            var dto = await _eventRepository.GetByIdReadOnlyAsync(id);

            if (dto == null)
                throw new InvalidEventException();

            return dto;
        }

        public async Task<PagedResult<EventDto>> GetEventsAsync(int pageNumber,int pageSize)
        {
            return await _eventRepository
                 .GetAll()
                 .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                 .ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task CreateAsync(CreateEventDto dto,string requestingUserId)
        {
            if (!await CategoryExistsAsync(dto.CategoryId))
                throw new InvalidCategoryException();
            if (!await LocationExistsAsync(dto.LocationId))
                throw new InvalidLocationException();

            if (!await UserExistsAsync(requestingUserId))
                throw new InvalidOrganizerException();

            if (!await _userProfileRepository.ExistsAsync(requestingUserId))
                throw new ProfileNotFoundException();
            

            var eventEntity = _mapper.Map<Event>(dto);

            eventEntity.OrganizerId = requestingUserId;

            await _eventRepository.AddAsync(eventEntity);
            await _eventRepository.SaveChangesAsync();
        }

        private async Task<bool> UserExistsAsync(string Id)=>
            await _participantsRepository.UserExistsAsync(Id) == null ? false : true;

        public async Task UpdateAsync(Guid id, EditEventDto dto,string requestingUserId, bool isAdmin)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);

            if (eventEntity == null)
                throw new InvalidEventException();

            if (!await CategoryExistsAsync(dto.CategoryId))
                throw new InvalidCategoryException();

            if (!await LocationExistsAsync(dto.LocationId))
                throw new InvalidLocationException();

         ValidateUserCanModifyEvent(isAdmin, eventEntity.OrganizerId, requestingUserId);
            
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

            await _eventRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid eventId,string requestingUserId, bool isAdmin)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);

            if (eventEntity == null)
                throw new InvalidEventException();

            ValidateUserCanModifyEvent(isAdmin, eventEntity.OrganizerId, requestingUserId);



            await _eventRepository.RemoveAsync(eventEntity);
            await _eventRepository.SaveChangesAsync();
        }

        private async Task<bool> LocationExistsAsync(Guid Id) =>
            await _locationRepository.GetByIdAsync(Id) != null ? true : false;

        private async Task<bool> CategoryExistsAsync(Guid Id)=>
             await _categoryRepository.GetByIdAsync(Id) != null ? true : false;

        public async Task<IEnumerable<EventDto>> GetEventsByOrganizerIdAsync(string organizerId)
        {
            return await _eventRepository.GetByOrganizerId(organizerId)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private void ValidateUserCanModifyEvent(bool isAdmin,string organizerId, string requestingUserId)
        {
            if (!isAdmin)
            {
                if (organizerId != requestingUserId)
                    throw new InvalidUserPermissionsException();
            }
        }

        public async Task<EditEventDto> GetForEditAsync(Guid id)
        {
            var entity = await _eventRepository.GetByIdAsync(id);

            if (entity == null)
                throw new InvalidEventException();

            return _mapper.Map<EditEventDto>(entity);
        }

        public async Task<PagedResult<EventDto>> SearchBy(string? Tite,
            DateTime? StartDate, 
            DateTime? EndDate,
            Guid? LocationId, 
            Guid? CategoryId,
            int pageNumber,
            int pageSize)
        {
            var query = _eventRepository.Query();
            query = ApplyFilters(Tite, StartDate, EndDate, LocationId, CategoryId, query);

            return await query
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    ImagePath = e.ImagePath,
                    City = e.Location.City,
                    Category = e.Category.Name,
                    ParticipantsCount = e.EventParticipants.Count(),
                    MaxParticipants = e.MaxParticipants
                })
                .ToPagedResultAsync(pageNumber,pageSize);
        }

        private static IQueryable<Event> ApplyFilters(string? Tite, DateTime? StartDate, DateTime? EndDate, Guid? LocationId, Guid? CategoryId, IQueryable<Event?> query)
        {
            if (!string.IsNullOrEmpty(Tite))
                query = query.Where(x => x.Title.Contains(Tite.Trim()));

            if (StartDate.HasValue)
                query = query.Where(x => x.StartDate >= StartDate.Value);

            if (EndDate.HasValue)
                query = query.Where(x => x.EndDate <= EndDate.Value);

            if (LocationId.HasValue)
                query = query.Where(x => x.LocationId == LocationId.Value);

            if (CategoryId.HasValue)
                query = query.Where(x => x.CategoryId == CategoryId.Value);

            return query;
        }
    }
}
