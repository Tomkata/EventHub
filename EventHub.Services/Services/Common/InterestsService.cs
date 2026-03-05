



namespace EventHub.Services.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using EventHub.Core.Common;
    using EventHub.Repositories.Interfaces.Common;
    using EventHub.Services.Interfaces.Common;
    using Microsoft.EntityFrameworkCore;
    public class InterestsService : IInterestsService
    {
        private readonly IInterestRepository _repository;
        private readonly IMapper _mapper;

        public InterestsService(IInterestRepository repository,
                                IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }
        public async Task<List<DropdownOptionModel>> GetInterestsForDropDownAsync(CancellationToken cancellation)
    => await _repository.GetAll()
        .ProjectTo<DropdownOptionModel>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellation);
    }
}
