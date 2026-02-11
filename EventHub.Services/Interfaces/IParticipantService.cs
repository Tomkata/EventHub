namespace EventHub.Services.Interfaces
{
    public interface IParticipantService
    {
        Task JoinEventAsync(string userId, Guid eventId);

        Task LeftEventAsync(string userId, Guid eventId);

    }
}
