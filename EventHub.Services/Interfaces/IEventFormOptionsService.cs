using EventHub.Core.DTOs;
using EventHub.Core.Models;
using EventHub.Services.Services;

namespace EventHub.Services.Interfaces

{
    public interface IEventFormOptionsService
    {
        public Task<EventFormOptionsDto> GetFormOptionsAsync();
    }
}
    