


namespace EventHub.Repositories.Repositories
{
    using EventHub.Core.Models.Common;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class InterestRepository : IInterestRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public InterestRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public IQueryable<Interest> GetAll()
        => _dbContext.Interests
            .OrderBy(x=>x.InterestName)
            .AsNoTracking();
    }
}
