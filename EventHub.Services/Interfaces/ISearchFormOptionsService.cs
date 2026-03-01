


namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs;

    public interface ISearchFormOptionsService
    {
        Task<EventFormOptionsDto> GetFormOptionsAsync(CancellationToken cancellationToken);
    }
}
