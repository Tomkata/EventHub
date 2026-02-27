


namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs;

    public interface ISearachFormOptionsService
    {
        Task<EventFormOptionsDto> GetFormOptionsAsync();
    }
}
