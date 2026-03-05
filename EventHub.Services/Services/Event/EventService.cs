namespace EventHub.Services.Services.Event
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Exceptions.Category;
    using EventHub.Core.Exceptions.Event;
    using EventHub.Core.Exceptions.Location;
    using EventHub.Core.Exceptions.User;
    using EventHub.Core.Exceptions.UserProfile;
    using EventHub.Core.Models.Events;
    using EventHub.Infrastructure.QueryExtensions;
    using EventHub.Repositories.Interfaces.Common;
    using EventHub.Repositories.Interfaces.Events;
    using EventHub.Repositories.Interfaces.User;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces.Event;

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventParticipantsRepository _participantsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILocationRepository _locationRepository;
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

        public async Task<DetailedEventDto> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            var dto = await _eventRepository.GetByIdReadOnlyAsync(id, cancellation);

            if (dto == null)
                throw new EventNotFoundException();

            return dto;
        }

        public async Task<PagedResult<EventDto>> GetEventsAsync(int pageNumber, int pageSize, CancellationToken cancellation)
        {
            return await _eventRepository
                 .GetAll()
                 .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                 .OrderBy(x => x.StartDate)
                 .ToPagedResultAsync(pageNumber, pageSize, cancellation);
        }

        public async Task CreateAsync(
            CreateEventDto dto,
            string requestingUserId,
            CancellationToken cancellation)
        {
            if (!await CategoryExistsAsync(dto.CategoryId, cancellation))
                throw new InvalidCategoryException();
            if (!await LocationExistsAsync(dto.LocationId, cancellation))
                throw new InvalidLocationException();

            if (!await UserExistsAsync(requestingUserId, cancellation))
                throw new UserNotFoundException();

            if (!await _userProfileRepository.ExistsAsync(requestingUserId, cancellation))
                throw new ProfileNotFoundException();


            var eventEntity = _mapper.Map<Event>(dto);

            eventEntity.OrganizerId = requestingUserId;

            await _eventRepository.AddAsync(eventEntity, cancellation);
            await _eventRepository.SaveChangesAsync(cancellation);
        }

        private async Task<bool> UserExistsAsync(string Id, CancellationToken cancellation) =>
            await _participantsRepository.UserExistsAsync(Id, cancellation) == null ? false : true;

        public async Task UpdateAsync(
            Guid id,
            EditEventDto dto,
            string requestingUserId,
            bool isAdmin,
            CancellationToken cancellation)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id, cancellation);

            if (eventEntity == null)
                throw new EventNotFoundException();

            if (!await CategoryExistsAsync(dto.CategoryId, cancellation))
                throw new InvalidCategoryException();

            if (!await LocationExistsAsync(dto.LocationId, cancellation))
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

            await _eventRepository.SaveChangesAsync(cancellation);
        }

        public async Task DeleteAsync(Guid eventId, string requestingUserId, bool isAdmin, CancellationToken cancellation)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId, cancellation);

            if (eventEntity == null)
                throw new EventNotFoundException();

            ValidateUserCanModifyEvent(isAdmin, eventEntity.OrganizerId, requestingUserId);



             _eventRepository.Remove(eventEntity);
            await _eventRepository.SaveChangesAsync(cancellation);
        }

        private async Task<bool> LocationExistsAsync(Guid Id, CancellationToken cancellation) =>
            await _locationRepository.GetByIdAsync(Id, cancellation) != null ? true : false;

        private async Task<bool> CategoryExistsAsync(Guid Id, CancellationToken cancellation) =>
             await _categoryRepository.GetByIdAsync(Id, cancellation) != null ? true : false;

        public async Task<PagedResult<EventDto>> GetEventsByOrganizerIdAsync(
             string organizerId,
             int pageNumber,
             int pageSize,
             CancellationToken cancellationToken)
        {
            return await _eventRepository.GetByOrganizerId(organizerId)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .OrderBy(x => x.StartDate)
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        private void ValidateUserCanModifyEvent(bool isAdmin, string organizerId, string requestingUserId)
        {
            if (!isAdmin)
            {
                if (organizerId != requestingUserId)
                    throw new ForbiddenOperationException();
            }
        }

        public async Task<EditEventDto> GetForEditAsync(Guid id, CancellationToken cancellation)
        {
            var entity = await _eventRepository.GetByIdAsync(id, cancellation);

            if (entity == null)
                throw new EventNotFoundException();

            return _mapper.Map<EditEventDto>(entity);
        }

        public async Task<PagedResult<EventDto>> SearchBy(string? title,
            DateTime? startDate,
            DateTime? endDate,
            Guid? locationId,
            Guid? categoryId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _eventRepository
                .Query()
                .FilterByTitle(title)
                .FilterByDate(startDate, endDate)
                .FilterByCategory(categoryId)
                .FilterByLocation(locationId);

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
                .OrderBy(x => x.StartDate)
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
