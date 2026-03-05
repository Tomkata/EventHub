namespace EventHub.Repositories.Interfaces.Common
{
    using EventHub.Core.Models.Common;

    public interface IInterestRepository
    {
        IQueryable<Interest> GetAll();
    }
}
