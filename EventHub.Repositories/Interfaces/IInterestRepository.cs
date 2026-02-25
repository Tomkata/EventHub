
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models;

    public interface IInterestRepository
    {
        IQueryable<Interest> GetAll();
    }
}
