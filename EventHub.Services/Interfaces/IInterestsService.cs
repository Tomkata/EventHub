
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.Common;
    using EventHub.Core.DTOs.Interest;

    public interface IInterestsService
    {
        public Task<List<DropdownOptionModel>> GetInterestsForDropDownAsync(CancellationToken cancellationToken);
    }
}
