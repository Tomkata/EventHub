

namespace EventHub.Services.Interfaces.User
{
    using EventHub.Core.DTOs.UserProfile;
    public interface IUserProfileFormOptionsService
    {
        public Task<UserProfileOptionDto> GetFormOptionsAsync(CancellationToken cancellationToken);

    }
}
