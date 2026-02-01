using EventHub.Core.DTOs;
using EventHub.Core.Models;
using EventHub.Infrastructure.Data;
using EventHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Xml.Schema;

namespace EventHub.Repositories.Repositories
{
    public class EventParticipantsRepository : IEventParticipantsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EventParticipantsRepository(ApplicationDbContext dbContext)
        {
           this._dbContext = dbContext;
        }

        public async Task<UserBasicInfo?> GetOrganizerUserNameAsync(string organizerId)
        {
            var organizer = await _dbContext.Users
                .AsNoTracking()
                .Select(x => new UserBasicInfo
                {
                    Id = x.Id,
                    UserName = x.UserName
                })
                .FirstOrDefaultAsync(x => x.Id == organizerId);

            return organizer;
        }

        public async Task<IEnumerable<UserBasicInfo>> GetParticipantsAsync(Guid id)
        {

            var participants =  _dbContext.EventParticipants
               .Where(ep => ep.EventId == id)
             .Join(
                 _dbContext.Users,
                 ep => ep.UserId,
                 u => u.Id,
                 (ep, u) => new UserBasicInfo
                 {
                     Id = u.Id,
                     UserName = u.UserName
                 }
             )
             .ToList();

            return participants;
        }
    }
}
