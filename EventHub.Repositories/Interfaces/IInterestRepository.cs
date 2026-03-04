
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models.Common;

    public interface IInterestRepository
    {
        IQueryable<Interest> GetAll();
    }
}
