


namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs;

    public interface ISerachFormOptionsService
    {
        Task<EventFormOptionsDto> GetFormOptionsAsync();
    }
}
